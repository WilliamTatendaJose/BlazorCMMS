using Microsoft.AspNetCore.Identity;
using BlazorApp1.Data;

namespace BlazorApp1.Data;

public static class IdentityDataSeeder
{
    public static async Task SeedRolesAndUsersAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles
        string[] roles = { "Admin", "Reliability Engineer", "Planner", "Technician" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed Admin User
        await CreateUserAsync(userManager, "admin@company.com", "Admin123!", "Admin User", "Management", "555-0001", "Admin");

        // Seed Reliability Engineer
        await CreateUserAsync(userManager, "sarah.johnson@company.com", "Sarah123!", "Sarah Johnson", "Engineering", "555-0002", "Reliability Engineer");

        // Seed Planner
        await CreateUserAsync(userManager, "emily.brown@company.com", "Emily123!", "Emily Brown", "Planning", "555-0003", "Planner");

        // Seed Technicians
        await CreateUserAsync(userManager, "john.smith@company.com", "John123!", "John Smith", "Maintenance", "555-0004", "Technician");
        await CreateUserAsync(userManager, "mike.davis@company.com", "Mike123!", "Mike Davis", "Maintenance", "555-0005", "Technician");
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName,
        string department,
        string phone,
        string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true, // Auto-confirm for demo
                FullName = fullName,
                Department = department,
                PhoneNumber = phone,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
