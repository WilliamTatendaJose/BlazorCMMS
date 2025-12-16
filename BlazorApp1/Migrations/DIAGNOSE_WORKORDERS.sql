-- =====================================================
-- DIAGNOSE AND FIX WORKORDERS COLUMN TYPES
-- Check actual column types and fix only what needs fixing
-- =====================================================

USE RBM_CMMS;
GO

PRINT '========================================';
PRINT 'DIAGNOSING WORKORDERS COLUMN TYPES';
PRINT '========================================';
PRINT '';

-- Step 1: Check ALL WorkOrders column types
PRINT 'Step 1: Checking ALL WorkOrders columns...';
PRINT '';

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'WorkOrders'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '========================================';
PRINT 'KEY COLUMNS TO CHECK:';
PRINT '========================================';
PRINT '';

-- Check specific problematic columns
SELECT 
    'Column Name' = COLUMN_NAME,
    'Data Type' = DATA_TYPE,
    'Max Length' = CHARACTER_MAXIMUM_LENGTH,
    'Is Nullable' = IS_NULLABLE,
    'Expected Type' = CASE COLUMN_NAME
        WHEN 'EstimatedDowntime' THEN 'int'
        WHEN 'ActualDowntime' THEN 'float (NOT NULL)'
        WHEN 'FaultTime' THEN 'nvarchar(50)'
        WHEN 'HandOverTime' THEN 'nvarchar(50)'
        WHEN 'Title' THEN 'nvarchar(200)'
        WHEN 'ScheduledDate' THEN 'datetime2 (NULL)'
        WHEN 'TenantId' THEN 'int (NULL)'
        ELSE 'OK'
    END,
    'Status' = CASE 
        WHEN COLUMN_NAME = 'EstimatedDowntime' AND DATA_TYPE != 'int' THEN '? NEEDS FIX'
        WHEN COLUMN_NAME = 'ActualDowntime' AND DATA_TYPE != 'float' THEN '? NEEDS FIX'
        WHEN COLUMN_NAME = 'ActualDowntime' AND IS_NULLABLE = 'YES' THEN '? NEEDS FIX (nullable)'
        WHEN COLUMN_NAME = 'FaultTime' AND DATA_TYPE != 'nvarchar' THEN '? NEEDS FIX'
        WHEN COLUMN_NAME = 'HandOverTime' AND DATA_TYPE != 'nvarchar' THEN '? NEEDS FIX'
        WHEN COLUMN_NAME = 'Title' AND DATA_TYPE != 'nvarchar' THEN '? NEEDS FIX'
        WHEN COLUMN_NAME = 'ScheduledDate' AND DATA_TYPE != 'datetime2' THEN '? NEEDS FIX'
        WHEN COLUMN_NAME = 'TenantId' AND DATA_TYPE != 'int' THEN '? NEEDS FIX'
        ELSE '? OK'
    END
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'WorkOrders'
  AND COLUMN_NAME IN (
      'EstimatedDowntime', 
      'ActualDowntime', 
      'FaultTime', 
      'HandOverTime', 
      'Title', 
      'ScheduledDate', 
      'TenantId',
      'AssetName'
  )
ORDER BY COLUMN_NAME;

PRINT '';
PRINT '========================================';
PRINT 'SAMPLE DATA FROM WORKORDERS:';
PRINT '========================================';
PRINT '';

-- Show sample data to see actual values
IF EXISTS (SELECT * FROM WorkOrders)
BEGIN
    SELECT TOP 3
        WorkOrderId,
        Title,
        EstimatedDowntime,
        ActualDowntime,
        Status
    FROM WorkOrders
    ORDER BY Id DESC;
END
ELSE
BEGIN
    PRINT 'No data in WorkOrders table';
END

PRINT '';
PRINT '========================================';
PRINT 'DIAGNOSIS COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Check the "Status" column above to see which columns need fixing.';
PRINT 'Run the appropriate fix script based on the results.';
GO
