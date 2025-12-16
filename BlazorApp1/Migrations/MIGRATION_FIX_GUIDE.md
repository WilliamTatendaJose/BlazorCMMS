# Migration Conflict Fix Guide

## Problem
Migration `20251209111543_Tenants2` is trying to create tables that already exist:
- AspNetRoles
- AspNetUsers  
- AspNetUserClaims
- AspNetUserLogins
- AspNetUserTokens
- AspNetRoleClaims
- AspNetUserRoles

## Solution: Create Clean Migration

### Step 1: Delete Conflicting Migrations
```powershell
# In Package Manager Console
# Delete these migration files manually:
# - 20251209111543_Tenants2.cs
# - 20251209111543_Tenants2.Designer.cs
# - 20251209145650_tenentsss.cs (if it hasn't been applied)
```

### Step 2: Verify Current Database State
```sql
-- Check what's in __EFMigrationsHistory
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;

-- Check if Tenant tables exist
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('Tenants', 'TenantUserMappings')
ORDER BY TABLE_NAME;
```

### Step 3: Create New Clean Migration
```powershell
# Only add what's missing
Add-Migration AddTenantTables -Context ApplicationDbContext
```

### Step 4: Review Generated Migration
Open the new migration file and verify it ONLY contains:
- ? CREATE TABLE Tenants (if doesn't exist)
- ? CREATE TABLE TenantUserMappings (if doesn't exist)
- ? ALTER TABLE to add TenantId columns (if they don't exist)
- ? NO AspNet* table creations

### Step 5: Apply Migration
```powershell
Update-Database -Context ApplicationDbContext
```

## Alternative: Manual Table Creation

If migrations are too messy, create tables manually:

```sql
-- Create Tenants table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tenants')
BEGIN
    CREATE TABLE [dbo].[Tenants] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [CompanyName] NVARCHAR(200) NOT NULL,
        [Domain] NVARCHAR(100) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        [SubscriptionPlan] NVARCHAR(50) NOT NULL DEFAULT 'Free',
        [MaxUsers] INT NOT NULL DEFAULT 5,
        [StorageQuotaGB] INT NOT NULL DEFAULT 10,
        [ContactEmail] NVARCHAR(100) NULL,
        [ContactPhone] NVARCHAR(20) NULL,
        [BillingAddress] NVARCHAR(500) NULL,
        [LogoUrl] NVARCHAR(500) NULL,
        [ThemeColor] NVARCHAR(20) NULL,
        [TimeZone] NVARCHAR(50) NULL,
        [DefaultLanguage] NVARCHAR(10) NULL DEFAULT 'en'
    );
    PRINT 'Tenants table created';
END

-- Create TenantUserMappings table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TenantUserMappings')
BEGIN
    CREATE TABLE [dbo].[TenantUserMappings] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [TenantId] INT NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [Role] NVARCHAR(50) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [AssignedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        CONSTRAINT [FK_TenantUserMappings_Tenants] 
            FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TenantUserMappings_Users] 
            FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_TenantUserMappings_TenantId] ON [TenantUserMappings]([TenantId]);
    CREATE INDEX [IX_TenantUserMappings_UserId] ON [TenantUserMappings]([UserId]);
    PRINT 'TenantUserMappings table created';
END

-- Add TenantId columns to existing tables
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'TenantId')
BEGIN
    ALTER TABLE Assets ADD TenantId INT NULL;
    PRINT 'TenantId added to Assets';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WorkOrders') AND name = 'TenantId')
BEGIN
    ALTER TABLE WorkOrders ADD TenantId INT NULL;
    PRINT 'TenantId added to WorkOrders';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Documents') AND name = 'TenantId')
BEGIN
    ALTER TABLE Documents ADD TenantId INT NULL;
    PRINT 'TenantId added to Documents';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SpareParts') AND name = 'TenantId')
BEGIN
    ALTER TABLE SpareParts ADD TenantId INT NULL;
    PRINT 'TenantId added to SpareParts';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ConditionReadings') AND name = 'TenantId')
BEGIN
    ALTER TABLE ConditionReadings ADD TenantId INT NULL;
    PRINT 'TenantId added to ConditionReadings';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FailureModes') AND name = 'TenantId')
BEGIN
    ALTER TABLE FailureModes ADD TenantId INT NULL;
    PRINT 'TenantId added to FailureModes';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MaintenanceSchedules') AND name = 'TenantId')
BEGIN
    ALTER TABLE MaintenanceSchedules ADD TenantId INT NULL;
    PRINT 'TenantId added to MaintenanceSchedules';
END

-- Mark migration as applied (replace with actual migration ID)
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = 'YourNewMigrationId')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('YourNewMigrationId', '10.0.0');
END
```

## Prevention: Best Practices

### 1. Always Check Database First
```powershell
# Before adding migration
Script-Migration -From 0 -To Latest -Idempotent
```

### 2. Use Idempotent Scripts
```sql
-- Always use IF NOT EXISTS
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MyTable')
BEGIN
    CREATE TABLE MyTable (...)
END
```

### 3. Backup Before Migration
```powershell
# Backup database
Backup-Database -DatabaseName "RBM_CMMS" -BackupFile "C:\Backups\RBM_CMMS_BeforeMigration.bak"
```

## Quick Recovery Commands

### Reset Migrations (Nuclear Option)
```powershell
# 1. Delete all migration files
# 2. Clear migration history
Invoke-Sqlcmd -Query "DELETE FROM __EFMigrationsHistory" -Database "RBM_CMMS"

# 3. Create initial migration
Add-Migration InitialCreate -Context ApplicationDbContext

# 4. Mark as applied (don't execute)
Invoke-Sqlcmd -Query "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('YourMigrationId', '10.0.0')" -Database "RBM_CMMS"
```

### Check What Will Be Applied
```powershell
# Generate SQL script without applying
Script-Migration -From LastGoodMigration -To LatestMigration
```

## Current Status Check

Run these queries to see what you have:

```sql
-- 1. Check migrations applied
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;

-- 2. Check if Tenant tables exist
SELECT name FROM sys.tables WHERE name LIKE '%Tenant%';

-- 3. Check for TenantId columns
SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType
FROM sys.columns c
JOIN sys.tables t ON c.object_id = t.object_id
JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE c.name = 'TenantId'
ORDER BY t.name;

-- 4. Check AspNet tables
SELECT name FROM sys.tables WHERE name LIKE 'AspNet%' ORDER BY name;
```

## Recommended Action Plan

1. **Backup your database** ?
2. **Run status check queries** ?
3. **Delete problematic migration files** ?
4. **Run manual SQL script** (if needed) ?
5. **Create new clean migration** ?
6. **Apply and test** ?

## Need Help?

If still stuck, provide output from:
```sql
-- What migrations are recorded
SELECT * FROM __EFMigrationsHistory;

-- What tables exist
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
ORDER BY TABLE_NAME;
```
