using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Models;

namespace BlazorApp1.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    // Multi-tenancy tables
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantUserMapping> UserTenantMappings { get; set; }
    
    // RBM CMMS Tables
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetAttachment> AssetAttachments { get; set; }
    public DbSet<AssetDowntime> AssetDowntime { get; set; }
    public DbSet<ReliabilityMetric> ReliabilityMetrics { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<WorkOrderSpareUsed> WorkOrderSparesUsed { get; set; }
    public DbSet<MaintenanceTask> MaintenanceTasks { get; set; }
    public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; }
    public DbSet<ConditionReading> ConditionReadings { get; set; }
    public DbSet<FailureMode> FailureModes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<SparePart> SpareParts { get; set; }
    public DbSet<SparePartTransaction> SparePartTransactions { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentAccessLog> DocumentAccessLogs { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<NotificationSettings> NotificationSettings { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }
    public DbSet<WhatsAppMessageLog> WhatsAppMessageLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure multi-tenancy relationships
        
        // Tenant relationships
        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.PrimaryTenantId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.TenantMappings)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TenantUserMapping>()
            .HasOne(m => m.Tenant)
            .WithMany()
            .HasForeignKey(m => m.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TenantUserMapping>()
            .HasIndex(m => new { m.TenantId, m.UserId })
            .IsUnique(false);

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

        // SparePart relationships
        modelBuilder.Entity<SparePart>()
            .HasOne(sp => sp.Asset)
            .WithMany()
            .HasForeignKey(sp => sp.AssetId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SparePart>()
            .HasMany(sp => sp.Transactions)
            .WithOne(spt => spt.SparePart)
            .HasForeignKey(spt => spt.SparePartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SparePart>()
            .HasIndex(sp => sp.PartNumber)
            .IsUnique();

        // SparePartTransaction relationships
        modelBuilder.Entity<SparePartTransaction>()
            .HasOne(spt => spt.WorkOrder)
            .WithMany()
            .HasForeignKey(spt => spt.WorkOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SparePartTransaction>()
            .HasOne(spt => spt.Asset)
            .WithMany()
            .HasForeignKey(spt => spt.AssetId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SparePartTransaction>()
            .HasIndex(spt => spt.TransactionDate);

        // Configure decimal precision for SparePart
        modelBuilder.Entity<SparePart>()
            .Property(sp => sp.UnitCost)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SparePartTransaction>()
            .Property(spt => spt.UnitCostAtTransaction)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SparePartTransaction>()
            .Property(spt => spt.TotalCost)
            .HasPrecision(18, 2);

        // WorkOrderSpareUsed relationships
        modelBuilder.Entity<WorkOrderSpareUsed>()
            .HasOne(ws => ws.WorkOrder)
            .WithMany(wo => wo.SparesUsed)
            .HasForeignKey(ws => ws.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkOrderSpareUsed>()
            .HasOne(ws => ws.SparePart)
            .WithMany()
            .HasForeignKey(ws => ws.SparePartId)
            .OnDelete(DeleteBehavior.SetNull);

        // Document relationships
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Asset)
            .WithMany()
            .HasForeignKey(d => d.AssetId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.WorkOrder)
            .WithMany()
            .HasForeignKey(d => d.WorkOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.FailureMode)
            .WithMany()
            .HasForeignKey(d => d.FailureModeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Document>()
            .HasMany(d => d.AccessLogs)
            .WithOne(al => al.Document)
            .HasForeignKey(al => al.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Document>()
            .HasIndex(d => d.DocumentNumber)
            .IsUnique();

        modelBuilder.Entity<Document>()
            .HasIndex(d => d.Category);

        modelBuilder.Entity<Document>()
            .HasIndex(d => d.CreatedDate);

        modelBuilder.Entity<DocumentAccessLog>()
            .HasIndex(dal => dal.AccessDate);

        // NotificationSettings relationships
        modelBuilder.Entity<NotificationSettings>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ns => ns.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NotificationSettings>()
            .HasIndex(ns => ns.UserId)
            .IsUnique();

        // NotificationLog relationships
        modelBuilder.Entity<NotificationLog>()
            .HasIndex(nl => nl.CreatedDate);

        modelBuilder.Entity<NotificationLog>()
            .HasIndex(nl => new { nl.UserId, nl.CreatedDate });
    }
}
