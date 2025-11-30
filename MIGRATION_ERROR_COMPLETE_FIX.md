# Migration Error Fix - Complete Guide

## ? Current Error

```
There is already an object named 'AspNetRoles' in the database.
```

This means:
- The database already has Identity tables
- The migration is trying to create them again
- This typically happens when migrations get out of sync

---

## ? Recommended Solution: Clean Slate

### Quick Fix (Easiest)

```powershell
# Run this script - it does everything automatically
.\clean-migration.ps1
```

### What It Does
1. Removes all migration files
2. Drops the database
3. Creates a fresh migration
4. Applies the migration
5. Ready for seeding on first run

---

## ?? Manual Fix Steps

If you prefer to do it manually:

### Step 1: Remove All Migrations
```powershell
# PowerShell
Remove-Item -Path "BlazorApp1\Migrations\*" -Recurse -Force
```

### Step 2: Drop Database
```powershell
dotnet ef database drop --project BlazorApp1 --force
```

**Output should be:**
```
Build started...
Build succeeded.
Dropping database 'aspnet-BlazorApp1-...' on server '(localdb)\mssqllocaldb'.
Successfully dropped database 'aspnet-BlazorApp1-...'.
```

### Step 3: Create Fresh Migration
```powershell
dotnet ef migrations add InitialCreate --project BlazorApp1
```

**Output should be:**
```
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'
```

### Step 4: Apply Migration
```powershell
dotnet ef database update --project BlazorApp1
```

**Output should be:**
```
Build started...
Build succeeded.
Applying migration '20241130xxxxxx_InitialCreate'.
Done.
```

### Step 5: Run Application
```powershell
dotnet run --project BlazorApp1
```

Application will auto-seed data on first run.

---

## ??? Alternative: Keep Existing Database

If you have data you want to preserve:

### Check Current State
```powershell
# See what tables exist
dotnet ef dbcontext info --project BlazorApp1

# See what migrations are applied
dotnet ef migrations list --project BlazorApp1
```

### Option A: Manual Table Creation

If Identity tables exist but RBM tables don't, you can create them manually:

1. Generate SQL script from migration:
```powershell
dotnet ef migrations script --project BlazorApp1 --output migration.sql
```

2. Edit `migration.sql` - remove Identity table creation
3. Run edited SQL in SQL Server Management Studio

### Option B: Baseline Migration

Tell EF Core that Identity tables already exist:

```powershell
# Remove failed migration
dotnet ef migrations remove --project BlazorApp1

# Create migration
dotnet ef migrations add InitialCreate --project BlazorApp1

# Generate SQL to see what it will create
dotnet ef migrations script --project BlazorApp1 --output check.sql
```

Then manually edit the migration file to skip existing tables.

---

## ?? Common Issues & Solutions

### Issue 1: "Cannot drop database because it is currently in use"

**Solution:**
```powershell
# Close all connections, then:
Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue
Stop-Process -Name "iisexpress" -Force -ErrorAction SilentlyContinue
dotnet ef database drop --project BlazorApp1 --force
```

### Issue 2: "Unable to create an object of type 'ApplicationDbContext'"

**Solution:**
Check `appsettings.json` has correct connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RBM_CMMS;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Issue 3: "The EF Core tools version is older than the runtime"

**Solution:**
```powershell
dotnet tool update --global dotnet-ef
```

### Issue 4: Multiple migration files exist

**Solution:**
```powershell
# Remove ALL migrations and start fresh
Remove-Item -Path "BlazorApp1\Migrations\*" -Force -Recurse
dotnet ef database drop --project BlazorApp1 --force
dotnet ef migrations add InitialCreate --project BlazorApp1
dotnet ef database update --project BlazorApp1
```

---

## ?? Verification Checklist

After running the fix, verify:

- [ ] No errors when running migration
- [ ] Database created successfully
- [ ] All 17 tables exist:
  - [ ] 7 Identity tables (AspNet*)
  - [ ] 10 RBM CMMS tables
- [ ] Application starts without errors
- [ ] Can navigate to `/rbm`
- [ ] Data is seeded (5 users, 10 assets, etc.)
- [ ] Can login with `admin@company.com` / `Admin123!`

---

## ?? What Should Work After Fix

### Database Tables

**Identity Tables (7):**
```
AspNetRoles
AspNetUsers
AspNetUserRoles
AspNetUserClaims
AspNetUserLogins
AspNetUserTokens
AspNetRoleClaims
```

**RBM CMMS Tables (10):**
```
Assets
AssetAttachments
AssetDowntime
ReliabilityMetrics
WorkOrders
MaintenanceTasks
MaintenanceSchedules
ConditionReadings
FailureModes
Users
```

### Sample Data Seeded

**Roles:**
- Admin
- Reliability Engineer
- Planner
- Technician

**Users:**
- admin@company.com / Admin123!
- sarah.johnson@company.com / Sarah123!
- emily.brown@company.com / Emily123!
- john.smith@company.com / John123!
- mike.davis@company.com / Mike123!

**Assets:**
- 10 assets with full details
- 150+ condition readings
- 5 failure modes
- 6 work orders
- 2 downtime records
- 60 reliability metrics

---

## ?? Quick Commands Reference

### Clean Everything
```powershell
.\clean-migration.ps1
```

### Check Status
```powershell
# List migrations
dotnet ef migrations list --project BlazorApp1

# Show database info
dotnet ef dbcontext info --project BlazorApp1

# Generate SQL script
dotnet ef migrations script --project BlazorApp1
```

### Manual Process
```powershell
# 1. Remove migrations
Remove-Item BlazorApp1\Migrations\* -Recurse -Force

# 2. Drop database
dotnet ef database drop --project BlazorApp1 --force

# 3. Create migration
dotnet ef migrations add InitialCreate --project BlazorApp1

# 4. Apply migration
dotnet ef database update --project BlazorApp1

# 5. Run app
dotnet run --project BlazorApp1
```

---

## ?? Additional Resources

- [EF Core Migrations Docs](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)
- [ASP.NET Core Identity Docs](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb)

---

## ?? Pro Tips

1. **Always backup** before dropping database in production
2. **Use migrations** for schema changes, not manual SQL
3. **Test migrations** on development before production
4. **Keep migrations** in source control
5. **Use scripts** for consistent deployments

---

## ? Summary

**Recommended approach for development:**
```powershell
.\clean-migration.ps1
dotnet run --project BlazorApp1
```

**Then navigate to:**
- http://localhost:5xxx/Account/Login
- Login: admin@company.com / Admin123!
- Go to: /rbm

**You're done!** ??
