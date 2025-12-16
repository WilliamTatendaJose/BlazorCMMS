-- =====================================================
-- MIGRATION CONFLICT FIX - IMMEDIATE SOLUTION
-- Run this in SQL Server Management Studio or Azure Data Studio
-- =====================================================

USE RBM_CMMS;
GO

PRINT '========================================';
PRINT 'Starting Migration Conflict Fix';
PRINT 'Date: ' + CAST(GETDATE() AS VARCHAR);
PRINT '========================================';

-- Step 1: Check current state
PRINT '';
PRINT '--- Step 1: Checking Current State ---';

-- Check what migrations are applied
SELECT 'Applied Migrations:' AS Info;
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;

-- Check if Tenant tables exist
SELECT 'Tenant Tables Status:' AS Info;
SELECT 
    CASE WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'Tenants') 
        THEN 'EXISTS' ELSE 'MISSING' END AS Tenants_Table,
    CASE WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'TenantUserMappings') 
        THEN 'EXISTS' ELSE 'MISSING' END AS TenantUserMappings_Table;

-- Step 2: Create Tenant tables if they don't exist
PRINT '';
PRINT '--- Step 2: Creating Tenant Tables (if missing) ---';

-- Create Tenants table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tenants')
BEGIN
    CREATE TABLE [dbo].[Tenants] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [CompanyName] NVARCHAR(200) NOT NULL,
        [Domain] NVARCHAR(100) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME2(7) NOT NULL,
        [ModifiedDate] DATETIME2(7) NULL,
        [SubscriptionPlan] NVARCHAR(50) NOT NULL,
        [MaxUsers] INT NOT NULL,
        [StorageQuotaGB] INT NOT NULL,
        [ContactEmail] NVARCHAR(100) NULL,
        [ContactPhone] NVARCHAR(20) NULL,
        [BillingAddress] NVARCHAR(500) NULL,
        [LogoUrl] NVARCHAR(500) NULL,
        [ThemeColor] NVARCHAR(20) NULL,
        [TimeZone] NVARCHAR(50) NULL,
        [DefaultLanguage] NVARCHAR(10) NULL,
        CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT '✓ Tenants table created';
END
ELSE
BEGIN
    PRINT '✓ Tenants table already exists';
END

-- Create TenantUserMappings table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TenantUserMappings')
BEGIN
    CREATE TABLE [dbo].[TenantUserMappings] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [TenantId] INT NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [Role] NVARCHAR(50) NOT NULL,
        [IsActive] BIT NOT NULL,
        [AssignedDate] DATETIME2(7) NOT NULL,
        [ModifiedDate] DATETIME2(7) NULL,
        CONSTRAINT [PK_TenantUserMappings] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_TenantUserMappings_Tenants_TenantId] 
            FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TenantUserMappings_AspNetUsers_UserId] 
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_TenantUserMappings_TenantId] 
        ON [dbo].[TenantUserMappings] ([TenantId] ASC);
    
    CREATE NONCLUSTERED INDEX [IX_TenantUserMappings_UserId] 
        ON [dbo].[TenantUserMappings] ([UserId] ASC);
    
    PRINT '✓ TenantUserMappings table created';
END
ELSE
BEGIN
    PRINT '✓ TenantUserMappings table already exists';
END

-- Step 3: Add TenantId columns to existing tables
PRINT '';
PRINT '--- Step 3: Adding TenantId Columns ---';

-- Assets
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'TenantId')
BEGIN
    ALTER TABLE [dbo].[Assets] ADD [TenantId] INT NULL;
    PRINT '✓ TenantId added to Assets';
END
ELSE PRINT '✓ Assets.TenantId already exists';

-- WorkOrders
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'TenantId')
BEGIN
    ALTER TABLE [dbo].[WorkOrders] ADD [TenantId] INT NULL;
    PRINT '✓ TenantId added to WorkOrders';
END
ELSE PRINT '✓ WorkOrders.TenantId already exists';

-- Documents
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Documents') AND name = 'TenantId')
BEGIN
    ALTER TABLE [dbo].[Documents] ADD [TenantId] INT NULL;
    PRINT '✓ TenantId added to Documents';
END
ELSE PRINT '✓ Documents.TenantId already exists';

-- SpareParts
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SpareParts') AND name = 'TenantId')
BEGIN
    ALTER TABLE [dbo].[SpareParts] ADD [TenantId] INT NULL;
    PRINT '✓ TenantId added to SpareParts';
END
ELSE PRINT '✓ SpareParts.TenantId already exists';

-- ConditionReadings
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ConditionReadings') AND name = 'TenantId')
BEGIN
    ALTER TABLE [dbo].[ConditionReadings] ADD [TenantId] INT NULL;
    PRINT '✓ TenantId added to ConditionReadings';
END
ELSE PRINT '✓ ConditionReadings.TenantId already exists';

-- FailureModes
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FailureModes') AND name = 'TenantId')
BEGIN
    ALTER TABLE [dbo].[FailureModes] ADD [TenantId] INT NULL;
    PRINT '✓ TenantId added to FailureModes';
END
ELSE PRINT '✓ FailureModes.TenantId already exists';

-- MaintenanceSchedules
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MaintenanceSchedules') AND name = 'TenantId')
BEGIN
    ALTER TABLE [dbo].[MaintenanceSchedules] ADD [TenantId] INT NULL;
    PRINT '✓ TenantId added to MaintenanceSchedules';
END
ELSE PRINT '✓ MaintenanceSchedules.TenantId already exists';

-- Step 4: Mark migration as applied
PRINT '';
PRINT '--- Step 4: Marking Migration as Applied ---';

-- Mark Tenants2 migration as applied
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251209111543_Tenants2')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251209111543_Tenants2', '10.0.0');
    PRINT '✓ 20251209111543_Tenants2 marked as applied';
END
ELSE
BEGIN
    PRINT '✓ 20251209111543_Tenants2 already in migration history';
END

-- Step 5: Verify final state
PRINT '';
PRINT '--- Step 5: Final Verification ---';

-- Show all tables with TenantId
SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.is_nullable AS IsNullable
FROM sys.columns c
JOIN sys.tables t ON c.object_id = t.object_id
JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE c.name = 'TenantId'
ORDER BY t.name;

-- Show migration history
SELECT 'Final Migration History:' AS Info;
SELECT MigrationId, ProductVersion 
FROM __EFMigrationsHistory 
ORDER BY MigrationId;

PRINT '';
PRINT '========================================';
PRINT 'Migration Conflict Fix COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Run Update-Database in Package Manager Console';
PRINT '2. If errors persist, delete migration files:';
PRINT '   - 20251209111543_Tenants2.cs';
PRINT '   - 20251209111543_Tenants2.Designer.cs';
PRINT '3. Create new migration if needed:';
PRINT '   Add-Migration AddRemainingTenantFeatures';
PRINT '';

GO
