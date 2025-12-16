-- =====================================================
-- ADD MISSING ASSET COLUMNS
-- These columns are in the model but missing from database
-- =====================================================

USE RBM_CMMS;
GO

PRINT 'Adding missing columns to Assets table...';

-- Check which columns are missing
SELECT 'Current Assets columns:' AS Info;
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Assets'
ORDER BY ORDINAL_POSITION;

-- Add AssetType column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'AssetType')
BEGIN
    ALTER TABLE [dbo].[Assets] ADD [AssetType] NVARCHAR(100) NOT NULL DEFAULT '';
    PRINT '✓ AssetType column added';
END
ELSE
BEGIN
    PRINT '✓ AssetType already exists';
END

-- Add Manufacturer column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Manufacturer')
BEGIN
    ALTER TABLE [dbo].[Assets] ADD [Manufacturer] NVARCHAR(100) NOT NULL DEFAULT '';
    PRINT '✓ Manufacturer column added';
END
ELSE
BEGIN
    PRINT '✓ Manufacturer already exists';
END

-- Add Model column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Model')
BEGIN
    ALTER TABLE [dbo].[Assets] ADD [Model] NVARCHAR(100) NOT NULL DEFAULT '';
    PRINT '✓ Model column added';
END
ELSE
BEGIN
    PRINT '✓ Model already exists';
END

-- Add LastMaintenanceDate column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'LastMaintenanceDate')
BEGIN
    ALTER TABLE [dbo].[Assets] ADD [LastMaintenanceDate] DATETIME2(7) NULL;
    PRINT '✓ LastMaintenanceDate column added';
END
ELSE
BEGIN
    PRINT '✓ LastMaintenanceDate already exists';
END

-- Add NextMaintenanceDate column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'NextMaintenanceDate')
BEGIN
    ALTER TABLE [dbo].[Assets] ADD [NextMaintenanceDate] DATETIME2(7) NULL;
    PRINT '✓ NextMaintenanceDate column added';
END
ELSE
BEGIN
    PRINT '✓ NextMaintenanceDate already exists';
END

-- Verify all columns now exist
PRINT '';
PRINT 'Final verification...';
SELECT 
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'AssetType') 
        THEN 'EXISTS' ELSE 'MISSING' END AS AssetType_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Manufacturer') 
        THEN 'EXISTS' ELSE 'MISSING' END AS Manufacturer_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Model') 
        THEN 'EXISTS' ELSE 'MISSING' END AS Model_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'LastMaintenanceDate') 
        THEN 'EXISTS' ELSE 'MISSING' END AS LastMaintenanceDate_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'NextMaintenanceDate') 
        THEN 'EXISTS' ELSE 'MISSING' END AS NextMaintenanceDate_Status;

PRINT '';
PRINT '========================================';
PRINT 'ASSET COLUMNS FIX COMPLETE!';
PRINT '========================================';
PRINT 'Your application should now run without errors.';
GO
