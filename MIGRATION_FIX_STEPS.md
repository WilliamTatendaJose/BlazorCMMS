# Manual Migration Fix Steps

## Current Issue
The migration is failing because Identity tables already exist in the database.

## Solution Options

### Option A: Clean Slate (Recommended for Development)

#### Step 1: Remove All Migrations
```powershell
# In PowerShell
Remove-Item -Path "BlazorApp1\Migrations\*" -Recurse -Force
```

OR manually delete the `BlazorApp1/Migrations` folder.

#### Step 2: Drop the Database
```powershell
dotnet ef database drop --project BlazorApp1 --force
```

#### Step 3: Create Fresh Migration
```powershell
dotnet ef migrations add InitialCreate --project BlazorApp1
```

#### Step 4: Apply Migration
```powershell
dotnet ef database update --project BlazorApp1
```

#### Step 5: Run Application (to seed data)
```powershell
dotnet run --project BlazorApp1
```

---

### Option B: Keep Existing Data (Production Scenario)

If you have data you want to keep:

#### Step 1: Remove Failed Migration
```powershell
dotnet ef migrations remove --project BlazorApp1
```

#### Step 2: Check What's in Database
```powershell
# List all migrations that have been applied
dotnet ef migrations list --project BlazorApp1
```

#### Step 3: Add Migration with Correct Baseline

If Identity tables exist but RBM tables don't:

```powershell
# Create migration only for RBM tables
dotnet ef migrations add AddRBMTables --project BlazorApp1
```

Then edit the migration file to remove Identity table creation code.

#### Step 4: Apply Migration
```powershell
dotnet ef database update --project BlazorApp1
```

---

### Option C: Use Script (Easiest)

```powershell
.\clean-migration.ps1
```

This will:
1. ? Remove all migrations
2. ? Drop database
3. ? Create fresh migration
4. ? Apply migration
5. ? Ready for seeding

---

## After Migration Success

Run the application:
```powershell
dotnet run --project BlazorApp1
```

The application will automatically seed:
- ? 4 Roles (Admin, Reliability Engineer, Planner, Technician)
- ? 5 Users with passwords
- ? 10 Assets
- ? Sample work orders, schedules, etc.

---

## Troubleshooting

### Error: "Cannot drop database because it is currently in use"
Close Visual Studio, SSMS, or any other connections to the database, then try again.

### Error: "No migrations found"
You need to create a migration first:
```powershell
dotnet ef migrations add InitialCreate --project BlazorApp1
```

### Error: "A network-related or instance-specific error"
Check your connection string in `appsettings.json`.

---

## Recommended Approach

For development: **Use Option A or Option C (clean-migration.ps1)**

For production: **Use Option B** (preserve existing data)

---

## What Tables Will Be Created

### Identity Tables (ASP.NET Core Identity)
- AspNetRoles
- AspNetUsers
- AspNetUserRoles
- AspNetUserClaims
- AspNetUserLogins
- AspNetUserTokens
- AspNetRoleClaims

### RBM CMMS Tables
- Assets
- AssetAttachments
- AssetDowntime
- ReliabilityMetrics
- WorkOrders
- MaintenanceTasks
- MaintenanceSchedules
- ConditionReadings
- FailureModes
- Users

**Total: 17 tables**

---

## Quick Start After Fix

1. **Clean migration**: `.\clean-migration.ps1`
2. **Run app**: `dotnet run --project BlazorApp1`
3. **Login**: `admin@company.com` / `Admin123!`
4. **Navigate to**: `/rbm`

Done! ??
