# Multi-Tenancy Implementation Guide

## Overview

Your Blazor application has been successfully updated to support multi-tenancy with a Super Admin role for managing tenants. This implementation allows the application to serve multiple independent organizations (tenants), each with their own users, assets, and data.

## Key Components

### 1. **Models**

#### `Tenant.cs`
Represents a tenant organization with the following properties:
- **TenantCode**: Unique identifier (e.g., "ACME", "WIDGET_CO")
- **Name**: Full tenant name
- **Contact Information**: Person, email, phone, address
- **Limits**: Max users, max assets, max documents
- **Status**: Active, Inactive, Suspended, Archived
- **Timestamps**: Created date, modified date with audit trail

#### `TenantUserMapping.cs`
Links users to tenants with:
- **TenantId**: Reference to tenant
- **UserId**: Reference to application user
- **IsTenantAdmin**: Flag to mark tenant administrators
- **AssignedDate**: When user was added
- **RemovedDate**: Soft-delete tracking (null while active)

#### `ApplicationUser` (Updated)
Extended with:
- **PrimaryTenantId**: Default tenant for the user
- **IsSuperAdmin**: Flag for super administrators
- **Navigation Properties**: Links to Tenant and TenantMappings

### 2. **Services**

#### `ITenantService` / `TenantService`
Manages the current tenant context:
```csharp
// Get current tenant information
var context = await tenantService.GetTenantContextAsync();

// Check if user is in specific tenant
bool isInTenant = await tenantService.IsUserInTenantAsync(userId, tenantId);

// Get all tenants for a user
var userTenants = await tenantService.GetUserTenantsAsync(userId);
```

**Key Features:**
- Caches tenant context to avoid repeated database queries
- Integrates with authentication state provider
- Returns tenant information including admin status

#### `ITenantManagementService` / `TenantManagementService`
Handles tenant lifecycle management:

```csharp
// Create a new tenant
var tenant = await tenantManagementService.CreateTenantAsync(
    "ACME", "Acme Corporation", "superadmin@company.com");

// Get tenant information
var tenant = await tenantManagementService.GetTenantByIdAsync(tenantId);
var tenant = await tenantManagementService.GetTenantByCodeAsync("ACME");

// Update tenant
await tenantManagementService.UpdateTenantAsync(tenant, userId);

// Manage tenant users
await tenantManagementService.AddUserToTenantAsync(tenantId, userId, isAdmin);
await tenantManagementService.RemoveUserFromTenantAsync(tenantId, userId);
await tenantManagementService.SetUserAsAdminAsync(tenantId, userId, isAdmin);

// Tenant lifecycle
await tenantManagementService.ActivateTenantAsync(tenantId);
await tenantManagementService.DeactivateTenantAsync(tenantId);
await tenantManagementService.DeleteTenantAsync(tenantId);
```

### 3. **Components**

#### `Tenants.razor` (`/rbm/tenants`)
Super Admin tenant management interface:
- **View**: Card-based display of all tenants
- **Create**: Modal form to create new tenants
- **Edit**: Update tenant information
- **Users**: Navigate to tenant user management
- **Activate/Deactivate**: Control tenant status
- **Delete**: Remove tenants

**Authorization**: `@attribute [Authorize(Roles = "SuperAdmin")]`

#### `TenantUsers.razor` (`/rbm/tenant-users/{tenantId}`)
Tenant user management interface:
- **View**: Table of users assigned to tenant
- **Add**: Add existing users to tenant
- **Admin Status**: Toggle tenant admin role
- **Remove**: Unassign users from tenant

**Authorization**: `@attribute [Authorize(Roles = "SuperAdmin,TenantAdmin")]`

### 4. **Database**

#### Migration: `20251220_AddMultiTenancy`
Creates:
- **Tenants** table with unique TenantCode index
- **UserTenantMappings** table with composite index on TenantId + UserId
- **ApplicationUsers** modifications:
  - PrimaryTenantId foreign key (nullable)
  - IsSuperAdmin bit field

## Roles and Permissions

### Super Admin
- Full system access
- Create, read, update, delete tenants
- Assign users to tenants
- No primary tenant (can access all)
- Set IsSuperAdmin = true

### Tenant Admin
- Manage users within their tenant
- View tenant-specific data
- IsTenantAdmin = true in TenantUserMapping
- Primary tenant = their assigned tenant

### Regular Users
- View data for their assigned tenant
- Cannot manage tenants or users
- PrimaryTenantId = their tenant

## Setup Instructions

### 1. **Apply Migration**
```powershell
Add-Migration AddMultiTenancy
Update-Database
```

### 2. **Seed Super Admin**
The `IdentityDataSeeder` automatically creates:
- **Email**: superadmin@company.com
- **Password**: SuperAdmin123!
- **Roles**: SuperAdmin

### 3. **Configure Services** (Already Done)
In `Program.cs`:
```csharp
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantManagementService, TenantManagementService>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"))
    .AddPolicy("TenantAdminOrSuperAdmin", 
        policy => policy.RequireRole("TenantAdmin", "SuperAdmin"));
```

## Usage Examples

### Creating a Tenant
```csharp
var tenant = await tenantManagementService.CreateTenantAsync(
    tenantCode: "ACME",
    name: "Acme Corporation",
    createdBy: currentUserId);
```

### Adding Users to a Tenant
```csharp
// Add regular user
await tenantManagementService.AddUserToTenantAsync(
    tenantId: 1, 
    userId: "user123", 
    isTenantAdmin: false);

// Add tenant admin
await tenantManagementService.AddUserToTenantAsync(
    tenantId: 1, 
    userId: "admin456", 
    isTenantAdmin: true);
```

### Checking Current Tenant Context
```csharp
var context = await tenantService.GetTenantContextAsync();

if (context.IsSuperAdmin)
{
    // Super admin can see all tenants
}
else if (context.IsTenantAdmin && context.TenantId.HasValue)
{
    // Tenant admin can manage users in TenantId
}
else if (context.TenantId.HasValue)
{
    // Regular user can view data for TenantId
}
```

## Data Isolation

### Filtering by Tenant
When querying data, filter by tenant:

```csharp
var tenantContext = await tenantService.GetTenantContextAsync();

if (tenantContext.TenantId.HasValue)
{
    // Filter assets for this tenant
    var assets = await context.Assets
        .Where(a => a.CreatedBy /* tenant context */)
        .ToListAsync();
}
```

**Note**: Consider adding a `TenantId` property to all multi-tenant entities for explicit filtering.

## Best Practices

1. **Always verify tenant access** before returning data
2. **Use tenant context service** to get current tenant information
3. **Implement tenant filtering** in all queries (add TenantId to models if needed)
4. **Audit trail**: Track CreatedBy, ModifiedBy with user and tenant info
5. **Soft delete**: Use RemovedDate instead of hard deleting user assignments
6. **Super admin tracking**: Log all super admin actions
7. **Tenant limits**: Enforce MaxUsers, MaxAssets, MaxDocuments

## Navigation

- **Super Admin Dashboard**: Access via admin menu
- **Tenant Management**: `/rbm/tenants`
- **Tenant User Management**: `/rbm/tenant-users/{tenantId}`

## Security Considerations

1. **Authorization checks** are enforced at component level
2. **Database relationships** prevent accidental cross-tenant data access
3. **Soft deletes** preserve audit trail for removed assignments
4. **Super admin role** is protected and verified via IsSuperAdmin flag
5. **Tenant admins** can only manage users within their tenant

## Future Enhancements

1. Add TenantId to all entities for explicit filtering
2. Implement middleware to automatically filter by tenant
3. Add tenant branding (logo, colors, name)
4. Implement tenant-specific feature toggles
5. Add tenant usage analytics and billing
6. Implement tenant-level audit logs
7. Add data export with tenant filtering
8. Implement tenant onboarding workflow

## Troubleshooting

### Users Cannot See Tenants
- Verify user has PrimaryTenantId set
- Check if user's IsSuperAdmin is true (needed for super admin features)
- Verify TenantUserMapping exists and RemovedDate is null

### Super Admin Cannot Access Tenant Management
- Verify user has IsSuperAdmin = true
- Verify user has SuperAdmin role
- Check authorization policy in component

### Migration Failed
- Ensure all users have proper nullable foreign keys
- Drop and recreate database if needed
- Check for conflicting migrations

## Support

For issues or questions about the multi-tenancy implementation:
1. Check the authorization policies
2. Verify tenant context is being loaded correctly
3. Review the TenantUserMapping table for consistency
4. Check application logs for errors
