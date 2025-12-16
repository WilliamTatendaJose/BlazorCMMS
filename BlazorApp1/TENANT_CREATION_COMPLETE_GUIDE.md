# TENANT CREATION - COMPLETE DIAGNOSTIC GUIDE

## ?? Quick Solution

If the "Create Tenant" button isn't working, use the new **Diagnostics Page** to identify the issue:

### 1. Open Diagnostics
```
URL: /rbm/tenants-diagnostics
```

### 2. Click "Run All Diagnostics"
- This will check:
  - ? Your authorization (SuperAdmin role)
  - ? Database connection
  - ? Tenants table
  - ? TenantManagementService
  - ? Create operation

### 3. Review Results
- Green = ? Working
- Orange = ?? Issue found
- See details below for each

### 4. Test Creation
- Fill test form on same page
- Click "Test Tenant Creation"
- See if it works

---

## ?? What the Diagnostic Page Checks

### Authorization Check
**What**: Verifies you're logged in as SuperAdmin  
**Expected**: Role = "SuperAdmin"  
**If fails**: Log out and log in as superadmin@company.com

### Database Connection  
**What**: Connects to SQL Server database  
**Expected**: Successfully connects  
**If fails**: Check connection string in appsettings.json

### Tenants Table
**What**: Checks if Tenants table exists  
**Expected**: Table exists with proper schema  
**If fails**: Run migration: `Update-Database`

### Tenant Service
**What**: Tests the TenantManagementService  
**Expected**: Service responds and returns tenant list  
**If fails**: Check service registration in Program.cs

### Create Operation
**What**: Tests creating a sample tenant  
**Expected**: Successfully creates and deletes test tenant  
**If fails**: Database or validation issue

---

## ?? Common Issues & Fixes

### Issue 1: Authorization Failed
```
? User is not SuperAdmin
```

**Fix**:
1. Logout
2. Login as: `superadmin@company.com`
3. Password: `SuperAdmin123!`
4. Try again

### Issue 2: Database Connection Failed
```
? Could not connect to database
```

**Fix**:
1. Check connection string in `appsettings.json`
2. Verify SQL Server is running
3. Verify database exists: `RBM_CMMS`

### Issue 3: Tenants Table Not Found
```
? Tenants table not accessible
```

**Fix**:
1. Open Package Manager Console
2. Run: `Update-Database`
3. This applies all pending migrations

### Issue 4: Create Operation Failed
```
? Failed to create tenant
```

**Possible fixes**:
- Check browser console for detailed error
- Try using different tenant code
- Verify all required fields filled

---

## ?? Browser Console Debugging

### Step 1: Open Console
- Press `F12`
- Click "Console" tab

### Step 2: Look for Messages
```
DEBUG: Attempting to create/update tenant...
DEBUG: Creating new tenant...
DEBUG: Tenant created successfully. ID=1
```

### Step 3: If Errors Appear
```
ERROR: Failed to create tenant: {error details}
```
- Note the error message
- Check if it's a validation error or database error

### Step 4: Network Tab
- Go to "Network" tab
- Create a tenant
- Look for POST requests
- Check response status

---

## ?? Direct Database Checks

### Check Tenants Table Exists
```sql
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'Tenants'
```

**Expected**: 1 row returned

### Check Table Structure
```sql
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tenants'
ORDER BY ORDINAL_POSITION
```

**Expected columns**:
- Id (int, NOT NULL)
- TenantCode (nvarchar(100), NOT NULL)
- Name (nvarchar(200), NOT NULL)
- CreatedDate (datetime2, NOT NULL)
- Status (nvarchar(50), NOT NULL)
- IsActive (bit, NOT NULL)

### Check for Existing Tenants
```sql
SELECT * FROM Tenants
```

**If empty**: No tenants created yet (that's fine)

### Check User Roles
```sql
SELECT u.Email, r.Name as Role
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'superadmin@company.com'
```

**Expected**: SuperAdmin role

---

## ?? Service & Configuration Checks

### Check Program.cs (Services Registration)
**File**: `BlazorApp1/Program.cs`

**Should contain**:
```csharp
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantManagementService, TenantManagementService>();
```

**If missing**: Add these lines and rebuild

### Check Tenants.razor (Component Directives)
**File**: `BlazorApp1/Components/Pages/RBM/Tenants.razor`

**Should have**:
```razor
@page "/rbm/tenants"
@layout RBMLayout
@attribute [Authorize(Roles = "SuperAdmin")]
@inject ITenantManagementService TenantManagementService
```

**If missing**: Add these lines and rebuild

### Check ApplicationDbContext
**File**: `BlazorApp1/Data/ApplicationDbContext.cs`

**Should have**:
```csharp
public DbSet<Tenant> Tenants { get; set; }
public DbSet<TenantUserMapping> UserTenantMappings { get; set; }
```

**If missing**: Add these lines and rebuild

---

## ?? Migration Status

### Check Applied Migrations
```sql
SELECT * FROM __EFMigrationsHistory
```

**Should include**:
- `20251209091112_Tenants`
- `20251209111543_Tenants2`
- `20251220_AddMultiTenancy` or similar

### If Migrations Not Applied
```powershell
# Open Package Manager Console
Update-Database
```

This applies all pending migrations

---

## ? Verification Steps

### Step 1: Authorization ?
- [ ] Logged in as superadmin@company.com
- [ ] Role shows "SuperAdmin"
- [ ] Can see "Tenants" in sidebar

### Step 2: Database ?
- [ ] Tenants table exists in SQL Server
- [ ] Table has correct columns
- [ ] Migrations applied

### Step 3: Services ?
- [ ] TenantManagementService registered in Program.cs
- [ ] TenantService registered in Program.cs
- [ ] Both services compile without errors

### Step 4: Component ?
- [ ] Tenants.razor has @layout RBMLayout
- [ ] Has @attribute [Authorize(Roles = "SuperAdmin")]
- [ ] Page loads without errors

### Step 5: Create Operation ?
- [ ] Click "Create New Tenant" - modal opens
- [ ] Enter tenant code (unique)
- [ ] Enter tenant name
- [ ] Click "Save Tenant"
- [ ] Modal closes
- [ ] Success message appears
- [ ] New tenant appears in list

---

## ?? Step-by-Step Test

### Test Procedure (5 minutes)

1. **Login**
   - Email: superadmin@company.com
   - Password: SuperAdmin123!

2. **Navigate**
   - Go to `/rbm/tenants`

3. **Run Diagnostics**
   - Go to `/rbm/tenants-diagnostics`
   - Click "Run All Diagnostics"
   - Note any failures

4. **Fix Issues**
   - Review fixes for any failed checks
   - Apply fixes
   - Run diagnostics again

5. **Test Creation**
   - Fill test form on diagnostics page
   - Click "Test Tenant Creation"
   - Verify success

6. **Create Real Tenant**
   - Go back to `/rbm/tenants`
   - Click "Create New Tenant"
   - Fill form:
     - Code: `TENANT001`
     - Name: `Test Company`
   - Click "Save"
   - Verify success message
   - Verify tenant appears in list

---

## ?? Support Info to Provide

If you still have issues, provide:

1. **Diagnostics Results**
   - Screenshot of `/rbm/tenants-diagnostics`
   - Note which tests failed

2. **Browser Console**
   - Press F12
   - Go to Console tab
   - Screenshot of any errors

3. **Database Status**
   - Run: `SELECT * FROM __EFMigrationsHistory`
   - Share which migrations are applied

4. **Build Status**
   - Run build
   - Share any errors or warnings

5. **Exact Error Message**
   - Copy the exact error text
   - Include full error details

---

## ?? How Tenant Creation Works

```
1. User clicks "Create New Tenant"
   ?
2. Modal opens with empty form
   ?
3. User fills:
   - Tenant Code (unique)
   - Tenant Name
   ?
4. User clicks "Save Tenant"
   ?
5. Component validates:
   - Code is not empty
   - Name is not empty
   ?
6. Component calls TenantManagementService.CreateTenantAsync()
   ?
7. Service checks:
   - Code is not already used
   - Valid input
   ?
8. Service creates Tenant entity
   ?
9. Service saves to database
   ?
10. Service returns created Tenant
    ?
11. Component shows success message
    ?
12. Component refreshes tenant list
    ?
13. New tenant appears in list
```

---

## ?? Required Configuration

### SuperAdmin User
```
Email: superadmin@company.com
Password: SuperAdmin123!
Role: SuperAdmin
```

### Database
```
Server: (LocalDb)
Database: RBM_CMMS
```

### Connection String
```
DefaultConnection=Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=RBM_CMMS;Integrated Security=true;
```

Location: `appsettings.json`

---

## ?? Success Indicators

You'll know it's working when:
1. ? Diagnostics page shows all green checks
2. ? Test Tenant Creation succeeds  
3. ? Modal opens on "Create New Tenant" click
4. ? Form accepts input
5. ? "Save Tenant" button responds
6. ? Success message appears
7. ? New tenant appears in list
8. ? No console errors

---

**Updated**: 2025-12-20  
**Version**: 2.0 - With Diagnostics Page

