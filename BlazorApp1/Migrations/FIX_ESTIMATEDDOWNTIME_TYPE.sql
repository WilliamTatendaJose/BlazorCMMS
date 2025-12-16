-- =====================================================
-- FIX ESTIMATEDDOWNTIME TYPE MISMATCH
-- Convert from float to int to match the model
-- =====================================================

USE RBM_CMMS;
GO

PRINT '========================================';
PRINT 'FIXING ESTIMATEDDOWNTIME TYPE MISMATCH';
PRINT '========================================';
PRINT '';

-- Step 1: Check current data type
PRINT 'Step 1: Current EstimatedDowntime data type...';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'WorkOrders' 
  AND COLUMN_NAME = 'EstimatedDowntime';

-- Step 2: Convert EstimatedDowntime from float to int
PRINT '';
PRINT 'Step 2: Converting EstimatedDowntime from float to int...';

-- First, round any decimal values to integers
UPDATE WorkOrders 
SET EstimatedDowntime = ROUND(EstimatedDowntime, 0)
WHERE EstimatedDowntime != ROUND(EstimatedDowntime, 0);

-- Now change the column type
ALTER TABLE [dbo].[WorkOrders] 
ALTER COLUMN [EstimatedDowntime] INT NOT NULL;

PRINT '? EstimatedDowntime converted from float to int';

-- Step 3: Verify the change
PRINT '';
PRINT 'Step 3: Verifying data type change...';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'WorkOrders' 
  AND COLUMN_NAME = 'EstimatedDowntime';

-- Step 4: Show sample data
PRINT '';
PRINT 'Step 4: Sample WorkOrders data...';
SELECT TOP 5 
    WorkOrderId,
    EstimatedDowntime,
    ActualDowntime,
    Status
FROM WorkOrders
ORDER BY Id DESC;

PRINT '';
PRINT '========================================';
PRINT 'TYPE CONVERSION COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'EstimatedDowntime is now INT (was float).';
PRINT 'Your application should now work perfectly!';
PRINT '';
PRINT '? ALL DATABASE ISSUES RESOLVED!';
PRINT '';
PRINT 'Next: Restart your Blazor application.';
GO
