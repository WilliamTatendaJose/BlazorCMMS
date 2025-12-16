using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorApp1.Services;

/// <summary>
/// Service for managing the current tenant context in multi-tenant applications
/// </summary>
public class TenantContext
{
    public int? TenantId { get; set; }
    public string? TenantCode { get; set; }
    public string? TenantName { get; set; }
    public bool IsSuperAdmin { get; set; }
    public bool IsTenantAdmin { get; set; }
    public string? UserId { get; set; }
    public bool IsInitialized { get; set; }
}

public interface ITenantService
{
    Task<TenantContext> GetTenantContextAsync();
    Task<bool> IsUserInTenantAsync(string userId, int tenantId);
    Task<List<int>> GetUserTenantsAsync(string userId);
    void SetTenantContext(int? tenantId, string? tenantCode, string? tenantName);
}

public class TenantService : ITenantService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private TenantContext _tenantContext = new();

    public TenantService(
        AuthenticationStateProvider authenticationStateProvider,
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _contextFactory = contextFactory;
    }

    public async Task<TenantContext> GetTenantContextAsync()
    {
        if (_tenantContext.IsInitialized)
        {
            return _tenantContext;
        }

        try
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                return _tenantContext;
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return _tenantContext;
            }

            using var context = _contextFactory.CreateDbContext();
            var applicationUsers = context.Set<ApplicationUser>().AsQueryable();
            var appUser = await applicationUsers
                .Include(u => u.Tenant)
                .Include(u => u.TenantMappings)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (appUser == null)
            {
                return _tenantContext;
            }

            _tenantContext.UserId = userId;
            _tenantContext.IsSuperAdmin = appUser.IsSuperAdmin;

            if (appUser.IsSuperAdmin)
            {
                _tenantContext.TenantId = null;
                _tenantContext.IsTenantAdmin = false;
            }
            else if (appUser.PrimaryTenantId.HasValue)
            {
                _tenantContext.TenantId = appUser.PrimaryTenantId;
                _tenantContext.TenantCode = appUser.Tenant?.TenantCode;
                _tenantContext.TenantName = appUser.Tenant?.Name;
                
                var mapping = appUser.TenantMappings.FirstOrDefault(m => m.TenantId == appUser.PrimaryTenantId);
                _tenantContext.IsTenantAdmin = mapping?.IsTenantAdmin ?? false;
            }

            _tenantContext.IsInitialized = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting tenant context: {ex.Message}");
        }

        return _tenantContext;
    }

    public async Task<bool> IsUserInTenantAsync(string userId, int tenantId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserTenantMappings
            .AnyAsync(m => m.UserId == userId && m.TenantId == tenantId && m.RemovedDate == null);
    }

    public async Task<List<int>> GetUserTenantsAsync(string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var applicationUsers = context.Set<ApplicationUser>().AsQueryable();
        var appUser = await applicationUsers
            .Include(u => u.TenantMappings)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (appUser == null)
            return new List<int>();

        if (appUser.IsSuperAdmin)
        {
            // Super admin can see all tenants
            return await context.Tenants
                .Select(t => t.Id)
                .ToListAsync();
        }

        return appUser.TenantMappings
            .Where(m => m.RemovedDate == null)
            .Select(m => m.TenantId)
            .ToList();
    }

    public void SetTenantContext(int? tenantId, string? tenantCode, string? tenantName)
    {
        _tenantContext = new TenantContext
        {
            TenantId = tenantId,
            TenantCode = tenantCode,
            TenantName = tenantName,
            IsInitialized = true
        };
    }
}
