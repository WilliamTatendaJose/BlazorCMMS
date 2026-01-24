-- Add missing columns to Assets table
-- Run this script against your database to fix the "Invalid column name" errors

-- Add AssetType column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'AssetType')
BEGIN
    ALTER TABLE Assets ADD AssetType NVARCHAR(100) NOT NULL DEFAULT '';
    PRINT 'Added AssetType column';
END
ELSE
BEGIN
    PRINT 'AssetType column already exists';
END

-- Add Manufacturer column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Manufacturer')
BEGIN
    ALTER TABLE Assets ADD Manufacturer NVARCHAR(100) NOT NULL DEFAULT '';
    PRINT 'Added Manufacturer column';
END
ELSE
BEGIN
    PRINT 'Manufacturer column already exists';
END

-- Add Model column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Model')
BEGIN
    ALTER TABLE Assets ADD Model NVARCHAR(100) NOT NULL DEFAULT '';
    PRINT 'Added Model column';
END
ELSE
BEGIN
    PRINT 'Model column already exists';
END

-- Add LastMaintenanceDate column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'LastMaintenanceDate')
BEGIN
    ALTER TABLE Assets ADD LastMaintenanceDate DATETIME2 NULL;
    PRINT 'Added LastMaintenanceDate column';
END
ELSE
BEGIN
    PRINT 'LastMaintenanceDate column already exists';
END

-- Add NextMaintenanceDate column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'NextMaintenanceDate')
BEGIN
    ALTER TABLE Assets ADD NextMaintenanceDate DATETIME2 NULL;
    PRINT 'Added NextMaintenanceDate column';
END
ELSE
BEGIN
    PRINT 'NextMaintenanceDate column already exists';
END

-- Add TenantId column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'TenantId')
BEGIN
    ALTER TABLE Assets ADD TenantId INT NULL;
    PRINT 'Added TenantId column';
END
ELSE
BEGIN
    PRINT 'TenantId column already exists';
END

PRINT 'All missing columns have been added to the Assets table.';
