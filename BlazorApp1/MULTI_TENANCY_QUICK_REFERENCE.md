# Multi-Tenancy Quick Reference

## Admin Credentials (Default Seeded Data)

| Role | Email | Password |
|------|-------|----------|
| Super Admin | superadmin@company.com | SuperAdmin123! |

## Key URLs

| Feature | URL | Role |
|---------|-----|------|
| Tenant Management | `/rbm/tenants` | SuperAdmin |
| Tenant Users | `/rbm/tenant-users/{tenantId}` | SuperAdmin, TenantAdmin |

## Database Tables

```
Tenants
??? Id (PK)
??? TenantCode (Unique)
??? Name
??? ContactPerson
??? Status (Active, Inactive, Suspended, Archived)
??? IsActive
??? MaxUsers, MaxAssets, MaxDocuments
??? Timestamps (CreatedDate, ModifiedDate)

UserTenantMappings
??? Id (PK)
??? TenantId (FK)
??? UserId (FK)
??? IsTenantAdmin
??? AssignedDate
??? RemovedDate (Soft Delete)

AspNetUsers (Extended)
??? PrimaryTenantId (FK, Nullable)
??? IsSuperAdmin
??? TenantMappings (Navigation)
```

## Role Hierarchy

```
SuperAdmin (Global Access)
  ?? Can manage all tenants
  ?? Can create/delete tenants
  ?? Can assign users to tenants
  ?? Has IsSuperAdmin = true

TenantAdmin (Tenant-scoped)
  ?? Can manage users in their tenant
  ?? Can view tenant data
  ?? Has IsTenantAdmin = true in TenantUserMapping

RegularUser (Tenant-scoped)
  ?? Can view tenant data
  ?? Cannot manage tenants
  ?? Has PrimaryTenantId set
```

## Common Tasks

### Create a Tenant
1. Log in as Super Admin
2. Navigate to `/rbm/tenants`
3. Click "Create New Tenant"
4. Fill in tenant details
5. Click "Save Tenant"

### Add User to Tenant
1. Go to `/rbm/tenants`
2. Click "Users" button on tenant card
3. Click "Add User" button
4. Select user from dropdown
5. (Optional) Check "Make Tenant Admin"
6. Click "Add User"

### Remove User from Tenant
1. Go to `/rbm/tenant-users/{tenantId}`
2. Find user in table
3. Click "Remove" button

### Set Tenant Admin
1. Go to `/rbm/tenant-users/{tenantId}`
2. Check/uncheck "Admin" checkbox for user
3. Changes apply immediately

### Deactivate Tenant
1. Go to `/rbm/tenants`
2. Click "Deactivate" button on tenant card
3. Confirm action

### Delete Tenant
1. Go to `/rbm/tenants`
2. Click "Delete" button on tenant card
3. Confirm action
4. All user mappings are removed automatically

## Code Snippets

### Get Tenant Context
```csharp
@inject ITenantService TenantService

protected override async Task OnInitializedAsync()
{
    var context = await TenantService.GetTenantContextAsync();
    if (context.IsSuperAdmin)
    {
        // Super admin logic
    }
}
```

### Create Tenant Programmatically
```csharp
@inject ITenantManagementService TenantService

var tenant = await TenantService.CreateTenantAsync(
    "ACME", 
    "Acme Corporation", 
    currentUserId);
```

### Get All Tenants
```csharp
var tenants = await TenantService.GetAllTenantsAsync();
foreach (var tenant in tenants)
{
    Console.WriteLine($"{tenant.Name} ({tenant.TenantCode})");
}
```

### Add User to Tenant
```csharp
await TenantService.AddUserToTenantAsync(
    tenantId: 1,
    userId: "user123",
    isTenantAdmin: false);
```

### Check if User is in Tenant
```csharp
bool isInTenant = await TenantService.IsUserInTenantAsync(
    userId: "user123",
    tenantId: 1);
```

## Authorization Policies

Available in `Program.cs`:

```csharp
// Requires SuperAdmin role
[Authorize(Roles = "SuperAdmin")]

// Requires TenantAdmin or SuperAdmin role
[Authorize(Roles = "SuperAdmin,TenantAdmin")]

// Custom policy
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("SuperAdminOnly", 
        policy => policy.RequireRole("SuperAdmin"));
```

## Service Registration

Already configured in `Program.cs`:

```csharp
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantManagementService, TenantManagementService>();
```

## Migration History

| Migration | Date | Changes |
|-----------|------|---------|
| 20251220_AddMultiTenancy | 2025-12-20 | Added Tenants, UserTenantMappings tables |

## Entity Relationships

```
ApplicationUser
  ?? Tenant (1:Many via PrimaryTenantId)
  ?? TenantMappings (1:Many)

Tenant
  ?? Users (1:Many via PrimaryTenantId)
  ?? TenantMappings (1:Many)

TenantUserMapping
  ?? Tenant (Many:1)
  ?? User (Many:1)
```

## Status Values

**Tenant Status**:
- `Active` - Tenant is operational
- `Inactive` - Tenant is disabled (users can't access)
- `Suspended` - Temporarily suspended
- `Archived` - Historical record kept

## Limits and Quotas

Each tenant has:
- **MaxUsers**: Maximum number of users allowed
- **MaxAssets**: Maximum number of assets allowed
- **MaxDocuments**: Maximum number of documents allowed

*Enforcement recommended in business logic layer*

## Important Notes

1. **Super Admin** has `IsSuperAdmin = true` and `PrimaryTenantId = null`
2. **Regular users** must have `PrimaryTenantId` set
3. **Soft deletes** are used for user mappings (RemovedDate != null)
4. **User reassignment**: When removed from primary tenant, user is assigned to next available tenant
5. **Data isolation**: Recommended to add TenantId to all entities for filtering
6. **Audit trail**: All changes tracked with CreatedBy/ModifiedBy and timestamps

## Performance Tips

1. Cache tenant context to avoid repeated queries
2. Use indexes on TenantId + UserId in UserTenantMappings
3. Lazy load tenant relationships only when needed
4. Consider pagination for large tenant lists
5. Filter data at database level, not in memory

## Troubleshooting Checklist

- [ ] User has PrimaryTenantId set
- [ ] User's IsSuperAdmin flag is correct
- [ ] TenantUserMapping exists and RemovedDate is null
- [ ] User has correct role (SuperAdmin, TenantAdmin, etc.)
- [ ] Tenant status is Active
- [ ] Authorization policies are configured
- [ ] Database migration applied successfully
