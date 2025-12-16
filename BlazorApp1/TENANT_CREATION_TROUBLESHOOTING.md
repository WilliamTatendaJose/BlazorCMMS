# Tenant Creation Button - Troubleshooting Guide

## Issue: Create Tenant Button Not Working

If the "Create New Tenant" button is not working, follow this troubleshooting guide to diagnose and fix the issue.

---

## ?? Step 1: Check Browser Console

1. **Open Developer Tools**
   - Press `F12` or `Ctrl+Shift+I`
   - Go to "Console" tab

2. **Look for DEBUG messages**
   - You should see lines like:
     ```
     DEBUG: Attempting to create/update tenant. Code=TENANT001, Name=ABC Corp...
     DEBUG: Creating new tenant...
     DEBUG: Tenant created successfully. ID=1
     ```

3. **Look for ERROR messages**
   - If you see `ERROR:` messages, note them down
   - These will help identify the root cause

---

## ?? Step 2: Test Connection

### Is the Page Loading?
- [ ] Can you see "Tenant Management" heading
- [ ] Can you see "Create New Tenant" button
- [ ] Can you see any existing tenants (if any exist)

### Does the Modal Open?
1. Click "Create New Tenant" button
2. A modal should appear
   - [ ] Modal title shows "Create New Tenant"
   - [ ] Form fields are empty with placeholders
   - [ ] "Save Tenant" button is visible

---

## ?? Step 3: Check Authorization

### Are You Logged In as SuperAdmin?
1. Check the top-right corner
2. You should see: `superadmin@company.com` or similar
3. Role should show: `SuperAdmin` or `Admin`

**If NOT SuperAdmin:**
- Log out and log in again
- Email: `superadmin@company.com`
- Password: `SuperAdmin123!`

### Check User Role in Database
```sql
SELECT u.Id, u.Email, r.Name as Role
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'superadmin@company.com'
```

Expected result: Role should be "SuperAdmin"

---

## ?? Step 4: Fill Form and Submit

### Fill Required Fields
1. **Tenant Code** (Required)
   - Example: `TENANT001` or `ACME001`
   - Must be unique
   - No spaces allowed

2. **Tenant Name** (Required)
   - Example: `ABC Corporation`
   - Can contain spaces

3. **Optional Fields**
   - Description, Contact Person, Email, Phone
   - These can be left empty

### Click Save

### Monitor Console
- Watch the console for `DEBUG:` messages
- Note any `ERROR:` messages

---

## ? Common Issues & Solutions

### Issue 1: "Failed to create tenant. The tenant code may already exist."

**Cause**: Tenant code is not unique

**Solution**:
```sql
SELECT * FROM Tenants WHERE TenantCode = 'TENANT001'
```
- If found, use a different code (e.g., TENANT002)
- Or delete the existing tenant first

### Issue 2: "Tenant Code is required"

**Cause**: Code field is empty

**Solution**:
- Make sure to fill the "Tenant Code" field
- Don't leave it blank

### Issue 3: "Tenant Name is required"

**Cause**: Name field is empty

**Solution**:
- Make sure to fill the "Tenant Name" field
- Don't leave it blank

### Issue 4: Modal opens but button doesn't respond

**Cause**: Possible service issue

**Solution**:
1. Check console for errors
2. Refresh the page (F5)
3. Try again

### Issue 5: Modal doesn't open at all

**Cause**: Possible RBMLayout issue

**Solution**:
1. Check if page has RBM sidebar
2. Refresh the page
3. Check browser console for JavaScript errors

### Issue 6: Error message: "Error saving tenant: Timeout"

**Cause**: Database or network timeout

**Solution**:
1. Check database connection string in `appsettings.json`
2. Verify database is running
3. Check network connectivity

---

## ?? Step 5: Check Database

### Verify Tenants Table Exists
```sql
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'Tenants'
```

**Expected**: One row with "Tenants"

### Check Tenants Table Structure
```sql
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tenants'
ORDER BY ORDINAL_POSITION
```

**Expected columns**:
- Id (int, NOT NULL)
- TenantCode (nvarchar, NOT NULL)
- Name (nvarchar, NOT NULL)
- Description (nvarchar, NULL)
- ... (other fields)

### Check for Data
```sql
SELECT * FROM Tenants
```

---

## ?? Step 6: Check Service Registration

### Verify Services in Program.cs

```csharp
// Should contain:
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantManagementService, TenantManagementService>();
```

**Location**: `BlazorApp1/Program.cs` around line 32-33

**If Missing**: Add these lines and rebuild

---

## ?? Step 7: Verify Component Directives

### Check Tenants.razor Top

```razor
@page "/rbm/tenants"
@layout RBMLayout
@attribute [Authorize(Roles = "SuperAdmin")]
@inject ITenantManagementService TenantManagementService
```

**All should be present**

---

## ?? Step 8: Check Network Tab

1. Open Developer Tools
2. Go to "Network" tab
3. Click "Create New Tenant"
4. Fill form and click "Save Tenant"
5. Look for POST requests to:
   - `/api/` or similar
   - Check response status (should be 200)

**If you see 404 or 500 errors**:
- Check server logs
- Verify endpoint routing

---

## ??? Advanced Debugging

### Enable Detailed Logging

Edit `Program.cs`:
```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

Then check:
1. Visual Studio Output window
2. Browser console
3. Server logs

### Check DbContext

Ensure DbContext is properly initialized:
```sql
SELECT * FROM __EFMigrationsHistory
```

Should show applied migrations including:
- `20251220_AddMultiTenancy`
- `20251209111543_Tenants2`

---

## ?? If Still Not Working

### Gather Information
1. **Screenshot of error message**
2. **Browser console output** (F12 ? Console)
3. **Network tab requests** (F12 ? Network)
4. **Database query results**
5. **Visual Studio output window**

### Provide Details
- When did it last work?
- What changed?
- What exact error message do you see?
- Did you apply migrations?

---

## ? Success Indicators

You'll know it's working when:

1. ? Modal opens on click
2. ? Form fields are editable
3. ? Can enter Tenant Code and Name
4. ? Click "Save Tenant"
5. ? Modal closes
6. ? Success message appears: "Tenant 'ABC Corporation' created successfully!"
7. ? New tenant appears in the list below

---

## ?? Quick Test

### Test Procedure (5 minutes)

1. Login as superadmin@company.com
2. Navigate to `/rbm/tenants`
3. Click "Create New Tenant"
4. Fill:
   - Code: `TEST001`
   - Name: `Test Company`
5. Click "Save Tenant"
6. Wait for success message
7. See tenant in list

**Result**: ? Success | ? Note error message

---

## ?? Diagnostic Checklist

- [ ] Logged in as SuperAdmin
- [ ] /rbm/tenants page loads
- [ ] "Create New Tenant" button visible
- [ ] Modal opens on click
- [ ] Form fields editable
- [ ] Console shows "DEBUG:" messages
- [ ] No "ERROR:" messages in console
- [ ] Tenants table exists in database
- [ ] Services registered in Program.cs
- [ ] Migration applied (20251220_AddMultiTenancy)
- [ ] RBMLayout working (sidebar visible)

---

## ?? Support

If you still can't create tenants:
1. Check browser console (F12)
2. Note down the ERROR message
3. Share:
   - Error message
   - Browser/version
   - When it started happening

---

**Updated**: 2025-12-20  
**Version**: 1.0

