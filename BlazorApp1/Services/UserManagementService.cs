using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;
using BlazorApp1.Models;

namespace BlazorApp1.Services;

/// <summary>
/// Production-ready user management service using ASP.NET Core Identity.
/// Automatically syncs changes to the legacy Users table for backward compatibility.
/// </summary>
public class UserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IEmailSender<ApplicationUser> _emailSender;
    private readonly RolePermissionService _rolePermissionService;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IDbContextFactory<ApplicationDbContext> contextFactory,
        IEmailSender<ApplicationUser> emailSender,
        RolePermissionService rolePermissionService,
        ILogger<UserManagementService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _contextFactory = contextFactory;
        _emailSender = emailSender;
        _rolePermissionService = rolePermissionService;
        _logger = logger;
    }

    #region User CRUD Operations

    /// <summary>
    /// Get all users with their roles
    /// </summary>
    public async Task<List<UserViewModel>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userViewModels = new List<UserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var passkeys = await _userManager.GetPasskeysAsync(user);
            
            userViewModels.Add(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                FullName = user.FullName ?? "",
                Department = user.Department ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                Roles = roles.ToList(),
                HasPasskeys = passkeys.Count > 0,
                PasskeyCount = passkeys.Count,
                TenantId = user.PrimaryTenantId
            });
        }

        return userViewModels.OrderBy(u => u.FullName).ToList();
    }

    /// <summary>
    /// Get a single user by ID
    /// </summary>
    public async Task<UserViewModel?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var passkeys = await _userManager.GetPasskeysAsync(user);

        return new UserViewModel
        {
            Id = user.Id,
            Email = user.Email ?? "",
            UserName = user.UserName ?? "",
            FullName = user.FullName ?? "",
            Department = user.Department ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd,
            AccessFailedCount = user.AccessFailedCount,
            CreatedDate = user.CreatedDate,
            LastLoginDate = user.LastLoginDate,
            Roles = roles.ToList(),
            HasPasskeys = passkeys.Count > 0,
            PasskeyCount = passkeys.Count,
            TenantId = user.PrimaryTenantId
        };
    }

    /// <summary>
    /// Create a new user with specified role
    /// </summary>
    public async Task<(bool Success, string[] Errors, string? UserId)> CreateUserAsync(CreateUserModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Department = model.Department,
            PhoneNumber = model.PhoneNumber,
            IsActive = true,
            CreatedDate = DateTime.Now,
            EmailConfirmed = model.EmailConfirmed,
            PrimaryTenantId = model.TenantId
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray(), null);
        }

        // Add to role
        if (!string.IsNullOrEmpty(model.Role))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                _logger.LogWarning("Failed to add user {Email} to role {Role}", user.Email, model.Role);
            }
        }

        // Sync to legacy Users table
        await SyncToLegacyUsersTableAsync(user, model.Role ?? "Technician");

        _logger.LogInformation("User {Email} created successfully with TenantId {TenantId}", user.Email, model.TenantId);

        // Send confirmation email if not auto-confirmed
        if (!model.EmailConfirmed)
        {
            await SendEmailConfirmationAsync(user);
        }

        return (true, [], user.Id);
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    public async Task<(bool Success, string[] Errors)> UpdateUserAsync(UpdateUserModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        user.FullName = model.FullName;
        user.Department = model.Department;
        user.PhoneNumber = model.PhoneNumber;
        user.IsActive = model.IsActive;
        
        if (model.TenantId.HasValue)
        {
            user.PrimaryTenantId = model.TenantId;
        }

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        // Update role if changed
        if (!string.IsNullOrEmpty(model.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(model.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }
        }

        // Sync to legacy Users table
        await SyncToLegacyUsersTableAsync(user, model.Role ?? "Technician");

        _logger.LogInformation("User {Email} updated successfully", user.Email);
        return (true, []);
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    public async Task<(bool Success, string[] Errors)> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        // Remove from legacy Users table first
        await RemoveFromLegacyUsersTableAsync(user.Id);

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        _logger.LogInformation("User {Email} deleted successfully", user.Email);
        return (true, []);
    }

    #endregion

    #region Password Management

    /// <summary>
    /// Reset a user's password with a new generated password
    /// </summary>
    public async Task<(bool Success, string[] Errors, string? NewPassword)> ResetPasswordAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"], null);
        }

        var newPassword = GenerateRandomPassword();
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray(), null);
        }

        _logger.LogInformation("Password reset for user {Email}", user.Email);
        return (true, [], newPassword);
    }

    /// <summary>
    /// Send a password reset email to the user
    /// </summary>
    public async Task<(bool Success, string[] Errors)> SendPasswordResetEmailAsync(string userId, string resetUrl)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = System.Net.WebUtility.UrlEncode(token);
        var fullResetUrl = $"{resetUrl}?userId={userId}&token={encodedToken}";

        try
        {
            await _emailSender.SendPasswordResetLinkAsync(user, user.Email!, fullResetUrl);
            _logger.LogInformation("Password reset email sent to {Email}", user.Email);
            return (true, []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
            return (false, ["Failed to send email"]);
        }
    }

    #endregion

    #region Role Management

    /// <summary>
    /// Get all available roles
    /// </summary>
    public async Task<List<string>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
    }

    /// <summary>
    /// Add user to a role
    /// </summary>
    public async Task<(bool Success, string[] Errors)> AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        await SyncToLegacyUsersTableAsync(user, role);

        _logger.LogInformation("User {Email} added to role {Role}", user.Email, role);
        return (true, []);
    }

    /// <summary>
    /// Remove user from a role
    /// </summary>
    public async Task<(bool Success, string[] Errors)> RemoveFromRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        var result = await _userManager.RemoveFromRoleAsync(user, role);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        _logger.LogInformation("User {Email} removed from role {Role}", user.Email, role);
        return (true, []);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    public async Task<(bool Success, string[] Errors)> CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return (false, ["Role already exists"]);
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        _logger.LogInformation("Role {Role} created", roleName);
        return (true, []);
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    public async Task<(bool Success, string[] Errors)> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return (false, ["Role not found"]);
        }

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        _logger.LogInformation("Role {Role} deleted", roleName);
        return (true, []);
    }

    #endregion

    #region Account Management

    /// <summary>
    /// Lock a user account
    /// </summary>
    public async Task<(bool Success, string[] Errors)> LockUserAsync(string userId, int daysToLock = 365)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        var end = DateTimeOffset.UtcNow.AddDays(daysToLock);
        var result = await _userManager.SetLockoutEndDateAsync(user, end);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        _logger.LogInformation("User {Email} locked until {LockoutEnd}", user.Email, end);
        return (true, []);
    }

    /// <summary>
    /// Unlock a user account
    /// </summary>
    public async Task<(bool Success, string[] Errors)> UnlockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        var result = await _userManager.SetLockoutEndDateAsync(user, null);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        // Reset access failed count
        await _userManager.ResetAccessFailedCountAsync(user);

        _logger.LogInformation("User {Email} unlocked", user.Email);
        return (true, []);
    }

    /// <summary>
    /// Toggle user active status
    /// </summary>
    public async Task<(bool Success, string[] Errors)> SetUserActiveStatusAsync(string userId, bool isActive)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        user.IsActive = isActive;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        // Sync to legacy table
        await SyncToLegacyUsersTableAsync(user, (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Technician");

        _logger.LogInformation("User {Email} active status set to {Status}", user.Email, isActive);
        return (true, []);
    }

    /// <summary>
    /// Send email confirmation to user
    /// </summary>
    public async Task<(bool Success, string[] Errors)> SendEmailConfirmationAsync(ApplicationUser user)
    {
        try
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailSender.SendConfirmationLinkAsync(user, user.Email!, $"/Account/ConfirmEmail?userId={user.Id}&token={System.Net.WebUtility.UrlEncode(token)}");
            return (true, []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            return (false, ["Failed to send email"]);
        }
    }

    #endregion

    #region Legacy Users Table Sync

    /// <summary>
    /// Sync user to the legacy Users table for backward compatibility
    /// </summary>
    private async Task SyncToLegacyUsersTableAsync(ApplicationUser user, string role)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            
            var legacyUser = await context.Users.FirstOrDefaultAsync(u => u.AspNetUserId == user.Id);

            if (legacyUser == null)
            {
                legacyUser = new User
                {
                    Name = user.FullName ?? user.Email ?? "Unknown",
                    Email = user.Email ?? "",
                    Role = role,
                    Department = user.Department ?? "",
                    Phone = user.PhoneNumber ?? "",
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    AspNetUserId = user.Id,
                    TenantId = user.PrimaryTenantId
                };
                context.Users.Add(legacyUser);
            }
            else
            {
                legacyUser.Name = user.FullName ?? user.Email ?? "Unknown";
                legacyUser.Email = user.Email ?? "";
                legacyUser.Role = role;
                legacyUser.Department = user.Department ?? "";
                legacyUser.Phone = user.PhoneNumber ?? "";
                legacyUser.IsActive = user.IsActive;
                legacyUser.TenantId = user.PrimaryTenantId;
            }

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync user {Email} to legacy Users table", user.Email);
        }
    }

    /// <summary>
    /// Remove user from legacy Users table
    /// </summary>
    private async Task RemoveFromLegacyUsersTableAsync(string aspNetUserId)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            
            var legacyUser = await context.Users.FirstOrDefaultAsync(u => u.AspNetUserId == aspNetUserId);
            if (legacyUser != null)
            {
                context.Users.Remove(legacyUser);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove user from legacy Users table");
        }
    }

    #endregion

    #region Statistics

    /// <summary>
    /// Get user statistics (tenant-filtered)
    /// </summary>
    public async Task<UserStatistics> GetUserStatisticsAsync()
    {
        // Get tenant context for filtering
        var isSuperAdmin = await _rolePermissionService.IsSuperAdminAsync();
        var currentTenantId = await _rolePermissionService.GetCurrentTenantIdAsync();
        
        var allUsers = await _userManager.Users.ToListAsync();
        
        // Filter users by tenant if not SuperAdmin
        var users = allUsers;
        if (!isSuperAdmin && currentTenantId.HasValue)
        {
            users = allUsers.Where(u => u.PrimaryTenantId == currentTenantId).ToList();
        }
        
        var stats = new UserStatistics
        {
            TotalUsers = users.Count,
            ActiveUsers = users.Count(u => u.IsActive),
            InactiveUsers = users.Count(u => !u.IsActive),
            LockedUsers = users.Count(u => u.LockoutEnd > DateTimeOffset.UtcNow),
            UnconfirmedEmails = users.Count(u => !u.EmailConfirmed),
            TwoFactorEnabled = users.Count(u => u.TwoFactorEnabled),
            RecentLogins = users.Count(u => u.LastLoginDate > DateTime.Now.AddDays(-7)),
            NeverLoggedIn = users.Count(u => u.LastLoginDate == null),
            TenantId = currentTenantId
        };

        // Get role counts (for filtered users)
        foreach (var role in await GetAllRolesAsync())
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            var count = usersInRole.Count(u => users.Any(filteredUser => filteredUser.Id == u.Id));
            stats.RoleCounts[role] = count;
        }

        _logger.LogDebug("Retrieved user statistics for tenant {TenantId}: {TotalUsers} total, {ActiveUsers} active", 
            currentTenantId, stats.TotalUsers, stats.ActiveUsers);
        
        return stats;
    }

    #endregion

    #region Helpers

    private static string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
        var random = new Random();
        var password = new char[12];
        
        // Ensure at least one of each required type
        password[0] = chars[random.Next(0, 24)]; // Upper
        password[1] = chars[random.Next(24, 48)]; // Lower
        password[2] = chars[random.Next(48, 56)]; // Digit
        password[3] = chars[random.Next(56, 61)]; // Special
        
        for (int i = 4; i < 12; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }
        
        // Shuffle
        return new string(password.OrderBy(x => random.Next()).ToArray());
    }

    #endregion
}

#region View Models

public class UserViewModel
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Department { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public List<string> Roles { get; set; } = [];
    public bool HasPasskeys { get; set; }
    public int PasskeyCount { get; set; }
    public int? TenantId { get; set; }
    public string? TenantName { get; set; }

    public string PrimaryRole => Roles.FirstOrDefault() ?? "No Role";
    public bool IsLocked => LockoutEnd.HasValue && LockoutEnd > DateTimeOffset.UtcNow;
}

public class CreateUserModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Department { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Role { get; set; } = "Technician";
    public bool EmailConfirmed { get; set; } = true;
    public int? TenantId { get; set; }
}

public class UpdateUserModel
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Department { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Role { get; set; } = "";
    public bool IsActive { get; set; }
    public int? TenantId { get; set; }
}

public class UserStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int LockedUsers { get; set; }
    public int UnconfirmedEmails { get; set; }
    public int TwoFactorEnabled { get; set; }
    public int RecentLogins { get; set; }
    public int NeverLoggedIn { get; set; }
    public Dictionary<string, int> RoleCounts { get; set; } = [];
    public int? TenantId { get; set; }
}

#endregion
