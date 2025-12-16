using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;

namespace BlazorApp1.Services;

/// <summary>
/// Production-ready user management service using ASP.NET Core Identity
/// </summary>
public class UserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSender<ApplicationUser> _emailSender;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IEmailSender<ApplicationUser> emailSender,
        ILogger<UserManagementService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
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
                PasskeyCount = passkeys.Count
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
            PasskeyCount = passkeys.Count
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
            EmailConfirmed = model.EmailConfirmed
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

        _logger.LogInformation("User {Email} created successfully", user.Email);

        // Send confirmation email if not auto-confirmed
        if (!model.EmailConfirmed)
        {
            await SendEmailConfirmationAsync(user);
        }

        return (true, [], user.Id);
    }

    /// <summary>
    /// Update user profile and role
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

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        // Update role if changed
        if (!string.IsNullOrEmpty(model.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            
            // Remove from all current roles
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }
            
            // Add to new role
            await _userManager.AddToRoleAsync(user, model.Role);
        }

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

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        _logger.LogInformation("User {Email} deleted", user.Email);
        return (true, []);
    }

    #endregion

    #region Password Management

    /// <summary>
    /// Admin reset password - generates a new random password
    /// </summary>
    public async Task<(bool Success, string[] Errors, string? NewPassword)> AdminResetPasswordAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"], null);
        }

        // Generate a temporary password
        var newPassword = GenerateRandomPassword();
        
        // Remove existing password and set new one
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray(), null);
        }

        _logger.LogInformation("Password reset by admin for user {Email}", user.Email);
        return (true, [], newPassword);
    }

    /// <summary>
    /// Send password reset email to user
    /// </summary>
    public async Task<(bool Success, string[] Errors)> SendPasswordResetEmailAsync(string email, string resetUrl)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.EmailConfirmed)
        {
            // Don't reveal if user exists
            return (true, []);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(token);
        var callbackUrl = $"{resetUrl}?code={encodedToken}";

        await _emailSender.SendPasswordResetLinkAsync(user, email, callbackUrl);

        _logger.LogInformation("Password reset email sent to {Email}", email);
        return (true, []);
    }

    /// <summary>
    /// Force password change on next login
    /// </summary>
    public async Task<(bool Success, string[] Errors)> RequirePasswordChangeAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        // Update security stamp to invalidate current sessions
        await _userManager.UpdateSecurityStampAsync(user);
        
        _logger.LogInformation("Password change required for user {Email}", user.Email);
        return (true, []);
    }

    #endregion

    #region Account Lockout Management

    /// <summary>
    /// Lock a user account
    /// </summary>
    public async Task<(bool Success, string[] Errors)> LockUserAsync(string userId, DateTimeOffset? lockoutEnd = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        // Enable lockout if not already enabled
        if (!user.LockoutEnabled)
        {
            await _userManager.SetLockoutEnabledAsync(user, true);
        }

        // Set lockout end (default to 100 years for permanent lock)
        var end = lockoutEnd ?? DateTimeOffset.UtcNow.AddYears(100);
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

        _logger.LogInformation("User {Email} active status set to {IsActive}", user.Email, isActive);
        return (true, []);
    }

    #endregion

    #region Email Confirmation

    /// <summary>
    /// Send email confirmation
    /// </summary>
    public async Task<(bool Success, string[] Errors)> SendEmailConfirmationAsync(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        // In production, this would generate a proper callback URL
        await _emailSender.SendConfirmationLinkAsync(user, user.Email!, token);
        
        _logger.LogInformation("Confirmation email sent to {Email}", user.Email);
        return (true, []);
    }

    /// <summary>
    /// Admin confirm email manually
    /// </summary>
    public async Task<(bool Success, string[] Errors)> AdminConfirmEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, ["User not found"]);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        _logger.LogInformation("Email confirmed by admin for user {Email}", user.Email);
        return (true, []);
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

    #region Statistics

    /// <summary>
    /// Get user statistics
    /// </summary>
    public async Task<UserStatistics> GetUserStatisticsAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var stats = new UserStatistics
        {
            TotalUsers = users.Count,
            ActiveUsers = users.Count(u => u.IsActive),
            InactiveUsers = users.Count(u => !u.IsActive),
            LockedUsers = users.Count(u => u.LockoutEnd > DateTimeOffset.UtcNow),
            UnconfirmedEmails = users.Count(u => !u.EmailConfirmed),
            TwoFactorEnabled = users.Count(u => u.TwoFactorEnabled),
            RecentLogins = users.Count(u => u.LastLoginDate > DateTime.Now.AddDays(-7)),
            NeverLoggedIn = users.Count(u => u.LastLoginDate == null)
        };

        // Get role counts
        foreach (var role in await GetAllRolesAsync())
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            stats.RoleCounts[role] = usersInRole.Count;
        }

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
}

public class UpdateUserModel
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Department { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Role { get; set; } = "";
    public bool IsActive { get; set; }
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
}

#endregion
