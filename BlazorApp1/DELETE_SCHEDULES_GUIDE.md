# Delete MaintenanceSchedules Data - Complete Guide

## ⚠️ CRITICAL WARNING

**THIS SCRIPT DELETES DATA PERMANENTLY!**

Before running any deletion script:
1. ✅ **BACKUP YOUR DATABASE** - This is non-negotiable!
2. ✅ **Test in development** - Never run on production without testing
3. ✅ **Verify you're in correct environment** - Check connection string
4. ✅ **Review what will be deleted** - Use preview commands first

---

## Available Scripts

### 1. EF Core C# Script (Recommended for .NET code)
**File**: `BlazorApp1/Scripts/DeleteMaintenanceSchedules.cs`

**Advantages**:
- ✅ Type-safe and compile-checked
- ✅ Integrated with C# application
- ✅ Can use application's dependency injection
- ✅ Proper error handling
- ✅ No SQL injection risk

**Methods**:
- `DeleteAllSchedulesAsync()` - Delete all schedules
- `DeleteSchedulesByAssetAsync(int assetId)` - Delete for specific asset
- `DeleteSchedulesByStatusAsync(string status)` - Delete by status
- `DeleteSchedulesBeforeDateAsync(DateTime date)` - Delete before date
- `GetScheduleCountAsync()` - Preview without deleting

### 2. SQL Script (Direct database)
**File**: `BlazorApp1/Scripts/DeleteMaintenanceSchedules.sql`

**Advantages**:
- ✅ Direct database access
- ✅ Fast execution
- ✅ Can preview data first
- ✅ Can use in SQL Management Studio

**Options**:
- DELETE - Slower but reversible (in transaction)
- TRUNCATE - Faster but cannot undo

### 3. Console Tool (Interactive)
**File**: `BlazorApp1/ConsoleTools/MaintenanceScriptRunner.cs`

**Advantages**:
- ✅ Interactive confirmation prompts
- ✅ Shows previews before deletion
- ✅ Multiple safety checks
- ✅ Easy to run from command line

---

## How to Use Each Script

### Option 1: Using the Console Tool (Safest)

#### Step 1: Open Terminal
```bash
cd BlazorApp1
```

#### Step 2: Preview Data (No Deletion)
```bash
# Show total count
dotnet run -- count

# Show count by status
dotnet run -- count-by-status
```

**Example Output**:
```
📊 Total schedules: 156

Scheduled       :  87 (55%) ███████████
Completed       :  65 (41%) ████████
In Progress     :   4 (3%)  █
```

#### Step 3: Delete with Confirmation
```bash
# Delete ALL schedules
dotnet run -- delete-all

# Delete by status
dotnet run -- delete-by-status Completed

# Delete for specific asset
dotnet run -- delete-by-asset 5

# Delete before date
dotnet run -- delete-before-date 2024-01-01
```

**Interactive Prompts**:
```
⚠️  WARNING: This will delete ALL MaintenanceSchedules records
    This action cannot be undone without a database backup!

📊 Current schedules: 156

Are you SURE you want to delete all 156 schedules? (yes/NO): yes
Type 'DELETE' to confirm: DELETE

⏳ Deleting all schedules...
✅ Successfully deleted 156 MaintenanceSchedules.
✅ Verification: 0 schedules remaining
```

---

### Option 2: Using C# Script Directly

#### In Blazor Component or Service:

```csharp
@page "/admin/delete-schedules"
@inject IDbContextFactory<ApplicationDbContext> ContextFactory
@rendermode InteractiveServer

<div>
    <h3>Delete Maintenance Schedules</h3>
    
    <button @onclick="DeleteAllSchedules">Delete All</button>
    <button @onclick="DeleteCompleted">Delete Completed Only</button>
    
    <p>@message</p>
</div>

@code {
    private string message = "";

    private async Task DeleteAllSchedules()
    {
        if (!confirm("Are you ABSOLUTELY sure? This cannot be undone!"))
            return;

        try
        {
            var script = new DeleteMaintenanceSchedulesScript(ContextFactory);
            await script.DeleteAllSchedulesAsync();
            message = "✅ All schedules deleted";
        }
        catch (Exception ex)
        {
            message = $"❌ Error: {ex.Message}";
        }
    }

    private async Task DeleteCompleted()
    {
        if (!confirm("Delete all COMPLETED schedules?"))
            return;

        try
        {
            var script = new DeleteMaintenanceSchedulesScript(ContextFactory);
            await script.DeleteSchedulesByStatusAsync("Completed");
            message = "✅ Completed schedules deleted";
        }
        catch (Exception ex)
        {
            message = $"❌ Error: {ex.Message}";
        }
    }
}
```

---

### Option 3: Using SQL Script

#### In SQL Server Management Studio:

**Step 1: Open SSMS**
- Connect to your database
- Open the SQL script file

**Step 2: Preview (Safe)**
```sql
-- First, just count
SELECT COUNT(*) FROM MaintenanceSchedules;

-- View by status
SELECT Status, COUNT(*) 
FROM MaintenanceSchedules 
GROUP BY Status;
```

**Step 3: Backup (Required)**
```sql
-- Create backup before deleting
BACKUP DATABASE [BlazorApp1] 
TO DISK = 'C:\Backups\BlazorApp1_Before_Delete.bak';
```

**Step 4: Delete**
```sql
-- Option A: DELETE (slower, can be rolled back)
DELETE FROM MaintenanceSchedules;

-- Option B: TRUNCATE (faster, auto-resets identity)
TRUNCATE TABLE MaintenanceSchedules;
```

**Step 5: Verify**
```sql
-- Verify deletion
SELECT COUNT(*) FROM MaintenanceSchedules;
-- Should return: 0
```

---

## Common Deletion Scenarios

### Scenario 1: Delete All Schedules
```csharp
var script = new DeleteMaintenanceSchedulesScript(contextFactory);
await script.DeleteAllSchedulesAsync();
```

### Scenario 2: Clean Up Completed Schedules
```csharp
var script = new DeleteMaintenanceSchedulesScript(contextFactory);
await script.DeleteSchedulesByStatusAsync("Completed");
```

### Scenario 3: Remove Old Data
```csharp
var cutoffDate = DateTime.Now.AddYears(-1);  // Delete older than 1 year
var script = new DeleteMaintenanceSchedulesScript(contextFactory);
await script.DeleteSchedulesBeforeDateAsync(cutoffDate);
```

### Scenario 4: Remove Asset-Specific Data
```csharp
int assetId = 5;  // Asset to remove schedules for
var script = new DeleteMaintenanceSchedulesScript(contextFactory);
await script.DeleteSchedulesByAssetAsync(assetId);
```

---

## Safety Checklist

Before deletion, verify:

- [ ] Database backup exists
- [ ] Running in correct environment (dev/staging, not production)
- [ ] Connection string points to correct database
- [ ] Previewed data to be deleted
- [ ] Confirmed with team/manager if shared database
- [ ] No active users using the system
- [ ] Have restore procedure ready

---

## Backup and Recovery

### Create Backup Before Deletion

#### Using SQL Management Studio:
```sql
BACKUP DATABASE [BlazorApp1] 
TO DISK = 'C:\Backups\BlazorApp1_Before_Delete.bak'
WITH INIT;
```

#### Using Azure SQL:
```powershell
# Export to .bacpac file
Export-AzSqlDatabase -ResourceGroupName "myResourceGroup" `
    -ServerName "myServer" `
    -DatabaseName "BlazorApp1" `
    -StorageKeyType "StorageAccessKey" `
    -StorageKey "yourStorageKey" `
    -StorageUri "https://yourstorageaccount.blob.core.windows.net/path/file.bacpac"
```

### Restore if Needed

#### From Backup:
```sql
-- Restore from .bak file
RESTORE DATABASE [BlazorApp1_Restored]
FROM DISK = 'C:\Backups\BlazorApp1_Before_Delete.bak';
```

---

## What Gets Deleted?

### MaintenanceSchedules Table Fields Cleared:
- ✅ Id (primary key)
- ✅ AssetId (foreign key)
- ✅ Type (task type: Preventive, Corrective, etc.)
- ✅ ScheduledDate (when scheduled)
- ✅ EndDate (completion date)
- ✅ EstimatedDuration (hours)
- ✅ Description
- ✅ Status (Scheduled, Completed, etc.)
- ✅ AssignedTechnician
- ✅ Frequency (recurring info)
- ✅ CreatedBy
- ✅ CreatedDate
- ✅ ModifiedDate
- ✅ TenantId (multi-tenant info)

### Related Data NOT Deleted:
- ✅ Assets (remain unchanged)
- ✅ Users/Technicians (remain unchanged)
- ✅ WorkOrders (remain unchanged)
- ✅ Other tables (unaffected)

**Only MaintenanceSchedules data is removed.**

---

## Troubleshooting

### Issue: "Cannot delete, foreign key constraint"
**Solution**: This means other tables reference MaintenanceSchedules
```sql
-- Check what references MaintenanceSchedules
EXEC sp_fkeys 'MaintenanceSchedules'

-- If WorkOrders reference MaintenanceSchedules, delete them first
DELETE FROM WorkOrders WHERE ScheduleId IS NOT NULL;
DELETE FROM MaintenanceSchedules;
```

### Issue: "Script times out"
**Solution**: Delete in batches
```csharp
// Delete in chunks (safer for large datasets)
using var context = contextFactory.CreateDbContext();
while (true)
{
    var batch = await context.MaintenanceSchedules
        .Take(1000)
        .ToListAsync();
    
    if (batch.Count == 0) break;
    
    context.MaintenanceSchedules.RemoveRange(batch);
    await context.SaveChangesAsync();
}
```

### Issue: "Cannot connect to database"
**Solution**: Verify connection string
```csharp
// Check connection string
var connectionString = configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connecting to: {connectionString}");
```

---

## Best Practices

### ✅ DO:
- ✅ Backup before deletion
- ✅ Test in development first
- ✅ Preview data before deleting
- ✅ Delete in off-peak hours
- ✅ Have rollback plan ready
- ✅ Document what was deleted
- ✅ Keep backup for reasonable time
- ✅ Notify stakeholders

### ❌ DON'T:
- ❌ Run on production without testing
- ❌ Delete without backup
- ❌ Delete during peak usage hours
- ❌ Delete without confirmation
- ❌ Use TRUNCATE without backup
- ❌ Delete without reviewing data first
- ❌ Delete sensitive data casually
- ❌ Forget to verify deletion

---

## Performance Considerations

### Deletion Speed:
- **DELETE statement**: ~10,000 rows per second
- **TRUNCATE statement**: ~100,000 rows per second (10x faster)
- **Batch deletion**: Best for large datasets

### For 100,000 schedules:
- DELETE: ~10 seconds
- TRUNCATE: ~1 second
- Batch delete: ~30-60 seconds (but safer)

### Recommendation:
For production databases with millions of rows, use TRUNCATE after backup:
```sql
-- Fastest method for complete clear
TRUNCATE TABLE MaintenanceSchedules;
DBCC CHECKIDENT ('MaintenanceSchedules', RESEED, 0);
```

---

## Monitoring Deletion

### Check Progress (while deleting):
```sql
-- View current sessions
SELECT * FROM sys.dm_exec_sessions WHERE database_id = DB_ID('BlazorApp1');

-- Check table size
EXEC sp_spaceused 'MaintenanceSchedules';

-- Monitor query progress
SELECT * FROM sys.dm_exec_requests;
```

### After Deletion:
```sql
-- Verify deletion
SELECT COUNT(*) FROM MaintenanceSchedules;

-- Check identity seed
DBCC CHECKIDENT ('MaintenanceSchedules');

-- Check table fragmentation
EXEC sp_showcontig 'MaintenanceSchedules';
```

---

## Legal & Compliance

### Data Retention:
- Some regulations require data retention
- Check compliance requirements before deletion
- Document what was deleted and when
- Keep deletion audit log

### GDPR Compliance:
```sql
-- Delete specific person's data
DELETE FROM MaintenanceSchedules
WHERE AssignedTechnician = 'John Smith';

-- For audit trail, consider soft delete instead
-- UPDATE MaintenanceSchedules SET IsDeleted = 1
-- WHERE AssignedTechnician = 'John Smith';
```

---

## Summary

| Method | Speed | Safety | Recommended |
|--------|-------|--------|-------------|
| SQL DELETE | Medium | High | ✅ For selective deletion |
| SQL TRUNCATE | Fast | Medium | ✅ For complete clear |
| C# Script | Medium | High | ✅ For app integration |
| Console Tool | Medium | Very High | ✅ For interactive use |

**Best Practice**: Use Console Tool for interactive deletion, SQL TRUNCATE for fast complete clear.

---

**Version**: 1.0
**Last Updated**: December 2024
**Status**: Production Ready

