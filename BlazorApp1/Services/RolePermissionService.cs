using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorApp1.Services;

public class RolePermissionService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public RolePermissionService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<string> GetCurrentUserRoleAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value ?? "Technician";
        }

        return "Guest";
    }

    public async Task<string> GetCurrentUserNameAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            return user.Identity.Name ?? "Unknown User";
        }

        return "Guest";
    }

    public async Task<string> GetCurrentUserEmailAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var emailClaim = user.FindFirst(ClaimTypes.Email);
            return emailClaim?.Value ?? "";
        }

        return "";
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.IsInRole(role);
    }

    public async Task<bool> CanViewAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Technician" => true,
            _ => false
        };
    }

    public async Task<bool> CanEditAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Technician" => true,
            _ => false
        };
    }

    public async Task<bool> CanDeleteAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "Admin" => true,
            "Reliability Engineer" => true,
            _ => false
        };
    }

    public async Task<bool> CanManageUsersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role == "Admin";
    }

    public async Task<bool> CanCreateWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            _ => false
        };
    }

    public async Task<bool> CanCompleteWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "Admin" => true,
            "Reliability Engineer" => true,
            "Technician" => true,
            _ => false
        };
    }

    public async Task<bool> CanAccessAnalyticsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            _ => false
        };
    }

    public async Task<bool> CanModifyFMEAAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "Admin" => true,
            "Reliability Engineer" => true,
            _ => false
        };
    }
}
