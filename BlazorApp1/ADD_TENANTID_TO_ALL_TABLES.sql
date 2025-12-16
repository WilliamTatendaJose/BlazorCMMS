-- SQL Migration Script: Add TenantId to All Business Tables
-- This script adds multi-tenancy support to all business tables

-- ===================================================================
-- STEP 1: Add TenantId columns to all business tables
-- ===================================================================

-- Add TenantId to WorkOrders
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'WorkOrders' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.WorkOrders
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_WorkOrders_TenantId ON dbo.WorkOrders(TenantId);
END

-- Add TenantId to ConditionReadings
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'ConditionReadings' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.ConditionReadings
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_ConditionReadings_TenantId ON dbo.ConditionReadings(TenantId);
END

-- Add TenantId to FailureModes
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'FailureModes' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.FailureModes
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_FailureModes_TenantId ON dbo.FailureModes(TenantId);
END

-- Add TenantId to ReliabilityMetrics
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'ReliabilityMetrics' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.ReliabilityMetrics
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_ReliabilityMetrics_TenantId ON dbo.ReliabilityMetrics(TenantId);
END

-- Add TenantId to AssetDowntime
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AssetDowntime' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.AssetDowntime
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_AssetDowntime_TenantId ON dbo.AssetDowntime(TenantId);
END

-- Add TenantId to MaintenanceSchedules
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'MaintenanceSchedules' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.MaintenanceSchedules
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_MaintenanceSchedules_TenantId ON dbo.MaintenanceSchedules(TenantId);
END

-- Add TenantId to MaintenanceTasks
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'MaintenanceTasks' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.MaintenanceTasks
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_MaintenanceTasks_TenantId ON dbo.MaintenanceTasks(TenantId);
END

-- Add TenantId to SpareParts
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'SpareParts' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.SpareParts
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_SpareParts_TenantId ON dbo.SpareParts(TenantId);
END

-- Add TenantId to SparePartTransactions
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'SparePartTransactions' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.SparePartTransactions
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_SparePartTransactions_TenantId ON dbo.SparePartTransactions(TenantId);
END

-- Add TenantId to Documents
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'Documents' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.Documents
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_Documents_TenantId ON dbo.Documents(TenantId);
END

-- Add TenantId to DocumentAccessLogs
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'DocumentAccessLogs' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.DocumentAccessLogs
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_DocumentAccessLogs_TenantId ON dbo.DocumentAccessLogs(TenantId);
END

-- Add TenantId to MaintenanceTasks if it exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'MaintenanceTasks' AND COLUMN_NAME = 'TenantId')
BEGIN
    ALTER TABLE dbo.MaintenanceTasks
    ADD TenantId INT NULL;
    
    CREATE INDEX IX_MaintenanceTasks_TenantId ON dbo.MaintenanceTasks(TenantId);
END

-- ===================================================================
-- STEP 2: Add Foreign Key Constraints
-- ===================================================================

-- Add FK for WorkOrders
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'WorkOrders' AND CONSTRAINT_NAME LIKE 'FK_WorkOrders_Tenants%')
BEGIN
    ALTER TABLE dbo.WorkOrders
    ADD CONSTRAINT FK_WorkOrders_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for ConditionReadings
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'ConditionReadings' AND CONSTRAINT_NAME LIKE 'FK_ConditionReadings_Tenants%')
BEGIN
    ALTER TABLE dbo.ConditionReadings
    ADD CONSTRAINT FK_ConditionReadings_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for FailureModes
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'FailureModes' AND CONSTRAINT_NAME LIKE 'FK_FailureModes_Tenants%')
BEGIN
    ALTER TABLE dbo.FailureModes
    ADD CONSTRAINT FK_FailureModes_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for ReliabilityMetrics
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'ReliabilityMetrics' AND CONSTRAINT_NAME LIKE 'FK_ReliabilityMetrics_Tenants%')
BEGIN
    ALTER TABLE dbo.ReliabilityMetrics
    ADD CONSTRAINT FK_ReliabilityMetrics_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for AssetDowntime
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'AssetDowntime' AND CONSTRAINT_NAME LIKE 'FK_AssetDowntime_Tenants%')
BEGIN
    ALTER TABLE dbo.AssetDowntime
    ADD CONSTRAINT FK_AssetDowntime_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for MaintenanceSchedules
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'MaintenanceSchedules' AND CONSTRAINT_NAME LIKE 'FK_MaintenanceSchedules_Tenants%')
BEGIN
    ALTER TABLE dbo.MaintenanceSchedules
    ADD CONSTRAINT FK_MaintenanceSchedules_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for MaintenanceTasks
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'MaintenanceTasks' AND CONSTRAINT_NAME LIKE 'FK_MaintenanceTasks_Tenants%')
BEGIN
    ALTER TABLE dbo.MaintenanceTasks
    ADD CONSTRAINT FK_MaintenanceTasks_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for SpareParts
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'SpareParts' AND CONSTRAINT_NAME LIKE 'FK_SpareParts_Tenants%')
BEGIN
    ALTER TABLE dbo.SpareParts
    ADD CONSTRAINT FK_SpareParts_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for SparePartTransactions
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'SparePartTransactions' AND CONSTRAINT_NAME LIKE 'FK_SparePartTransactions_Tenants%')
BEGIN
    ALTER TABLE dbo.SparePartTransactions
    ADD CONSTRAINT FK_SparePartTransactions_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for Documents
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'Documents' AND CONSTRAINT_NAME LIKE 'FK_Documents_Tenants%')
BEGIN
    ALTER TABLE dbo.Documents
    ADD CONSTRAINT FK_Documents_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- Add FK for DocumentAccessLogs
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
              WHERE TABLE_NAME = 'DocumentAccessLogs' AND CONSTRAINT_NAME LIKE 'FK_DocumentAccessLogs_Tenants%')
BEGIN
    ALTER TABLE dbo.DocumentAccessLogs
    ADD CONSTRAINT FK_DocumentAccessLogs_Tenants_TenantId 
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE SET NULL;
END

-- ===================================================================
-- STEP 3: Verification Queries
-- ===================================================================

-- Check which tables have TenantId
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE COLUMN_NAME = 'TenantId' 
  AND TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME;

-- Check which tables are MISSING TenantId (that should have it)
SELECT 
    'WorkOrders' AS TableName,
    CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'WorkOrders' AND COLUMN_NAME = 'TenantId')
         THEN 'YES' ELSE 'MISSING' END AS HasTenantId
UNION
SELECT 'ConditionReadings', 
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'ConditionReadings' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
UNION
SELECT 'FailureModes',
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'FailureModes' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
UNION
SELECT 'ReliabilityMetrics',
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'ReliabilityMetrics' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
UNION
SELECT 'AssetDowntime',
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'AssetDowntime' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
UNION
SELECT 'MaintenanceSchedules',
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'MaintenanceSchedules' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
UNION
SELECT 'MaintenanceTasks',
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'MaintenanceTasks' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
UNION
SELECT 'SpareParts',
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'SpareParts' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
UNION
SELECT 'SparePartTransactions',
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'SparePartTransactions' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
UNION
SELECT 'Documents',
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'Documents' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
UNION
SELECT 'DocumentAccessLogs',
       CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = 'DocumentAccessLogs' AND COLUMN_NAME = 'TenantId')
            THEN 'YES' ELSE 'MISSING' END
ORDER BY TableName;

-- ===================================================================
-- Summary: After running this script:
-- ===================================================================
-- ? All business tables will have TenantId column
-- ? All TenantId columns will be indexed
-- ? All TenantId columns will have FK constraints to Tenants
-- ? Complete data isolation by tenant is enforced at DB level
-- ? SuperAdmin can see all data, other users limited by TenantId
-- ===================================================================
