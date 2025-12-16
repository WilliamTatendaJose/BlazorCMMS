using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorApp1.Services;

public interface ITenantManagementService
{
    Task<Tenant?> CreateTenantAsync(string tenantCode, string name, string createdBy);
    Task<Tenant?> GetTenantByIdAsync(int tenantId);
    Task<Tenant?> GetTenantByCodeAsync(string tenantCode);
    Task<List<Tenant>> GetAllTenantsAsync();
    Task<List<Tenant>> GetAccessibleTenantsAsync();
    Task<bool> UpdateTenantAsync(Tenant tenant, string modifiedBy);
    Task<bool> DeleteTenantAsync(int tenantId);
    Task<bool> AddUserToTenantAsync(int tenantId, string userId, bool isTenantAdmin = false);
    Task<bool> RemoveUserFromTenantAsync(int tenantId, string userId);
    Task<bool> SetUserAsAdminAsync(int tenantId, string userId, bool isAdmin);
    Task<List<ApplicationUser>> GetTenantUsersAsync(int tenantId);
    Task<int> GetTenantUserCountAsync(int tenantId);
    Task<bool> ActivateTenantAsync(int tenantId);
    Task<bool> DeactivateTenantAsync(int tenantId);
}

public class TenantManagementService : ITenantManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RolePermissionService _rolePermissionService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        UserManager<ApplicationUser> userManager,
        RolePermissionService rolePermissionService,
        IHttpContextAccessor httpContextAccessor)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
        _rolePermissionService = rolePermissionService;
        _httpContextAccessor = httpContextAccessor;
    }

    // ==================== Helper Methods ====================

    private async Task<int?> GetCurrentTenantIdAsync()
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return null;

            using var context = _contextFactory.CreateDbContext();
            var appUser = await context.Set<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == userId);
            return appUser?.PrimaryTenantId;
        }
        catch
        {
            return null;
        }
    }

    private async Task<bool> IsSuperAdminAsync()
    {
        try
        {
            return await _rolePermissionService.IsSuperAdminAsync();
        }
        catch
        {
            return false;
        }
    }

    // ==================== Tenant Management ====================

    public async Task<Tenant?> CreateTenantAsync(string tenantCode, string name, string createdBy)
    {
        // Only SuperAdmin can create tenants
        if (!await IsSuperAdminAsync())
            throw new UnauthorizedAccessException("Only SuperAdmin can create tenants");

        if (string.IsNullOrWhiteSpace(tenantCode) || string.IsNullOrWhiteSpace(name))
            return null;

        using var context = _contextFactory.CreateDbContext();
        
        var existingTenant = await context.Tenants
            .FirstOrDefaultAsync(t => t.TenantCode == tenantCode);
        
        if (existingTenant != null)
            return null;

        var tenant = new Tenant
        {
            TenantCode = tenantCode,
            Name = name,
            CreatedBy = createdBy,
            CreatedDate = DateTime.Now,
            Status = "Active",
            IsActive = true
        };

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        return tenant;
    }

    public async Task<Tenant?> GetTenantByIdAsync(int tenantId)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var currentTenantId = await GetCurrentTenantIdAsync();

        var tenant = await context.Tenants
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant == null)
            return null;

        // Verify access
        if (!isSuperAdmin && currentTenantId.HasValue && tenant.Id != currentTenantId)
            throw new UnauthorizedAccessException("You do not have access to this tenant");

        return tenant;
    }

    public async Task<Tenant?> GetTenantByCodeAsync(string tenantCode)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var currentTenantId = await GetCurrentTenantIdAsync();

        var tenant = await context.Tenants
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.TenantCode == tenantCode);

        if (tenant == null)
            return null;

        // Verify access
        if (!isSuperAdmin && currentTenantId.HasValue && tenant.Id != currentTenantId)
            throw new UnauthorizedAccessException("You do not have access to this tenant");

        return tenant;
    }

    public async Task<List<Tenant>> GetAllTenantsAsync()
    {
        // Only SuperAdmin can see all tenants
        if (!await IsSuperAdminAsync())
            throw new UnauthorizedAccessException("Only SuperAdmin can view all tenants");

        using var context = _contextFactory.CreateDbContext();
        return await context.Tenants
            .Include(t => t.Users)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<Tenant>> GetAccessibleTenantsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        
        if (isSuperAdmin)
        {
            // SuperAdmin sees all tenants
            return await context.Tenants
                .Include(t => t.Users)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        // Regular users see only their assigned tenants
        var userId = _httpContextAccessor.HttpContext?.User
            ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return new List<Tenant>();

        var tenants = await context.Tenants
            .Include(t => t.Users)
            .Where(t => t.Users.Any(u => u.Id == userId))
            .OrderBy(t => t.Name)
            .ToListAsync();

        return tenants;
    }

    public async Task<bool> UpdateTenantAsync(Tenant tenant, string modifiedBy)
    {
        if (tenant.Id <= 0)
            return false;

        // Only SuperAdmin can update tenants
        if (!await IsSuperAdminAsync())
            throw new UnauthorizedAccessException("Only SuperAdmin can update tenants");

        using var context = _contextFactory.CreateDbContext();
        var existingTenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == tenant.Id);
        
        if (existingTenant == null)
            return false;

        existingTenant.Name = tenant.Name;
        existingTenant.Description = tenant.Description;
        existingTenant.ContactPerson = tenant.ContactPerson;
        existingTenant.ContactPhone = tenant.ContactPhone;
        existingTenant.ContactEmail = tenant.ContactEmail;
        existingTenant.Address = tenant.Address;
        existingTenant.City = tenant.City;
        existingTenant.Country = tenant.Country;
        existingTenant.PostalCode = tenant.PostalCode;
        existingTenant.MaxUsers = tenant.MaxUsers;
        existingTenant.MaxAssets = tenant.MaxAssets;
        existingTenant.MaxDocuments = tenant.MaxDocuments;
        existingTenant.ModifiedDate = DateTime.Now;
        existingTenant.ModifiedBy = modifiedBy;

        context.Tenants.Update(existingTenant);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteTenantAsync(int tenantId)
    {
        // Only SuperAdmin can delete tenants
        if (!await IsSuperAdminAsync())
            throw new UnauthorizedAccessException("Only SuperAdmin can delete tenants");

        using var context = _contextFactory.CreateDbContext();
        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        
        if (tenant == null)
            return false;

        var mappings = await context.UserTenantMappings
            .Where(m => m.TenantId == tenantId)
            .ToListAsync();
        
        context.UserTenantMappings.RemoveRange(mappings);

        var applicationUsers = context.Set<ApplicationUser>().AsQueryable();
        var usersToUpdate = await applicationUsers
            .Where(u => u.PrimaryTenantId == tenantId)
            .ToListAsync();
        
        foreach (var appUser in usersToUpdate)
        {
            appUser.PrimaryTenantId = null;
        }

        context.Tenants.Remove(tenant);
        await context.SaveChangesAsync();

        return true;
    }

    // ==================== User Management ====================

    public async Task<bool> AddUserToTenantAsync(int tenantId, string userId, bool isTenantAdmin = false)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var currentTenantId = await GetCurrentTenantIdAsync();

        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        if (tenant == null)
            return false;

        // Only SuperAdmin or TenantAdmin of that tenant can add users
        if (!isSuperAdmin)
        {
            if (!currentTenantId.HasValue || currentTenantId.Value != tenantId)
                throw new UnauthorizedAccessException("You do not have access to this tenant");
        }

        var applicationUsers = context.Set<ApplicationUser>().AsQueryable();
        var appUser = await applicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
        if (appUser == null)
            return false;

        var existingMapping = await context.UserTenantMappings
            .FirstOrDefaultAsync(m => m.TenantId == tenantId && m.UserId == userId && m.RemovedDate == null);
        
        if (existingMapping != null)
            return true;

        var mapping = new TenantUserMapping
        {
            TenantId = tenantId,
            UserId = userId,
            IsTenantAdmin = isTenantAdmin,
            AssignedDate = DateTime.Now
        };

        context.UserTenantMappings.Add(mapping);

        if (!appUser.PrimaryTenantId.HasValue)
        {
            appUser.PrimaryTenantId = tenantId;
            context.Set<ApplicationUser>().Update(appUser);
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveUserFromTenantAsync(int tenantId, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var currentTenantId = await GetCurrentTenantIdAsync();

        // Only SuperAdmin or TenantAdmin of that tenant can remove users
        if (!isSuperAdmin)
        {
            if (!currentTenantId.HasValue || currentTenantId.Value != tenantId)
                throw new UnauthorizedAccessException("You do not have access to this tenant");
        }

        var mapping = await context.UserTenantMappings
            .FirstOrDefaultAsync(m => m.TenantId == tenantId && m.UserId == userId && m.RemovedDate == null);
        
        if (mapping == null)
            return false;

        mapping.RemovedDate = DateTime.Now;
        context.UserTenantMappings.Update(mapping);

        var applicationUsers = context.Set<ApplicationUser>().AsQueryable();
        var appUser = await applicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
        if (appUser != null && appUser.PrimaryTenantId == tenantId)
        {
            var otherMapping = await context.UserTenantMappings
                .FirstOrDefaultAsync(m => m.UserId == userId && m.RemovedDate == null);
            
            if (otherMapping != null)
            {
                appUser.PrimaryTenantId = otherMapping.TenantId;
            }
            else
            {
                appUser.PrimaryTenantId = null;
            }

            context.Set<ApplicationUser>().Update(appUser);
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetUserAsAdminAsync(int tenantId, string userId, bool isAdmin)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var currentTenantId = await GetCurrentTenantIdAsync();

        // Only SuperAdmin or TenantAdmin of that tenant can set role
        if (!isSuperAdmin)
        {
            if (!currentTenantId.HasValue || currentTenantId.Value != tenantId)
                throw new UnauthorizedAccessException("You do not have access to this tenant");
        }

        var mapping = await context.UserTenantMappings
            .FirstOrDefaultAsync(m => m.TenantId == tenantId && m.UserId == userId && m.RemovedDate == null);
        
        if (mapping == null)
            return false;

        mapping.IsTenantAdmin = isAdmin;
        context.UserTenantMappings.Update(mapping);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ApplicationUser>> GetTenantUsersAsync(int tenantId)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var currentTenantId = await GetCurrentTenantIdAsync();

        // Verify access
        if (!isSuperAdmin)
        {
            if (!currentTenantId.HasValue || currentTenantId.Value != tenantId)
                throw new UnauthorizedAccessException("You do not have access to this tenant");
        }

        var applicationUsers = context.Set<ApplicationUser>().AsQueryable();
        return await applicationUsers
            .Where(u => u.PrimaryTenantId == tenantId || u.TenantMappings.Any(m => m.TenantId == tenantId && m.RemovedDate == null))
            .ToListAsync();
    }

    public async Task<int> GetTenantUserCountAsync(int tenantId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserTenantMappings
            .Where(m => m.TenantId == tenantId && m.RemovedDate == null)
            .CountAsync();
    }

    // ==================== Tenant Status ====================

    public async Task<bool> ActivateTenantAsync(int tenantId)
    {
        // Only SuperAdmin can activate tenants
        if (!await IsSuperAdminAsync())
            throw new UnauthorizedAccessException("Only SuperAdmin can activate tenants");

        using var context = _contextFactory.CreateDbContext();
        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        
        if (tenant == null)
            return false;

        tenant.IsActive = true;
        tenant.Status = "Active";
        tenant.ModifiedDate = DateTime.Now;
        
        context.Tenants.Update(tenant);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateTenantAsync(int tenantId)
    {
        // Only SuperAdmin can deactivate tenants
        if (!await IsSuperAdminAsync())
            throw new UnauthorizedAccessException("Only SuperAdmin can deactivate tenants");

        using var context = _contextFactory.CreateDbContext();
        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        
        if (tenant == null)
            return false;

        tenant.IsActive = false;
        tenant.Status = "Inactive";
        tenant.ModifiedDate = DateTime.Now;
        
        context.Tenants.Update(tenant);
        await context.SaveChangesAsync();

        return true;
    }
}
