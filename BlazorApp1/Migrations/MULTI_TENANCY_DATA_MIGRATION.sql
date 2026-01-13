-- =====================================================
-- Multi-Tenancy Data Migration Script
-- Run this script to add TenantId columns to all tables
-- and update existing data with proper tenant assignments
-- =====================================================

-- Step 1: Add TenantId columns to tables that don't have them
-- (Run these only if the columns don't exist)

-- Check and add TenantId to Users table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'TenantId')
BEGIN
    ALTER TABLE Users ADD TenantId INT NULL;
    CREATE INDEX IX_Users_TenantId ON Users(TenantId);
    PRINT 'Added TenantId to Users table';
END

-- Check and add TenantId to AssetDowntime table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AssetDowntime') AND name = 'TenantId')
BEGIN
    ALTER TABLE AssetDowntime ADD TenantId INT NULL;
    CREATE INDEX IX_AssetDowntime_TenantId ON AssetDowntime(TenantId);
    PRINT 'Added TenantId to AssetDowntime table';
END

-- Check and add TenantId to ReliabilityMetrics table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('ReliabilityMetrics') AND name = 'TenantId')
BEGIN
    ALTER TABLE ReliabilityMetrics ADD TenantId INT NULL;
    CREATE INDEX IX_ReliabilityMetrics_TenantId ON ReliabilityMetrics(TenantId);
    PRINT 'Added TenantId to ReliabilityMetrics table';
END

-- Check and add TenantId to MaintenanceTasks table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('MaintenanceTasks') AND name = 'TenantId')
BEGIN
    ALTER TABLE MaintenanceTasks ADD TenantId INT NULL;
    CREATE INDEX IX_MaintenanceTasks_TenantId ON MaintenanceTasks(TenantId);
    PRINT 'Added TenantId to MaintenanceTasks table';
END

-- Check and add TenantId to AssetAttachments table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AssetAttachments') AND name = 'TenantId')
BEGIN
    ALTER TABLE AssetAttachments ADD TenantId INT NULL;
    CREATE INDEX IX_AssetAttachments_TenantId ON AssetAttachments(TenantId);
    PRINT 'Added TenantId to AssetAttachments table';
END

-- Check and add TenantId to SparePartTransactions table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('SparePartTransactions') AND name = 'TenantId')
BEGIN
    ALTER TABLE SparePartTransactions ADD TenantId INT NULL;
    CREATE INDEX IX_SparePartTransactions_TenantId ON SparePartTransactions(TenantId);
    PRINT 'Added TenantId to SparePartTransactions table';
END

-- Check and add TenantId to DocumentAccessLogs table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DocumentAccessLogs') AND name = 'TenantId')
BEGIN
    ALTER TABLE DocumentAccessLogs ADD TenantId INT NULL;
    CREATE INDEX IX_DocumentAccessLogs_TenantId ON DocumentAccessLogs(TenantId);
    PRINT 'Added TenantId to DocumentAccessLogs table';
END

-- Check and add TenantId to NotificationSettings table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('NotificationSettings') AND name = 'TenantId')
BEGIN
    ALTER TABLE NotificationSettings ADD TenantId INT NULL;
    CREATE INDEX IX_NotificationSettings_TenantId ON NotificationSettings(TenantId);
    PRINT 'Added TenantId to NotificationSettings table';
END

-- Check and add TenantId to NotificationLogs table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('NotificationLogs') AND name = 'TenantId')
BEGIN
    ALTER TABLE NotificationLogs ADD TenantId INT NULL;
    CREATE INDEX IX_NotificationLogs_TenantId ON NotificationLogs(TenantId);
    PRINT 'Added TenantId to NotificationLogs table';
END

-- Check and add TenantId to UserSettings table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UserSettings') AND name = 'TenantId')
BEGIN
    ALTER TABLE UserSettings ADD TenantId INT NULL;
    CREATE INDEX IX_UserSettings_TenantId ON UserSettings(TenantId);
    PRINT 'Added TenantId to UserSettings table';
END

-- =====================================================
-- Step 2: Update existing data with TenantId from parent entities
-- =====================================================

-- Update AssetDowntime with TenantId from Assets
UPDATE ad
SET ad.TenantId = a.TenantId
FROM AssetDowntime ad
INNER JOIN Assets a ON ad.AssetId = a.Id
WHERE ad.TenantId IS NULL AND a.TenantId IS NOT NULL;
PRINT 'Updated AssetDowntime TenantId from Assets';

-- Update ReliabilityMetrics with TenantId from Assets
UPDATE rm
SET rm.TenantId = a.TenantId
FROM ReliabilityMetrics rm
INNER JOIN Assets a ON rm.AssetId = a.Id
WHERE rm.TenantId IS NULL AND a.TenantId IS NOT NULL;
PRINT 'Updated ReliabilityMetrics TenantId from Assets';

-- Update AssetAttachments with TenantId from Assets
UPDATE aa
SET aa.TenantId = a.TenantId
FROM AssetAttachments aa
INNER JOIN Assets a ON aa.AssetId = a.Id
WHERE aa.TenantId IS NULL AND a.TenantId IS NOT NULL;
PRINT 'Updated AssetAttachments TenantId from Assets';

-- Update ConditionReadings with TenantId from Assets
UPDATE cr
SET cr.TenantId = a.TenantId
FROM ConditionReadings cr
INNER JOIN Assets a ON cr.AssetId = a.Id
WHERE cr.TenantId IS NULL AND a.TenantId IS NOT NULL;
PRINT 'Updated ConditionReadings TenantId from Assets';

-- Update FailureModes with TenantId from Assets
UPDATE fm
SET fm.TenantId = a.TenantId
FROM FailureModes fm
INNER JOIN Assets a ON fm.AssetId = a.Id
WHERE fm.TenantId IS NULL AND a.TenantId IS NOT NULL;
PRINT 'Updated FailureModes TenantId from Assets';

-- Update MaintenanceSchedules with TenantId from Assets
UPDATE ms
SET ms.TenantId = a.TenantId
FROM MaintenanceSchedules ms
INNER JOIN Assets a ON ms.AssetId = a.Id
WHERE ms.TenantId IS NULL AND a.TenantId IS NOT NULL;
PRINT 'Updated MaintenanceSchedules TenantId from Assets';

-- Update MaintenanceTasks with TenantId from WorkOrders
UPDATE mt
SET mt.TenantId = wo.TenantId
FROM MaintenanceTasks mt
INNER JOIN WorkOrders wo ON mt.WorkOrderId = wo.Id
WHERE mt.TenantId IS NULL AND wo.TenantId IS NOT NULL;
PRINT 'Updated MaintenanceTasks TenantId from WorkOrders';

-- Update SparePartTransactions with TenantId from SpareParts
UPDATE spt
SET spt.TenantId = sp.TenantId
FROM SparePartTransactions spt
INNER JOIN SpareParts sp ON spt.SparePartId = sp.Id
WHERE spt.TenantId IS NULL AND sp.TenantId IS NOT NULL;
PRINT 'Updated SparePartTransactions TenantId from SpareParts';

-- Update DocumentAccessLogs with TenantId from Documents
UPDATE dal
SET dal.TenantId = d.TenantId
FROM DocumentAccessLogs dal
INNER JOIN Documents d ON dal.DocumentId = d.Id
WHERE dal.TenantId IS NULL AND d.TenantId IS NOT NULL;
PRINT 'Updated DocumentAccessLogs TenantId from Documents';

-- Update NotificationSettings with TenantId from AspNetUsers
UPDATE ns
SET ns.TenantId = au.PrimaryTenantId
FROM NotificationSettings ns
INNER JOIN AspNetUsers au ON ns.UserId = au.Id
WHERE ns.TenantId IS NULL AND au.PrimaryTenantId IS NOT NULL;
PRINT 'Updated NotificationSettings TenantId from AspNetUsers';

-- Update NotificationLogs with TenantId from AspNetUsers
UPDATE nl
SET nl.TenantId = au.PrimaryTenantId
FROM NotificationLogs nl
INNER JOIN AspNetUsers au ON nl.UserId = au.Id
WHERE nl.TenantId IS NULL AND au.PrimaryTenantId IS NOT NULL;
PRINT 'Updated NotificationLogs TenantId from AspNetUsers';

-- Update UserSettings with TenantId from AspNetUsers
UPDATE us
SET us.TenantId = au.PrimaryTenantId
FROM UserSettings us
INNER JOIN AspNetUsers au ON us.UserId = au.Id
WHERE us.TenantId IS NULL AND au.PrimaryTenantId IS NOT NULL;
PRINT 'Updated UserSettings TenantId from AspNetUsers';

-- =====================================================
-- Step 3: Verification queries
-- =====================================================

PRINT '';
PRINT '=== Verification Report ===';

SELECT 'Assets' AS TableName, COUNT(*) AS TotalRows, 
       SUM(CASE WHEN TenantId IS NOT NULL THEN 1 ELSE 0 END) AS WithTenant,
       SUM(CASE WHEN TenantId IS NULL THEN 1 ELSE 0 END) AS WithoutTenant
FROM Assets
UNION ALL
SELECT 'WorkOrders', COUNT(*), 
       SUM(CASE WHEN TenantId IS NOT NULL THEN 1 ELSE 0 END),
       SUM(CASE WHEN TenantId IS NULL THEN 1 ELSE 0 END)
FROM WorkOrders
UNION ALL
SELECT 'SpareParts', COUNT(*), 
       SUM(CASE WHEN TenantId IS NOT NULL THEN 1 ELSE 0 END),
       SUM(CASE WHEN TenantId IS NULL THEN 1 ELSE 0 END)
FROM SpareParts
UNION ALL
SELECT 'Documents', COUNT(*), 
       SUM(CASE WHEN TenantId IS NOT NULL THEN 1 ELSE 0 END),
       SUM(CASE WHEN TenantId IS NULL THEN 1 ELSE 0 END)
FROM Documents
UNION ALL
SELECT 'MaintenanceSchedules', COUNT(*), 
       SUM(CASE WHEN TenantId IS NOT NULL THEN 1 ELSE 0 END),
       SUM(CASE WHEN TenantId IS NULL THEN 1 ELSE 0 END)
FROM MaintenanceSchedules;

PRINT '';
PRINT 'Multi-tenancy migration completed successfully!';
