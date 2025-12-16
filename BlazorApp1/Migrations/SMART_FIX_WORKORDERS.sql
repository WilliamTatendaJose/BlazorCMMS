-- =====================================================
-- SMART FIX FOR WORKORDERS - ONLY FIX WHAT'S BROKEN
-- This script checks each column and only fixes mismatches
-- =====================================================

USE RBM_CMMS;
GO

PRINT '========================================';
PRINT 'SMART FIX FOR WORKORDERS COLUMNS';
PRINT '========================================';
PRINT '';

-- Declare variables to track what needs fixing
DECLARE @NeedsFix_EstimatedDowntime BIT = 0;
DECLARE @NeedsFix_ActualDowntime BIT = 0;
DECLARE @NeedsFix_FaultTime BIT = 0;
DECLARE @NeedsFix_HandOverTime BIT = 0;

-- Check EstimatedDowntime
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'WorkOrders' 
      AND COLUMN_NAME = 'EstimatedDowntime' 
      AND DATA_TYPE != 'int'
)
BEGIN
    SET @NeedsFix_EstimatedDowntime = 1;
    PRINT '? EstimatedDowntime needs fixing (current type: float, expected: int)';
END
ELSE
BEGIN
    PRINT '? EstimatedDowntime is correct (int)';
END

-- Check ActualDowntime
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'WorkOrders' 
      AND COLUMN_NAME = 'ActualDowntime' 
      AND (DATA_TYPE != 'float' OR IS_NULLABLE = 'YES')
)
BEGIN
    SET @NeedsFix_ActualDowntime = 1;
    PRINT '? ActualDowntime needs fixing';
END
ELSE
BEGIN
    PRINT '? ActualDowntime is correct (float NOT NULL)';
END

-- Check FaultTime
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'WorkOrders' 
      AND COLUMN_NAME = 'FaultTime' 
      AND DATA_TYPE NOT IN ('nvarchar', 'varchar')
)
BEGIN
    SET @NeedsFix_FaultTime = 1;
    PRINT '? FaultTime needs converting from datetime to string';
END
ELSE
BEGIN
    PRINT '? FaultTime is correct (nvarchar)';
END

-- Check HandOverTime
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'WorkOrders' 
      AND COLUMN_NAME = 'HandOverTime' 
      AND DATA_TYPE NOT IN ('nvarchar', 'varchar')
)
BEGIN
    SET @NeedsFix_HandOverTime = 1;
    PRINT '? HandOverTime needs converting from datetime to string';
END
ELSE
BEGIN
    PRINT '? HandOverTime is correct (nvarchar)';
END

PRINT '';
PRINT '--- Applying Fixes ---';
PRINT '';

-- Fix EstimatedDowntime (float ? int)
IF @NeedsFix_EstimatedDowntime = 1
BEGIN
    PRINT 'Fixing EstimatedDowntime...';
    
    -- Round any decimal values
    UPDATE WorkOrders 
    SET EstimatedDowntime = ROUND(EstimatedDowntime, 0)
    WHERE EstimatedDowntime != ROUND(EstimatedDowntime, 0);
    
    -- Convert column type
    ALTER TABLE [dbo].[WorkOrders] 
    ALTER COLUMN [EstimatedDowntime] INT NOT NULL;
    
    PRINT '? EstimatedDowntime fixed: float ? int';
END

-- Fix ActualDowntime (nullable ? NOT NULL)
IF @NeedsFix_ActualDowntime = 1
BEGIN
    PRINT 'Fixing ActualDowntime...';
    
    -- Set NULLs to 0
    UPDATE WorkOrders 
    SET ActualDowntime = 0.0 
    WHERE ActualDowntime IS NULL;
    
    -- Make NOT NULL if needed
    IF EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'WorkOrders' 
          AND COLUMN_NAME = 'ActualDowntime' 
          AND IS_NULLABLE = 'YES'
    )
    BEGIN
        ALTER TABLE [dbo].[WorkOrders] 
        ALTER COLUMN [ActualDowntime] FLOAT NOT NULL;
    END
    
    PRINT '? ActualDowntime fixed: now NOT NULL with default 0';
END

-- Fix FaultTime (datetime ? nvarchar)
IF @NeedsFix_FaultTime = 1
BEGIN
    PRINT 'Fixing FaultTime...';
    
    -- Add temporary column
    ALTER TABLE [dbo].[WorkOrders] ADD [FaultTime_New] NVARCHAR(50) NULL;
    
    -- Convert datetime to string
    UPDATE WorkOrders 
    SET FaultTime_New = CASE 
        WHEN FaultTime IS NOT NULL THEN FORMAT(CAST(FaultTime AS DATETIME), 'HH:mm')
        ELSE ''
    END;
    
    -- Drop old column
    ALTER TABLE [dbo].[WorkOrders] DROP COLUMN [FaultTime];
    
    -- Rename new column
    EXEC sp_rename 'WorkOrders.FaultTime_New', 'FaultTime', 'COLUMN';
    
    -- Make NOT NULL
    UPDATE WorkOrders SET FaultTime = '' WHERE FaultTime IS NULL;
    ALTER TABLE [dbo].[WorkOrders] ALTER COLUMN [FaultTime] NVARCHAR(50) NOT NULL;
    
    PRINT '? FaultTime fixed: datetime ? nvarchar(50)';
END

-- Fix HandOverTime (datetime ? nvarchar)
IF @NeedsFix_HandOverTime = 1
BEGIN
    PRINT 'Fixing HandOverTime...';
    
    -- Add temporary column
    ALTER TABLE [dbo].[WorkOrders] ADD [HandOverTime_New] NVARCHAR(50) NULL;
    
    -- Convert datetime to string
    UPDATE WorkOrders 
    SET HandOverTime_New = CASE 
        WHEN HandOverTime IS NOT NULL THEN FORMAT(CAST(HandOverTime AS DATETIME), 'HH:mm')
        ELSE ''
    END;
    
    -- Drop old column
    ALTER TABLE [dbo].[WorkOrders] DROP COLUMN [HandOverTime];
    
    -- Rename new column
    EXEC sp_rename 'WorkOrders.HandOverTime_New', 'HandOverTime', 'COLUMN';
    
    -- Make NOT NULL
    UPDATE WorkOrders SET HandOverTime = '' WHERE HandOverTime IS NULL;
    ALTER TABLE [dbo].[WorkOrders] ALTER COLUMN [HandOverTime] NVARCHAR(50) NOT NULL;
    
    PRINT '? HandOverTime fixed: datetime ? nvarchar(50)';
END

-- Final verification
PRINT '';
PRINT '--- Final Verification ---';
PRINT '';

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'WorkOrders'
  AND COLUMN_NAME IN ('EstimatedDowntime', 'ActualDowntime', 'FaultTime', 'HandOverTime')
ORDER BY COLUMN_NAME;

PRINT '';
PRINT '========================================';
PRINT 'SMART FIX COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Only the columns that needed fixing were updated.';
PRINT 'Restart your Blazor application now.';
GO
