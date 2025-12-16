-- =====================================================
-- FIX NULL VALUES IN WORKORDERS
-- Update all NULL values to match the migration expectations
-- =====================================================

USE RBM_CMMS;
GO

PRINT '========================================';
PRINT 'FIXING NULL VALUES IN WORKORDERS';
PRINT '========================================';
PRINT '';

-- Step 1: Fix ActualDowntime NULL values
PRINT 'Step 1: Fixing ActualDowntime NULL values...';
UPDATE WorkOrders 
SET ActualDowntime = 0.0 
WHERE ActualDowntime IS NULL;
PRINT '? ActualDowntime NULL values set to 0.0';

-- Step 2: Fix other NULL values that should have defaults
PRINT '';
PRINT 'Step 2: Fixing other NULL string values...';

UPDATE WorkOrders SET HandOverTime = '' WHERE HandOverTime IS NULL;
PRINT '? HandOverTime NULL values set to empty string';

UPDATE WorkOrders SET FaultTime = '' WHERE FaultTime IS NULL;
PRINT '? FaultTime NULL values set to empty string';

UPDATE WorkOrders SET RecurrenceInterval = 0 WHERE RecurrenceInterval IS NULL;
PRINT '? RecurrenceInterval NULL values set to 0';

-- Step 3: Verify NULL value counts
PRINT '';
PRINT 'Step 3: Verification - checking for remaining NULLs...';

SELECT 
    'ActualDowntime' AS ColumnName,
    COUNT(*) AS NullCount
FROM WorkOrders 
WHERE ActualDowntime IS NULL
UNION ALL
SELECT 
    'HandOverTime',
    COUNT(*)
FROM WorkOrders 
WHERE HandOverTime IS NULL
UNION ALL
SELECT 
    'FaultTime',
    COUNT(*)
FROM WorkOrders 
WHERE FaultTime IS NULL
UNION ALL
SELECT 
    'RecurrenceInterval',
    COUNT(*)
FROM WorkOrders 
WHERE RecurrenceInterval IS NULL;

-- Step 4: Show sample data
PRINT '';
PRINT 'Step 4: Sample WorkOrders data...';
SELECT TOP 5 
    WorkOrderId,
    ActualDowntime,
    HandOverTime,
    FaultTime,
    RecurrenceInterval,
    Status
FROM WorkOrders
ORDER BY Id DESC;

PRINT '';
PRINT '========================================';
PRINT 'NULL VALUES FIX COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'All NULL values have been replaced with defaults.';
PRINT 'Your application should now work!';
PRINT '';
PRINT 'Next: Restart your Blazor application.';
GO
