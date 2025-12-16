# ?? DATABASE MIGRATION GUIDE

## ?? Overview

This guide walks you through adding the UserSettings table to your database to support app-wide unit preferences.

---

## ? Pre-Migration Checklist

- [ ] Build is successful (`dotnet build`)
- [ ] Database is accessible
- [ ] You have a backup (recommended for production)
- [ ] No other migrations are pending

---

## ?? Step-by-Step Migration

### Step 1: Create Migration

Open PowerShell/Terminal in the `BlazorApp1` directory:

```bash
cd BlazorApp1
dotnet ef migrations add AddUserSettings
```

**Expected Output:**
```
info: Microsoft.EntityFrameworkCore.Infrastructure
An Entity Framework migration 'AddUserSettings' was successfully generated.
```

**What it does:**
- Creates a migration file: `Migrations/[timestamp]_AddUserSettings.cs`
- Defines the schema changes needed
- Does NOT modify the database yet

---

### Step 2: Apply Migration to Database

```bash
dotnet ef database update
```

**Expected Output:**
```
info: Microsoft.EntityFrameworkCore.Infrastructure
No type was specified for the decimal column 'UserSettings.DecimalPlaces' in the migration...
info: Microsoft.EntityFrameworkCore.Database.Command
Executed DbCommand (XX ms)
Done.
```

**What it does:**
- Applies the migration to your database
- Creates the `UserSettings` table
- Creates necessary indexes
- Updates the `__EFMigrationsHistory` table

---

## ?? Verify Migration Success

### Method 1: Check Migration History

```bash
dotnet ef migrations list
```

Should show:
```
20241216000000_AddUserSettings
(Most recent)
```

### Method 2: Check Database (SQL Server)

```sql
-- Check if table exists
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'UserSettings';

-- Check table structure
EXEC sp_columns UserSettings;

-- Count records
SELECT COUNT(*) FROM [UserSettings];
```

### Method 3: Application Test

1. Run the application: `dotnet run`
2. Navigate to My Profile (`/rbm/profile`)
3. Scroll to "?? Units & Measurements"
4. Try selecting a different unit system
5. Should see: "? Settings saved successfully"

---

## ?? Rollback (If Needed)

### Rollback Last Migration

```bash
dotnet ef database update [PreviousMigrationName]
```

Example - if you want to revert to the previous migration:

```bash
dotnet ef migrations list
```

Find the migration BEFORE `AddUserSettings`, then:

```bash
dotnet ef database update [PreviousMigrationName]
```

Then remove the migration file:

```bash
dotnet ef migrations remove
```

---

## ?? Migration File Structure

The migration file will look like:

```csharp
public partial class AddUserSettings : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserSettings",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                PreferredUnitSystem = table.Column<string>(type: "nvarchar(20)", nullable: false, defaultValue: "imperial"),
                TemperatureUnit = table.Column<string>(type: "nvarchar(10)", nullable: true),
                PressureUnit = table.Column<string>(type: "nvarchar(10)", nullable: true),
                // ... more columns ...
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserSettings", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserSettings_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserSettings_UserId",
            table: "UserSettings",
            column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserSettings");
    }
}
```

---

## ?? Expected Database Schema

### UserSettings Table

```sql
CREATE TABLE [UserSettings] (
    [Id] INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [PreferredUnitSystem] NVARCHAR(20) NOT NULL DEFAULT 'imperial',
    [TemperatureUnit] NVARCHAR(10) NULL,
    [PressureUnit] NVARCHAR(10) NULL,
    [FlowRateUnit] NVARCHAR(10) NULL,
    [WeightUnit] NVARCHAR(10) NULL,
    [LengthUnit] NVARCHAR(10) NULL,
    [DistanceUnit] NVARCHAR(10) NULL,
    [ThemePreference] NVARCHAR(10) NOT NULL DEFAULT 'auto',
    [DateFormat] NVARCHAR(20) NOT NULL DEFAULT 'MM/dd/yyyy',
    [TimeFormat] NVARCHAR(10) NOT NULL DEFAULT '12h',
    [DecimalPlaces] INT NOT NULL DEFAULT 2,
    [EnableNotifications] BIT NOT NULL DEFAULT 1,
    [NotificationFrequency] NVARCHAR(20) NOT NULL DEFAULT 'immediate',
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME2 NULL,
    
    CONSTRAINT [FK_UserSettings_AspNetUsers_UserId] 
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE,
    
    INDEX [IX_UserSettings_UserId] ON [UserId]
);
```

---

## ?? Troubleshooting

### Issue: "Unable to create a new context"
**Solution:** Check connection string in `appsettings.json`

### Issue: "Migration already exists"
**Solution:** Migration may have been run before:
```bash
dotnet ef database update
```

### Issue: Foreign key constraint fails
**Solution:** Ensure `AspNetUsers` table exists:
```bash
dotnet ef database update  # Update to latest migration
```

### Issue: "No migrations pending"
**Solution:** Migration may already be applied:
```bash
dotnet ef database update
dotnet ef migrations list
```

---

## ?? Performance Considerations

- Migration creates single index on `UserId` for lookups
- Table is small (one row per user)
- No impact on existing queries
- Async operations used everywhere

---

## ?? Security Notes

- Foreign key ensures data integrity
- Cascade delete removes settings if user deleted
- User can only modify their own settings
- Service validates user authorization

---

## ?? Migration Checklist

After migration:

- [ ] Migration applied successfully
- [ ] My Profile page loads
- [ ] Units & Measurements section visible
- [ ] Can save unit preferences
- [ ] Settings persist on reload
- [ ] No console errors
- [ ] Database shows UserSettings table
- [ ] No performance issues

---

## ?? Next Steps

1. ? Migration applied
2. Navigate to `/rbm/profile`
3. Test unit system selection
4. Integrate with other components
5. Monitor for any issues

---

## ?? Support

If migration fails:

1. Check error message in console
2. Verify database connection
3. Check `appsettings.json`
4. Run: `dotnet ef database update --verbose` for detailed logs
5. Rollback if necessary: `dotnet ef database update [PreviousMigration]`

---

**Created**: December 16, 2024  
**Version**: 1.0  
**Status**: ? Ready to Deploy
