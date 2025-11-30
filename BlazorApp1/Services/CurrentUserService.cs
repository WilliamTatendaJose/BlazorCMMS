using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorApp1.Services;

public class CurrentUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly RolePermissionService _permissionService;

    public string UserName { get; private set; } = "Guest";
    public string Role { get; private set; } = "Guest";
    public string Email { get; private set; } = "";
    public string Department { get; private set; } = "";
    
    public bool IsAuthenticated { get; private set; }
    public bool CanEdit { get; private set; }
    public bool CanDelete { get; private set; }
    public bool CanCreateWork { get; private set; }
    
    // Computed property
    public bool IsAdmin => Role == "Admin";

    public CurrentUserService(
        AuthenticationStateProvider authenticationStateProvider,
        RolePermissionService permissionService)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _permissionService = permissionService;
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
            Role = user.FindFirst(ClaimTypes.Role)?.Value ?? "Technician";
            
            // Get permissions based on role
            CanEdit = await _permissionService.CanEditAsync();
            CanDelete = await _permissionService.CanDeleteAsync();
            CanCreateWork = await _permissionService.CanCreateWorkOrdersAsync();
        }
        else
        {
            IsAuthenticated = false;
            UserName = "Guest";
            Role = "Guest";
            Email = "";
            CanEdit = false;
            CanDelete = false;
        }
    }

    public async Task  SetUser(string userName, string role, string email, string department)
    {
        UserName = userName;
        Role = role;
        Email = email;
        Department = department;
        CanEdit= await _permissionService.CanEditAsync();
        CanDelete= await _permissionService.CanDeleteAsync();
        CanCreateWork = await _permissionService.CanCreateWorkOrdersAsync();
    }
}
