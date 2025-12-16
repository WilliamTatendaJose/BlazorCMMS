-- =====================================================
-- FIX DATETIME TO STRING CONVERSION
-- Convert FaultTime and HandOverTime from DATETIME2 to NVARCHAR
-- =====================================================

USE RBM_CMMS;
GO

PRINT '========================================';
PRINT 'FIXING DATETIME TO STRING CONVERSIONS';
PRINT '========================================';
PRINT '';

-- Step 1: Check current data types
PRINT 'Step 1: Current column types...';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'WorkOrders' 
  AND COLUMN_NAME IN ('FaultTime', 'HandOverTime')
ORDER BY COLUMN_NAME;

-- Step 2: Add temporary columns with STRING type
PRINT '';
PRINT 'Step 2: Creating temporary string columns...';

ALTER TABLE [dbo].[WorkOrders] ADD [FaultTime_Temp] NVARCHAR(50) NULL;
ALTER TABLE [dbo].[WorkOrders] ADD [HandOverTime_Temp] NVARCHAR(50) NULL;

PRINT '? Temporary columns created';

-- Step 3: Convert DATETIME values to STRING format
PRINT '';
PRINT 'Step 3: Converting datetime values to strings...';

-- Convert FaultTime - format as "HH:mm" or full datetime string
UPDATE WorkOrders 
SET FaultTime_Temp = CASE 
    WHEN FaultTime IS NOT NULL THEN FORMAT(FaultTime, 'HH:mm')
    ELSE ''
END;

-- Convert HandOverTime - format as "HH:mm" or full datetime string  
UPDATE WorkOrders 
SET HandOverTime_Temp = CASE 
    WHEN HandOverTime IS NOT NULL THEN FORMAT(HandOverTime, 'HH:mm')
    ELSE ''
END;

PRINT '? Values converted to string format';

-- Step 4: Drop old DATETIME columns
PRINT '';
PRINT 'Step 4: Dropping old datetime columns...';

ALTER TABLE [dbo].[WorkOrders] DROP COLUMN [FaultTime];
ALTER TABLE [dbo].[WorkOrders] DROP COLUMN [HandOverTime];

PRINT '? Old columns dropped';

-- Step 5: Rename temporary columns to original names
PRINT '';
PRINT 'Step 5: Renaming temporary columns...';

EXEC sp_rename 'WorkOrders.FaultTime_Temp', 'FaultTime', 'COLUMN';
EXEC sp_rename 'WorkOrders.HandOverTime_Temp', 'HandOverTime', 'COLUMN';

PRINT '? Columns renamed';

-- Step 6: Make columns NOT NULL with defaults
PRINT '';
PRINT 'Step 6: Setting NOT NULL constraints...';

UPDATE WorkOrders SET FaultTime = '' WHERE FaultTime IS NULL;
UPDATE WorkOrders SET HandOverTime = '' WHERE HandOverTime IS NULL;

ALTER TABLE [dbo].[WorkOrders] 
ALTER COLUMN [FaultTime] NVARCHAR(50) NOT NULL;

ALTER TABLE [dbo].[WorkOrders] 
ALTER COLUMN [HandOverTime] NVARCHAR(50) NOT NULL;

PRINT '? NOT NULL constraints applied';

-- Step 7: Verify final state
PRINT '';
PRINT 'Step 7: Final verification...';

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'WorkOrders' 
  AND COLUMN_NAME IN ('FaultTime', 'HandOverTime')
ORDER BY COLUMN_NAME;

-- Show sample data
PRINT '';
PRINT 'Sample data after conversion:';
SELECT TOP 5 
    WorkOrderId,
    FaultTime,
    HandOverTime,
    Status
FROM WorkOrders
ORDER BY Id DESC;

PRINT '';
PRINT '========================================';
PRINT 'DATETIME TO STRING CONVERSION COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'FaultTime and HandOverTime are now NVARCHAR(50).';
PRINT 'All datetime values have been converted to time strings (HH:mm format).';
PRINT '';
PRINT '? ALL DATABASE TYPE MISMATCHES RESOLVED!';
PRINT '';
PRINT 'Next: Restart your Blazor application - IT SHOULD NOW WORK!';
GO
