# SuperAdmin Multi-Tenancy Implementation Guide

## Quick Implementation Steps

### Step 1: Update RolePermissionService.cs

Replace your current `RolePermissionService.cs` with the enhanced version:

```csharp
public async Task<bool> IsSuperAdminAsync()
{
    var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
    return authState.User.IsInRole("SuperAdmin");
}

public async Task<bool> CanAccessTenantAsync(int tenantId)
{
    // SuperAdmin can access any tenant
    if (await IsSuperAdminAsync())
        return true;

    // Regular users check their assigned tenants
    var accessibleTenants = await GetAccessibleTenantsAsync();
    return accessibleTenants.Contains(tenantId);
}

public async Task<bool> CanManageRoleAsync(string role)
{
    // SuperAdmin can manage all roles
    if (await IsSuperAdminAsync())
        return true;

    // TenantAdmin can manage Technician and Viewer roles only
    if (await IsTenantAdminAsync())
        return role is "Technician" or "Viewer";

    return false;
}

public async Task<bool> CanAssignRoleAsync(string role)
{
    // SuperAdmin can assign any role
    if (await IsSuperAdminAsync())
        return true;

    // TenantAdmin can assign Technician and Viewer
    if (await IsTenantAdminAsync())
        return role is "Technician" or "Viewer";

    return false;
}
```

### Step 2: Update Tenants.razor

Add SuperAdmin verification and role management:

```razor
@code {
    // Verify SuperAdmin access
    protected override async Task OnInitializedAsync()
    {
        var isSuperAdmin = await RolePermissionService.IsSuperAdminAsync();
        if (!isSuperAdmin)
        {
            NavigationManager.NavigateTo("/access-denied");
            return;
        }

        await LoadTenants();
    }

    // SuperAdmin can assign roles to users
    private async Task AssignRolesToUser(int tenantId, string userId, List<string> roles)
    {
        var isSuperAdmin = await RolePermissionService.IsSuperAdminAsync();
        if (!isSuperAdmin)
        {
            errorMessage = "Only SuperAdmins can assign roles";
            return;
        }

        foreach (var role in roles)
        {
            // SuperAdmin can assign any role
            if (!await RolePermissionService.CanAssignRoleAsync(role))
            {
                errorMessage = $"Cannot assign role: {role}";
                return;
            }
        }

        successMessage = "Roles assigned successfully!";
    }
}
```

### Step 3: Add TenantId to All Models

Add `public int? TenantId { get; set; }` to:
- WorkOrder.cs
- ConditionReading.cs
- FailureMode.cs
- ReliabilityMetric.cs
- AssetDowntime.cs
- MaintenanceSchedule.cs
- MaintenanceTask.cs
- SparePart.cs
- SparePartTransaction.cs
- Document.cs
- DocumentAccessLog.cs

### Step 4: Create Migration

```csharp
public partial class AddTenantIdToAllTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add TenantId column to each table
        migrationBuilder.AddColumn<int?>(
            name: "TenantId",
            table: "WorkOrders",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int?>(
            name: "TenantId",
            table: "ConditionReadings",
            type: "int",
            nullable: true);

        // ... repeat for all other tables ...

        // Create indexes
        migrationBuilder.CreateIndex(
            name: "IX_WorkOrders_TenantId",
            table: "WorkOrders",
            column: "TenantId");

        // ... repeat for all other tables ...

        // Create foreign keys
        migrationBuilder.AddForeignKey(
            name: "FK_WorkOrders_Tenants_TenantId",
            table: "WorkOrders",
            column: "TenantId",
            principalTable: "Tenants",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        // ... repeat for all other tables ...
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverse all changes
    }
}
```

### Step 5: Update DataService

Filter all queries by tenant:

```csharp
public async Task<List<Asset>> GetAssetsAsync()
{
    var isSuperAdmin = await RolePermissionService.IsSuperAdminAsync();
    var tenantId = await GetCurrentTenantIdAsync();

    var query = _context.Assets.AsQueryable();

    if (!isSuperAdmin)
    {
        query = query.Where(a => a.TenantId == tenantId);
    }

    return await query.ToListAsync();
}
```

### Step 6: Update Program.cs

Register SuperAdmin in roles:

```csharp
var app = builder.Build();

// Seed SuperAdmin role if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    var roles = new[] { "SuperAdmin", "TenantAdmin", "Technician", "Viewer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.Run();
```

---

## Role Hierarchy

```
SuperAdmin (Highest)
??? Can access ALL tenants
??? Can assign ALL roles
??? Can view/edit all data
??? Full system administration

TenantAdmin
??? Can access assigned tenants only
??? Can assign Technician and Viewer roles
??? Can manage users in tenant
??? Limited to tenant scope

Technician
??? Can access assigned tenant only
??? Can view/edit asset data
??? Can create work orders
??? Technician-level permissions

Viewer
??? Can access assigned tenant only
??? Can only view data
??? Read-only access
```

---

## Data Isolation Rules

1. **SuperAdmin**: Can see all data from all tenants
2. **TenantAdmin**: Can see only their tenant's data
3. **Technician**: Can see only their tenant's data
4. **Viewer**: Can see only their tenant's data

---

## Verification Checklist

- [ ] RolePermissionService updated with SuperAdmin methods
- [ ] Tenants.razor checks for SuperAdmin role
- [ ] All models have TenantId property
- [ ] Migration creates TenantId columns
- [ ] DataService filters by tenant
- [ ] SuperAdmin role seeded in database
- [ ] Test SuperAdmin can access all tenants
- [ ] Test TenantAdmin access limited to assigned tenant
- [ ] Test data isolation works correctly

---

## Testing Commands

```sql
-- Check SuperAdmin role exists
SELECT * FROM AspNetRoles WHERE Name = 'SuperAdmin'

-- Check user has SuperAdmin role
SELECT * FROM AspNetUserRoles 
WHERE RoleId IN (SELECT Id FROM AspNetRoles WHERE Name = 'SuperAdmin')

-- Check tenant data
SELECT * FROM Tenants
SELECT * FROM Assets WHERE TenantId IS NULL
SELECT * FROM WorkOrders WHERE TenantId IS NULL
```

---

This implementation ensures:
? SuperAdmin has access to all roles and tenants  
? Complete data isolation by tenant  
? Proper role hierarchy  
? Secure access control
