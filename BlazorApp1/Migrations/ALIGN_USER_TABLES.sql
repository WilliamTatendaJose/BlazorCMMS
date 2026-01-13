-- =============================================
-- USER TABLES ALIGNMENT SCRIPT
-- Links legacy Users table with ASP.NET Identity
-- =============================================

-- Step 1: Add AspNetUserId column to Users table if not exists
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'AspNetUserId')
BEGIN
    ALTER TABLE Users ADD AspNetUserId NVARCHAR(450) NULL;
    PRINT 'Added AspNetUserId column to Users table';
END

-- Step 2: Create indexes for faster lookups
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_AspNetUserId')
BEGIN
    CREATE INDEX IX_Users_AspNetUserId ON Users(AspNetUserId);
    PRINT 'Created index IX_Users_AspNetUserId';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_Email')
BEGIN
    CREATE INDEX IX_Users_Email ON Users(Email);
    PRINT 'Created index IX_Users_Email';
END

-- Step 3: Sync existing users by matching on Email
UPDATE u
SET u.AspNetUserId = au.Id
FROM Users u
INNER JOIN AspNetUsers au ON u.Email = au.Email
WHERE u.AspNetUserId IS NULL;

PRINT 'Synced AspNetUserId for existing users';

-- Step 4: Insert missing users from AspNetUsers into Users table
INSERT INTO Users (Name, Email, Role, Department, Phone, IsActive, CreatedDate, TenantId, AspNetUserId)
SELECT 
    COALESCE(au.FullName, au.Email, 'Unknown') as Name,
    au.Email,
    COALESCE(
        (SELECT TOP 1 r.Name 
         FROM AspNetUserRoles ur 
         INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
         WHERE ur.UserId = au.Id), 
        'Technician'
    ) as Role,
    COALESCE(au.Department, '') as Department,
    COALESCE(au.PhoneNumber, '') as Phone,
    au.IsActive,
    au.CreatedDate,
    au.PrimaryTenantId,
    au.Id as AspNetUserId
FROM AspNetUsers au
WHERE NOT EXISTS (SELECT 1 FROM Users u WHERE u.Email = au.Email OR u.AspNetUserId = au.Id);

PRINT 'Inserted missing users from AspNetUsers';

-- Step 5: Verify the sync
SELECT 
    'Identity Users' as TableType,
    COUNT(*) as Count
FROM AspNetUsers
UNION ALL
SELECT 
    'Legacy Users' as TableType,
    COUNT(*) as Count
FROM Users
UNION ALL
SELECT 
    'Synced Users (with AspNetUserId)' as TableType,
    COUNT(*) as Count
FROM Users WHERE AspNetUserId IS NOT NULL;

-- Step 6: Show users without links (should be 0)
SELECT 
    'Unlinked Legacy Users' as Issue,
    u.Id, u.Name, u.Email
FROM Users u
WHERE u.AspNetUserId IS NULL;

SELECT 
    'Identity Users without Legacy record' as Issue,
    au.Id, au.Email, au.FullName
FROM AspNetUsers au
WHERE NOT EXISTS (SELECT 1 FROM Users u WHERE u.AspNetUserId = au.Id);

PRINT 'User tables alignment complete!';
