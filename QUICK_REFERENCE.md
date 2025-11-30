# ?? RBM CMMS - Quick Reference Card

## Start the Application

```powershell
# Option 1: Use quick start script
.\start-with-auth.ps1

# Option 2: Manual
dotnet ef migrations add ExtendApplicationUser --project BlazorApp1
dotnet ef database update --project BlazorApp1
dotnet run --project BlazorApp1
```

## Default Login Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@company.com | Admin123! |
| Engineer | sarah.johnson@company.com | Sarah123! |
| Planner | emily.brown@company.com | Emily123! |
| Technician | john.smith@company.com | John123! |

## URLs

- **Home**: https://localhost:7xxx/
- **Login**: https://localhost:7xxx/Account/Login
- **RBM Dashboard**: https://localhost:7xxx/rbm

## Database Tables

1. Assets - Equipment tracking
2. AssetAttachments - Files & documents
3. AssetDowntime - Downtime events
4. ReliabilityMetrics - MTBF, MTTR, OEE
5. WorkOrders - Maintenance work
6. MaintenanceTasks - Task details
7. MaintenanceSchedules - Planned maintenance
8. ConditionReadings - Sensor data
9. FailureModes - FMEA analysis
10. Users - User management

## Roles & Permissions

| Permission | Admin | Engineer | Planner | Technician |
|------------|-------|----------|---------|------------|
| View | ? | ? | ? | ? |
| Edit | ? | ? | ? | ? |
| Delete | ? | ? | ? | ? |
| Manage Users | ? | ? | ? | ? |
| Modify FMEA | ? | ? | ? | ? |

## Key Files

**Pages:**
- `/rbm` - Dashboard
- `/rbm/assets` - Asset Management
- `/rbm/work-orders` - Work Orders
- `/rbm/analytics` - Analytics
- `/rbm/users` - User Management (Admin only)

**Services:**
- `DataService.cs` - Data operations
- `RolePermissionService.cs` - Authorization
- `CurrentUserService.cs` - Current user info

**Database:**
- `ApplicationDbContext.cs` - EF Core context
- `DbInitializer.cs` - Data seeding
- `IdentityDataSeeder.cs` - User/role seeding

## Common Commands

```bash
# Build
dotnet build

# Run
dotnet run --project BlazorApp1

# Create migration
dotnet ef migrations add MigrationName --project BlazorApp1

# Update database
dotnet ef database update --project BlazorApp1

# List migrations
dotnet ef migrations list --project BlazorApp1

# Drop database (WARNING!)
dotnet ef database drop --project BlazorApp1
```

## Troubleshooting

**Build errors?**
```bash
dotnet clean
dotnet build
```

**Migration issues?**
```bash
dotnet ef migrations remove --project BlazorApp1
dotnet ef migrations add NewMigration --project BlazorApp1
```

**Can't login?**
- Check database for AspNetUsers table
- Verify seeding ran successfully
- Check connection string in appsettings.json

**Hot reload warnings?**
- Normal during development
- Restart app to clear

## Documentation

- `COMPLETE_IMPLEMENTATION_SUMMARY.md` - Full overview
- `AUTHENTICATION_IMPLEMENTATION.md` - Auth guide
- `DATABASE_SCHEMA.md` - Database reference
- `DATABASE_SETUP.md` - Setup instructions

## Support

Check documentation files for:
- Detailed setup
- Customization
- Troubleshooting
- API reference

---

**You're ready to go!** ??

Start with: `.\start-with-auth.ps1`
