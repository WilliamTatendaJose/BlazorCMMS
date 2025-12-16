# ?? Assets Module - Quick Reference

## Access

```
URL: /rbm/assets
Navigation: RBM Layout ? Assets Management
```

## Key Statistics

| Metric | Description |
|--------|-------------|
| Total Assets | All non-retired assets |
| Active Assets | Non-retired + IsActive=true |
| Critical | Status="Critical" \| Criticality="Critical" |
| Avg Health | Average health score % |
| Overdue Maintenance | NextMaintenance < Today |

## Search

| Type | Example |
|------|---------|
| ID | "PMP-011" |
| Name | "Pump" |
| Location | "Building 1" |
| Model | "Model-X" |
| Serial | "SN123456" |

## Filters

```
Criticality: Low, Medium, High, Critical
Status: Healthy, Warning, Critical, Maintenance
```

## Color Coding

### Status
- ?? **Healthy** - #43a047
- ?? **Warning** - #fb8c00
- ?? **Critical** - #e53935
- ?? **Maintenance** - #1e88e5
- ? **Retired** - #90a4ae

### Criticality
- ?? **Low** - #43a047
- ?? **Medium** - #fb8c00
- ?? **High** - #ff6f00
- ?? **Critical** - #e53935

### Health
- ?? **80+** - Healthy
- ?? **60-79** - Warning
- ?? **<60** - Critical

## Asset Lifecycle

```
Create ? Active ? Monitor ? Maintain ? Retire ? Reactivate
```

### Status Flow
```
Healthy ? Warning ? Critical ? Maintenance ? Retired (one-way)
         (auto-calculated from health score)
```

## Data Fields

### Required
- Asset ID (max 50 chars)
- Name (max 200 chars)

### Common
- Location
- Department  
- Manufacturer
- Model Number
- Serial Number
- Criticality
- Health Score

### Tracking
- Last Maintenance
- Next Scheduled Maintenance
- Uptime %
- Downtime %

### System
- Status (auto)
- Is Active
- Is Retired
- Created Date
- Modified Date

## Actions

| Button | Action |
|--------|--------|
| ? Add Asset | Create new |
| ??? View | View details |
| ?? Edit | Modify asset |
| ??? Retire | Mark as retired |
| ?? Reactivate | Unretire asset |
| ?? Schedule | Schedule maintenance |

## Calculations

### Auto Status
```
Health ? 80 ? Healthy
Health 60-79 ? Warning
Health < 60 ? Critical
```

### Maintenance Days
```
DaysUntilMaintenance = NextDate - Today
DaysSinceLastMaintenance = Today - LastDate
IsOverdue = NextDate < Today
```

## DataService Methods

### Get
```csharp
GetAssets()                      // Active only
GetAsset(id)                     // With related data
GetAllAssets(includeRetired)     // Can include retired
GetAssetByAssetId(assetId)       // Search by ID
```

### Search & Filter
```csharp
SearchAssets(term)               // Full-text
GetAssetsByLocation(loc)         // By location
GetAssetsByDepartment(dept)      // By department
GetAssetsByCriticality(level)    // By criticality
GetCriticalAssets()              // All critical
```

### Maintenance
```csharp
GetAssetsNeedingMaintenance()    // Due within 7 days
GetOverdueMaintenance()          // Past due date
GetLowHealthScoreAssets(60)      // Below threshold
```

### Stats
```csharp
GetTotalAssets()                 // Count
GetActiveAssets()                // Count
GetCriticalAssetsCount()         // Count
GetOverdueMaintenanceCount()     // Count
GetAverageHealthScore()          // Average
```

### CRUD
```csharp
AddAsset(asset)                  // Create
UpdateAsset(asset)               // Update
DeleteAsset(id)                  // Soft delete
RetireAsset(id)                  // Mark retired
ReactivateAsset(id)              // Unretire
```

## Validations

| Field | Rules |
|-------|-------|
| Asset ID | Required, Max 50 |
| Name | Required, Max 200 |
| Health | 0-100 |
| Uptime | 0-100 |
| Location | Max 500 |
| Department | Max 200 |

## Tips & Tricks

### Search Multiple Ways
- By asset ID
- By equipment name
- By location
- By model number
- By serial number

### Filter Combinations
```
Search "Pump" + Criticality "Critical" + Status "Warning"
= Critical warning pumps
```

### Bulk Identify Issues
1. Open Assets
2. Filter Criticality = "Critical"
3. Sort by Health Score
4. Identify lowest health first

### Track Maintenance
1. Set Next Scheduled Maintenance
2. System auto-calculates days
3. Shows overdue when past date
4. Visual alert with ??

### Monitor Health
1. Health score auto ? Status
2. Scores below 60 = Critical
3. Use for priority decisions
4. Track trends over time

## Permissions

```csharp
CurrentUser.CanEdit     // Edit/Add/Retire
CurrentUser.CanDelete   // Retire asset
```

## Soft Delete

- Not truly deleted
- Marked as IsRetired=true
- Historical data preserved
- Can be reactivated
- Won't show in lists

## Database

### Table: Assets

```
Id (PK)
AssetId (indexed)
Name
Description
Location
Department
EquipmentManufacturer
ModelNumber
SerialNumber
ManufactureDate
InstallationDate
Criticality (indexed)
HealthScore
Status (indexed)
LastMaintenance
NextScheduledMaintenance
Uptime
Downtime
TotalDowntimeMinutes
MaintenanceCount
IsActive
IsRetired
RetirementDate
CreatedDate
ModifiedDate
CreatedBy
ModifiedBy
```

## Common Use Cases

### Scenario 1: New Equipment
```
1. Click Add Asset
2. Enter Asset ID & Name
3. Set location & department
4. Enter manufacturer details
5. Set criticality = High (if important)
6. Create
```

### Scenario 2: Schedule Maintenance
```
1. View asset details
2. Set "Next Scheduled Maintenance"
3. Days auto-calculated
4. System alerts when overdue
```

### Scenario 3: Find Critical Assets
```
1. Filter by Status = "Critical"
2. Sort by Health Score
3. Act on lowest health first
```

### Scenario 4: Track Deprecation
```
1. Create asset with dates
2. Track health over time
3. Retire when obsolete
4. Can reactivate if needed
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Asset not found | Check IsRetired flag |
| Search not working | Verify IsRetired=false |
| Status wrong | Check health score |
| Date empty | Add installation/manufacture date |
| Can't edit | Check permissions |
| Can't delete | Use Retire instead |

## Performance Tips

1. **Search optimized** - Full-text on ID, Name, Location
2. **Filters indexed** - Status, Criticality columns
3. **Pagination ready** - Can add pagination
4. **Caching possible** - Statistics can be cached
5. **Async ready** - All queries ready for async

## Future Features

- [ ] Maintenance schedules
- [ ] Bulk import
- [ ] QR codes
- [ ] Mobile app
- [ ] Analytics
- [ ] Predictions
- [ ] Attachments
- [ ] Timeline view

---

**Everything you need to manage assets! ??**
