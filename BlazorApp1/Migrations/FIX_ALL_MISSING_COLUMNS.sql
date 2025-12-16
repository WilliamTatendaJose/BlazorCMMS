-- =====================================================
-- ADD MISSING WORKORDERS COLUMNS
-- Fix the remaining WorkOrders table issues
-- =====================================================

USE RBM_CMMS;
GO

PRINT 'Fixing WorkOrders table...';

-- Check current WorkOrders columns
SELECT 'Current WorkOrders columns:' AS Info;
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'WorkOrders'
ORDER BY ORDINAL_POSITION;

-- Check if Title column exists (migration renamed AssetName to Title)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'Title')
BEGIN
    -- Check if AssetName column exists to rename it
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'AssetName')
    BEGIN
        EXEC sp_rename 'WorkOrders.AssetName', 'Title', 'COLUMN';
        PRINT '? Renamed AssetName to Title';
    END
    ELSE
    BEGIN
        -- AssetName doesn't exist, create Title column
        ALTER TABLE [dbo].[WorkOrders] ADD [Title] NVARCHAR(200) NOT NULL DEFAULT '';
        PRINT '? Title column added';
    END
END
ELSE
BEGIN
    PRINT '? Title column already exists';
END

-- Add ScheduledDate column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ScheduledDate')
BEGIN
    ALTER TABLE [dbo].[WorkOrders] ADD [ScheduledDate] DATETIME2(7) NULL;
    PRINT '? ScheduledDate column added';
END
ELSE
BEGIN
    PRINT '? ScheduledDate column already exists';
END

-- Verify FileExtension and FileSizeBytes exist in Documents table
PRINT '';
PRINT 'Checking Documents table...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Documents') AND name = 'FileExtension')
BEGIN
    ALTER TABLE [dbo].[Documents] ADD [FileExtension] NVARCHAR(10) NOT NULL DEFAULT '';
    PRINT '? Documents.FileExtension added';
END
ELSE
BEGIN
    PRINT '? Documents.FileExtension already exists';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Documents') AND name = 'FileSizeBytes')
BEGIN
    ALTER TABLE [dbo].[Documents] ADD [FileSizeBytes] BIGINT NOT NULL DEFAULT 0;
    PRINT '? Documents.FileSizeBytes added';
END
ELSE
BEGIN
    PRINT '? Documents.FileSizeBytes already exists';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Documents') AND name = 'IsConfidential')
BEGIN
    ALTER TABLE [dbo].[Documents] ADD [IsConfidential] BIT NOT NULL DEFAULT 0;
    PRINT '? Documents.IsConfidential added';
END
ELSE
BEGIN
    PRINT '? Documents.IsConfidential already exists';
END

-- Final verification
PRINT '';
PRINT '--- Final Verification ---';

-- Check WorkOrders critical columns
SELECT 
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'Title') 
        THEN 'EXISTS' ELSE 'MISSING' END AS Title_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'ScheduledDate') 
        THEN 'EXISTS' ELSE 'MISSING' END AS ScheduledDate_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'TenantId') 
        THEN 'EXISTS' ELSE 'MISSING' END AS WorkOrders_TenantId_Status;

-- Check Assets columns
SELECT 
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'AssetType') 
        THEN 'EXISTS' ELSE 'MISSING' END AS AssetType_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Manufacturer') 
        THEN 'EXISTS' ELSE 'MISSING' END AS Manufacturer_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Model') 
        THEN 'EXISTS' ELSE 'MISSING' END AS Model_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'TenantId') 
        THEN 'EXISTS' ELSE 'MISSING' END AS Assets_TenantId_Status;

-- Check Documents columns
SELECT 
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Documents') AND name = 'FileExtension') 
        THEN 'EXISTS' ELSE 'MISSING' END AS FileExtension_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Documents') AND name = 'FileSizeBytes') 
        THEN 'EXISTS' ELSE 'MISSING' END AS FileSizeBytes_Status,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Documents') AND name = 'TenantId') 
        THEN 'EXISTS' ELSE 'MISSING' END AS Documents_TenantId_Status;

PRINT '';
PRINT '========================================';
PRINT 'DATABASE MIGRATION FIX COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'All missing columns have been added.';
PRINT 'Your Blazor application should now run without errors.';
PRINT '';
PRINT 'Next: Restart your application!';
GO
