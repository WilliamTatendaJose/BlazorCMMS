using Microsoft.EntityFrameworkCore;
using BlazorApp1.Models;

namespace BlazorApp1.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if we already have data
        if (await context.Assets.AnyAsync())
        {
            return; // DB has been seeded
        }

        // Seed Assets
        var assets = new List<Asset>
        {
            new() { AssetId = "PMP-001", Name = "Hydraulic Pump A", Location = "Building 1, Floor 2", Criticality = "Critical", HealthScore = 85, Uptime = 98.5, Downtime = 1.5, Status = "Healthy", LastMaintenance = DateTime.Now.AddDays(-30) },
            new() { AssetId = "MTR-002", Name = "Electric Motor B", Location = "Building 1, Floor 3", Criticality = "High", HealthScore = 72, Uptime = 95.0, Downtime = 5.0, Status = "Warning", LastMaintenance = DateTime.Now.AddDays(-45) },
            new() { AssetId = "CMP-003", Name = "Air Compressor C", Location = "Building 2, Floor 1", Criticality = "Critical", HealthScore = 55, Uptime = 92.0, Downtime = 8.0, Status = "Critical", LastMaintenance = DateTime.Now.AddDays(-60) },
            new() { AssetId = "CNV-004", Name = "Conveyor Belt D", Location = "Building 2, Floor 2", Criticality = "Medium", HealthScore = 88, Uptime = 97.0, Downtime = 3.0, Status = "Healthy", LastMaintenance = DateTime.Now.AddDays(-20) },
            new() { AssetId = "GBX-009", Name = "Gearbox I", Location = "Building 3, Floor 1", Criticality = "High", HealthScore = 90, Uptime = 99.0, Downtime = 1.0, Status = "Healthy", LastMaintenance = DateTime.Now.AddDays(-15) }
        };

        await context.Assets.AddRangeAsync(assets);
        await context.SaveChangesAsync();

        // Seed Work Orders
        var workOrders = new List<WorkOrder>
        {
            new() { WorkOrderId = "WO-20240101-1001", AssetId = assets[0].Id, AssetName = assets[0].Name, Priority = "Medium", Type = "Preventive", Status = "Open", DueDate = DateTime.Now.AddDays(7), AssignedTo = "John Smith", Description = "Quarterly maintenance check", EstimatedDowntime = 4, EstimatedCost = 500 },
            new() { WorkOrderId = "WO-20240102-1002", AssetId = assets[2].Id, AssetName = assets[2].Name, Priority = "Critical", Type = "Corrective", Status = "In Progress", DueDate = DateTime.Now.AddDays(1), AssignedTo = "John Smith", Description = "Fix vibration issue", EstimatedDowntime = 8, EstimatedCost = 1200, StartedDate = DateTime.Now.AddHours(-2) }
        };

        await context.WorkOrders.AddRangeAsync(workOrders);
        await context.SaveChangesAsync();

        // Seed Maintenance Tasks for first work order
        var tasks = new List<MaintenanceTask>
        {
            new() { WorkOrderId = workOrders[0].Id, TaskName = "Inspect pump seals", Description = "Visual inspection of all seals for wear", Sequence = 1, EstimatedDuration = 0.5, Status = "Pending", ToolsRequired = "Flashlight, inspection mirror", SafetyLevel = "Low" },
            new() { WorkOrderId = workOrders[0].Id, TaskName = "Check fluid levels", Description = "Verify hydraulic fluid levels and top up if needed", Sequence = 2, EstimatedDuration = 0.5, Status = "Pending", ToolsRequired = "Dipstick, funnel", SafetyLevel = "Low" },
            new() { WorkOrderId = workOrders[0].Id, TaskName = "Lubricate bearings", Description = "Apply grease to all bearing points", Sequence = 3, EstimatedDuration = 1.0, Status = "Pending", ToolsRequired = "Grease gun", PartsRequired = "Hydraulic grease", SafetyLevel = "Medium" },
            new() { WorkOrderId = workOrders[0].Id, TaskName = "Test operation", Description = "Run pump and check for abnormal noise/vibration", Sequence = 4, EstimatedDuration = 2.0, Status = "Pending", ToolsRequired = "Vibration analyzer", SafetyLevel = "Medium", SafetyPrecautions = "Ensure all guards are in place" }
        };

        await context.MaintenanceTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();

        // Seed Condition Readings
        var readings = new List<ConditionReading>();
        foreach (var asset in assets)
        {
            for (int i = 0; i < 30; i++)
            {
                readings.Add(new ConditionReading
                {
                    AssetId = asset.Id,
                    ReadingDate = DateTime.Now.AddDays(-i),
                    Temperature = 165 + (i % 20),
                    Vibration = 2.5 + (i % 10) * 0.3,
                    Pressure = 120 + (i % 15),
                    OverallStatus = i % 10 == 0 ? "Warning" : "Normal",
                    RecordedBy = "System"
                });
            }
        }

        await context.ConditionReadings.AddRangeAsync(readings);
        await context.SaveChangesAsync();

        // Seed Failure Modes
        var failureModes = new List<FailureMode>
        {
            new() { AssetId = assets[0].Id, Mode = "Seal failure", Cause = "Wear and tear", Effect = "Hydraulic fluid leak", Severity = 8, Occurrence = 4, Detection = 6, CurrentControls = "Visual inspections", RecommendedActions = "Increase inspection frequency" },
            new() { AssetId = assets[1].Id, Mode = "Bearing failure", Cause = "Insufficient lubrication", Effect = "Motor seizure", Severity = 9, Occurrence = 3, Detection = 5, CurrentControls = "Vibration monitoring", RecommendedActions = "Implement automated lubrication" },
            new() { AssetId = assets[2].Id, Mode = "Overheating", Cause = "Clogged air filter", Effect = "Reduced efficiency", Severity = 6, Occurrence = 7, Detection = 4, CurrentControls = "Temperature sensors", RecommendedActions = "Monthly filter replacement" }
        };

        await context.FailureModes.AddRangeAsync(failureModes);
        await context.SaveChangesAsync();

        // Seed Asset Downtime
        var downtimeRecords = new List<AssetDowntime>
        {
            new() { AssetId = assets[2].Id, StartTime = DateTime.Now.AddDays(-5), EndTime = DateTime.Now.AddDays(-5).AddHours(6), Reason = "Compressor failure", Category = "Unplanned", Description = "Emergency repair due to sudden shutdown", ProductionLoss = 150, FinancialImpact = 3500, RecordedBy = "Mike Johnson" },
            new() { AssetId = assets[0].Id, StartTime = DateTime.Now.AddDays(-30), EndTime = DateTime.Now.AddDays(-30).AddHours(4), Reason = "Scheduled maintenance", Category = "Planned", Description = "Quarterly preventive maintenance", ProductionLoss = 0, FinancialImpact = 500, RecordedBy = "System", RelatedWorkOrderId = workOrders[0].Id }
        };

        await context.AssetDowntime.AddRangeAsync(downtimeRecords);
        await context.SaveChangesAsync();

        // Seed Reliability Metrics
        var metrics = new List<ReliabilityMetric>();
        foreach (var asset in assets)
        {
            for (int month = 1; month <= 12; month++)
            {
                metrics.Add(new ReliabilityMetric
                {
                    AssetId = asset.Id,
                    MetricDate = new DateTime(DateTime.Now.Year, month, 1),
                    MTBF = 1000 + (month * 20),
                    MTTR = 5 - (month * 0.1),
                    Availability = 95 + (month * 0.2),
                    Reliability = 92 + (month * 0.3),
                    OEE = 85 + (month * 0.5),
                    FailureCount = 12 - month,
                    TotalDowntimeHours = 50 - (month * 2),
                    TotalUptimeHours = 680 + (month * 5),
                    Period = "Monthly"
                });
            }
        }

        await context.ReliabilityMetrics.AddRangeAsync(metrics);
        await context.SaveChangesAsync();

        // Seed Maintenance Schedules
        var schedules = new List<MaintenanceSchedule>
        {
            new() { AssetId = assets[0].Id, AssetName = assets[0].Name, ScheduledDate = DateTime.Now.AddDays(14).AddHours(8), EndDate = DateTime.Now.AddDays(14).AddHours(12), Type = "Preventive", AssignedTechnician = "Mike Davis", Status = "Scheduled", Description = "Biweekly preventive maintenance", EstimatedDuration = 4, Frequency = "Biweekly", CreatedBy = "Planner" },
            new() { AssetId = assets[3].Id, AssetName = assets[3].Name, ScheduledDate = DateTime.Now.AddDays(7).AddHours(8), EndDate = DateTime.Now.AddDays(7).AddHours(10), Type = "Preventive", AssignedTechnician = "John Smith", Status = "Scheduled", Description = "Weekly belt tension check", EstimatedDuration = 2, Frequency = "Weekly", CreatedBy = "Planner" }
        };

        await context.MaintenanceSchedules.AddRangeAsync(schedules);
        await context.SaveChangesAsync();

        // Seed Users
        var users = new List<User>
        {
            new() { Name = "Admin User", Email = "admin@company.com", Role = "Admin", Department = "Management", Phone = "555-0001", IsActive = true },
            new() { Name = "Sarah Johnson", Email = "sarah.johnson@company.com", Role = "Reliability Engineer", Department = "Engineering", Phone = "555-0002", IsActive = true },
            new() { Name = "Emily Brown", Email = "emily.brown@company.com", Role = "Planner", Department = "Planning", Phone = "555-0003", IsActive = true },
            new() { Name = "John Smith", Email = "john.smith@company.com", Role = "Technician", Department = "Maintenance", Phone = "555-0004", IsActive = true },
            new() { Name = "Mike Davis", Email = "mike.davis@company.com", Role = "Technician", Department = "Maintenance", Phone = "555-0005", IsActive = true }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}
