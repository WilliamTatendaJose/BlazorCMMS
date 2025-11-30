# Database Schema Documentation - RBM CMMS

## Overview
This document describes the complete database schema for the Reliability-Based Maintenance (RBM) Computerized Maintenance Management System (CMMS).

## Database Tables

### 1. Assets
**Purpose:** Core asset/equipment tracking
**Table:** `Assets`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| AssetId | string(50) | Unique asset identifier (e.g., "PMP-001") |
| Name | string(200) | Asset name |
| Location | string(500) | Physical location |
| Criticality | string(50) | Low, Medium, High, Critical |
| HealthScore | double | Current health score (0-100) |
| LastMaintenance | DateTime? | Last maintenance date |
| Uptime | double | Uptime percentage |
| Downtime | double | Downtime percentage |
| Status | string(50) | Healthy, Warning, Critical |
| CreatedDate | DateTime | Record creation date |
| ModifiedDate | DateTime? | Last modification date |

**Relationships:**
- One-to-Many: AssetAttachments
- One-to-Many: AssetDowntime
- One-to-Many: WorkOrders
- One-to-Many: ConditionReadings
- One-to-Many: FailureModes
- One-to-Many: ReliabilityMetrics

---

### 2. AssetAttachments
**Purpose:** Store documents, manuals, photos, schematics for assets
**Table:** `AssetAttachments`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| AssetId | int (FK) | Foreign key to Assets |
| FileName | string(500) | Original file name |
| FilePath | string(1000) | Storage path/URL |
| FileType | string(100) | PDF, Image, Document, etc. |
| FileSize | long | File size in bytes |
| Category | string(50) | Manual, Photo, Schematic, Warranty, Invoice |
| Description | string(1000) | File description |
| UploadedDate | DateTime | Upload timestamp |
| UploadedBy | string(200) | User who uploaded |

**Use Cases:**
- Equipment manuals
- Warranty documents
- Photos of equipment/damage
- Electrical schematics
- Purchase invoices
- Inspection reports

---

### 3. AssetDowntime
**Purpose:** Track all downtime events per asset
**Table:** `AssetDowntime`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| AssetId | int (FK) | Foreign key to Assets |
| StartTime | DateTime | Downtime start |
| EndTime | DateTime? | Downtime end (null if ongoing) |
| DurationHours | double (calculated) | Calculated duration |
| Reason | string(100) | Breakdown, Maintenance, Setup |
| Category | string(50) | Planned, Unplanned |
| Description | string(2000) | Detailed description |
| RelatedWorkOrderId | int? (FK) | Related work order |
| ProductionLoss | decimal(18,2) | Units not produced |
| FinancialImpact | decimal(18,2) | Estimated cost impact |
| RecordedBy | string(200) | Who recorded the event |
| RecordedDate | DateTime | When recorded |

**Key Metrics Enabled:**
- Total downtime per asset
- Planned vs unplanned downtime
- Financial impact analysis
- Production loss tracking
- Downtime by reason/category

---

### 4. ReliabilityMetrics
**Purpose:** Store calculated reliability analytics per asset
**Table:** `ReliabilityMetrics`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| AssetId | int (FK) | Foreign key to Assets |
| MetricDate | DateTime | Date of calculation |
| MTBF | double | Mean Time Between Failures (hours) |
| MTTR | double | Mean Time To Repair (hours) |
| MTTF | double | Mean Time To Failure (hours) |
| Availability | double | Availability percentage |
| Reliability | double | Reliability percentage |
| OEE | double | Overall Equipment Effectiveness |
| FailureCount | int | Number of failures |
| TotalDowntimeHours | double | Total downtime |
| TotalUptimeHours | double | Total uptime |
| Period | string(50) | Daily, Weekly, Monthly, Yearly |
| Notes | string(1000) | Additional notes |
| CalculatedDate | DateTime | When metrics were calculated |

**Calculations:**
```
MTBF = Total Uptime Hours / Failure Count
MTTR = Total Downtime Hours / Failure Count
Availability = (Uptime / (Uptime + Downtime)) * 100
OEE = Availability * Performance * Quality
```

---

### 5. WorkOrders
**Purpose:** Manage maintenance work orders
**Table:** `WorkOrders`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| WorkOrderId | string(50) | Unique WO identifier |
| AssetId | int (FK) | Foreign key to Assets |
| AssetName | string(200) | Asset name (denormalized) |
| Priority | string(50) | Low, Medium, High, Critical |
| Type | string(50) | Preventive, Corrective, Predictive |
| Status | string(50) | Open, In Progress, Completed, Cancelled |
| CreatedDate | DateTime | Creation date |
| DueDate | DateTime | Due date |
| StartedDate | DateTime? | When work started |
| CompletedDate | DateTime? | When completed |
| AssignedTo | string(200) | Assigned technician |
| Description | string(2000) | Work description |
| EstimatedDowntime | double | Estimated hours |
| ActualDowntime | double? | Actual hours |
| EstimatedCost | decimal(18,2) | Estimated cost |
| ActualCost | decimal(18,2)? | Actual cost |
| CompletionNotes | string(2000) | Completion notes |
| PartsUsed | string(500) | Parts used |
| LaborHours | double? | Labor hours spent |

**Relationships:**
- Many-to-One: Asset
- One-to-Many: MaintenanceTasks
- One-to-Many: AssetDowntime

**Workflow:**
1. Created (Status: Open)
2. Assigned to technician
3. Started (Status: In Progress)
4. Completed (Status: Completed)

---

### 6. MaintenanceTasks
**Purpose:** Detailed task breakdown for work orders and schedules
**Table:** `MaintenanceTasks`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| WorkOrderId | int? (FK) | Related work order |
| ScheduleId | int? (FK) | Related schedule |
| TaskName | string(500) | Task name |
| Description | string(2000) | Task description |
| Status | string(50) | Pending, In Progress, Completed, Skipped |
| Sequence | int | Execution order |
| EstimatedDuration | double | Estimated hours |
| ActualDuration | double? | Actual hours |
| AssignedTo | string(200) | Assigned person |
| StartedDate | DateTime? | Start time |
| CompletedDate | DateTime? | Completion time |
| ToolsRequired | string(1000) | Required tools |
| PartsRequired | string(1000) | Required parts |
| SafetyLevel | string(50) | Low, Medium, High |
| SafetyPrecautions | string(2000) | Safety notes |
| CompletionNotes | string(2000) | Notes on completion |
| IsCompleted | bool | Completion flag |
| CompletedBy | string(200) | Who completed |

**Example Tasks:**
1. Shut down equipment
2. Lock out/Tag out (LOTO)
3. Drain fluids
4. Remove old bearing
5. Install new bearing
6. Lubricate
7. Test run
8. Document completion

---

### 7. MaintenanceSchedules
**Purpose:** Scheduled preventive maintenance
**Table:** `MaintenanceSchedules`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| AssetId | int (FK) | Foreign key to Assets |
| AssetName | string(200) | Asset name |
| ScheduledDate | DateTime | Scheduled date/time |
| EndDate | DateTime? | Expected end time |
| Type | string(50) | Preventive, Predictive, Condition-Based |
| AssignedTechnician | string(200) | Assigned technician |
| Status | string(50) | Scheduled, In Progress, Completed, Cancelled |
| Description | string(2000) | Description |
| EstimatedDuration | double | Estimated hours |
| ActualDuration | double? | Actual hours |
| Frequency | string(50) | Daily, Weekly, Monthly, Quarterly, Annually |
| NextScheduledDate | DateTime? | Next occurrence |
| CreatedDate | DateTime | Creation date |
| CreatedBy | string(200) | Creator |
| CompletedDate | DateTime? | Completion date |
| CompletionNotes | string(2000) | Notes |

**Relationships:**
- Many-to-One: Asset
- One-to-Many: MaintenanceTasks

---

### 8. ConditionReadings
**Purpose:** Store condition monitoring sensor data
**Table:** `ConditionReadings`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| AssetId | int (FK) | Foreign key to Assets |
| ReadingDate | DateTime | Reading timestamp |
| Temperature | double? | Temperature (°F) |
| Vibration | double? | Vibration (mm/s) |
| Pressure | double? | Pressure (PSI) |
| OilAnalysis | string(50) | Normal, Warning, Critical |
| Current | double? | Current (Amps) |
| Voltage | double? | Voltage (Volts) |
| NoiseLevel | double? | Noise (dB) |
| FlowRate | double? | Flow rate (GPM) |
| Notes | string(2000) | Additional notes |
| RecordedBy | string(200) | Who recorded |
| OverallStatus | string(50) | Normal, Warning, Critical |
| AlertGenerated | bool | Alert triggered? |

---

### 9. FailureModes
**Purpose:** FMEA (Failure Mode and Effects Analysis)
**Table:** `FailureModes`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| AssetId | int (FK) | Foreign key to Assets |
| Mode | string(500) | Failure mode |
| Cause | string(1000) | Root cause |
| Effect | string(1000) | Effect/consequence |
| Severity | int (1-10) | Severity rating |
| Occurrence | int (1-10) | Occurrence rating |
| Detection | int (1-10) | Detection rating |
| RPN | int (calculated) | Risk Priority Number (S×O×D) |
| CurrentControls | string(1000) | Current controls |
| RecommendedActions | string(1000) | Recommended actions |
| ResponsiblePerson | string(200) | Responsible person |
| TargetCompletionDate | DateTime? | Target date |
| ActionsTaken | string(2000) | Actions taken |
| RevisedSeverity | int? | Revised S rating |
| RevisedOccurrence | int? | Revised O rating |
| RevisedDetection | int? | Revised D rating |
| RevisedRPN | int? (calculated) | Revised RPN |
| CreatedDate | DateTime | Creation date |
| ModifiedDate | DateTime? | Modification date |

**RPN Interpretation:**
- **1-100**: Low Risk
- **100-200**: Medium Risk
- **200-1000**: High Risk (Immediate Action Required)

---

### 10. Users
**Purpose:** User management for RBM CMMS
**Table:** `Users`

| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Primary key |
| Name | string(200) | User name |
| Email | string(200) | Email address |
| Role | string(50) | Admin, Reliability Engineer, Planner, Technician |
| Department | string(100) | Department |
| Phone | string(20) | Phone number |
| IsActive | bool | Active status |
| CreatedDate | DateTime | Creation date |
| LastLoginDate | DateTime? | Last login |
| Notes | string(500) | Additional notes |

---

## Indexes

For optimal performance, the following indexes are created:

- `Assets.AssetId` (Unique)
- `WorkOrders.WorkOrderId` (Unique)
- `ConditionReadings.ReadingDate`
- `AssetDowntime.StartTime`
- `ReliabilityMetrics.MetricDate`

---

## Entity Relationships

```
Asset (1) ?????< (M) AssetAttachment
Asset (1) ?????< (M) AssetDowntime
Asset (1) ?????< (M) WorkOrder
Asset (1) ?????< (M) ConditionReading
Asset (1) ?????< (M) FailureMode
Asset (1) ?????< (M) ReliabilityMetric
Asset (1) ?????< (M) MaintenanceSchedule

WorkOrder (1) ?????< (M) MaintenanceTask
WorkOrder (1) ?????< (M) AssetDowntime

MaintenanceSchedule (1) ?????< (M) MaintenanceTask
```

---

## Running Migrations

### Create Migration
```powershell
.\add-migration.ps1
```

OR manually:

```bash
dotnet ef migrations add InitialRBM_CMMS --project BlazorApp1 --context ApplicationDbContext
dotnet ef database update --project BlazorApp1 --context ApplicationDbContext
```

### View Pending Migrations
```bash
dotnet ef migrations list --project BlazorApp1
```

### Remove Last Migration (if needed)
```bash
dotnet ef migrations remove --project BlazorApp1
```

### Generate SQL Script
```bash
dotnet ef migrations script --project BlazorApp1 --output migration.sql
```

---

## Sample Queries

### Get Asset with All Downtime Records
```csharp
var asset = await context.Assets
    .Include(a => a.DowntimeRecords)
    .FirstOrDefaultAsync(a => a.Id == assetId);

var totalDowntime = asset.DowntimeRecords
    .Sum(d => d.DurationHours);
```

### Get Work Order with Tasks
```csharp
var workOrder = await context.WorkOrders
    .Include(wo => wo.MaintenanceTasks.OrderBy(t => t.Sequence))
    .FirstOrDefaultAsync(wo => wo.Id == workOrderId);
```

### Calculate MTBF for Asset
```csharp
var failures = await context.AssetDowntime
    .Where(d => d.AssetId == assetId && d.Category == "Unplanned")
    .CountAsync();

var totalUptime = await context.ConditionReadings
    .Where(cr => cr.AssetId == assetId)
    .SumAsync(cr => cr.UptimeHours); // You'd need to track this

var mtbf = totalUptime / failures;
```

### Get Attachments by Category
```csharp
var manuals = await context.AssetAttachments
    .Where(a => a.AssetId == assetId && a.Category == "Manual")
    .OrderByDescending(a => a.UploadedDate)
    .ToListAsync();
```

---

## Data Validation Rules

- **AssetId**: Must be unique across all assets
- **WorkOrderId**: Must be unique across all work orders
- **Severity, Occurrence, Detection**: Must be 1-10
- **Email**: Must be valid email format
- **Phone**: Must be valid phone format
- **Dates**: CreatedDate cannot be in future
- **Costs**: Must be >= 0
- **Percentages**: Must be 0-100

---

## Best Practices

1. **Always use transactions** for multi-table updates
2. **Eager load related entities** when you know you'll need them
3. **Use async/await** for all database operations
4. **Implement soft deletes** for critical records (add IsDeleted flag)
5. **Track audit fields**: CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
6. **Use stored procedures** for complex calculations
7. **Regular backups**: Implement automated backup strategy
8. **Monitor performance**: Use query profiling tools

---

## Future Enhancements

- [ ] Audit logging table
- [ ] Notification preferences
- [ ] Spare parts inventory
- [ ] Work order approval workflow
- [ ] Predictive maintenance AI models
- [ ] Mobile app data sync
- [ ] Document version control
- [ ] Cost center allocation
