using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;
using BlazorApp1.Models;
using System.Security.Claims;
using BlazorApp1.Services;

namespace BlazorApp1.Data;

public static class IdentityDataSeeder
{
    /// <summary>
    /// All available roles in the system (matches RolePermissionService.RoleHierarchy)
    /// </summary>
    public static readonly string[] AllRoles = 
    {
        "SuperAdmin",
        "TenantAdmin",
        "Admin",
        "Reliability Engineer",
        "Planner",
        "Supervisor",
        "Technician",
        "Viewer"
    };

    public static async Task SeedRolesAndUsersAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        // Seed all roles and attach a basic role-level claim for each role
        foreach (var role in AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var createRoleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (createRoleResult.Succeeded)
                {
                    // Add a role-level claim to help with permission checks if needed
                    var createdRole = await roleManager.FindByNameAsync(role);
                    if (createdRole != null)
                    {
                        var level = RolePermissionService.GetRoleLevel(role);
                        // Add or ensure the claim exists
                        var claim = new Claim("RoleLevel", level.ToString());
                        await roleManager.AddClaimAsync(createdRole, claim);
                    }
                }
            }
            else
            {
                // Ensure role has RoleLevel claim (in case it was created in a previous run without claims)
                var existingRole = await roleManager.FindByNameAsync(role);
                if (existingRole != null)
                {
                    var claims = await roleManager.GetClaimsAsync(existingRole);
                    if (!claims.Any(c => c.Type == "RoleLevel"))
                    {
                        var level = RolePermissionService.GetRoleLevel(role);
                        await roleManager.AddClaimAsync(existingRole, new Claim("RoleLevel", level.ToString()));
                    }
                }
            }
        }

        // Seed Super Admin User
        await CreateAndSyncUserAsync(
            userManager, context,
            email: "superadmin@company.com",
            password: "SuperAdmin123!",
            fullName: "Super Administrator",
            department: "Administration",
            phone: "555-0000",
            role: "SuperAdmin",
            isSuperAdmin: true);

        // Seed Admin User
        await CreateAndSyncUserAsync(
            userManager, context,
            email: "admin@company.com",
            password: "Admin123!",
            fullName: "Admin User",
            department: "Management",
            phone: "555-0001",
            role: "Admin");

        // Seed Reliability Engineer
        await CreateAndSyncUserAsync(
            userManager, context,
            email: "sarah.johnson@company.com",
            password: "Sarah123!",
            fullName: "Sarah Johnson",
            department: "Engineering",
            phone: "555-0002",
            role: "Reliability Engineer");

        // Seed Planner
        await CreateAndSyncUserAsync(
            userManager, context,
            email: "emily.brown@company.com",
            password: "Emily123!",
            fullName: "Emily Brown",
            department: "Planning",
            phone: "555-0003",
            role: "Planner");

        // Seed Supervisor
        await CreateAndSyncUserAsync(
            userManager, context,
            email: "david.wilson@company.com",
            password: "David123!",
            fullName: "David Wilson",
            department: "Maintenance",
            phone: "555-0006",
            role: "Supervisor");

        // Seed Technicians
        await CreateAndSyncUserAsync(
            userManager, context,
            email: "john.smith@company.com",
            password: "John123!",
            fullName: "John Smith",
            department: "Maintenance",
            phone: "555-0004",
            role: "Technician");

        await CreateAndSyncUserAsync(
            userManager, context,
            email: "mike.davis@company.com",
            password: "Mike123!",
            fullName: "Mike Davis",
            department: "Maintenance",
            phone: "555-0005",
            role: "Technician");

        // Seed Viewer (read-only user)
        await CreateAndSyncUserAsync(
            userManager, context,
            email: "viewer@company.com",
            password: "Viewer123!",
            fullName: "Read Only User",
            department: "Quality",
            phone: "555-0007",
            role: "Viewer");

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates an Identity user and syncs to the legacy Users table
    /// </summary>
    private static async Task CreateAndSyncUserAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        string email,
        string password,
        string fullName,
        string department,
        string phone,
        string role,
        bool isSuperAdmin = false)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        
        if (existingUser == null)
        {
            // Create Identity user
            var identityUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                Department = department,
                PhoneNumber = phone,
                IsActive = true,
                IsSuperAdmin = isSuperAdmin,
                PrimaryTenantId = null,
                CreatedDate = DateTime.Now
            };

            var result = await userManager.CreateAsync(identityUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(identityUser, role);

                // Add common claims to the created user so roles/claims appear in AspNetUserClaims
                var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, identityUser.FullName ?? identityUser.Email ?? ""),
                    new Claim(ClaimTypes.Email, identityUser.Email ?? ""),
                    new Claim("FullName", identityUser.FullName ?? ""),
                    new Claim(ClaimTypes.MobilePhone, phone ?? "")
                };

                // Only add claims that don't already exist
                var existingClaims = await userManager.GetClaimsAsync(identityUser);
                var claimsToAdd = userClaims.Where(uc => !existingClaims.Any(ec => ec.Type == uc.Type && ec.Value == uc.Value)).ToList();
                if (claimsToAdd.Count > 0)
                {
                    await userManager.AddClaimsAsync(identityUser, claimsToAdd);
                }

                // Sync to legacy Users table
                await SyncToLegacyUsersTable(context, identityUser, role, phone);
            }
        }
        else
        {
            // User exists in Identity - ensure they're synced to legacy table
            await SyncToLegacyUsersTable(context, existingUser, role, phone);

            // Ensure existing Identity user has basic claims persisted
            var existingClaims = await userManager.GetClaimsAsync(existingUser);
            var required = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.FullName ?? existingUser.Email ?? ""),
                new Claim(ClaimTypes.Email, existingUser.Email ?? ""),
                new Claim("FullName", existingUser.FullName ?? ""),
                new Claim(ClaimTypes.MobilePhone, phone ?? "")
            };

            var toAdd = required.Where(rc => !existingClaims.Any(ec => ec.Type == rc.Type && ec.Value == rc.Value)).ToList();
            if (toAdd.Count > 0)
            {
                await userManager.AddClaimsAsync(existingUser, toAdd);
            }
        }
    }

    /// <summary>
    /// Syncs an Identity user to the legacy Users table
    /// </summary>
    private static async Task SyncToLegacyUsersTable(
        ApplicationDbContext context,
        ApplicationUser identityUser,
        string role,
        string? phone)
    {
        // Check if already exists in legacy table
        var legacyUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email == identityUser.Email || u.AspNetUserId == identityUser.Id);

        if (legacyUser == null)
        {
            // Create new legacy user
            legacyUser = new User
            {
                Name = identityUser.FullName ?? identityUser.Email ?? "Unknown",
                Email = identityUser.Email ?? "",
                Role = role,
                Department = identityUser.Department ?? "",
                Phone = phone ?? "",
                IsActive = identityUser.IsActive,
                CreatedDate = identityUser.CreatedDate,
                AspNetUserId = identityUser.Id,
                TenantId = identityUser.PrimaryTenantId
            };
            context.Users.Add(legacyUser);
        }
        else
        {
            // Update existing legacy user
            legacyUser.Name = identityUser.FullName ?? identityUser.Email ?? "Unknown";
            legacyUser.Email = identityUser.Email ?? "";
            legacyUser.Role = role;
            legacyUser.Department = identityUser.Department ?? "";
            legacyUser.Phone = phone ?? "";
            legacyUser.IsActive = identityUser.IsActive;
            legacyUser.AspNetUserId = identityUser.Id;
            legacyUser.TenantId = identityUser.PrimaryTenantId;
        }
    }

    /// <summary>
    /// Synchronizes all Identity users to the legacy Users table
    /// Call this to fix any inconsistencies
    /// </summary>
    public static async Task SyncAllUsersToLegacyTableAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        var identityUsers = await userManager.Users.ToListAsync();

        foreach (var identityUser in identityUsers)
        {
            var roles = await userManager.GetRolesAsync(identityUser);
            var primaryRole = roles.FirstOrDefault() ?? "Technician";
            
            await SyncToLegacyUsersTable(context, identityUser, primaryRole, identityUser.PhoneNumber ?? "");
        }

        await context.SaveChangesAsync();
    }
}
