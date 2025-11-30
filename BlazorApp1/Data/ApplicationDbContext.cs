using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Models;

namespace BlazorApp1.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    // RBM CMMS Tables
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetAttachment> AssetAttachments { get; set; }
    public DbSet<AssetDowntime> AssetDowntime { get; set; }
    public DbSet<ReliabilityMetric> ReliabilityMetrics { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<MaintenanceTask> MaintenanceTasks { get; set; }
    public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; }
    public DbSet<ConditionReading> ConditionReadings { get; set; }
    public DbSet<FailureMode> FailureModes { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships and constraints
        
        // Asset relationships
        modelBuilder.Entity<Asset>()
            .HasMany(a => a.Attachments)
            .WithOne(at => at.Asset)
            .HasForeignKey(at => at.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Asset>()
            .HasMany(a => a.DowntimeRecords)
            .WithOne(dt => dt.Asset)
            .HasForeignKey(dt => dt.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Asset>()
            .HasMany(a => a.WorkOrders)
            .WithOne(wo => wo.Asset)
            .HasForeignKey(wo => wo.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asset>()
            .HasMany(a => a.ConditionReadings)
            .WithOne(cr => cr.Asset)
            .HasForeignKey(cr => cr.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Asset>()
            .HasMany(a => a.FailureModes)
            .WithOne(fm => fm.Asset)
            .HasForeignKey(fm => fm.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Asset>()
            .HasMany(a => a.ReliabilityMetrics)
            .WithOne(rm => rm.Asset)
            .HasForeignKey(rm => rm.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        // WorkOrder relationships
        modelBuilder.Entity<WorkOrder>()
            .HasMany(wo => wo.MaintenanceTasks)
            .WithOne(mt => mt.WorkOrder)
            .HasForeignKey(mt => mt.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkOrder>()
            .HasMany(wo => wo.DowntimeRecords)
            .WithOne(dt => dt.RelatedWorkOrder)
            .HasForeignKey(dt => dt.RelatedWorkOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        // MaintenanceSchedule relationships
        modelBuilder.Entity<MaintenanceSchedule>()
            .HasMany(ms => ms.MaintenanceTasks)
            .WithOne(mt => mt.Schedule)
            .HasForeignKey(mt => mt.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure decimal precision
        modelBuilder.Entity<AssetDowntime>()
            .Property(ad => ad.ProductionLoss)
            .HasPrecision(18, 2);

        modelBuilder.Entity<AssetDowntime>()
            .Property(ad => ad.FinancialImpact)
            .HasPrecision(18, 2);

        modelBuilder.Entity<WorkOrder>()
            .Property(wo => wo.EstimatedCost)
            .HasPrecision(18, 2);

        modelBuilder.Entity<WorkOrder>()
            .Property(wo => wo.ActualCost)
            .HasPrecision(18, 2);

        // Create indexes for better performance
        modelBuilder.Entity<Asset>()
            .HasIndex(a => a.AssetId)
            .IsUnique();

        modelBuilder.Entity<WorkOrder>()
            .HasIndex(wo => wo.WorkOrderId)
            .IsUnique();

        modelBuilder.Entity<ConditionReading>()
            .HasIndex(cr => cr.ReadingDate);

        modelBuilder.Entity<AssetDowntime>()
            .HasIndex(ad => ad.StartTime);

        modelBuilder.Entity<ReliabilityMetric>()
            .HasIndex(rm => rm.MetricDate);
    }
}
