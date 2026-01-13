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
            new() { WorkOrderId = "WO-20240101-1001", AssetId = assets[0].Id, Priority = "Medium", Type = "Preventive", Status = "Open", DueDate = DateTime.Now.AddDays(7), AssignedTo = "John Smith", Description = "Quarterly maintenance check", EstimatedDowntime = 4, EstimatedCost = 500 },
            new() { WorkOrderId = "WO-20240102-1002", AssetId = assets[2].Id, Priority = "Critical", Type = "Corrective", Status = "In Progress", DueDate = DateTime.Now.AddDays(1), AssignedTo = "John Smith", Description = "Fix vibration issue", EstimatedDowntime = 8, EstimatedCost = 1200, StartedDate = DateTime.Now.AddHours(-2) }
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

        // NOTE: Users are now seeded via IdentityDataSeeder which syncs both
        // AspNetUsers (Identity) and the legacy Users table together.
        // See IdentityDataSeeder.SeedRolesAndUsersAsync()

        // Seed Spare Parts
        var spareParts = new List<SparePart>
        {
            // Generic parts
            new() { PartNumber = "BRG-001", Name = "SKF Bearing 6205", Description = "Deep groove ball bearing", Category = "Bearings", Manufacturer = "SKF", ManufacturerPartNumber = "6205-2RS1", Supplier = "Industrial Supplies Co", QuantityInStock = 25, MinimumStockLevel = 5, ReorderPoint = 10, ReorderQuantity = 20, UnitCost = 45.50m, Location = "Shelf A-12", IsGeneric = true, CompatibleAssets = "PMP-001, MTR-002, GBX-009", Status = "In Stock", CreatedBy = "Admin" },
            new() { PartNumber = "BRG-002", Name = "Timken Bearing 6206", Description = "Deep groove ball bearing - heavy duty", Category = "Bearings", Manufacturer = "Timken", ManufacturerPartNumber = "6206-2RS", Supplier = "Industrial Supplies Co", QuantityInStock = 8, MinimumStockLevel = 5, ReorderPoint = 10, ReorderQuantity = 15, UnitCost = 52.75m, Location = "Shelf A-13", IsGeneric = true, CompatibleAssets = "MTR-002, CMP-003", Status = "Reorder Soon", CreatedBy = "Admin" },
            new() { PartNumber = "SEL-001", Name = "Hydraulic Seal Kit", Description = "Complete seal kit for hydraulic pumps", Category = "Seals", Manufacturer = "Parker", ManufacturerPartNumber = "HS-100", Supplier = "Hydraulics Inc", QuantityInStock = 12, MinimumStockLevel = 3, ReorderPoint = 5, ReorderQuantity = 10, UnitCost = 125.00m, Location = "Cabinet B-5", IsGeneric = true, CompatibleAssets = "PMP-001", Status = "In Stock", CreatedBy = "Admin" },
            new() { PartNumber = "FLT-001", Name = "Air Filter Element", Description = "High-efficiency air filter", Category = "Filters", Manufacturer = "Donaldson", ManufacturerPartNumber = "P191280", Supplier = "Filter World", QuantityInStock = 15, MinimumStockLevel = 8, ReorderPoint = 12, ReorderQuantity = 24, UnitCost = 28.50m, Location = "Shelf C-8", IsGeneric = true, CompatibleAssets = "CMP-003", Status = "In Stock", CreatedBy = "Admin" },
            new() { PartNumber = "MTR-101", Name = "1HP Electric Motor", Description = "Single phase 1HP electric motor", Category = "Motors", Manufacturer = "Baldor", ManufacturerPartNumber = "VEM3554T", Supplier = "Motor Supply", QuantityInStock = 2, MinimumStockLevel = 2, ReorderPoint = 3, ReorderQuantity = 5, UnitCost = 385.00m, Location = "Floor Storage-1", IsGeneric = true, CompatibleAssets = "Various", Status = "Low Stock", CreatedBy = "Admin" },
            new() { PartNumber = "BLT-001", Name = "V-Belt A43", Description = "Industrial V-belt", Category = "Belts", Manufacturer = "Gates", ManufacturerPartNumber = "A43", Supplier = "Power Transmission", QuantityInStock = 18, MinimumStockLevel = 6, ReorderPoint = 10, ReorderQuantity = 20, UnitCost = 15.75m, Location = "Shelf D-3", IsGeneric = true, CompatibleAssets = "CNV-004, Various", Status = "In Stock", CreatedBy = "Admin" },
            new() { PartNumber = "OIL-001", Name = "Hydraulic Oil ISO 46", Description = "Premium hydraulic oil", Category = "Hydraulic", Manufacturer = "Mobil", ManufacturerPartNumber = "DTE-25", Supplier = "Lubricants Supply", QuantityInStock = 40, MinimumStockLevel = 20, ReorderPoint = 30, ReorderQuantity = 60, UnitCost = 12.50m, Unit = "Liter", Location = "Oil Storage Room", IsGeneric = true, CompatibleAssets = "All hydraulic equipment", Status = "In Stock", CreatedBy = "Admin" },
            
            // Asset-specific parts
            new() { PartNumber = "PMP-001-IMP", Name = "Impeller for PMP-001", Description = "Replacement impeller", Category = "Pump Parts", Manufacturer = "Goulds", ManufacturerPartNumber = "IMP-3196", Supplier = "Pump Specialists", QuantityInStock = 1, MinimumStockLevel = 1, ReorderPoint = 1, ReorderQuantity = 2, UnitCost = 650.00m, Location = "Critical Parts-1", IsGeneric = false, AssetId = assets[0].Id, Status = "In Stock", CreatedBy = "Admin" },
            new() { PartNumber = "MTR-002-WND", Name = "Motor Winding for MTR-002", Description = "Replacement motor winding", Category = "Electrical", Manufacturer = "Custom Rewind", ManufacturerPartNumber = "CW-MTR002", Supplier = "Electric Motor Service", QuantityInStock = 0, MinimumStockLevel = 1, ReorderPoint = 1, ReorderQuantity = 1, UnitCost = 1200.00m, Location = "On Order", IsGeneric = false, AssetId = assets[1].Id, Status = "Out of Stock", CreatedBy = "Admin" },
            new() { PartNumber = "CMP-003-VLV", Name = "Inlet Valve for CMP-003", Description = "Compressor inlet valve assembly", Category = "Pneumatic", Manufacturer = "Atlas Copco", ManufacturerPartNumber = "IV-500", Supplier = "Compressor Parts Co", QuantityInStock = 2, MinimumStockLevel = 1, ReorderPoint = 1, ReorderQuantity = 2, UnitCost = 425.00m, Location = "Cabinet A-8", IsGeneric = false, AssetId = assets[2].Id, Status = "In Stock", CreatedBy = "Admin" }
        };

        await context.SpareParts.AddRangeAsync(spareParts);
        await context.SaveChangesAsync();

        // Seed some Spare Part Transactions
        var transactions = new List<SparePartTransaction>
        {
            new() { SparePartId = spareParts[0].Id, TransactionType = "Restock", Quantity = 30, StockBefore = 0, StockAfter = 30, TransactionDate = DateTime.Now.AddDays(-90), UnitCostAtTransaction = 45.50m, TotalCost = 1365.00m, TransactionBy = "Admin", Reason = "Initial stock", ReferenceNumber = "PO-2024-001" },
            new() { SparePartId = spareParts[0].Id, TransactionType = "Issue", Quantity = 2, StockBefore = 30, StockAfter = 28, TransactionDate = DateTime.Now.AddDays(-60), IssuedTo = "John Smith", WorkOrderId = workOrders[0].Id, UnitCostAtTransaction = 45.50m, TotalCost = 91.00m, TransactionBy = "Emily Brown", Reason = "Bearing replacement on PMP-001" },
            new() { SparePartId = spareParts[0].Id, TransactionType = "Issue", Quantity = 1, StockBefore = 28, StockAfter = 27, TransactionDate = DateTime.Now.AddDays(-30), IssuedTo = "Mike Davis", UnitCostAtTransaction = 45.50m, TotalCost = 45.50m, TransactionBy = "Emily Brown", Reason = "Preventive maintenance MTR-002" },
            new() { SparePartId = spareParts[0].Id, TransactionType = "Return", Quantity = 1, StockBefore = 27, StockAfter = 28, TransactionDate = DateTime.Now.AddDays(-29), IssuedTo = "Mike Davis", UnitCostAtTransaction = 45.50m, TotalCost = -45.50m, TransactionBy = "Emily Brown", Reason = "Unused part returned" },
            new() { SparePartId = spareParts[0].Id, TransactionType = "Issue", Quantity = 3, StockBefore = 28, StockAfter = 25, TransactionDate = DateTime.Now.AddDays(-10), IssuedTo = "John Smith", UnitCostAtTransaction = 45.50m, TotalCost = 136.50m, TransactionBy = "Admin", Reason = "Multiple asset maintenance" },
            new() { SparePartId = spareParts[2].Id, TransactionType = "Issue", Quantity = 1, StockBefore = 15, StockAfter = 14, TransactionDate = DateTime.Now.AddDays(-20), IssuedTo = "Mike Davis", WorkOrderId = workOrders[0].Id, UnitCostAtTransaction = 125.00m, TotalCost = 125.00m, TransactionBy = "Admin", Reason = "Seal replacement" },
            new() { SparePartId = spareParts[2].Id, TransactionType = "Restock", Quantity = 3, StockBefore = 14, StockAfter = 17, TransactionDate = DateTime.Now.AddDays(-5), UnitCostAtTransaction = 120.00m, TotalCost = 360.00m, TransactionBy = "Admin", Reason = "Reorder", ReferenceNumber = "PO-2024-045" },
            new() { SparePartId = spareParts[2].Id, TransactionType = "Issue", Quantity = 5, StockBefore = 17, StockAfter = 12, TransactionDate = DateTime.Now.AddDays(-2), IssuedTo = "John Smith", UnitCostAtTransaction = 120.00m, TotalCost = 600.00m, TransactionBy = "Emily Brown", Reason = "Seal kit for multiple pumps" }
        };

        await context.SparePartTransactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();

        // Seed Documents
        var documents = new List<Document>
        {
            // Manuals
            new() { DocumentNumber = "DOC-2024-001", Title = "Hydraulic Pump A Operation Manual", Description = "Complete operation and maintenance manual for PMP-001", Category = "Manual", SubCategory = "Operation Manual", FileName = "PMP-001_Manual.pdf", FilePath = "/uploads/docs/pmp001_manual.pdf", FileType = "PDF", FileSize = 2458000, Version = "1.2", RevisionNumber = 2, Status = "Active", AssetId = assets[0].Id, Department = "Maintenance", EffectiveDate = DateTime.Now.AddYears(-1), ReviewDate = DateTime.Now.AddMonths(6), CreatedBy = "Admin", Author = "Manufacturer", AccessLevel = "Public" },
            new() { DocumentNumber = "DOC-2024-002", Title = "Electric Motor B Technical Specifications", Description = "Technical specs and wiring diagrams", Category = "Specification", FileName = "MTR-002_Specs.pdf", FilePath = "/uploads/docs/mtr002_specs.pdf", FileType = "PDF", FileSize = 1520000, Version = "1.0", Status = "Active", AssetId = assets[1].Id, Department = "Engineering", CreatedBy = "Sarah Johnson", Author = "Engineering Team", AccessLevel = "Public" },
            new() { DocumentNumber = "DOC-2024-003", Title = "Air Compressor C Maintenance Procedure", Description = "Step-by-step maintenance procedures", Category = "SOP", SubCategory = "Maintenance SOP", FileName = "CMP-003_Maintenance_SOP.pdf", FilePath = "/uploads/docs/cmp003_maint.pdf", FileType = "PDF", FileSize = 980000, Version = "2.0", RevisionNumber = 3, Status = "Active", AssetId = assets[2].Id, Department = "Maintenance", EffectiveDate = DateTime.Now.AddMonths(-3), ReviewDate = DateTime.Now.AddMonths(3), CreatedBy = "Emily Brown", ApprovedBy = "Admin", ApprovalDate = DateTime.Now.AddMonths(-2), AccessLevel = "Public" },
            
            // Drawings
            new() { DocumentNumber = "DOC-2024-004", Title = "Hydraulic System Schematic", Description = "Complete hydraulic system layout and connections", Category = "Drawing", SubCategory = "Schematic", FileName = "Hydraulic_System.dwg", FilePath = "/uploads/docs/hydraulic_system.dwg", FileType = "DWG", FileSize = 3250000, Version = "1.1", RevisionNumber = 2, Status = "Active", Department = "Engineering", CreatedBy = "Sarah Johnson", Author = "Design Team", AccessLevel = "Restricted", AllowedRoles = "Admin,Reliability Engineer,Planner" },
            new() { DocumentNumber = "DOC-2024-005", Title = "Electrical Layout - Building 1", Description = "Electrical distribution layout for Building 1", Category = "Drawing", SubCategory = "Electrical Drawing", FileName = "Electrical_B1.pdf", FilePath = "/uploads/docs/elec_b1.pdf", FileType = "PDF", FileSize = 5600000, Version = "1.0", Status = "Active", Department = "Engineering", CreatedBy = "Sarah Johnson", AccessLevel = "Restricted", AllowedRoles = "Admin,Reliability Engineer" },
            
            // Certificates
            new() { DocumentNumber = "DOC-2024-006", Title = "Pressure Vessel Inspection Certificate", Description = "Annual inspection certificate for air compressor", Category = "Certificate", FileName = "CMP-003_Inspection_2024.pdf", FilePath = "/uploads/docs/cmp003_cert.pdf", FileType = "PDF", FileSize = 450000, Version = "1.0", Status = "Active", AssetId = assets[2].Id, EffectiveDate = DateTime.Now.AddMonths(-2), ExpiryDate = DateTime.Now.AddMonths(10), Department = "Quality", CreatedBy = "Admin", Author = "Inspection Authority", AccessLevel = "Public" },
            new() { DocumentNumber = "DOC-2024-007", Title = "Motor Warranty Certificate", Description = "Warranty documentation for electric motor", Category = "Warranty", FileName = "MTR-002_Warranty.pdf", FilePath = "/uploads/docs/mtr002_warranty.pdf", FileType = "PDF", FileSize = 320000, Version = "1.0", Status = "Active", AssetId = assets[1].Id, EffectiveDate = DateTime.Now.AddMonths(-6), ExpiryDate = DateTime.Now.AddMonths(18), CreatedBy = "Emily Brown", AccessLevel = "Public" },
            
            // SOPs
            new() { DocumentNumber = "DOC-2024-008", Title = "Lockout/Tagout Procedure", Description = "Safety procedure for equipment isolation", Category = "SOP", SubCategory = "Safety SOP", FileName = "LOTO_Procedure.pdf", FilePath = "/uploads/docs/loto_sop.pdf", FileType = "PDF", FileSize = 1200000, Version = "3.0", RevisionNumber = 5, Status = "Active", Department = "Safety", EffectiveDate = DateTime.Now.AddMonths(-1), ReviewDate = DateTime.Now.AddMonths(11), CreatedBy = "Admin", ApprovedBy = "Safety Manager", ApprovalDate = DateTime.Now.AddMonths(-1), Author = "Safety Team", AccessLevel = "Public", Tags = "safety,lockout,tagout,procedure" },
            new() { DocumentNumber = "DOC-2024-009", Title = "Emergency Shutdown Procedure", Description = "Emergency shutdown procedures for all equipment", Category = "SOP", SubCategory = "Emergency SOP", FileName = "Emergency_Shutdown.pdf", FilePath = "/uploads/docs/emerg_shutdown.pdf", FileType = "PDF", FileSize = 890000, Version = "2.0", RevisionNumber = 2, Status = "Active", Department = "Operations", EffectiveDate = DateTime.Now.AddMonths(-4), ReviewDate = DateTime.Now.AddMonths(2), CreatedBy = "Admin", ApprovedBy = "Operations Manager", ApprovalDate = DateTime.Now.AddMonths(-3), AccessLevel = "Public", Tags = "emergency,shutdown,safety" },
            
            // Reports
            new() { DocumentNumber = "DOC-2024-010", Title = "Quarterly Reliability Report Q1 2024", Description = "Reliability metrics and analysis for Q1", Category = "Report", SubCategory = "Quarterly Report", FileName = "Reliability_Q1_2024.pdf", FilePath = "/uploads/docs/rel_q1_2024.pdf", FileType = "PDF", FileSize = 2100000, Version = "1.0", Status = "Active", Department = "Reliability", EffectiveDate = DateTime.Now.AddMonths(-1), CreatedBy = "Sarah Johnson", Author = "Reliability Team", AccessLevel = "Restricted", AllowedRoles = "Admin,Reliability Engineer,Planner", Tags = "reliability,metrics,quarterly,report" },
            
            // Photos
            new() { DocumentNumber = "DOC-2024-011", Title = "Conveyor Belt Damage Photo", Description = "Photo evidence of belt wear on CNV-004", Category = "Photo", FileName = "CNV-004_Damage_20241201.jpg", FilePath = "/uploads/docs/cnv004_damage.jpg", FileType = "JPG", FileSize = 1850000, Version = "1.0", Status = "Active", AssetId = assets[3].Id, WorkOrderId = workOrders[0].Id, CreatedBy = "John Smith", Department = "Maintenance", AccessLevel = "Public" },
            
            // Expired document
            new() { DocumentNumber = "DOC-2023-015", Title = "Old Safety Procedure (Obsolete)", Description = "Previous version of safety procedure - superseded", Category = "SOP", SubCategory = "Safety SOP", FileName = "Old_Safety_SOP.pdf", FilePath = "/uploads/docs/old_safety.pdf", FileType = "PDF", FileSize = 650000, Version = "1.0", Status = "Obsolete", Department = "Safety", EffectiveDate = DateTime.Now.AddYears(-2), ExpiryDate = DateTime.Now.AddMonths(-2), CreatedBy = "Admin", AccessLevel = "Public" },
            
            // Needs review
            new() { DocumentNumber = "DOC-2024-012", Title = "Preventive Maintenance Schedule", Description = "Annual PM schedule - needs quarterly review", Category = "Report", SubCategory = "Schedule", FileName = "PM_Schedule_2024.xlsx", FilePath = "/uploads/docs/pm_schedule_2024.xlsx", FileType = "XLSX", FileSize = 580000, Version = "1.0", Status = "Active", Department = "Planning", EffectiveDate = DateTime.Now.AddMonths(-4), ReviewDate = DateTime.Now.AddDays(-5), CreatedBy = "Emily Brown", Author = "Planning Team", AccessLevel = "Public", Tags = "maintenance,schedule,pm" }
        };

        await context.Documents.AddRangeAsync(documents);
        await context.SaveChangesAsync();

        // Seed Document Access Logs
        var docAccessLogs = new List<DocumentAccessLog>
        {
            new() { DocumentId = documents[0].Id, ActionType = "View", AccessedBy = "John Smith", UserRole = "Technician", AccessDate = DateTime.Now.AddDays(-5) },
            new() { DocumentId = documents[0].Id, ActionType = "Download", AccessedBy = "John Smith", UserRole = "Technician", AccessDate = DateTime.Now.AddDays(-5).AddMinutes(2) },
            new() { DocumentId = documents[0].Id, ActionType = "View", AccessedBy = "Mike Davis", UserRole = "Technician", AccessDate = DateTime.Now.AddDays(-3) },
            new() { DocumentId = documents[1].Id, ActionType = "View", AccessedBy = "Sarah Johnson", UserRole = "Reliability Engineer", AccessDate = DateTime.Now.AddDays(-7) },
            new() { DocumentId = documents[1].Id, ActionType = "Download", AccessedBy = "Sarah Johnson", UserRole = "Reliability Engineer", AccessDate = DateTime.Now.AddDays(-7).AddMinutes(1) },
            new() { DocumentId = documents[2].Id, ActionType = "View", AccessedBy = "Emily Brown", UserRole = "Planner", AccessDate = DateTime.Now.AddDays(-2) },
            new() { DocumentId = documents[2].Id, ActionType = "View", AccessedBy = "John Smith", UserRole = "Technician", AccessDate = DateTime.Now.AddDays(-1) },
            new() { DocumentId = documents[7].Id, ActionType = "View", AccessedBy = "Admin", UserRole = "Admin", AccessDate = DateTime.Now.AddHours(-3) },
            new() { DocumentId = documents[7].Id, ActionType = "Download", AccessedBy = "Mike Davis", UserRole = "Technician", AccessDate = DateTime.Now.AddHours(-2) },
            new() { DocumentId = documents[9].Id, ActionType = "View", AccessedBy = "Sarah Johnson", UserRole = "Reliability Engineer", AccessDate = DateTime.Now.AddDays(-1) }
        };

        await context.DocumentAccessLogs.AddRangeAsync(docAccessLogs);
        await context.SaveChangesAsync();
        
        // Update document view/download counts
        foreach (var doc in documents)
        {
            doc.ViewCount = docAccessLogs.Count(l => l.DocumentId == doc.Id && l.ActionType == "View");
            doc.DownloadCount = docAccessLogs.Count(l => l.DocumentId == doc.Id && l.ActionType == "Download");
        }
        await context.SaveChangesAsync();
    }
}
