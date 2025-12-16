using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorApp1.Services;

public class WorkOrderService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly CurrentUserService _currentUserService;
    private readonly RolePermissionService _rolePermissionService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WorkOrderService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        CurrentUserService currentUserService,
        RolePermissionService rolePermissionService,
        IHttpContextAccessor httpContextAccessor)
    {
        _contextFactory = contextFactory;
        _currentUserService = currentUserService;
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

    // ==================== Work Order CRUD Operations ====================

    public async Task<WorkOrder?> GetWorkOrderAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders
            .Include(wo => wo.Asset)
            .Include(wo => wo.MaintenanceTasks)
            .Include(wo => wo.SparesUsed)
                .ThenInclude(su => su.SparePart)
            .FirstOrDefaultAsync(wo => wo.Id == id);

        if (workOrder == null)
            return null;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        return workOrder;
    }

    public async Task<List<WorkOrder>> GetAllWorkOrdersAsync()
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

        return await query
            .OrderByDescending(wo => wo.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetWorkOrdersByStatusAsync(string status)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.WorkOrders
            .Include(wo => wo.Asset)
            .Where(wo => wo.Status == status)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(wo => wo.TenantId == tenantId);
        }

        return await query
            .OrderByDescending(wo => wo.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetWorkOrdersByAssetAsync(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        // Verify asset access
        var asset = await context.Assets.FindAsync(assetId);
        if (asset != null && !isSuperAdmin && tenantId.HasValue && asset.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this asset");

        var query = context.WorkOrders
            .Include(wo => wo.Asset)
            .Where(wo => wo.AssetId == assetId)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(wo => wo.TenantId == tenantId);
        }

        return await query
            .OrderByDescending(wo => wo.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetOverdueWorkOrdersAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.WorkOrders
            .Include(wo => wo.Asset)
            .Where(wo => wo.DueDate < DateTime.Now && wo.Status != "Completed" && wo.Status != "Cancelled")
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(wo => wo.TenantId == tenantId);
        }

        return await query
            .OrderBy(wo => wo.DueDate)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetPendingApprovalAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var query = context.WorkOrders
            .Include(wo => wo.Asset)
            .Where(wo => wo.Status == "Requested" || wo.Status == "Pending Approval")
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(wo => wo.TenantId == tenantId);
        }

        return await query
            .OrderByDescending(wo => wo.RequestedDate)
            .ToListAsync();
    }

    // ==================== Work Order Creation ====================

    public async Task<WorkOrder> CreateWorkOrderAsync(WorkOrder workOrder)
    {
        using var context = _contextFactory.CreateDbContext();
        var tenantId = await GetCurrentTenantIdAsync();

        // Set tenant for non-SuperAdmin users
        if (!workOrder.TenantId.HasValue && tenantId.HasValue)
        {
            workOrder.TenantId = tenantId;
        }

        // Generate Work Order ID if not provided
        if (string.IsNullOrEmpty(workOrder.WorkOrderId))
        {
            workOrder.WorkOrderId = await GenerateWorkOrderIdAsync(context);
        }

        // Set creation details
        workOrder.CreatedDate = DateTime.Now;
        workOrder.RequestedBy = _currentUserService.UserName;
        workOrder.RequestedDate = DateTime.Now;

        // Set default status if not provided
        if (string.IsNullOrEmpty(workOrder.Status))
        {
            workOrder.Status = "Open";
        }

        context.WorkOrders.Add(workOrder);
        await context.SaveChangesAsync();

        return workOrder;
    }

    // ==================== Work Order Status Updates ====================

    public async Task<bool> ApproveWorkOrderAsync(int id, string approvalNotes = "")
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(id);

        if (workOrder == null || workOrder.Status != "Pending Approval")
            return false;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        workOrder.Status = "Approved";
        workOrder.ApprovedBy = _currentUserService.UserName;
        workOrder.ApprovedDate = DateTime.Now;
        workOrder.ApprovalNotes = approvalNotes;
        workOrder.LastModifiedBy = _currentUserService.UserName;
        workOrder.LastModifiedDate = DateTime.Now;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectWorkOrderAsync(int id, string rejectionReason)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(id);

        if (workOrder == null)
            return false;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        workOrder.Status = "Rejected";
        workOrder.RejectedBy = _currentUserService.UserName;
        workOrder.RejectedDate = DateTime.Now;
        workOrder.RejectionReason = rejectionReason;
        workOrder.LastModifiedBy = _currentUserService.UserName;
        workOrder.LastModifiedDate = DateTime.Now;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> StartWorkOrderAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(id);

        if (workOrder == null)
            return false;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        workOrder.Status = "In Progress";
        workOrder.StartedDate = DateTime.Now;
        workOrder.TimeSubmitted = DateTime.Now;
        workOrder.AcknowledgedBy = _currentUserService.UserName;
        workOrder.AcknowledgedDate = DateTime.Now;
        workOrder.IsAcknowledged = true;
        workOrder.LastModifiedBy = _currentUserService.UserName;
        workOrder.LastModifiedDate = DateTime.Now;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HoldWorkOrderAsync(int id, string reason = "")
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(id);

        if (workOrder == null || workOrder.Status != "In Progress")
            return false;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        workOrder.Status = "On Hold";
        workOrder.LastModifiedBy = _currentUserService.UserName;
        workOrder.LastModifiedDate = DateTime.Now;
        
        if (!string.IsNullOrEmpty(reason))
        {
            workOrder.AnyOtherDetails += $"\n[{DateTime.Now:yyyy-MM-dd HH:mm}] Put on hold: {reason}";
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResumeWorkOrderAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(id);

        if (workOrder == null || workOrder.Status != "On Hold")
            return false;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        workOrder.Status = "In Progress";
        workOrder.LastModifiedBy = _currentUserService.UserName;
        workOrder.LastModifiedDate = DateTime.Now;
        workOrder.AnyOtherDetails += $"\n[{DateTime.Now:yyyy-MM-dd HH:mm}] Work resumed";

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteWorkOrderAsync(int id, WorkOrderCompletionData completionData)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(id);

        if (workOrder == null || workOrder.Status == "Completed")
            return false;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        workOrder.Status = "Completed";
        workOrder.CompletedDate = DateTime.Now;
        workOrder.TimeDone = DateTime.Now;
        workOrder.TimeCompleted = DateTime.Now;
        
        workOrder.DetailsOfWorkCarriedOut = completionData.WorkCarriedOut;
        workOrder.CorrectiveAction = completionData.CorrectiveAction;
        workOrder.CompletionNotes = completionData.CompletionNotes;
        workOrder.ActualDowntime = completionData.ActualDowntime;
        workOrder.ActualCost = completionData.ActualCost;
        workOrder.LaborHours = completionData.LaborHours;
        
        workOrder.LastModifiedBy = _currentUserService.UserName;
        workOrder.LastModifiedDate = DateTime.Now;

        if (completionData.UpdateAssetStatus && completionData.NewAssetStatus != null)
        {
            var asset = await context.Assets.FindAsync(workOrder.AssetId);
            if (asset != null)
            {
                asset.Status = completionData.NewAssetStatus;
                asset.ModifiedDate = DateTime.Now;
            }
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelWorkOrderAsync(int id, string reason)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(id);

        if (workOrder == null)
            return false;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        workOrder.Status = "Cancelled";
        workOrder.LastModifiedBy = _currentUserService.UserName;
        workOrder.LastModifiedDate = DateTime.Now;
        workOrder.AnyOtherDetails += $"\n[{DateTime.Now:yyyy-MM-dd HH:mm}] Cancelled: {reason}";

        await context.SaveChangesAsync();
        return true;
    }

    // ==================== Spare Parts Management ====================

    public async Task<bool> AddSparePartUsageAsync(int workOrderId, WorkOrderSpareUsed spareUsage)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(workOrderId);
        if (workOrder == null)
            return false;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");
        
        spareUsage.WorkOrderId = workOrderId;
        context.WorkOrderSparesUsed.Add(spareUsage);

        if (spareUsage.SparePartId.HasValue)
        {
            var sparePart = await context.SpareParts.FindAsync(spareUsage.SparePartId.Value);
            if (sparePart != null)
            {
                sparePart.QuantityInStock -= spareUsage.Quantity;
                sparePart.LastUsedDate = DateTime.Now;
                sparePart.Status = sparePart.StockStatus;
            }
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<WorkOrderSpareUsed>> GetSparePartsUsedAsync(int workOrderId)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(workOrderId);
        if (workOrder != null && !isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        return await context.WorkOrderSparesUsed
            .Include(su => su.SparePart)
            .Where(su => su.WorkOrderId == workOrderId)
            .ToListAsync();
    }

    // ==================== Assignment ====================

    public async Task<bool> AssignWorkOrderAsync(int id, string assignedTo)
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var workOrder = await context.WorkOrders.FindAsync(id);

        if (workOrder == null)
            return false;

        // Verify tenant access
        if (!isSuperAdmin && tenantId.HasValue && workOrder.TenantId != tenantId)
            throw new UnauthorizedAccessException("You do not have access to this work order");

        workOrder.AssignedTo = assignedTo;
        workOrder.LastModifiedBy = _currentUserService.UserName;
        workOrder.LastModifiedDate = DateTime.Now;

        await context.SaveChangesAsync();
        return true;
    }

    // ==================== Statistics & Analytics ====================

    public async Task<WorkOrderStatistics> GetStatisticsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var isSuperAdmin = await IsSuperAdminAsync();
        var tenantId = await GetCurrentTenantIdAsync();

        var now = DateTime.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        // Build base query with tenant filtering
        var baseQuery = context.WorkOrders.AsQueryable();
        if (!isSuperAdmin && tenantId.HasValue)
        {
            baseQuery = baseQuery.Where(wo => wo.TenantId == tenantId);
        }

        var stats = new WorkOrderStatistics
        {
            Total = await baseQuery.CountAsync(),
            Open = await baseQuery.CountAsync(wo => wo.Status == "Open"),
            InProgress = await baseQuery.CountAsync(wo => wo.Status == "In Progress"),
            Completed = await baseQuery.CountAsync(wo => wo.Status == "Completed"),
            Overdue = await baseQuery.CountAsync(wo => 
                wo.DueDate < now && wo.Status != "Completed" && wo.Status != "Cancelled"),
            PendingApproval = await baseQuery.CountAsync(wo => 
                wo.Status == "Requested" || wo.Status == "Pending Approval"),
            CompletedThisMonth = await baseQuery.CountAsync(wo => 
                wo.Status == "Completed" && 
                wo.CompletedDate.HasValue && 
                wo.CompletedDate.Value >= monthStart),
            AverageCompletionTime = await GetAverageCompletionTimeAsync(context, isSuperAdmin, tenantId),
            TotalCostThisMonth = await GetTotalCostThisMonthAsync(context, monthStart, isSuperAdmin, tenantId)
        };

        return stats;
    }

    private async Task<double> GetAverageCompletionTimeAsync(ApplicationDbContext context, bool isSuperAdmin, int? tenantId)
    {
        var query = context.WorkOrders
            .Where(wo => wo.Status == "Completed" && wo.CompletedDate.HasValue)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(wo => wo.TenantId == tenantId);
        }

        var completedOrders = await query.ToListAsync();

        if (!completedOrders.Any())
            return 0;

        return completedOrders
            .Average(wo => (wo.CompletedDate!.Value - wo.CreatedDate).TotalHours);
    }

    private async Task<decimal> GetTotalCostThisMonthAsync(ApplicationDbContext context, DateTime monthStart, bool isSuperAdmin, int? tenantId)
    {
        var query = context.WorkOrders
            .Where(wo => wo.CreatedDate >= monthStart && wo.ActualCost.HasValue)
            .AsQueryable();

        if (!isSuperAdmin && tenantId.HasValue)
        {
            query = query.Where(wo => wo.TenantId == tenantId);
        }

        return await query.SumAsync(wo => wo.ActualCost!.Value);
    }

    // ==================== Helper Methods ====================

    private async Task<string> GenerateWorkOrderIdAsync(ApplicationDbContext context)
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        
        var count = await context.WorkOrders
            .Where(wo => wo.CreatedDate.Year == year && wo.CreatedDate.Month == month)
            .CountAsync();

        return $"WO-{year:0000}{month:00}-{(count + 1):0000}";
    }
}

// Supporting Classes
public class WorkOrderCompletionData
{
    public string WorkCarriedOut { get; set; } = string.Empty;
    public string CorrectiveAction { get; set; } = string.Empty;
    public string CompletionNotes { get; set; } = string.Empty;
    public double ActualDowntime { get; set; }
    public decimal ActualCost { get; set; }
    public double LaborHours { get; set; }
    public bool UpdateAssetStatus { get; set; }
    public string? NewAssetStatus { get; set; }
}

public class WorkOrderStatistics
{
    public int Total { get; set; }
    public int Open { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Overdue { get; set; }
    public int PendingApproval { get; set; }
    public int CompletedThisMonth { get; set; }
    public double AverageCompletionTime { get; set; }
    public decimal TotalCostThisMonth { get; set; }
}
