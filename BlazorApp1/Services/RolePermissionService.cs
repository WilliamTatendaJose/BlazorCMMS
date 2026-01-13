using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;
using System.Security.Claims;

namespace BlazorApp1.Services;

/// <summary>
/// Centralized permission management service.
/// Provides role-based access control (RBAC) with tenant isolation.
/// </summary>
public class RolePermissionService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<RolePermissionService> _logger;

    // Cache for current user to avoid repeated auth state lookups
    private ClaimsPrincipal? _cachedUser;
    private DateTime _cacheTime = DateTime.MinValue;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(30);

    public RolePermissionService(
        AuthenticationStateProvider authenticationStateProvider,
        IDbContextFactory<ApplicationDbContext> contextFactory,
        ILogger<RolePermissionService> logger)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _contextFactory = contextFactory;
        _logger = logger;
    }

    #region Role Hierarchy

    /// <summary>
    /// Role hierarchy from highest to lowest privilege
    /// </summary>
    public static readonly string[] RoleHierarchy = 
    {
        "SuperAdmin",       // Level 0 - Full system access
        "TenantAdmin",      // Level 1 - Full tenant access
        "Admin",            // Level 2 - Administrative access
        "Reliability Engineer", // Level 3 - Engineering access
        "Planner",          // Level 4 - Planning access
        "Supervisor",       // Level 5 - Supervisory access
        "Technician",       // Level 6 - Operational access
        "Viewer"            // Level 7 - Read-only access
    };

    /// <summary>
    /// Get the privilege level of a role (lower = more privileged)
    /// </summary>
    public static int GetRoleLevel(string role)
    {
        var index = Array.IndexOf(RoleHierarchy, role);
        return index >= 0 ? index : int.MaxValue;
    }

    /// <summary>
    /// Check if role1 has higher or equal privilege than role2
    /// </summary>
    public static bool IsRoleHigherOrEqual(string role1, string role2)
    {
        return GetRoleLevel(role1) <= GetRoleLevel(role2);
    }

    #endregion

    #region Authentication State

    /// <summary>
    /// Get the current authenticated user
    /// </summary>
    private async Task<ClaimsPrincipal> GetCurrentUserAsync()
    {
        // Return cached user if still valid
        if (_cachedUser != null && DateTime.Now - _cacheTime < _cacheDuration)
        {
            return _cachedUser;
        }

        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        _cachedUser = authState.User;
        _cacheTime = DateTime.Now;
        return _cachedUser;
    }

    /// <summary>
    /// Invalidate the cached user (call after login/logout)
    /// </summary>
    public void InvalidateCache()
    {
        _cachedUser = null;
        _cacheTime = DateTime.MinValue;
    }

    /// <summary>
    /// Check if the current user is authenticated
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var user = await GetCurrentUserAsync();
        return user.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Get the current user's ID
    /// </summary>
    public async Task<string?> GetCurrentUserIdAsync()
    {
        var user = await GetCurrentUserAsync();
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Get the current user's email
    /// </summary>
    public async Task<string> GetCurrentUserEmailAsync()
    {
        var user = await GetCurrentUserAsync();
        return user.FindFirst(ClaimTypes.Email)?.Value ?? 
               user.FindFirst(ClaimTypes.Name)?.Value ?? "";
    }

    /// <summary>
    /// Get the current user's display name
    /// </summary>
    public async Task<string> GetCurrentUserNameAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user.Identity?.IsAuthenticated != true)
            return "Guest";

        // Try to get full name from custom claim, fallback to email
        var fullName = user.FindFirst("FullName")?.Value;
        if (!string.IsNullOrEmpty(fullName))
            return fullName;

        return user.Identity.Name ?? "Unknown User";
    }

    /// <summary>
    /// Get the current user's primary role
    /// </summary>
    public async Task<string> GetCurrentUserRoleAsync()
    {
        var user = await GetCurrentUserAsync();

        if (user.Identity?.IsAuthenticated != true)
            return "Guest";

        // Check roles in hierarchy order (most privileged first)
        foreach (var role in RoleHierarchy)
        {
            if (user.IsInRole(role))
                return role;
        }

        return "Viewer"; // Default role for authenticated users
    }

    /// <summary>
    /// Get all roles the current user belongs to
    /// </summary>
    public async Task<List<string>> GetCurrentUserRolesAsync()
    {
        var user = await GetCurrentUserAsync();
        var roles = new List<string>();

        foreach (var role in RoleHierarchy)
        {
            if (user.IsInRole(role))
                roles.Add(role);
        }

        return roles;
    }

    /// <summary>
    /// Check if the current user is in a specific role
    /// </summary>
    public async Task<bool> IsInRoleAsync(string role)
    {
        var user = await GetCurrentUserAsync();
        return user.IsInRole(role);
    }

    #endregion

    #region SuperAdmin & TenantAdmin

    /// <summary>
    /// Check if current user is SuperAdmin (system-wide access)
    /// </summary>
    public async Task<bool> IsSuperAdminAsync()
    {
        var user = await GetCurrentUserAsync();
        return user.IsInRole("SuperAdmin");
    }

    /// <summary>
    /// Check if current user is TenantAdmin (tenant-wide access)
    /// </summary>
    public async Task<bool> IsTenantAdminAsync()
    {
        var user = await GetCurrentUserAsync();
        return user.IsInRole("TenantAdmin");
    }

    /// <summary>
    /// Check if current user has administrative privileges
    /// </summary>
    public async Task<bool> IsAdminOrAboveAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin";
    }

    #endregion

    #region Tenant Access

    /// <summary>
    /// Get the current user's tenant ID
    /// </summary>
    public async Task<int?> GetCurrentTenantIdAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        if (string.IsNullOrEmpty(userId))
            return null;

        try
        {
            using var context = _contextFactory.CreateDbContext();
            var appUser = await context.Set<ApplicationUser>()
                .FirstOrDefaultAsync(u => u.Id == userId);
            return appUser?.PrimaryTenantId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tenant ID for user {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// Check if user can access a specific tenant
    /// </summary>
    public async Task<bool> CanAccessTenantAsync(int tenantId)
    {
        // SuperAdmin can access any tenant
        if (await IsSuperAdminAsync())
            return true;

        // Get user's accessible tenants
        var accessibleTenants = await GetAccessibleTenantIdsAsync();
        return accessibleTenants.Contains(tenantId);
    }

    /// <summary>
    /// Get list of tenant IDs the current user can access
    /// </summary>
    public async Task<List<int>> GetAccessibleTenantIdsAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        if (string.IsNullOrEmpty(userId))
            return new List<int>();

        // SuperAdmin can access all tenants
        if (await IsSuperAdminAsync())
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.Tenants
                    .Where(t => t.IsActive)
                    .Select(t => t.Id)
                    .ToListAsync();
            }
            catch
            {
                return new List<int>();
            }
        }

        // Regular users - get from mappings and primary tenant
        try
        {
            using var context = _contextFactory.CreateDbContext();
            
            var appUser = await context.Set<ApplicationUser>()
                .Include(u => u.TenantMappings)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (appUser == null)
                return new List<int>();

            var tenantIds = new HashSet<int>();

            // Add primary tenant
            if (appUser.PrimaryTenantId.HasValue)
                tenantIds.Add(appUser.PrimaryTenantId.Value);

            // Add mapped tenants
            foreach (var mapping in appUser.TenantMappings.Where(m => m.IsActive))
            {
                tenantIds.Add(mapping.TenantId);
            }

            return tenantIds.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting accessible tenants for user {UserId}", userId);
            return new List<int>();
        }
    }

    #endregion

    #region User Management Permissions

    /// <summary>
    /// Check if current user can manage other users
    /// </summary>
    public async Task<bool> CanManageUsersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin";
    }

    /// <summary>
    /// Check if current user can create users
    /// </summary>
    public async Task<bool> CanCreateUsersAsync()
    {
        return await CanManageUsersAsync();
    }

    /// <summary>
    /// Check if current user can delete users
    /// </summary>
    public async Task<bool> CanDeleteUsersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "Admin";
    }

    /// <summary>
    /// Check if current user can manage a specific role
    /// </summary>
    public async Task<bool> CanManageRoleAsync(string targetRole)
    {
        var currentRole = await GetCurrentUserRoleAsync();

        // SuperAdmin can manage all roles
        if (currentRole == "SuperAdmin")
            return true;

        // TenantAdmin can manage Planner, Supervisor, Technician, Viewer
        if (currentRole == "TenantAdmin")
            return targetRole is "Planner" or "Supervisor" or "Technician" or "Viewer";

        // Admin can manage Supervisor, Technician, Viewer
        if (currentRole == "Admin")
            return targetRole is "Supervisor" or "Technician" or "Viewer";

        return false;
    }

    /// <summary>
    /// Check if current user can assign a specific role
    /// </summary>
    public async Task<bool> CanAssignRoleAsync(string targetRole)
    {
        return await CanManageRoleAsync(targetRole);
    }

    /// <summary>
    /// Get roles that the current user can assign to others
    /// </summary>
    public async Task<List<string>> GetAssignableRolesAsync()
    {
        var currentRole = await GetCurrentUserRoleAsync();
        var assignableRoles = new List<string>();

        foreach (var role in RoleHierarchy)
        {
            if (await CanAssignRoleAsync(role))
                assignableRoles.Add(role);
        }

        return assignableRoles;
    }

    #endregion

    #region General CRUD Permissions

    /// <summary>
    /// Check if user can view data (all authenticated users)
    /// </summary>
    public async Task<bool> CanViewAsync()
    {
        return await IsAuthenticatedAsync();
    }

    /// <summary>
    /// Check if user can create/edit data
    /// </summary>
    public async Task<bool> CanEditAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is not ("Viewer" or "Guest");
    }

    /// <summary>
    /// Check if user can delete data
    /// </summary>
    public async Task<bool> CanDeleteAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer";
    }

    #endregion

    #region Asset Permissions

    /// <summary>
    /// Check if user can manage assets
    /// </summary>
    public async Task<bool> CanManageAssetsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner";
    }

    /// <summary>
    /// Check if user can retire assets
    /// </summary>
    public async Task<bool> CanRetireAssetsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer";
    }

    #endregion

    #region Work Order Permissions

    /// <summary>
    /// Check if user can create work orders
    /// </summary>
    public async Task<bool> CanCreateWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner" or "Supervisor";
    }

    /// <summary>
    /// Check if user can request work orders (all non-viewers)
    /// </summary>
    public async Task<bool> CanRequestWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is not ("Viewer" or "Guest");
    }

    /// <summary>
    /// Check if user can approve work orders
    /// </summary>
    public async Task<bool> CanApproveWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner" or "Supervisor";
    }

    /// <summary>
    /// Check if user can assign work orders
    /// </summary>
    public async Task<bool> CanAssignWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner" or "Supervisor";
    }

    /// <summary>
    /// Check if user can complete work orders
    /// </summary>
    public async Task<bool> CanCompleteWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Supervisor" or "Technician";
    }

    /// <summary>
    /// Check if user can reject work orders
    /// </summary>
    public async Task<bool> CanRejectWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner" or "Supervisor";
    }

    /// <summary>
    /// Check if user can cancel work orders
    /// </summary>
    public async Task<bool> CanCancelWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner";
    }

    /// <summary>
    /// Check if user can view all work orders (vs only assigned)
    /// </summary>
    public async Task<bool> CanViewAllWorkOrdersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner" or "Supervisor";
    }

    #endregion

    #region Schedule & Planning Permissions

    /// <summary>
    /// Check if user can manage maintenance schedules
    /// </summary>
    public async Task<bool> CanManageSchedulesAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner";
    }

    /// <summary>
    /// Check if user can create recurring schedules
    /// </summary>
    public async Task<bool> CanCreateRecurringSchedulesAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner";
    }

    #endregion

    #region Analysis & Reporting Permissions

    /// <summary>
    /// Check if user can access analytics dashboard
    /// </summary>
    public async Task<bool> CanAccessAnalyticsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner" or "Supervisor";
    }

    /// <summary>
    /// Check if user can modify FMEA data
    /// </summary>
    public async Task<bool> CanModifyFMEAAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer";
    }

    /// <summary>
    /// Check if user can access reliability analysis
    /// </summary>
    public async Task<bool> CanAccessReliabilityAnalysisAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner";
    }

    /// <summary>
    /// Check if user can export data
    /// </summary>
    public async Task<bool> CanExportDataAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner";
    }

    /// <summary>
    /// Check if user can import data
    /// </summary>
    public async Task<bool> CanImportDataAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin";
    }

    #endregion

    #region Spare Parts & Inventory Permissions

    /// <summary>
    /// Check if user can manage spare parts
    /// </summary>
    public async Task<bool> CanManageSparePartsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner";
    }

    /// <summary>
    /// Check if user can issue spare parts
    /// </summary>
    public async Task<bool> CanIssueSparePartsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer" or "Planner" or "Supervisor" or "Technician";
    }

    /// <summary>
    /// Check if user can adjust inventory
    /// </summary>
    public async Task<bool> CanAdjustInventoryAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Planner";
    }

    #endregion

    #region Document Permissions

    /// <summary>
    /// Check if user can upload documents
    /// </summary>
    public async Task<bool> CanUploadDocumentsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is not ("Viewer" or "Guest");
    }

    /// <summary>
    /// Check if user can delete documents
    /// </summary>
    public async Task<bool> CanDeleteDocumentsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin" or "Reliability Engineer";
    }

    /// <summary>
    /// Check if user can manage document categories
    /// </summary>
    public async Task<bool> CanManageDocumentCategoriesAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin";
    }

    #endregion

    #region Tenant Management Permissions

    /// <summary>
    /// Check if user can manage tenants
    /// </summary>
    public async Task<bool> CanManageTenantsAsync()
    {
        return await IsSuperAdminAsync();
    }

    /// <summary>
    /// Check if user can create tenants
    /// </summary>
    public async Task<bool> CanCreateTenantsAsync()
    {
        return await IsSuperAdminAsync();
    }

    /// <summary>
    /// Check if user can delete tenants
    /// </summary>
    public async Task<bool> CanDeleteTenantsAsync()
    {
        return await IsSuperAdminAsync();
    }

    /// <summary>
    /// Check if user can manage tenant users
    /// </summary>
    public async Task<bool> CanManageTenantUsersAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin";
    }

    #endregion

    #region Settings & Configuration Permissions

    /// <summary>
    /// Check if user can access system settings
    /// </summary>
    public async Task<bool> CanAccessSettingsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin";
    }

    /// <summary>
    /// Check if user can modify system settings
    /// </summary>
    public async Task<bool> CanModifySettingsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin";
    }

    /// <summary>
    /// Check if user can manage notification settings
    /// </summary>
    public async Task<bool> CanManageNotificationsAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin";
    }

    /// <summary>
    /// Check if user can manage WhatsApp integration
    /// </summary>
    public async Task<bool> CanManageWhatsAppAsync()
    {
        var role = await GetCurrentUserRoleAsync();
        return role is "SuperAdmin" or "TenantAdmin" or "Admin";
    }

    #endregion

    #region Permission Summary

    /// <summary>
    /// Get a summary of all permissions for the current user
    /// </summary>
    public async Task<PermissionSummary> GetPermissionSummaryAsync()
    {
        return new PermissionSummary
        {
            CurrentRole = await GetCurrentUserRoleAsync(),
            IsSuperAdmin = await IsSuperAdminAsync(),
            IsTenantAdmin = await IsTenantAdminAsync(),
            CanView = await CanViewAsync(),
            CanEdit = await CanEditAsync(),
            CanDelete = await CanDeleteAsync(),
            CanManageUsers = await CanManageUsersAsync(),
            CanManageAssets = await CanManageAssetsAsync(),
            CanManageWorkOrders = await CanCreateWorkOrdersAsync(),
            CanManageSchedules = await CanManageSchedulesAsync(),
            CanAccessAnalytics = await CanAccessAnalyticsAsync(),
            CanManageSettings = await CanModifySettingsAsync(),
            CanManageTenants = await CanManageTenantsAsync()
        };
    }

    #endregion
}

/// <summary>
/// Summary of user permissions
/// </summary>
public class PermissionSummary
{
    public string CurrentRole { get; set; } = "Guest";
    public bool IsSuperAdmin { get; set; }
    public bool IsTenantAdmin { get; set; }
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanManageUsers { get; set; }
    public bool CanManageAssets { get; set; }
    public bool CanManageWorkOrders { get; set; }
    public bool CanManageSchedules { get; set; }
    public bool CanAccessAnalytics { get; set; }
    public bool CanManageSettings { get; set; }
    public bool CanManageTenants { get; set; }
}
