-- SQL Script to Delete All MaintenanceSchedules Data
-- Database: BlazorApp1
-- Table: MaintenanceSchedules

-- ⚠️ WARNING: BACKUP DATABASE BEFORE RUNNING!
-- This will permanently delete all data in MaintenanceSchedules table

-- ═══════════════════════════════════════════════════════════
-- OPTION 1: Delete ALL MaintenanceSchedules
-- ═══════════════════════════════════════════════════════════

DELETE FROM MaintenanceSchedules;

-- Alternative syntax (same result):
-- TRUNCATE TABLE MaintenanceSchedules;

-- ═══════════════════════════════════════════════════════════
-- OPTION 2: Delete by Status
-- ═══════════════════════════════════════════════════════════

-- Delete all completed schedules
DELETE FROM MaintenanceSchedules
WHERE Status = 'Completed';

-- Delete all scheduled (not yet completed)
DELETE FROM MaintenanceSchedules
WHERE Status = 'Scheduled';

-- Delete specific status
DELETE FROM MaintenanceSchedules
WHERE Status = 'Cancelled';

-- ═══════════════════════════════════════════════════════════
-- OPTION 3: Delete by Date Range
-- ═══════════════════════════════════════════════════════════

-- Delete schedules before a specific date
DELETE FROM MaintenanceSchedules
WHERE ScheduledDate < '2024-01-01';

-- Delete schedules after a specific date
DELETE FROM MaintenanceSchedules
WHERE ScheduledDate > '2024-12-31';

-- Delete schedules in a date range
DELETE FROM MaintenanceSchedules
WHERE ScheduledDate BETWEEN '2024-01-01' AND '2024-12-31';

-- ═══════════════════════════════════════════════════════════
-- OPTION 4: Delete by Asset
-- ═══════════════════════════════════════════════════════════

-- Delete schedules for a specific asset (AssetId = 5)
DELETE FROM MaintenanceSchedules
WHERE AssetId = 5;

-- Delete schedules for multiple assets
DELETE FROM MaintenanceSchedules
WHERE AssetId IN (1, 2, 3);

-- ═══════════════════════════════════════════════════════════
-- OPTION 5: Safe Preview (No Deletion)
-- ═══════════════════════════════════════════════════════════

-- Count total schedules
SELECT COUNT(*) AS TotalSchedules
FROM MaintenanceSchedules;

-- Count by status
SELECT Status, COUNT(*) AS Count
FROM MaintenanceSchedules
GROUP BY Status
ORDER BY Count DESC;

-- Count by asset
SELECT AssetId, COUNT(*) AS Count
FROM MaintenanceSchedules
GROUP BY AssetId
ORDER BY Count DESC;

-- Count by date range
SELECT 
    CAST(ScheduledDate AS DATE) AS ScheduleDate,
    COUNT(*) AS Count
FROM MaintenanceSchedules
GROUP BY CAST(ScheduledDate AS DATE)
ORDER BY ScheduleDate DESC;

-- ═══════════════════════════════════════════════════════════
-- OPTION 6: Reset Identity Seed (After Deletion)
-- ═══════════════════════════════════════════════════════════

-- This resets the auto-increment ID to start from 1 again
DBCC CHECKIDENT ('MaintenanceSchedules', RESEED, 0);

-- ═══════════════════════════════════════════════════════════
-- RECOMMENDED WORKFLOW
-- ═══════════════════════════════════════════════════════════

-- Step 1: Take a backup
-- BACKUP DATABASE [BlazorApp1] TO DISK = 'C:\Backups\BlazorApp1_Before_Delete.bak';

-- Step 2: Preview data to be deleted
-- SELECT COUNT(*) FROM MaintenanceSchedules;

-- Step 3: Delete the data
-- DELETE FROM MaintenanceSchedules;

-- Step 4: Verify deletion
-- SELECT COUNT(*) FROM MaintenanceSchedules;

-- Step 5: Reset identity (optional)
-- DBCC CHECKIDENT ('MaintenanceSchedules', RESEED, 0);

-- ═══════════════════════════════════════════════════════════
-- DIFFERENCES: DELETE vs TRUNCATE
-- ═══════════════════════════════════════════════════════════

-- DELETE:
--   - Slower (logs each deletion)
--   - Can have WHERE clause
--   - Can rollback if in transaction
--   - Identity not reset

-- TRUNCATE:
--   - Faster (single log entry)
--   - No WHERE clause allowed
--   - Cannot rollback (in most cases)
--   - Identity resets automatically

-- Use DELETE for selective deletions
-- Use TRUNCATE for complete table clear (faster)

-- ═══════════════════════════════════════════════════════════
-- DANGER! ONLY IF YOU'RE ABSOLUTELY SURE
-- ═══════════════════════════════════════════════════════════

-- WARNING: This deletes ALL data permanently!
-- TRUNCATE TABLE MaintenanceSchedules;
-- DBCC CHECKIDENT ('MaintenanceSchedules', RESEED, 0);
