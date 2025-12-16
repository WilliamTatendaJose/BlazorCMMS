using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorApp1.Services;

public class DataService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RolePermissionService _rolePermissionService;

    public DataService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        IHttpContextAccessor httpContextAccessor,
        RolePermissionService rolePermissionService)
    {
        _contextFactory = contextFactory;
        _httpContextAccessor = httpContextAccessor;
        _rolePermissionService = rolePermissionService;
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

    // ==================== Asset Methods ====================

    public async Task<List<Asset>> GetAssetsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Where(a => !a.IsRetired)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.AssetId)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetAllAssetsAsync(bool includeRetired = false)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets.AsQueryable();
        
        if (!includeRetired)
        {
            query = query.Where(a => !a.IsRetired);
        }

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.AssetId)
            .ToListAsync();
    }

    public async Task<Asset?> GetAssetAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var asset = await context.Assets
            .Include(a => a.ConditionReadings)
            .Include(a => a.WorkOrders)
            .Include(a => a.FailureModes)
            .Include(a => a.ReliabilityMetrics)
            .Include(a => a.Attachments)
            .Include(a => a.DowntimeRecords)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (asset == null)
            return null;

        // Check tenant access
        if (!isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
            return null;

        return asset;
    }

    public async Task<Asset?> GetAssetByAssetIdAsync(string assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Include(a => a.ConditionReadings)
            .Include(a => a.WorkOrders)
            .Include(a => a.FailureModes)
            .Include(a => a.ReliabilityMetrics)
            .Where(a => a.AssetId == assetId && !a.IsRetired)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task AddAssetAsync(Asset asset)
    {
        using var context = _contextFactory.CreateDbContext();
        
        // Set tenant for non-SuperAdmin users
        var tenantId = await GetCurrentTenantIdAsync();
        if (!asset.TenantId.HasValue && tenantId.HasValue)
        {
            asset.TenantId = tenantId;
        }

        asset.CreatedDate = DateTime.Now;
        asset.IsActive = true;
        asset.IsRetired = false;
        
        if (string.IsNullOrEmpty(asset.AssetId))
            throw new ArgumentException("Asset ID is required");
        if (string.IsNullOrEmpty(asset.Name))
            throw new ArgumentException("Asset Name is required");
        
        context.Assets.Add(asset);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAssetAsync(Asset asset)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        // Verify tenant access
        var existingAsset = await context.Assets.FindAsync(asset.Id);
        if (existingAsset == null)
            throw new InvalidOperationException("Asset not found");

        if (!isSuperAdmin && tenantId.HasValue && existingAsset.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this asset");

        asset.ModifiedDate = DateTime.Now;
        if (string.IsNullOrEmpty(asset.Name))
            throw new ArgumentException("Asset Name is required");
        
        context.Assets.Update(asset);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAssetAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var asset = await context.Assets.FindAsync(id);
        if (asset == null)
            return;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this asset");

        asset.IsRetired = true;
        asset.IsActive = false;
        asset.Status = "Retired";
        asset.RetirementDate = DateTime.Now;
        context.Assets.Update(asset);
        await context.SaveChangesAsync();
    }

    public async Task RetireAssetAsync(int id, string reason = "")
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var asset = await context.Assets.FindAsync(id);
        if (asset == null)
            return;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this asset");

        asset.IsRetired = true;
        asset.IsActive = false;
        asset.Status = "Retired";
        asset.RetirementDate = DateTime.Now;
        context.Assets.Update(asset);
        await context.SaveChangesAsync();
    }

    public async Task ReactivateAssetAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var asset = await context.Assets.FindAsync(id);
        if (asset == null)
            return;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this asset");

        asset.IsRetired = false;
        asset.IsActive = true;
        asset.Status = "Healthy";
        context.Assets.Update(asset);
        await context.SaveChangesAsync();
    }

    public async Task<List<Asset>> GetCriticalAssetsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Where(a => !a.IsRetired && (a.Status == "Critical" || a.Criticality == "Critical"))
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.HealthScore)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetAssetsNeedingMaintenanceAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Where(a => !a.IsRetired && a.IsActive && 
                       a.NextScheduledMaintenance.HasValue && 
                       a.NextScheduledMaintenance.Value <= DateTime.Now.AddDays(7))
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.NextScheduledMaintenance)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetOverdueMaintenanceAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Where(a => !a.IsRetired && a.IsActive &&
                       a.NextScheduledMaintenance.HasValue && 
                       a.NextScheduledMaintenance.Value < DateTime.Now)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.NextScheduledMaintenance)
            .ToListAsync();
    }

    public async Task<List<Asset>> SearchAssetsAsync(string searchTerm)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var term = searchTerm.ToLower();
        var query = context.Assets
            .Where(a => !a.IsRetired &&
                       (a.AssetId.ToLower().Contains(term) ||
                        a.Name.ToLower().Contains(term) ||
                        a.Location.ToLower().Contains(term) ||
                        a.ModelNumber.ToLower().Contains(term) ||
                        a.SerialNumber.ToLower().Contains(term)))
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.AssetId)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetAssetsByDepartmentAsync(string department)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Where(a => !a.IsRetired && a.Department == department)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.AssetId)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetAssetsByLocationAsync(string location)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Where(a => !a.IsRetired && a.Location == location)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.AssetId)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetAssetsByCriticalityAsync(string criticality)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Where(a => !a.IsRetired && a.Criticality == criticality)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.AssetId)
            .ToListAsync();
    }

    public async Task<int> GetTotalAssetsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets.Where(a => !a.IsRetired);

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query.CountAsync();
    }

    public async Task<int> GetActiveAssetsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets.Where(a => !a.IsRetired && a.IsActive);

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query.CountAsync();
    }

    public async Task<int> GetRetiredAssetsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets.Where(a => a.IsRetired);

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query.CountAsync();
    }

    public async Task<int> GetCriticalAssetsCountAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets.Where(a => !a.IsRetired && 
                                        (a.Status == "Critical" || a.Criticality == "Critical"));

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query.CountAsync();
    }

    public async Task<int> GetOverdueMaintenanceCountAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets.Where(a => !a.IsRetired && a.IsActive &&
                                        a.NextScheduledMaintenance.HasValue && 
                                        a.NextScheduledMaintenance.Value < DateTime.Now);

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query.CountAsync();
    }

    public async Task<double> GetAverageHealthScoreAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets.Where(a => !a.IsRetired);

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        var assets = await query.ToListAsync();
        return assets.Any() ? assets.Average(a => a.HealthScore) : 0;
    }

    public async Task<List<Asset>> GetCriticalAssetsListAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Where(a => !a.IsRetired && a.Status == "Critical")
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.HealthScore)
            .Take(5)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetLowHealthScoreAssetsAsync(double threshold = 60)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.Assets
            .Where(a => !a.IsRetired && a.HealthScore < threshold)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query
            .OrderBy(a => a.HealthScore)
            .ToListAsync();
    }

    // ==================== Condition Reading Methods ====================

    public async Task<List<ConditionReading>> GetConditionReadingsAsync(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var asset = await context.Assets.FindAsync(assetId);
        if (asset != null && !isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this asset");

        return await context.ConditionReadings
            .Where(cr => cr.AssetId == assetId)
            .OrderByDescending(cr => cr.ReadingDate)
            .ToListAsync();
    }

    public async Task AddConditionReadingAsync(ConditionReading reading)
    {
        using var context = _contextFactory.CreateDbContext();
        reading.ReadingDate = DateTime.Now;
        
        var asset = await context.Assets.FindAsync(reading.AssetId);
        if (asset != null)
        {
            reading.TenantId = asset.TenantId;
        }

        context.ConditionReadings.Add(reading);
        await context.SaveChangesAsync();
    }

    // ==================== Failure Mode Methods ====================

    public async Task<List<FailureMode>> GetFailureModesAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.FailureModes
            .Include(fm => fm.Asset)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(fm => fm.TenantId == tenantId);
        }

        var results = await query.ToListAsync();
        return results.OrderByDescending(fm => fm.RPN).ToList();
    }

    public async Task<List<FailureMode>> GetFailureModesAsync(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var asset = await context.Assets.FindAsync(assetId);
        if (asset != null && !isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this asset");

        var results = await context.FailureModes
            .Where(fm => fm.AssetId == assetId)
            .ToListAsync();

        return results.OrderByDescending(fm => fm.RPN).ToList();
    }

    public async Task AddFailureModeAsync(FailureMode failureMode)
    {
        using var context = _contextFactory.CreateDbContext();
        failureMode.CreatedDate = DateTime.Now;
        
        var asset = await context.Assets.FindAsync(failureMode.AssetId);
        if (asset != null)
        {
            failureMode.TenantId = asset.TenantId;
        }

        context.FailureModes.Add(failureMode);
        await context.SaveChangesAsync();
    }

    public async Task UpdateFailureModeAsync(FailureMode failureMode)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var existing = await context.FailureModes.FindAsync(failureMode.Id);
        if (existing != null && !isSuperAdmin && tenantId.HasValue && existing.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this failure mode");

        failureMode.ModifiedDate = DateTime.Now;
        context.FailureModes.Update(failureMode);
        await context.SaveChangesAsync();
    }

    public async Task DeleteFailureModeAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var failureMode = await context.FailureModes.FindAsync(id);
        if (failureMode != null)
        {
            if (!isSuperAdmin && tenantId.HasValue && failureMode.TenantId != tenantId)
                throw new UnauthorizedAccessException("You do not have access to this failure mode");

            context.FailureModes.Remove(failureMode);
            await context.SaveChangesAsync();
        }
    }

    // ==================== Work Order Methods ====================

    public async Task<List<WorkOrder>> GetWorkOrdersAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.WorkOrders
            .Include(wo => wo.Asset)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(wo => wo.TenantId == tenantId);
        }

        var results = await query.ToListAsync();
        return results.OrderByDescending(wo => wo.WorkOrderId).ToList();
    }

    public async Task<WorkOrder?> GetWorkOrderAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.WorkOrders
            .Include(wo => wo.Asset)
            .Include(wo => wo.MaintenanceTasks)
            .Where(wo => wo.Id == id)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(wo => wo.TenantId == tenantId);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task AddWorkOrderAsync(WorkOrder workOrder)
    {
        using var context = _contextFactory.CreateDbContext();
        
        var tenantId = await GetCurrentTenantIdAsync();
        if (!workOrder.TenantId.HasValue && tenantId.HasValue)
        {
            workOrder.TenantId = tenantId;
        }

        if (string.IsNullOrEmpty(workOrder.WorkOrderId))
        {
            var maxId = context.WorkOrders.Any() 
                ? context.WorkOrders.Max(w => w.Id) 
                : 0;
            workOrder.WorkOrderId = $"WO-2024-{(maxId + 1):000}";
        }
        
        workOrder.CreatedDate = DateTime.Now;
        context.WorkOrders.Add(workOrder);
        await context.SaveChangesAsync();
    }

    public async Task UpdateWorkOrderAsync(WorkOrder workOrder)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var existing = await context.WorkOrders.FindAsync(workOrder.Id);
        if (existing != null && !isSuperAdmin && tenantId.HasValue && existing.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        if (workOrder.Status == "Completed" && !workOrder.CompletedDate.HasValue)
        {
            workOrder.CompletedDate = DateTime.Now;
        }
        
        context.WorkOrders.Update(workOrder);
        await context.SaveChangesAsync();
    }

    public async Task DeleteWorkOrderAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(id);
        if (workOrder != null)
        {
            if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
                throw new UnauthorizedAccessException("You do not have access to this work order");

            context.WorkOrders.Remove(workOrder);
            await context.SaveChangesAsync();
        }
    }

    // Keep old sync methods for backward compatibility, but they delegate to async versions
    public List<Asset> GetAssets() => GetAssetsAsync().GetAwaiter().GetResult();
    public List<Asset> GetAllAssets(bool includeRetired = false) => GetAllAssetsAsync(includeRetired).GetAwaiter().GetResult();
    public Asset? GetAsset(int id) => GetAssetAsync(id).GetAwaiter().GetResult();
    public Asset? GetAssetByAssetId(string assetId) => GetAssetByAssetIdAsync(assetId).GetAwaiter().GetResult();
    public void AddAsset(Asset asset) => AddAssetAsync(asset).GetAwaiter().GetResult();
    public void UpdateAsset(Asset asset) => UpdateAssetAsync(asset).GetAwaiter().GetResult();
    public void DeleteAsset(int id) => DeleteAssetAsync(id).GetAwaiter().GetResult();
    public void RetireAsset(int id, string reason = "") => RetireAssetAsync(id, reason).GetAwaiter().GetResult();
    public void ReactivateAsset(int id) => ReactivateAssetAsync(id).GetAwaiter().GetResult();
    public List<Asset> GetCriticalAssets() => GetCriticalAssetsAsync().GetAwaiter().GetResult();
    public List<Asset> GetAssetsNeedingMaintenance() => GetAssetsNeedingMaintenanceAsync().GetAwaiter().GetResult();
    public List<Asset> GetOverdueMaintenance() => GetOverdueMaintenanceAsync().GetAwaiter().GetResult();
    public List<Asset> SearchAssets(string searchTerm) => SearchAssetsAsync(searchTerm).GetAwaiter().GetResult();
    public List<Asset> GetAssetsByDepartment(string department) => GetAssetsByDepartmentAsync(department).GetAwaiter().GetResult();
    public List<Asset> GetAssetsByLocation(string location) => GetAssetsByLocationAsync(location).GetAwaiter().GetResult();
    public List<Asset> GetAssetsByCriticality(string criticality) => GetAssetsByCriticalityAsync(criticality).GetAwaiter().GetResult();
    public int GetTotalAssets() => GetTotalAssetsAsync().GetAwaiter().GetResult();
    public int GetActiveAssets() => GetActiveAssetsAsync().GetAwaiter().GetResult();
    public int GetRetiredAssets() => GetRetiredAssetsAsync().GetAwaiter().GetResult();
    public int GetCriticalAssetsCount() => GetCriticalAssetsCountAsync().GetAwaiter().GetResult();
    public int GetOverdueMaintenanceCount() => GetOverdueMaintenanceCountAsync().GetAwaiter().GetResult();
    public double GetAverageHealthScore() => GetAverageHealthScoreAsync().GetAwaiter().GetResult();
    public List<Asset> GetCriticalAssetsList() => GetCriticalAssetsListAsync().GetAwaiter().GetResult();
    public List<Asset> GetLowHealthScoreAssets(double threshold = 60) => GetLowHealthScoreAssetsAsync(threshold).GetAwaiter().GetResult();
    public List<ConditionReading> GetConditionReadings(int assetId) => GetConditionReadingsAsync(assetId).GetAwaiter().GetResult();
    public void AddConditionReading(ConditionReading reading) => AddConditionReadingAsync(reading).GetAwaiter().GetResult();
    public List<FailureMode> GetFailureModes() => GetFailureModesAsync().GetAwaiter().GetResult();
    public List<FailureMode> GetFailureModes(int assetId) => GetFailureModesAsync(assetId).GetAwaiter().GetResult();
    public void AddFailureMode(FailureMode failureMode) => AddFailureModeAsync(failureMode).GetAwaiter().GetResult();
    public void UpdateFailureMode(FailureMode failureMode) => UpdateFailureModeAsync(failureMode).GetAwaiter().GetResult();
    public void DeleteFailureMode(int id) => DeleteFailureModeAsync(id).GetAwaiter().GetResult();
    public List<WorkOrder> GetWorkOrders() => GetWorkOrdersAsync().GetAwaiter().GetResult();
    public WorkOrder? GetWorkOrder(int id) => GetWorkOrderAsync(id).GetAwaiter().GetResult();
    public void AddWorkOrder(WorkOrder workOrder) => AddWorkOrderAsync(workOrder).GetAwaiter().GetResult();
    public void UpdateWorkOrder(WorkOrder workOrder) => UpdateWorkOrderAsync(workOrder).GetAwaiter().GetResult();
    public void DeleteWorkOrder(int id) => DeleteWorkOrderAsync(id).GetAwaiter().GetResult();

    // ==================== Async Spare Parts Methods ====================

    public async Task<List<SparePart>> GetSparePartsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.SpareParts
            .Include(sp => sp.Asset)
            .OrderBy(sp => sp.PartNumber)
            .ToListAsync();
    }

    public async Task<List<SparePart>> GetGenericSparePartsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.SpareParts
            .Where(sp => sp.IsGeneric)
            .OrderBy(sp => sp.PartNumber)
            .ToListAsync();
    }

    public async Task<List<SparePart>> GetAssetSpecificSparePartsAsync(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.SpareParts
            .Include(sp => sp.Asset)
            .Where(sp => sp.AssetId == assetId)
            .OrderBy(sp => sp.PartNumber)
            .ToListAsync();
    }

    public async Task<List<SparePart>> GetLowStockPartsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.SpareParts
            .Include(sp => sp.Asset)
            .Where(sp => sp.QuantityInStock <= sp.ReorderPoint)
            .OrderBy(sp => sp.QuantityInStock)
            .ToListAsync();
    }

    public async Task<SparePart?> GetSparePartAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.SpareParts
            .Include(sp => sp.Asset)
            .Include(sp => sp.Transactions)
            .FirstOrDefaultAsync(sp => sp.Id == id);
    }

    public async Task AddSparePartAsync(SparePart sparePart)
    {
        using var context = _contextFactory.CreateDbContext();
        sparePart.CreatedDate = DateTime.Now;
        sparePart.Status = sparePart.StockStatus;
        context.SpareParts.Add(sparePart);
        await context.SaveChangesAsync();
    }

    public async Task UpdateSparePartAsync(SparePart sparePart)
    {
        using var context = _contextFactory.CreateDbContext();
        sparePart.ModifiedDate = DateTime.Now;
        sparePart.Status = sparePart.StockStatus;
        context.SpareParts.Update(sparePart);
        await context.SaveChangesAsync();
    }

    public async Task DeleteSparePartAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var sparePart = await context.SpareParts.FindAsync(id);
        if (sparePart != null)
        {
            context.SpareParts.Remove(sparePart);
            await context.SaveChangesAsync();
        }
    }

    // ==================== Async Spare Part Transaction Methods ====================

    public async Task<List<SparePartTransaction>> GetSparePartTransactionsAsync(int sparePartId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.SparePartTransactions
            .Include(spt => spt.WorkOrder)
            .Include(spt => spt.Asset)
            .Where(spt => spt.SparePartId == sparePartId)
            .OrderByDescending(spt => spt.TransactionDate)
            .ToListAsync();
    }

    public async Task<List<SparePartTransaction>> GetRecentTransactionsAsync(int count = 10)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.SparePartTransactions
            .Include(spt => spt.SparePart)
            .Include(spt => spt.WorkOrder)
            .Include(spt => spt.Asset)
            .OrderByDescending(spt => spt.TransactionDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddSparePartTransactionAsync(SparePartTransaction transaction)
    {
        using var context = _contextFactory.CreateDbContext();
        
        var sparePart = await context.SpareParts.FindAsync(transaction.SparePartId);
        if (sparePart != null)
        {
            transaction.StockBefore = sparePart.QuantityInStock;
            
            // Adjust stock based on transaction type
            if (transaction.TransactionType == "Issue")
            {
                sparePart.QuantityInStock -= transaction.Quantity;
                sparePart.LastUsedDate = DateTime.Now;
            }
            else if (transaction.TransactionType == "Return")
            {
                sparePart.QuantityInStock += transaction.Quantity;
            }
            else if (transaction.TransactionType == "Restock")
            {
                sparePart.QuantityInStock += transaction.Quantity;
                sparePart.LastRestockDate = DateTime.Now;
            }
            else if (transaction.TransactionType == "Adjustment")
            {
                sparePart.QuantityInStock = transaction.Quantity;
            }
            
            transaction.StockAfter = sparePart.QuantityInStock;
            transaction.TotalCost = transaction.Quantity * (transaction.UnitCostAtTransaction ?? sparePart.UnitCost);
            
            sparePart.Status = sparePart.StockStatus;
            
            context.SparePartTransactions.Add(transaction);
            await context.SaveChangesAsync();
        }
    }

    // ==================== Async Document Methods ====================

    public async Task<List<Document>> GetDocumentsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Documents
            .Include(d => d.Asset)
            .Include(d => d.WorkOrder)
            .Include(d => d.FailureMode)
            .OrderByDescending(d => d.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Document>> GetDocumentsByCategoryAsync(string category)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Documents
            .Include(d => d.Asset)
            .Where(d => d.Category == category)
            .OrderByDescending(d => d.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Document>> GetDocumentsByWorkOrderAsync(int workOrderId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Documents
            .Include(d => d.WorkOrder)
            .Where(d => d.WorkOrderId == workOrderId)
            .OrderByDescending(d => d.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Document>> GetExpiredDocumentsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Documents
            .Include(d => d.Asset)
            .Where(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value < DateTime.Now)
            .OrderBy(d => d.ExpiryDate)
            .ToListAsync();
    }

    public async Task<List<Document>> GetDocumentsNeedingReviewAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Documents
            .Include(d => d.Asset)
            .Where(d => d.ReviewDate.HasValue && d.ReviewDate.Value < DateTime.Now)
            .OrderBy(d => d.ReviewDate)
            .ToListAsync();
    }

    public async Task<Document?> GetDocumentAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Documents
            .Include(d => d.Asset)
            .Include(d => d.WorkOrder)
            .Include(d => d.FailureMode)
            .Include(d => d.AccessLogs)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task AddDocumentAsync(Document document)
    {
        using var context = _contextFactory.CreateDbContext();
        document.CreatedDate = DateTime.Now;
        context.Documents.Add(document);
        await context.SaveChangesAsync();
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        using var context = _contextFactory.CreateDbContext();
        document.ModifiedDate = DateTime.Now;
        context.Documents.Update(document);
        await context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var document = await context.Documents.FindAsync(id);
        if (document != null)
        {
            // Delete physical file if exists
            if (File.Exists(document.FilePath))
            {
                File.Delete(document.FilePath);
            }
            
            context.Documents.Remove(document);
            await context.SaveChangesAsync();
        }
    }

    public async Task LogDocumentAccessAsync(DocumentAccessLog log)
    {
        using var context = _contextFactory.CreateDbContext();
        context.DocumentAccessLogs.Add(log);
        
        // Update document view/download count
        var document = await context.Documents.FindAsync(log.DocumentId);
        if (document != null)
        {
            if (log.ActionType == "View")
            {
                document.ViewCount++;
            }
            else if (log.ActionType == "Download")
            {
                document.DownloadCount++;
            }
        }
        
        await context.SaveChangesAsync();
    }

    public async Task<List<DocumentAccessLog>> GetDocumentAccessLogsAsync(int documentId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.DocumentAccessLogs
            .Where(dal => dal.DocumentId == documentId)
            .OrderByDescending(dal => dal.AccessDate)
            .Take(50)
            .ToListAsync();
    }

    // ==================== Async Reliability Metrics Methods ====================

    public async Task<List<ReliabilityMetric>> GetReliabilityMetricsAsync(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.ReliabilityMetrics
            .Where(rm => rm.AssetId == assetId)
            .OrderByDescending(rm => rm.MetricDate)
            .ToListAsync();
    }

    public async Task AddReliabilityMetricAsync(ReliabilityMetric metric)
    {
        using var context = _contextFactory.CreateDbContext();
        metric.CalculatedDate = DateTime.Now;
        context.ReliabilityMetrics.Add(metric);
        await context.SaveChangesAsync();
    }

    // ==================== Async Asset Downtime Methods ====================

    public async Task<List<AssetDowntime>> GetAssetDowntimeAsync(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.AssetDowntime
            .Where(ad => ad.AssetId == assetId)
            .OrderByDescending(ad => ad.StartTime)
            .ToListAsync();
    }

    public async Task<List<AssetDowntime>> GetRecentDowntimeEventsAsync(int count = 20)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.AssetDowntime
            .Include(ad => ad.Asset)
            .Include(ad => ad.RelatedWorkOrder)
            .OrderByDescending(ad => ad.StartTime)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddAssetDowntimeAsync(AssetDowntime downtime)
    {
        using var context = _contextFactory.CreateDbContext();
        downtime.RecordedDate = DateTime.Now;
        context.AssetDowntime.Add(downtime);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAssetDowntimeAsync(AssetDowntime downtime)
    {
        using var context = _contextFactory.CreateDbContext();
        context.AssetDowntime.Update(downtime);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAssetDowntimeAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var downtime = await context.AssetDowntime.FindAsync(id);
        if (downtime != null)
        {
            context.AssetDowntime.Remove(downtime);
            await context.SaveChangesAsync();
        }
    }

    // ==================== User Methods ====================

    public async Task<List<User>> GetUsersAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Users
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public List<User> GetUsers() => GetUsersAsync().GetAwaiter().GetResult();

    // ==================== MaintenanceSchedule Methods ====================

    public async Task<List<MaintenanceSchedule>> GetSchedulesAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.MaintenanceSchedules
            .Include(ms => ms.Asset)
            .OrderByDescending(ms => ms.ScheduledDate)
            .ToListAsync();
    }

    public async Task AddScheduleAsync(MaintenanceSchedule schedule)
    {
        using var context = _contextFactory.CreateDbContext();
        schedule.CreatedDate = DateTime.Now;
        context.MaintenanceSchedules.Add(schedule);
        await context.SaveChangesAsync();
    }

    public async Task UpdateScheduleAsync(MaintenanceSchedule schedule)
    {
        using var context = _contextFactory.CreateDbContext();
        schedule.CreatedDate = schedule.CreatedDate;
        context.MaintenanceSchedules.Update(schedule);
        await context.SaveChangesAsync();
    }

    public async Task DeleteScheduleAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var schedule = await context.MaintenanceSchedules.FindAsync(id);
        if (schedule != null)
        {
            context.MaintenanceSchedules.Remove(schedule);
            await context.SaveChangesAsync();
        }
    }

    public List<MaintenanceSchedule> GetSchedules() => GetSchedulesAsync().GetAwaiter().GetResult();
    public void AddSchedule(MaintenanceSchedule schedule) => AddScheduleAsync(schedule).GetAwaiter().GetResult();
    public void UpdateSchedule(MaintenanceSchedule schedule) => UpdateScheduleAsync(schedule).GetAwaiter().GetResult();
    public void DeleteSchedule(int id) => DeleteScheduleAsync(id).GetAwaiter().GetResult();

    // ==================== Additional Methods ====================

    public async Task<List<Document>> GetDocumentsByAssetAsync(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Documents
            .Include(d => d.Asset)
            .Where(d => d.AssetId == assetId)
            .OrderByDescending(d => d.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetAllWorkOrdersAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.WorkOrders
            .Include(wo => wo.Asset)
            .OrderByDescending(wo => wo.CreatedDate)
            .ToListAsync();
    }

    public int GetLowStockCount()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.SpareParts
            .Count(sp => sp.QuantityInStock <= sp.ReorderPoint);
    }

    public decimal GetTotalInventoryValue()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.SpareParts
            .AsEnumerable()
            .Sum(sp => (decimal)(sp.QuantityInStock * sp.UnitCost));
    }

    public int GetTotalDocuments()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Documents.Count();
    }

    public int GetExpiredDocumentsCount()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Documents
            .Count(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value < DateTime.Now);
    }

    public int GetDocumentsNeedingReviewCount()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Documents
            .Count(d => d.ReviewDate.HasValue && d.ReviewDate.Value < DateTime.Now);
    }
}
