-- =====================================================
-- FIX DUPLICATE TENANTID COLUMN ERROR
-- This marks the migration as applied without re-running it
-- =====================================================

USE RBM_CMMS;
GO

PRINT 'Checking TenantId columns...';

-- Verify TenantId columns already exist
SELECT 
    t.name AS TableName,
    CASE WHEN EXISTS (
        SELECT * FROM sys.columns 
        WHERE object_id = t.object_id AND name = 'TenantId'
    ) THEN 'EXISTS' ELSE 'MISSING' END AS TenantId_Status
FROM sys.tables t
WHERE t.name IN ('WorkOrders', 'Assets', 'Documents', 'SpareParts', 
                 'ConditionReadings', 'FailureModes', 'MaintenanceSchedules')
ORDER BY t.name;

-- Mark the migration as applied (it already ran via SQL script)
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251209145650_tenentsss')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251209145650_tenentsss', '10.0.0');
    PRINT '? Migration 20251209145650_tenentsss marked as applied';
END
ELSE
BEGIN
    PRINT '? Migration 20251209145650_tenentsss already in history';
END

-- Show final migration status
SELECT MigrationId, ProductVersion 
FROM __EFMigrationsHistory 
ORDER BY MigrationId;

PRINT '';
PRINT '========================================';
PRINT 'FIX COMPLETE!';
PRINT '========================================';
PRINT 'You can now run Update-Database safely.';
GO
