# Setup & Usage Instructions

## Files Created

```
BlazorApp1/
├── Scripts/
│   ├── DeleteMaintenanceSchedules.cs      ← EF Core C# script
│   └── DeleteMaintenanceSchedules.sql     ← Raw SQL script
├── ConsoleTools/
│   └── MaintenanceScriptRunner.cs         ← Interactive console tool
└── [Documentation files]
    ├── DELETE_SCHEDULES_GUIDE.md          ← Full guide
    └── DELETE_SCHEDULES_QUICK_REF.md      ← Quick reference
```

---

## Setup Instructions

### Step 1: Verify Files Exist
```bash
# Check if files are created
ls -la BlazorApp1/Scripts/DeleteMaintenanceSchedules.cs
ls -la BlazorApp1/ConsoleTools/MaintenanceScriptRunner.cs
```

### Step 2: Update Program.cs (if using console tool)
```csharp
// Add to your Program.cs if you want console tool support
var app = builder.Build();

// ... existing middleware ...

// For console tools, you might set up differently
```

### Step 3: Test Connection
```bash
cd BlazorApp1
dotnet build
```

---

## Usage Methods

### Method 1: Console Tool (Recommended)

#### Install/Build:
```bash
cd BlazorApp1
dotnet build
```

#### Run Commands:
```bash
# Preview data
dotnet run -- count
dotnet run -- count-by-status

# Delete data
dotnet run -- delete-all
dotnet run -- delete-by-status Completed
dotnet run -- delete-by-asset 5
dotnet run -- delete-before-date 2024-01-01

# Help
dotnet run -- help
```

---

### Method 2: Direct C# in Component

```csharp
@page "/admin/maintenance/cleanup"
@inject IDbContextFactory<ApplicationDbContext> ContextFactory

<h3>Maintenance Schedule Cleanup</h3>

<button @onclick="Preview">Preview Schedules to Delete</button>
<button @onclick="DeleteAll">Delete All Schedules</button>

@if (!string.IsNullOrEmpty(message))
{
    <p>@message</p>
}

@if (schedulesByStatus != null)
{
    <table>
        <tr>
            <th>Status</th>
            <th>Count</th>
        </tr>
        @foreach (var kvp in schedulesByStatus)
        {
            <tr>
                <td>@kvp.Key</td>
                <td>@kvp.Value</td>
            </tr>
        }
    </table>
}

@code {
    private Dictionary<string, int>? schedulesByStatus;
    private string message = "";

    private async Task Preview()
    {
        var script = new DeleteMaintenanceSchedulesScript(ContextFactory);
        schedulesByStatus = await script.GetScheduleCountByStatusAsync();
    }

    private async Task DeleteAll()
    {
        if (!confirm("Delete all schedules?")) return;
        
        var script = new DeleteMaintenanceSchedulesScript(ContextFactory);
        try
        {
            await script.DeleteAllSchedulesAsync();
            message = "✅ Deleted successfully";
            schedulesByStatus = null;
        }
        catch (Exception ex)
        {
            message = $"❌ Error: {ex.Message}";
        }
    }
}
```

---

### Method 3: SQL Server Management Studio

#### Step 1: Open SQL Script
1. Open SQL Server Management Studio (SSMS)
2. Connect to your database
3. Open file: `BlazorApp1\Scripts\DeleteMaintenanceSchedules.sql`

#### Step 2: Preview
```sql
-- Run these first to see what will be deleted:
SELECT COUNT(*) FROM MaintenanceSchedules;
SELECT Status, COUNT(*) FROM MaintenanceSchedules GROUP BY Status;
```

#### Step 3: Backup
```sql
-- Create backup (required!)
BACKUP DATABASE [BlazorApp1] 
TO DISK = 'C:\Backups\BlazorApp1_Before_Delete.bak';
```

#### Step 4: Delete
```sql
-- Choose one method:

-- Method 1: DELETE (slower, reversible)
DELETE FROM MaintenanceSchedules;

-- Method 2: TRUNCATE (faster, auto-resets ID)
TRUNCATE TABLE MaintenanceSchedules;
DBCC CHECKIDENT ('MaintenanceSchedules', RESEED, 0);
```

---

## Pre-Deletion Checklist

### ✅ Before Running Any Script:

1. **Backup Database**
   ```bash
   # SQL Server
   BACKUP DATABASE [BlazorApp1] 
   TO DISK = 'C:\Backups\BlazorApp1_Before_Delete.bak';
   ```

2. **Preview Data**
   ```bash
   # Using console tool
   dotnet run -- count
   dotnet run -- count-by-status
   ```

3. **Verify Environment**
   - [ ] Are you in the correct database?
   - [ ] Is this development/staging, not production?
   - [ ] Are no users currently using the system?

4. **Document Deletion**
   - Write down what's being deleted
   - Note the date and time
   - Save the backup location

5. **Inform Stakeholders**
   - Notify managers/team if shared database
   - Schedule during maintenance window if needed

---

## Post-Deletion Verification

### ✅ After Deletion:

```bash
# Verify deletion succeeded
dotnet run -- count
# Should show: 0

# Or in SQL:
SELECT COUNT(*) FROM MaintenanceSchedules;
-- Should return: 0
```

### Check table status:
```sql
-- Verify empty
SELECT COUNT(*) FROM MaintenanceSchedules;

-- Check last modified
SELECT * FROM MaintenanceSchedules;
-- Should return: (0 rows affected)

-- Verify identity seed (if using TRUNCATE)
DBCC CHECKIDENT ('MaintenanceSchedules');
```

---

## Troubleshooting

### Issue: "Script not found"
**Solution**: Ensure file exists in correct location
```bash
find . -name "DeleteMaintenanceSchedules.cs"
# Should find: ./BlazorApp1/Scripts/DeleteMaintenanceSchedules.cs
```

### Issue: "Database connection error"
**Solution**: Update connection string
1. Edit `appsettings.json`
2. Verify `DefaultConnection`
3. Test connection: `dotnet run -- count`

### Issue: "Foreign key constraint violation"
**Solution**: Delete related data first
```sql
-- Check what references MaintenanceSchedules
EXEC sp_fkeys 'MaintenanceSchedules'

-- Delete WorkOrders that reference MaintenanceSchedules
DELETE FROM WorkOrders 
WHERE ScheduleId IN (SELECT Id FROM MaintenanceSchedules);

-- Now delete MaintenanceSchedules
DELETE FROM MaintenanceSchedules;
```

### Issue: "Operation timed out"
**Solution**: Delete in smaller batches
```csharp
// Delete in chunks instead of all at once
while (true)
{
    var batch = context.MaintenanceSchedules
        .Take(1000)
        .ToList();
    if (batch.Count == 0) break;
    context.RemoveRange(batch);
    await context.SaveChangesAsync();
}
```

---

## Recovery Procedures

### If Deletion Was Accidental

#### Option 1: Rollback (if still in transaction)
```sql
-- Within same transaction window
ROLLBACK;
-- This will undo the deletion if not yet committed
```

#### Option 2: Restore from Backup
```sql
-- Restore entire database from backup
RESTORE DATABASE [BlazorApp1] 
FROM DISK = 'C:\Backups\BlazorApp1_Before_Delete.bak'
WITH REPLACE;
```

#### Option 3: Point-in-time Recovery
```sql
-- For Azure SQL or with transaction logs
RESTORE DATABASE [BlazorApp1] 
FROM DISK = 'C:\Backups\BlazorApp1.bak'
WITH RECOVERY, 
STOPATMARK = 'before_deletion';
```

---

## Automation (Optional)

### Schedule Regular Cleanup

#### Create SQL Agent Job:
```sql
-- Create job to delete old schedules monthly
USE msdb;
EXEC sp_add_job @job_name = 'Delete_Old_MaintenanceSchedules';

EXEC sp_add_jobstep
    @job_name = 'Delete_Old_MaintenanceSchedules',
    @step_name = 'Delete Completed Schedules',
    @command = 'DELETE FROM MaintenanceSchedules WHERE Status = ''Completed'' AND ModifiedDate < DATEADD(MONTH, -6, GETDATE())',
    @database_name = 'BlazorApp1';

EXEC sp_add_schedule 
    @schedule_name = 'Monthly_Cleanup',
    @freq_type = 4,
    @freq_interval = 1;
```

#### Or in C# (Recurring Task):
```csharp
// In Startup.cs or Hosted Service
services.AddHostedService<MaintenanceScheduleCleanupService>();

// CleanupService.cs
public class MaintenanceScheduleCleanupService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Delete schedules older than 6 months, once per month
            var cutoffDate = DateTime.Now.AddMonths(-6);
            var script = new DeleteMaintenanceSchedulesScript(...);
            await script.DeleteSchedulesBeforeDateAsync(cutoffDate);
            
            // Sleep for 30 days
            await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
        }
    }
}
```

---

## Monitoring & Logging

### Log Deletions:
```csharp
private readonly ILogger<DeleteMaintenanceSchedulesScript> _logger;

public async Task DeleteAllSchedulesAsync()
{
    var count = await context.MaintenanceSchedules.CountAsync();
    _logger.LogWarning($"Deleting {count} MaintenanceSchedules records");
    
    try
    {
        await context.MaintenanceSchedules.ExecuteDeleteAsync();
        _logger.LogInformation($"Successfully deleted {count} schedules");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to delete MaintenanceSchedules");
        throw;
    }
}
```

### Create Audit Trail:
```sql
-- Add deletion log table
CREATE TABLE MaintenanceSchedules_DeleteLog (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DeletedCount INT,
    DeletedBy NVARCHAR(256),
    DeletedDate DATETIME,
    Reason NVARCHAR(500)
);

-- Log deletion
INSERT INTO MaintenanceSchedules_DeleteLog 
VALUES (156, 'admin@company.com', GETDATE(), 'Data cleanup');
```

---

## Support Resources

| File | Purpose |
|------|---------|
| `DELETE_SCHEDULES_GUIDE.md` | Complete detailed guide |
| `DELETE_SCHEDULES_QUICK_REF.md` | Quick command reference |
| `Scripts/DeleteMaintenanceSchedules.cs` | C# implementation |
| `Scripts/DeleteMaintenanceSchedules.sql` | SQL implementation |
| `ConsoleTools/MaintenanceScriptRunner.cs` | Interactive console tool |

---

## Summary

**Quick Delete**:
```bash
# Preview
dotnet run -- count

# Delete
dotnet run -- delete-all
```

**Safe Delete (SQL)**:
```sql
-- Backup first!
BACKUP DATABASE [BlazorApp1] 
TO DISK = 'C:\Backups\BlazorApp1_Before_Delete.bak';

-- Then delete
DELETE FROM MaintenanceSchedules;
```

**Always remember**: 
✅ Backup first
✅ Test in development  
✅ Preview before deleting
✅ Verify after deletion

---

**Status**: ✅ READY TO USE
**Last Updated**: December 2024

