# Delete MaintenanceSchedules - Quick Reference

## 🚨 QUICK START

### Safest Method (Console Tool):
```bash
# Preview without deleting
dotnet run -- count

# Delete with confirmation prompts
dotnet run -- delete-all
```

### SQL Method:
```sql
-- Preview
SELECT COUNT(*) FROM MaintenanceSchedules;

-- Delete
DELETE FROM MaintenanceSchedules;
-- OR (faster)
TRUNCATE TABLE MaintenanceSchedules;
```

---

## Command Reference

### Preview (NO Deletion)
```bash
dotnet run -- count                    # Total count
dotnet run -- count-by-status          # Count by status
```

### Delete All
```bash
dotnet run -- delete-all               # Delete everything
```

### Delete by Status
```bash
dotnet run -- delete-by-status Completed
dotnet run -- delete-by-status Scheduled
```

### Delete by Asset
```bash
dotnet run -- delete-by-asset 5        # Asset ID = 5
```

### Delete Before Date
```bash
dotnet run -- delete-before-date 2024-01-01
```

---

## SQL Quick Commands

```sql
-- COUNT
SELECT COUNT(*) FROM MaintenanceSchedules;

-- COUNT BY STATUS
SELECT Status, COUNT(*) AS Count 
FROM MaintenanceSchedules 
GROUP BY Status;

-- DELETE ALL (SAFE - reversible in transaction)
DELETE FROM MaintenanceSchedules;

-- DELETE FAST (TRUNCATE - cannot undo)
TRUNCATE TABLE MaintenanceSchedules;

-- DELETE BY STATUS
DELETE FROM MaintenanceSchedules 
WHERE Status = 'Completed';

-- DELETE BEFORE DATE
DELETE FROM MaintenanceSchedules 
WHERE ScheduledDate < '2024-01-01';

-- DELETE FOR ASSET
DELETE FROM MaintenanceSchedules 
WHERE AssetId = 5;

-- RESET IDENTITY
DBCC CHECKIDENT ('MaintenanceSchedules', RESEED, 0);
```

---

## C# Code Examples

### In Component/Service:
```csharp
var script = new DeleteMaintenanceSchedulesScript(contextFactory);

// Delete all
await script.DeleteAllSchedulesAsync();

// Delete by status
await script.DeleteSchedulesByStatusAsync("Completed");

// Delete by asset
await script.DeleteSchedulesByAssetAsync(5);

// Delete before date
await script.DeleteSchedulesBeforeDateAsync(new DateTime(2024, 1, 1));

// Preview
var count = await script.GetScheduleCountAsync();
var byStatus = await script.GetScheduleCountByStatusAsync();
```

---

## Safety Checklist

- [ ] Backup database
- [ ] Test in development
- [ ] Preview data first
- [ ] Confirm deletion intent
- [ ] Check correct environment
- [ ] No active users
- [ ] Have restore plan

---

## Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| Timeout | Delete in batches (smaller chunks) |
| Foreign Key Error | Delete related tables first |
| Cannot connect | Check connection string |
| Accidentally deleted? | Restore from backup |
| Want to undo? | Stop, close connection, rollback (if in transaction) |

---

## File Locations

| File | Purpose |
|------|---------|
| `Scripts/DeleteMaintenanceSchedules.cs` | EF Core C# script |
| `Scripts/DeleteMaintenanceSchedules.sql` | Raw SQL script |
| `ConsoleTools/MaintenanceScriptRunner.cs` | Interactive console tool |
| `DELETE_SCHEDULES_GUIDE.md` | Full documentation |

---

## Workflow

```
START
  ↓
[Backup Database] ← CRITICAL!
  ↓
[Run Preview Command]
  → dotnet run -- count
  ↓
[Review Results]
  ↓
[Confirm Deletion]
  ↓
[Run Delete Command]
  → dotnet run -- delete-all
  ↓
[Verify Results]
  → dotnet run -- count
  ↓
END
```

---

## Recovery

If you accidentally deleted important data:

```sql
-- OPTION 1: Rollback (if in transaction)
ROLLBACK;

-- OPTION 2: Restore from backup
RESTORE DATABASE [BlazorApp1] 
FROM DISK = 'C:\Backups\BlazorApp1_Before_Delete.bak';
```

---

**⚠️ ALWAYS BACKUP FIRST!**

