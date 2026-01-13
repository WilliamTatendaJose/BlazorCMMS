# Permission Management System

## Overview

The BlazorCMMS uses a role-based access control (RBAC) system with tenant isolation. This document describes the permission architecture and how to use it.

## Role Hierarchy

Roles are arranged in a hierarchy from most to least privileged:

| Level | Role | Description |
|-------|------|-------------|
| 0 | **SuperAdmin** | Full system access, all tenants |
| 1 | **TenantAdmin** | Full tenant access, manage tenant users |
| 2 | **Admin** | Administrative access within tenant |
| 3 | **Reliability Engineer** | Engineering access, FMEA, analysis |
| 4 | **Planner** | Planning access, schedules, work orders |
| 5 | **Supervisor** | Supervisory access, team management |
| 6 | **Technician** | Operational access, complete work |
| 7 | **Viewer** | Read-only access |

## Permission Matrix

### User Management

| Permission | SuperAdmin | TenantAdmin | Admin | Rel. Eng. | Planner | Supervisor | Technician | Viewer |
|------------|:----------:|:-----------:|:-----:|:---------:|:-------:|:----------:|:----------:|:------:|
| View Users | ? | ? | ? | ? | ? | ? | ? | ? |
| Create Users | ? | ? | ? | ? | ? | ? | ? | ? |
| Edit Users | ? | ? | ? | ? | ? | ? | ? | ? |
| Delete Users | ? | ? | ? | ? | ? | ? | ? | ? |
| Assign SuperAdmin | ? | ? | ? | ? | ? | ? | ? | ? |
| Assign TenantAdmin | ? | ? | ? | ? | ? | ? | ? | ? |
| Assign Admin | ? | ? | ? | ? | ? | ? | ? | ? |
| Assign Planner+ | ? | ? | ? | ? | ? | ? | ? | ? |
| Assign Technician/Viewer | ? | ? | ? | ? | ? | ? | ? | ? |

### Asset Management

| Permission | SuperAdmin | TenantAdmin | Admin | Rel. Eng. | Planner | Supervisor | Technician | Viewer |
|------------|:----------:|:-----------:|:-----:|:---------:|:-------:|:----------:|:----------:|:------:|
| View Assets | ? | ? | ? | ? | ? | ? | ? | ? |
| Create Assets | ? | ? | ? | ? | ? | ? | ? | ? |
| Edit Assets | ? | ? | ? | ? | ? | ? | ? | ? |
| Delete/Retire Assets | ? | ? | ? | ? | ? | ? | ? | ? |

### Work Orders

| Permission | SuperAdmin | TenantAdmin | Admin | Rel. Eng. | Planner | Supervisor | Technician | Viewer |
|------------|:----------:|:-----------:|:-----:|:---------:|:-------:|:----------:|:----------:|:------:|
| View All WOs | ? | ? | ? | ? | ? | ? | ? | ? |
| View Assigned WOs | ? | ? | ? | ? | ? | ? | ? | ? |
| Create WOs | ? | ? | ? | ? | ? | ? | ? | ? |
| Request WOs | ? | ? | ? | ? | ? | ? | ? | ? |
| Assign WOs | ? | ? | ? | ? | ? | ? | ? | ? |
| Approve WOs | ? | ? | ? | ? | ? | ? | ? | ? |
| Complete WOs | ? | ? | ? | ? | ? | ? | ? | ? |
| Reject WOs | ? | ? | ? | ? | ? | ? | ? | ? |
| Cancel WOs | ? | ? | ? | ? | ? | ? | ? | ? |

### Schedules & Planning

| Permission | SuperAdmin | TenantAdmin | Admin | Rel. Eng. | Planner | Supervisor | Technician | Viewer |
|------------|:----------:|:-----------:|:-----:|:---------:|:-------:|:----------:|:----------:|:------:|
| View Schedules | ? | ? | ? | ? | ? | ? | ? | ? |
| Create Schedules | ? | ? | ? | ? | ? | ? | ? | ? |
| Edit Schedules | ? | ? | ? | ? | ? | ? | ? | ? |
| Delete Schedules | ? | ? | ? | ? | ? | ? | ? | ? |

### Analytics & Reporting

| Permission | SuperAdmin | TenantAdmin | Admin | Rel. Eng. | Planner | Supervisor | Technician | Viewer |
|------------|:----------:|:-----------:|:-----:|:---------:|:-------:|:----------:|:----------:|:------:|
| View Dashboard | ? | ? | ? | ? | ? | ? | ? | ? |
| Access Analytics | ? | ? | ? | ? | ? | ? | ? | ? |
| Reliability Analysis | ? | ? | ? | ? | ? | ? | ? | ? |
| FMEA Management | ? | ? | ? | ? | ? | ? | ? | ? |
| Export Data | ? | ? | ? | ? | ? | ? | ? | ? |
| Import Data | ? | ? | ? | ? | ? | ? | ? | ? |

### Spare Parts & Inventory

| Permission | SuperAdmin | TenantAdmin | Admin | Rel. Eng. | Planner | Supervisor | Technician | Viewer |
|------------|:----------:|:-----------:|:-----:|:---------:|:-------:|:----------:|:----------:|:------:|
| View Parts | ? | ? | ? | ? | ? | ? | ? | ? |
| Manage Parts | ? | ? | ? | ? | ? | ? | ? | ? |
| Issue Parts | ? | ? | ? | ? | ? | ? | ? | ? |
| Adjust Inventory | ? | ? | ? | ? | ? | ? | ? | ? |

### System Settings

| Permission | SuperAdmin | TenantAdmin | Admin | Rel. Eng. | Planner | Supervisor | Technician | Viewer |
|------------|:----------:|:-----------:|:-----:|:---------:|:-------:|:----------:|:----------:|:------:|
| Access Settings | ? | ? | ? | ? | ? | ? | ? | ? |
| Modify Settings | ? | ? | ? | ? | ? | ? | ? | ? |
| Manage Tenants | ? | ? | ? | ? | ? | ? | ? | ? |
| Manage Notifications | ? | ? | ? | ? | ? | ? | ? | ? |
| Manage WhatsApp | ? | ? | ? | ? | ? | ? | ? | ? |

## Using Permissions in Components

### Razor Page Authorization

```razor
@attribute [Authorize(Roles = "SuperAdmin,TenantAdmin,Admin")]
```

### Checking Permissions in Code

```razor
@inject RolePermissionService PermissionService

@code {
    private bool canEdit;
    private bool canDelete;

    protected override async Task OnInitializedAsync()
    {
        canEdit = await PermissionService.CanEditAsync();
        canDelete = await PermissionService.CanDeleteAsync();
    }
}
```

### Conditional UI Elements

```razor
@if (await PermissionService.CanDeleteAsync())
{
    <button @onclick="Delete">Delete</button>
}
```

### Getting Permission Summary

```csharp
var summary = await PermissionService.GetPermissionSummaryAsync();
// summary.CanView, summary.CanEdit, etc.
```

## Tenant Access

### SuperAdmin
- Can access all tenants
- Not restricted by tenant boundaries

### Other Roles
- Limited to their assigned tenant(s)
- Data automatically filtered by `DataService`

### Checking Tenant Access

```csharp
var canAccess = await PermissionService.CanAccessTenantAsync(tenantId);
var accessibleTenants = await PermissionService.GetAccessibleTenantIdsAsync();
```

## Default Users

| Email | Password | Role |
|-------|----------|------|
| superadmin@company.com | SuperAdmin123! | SuperAdmin |
| admin@company.com | Admin123! | Admin |
| sarah.johnson@company.com | Sarah123! | Reliability Engineer |
| emily.brown@company.com | Emily123! | Planner |
| david.wilson@company.com | David123! | Supervisor |
| john.smith@company.com | John123! | Technician |
| mike.davis@company.com | Mike123! | Technician |
| viewer@company.com | Viewer123! | Viewer |

## Key Service Methods

### RolePermissionService

| Method | Description |
|--------|-------------|
| `IsSuperAdminAsync()` | Check if user is SuperAdmin |
| `IsTenantAdminAsync()` | Check if user is TenantAdmin |
| `GetCurrentUserRoleAsync()` | Get user's primary role |
| `CanManageUsersAsync()` | Check user management permission |
| `CanEditAsync()` | Check general edit permission |
| `CanDeleteAsync()` | Check general delete permission |
| `CanAccessTenantAsync(int)` | Check tenant access |
| `GetAssignableRolesAsync()` | Get roles user can assign |
| `GetPermissionSummaryAsync()` | Get all permissions summary |

## Security Best Practices

1. **Always check permissions server-side** - Client-side checks are for UX only
2. **Use `[Authorize]` attributes** - Protect all sensitive pages
3. **Filter data by tenant** - `DataService` handles this automatically
4. **Log permission failures** - Track unauthorized access attempts
5. **Principle of least privilege** - Assign minimum required role

## Troubleshooting

### User Can't Access Feature
1. Check their role: `await PermissionService.GetCurrentUserRoleAsync()`
2. Verify role assignment in AspNetUserRoles table
3. Check tenant assignment if multi-tenant

### Permission Not Working
1. Ensure user is authenticated
2. Check if permission service is injected
3. Verify role name matches exactly (case-sensitive)

### Tenant Access Issues
1. Check PrimaryTenantId in AspNetUsers
2. Verify TenantUserMapping entries
3. SuperAdmin bypasses tenant restrictions
