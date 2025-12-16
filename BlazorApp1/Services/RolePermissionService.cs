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

    // ==================== SuperAdmin & Core Access ====================

    /// <summary>
    /// Check if current user is SuperAdmin
    /// SuperAdmin has access to all tenants and all roles
    /// </summary>
    public async Task<bool> IsSuperAdminAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.IsInRole("SuperAdmin");
    }

    /// <summary>
    /// Check if current user is TenantAdmin
    /// </summary>
    public async Task<bool> IsTenantAdminAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.IsInRole("TenantAdmin");
    }

    /// <summary>
    /// Check if user can access a specific tenant
    /// SuperAdmin can access all, others limited to assigned tenants
    /// </summary>
    public async Task<bool> CanAccessTenantAsync(int tenantId)
    {
        // SuperAdmin can access any tenant
        if (await IsSuperAdminAsync())
            return true;

        // TODO: For regular users, check their assigned tenants
        // var userId = GetCurrentUserId();
        // var accessibleTenants = await GetAccessibleTenantsAsync();
        // return accessibleTenants.Contains(tenantId);
        return false;
    }

    /// <summary>
    /// Check if user can manage a specific role
    /// SuperAdmin can manage all roles
    /// TenantAdmin can manage Technician and Viewer roles
    /// </summary>
    public async Task<bool> CanManageRoleAsync(string role)
    {
        // SuperAdmin can manage all roles
        if (await IsSuperAdminAsync())
            return true;

        // TenantAdmin can manage Technician and Viewer roles only
        if (await IsTenantAdminAsync())
            return role is "Technician" or "Viewer";

        return false;
    }

    /// <summary>
    /// Check if user can assign a specific role
    /// SuperAdmin can assign any role
    /// TenantAdmin can assign Technician and Viewer roles
    /// </summary>
    public async Task<bool> CanAssignRoleAsync(string role)
    {
        // SuperAdmin can assign any role
        if (await IsSuperAdminAsync())
            return true;

        // TenantAdmin can assign Technician and Viewer
        if (await IsTenantAdminAsync())
            return role is "Technician" or "Viewer";

        return false;
    }

    // ==================== Legacy Methods ====================

    public async Task<string> GetCurrentUserRoleAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            // Check for SuperAdmin first
            if (user.IsInRole("SuperAdmin")) return "SuperAdmin";
            if (user.IsInRole("TenantAdmin")) return "TenantAdmin";
            if (user.IsInRole("Admin")) return "Admin";
            if (user.IsInRole("Reliability Engineer")) return "Reliability Engineer";
            if (user.IsInRole("Planner")) return "Planner";
            if (user.IsInRole("Supervisor")) return "Supervisor";
            if (user.IsInRole("Technician")) return "Technician";
            if (user.IsInRole("Viewer")) return "Viewer";
            
            return "Technician"; // Default role
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
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Supervisor" => true,
            "Technician" => true,
            "Viewer" => true,
            _ => false
        };
    }

    public async Task<bool> CanEditAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Supervisor" => true,
            "Technician" => true,
            _ => false
        };
    }

    public async Task<bool> CanDeleteAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            _ => false
        };
    }

    public async Task<bool> CanManageUsersAsync()
    {
        // SuperAdmin and TenantAdmin can manage users
        if (await IsSuperAdminAsync())
            return true;

        if (await IsTenantAdminAsync())
            return true;

        var role = await GetCurrentUserRoleAsync();
        return role == "Admin";
    }

    // Work Order Permissions
    public async Task<bool> CanCreateWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Supervisor" => true,
            _ => false
        };
    }

    public async Task<bool> CanRequestWorkOrdersAsync()
    {
        // All authenticated users can request work orders
        var role = await GetCurrentUserRoleAsync();
        return role != "Guest";
    }

    public async Task<bool> CanApproveWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Supervisor" => true,
            _ => false
        };
    }

    public async Task<bool> CanAssignWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Supervisor" => true,
            _ => false
        };
    }

    public async Task<bool> CanCompleteWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Supervisor" => true,
            "Technician" => true,
            _ => false
        };
    }

    public async Task<bool> CanRejectWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Supervisor" => true,
            _ => false
        };
    }

    public async Task<bool> CanCancelWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            _ => false
        };
    }

    public async Task<bool> CanAccessAnalyticsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Supervisor" => true,
            _ => false
        };
    }

    public async Task<bool> CanModifyFMEAAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            _ => false
        };
    }

    public async Task<bool> CanManageSchedulesAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            _ => false
        };
    }

    public async Task<bool> CanViewAllWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role switch
        {
            "SuperAdmin" => true,
            "TenantAdmin" => true,
            "Admin" => true,
            "Reliability Engineer" => true,
            "Planner" => true,
            "Supervisor" => true,
            _ => false
        };
    }
}
