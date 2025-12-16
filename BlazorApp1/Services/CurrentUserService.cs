using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;

namespace BlazorApp1.Services;

public class CurrentUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly RolePermissionService _permissionService;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public string UserName { get; private set; } = "Guest";
    public string Role { get; private set; } = "Guest";
    public string Email { get; private set; } = "";
    public string Department { get; private set; } = "";
    public string UserId { get; private set; } = "";
    public int? TenantId { get; private set; }
    public bool IsSuperAdmin { get; private set; }
    
    public bool IsAuthenticated { get; private set; }
    public bool CanEdit { get; private set; }
    public bool CanDelete { get; private set; }
    public bool CanCreateWork { get; private set; }
    public bool CanRequestWork { get; private set; }
    public bool CanApproveWork { get; private set; }
    public bool CanAssignWork { get; private set; }
    public bool CanCompleteWork { get; private set; }
    public bool CanRejectWork { get; private set; }
    public bool CanCancelWork { get; private set; }
    public bool CanViewAllWorkOrders { get; private set; }
    
    // Computed property
    public bool IsAdmin => Role == "Admin" || IsSuperAdmin;
    public bool IsSupervisor => Role == "Supervisor" || Role == "Admin" || Role == "Reliability Engineer" || Role == "Planner" || IsSuperAdmin;

    public CurrentUserService(
        AuthenticationStateProvider authenticationStateProvider,
        RolePermissionService permissionService,
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _permissionService = permissionService;
        _contextFactory = contextFactory;
    }

    public async Task InitializeAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            IsAuthenticated = true;
            UserName = user.Identity.Name ?? "Unknown User";
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? "";
            UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            Role = user.FindFirst(ClaimTypes.Role)?.Value ?? "Technician";
            
            // Load tenant information from database
            await LoadTenantInfoAsync();
            
            // Get permissions based on role
            await LoadPermissionsAsync();
        }
        else
        {
            IsAuthenticated = false;
            UserName = "Guest";
            UserId = "";
            Role = "Guest";
            Email = "";
            TenantId = null;
            IsSuperAdmin = false;
            ResetPermissions();
        }
    }

    private async Task LoadTenantInfoAsync()
    {
        if (string.IsNullOrEmpty(UserId))
            return;

        try
        {
            using var context = _contextFactory.CreateDbContext();
            // Use Set<ApplicationUser>() to get Identity users, not RBM Users table
            var appUser = await context.Set<ApplicationUser>()
                .FirstOrDefaultAsync(u => u.Id == UserId);

            if (appUser != null)
            {
                TenantId = appUser.PrimaryTenantId;
                IsSuperAdmin = appUser.IsSuperAdmin;
                
                // SuperAdmin role takes precedence
                if (IsSuperAdmin)
                {
                    Role = "SuperAdmin";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tenant info: {ex.Message}");
        }
    }

    private async Task LoadPermissionsAsync()
    {
        CanEdit = await _permissionService.CanEditAsync();
        CanDelete = await _permissionService.CanDeleteAsync();
        CanCreateWork = await _permissionService.CanCreateWorkOrdersAsync();
        CanRequestWork = await _permissionService.CanRequestWorkOrdersAsync();
        CanApproveWork = await _permissionService.CanApproveWorkOrdersAsync();
        CanAssignWork = await _permissionService.CanAssignWorkOrdersAsync();
        CanCompleteWork = await _permissionService.CanCompleteWorkOrdersAsync();
        CanRejectWork = await _permissionService.CanRejectWorkOrdersAsync();
        CanCancelWork = await _permissionService.CanCancelWorkOrdersAsync();
        CanViewAllWorkOrders = await _permissionService.CanViewAllWorkOrdersAsync();
    }

    private void ResetPermissions()
    {
        CanEdit = false;
        CanDelete = false;
        CanCreateWork = false;
        CanRequestWork = false;
        CanApproveWork = false;
        CanAssignWork = false;
        CanCompleteWork = false;
        CanRejectWork = false;
        CanCancelWork = false;
        CanViewAllWorkOrders = false;
    }

    public async Task SetUser(string userName, string role, string email, string department)
    {
        UserName = userName;
        Role = role;
        Email = email;
        Department = department;
        await LoadPermissionsAsync();
    }
}
