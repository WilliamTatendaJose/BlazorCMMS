# User Authentication & Authorization System

## Overview

This document describes the authentication and authorization architecture in the BlazorCMMS application.

## User Table Architecture

The system has **two user tables** that work together:

### 1. `AspNetUsers` (Primary - ASP.NET Core Identity)

| Purpose | Authentication & Authorization |
|---------|------------------------------|
| Used For | Login, password management, roles, 2FA, lockout |
| Managed By | `UserManagementService`, Identity framework |
| Key Fields | `Id (GUID)`, `Email`, `FullName`, `Department`, `PrimaryTenantId`, `IsSuperAdmin` |

### 2. `Users` (Legacy - Work Order Assignments)

| Purpose | Technician assignments, dropdown lists |
| Used For | Work order "Assigned To" field, display names |
| Managed By | Auto-synced from `AspNetUsers` |
| Key Fields | `Id (int)`, `Email`, `Name`, `Role`, `AspNetUserId` (link) |

## Synchronization

The tables are automatically synchronized:

1. **On User Creation** - `UserManagementService.CreateUserAsync()` creates both records
2. **On User Update** - `UserManagementService.UpdateUserAsync()` updates both records
3. **On User Delete** - `UserManagementService.DeleteUserAsync()` removes from both tables
4. **On Startup** - `IdentityDataSeeder.SyncAllUsersToLegacyTableAsync()` syncs all users

## Roles

| Role | Description | Permissions |
|------|-------------|-------------|
| **SuperAdmin** | System administrator | Full access to all tenants and features |
| **TenantAdmin** | Tenant administrator | Manage users within their tenant |
| **Admin** | Organization admin | Manage most features |
| **Reliability Engineer** | Engineering role | Full CRUD on assets, analysis |
| **Planner** | Maintenance planning | Create work orders, schedules |
| **Technician** | Field worker | Complete work orders |
| **Viewer** | Read-only | View all data |

## Authentication Flow

```
???????????????????     ????????????????????     ???????????????????
?   Login Page    ???????  ASP.NET Identity???????  AspNetUsers    ?
???????????????????     ????????????????????     ???????????????????
                                ?
                                ?
                        ????????????????????
                        ? AuthenticationState?
                        ?    Provider       ?
                        ????????????????????
                                ?
                                ?
                        ????????????????????
                        ? RolePermission   ?
                        ?    Service       ?
                        ????????????????????
```

## Authorization in Components

### Using RolePermissionService

```csharp
@inject RolePermissionService RoleService

@code {
    private bool canEdit;
    private bool isSuperAdmin;

    protected override async Task OnInitializedAsync()
    {
        canEdit = await RoleService.CanEditAsync();
        isSuperAdmin = await RoleService.IsSuperAdminAsync();
    }
}
```

### Using AuthorizeView

```razor
<AuthorizeView Roles="SuperAdmin,Admin">
    <Authorized>
        <button>Admin Only Button</button>
    </Authorized>
</AuthorizeView>
```

## Default Users

| Email | Password | Role |
|-------|----------|------|
| superadmin@company.com | SuperAdmin123! | SuperAdmin |
| admin@company.com | Admin123! | Admin |
| sarah.johnson@company.com | Sarah123! | Reliability Engineer |
| emily.brown@company.com | Emily123! | Planner |
| john.smith@company.com | John123! | Technician |
| mike.davis@company.com | Mike123! | Technician |

## Multi-Tenancy

- `SuperAdmin` can access all tenants
- Other users are restricted to their `PrimaryTenantId`
- Data is filtered by `TenantId` in `DataService`

## Manual Sync

If tables get out of sync, run:

```sql
-- Execute: BlazorApp1/Migrations/ALIGN_USER_TABLES.sql
```

Or call programmatically:

```csharp
await IdentityDataSeeder.SyncAllUsersToLegacyTableAsync(userManager, context);
```

## Services Reference

| Service | Purpose |
|---------|---------|
| `UserManagementService` | CRUD operations for users |
| `RolePermissionService` | Check permissions |
| `CurrentUserService` | Get current user info |
| `TenantService` | Tenant operations |
| `TenantManagementService` | Tenant user management |

## Security Considerations

1. **Passwords** - Use ASP.NET Identity password hashing
2. **Lockout** - Account lockout after failed attempts
3. **2FA** - Two-factor authentication available
4. **Tenant Isolation** - Data filtered by TenantId
5. **Role-based Access** - All actions check permissions
