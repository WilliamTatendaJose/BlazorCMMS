using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BlazorApp1.Services;

/// <summary>
/// Custom exception for data service operations
/// </summary>
public class DataServiceException : Exception
{
    public string Operation { get; }
    public string? EntityType { get; }
    public int? EntityId { get; }

    public DataServiceException(string message, string operation, string? entityType = null, int? entityId = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Operation = operation;
        EntityType = entityType;
        EntityId = entityId;
    }
}

/// <summary>
/// Exception thrown when a user attempts to access data they don't have permission for
/// </summary>
public class TenantAccessDeniedException : DataServiceException
{
    public int? RequestedTenantId { get; }
    public int? UserTenantId { get; }

    public TenantAccessDeniedException(string entityType, int? entityId, int? requestedTenantId, int? userTenantId)
        : base($"Access denied to {entityType} with ID {entityId}. User tenant: {userTenantId}, Resource tenant: {requestedTenantId}",
               "TenantAccess", entityType, entityId)
    {
        RequestedTenantId = requestedTenantId;
        UserTenantId = userTenantId;
    }
}

/// <summary>
/// Exception thrown when a requested entity is not found
/// </summary>
public class EntityNotFoundException : DataServiceException
{
    public EntityNotFoundException(string entityType, int entityId)
        : base($"{entityType} with ID {entityId} was not found.", "Find", entityType, entityId)
    {
    }

    public EntityNotFoundException(string entityType, string identifier)
        : base($"{entityType} with identifier '{identifier}' was not found.", "Find", entityType)
    {
    }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : DataServiceException
{
    public Dictionary<string, string> ValidationErrors { get; }

    public ValidationException(string message, Dictionary<string, string>? errors = null)
        : base(message, "Validation")
    {
        ValidationErrors = errors ?? new Dictionary<string, string>();
    }
}

public class DataService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RolePermissionService _rolePermissionService;
    private readonly ILogger<DataService> _logger;

    public DataService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        IHttpContextAccessor httpContextAccessor,
        RolePermissionService rolePermissionService,
        ILogger<DataService> logger)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _rolePermissionService = rolePermissionService ?? throw new ArgumentNullException(nameof(rolePermissionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ==================== Helper Methods ====================

    private async Task<int?> GetCurrentTenantIdAsync()
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogDebug("No authenticated user found when getting tenant ID");
                return null;
            }

            using var context = _contextFactory.CreateDbContext();
            var appUser = await context.Set<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == userId);
            
            if (appUser == null)
            {
                _logger.LogWarning("User {UserId} not found in database", userId);
                return null;
            }

            return appUser.PrimaryTenantId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current tenant ID");
            return null;
        }
    }

    private async Task<bool> IsSuperAdminAsync()
    {
        try
        {
            return await _rolePermissionService.IsSuperAdminAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking SuperAdmin status");
            return false;
        }
    }

    private void ValidateNotNull<T>(T? entity, string entityName) where T : class
    {
        if (entity == null)
        {
            throw new ArgumentNullException(entityName, $"{entityName} cannot be null");
        }
    }

    private async Task<(bool IsSuperAdmin, int? TenantId)> GetTenantContextAsync()
    {
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();
        return (isSuperAdmin, tenantId);
    }

    private void ThrowIfTenantAccessDenied(int? entityTenantId, int? userTenantId, bool isSuperAdmin, string entityType, int? entityId = null)
    {
        if (!isSuperAdmin && userTenantId.HasValue && entityTenantId != userTenantId)
        {
            _logger.LogWarning("Tenant access denied: User tenant {UserTenant} tried to access {EntityType} ID {EntityId} belonging to tenant {EntityTenant}",
                userTenantId, entityType, entityId, entityTenantId);
            throw new TenantAccessDeniedException(entityType, entityId, entityTenantId, userTenantId);
        }
    }

    // ==================== Asset Methods ====================

    public async Task<List<Asset>> GetAssetsAsync()
    {
        try
        {
            _logger.LogDebug("Fetching assets list");
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.Assets
                .Where(a => !a.IsRetired)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(a => a.TenantId == tenantId);
            }

            var assets = await query.OrderBy(a => a.AssetId).ToListAsync();
            _logger.LogDebug("Retrieved {Count} assets", assets.Count);
            
            return assets;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assets list");
            throw new DataServiceException("Failed to retrieve assets list", "GetAssets", "Asset", innerException: ex);
        }
    }

    public async Task<List<Asset>> GetAllAssetsAsync(bool includeRetired = false)
    {
        try
        {
            _logger.LogDebug("Fetching all assets (includeRetired: {IncludeRetired})", includeRetired);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.Assets.AsQueryable();
            
            if (!includeRetired)
            {
                query = query.Where(a => !a.IsRetired);
            }

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(a => a.TenantId == tenantId);
            }

            var assets = await query.OrderBy(a => a.AssetId).ToListAsync();
            _logger.LogDebug("Retrieved {Count} assets (all)", assets.Count);
            
            return assets;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all assets");
            throw new DataServiceException("Failed to retrieve all assets", "GetAllAssets", "Asset", innerException: ex);
        }
    }

    public async Task<Asset?> GetAssetAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching asset with ID {AssetId}", id);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var asset = await context.Assets
                .Include(a => a.ConditionReadings)
                .Include(a => a.WorkOrders)
                .Include(a => a.FailureModes)
                .Include(a => a.ReliabilityMetrics)
                .Include(a => a.Attachments)
                .Include(a => a.DowntimeRecords)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null)
            {
                _logger.LogDebug("Asset with ID {AssetId} not found", id);
                return null;
            }

            // Check tenant access
            if (!isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
            {
                _logger.LogWarning("Tenant access denied for asset {AssetId}", id);
                return null;
            }

            return asset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching asset with ID {AssetId}", id);
            throw new DataServiceException($"Failed to retrieve asset with ID {id}", "GetAsset", "Asset", id, ex);
        }
    }

    public async Task<Asset?> GetAssetByAssetIdAsync(string assetId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assetId))
            {
                _logger.LogWarning("GetAssetByAssetIdAsync called with null or empty assetId");
                return null;
            }

            _logger.LogDebug("Fetching asset with AssetId {AssetId}", assetId);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching asset with AssetId {AssetId}", assetId);
            throw new DataServiceException($"Failed to retrieve asset with AssetId '{assetId}'", "GetAssetByAssetId", "Asset", innerException: ex);
        }
    }

    public async Task AddAssetAsync(Asset asset)
    {
        try
        {
            ValidateNotNull(asset, nameof(asset));
            
            // Validate required fields
            var validationErrors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(asset.AssetId))
                validationErrors.Add("AssetId", "Asset ID is required");
            if (string.IsNullOrWhiteSpace(asset.Name))
                validationErrors.Add("Name", "Asset Name is required");
            
            if (validationErrors.Count > 0)
                throw new ValidationException("Asset validation failed", validationErrors);

            _logger.LogInformation("Adding new asset: {AssetId} - {AssetName}", asset.AssetId, asset.Name);
            
            using var context = _contextFactory.CreateDbContext();
            
            // Check for duplicate AssetId
            var existingAsset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == asset.AssetId);
            if (existingAsset != null)
            {
                throw new ValidationException($"An asset with ID '{asset.AssetId}' already exists", 
                    new Dictionary<string, string> { { "AssetId", "Asset ID must be unique" } });
            }
            
            // Set tenant for non-SuperAdmin users
            var tenantId = await GetCurrentTenantIdAsync();
            if (!asset.TenantId.HasValue && tenantId.HasValue)
            {
                asset.TenantId = tenantId;
            }

            asset.CreatedDate = DateTime.Now;
            asset.IsActive = true;
            asset.IsRetired = false;
            
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully added asset: {AssetId} with database ID {Id}", asset.AssetId, asset.Id);
        }
        catch (ValidationException)
        {
            throw; // Re-throw validation exceptions as-is
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while adding asset {AssetId}", asset?.AssetId);
            throw new DataServiceException("Failed to save asset to database. The asset ID may already exist.", 
                "AddAsset", "Asset", innerException: ex);
        }
        catch (Exception ex) when (ex is not DataServiceException)
        {
            _logger.LogError(ex, "Error adding asset {AssetId}", asset?.AssetId);
            throw new DataServiceException("Failed to add asset", "AddAsset", "Asset", innerException: ex);
        }
    }

    public async Task UpdateAssetAsync(Asset asset)
    {
        try
        {
            ValidateNotNull(asset, nameof(asset));
            
            if (string.IsNullOrWhiteSpace(asset.Name))
                throw new ValidationException("Asset Name is required", 
                    new Dictionary<string, string> { { "Name", "Asset Name is required" } });

            _logger.LogInformation("Updating asset ID {AssetId}", asset.Id);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            // Verify asset exists
            var existingAsset = await context.Assets.AsNoTracking().FirstOrDefaultAsync(a => a.Id == asset.Id);
            if (existingAsset == null)
                throw new EntityNotFoundException("Asset", asset.Id);

            // Verify tenant access
            ThrowIfTenantAccessDenied(existingAsset.TenantId, tenantId, isSuperAdmin, "Asset", asset.Id);

            // Preserve tenant ID
            asset.TenantId = existingAsset.TenantId;
            asset.ModifiedDate = DateTime.Now;
            
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully updated asset ID {AssetId}", asset.Id);
        }
        catch (Exception ex) when (ex is not DataServiceException and not ValidationException and not TenantAccessDeniedException and not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error updating asset ID {AssetId}", asset?.Id);
            throw new DataServiceException($"Failed to update asset with ID {asset?.Id}", "UpdateAsset", "Asset", asset?.Id, ex);
        }
    }

    public async Task DeleteAssetAsync(int id)
    {
        try
        {
            _logger.LogInformation("Soft-deleting (retiring) asset ID {AssetId}", id);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var asset = await context.Assets.FindAsync(id);
            if (asset == null)
            {
                _logger.LogWarning("Attempted to delete non-existent asset ID {AssetId}", id);
                return;
            }

            // Verify tenant access
            ThrowIfTenantAccessDenied(asset.TenantId, tenantId, isSuperAdmin, "Asset", id);

            asset.IsRetired = true;
            asset.IsActive = false;
            asset.Status = "Retired";
            asset.RetirementDate = DateTime.Now;
            asset.ModifiedDate = DateTime.Now;
            
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully retired asset ID {AssetId}", id);
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException)
        {
            _logger.LogError(ex, "Error deleting asset ID {AssetId}", id);
            throw new DataServiceException($"Failed to delete asset with ID {id}", "DeleteAsset", "Asset", id, ex);
        }
    }

    public async Task RetireAssetAsync(int id, string reason = "")
    {
        try
        {
            _logger.LogInformation("Retiring asset ID {AssetId}. Reason: {Reason}", id, reason);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var asset = await context.Assets.FindAsync(id);
            if (asset == null)
            {
                _logger.LogWarning("Attempted to retire non-existent asset ID {AssetId}", id);
                throw new EntityNotFoundException("Asset", id);
            }

            // Verify tenant access
            ThrowIfTenantAccessDenied(asset.TenantId, tenantId, isSuperAdmin, "Asset", id);

            asset.IsRetired = true;
            asset.IsActive = false;
            asset.Status = "Retired";
            asset.RetirementDate = DateTime.Now;
            asset.ModifiedDate = DateTime.Now;
            
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully retired asset ID {AssetId}", id);
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException and not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error retiring asset ID {AssetId}", id);
            throw new DataServiceException($"Failed to retire asset with ID {id}", "RetireAsset", "Asset", id, ex);
        }
    }

    public async Task ReactivateAssetAsync(int id)
    {
        try
        {
            _logger.LogInformation("Reactivating asset ID {AssetId}", id);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var asset = await context.Assets.FindAsync(id);
            if (asset == null)
            {
                _logger.LogWarning("Attempted to reactivate non-existent asset ID {AssetId}", id);
                throw new EntityNotFoundException("Asset", id);
            }

            // Verify tenant access
            ThrowIfTenantAccessDenied(asset.TenantId, tenantId, isSuperAdmin, "Asset", id);

            asset.IsRetired = false;
            asset.IsActive = true;
            asset.Status = "Healthy";
            asset.RetirementDate = null;
            asset.ModifiedDate = DateTime.Now;
            
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully reactivated asset ID {AssetId}", id);
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException and not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error reactivating asset ID {AssetId}", id);
            throw new DataServiceException($"Failed to reactivate asset with ID {id}", "ReactivateAsset", "Asset", id, ex);
        }
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
        try
        {
            _logger.LogDebug("Fetching work orders list");
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.WorkOrders
                .Include(wo => wo.Asset)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(wo => wo.TenantId == tenantId);
            }

            var results = await query.ToListAsync();
            _logger.LogDebug("Retrieved {Count} work orders", results.Count);
            
            return results.OrderByDescending(wo => wo.WorkOrderId).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching work orders list");
            throw new DataServiceException("Failed to retrieve work orders", "GetWorkOrders", "WorkOrder", innerException: ex);
        }
    }

    public async Task<WorkOrder?> GetWorkOrderAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching work order with ID {WorkOrderId}", id);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.WorkOrders
                .Include(wo => wo.Asset)
                .Include(wo => wo.MaintenanceTasks)
                .Where(wo => wo.Id == id)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(wo => wo.TenantId == tenantId);
            }

            var workOrder = await query.FirstOrDefaultAsync();
            
            if (workOrder == null)
            {
                _logger.LogDebug("Work order with ID {WorkOrderId} not found", id);
            }
            
            return workOrder;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching work order with ID {WorkOrderId}", id);
            throw new DataServiceException($"Failed to retrieve work order with ID {id}", "GetWorkOrder", "WorkOrder", id, ex);
        }
    }

    public async Task AddWorkOrderAsync(WorkOrder workOrder)
    {
        try
        {
            ValidateNotNull(workOrder, nameof(workOrder));
            
            _logger.LogInformation("Adding new work order");
            
            using var context = _contextFactory.CreateDbContext();
            
            var tenantId = await GetCurrentTenantIdAsync();
            if (!workOrder.TenantId.HasValue && tenantId.HasValue)
            {
                workOrder.TenantId = tenantId;
            }

            // Generate WorkOrderId if not provided
            if (string.IsNullOrEmpty(workOrder.WorkOrderId))
            {
                var year = DateTime.Now.Year;
                var maxId = await context.WorkOrders.AnyAsync() 
                    ? await context.WorkOrders.MaxAsync(w => w.Id) 
                    : 0;
                workOrder.WorkOrderId = $"WO-{year}-{(maxId + 1):0000}";
                _logger.LogDebug("Generated work order ID: {WorkOrderId}", workOrder.WorkOrderId);
            }
            
            workOrder.CreatedDate = DateTime.Now;
            context.WorkOrders.Add(workOrder);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully added work order {WorkOrderId} with database ID {Id}", 
                workOrder.WorkOrderId, workOrder.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while adding work order");
            throw new DataServiceException("Failed to save work order to database", "AddWorkOrder", "WorkOrder", innerException: ex);
        }
        catch (Exception ex) when (ex is not DataServiceException)
        {
            _logger.LogError(ex, "Error adding work order");
            throw new DataServiceException("Failed to add work order", "AddWorkOrder", "WorkOrder", innerException: ex);
        }
    }

    public async Task UpdateWorkOrderAsync(WorkOrder workOrder)
    {
        try
        {
            ValidateNotNull(workOrder, nameof(workOrder));
            
            _logger.LogInformation("Updating work order ID {WorkOrderId}", workOrder.Id);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var existing = await context.WorkOrders.AsNoTracking().FirstOrDefaultAsync(wo => wo.Id == workOrder.Id);
            if (existing == null)
                throw new EntityNotFoundException("WorkOrder", workOrder.Id);

            // Verify tenant access
            ThrowIfTenantAccessDenied(existing.TenantId, tenantId, isSuperAdmin, "WorkOrder", workOrder.Id);

            // Preserve tenant ID
            workOrder.TenantId = existing.TenantId;
            
            // Auto-set completed date if status changed to Completed
            if (workOrder.Status == "Completed" && !workOrder.CompletedDate.HasValue)
            {
                workOrder.CompletedDate = DateTime.Now;
                _logger.LogDebug("Auto-set completed date for work order {WorkOrderId}", workOrder.Id);
            }

            workOrder.LastModifiedDate = DateTime.Now;
            
            context.WorkOrders.Update(workOrder);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully updated work order ID {WorkOrderId}", workOrder.Id);
        }
        catch (Exception ex) when (ex is not DataServiceException and not TenantAccessDeniedException and not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error updating work order ID {WorkOrderId}", workOrder?.Id);
            throw new DataServiceException($"Failed to update work order with ID {workOrder?.Id}", "UpdateWorkOrder", "WorkOrder", workOrder?.Id, ex);
        }
    }

    public async Task DeleteWorkOrderAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting work order ID {WorkOrderId}", id);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var workOrder = await context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                _logger.LogWarning("Attempted to delete non-existent work order ID {WorkOrderId}", id);
                return;
            }

            // Verify tenant access
            ThrowIfTenantAccessDenied(workOrder.TenantId, tenantId, isSuperAdmin, "WorkOrder", id);

            context.WorkOrders.Remove(workOrder);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully deleted work order ID {WorkOrderId}", id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while deleting work order ID {WorkOrderId}. It may have related records.", id);
            throw new DataServiceException(
                "Failed to delete work order. It may have related maintenance tasks or other records that must be deleted first.", 
                "DeleteWorkOrder", "WorkOrder", id, ex);
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException)
        {
            _logger.LogError(ex, "Error deleting work order ID {WorkOrderId}", id);
            throw new DataServiceException($"Failed to delete work order with ID {id}", "DeleteWorkOrder", "WorkOrder", id, ex);
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
        try
        {
            _logger.LogDebug("Fetching spare parts list");
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.SpareParts
                .Include(sp => sp.Asset)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(sp => sp.TenantId == tenantId);
            }

            var parts = await query.OrderBy(sp => sp.PartNumber).ToListAsync();
            _logger.LogDebug("Retrieved {Count} spare parts", parts.Count);
            
            return parts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching spare parts list");
            throw new DataServiceException("Failed to retrieve spare parts", "GetSpareParts", "SparePart", innerException: ex);
        }
    }

    public async Task<List<SparePart>> GetGenericSparePartsAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.SpareParts
                .Where(sp => sp.IsGeneric)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(sp => sp.TenantId == tenantId);
            }

            return await query.OrderBy(sp => sp.PartNumber).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching generic spare parts");
            throw new DataServiceException("Failed to retrieve generic spare parts", "GetGenericSpareParts", "SparePart", innerException: ex);
        }
    }

    public async Task<List<SparePart>> GetAssetSpecificSparePartsAsync(int assetId)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.SpareParts
                .Include(sp => sp.Asset)
                .Where(sp => sp.AssetId == assetId)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(sp => sp.TenantId == tenantId);
            }

            return await query.OrderBy(sp => sp.PartNumber).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching spare parts for asset {AssetId}", assetId);
            throw new DataServiceException($"Failed to retrieve spare parts for asset {assetId}", "GetAssetSpecificSpareParts", "SparePart", innerException: ex);
        }
    }

    public async Task<List<SparePart>> GetLowStockPartsAsync()
    {
        try
        {
            _logger.LogDebug("Fetching low stock spare parts");
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.SpareParts
                .Include(sp => sp.Asset)
                .Where(sp => sp.QuantityInStock <= sp.ReorderPoint)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(sp => sp.TenantId == tenantId);
            }

            return await query.OrderBy(sp => sp.QuantityInStock).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching low stock spare parts");
            throw new DataServiceException("Failed to retrieve low stock spare parts", "GetLowStockParts", "SparePart", innerException: ex);
        }
    }

    public async Task<SparePart?> GetSparePartAsync(int id)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var sparePart = await context.SpareParts
                .Include(sp => sp.Asset)
                .Include(sp => sp.Transactions)
                .FirstOrDefaultAsync(sp => sp.Id == id);

            if (sparePart != null && !isSuperAdmin && tenantId.HasValue && sparePart.TenantId != tenantId)
            {
                _logger.LogWarning("Tenant access denied for spare part {SparePartId}", id);
                return null;
            }

            return sparePart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching spare part with ID {SparePartId}", id);
            throw new DataServiceException($"Failed to retrieve spare part with ID {id}", "GetSparePart", "SparePart", id, ex);
        }
    }

    public async Task AddSparePartAsync(SparePart sparePart)
    {
        try
        {
            ValidateNotNull(sparePart, nameof(sparePart));
            
            // Validate required fields
            var validationErrors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(sparePart.PartNumber))
                validationErrors.Add("PartNumber", "Part Number is required");
            if (string.IsNullOrWhiteSpace(sparePart.Name))
                validationErrors.Add("Name", "Part Name is required");
            
            if (validationErrors.Count > 0)
                throw new ValidationException("Spare part validation failed", validationErrors);

            _logger.LogInformation("Adding new spare part: {PartNumber} - {Name}", sparePart.PartNumber, sparePart.Name);
            
            using var context = _contextFactory.CreateDbContext();
            
            // Check for duplicate part number
            var existingPart = await context.SpareParts.FirstOrDefaultAsync(sp => sp.PartNumber == sparePart.PartNumber);
            if (existingPart != null)
            {
                throw new ValidationException($"A spare part with part number '{sparePart.PartNumber}' already exists", 
                    new Dictionary<string, string> { { "PartNumber", "Part number must be unique" } });
            }
            
            var tenantId = await GetCurrentTenantIdAsync();
            if (!sparePart.TenantId.HasValue && tenantId.HasValue)
            {
                sparePart.TenantId = tenantId;
            }

            sparePart.CreatedDate = DateTime.Now;
            sparePart.Status = sparePart.StockStatus;
            context.SpareParts.Add(sparePart);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully added spare part: {PartNumber} with ID {Id}", sparePart.PartNumber, sparePart.Id);
        }
        catch (ValidationException)
        {
            throw; // Re-throw validation exceptions as-is
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while adding spare part {PartNumber}", sparePart?.PartNumber);
            throw new DataServiceException("Failed to save spare part to database. The part number may already exist.", 
                "AddSparePart", "SparePart", innerException: ex);
        }
        catch (Exception ex) when (ex is not DataServiceException)
        {
            _logger.LogError(ex, "Error adding spare part {PartNumber}", sparePart?.PartNumber);
            throw new DataServiceException("Failed to add spare part", "AddSparePart", "SparePart", innerException: ex);
        }
    }

    public async Task UpdateSparePartAsync(SparePart sparePart)
    {
        try
        {
            ValidateNotNull(sparePart, nameof(sparePart));
            
            _logger.LogInformation("Updating spare part ID {SparePartId}", sparePart.Id);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var existing = await context.SpareParts.AsNoTracking().FirstOrDefaultAsync(sp => sp.Id == sparePart.Id);
            if (existing == null)
                throw new EntityNotFoundException("SparePart", sparePart.Id);

            // Verify tenant access
            ThrowIfTenantAccessDenied(existing.TenantId, tenantId, isSuperAdmin, "SparePart", sparePart.Id);

            // Preserve tenant ID
            sparePart.TenantId = existing.TenantId;
            sparePart.ModifiedDate = DateTime.Now;
            sparePart.Status = sparePart.StockStatus;
            
            context.SpareParts.Update(sparePart);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully updated spare part ID {SparePartId}", sparePart.Id);
        }
        catch (Exception ex) when (ex is not DataServiceException and not TenantAccessDeniedException and not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error updating spare part ID {SparePartId}", sparePart?.Id);
            throw new DataServiceException($"Failed to update spare part with ID {sparePart?.Id}", "UpdateSparePart", "SparePart", sparePart?.Id, ex);
        }
    }

    public async Task DeleteSparePartAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting spare part ID {SparePartId}", id);
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var sparePart = await context.SpareParts.FindAsync(id);
            if (sparePart == null)
            {
                _logger.LogWarning("Attempted to delete non-existent spare part ID {SparePartId}", id);
                return;
            }

            // Verify tenant access
            ThrowIfTenantAccessDenied(sparePart.TenantId, tenantId, isSuperAdmin, "SparePart", id);

            // Check for related transactions
            var hasTransactions = await context.SparePartTransactions.AnyAsync(t => t.SparePartId == id);
            if (hasTransactions)
            {
                throw new DataServiceException(
                    "Cannot delete spare part because it has transaction history. Consider marking it as inactive instead.", 
                    "DeleteSparePart", "SparePart", id);
            }

            context.SpareParts.Remove(sparePart);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully deleted spare part ID {SparePartId}", id);
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException and not DataServiceException)
        {
            _logger.LogError(ex, "Error deleting spare part ID {SparePartId}", id);
            throw new DataServiceException($"Failed to delete spare part with ID {id}", "DeleteSparePart", "SparePart", id, ex);
        }
    }

    // ==================== Async Spare Part Transaction Methods ====================

    public async Task<List<SparePartTransaction>> GetSparePartTransactionsAsync(int sparePartId)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.SparePartTransactions
                .Include(spt => spt.WorkOrder)
                .Include(spt => spt.Asset)
                .Where(spt => spt.SparePartId == sparePartId)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(spt => spt.TenantId == tenantId);
            }

            return await query.OrderByDescending(spt => spt.TransactionDate).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching transactions for spare part {SparePartId}", sparePartId);
            throw new DataServiceException($"Failed to retrieve transactions for spare part {sparePartId}", 
                "GetSparePartTransactions", "SparePartTransaction", innerException: ex);
        }
    }

    public async Task<List<SparePartTransaction>> GetRecentTransactionsAsync(int count = 10)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.SparePartTransactions
                .Include(spt => spt.SparePart)
                .Include(spt => spt.WorkOrder)
                .Include(spt => spt.Asset)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(spt => spt.TenantId == tenantId);
            }

            return await query
                .OrderByDescending(spt => spt.TransactionDate)
                .Take(count)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent transactions");
            throw new DataServiceException("Failed to retrieve recent transactions", 
                "GetRecentTransactions", "SparePartTransaction", innerException: ex);
        }
    }

    public async Task AddSparePartTransactionAsync(SparePartTransaction transaction)
    {
        try
        {
            ValidateNotNull(transaction, nameof(transaction));
            
            _logger.LogInformation("Adding spare part transaction for part {SparePartId}", transaction.SparePartId);
            
            using var context = _contextFactory.CreateDbContext();
            
            var sparePart = await context.SpareParts.FindAsync(transaction.SparePartId);
            if (sparePart == null)
                throw new EntityNotFoundException("SparePart", transaction.SparePartId);

            // Inherit tenant from spare part
            transaction.TenantId = sparePart.TenantId;
            transaction.StockBefore = sparePart.QuantityInStock;
            
            // Adjust stock based on transaction type
            switch (transaction.TransactionType)
            {
                case "Issue":
                    if (sparePart.QuantityInStock < transaction.Quantity)
                        throw new ValidationException($"Insufficient stock. Available: {sparePart.QuantityInStock}, Requested: {transaction.Quantity}");
                    sparePart.QuantityInStock -= transaction.Quantity;
                    sparePart.LastUsedDate = DateTime.Now;
                    break;
                case "Return":
                    sparePart.QuantityInStock += transaction.Quantity;
                    break;
                case "Restock":
                    sparePart.QuantityInStock += transaction.Quantity;
                    sparePart.LastRestockDate = DateTime.Now;
                    break;
                case "Adjustment":
                    sparePart.QuantityInStock = transaction.Quantity;
                    break;
            }
            
            transaction.StockAfter = sparePart.QuantityInStock;
            transaction.TotalCost = transaction.Quantity * (transaction.UnitCostAtTransaction ?? sparePart.UnitCost);
            transaction.TransactionDate = DateTime.Now;
            
            sparePart.Status = sparePart.StockStatus;
            
            context.SparePartTransactions.Add(transaction);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully added transaction for spare part {SparePartId}", transaction.SparePartId);
        }
        catch (Exception ex) when (ex is not DataServiceException and not ValidationException and not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error adding spare part transaction");
            throw new DataServiceException("Failed to add spare part transaction", 
                "AddSparePartTransaction", "SparePartTransaction", innerException: ex);
        }
    }

    // ==================== Async Document Methods ====================

    public async Task<List<Document>> GetDocumentsAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.Documents
                .Include(d => d.Asset)
                .Include(d => d.WorkOrder)
                .Include(d => d.FailureMode)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(d => d.TenantId == tenantId);
            }

            return await query.OrderByDescending(d => d.CreatedDate).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching documents");
            throw new DataServiceException("Failed to retrieve documents", "GetDocuments", "Document", innerException: ex);
        }
    }

    public async Task<Document?> GetDocumentAsync(int id)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var document = await context.Documents
                .Include(d => d.Asset)
                .Include(d => d.WorkOrder)
                .Include(d => d.FailureMode)
                .Include(d => d.AccessLogs)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document != null && !isSuperAdmin && tenantId.HasValue && document.TenantId != tenantId)
                return null;

            return document;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching document {DocumentId}", id);
            throw new DataServiceException($"Failed to retrieve document with ID {id}", "GetDocument", "Document", id, ex);
        }
    }

    public async Task AddDocumentAsync(Document document)
    {
        try
        {
            ValidateNotNull(document, nameof(document));
            
            _logger.LogInformation("Adding new document: {Title}", document.Title);
            
            using var context = _contextFactory.CreateDbContext();
            
            var tenantId = await GetCurrentTenantIdAsync();
            if (!document.TenantId.HasValue && tenantId.HasValue)
            {
                document.TenantId = tenantId;
            }

            document.CreatedDate = DateTime.Now;
            context.Documents.Add(document);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully added document with ID {DocumentId}", document.Id);
        }
        catch (Exception ex) when (ex is not DataServiceException)
        {
            _logger.LogError(ex, "Error adding document");
            throw new DataServiceException("Failed to add document", "AddDocument", "Document", innerException: ex);
        }
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        try
        {
            ValidateNotNull(document, nameof(document));
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var existing = await context.Documents.AsNoTracking().FirstOrDefaultAsync(d => d.Id == document.Id);
            if (existing == null)
                throw new EntityNotFoundException("Document", document.Id);

            ThrowIfTenantAccessDenied(existing.TenantId, tenantId, isSuperAdmin, "Document", document.Id);

            document.TenantId = existing.TenantId;
            document.ModifiedDate = DateTime.Now;
            
            context.Documents.Update(document);
            await context.SaveChangesAsync();
        }
        catch (Exception ex) when (ex is not DataServiceException and not TenantAccessDeniedException and not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error updating document {DocumentId}", document?.Id);
            throw new DataServiceException($"Failed to update document with ID {document?.Id}", "UpdateDocument", "Document", document?.Id, ex);
        }
    }

    public async Task DeleteDocumentAsync(int id)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var document = await context.Documents.FindAsync(id);
            if (document == null)
                return;

            ThrowIfTenantAccessDenied(document.TenantId, tenantId, isSuperAdmin, "Document", id);

            // Delete physical file if exists
            if (!string.IsNullOrEmpty(document.FilePath) && File.Exists(document.FilePath))
            {
                try
                {
                    File.Delete(document.FilePath);
                    _logger.LogDebug("Deleted physical file: {FilePath}", document.FilePath);
                }
                catch (Exception fileEx)
                {
                    _logger.LogWarning(fileEx, "Failed to delete physical file {FilePath}", document.FilePath);
                }
            }
            
            context.Documents.Remove(document);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully deleted document {DocumentId}", id);
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", id);
            throw new DataServiceException($"Failed to delete document with ID {id}", "DeleteDocument", "Document", id, ex);
        }
    }

    // ==================== MaintenanceSchedule Methods ====================

    public async Task<List<MaintenanceSchedule>> GetSchedulesAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.MaintenanceSchedules
                .Include(ms => ms.Asset)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(ms => ms.TenantId == tenantId);
            }

            return await query.OrderByDescending(ms => ms.ScheduledDate).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching maintenance schedules");
            throw new DataServiceException("Failed to retrieve maintenance schedules", "GetSchedules", "MaintenanceSchedule", innerException: ex);
        }
    }

    public async Task AddScheduleAsync(MaintenanceSchedule schedule)
    {
        try
        {
            ValidateNotNull(schedule, nameof(schedule));
            
            using var context = _contextFactory.CreateDbContext();
            
            var tenantId = await GetCurrentTenantIdAsync();
            if (!schedule.TenantId.HasValue && tenantId.HasValue)
            {
                schedule.TenantId = tenantId;
            }

            schedule.CreatedDate = DateTime.Now;
            context.MaintenanceSchedules.Add(schedule);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Added maintenance schedule with ID {ScheduleId}", schedule.Id);
        }
        catch (Exception ex) when (ex is not DataServiceException)
        {
            _logger.LogError(ex, "Error adding maintenance schedule");
            throw new DataServiceException("Failed to add maintenance schedule", "AddSchedule", "MaintenanceSchedule", innerException: ex);
        }
    }

    public async Task UpdateScheduleAsync(MaintenanceSchedule schedule)
    {
        try
        {
            ValidateNotNull(schedule, nameof(schedule));
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var existing = await context.MaintenanceSchedules.AsNoTracking().FirstOrDefaultAsync(s => s.Id == schedule.Id);
            if (existing == null)
                throw new EntityNotFoundException("MaintenanceSchedule", schedule.Id);

            ThrowIfTenantAccessDenied(existing.TenantId, tenantId, isSuperAdmin, "MaintenanceSchedule", schedule.Id);

            schedule.TenantId = existing.TenantId;
            context.MaintenanceSchedules.Update(schedule);
            await context.SaveChangesAsync();
        }
        catch (Exception ex) when (ex is not DataServiceException and not TenantAccessDeniedException and not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error updating maintenance schedule {ScheduleId}", schedule?.Id);
            throw new DataServiceException($"Failed to update maintenance schedule with ID {schedule?.Id}", 
                "UpdateSchedule", "MaintenanceSchedule", schedule?.Id, ex);
        }
    }

    public async Task DeleteScheduleAsync(int id)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var schedule = await context.MaintenanceSchedules.FindAsync(id);
            if (schedule == null)
                return;

            ThrowIfTenantAccessDenied(schedule.TenantId, tenantId, isSuperAdmin, "MaintenanceSchedule", id);

            context.MaintenanceSchedules.Remove(schedule);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted maintenance schedule {ScheduleId}", id);
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException)
        {
            _logger.LogError(ex, "Error deleting maintenance schedule {ScheduleId}", id);
            throw new DataServiceException($"Failed to delete maintenance schedule with ID {id}", 
                "DeleteSchedule", "MaintenanceSchedule", id, ex);
        }
    }

    // ==================== Sync Wrapper Methods for Backward Compatibility ====================
    
    public List<MaintenanceSchedule> GetSchedules() => GetSchedulesAsync().GetAwaiter().GetResult();
    public void AddSchedule(MaintenanceSchedule schedule) => AddScheduleAsync(schedule).GetAwaiter().GetResult();
    public void UpdateSchedule(MaintenanceSchedule schedule) => UpdateScheduleAsync(schedule).GetAwaiter().GetResult();
    public void DeleteSchedule(int id) => DeleteScheduleAsync(id).GetAwaiter().GetResult();

    // ==================== Utility Count Methods ====================

    public async Task<int> GetLowStockCountAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.SpareParts
                .Where(sp => sp.QuantityInStock <= sp.ReorderPoint)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(sp => sp.TenantId == tenantId);
            }

            return await query.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock count");
            return 0;
        }
    }

    public int GetLowStockCount() => GetLowStockCountAsync().GetAwaiter().GetResult();

    public async Task<decimal> GetTotalInventoryValueAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.SpareParts.AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(sp => sp.TenantId == tenantId);
            }

            var parts = await query.ToListAsync();
            return parts.Sum(sp => (decimal)(sp.QuantityInStock * sp.UnitCost));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total inventory value");
            return 0;
        }
    }

    public decimal GetTotalInventoryValue() => GetTotalInventoryValueAsync().GetAwaiter().GetResult();

    public async Task<int> GetTotalDocumentsAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.Documents.AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(d => d.TenantId == tenantId);
            }

            return await query.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total documents count");
            return 0;
        }
    }

    public int GetTotalDocuments() => GetTotalDocumentsAsync().GetAwaiter().GetResult();

    public async Task<int> GetExpiredDocumentsCountAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.Documents
                .Where(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value < DateTime.Now)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(d => d.TenantId == tenantId);
            }

            return await query.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired documents count");
            return 0;
        }
    }

    public int GetExpiredDocumentsCount() => GetExpiredDocumentsCountAsync().GetAwaiter().GetResult();

    public async Task<int> GetDocumentsNeedingReviewCountAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.Documents
                .Where(d => d.ReviewDate.HasValue && d.ReviewDate.Value < DateTime.Now)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(d => d.TenantId == tenantId);
            }

            return await query.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents needing review count");
            return 0;
        }
    }

    public int GetDocumentsNeedingReviewCount() => GetDocumentsNeedingReviewCountAsync().GetAwaiter().GetResult();

    // ==================== Additional Document Methods ====================

    public async Task<List<Document>> GetDocumentsByAssetAsync(int assetId)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.Documents
                .Include(d => d.Asset)
                .Where(d => d.AssetId == assetId)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(d => d.TenantId == tenantId);
            }

            return await query.OrderByDescending(d => d.CreatedDate).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching documents for asset {AssetId}", assetId);
            throw new DataServiceException($"Failed to retrieve documents for asset {assetId}", 
                "GetDocumentsByAsset", "Document", innerException: ex);
        }
    }

    public async Task LogDocumentAccessAsync(DocumentAccessLog log)
    {
        try
        {
            ValidateNotNull(log, nameof(log));
            
            using var context = _contextFactory.CreateDbContext();
            
            // Inherit tenant from document
            var document = await context.Documents.FindAsync(log.DocumentId);
            if (document != null)
            {
                log.TenantId = document.TenantId;
                
                if (log.ActionType == "View")
                    document.ViewCount++;
                else if (log.ActionType == "Download")
                    document.DownloadCount++;
            }
            
            log.AccessDate = DateTime.Now;
            context.DocumentAccessLogs.Add(log);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging document access for document {DocumentId}", log?.DocumentId);
            // Don't throw - logging failure shouldn't break the application
        }
    }

    public async Task<List<DocumentAccessLog>> GetDocumentAccessLogsAsync(int documentId)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.DocumentAccessLogs
                .Where(dal => dal.DocumentId == documentId)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(dal => dal.TenantId == tenantId);
            }

            return await query
                .OrderByDescending(dal => dal.AccessDate)
                .Take(50)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching access logs for document {DocumentId}", documentId);
            throw new DataServiceException($"Failed to retrieve access logs for document {documentId}", 
                "GetDocumentAccessLogs", "DocumentAccessLog", innerException: ex);
        }
    }

    // ==================== Reliability Metrics Methods ====================

    public async Task<List<ReliabilityMetric>> GetReliabilityMetricsAsync(int assetId)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            // Verify asset access first
            var asset = await context.Assets.FindAsync(assetId);
            if (asset != null && !isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
                throw new TenantAccessDeniedException("Asset", assetId, asset.TenantId, tenantId);

            return await context.ReliabilityMetrics
                .Where(rm => rm.AssetId == assetId)
                .OrderByDescending(rm => rm.MetricDate)
                .ToListAsync();
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException)
        {
            _logger.LogError(ex, "Error fetching reliability metrics for asset {AssetId}", assetId);
            throw new DataServiceException($"Failed to retrieve reliability metrics for asset {assetId}", 
                "GetReliabilityMetrics", "ReliabilityMetric", innerException: ex);
        }
    }

    public async Task AddReliabilityMetricAsync(ReliabilityMetric metric)
    {
        try
        {
            ValidateNotNull(metric, nameof(metric));
            
            using var context = _contextFactory.CreateDbContext();
            
            // Inherit tenant from asset
            var asset = await context.Assets.FindAsync(metric.AssetId);
            if (asset != null)
            {
                metric.TenantId = asset.TenantId;
            }

            metric.CalculatedDate = DateTime.Now;
            context.ReliabilityMetrics.Add(metric);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Added reliability metric for asset {AssetId}", metric.AssetId);
        }
        catch (Exception ex) when (ex is not DataServiceException)
        {
            _logger.LogError(ex, "Error adding reliability metric");
            throw new DataServiceException("Failed to add reliability metric", 
                "AddReliabilityMetric", "ReliabilityMetric", innerException: ex);
        }
    }

    // ==================== Asset Downtime Methods ====================

    public async Task<List<AssetDowntime>> GetAssetDowntimeAsync(int assetId)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            // Verify asset access first
            var asset = await context.Assets.FindAsync(assetId);
            if (asset != null && !isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
                throw new TenantAccessDeniedException("Asset", assetId, asset.TenantId, tenantId);

            return await context.AssetDowntime
                .Where(ad => ad.AssetId == assetId)
                .OrderByDescending(ad => ad.StartTime)
                .ToListAsync();
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException)
        {
            _logger.LogError(ex, "Error fetching downtime for asset {AssetId}", assetId);
            throw new DataServiceException($"Failed to retrieve downtime records for asset {assetId}", 
                "GetAssetDowntime", "AssetDowntime", innerException: ex);
        }
    }

    public async Task<List<AssetDowntime>> GetRecentDowntimeEventsAsync(int count = 20)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.AssetDowntime
                .Include(ad => ad.Asset)
                .Include(ad => ad.RelatedWorkOrder)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(ad => ad.TenantId == tenantId);
            }

            return await query
                .OrderByDescending(ad => ad.StartTime)
                .Take(count)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent downtime events");
            throw new DataServiceException("Failed to retrieve recent downtime events", 
                "GetRecentDowntimeEvents", "AssetDowntime", innerException: ex);
        }
    }

    public async Task AddAssetDowntimeAsync(AssetDowntime downtime)
    {
        try
        {
            ValidateNotNull(downtime, nameof(downtime));
            
            using var context = _contextFactory.CreateDbContext();
            
            // Inherit tenant from asset
            var asset = await context.Assets.FindAsync(downtime.AssetId);
            if (asset != null)
            {
                downtime.TenantId = asset.TenantId;
            }

            downtime.RecordedDate = DateTime.Now;
            context.AssetDowntime.Add(downtime);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Added downtime record for asset {AssetId}", downtime.AssetId);
        }
        catch (Exception ex) when (ex is not DataServiceException)
        {
            _logger.LogError(ex, "Error adding asset downtime");
            throw new DataServiceException("Failed to add asset downtime", 
                "AddAssetDowntime", "AssetDowntime", innerException: ex);
        }
    }

    public async Task UpdateAssetDowntimeAsync(AssetDowntime downtime)
    {
        try
        {
            ValidateNotNull(downtime, nameof(downtime));
            
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var existing = await context.AssetDowntime.AsNoTracking().FirstOrDefaultAsync(ad => ad.Id == downtime.Id);
            if (existing == null)
                throw new EntityNotFoundException("AssetDowntime", downtime.Id);

            ThrowIfTenantAccessDenied(existing.TenantId, tenantId, isSuperAdmin, "AssetDowntime", downtime.Id);

            downtime.TenantId = existing.TenantId;
            context.AssetDowntime.Update(downtime);
            await context.SaveChangesAsync();
        }
        catch (Exception ex) when (ex is not DataServiceException and not TenantAccessDeniedException and not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error updating asset downtime {DowntimeId}", downtime?.Id);
            throw new DataServiceException($"Failed to update asset downtime with ID {downtime?.Id}", 
                "UpdateAssetDowntime", "AssetDowntime", downtime?.Id, ex);
        }
    }

    public async Task DeleteAssetDowntimeAsync(int id)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var downtime = await context.AssetDowntime.FindAsync(id);
            if (downtime == null)
                return;

            ThrowIfTenantAccessDenied(downtime.TenantId, tenantId, isSuperAdmin, "AssetDowntime", id);

            context.AssetDowntime.Remove(downtime);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted downtime record {DowntimeId}", id);
        }
        catch (Exception ex) when (ex is not TenantAccessDeniedException)
        {
            _logger.LogError(ex, "Error deleting asset downtime {DowntimeId}", id);
            throw new DataServiceException($"Failed to delete asset downtime with ID {id}", 
                "DeleteAssetDowntime", "AssetDowntime", id, ex);
        }
    }

    // ==================== WorkOrder Additional Methods ====================

    public async Task<List<WorkOrder>> GetAllWorkOrdersAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.WorkOrders
                .Include(wo => wo.Asset)
                .AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(wo => wo.TenantId == tenantId);
            }

            return await query.OrderByDescending(wo => wo.CreatedDate).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all work orders");
            throw new DataServiceException("Failed to retrieve all work orders", 
                "GetAllWorkOrders", "WorkOrder", innerException: ex);
        }
    }

    // ==================== User Methods ====================

    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var (isSuperAdmin, tenantId) = await GetTenantContextAsync();

            var query = context.Users.AsQueryable();

            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(u => u.TenantId == tenantId);
            }

            return await query.OrderBy(u => u.Name).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users");
            throw new DataServiceException("Failed to retrieve users", "GetUsers", "User", innerException: ex);
        }
    }

    public List<User> GetUsers() => GetUsersAsync().GetAwaiter().GetResult();
}
