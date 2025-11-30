using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

public class DataService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DataService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // Asset methods
    public List<Asset> GetAssets()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Assets
            .OrderBy(a => a.AssetId)
            .ToList();
    }

    public Asset? GetAsset(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Assets
            .Include(a => a.ConditionReadings)
            .Include(a => a.WorkOrders)
            .Include(a => a.FailureModes)
            .Include(a => a.ReliabilityMetrics)
            .FirstOrDefault(a => a.Id == id);
    }

    public void AddAsset(Asset asset)
    {
        using var context = _contextFactory.CreateDbContext();
        asset.CreatedDate = DateTime.Now;
        context.Assets.Add(asset);
        context.SaveChanges();
    }

    public void UpdateAsset(Asset asset)
    {
        using var context = _contextFactory.CreateDbContext();
        asset.ModifiedDate = DateTime.Now;
        context.Assets.Update(asset);
        context.SaveChanges();
    }

    public void DeleteAsset(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var asset = context.Assets.Find(id);
        if (asset != null)
        {
            context.Assets.Remove(asset);
            context.SaveChanges();
        }
    }

    // Condition Reading methods
    public List<ConditionReading> GetConditionReadings(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.ConditionReadings
            .Where(cr => cr.AssetId == assetId)
            .OrderByDescending(cr => cr.ReadingDate)
            .ToList();
    }

    public void AddConditionReading(ConditionReading reading)
    {
        using var context = _contextFactory.CreateDbContext();
        reading.ReadingDate = DateTime.Now;
        context.ConditionReadings.Add(reading);
        context.SaveChanges();
    }

    // Failure Mode methods
    public List<FailureMode> GetFailureModes()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.FailureModes
            .Include(fm => fm.Asset)
            .OrderByDescending(fm => fm.RPN)
            .ToList();
    }

    public List<FailureMode> GetFailureModes(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.FailureModes
            .Where(fm => fm.AssetId == assetId)
            .OrderByDescending(fm => fm.RPN)
            .ToList();
    }

    public void AddFailureMode(FailureMode failureMode)
    {
        using var context = _contextFactory.CreateDbContext();
        failureMode.CreatedDate = DateTime.Now;
        context.FailureModes.Add(failureMode);
        context.SaveChanges();
    }

    public void UpdateFailureMode(FailureMode failureMode)
    {
        using var context = _contextFactory.CreateDbContext();
        failureMode.ModifiedDate = DateTime.Now;
        context.FailureModes.Update(failureMode);
        context.SaveChanges();
    }

    public void DeleteFailureMode(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var failureMode = context.FailureModes.Find(id);
        if (failureMode != null)
        {
            context.FailureModes.Remove(failureMode);
            context.SaveChanges();
        }
    }

    // Work Order methods
    public List<WorkOrder> GetWorkOrders()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.WorkOrders
            .Include(wo => wo.Asset)
            .OrderByDescending(wo => wo.CreatedDate)
            .ToList();
    }

    public WorkOrder? GetWorkOrder(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.WorkOrders
            .Include(wo => wo.Asset)
            .Include(wo => wo.MaintenanceTasks)
            .FirstOrDefault(wo => wo.Id == id);
    }

    public void AddWorkOrder(WorkOrder workOrder)
    {
        using var context = _contextFactory.CreateDbContext();
        
        // Generate WorkOrderId if not provided
        if (string.IsNullOrEmpty(workOrder.WorkOrderId))
        {
            var maxId = context.WorkOrders.Any() 
                ? context.WorkOrders.Max(w => w.Id) 
                : 0;
            workOrder.WorkOrderId = $"WO-2024-{(maxId + 1):000}";
        }
        
        workOrder.CreatedDate = DateTime.Now;
        context.WorkOrders.Add(workOrder);
        context.SaveChanges();
    }

    public void UpdateWorkOrder(WorkOrder workOrder)
    {
        using var context = _contextFactory.CreateDbContext();
        
        // If marking as completed, set completed date
        if (workOrder.Status == "Completed" && !workOrder.CompletedDate.HasValue)
        {
            workOrder.CompletedDate = DateTime.Now;
        }
        
        context.WorkOrders.Update(workOrder);
        context.SaveChanges();
    }

    public void DeleteWorkOrder(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var workOrder = context.WorkOrders.Find(id);
        if (workOrder != null)
        {
            context.WorkOrders.Remove(workOrder);
            context.SaveChanges();
        }
    }

    // User methods
    public List<User> GetUsers()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Users
            .OrderBy(u => u.Name)
            .ToList();
    }

    public void AddUser(User user)
    {
        using var context = _contextFactory.CreateDbContext();
        user.CreatedDate = DateTime.Now;
        user.IsActive = true;
        context.Users.Add(user);
        context.SaveChanges();
    }

    public void UpdateUser(User user)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Users.Update(user);
        context.SaveChanges();
    }

    public void DeleteUser(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var user = context.Users.Find(id);
        if (user != null)
        {
            context.Users.Remove(user);
            context.SaveChanges();
        }
    }

    // Maintenance Schedule methods
    public List<MaintenanceSchedule> GetSchedules()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.MaintenanceSchedules
            .OrderBy(s => s.ScheduledDate)
            .ToList();
    }

    public void AddSchedule(MaintenanceSchedule schedule)
    {
        using var context = _contextFactory.CreateDbContext();
        schedule.CreatedDate = DateTime.Now;
        context.MaintenanceSchedules.Add(schedule);
        context.SaveChanges();
    }

    public void UpdateSchedule(MaintenanceSchedule schedule)
    {
        using var context = _contextFactory.CreateDbContext();
        context.MaintenanceSchedules.Update(schedule);
        context.SaveChanges();
    }

    public void DeleteSchedule(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var schedule = context.MaintenanceSchedules.Find(id);
        if (schedule != null)
        {
            context.MaintenanceSchedules.Remove(schedule);
            context.SaveChanges();
        }
    }

    // Reliability Metrics methods
    public List<ReliabilityMetric> GetReliabilityMetrics(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.ReliabilityMetrics
            .Where(rm => rm.AssetId == assetId)
            .OrderByDescending(rm => rm.MetricDate)
            .ToList();
    }

    public void AddReliabilityMetric(ReliabilityMetric metric)
    {
        using var context = _contextFactory.CreateDbContext();
        metric.CalculatedDate = DateTime.Now;
        context.ReliabilityMetrics.Add(metric);
        context.SaveChanges();
    }

    // Dashboard Statistics methods
    public int GetTotalAssets()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Assets.Count();
    }

    public int GetCriticalAssets()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Assets.Count(a => a.Status == "Critical" || a.Criticality == "Critical");
    }

    public int GetOpenWorkOrders()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.WorkOrders.Count(wo => wo.Status == "Open" || wo.Status == "In Progress");
    }

    public double GetAverageHealthScore()
    {
        using var context = _contextFactory.CreateDbContext();
        var assets = context.Assets.ToList();
        return assets.Any() ? assets.Average(a => a.HealthScore) : 0;
    }

    public List<Asset> GetCriticalAssetsList()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Assets
            .Where(a => a.Status == "Critical")
            .OrderBy(a => a.HealthScore)
            .Take(5)
            .ToList();
    }

    public List<WorkOrder> GetUpcomingWorkOrders()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.WorkOrders
            .Include(wo => wo.Asset)
            .Where(wo => wo.Status != "Completed")
            .OrderBy(wo => wo.DueDate)
            .Take(5)
            .ToList();
    }
}
