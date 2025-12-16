# Multi-Tenancy Complete Enforcement Guide ??

## Status: Implementation Plan for Full Data Isolation

**Objective:** Ensure SuperAdmin has access to all roles, every database table belongs to a tenant, and complete data isolation is enforced.

---

## ?? Phase 1: SuperAdmin Access & Role Management

### 1.1 SuperAdmin Role Enhancements

**Current Issue:** SuperAdmin should access all tenants and all roles with full permissions.

**Solution Implementation:**

```csharp
// In RolePermissionService.cs
public class RolePermissionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantManagementService _tenantService;

    public async Task<bool> IsSuperAdminAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.IsInRole("SuperAdmin") ?? false;
    }

    public async Task<bool> CanAccessTenantAsync(int tenantId)
    {
        // SuperAdmin can access all tenants
        if (await IsSuperAdminAsync())
            return true;

        // Regular users can only access their assigned tenants
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        // Check if user has mapping to this tenant
        return user.TenantMappings.Any(m => m.TenantId == tenantId && m.RemovedDate == null);
    }

    public async Task<List<int>> GetAccessibleTenantsAsync()
    {
        // SuperAdmin gets all tenants
        if (await IsSuperAdminAsync())
        {
            var allTenants = await _dbContext.Tenants
                .Where(t => t.IsActive)
                .Select(t => t.Id)
                .ToListAsync();
            return allTenants;
        }

        // Regular users get their assigned tenants
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userTenants = await _dbContext.UserTenantMappings
            .Where(m => m.UserId == userId && m.RemovedDate == null)
            .Select(m => m.TenantId)
            .ToListAsync();
        return userTenants;
    }

    public async Task<bool> CanAccessRoleAsync(string role)
    {
        // SuperAdmin can access all roles
        if (await IsSuperAdminAsync())
            return true;

        // TenantAdmin can access TenantAdmin, Technician, Viewer roles
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.IsInRole("TenantAdmin") ?? false)
            return role is "TenantAdmin" or "Technician" or "Viewer";

        // Technician can only access Technician role
        if (user?.IsInRole("Technician") ?? false)
            return role == "Technician";

        return false;
    }
}
```

---

## ??? Phase 2: Complete Database Multi-Tenancy

### 2.1 Required TenantId Fields

Every business table must have `TenantId`:

```csharp
// List of tables that MUST have TenantId
1. ? Assets (Already has TenantId)
2. ? WorkOrders (Add TenantId)
3. ? ConditionReadings (Add TenantId)
4. ? FailureModes (Add TenantId)
5. ? ReliabilityMetrics (Add TenantId)
6. ? AssetDowntime (Add TenantId)
7. ? MaintenanceSchedules (Add TenantId)
8. ? MaintenanceTasks (Add TenantId)
9. ? SpareParts (Add TenantId)
10. ? SparePartTransactions (Add TenantId)
11. ? Documents (Add TenantId)
12. ? DocumentAccessLogs (Add TenantId)
```

### 2.2 Migration Strategy

Create a comprehensive migration:

```csharp
public partial class AddTenantIdToAllTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add TenantId columns to all tables
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

        // Create foreign keys
        migrationBuilder.CreateIndex(
            name: "IX_WorkOrders_TenantId",
            table: "WorkOrders",
            column: "TenantId");

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
        // Remove foreign keys and columns
        migrationBuilder.DropForeignKey(
            name: "FK_WorkOrders_Tenants_TenantId",
            table: "WorkOrders");

        migrationBuilder.DropIndex(
            name: "IX_WorkOrders_TenantId",
            table: "WorkOrders");

        migrationBuilder.DropColumn(
            name: "TenantId",
            table: "WorkOrders");

        // ... repeat for all other tables ...
    }
}
```

---

## ?? Phase 3: Query Filtering

### 3.1 Global Query Filter Implementation

```csharp
// In ApplicationDbContext.cs OnModelCreating()

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Get the current tenant context
    var tenantId = GetCurrentTenantId();

    // Apply tenant filters to all tables
    modelBuilder.Entity<Asset>()
        .HasQueryFilter(a => a.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<WorkOrder>()
        .HasQueryFilter(w => w.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<ConditionReading>()
        .HasQueryFilter(c => c.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<FailureMode>()
        .HasQueryFilter(f => f.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<ReliabilityMetric>()
        .HasQueryFilter(r => r.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<AssetDowntime>()
        .HasQueryFilter(a => a.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<MaintenanceSchedule>()
        .HasQueryFilter(m => m.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<MaintenanceTask>()
        .HasQueryFilter(m => m.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<SparePart>()
        .HasQueryFilter(s => s.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<SparePartTransaction>()
        .HasQueryFilter(s => s.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<Document>()
        .HasQueryFilter(d => d.TenantId == tenantId || IsSuperAdmin());

    modelBuilder.Entity<DocumentAccessLog>()
        .HasQueryFilter(d => d.TenantId == tenantId || IsSuperAdmin());
}

private int GetCurrentTenantId()
{
    // Implementation to get current tenant from context
    // Will be resolved at query time
    return -1; // Placeholder
}

private bool IsSuperAdmin()
{
    // Check if current user is SuperAdmin
    return false; // Placeholder
}
```

### 3.2 Service Layer Filtering

```csharp
// In DataService.cs

public class DataService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantManagementService _tenantService;

    public async Task<List<Asset>> GetAssetsAsync()
    {
        var tenantId = await GetCurrentTenantIdAsync();
        var isSuperAdmin = await IsSuperAdminAsync();

        var query = _context.Assets.AsQueryable();

        // Apply tenant filter
        if (!isSuperAdmin)
        {
            query = query.Where(a => a.TenantId == tenantId);
        }

        return await query.ToListAsync();
    }

    public async Task<List<WorkOrder>> GetWorkOrdersAsync()
    {
        var tenantId = await GetCurrentTenantIdAsync();
        var isSuperAdmin = await IsSuperAdminAsync();

        var query = _context.WorkOrders.AsQueryable();

        // Apply tenant filter
        if (!isSuperAdmin)
        {
            query = query.Where(w => w.TenantId == tenantId);
        }

        return await query.ToListAsync();
    }

    private async Task<int> GetCurrentTenantIdAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User
            ?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
            throw new InvalidOperationException("User not found in context");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user?.PrimaryTenantId ?? 0;
    }

    private async Task<bool> IsSuperAdminAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.IsInRole("SuperAdmin") ?? false;
    }
}
```

---

## ?? Phase 4: Tenants.razor Enhancement

### 4.1 Updated Tenants Component

```razor
@* Add SuperAdmin access control *@
@attribute [Authorize(Roles = "SuperAdmin")]

@code {
    // Ensure only SuperAdmin can see all tenants
    protected override async Task OnInitializedAsync()
    {
        var isSuperAdmin = await AuthorizationService.AuthorizeAsync(User, "SuperAdminOnly");
        if (!isSuperAdmin.Succeeded)
        {
            NavigationManager.NavigateTo("/access-denied");
            return;
        }

        await LoadTenants();
    }

    // SuperAdmin can edit ALL tenant properties
    private async Task SaveTenant()
    {
        // Validation for SuperAdmin operations
        if (editingTenant?.Id == 0)
        {
            // Create new tenant (SuperAdmin only)
            await ValidateAndCreateTenant();
        }
        else
        {
            // Update tenant (SuperAdmin only)
            await ValidateAndUpdateTenant();
        }
    }

    // SuperAdmin can assign roles to users across all tenants
    private async Task AssignRolesToUser(int tenantId, string userId, List<string> roles)
    {
        // SuperAdmin can assign: SuperAdmin, TenantAdmin, Technician, Viewer
        var validRoles = new[] { "SuperAdmin", "TenantAdmin", "Technician", "Viewer" };
        
        foreach (var role in roles)
        {
            if (!validRoles.Contains(role))
            {
                errorMessage = $"Invalid role: {role}";
                return;
            }

            // Assign role
            await RolePermissionService.AssignRoleAsync(userId, role, tenantId);
        }

        successMessage = "Roles assigned successfully!";
        await LoadTenants();
    }
}
```

---

## ?? Phase 5: Data Isolation Enforcement

### 5.1 Create Data Isolation Middleware

```csharp
// Create new file: Middleware/TenantIsolationMiddleware.cs

public class TenantIsolationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantIsolationMiddleware> _logger;

    public TenantIsolationMiddleware(RequestDelegate next, ILogger<TenantIsolationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        var user = context.User;

        if (user?.Identity?.IsAuthenticated ?? false)
        {
            // Set tenant context for the request
            var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isSuperAdmin = user.IsInRole("SuperAdmin");

            // Store tenant info in context items
            context.Items["IsSuperAdmin"] = isSuperAdmin;
            context.Items["UserId"] = userId;

            _logger.LogInformation($"Request from user {userId}, SuperAdmin: {isSuperAdmin}");
        }

        await _next(context);
    }
}
```

### 5.2 Register Middleware in Program.cs

```csharp
// In Program.cs
builder.Services.AddScoped<TenantIsolationMiddleware>();

var app = builder.Build();

// Add middleware
app.UseMiddleware<TenantIsolationMiddleware>();
```

---

## ?? Phase 6: Authorization Policies

### 6.1 Create Custom Authorization Policies

```csharp
// In Program.cs or separate file
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireRole("SuperAdmin"));

    options.AddPolicy("TenantAdminOrHigher", policy =>
        policy.RequireRole("SuperAdmin", "TenantAdmin"));

    options.AddPolicy("CanEditData", policy =>
        policy.RequireRole("SuperAdmin", "TenantAdmin", "Technician"));

    options.AddPolicy("CanViewData", policy =>
        policy.RequireRole("SuperAdmin", "TenantAdmin", "Technician", "Viewer"));

    options.AddPolicy("CanManageTenants", policy =>
        policy.RequireRole("SuperAdmin"));

    options.AddPolicy("CanManageUsers", policy =>
        policy.RequireRole("SuperAdmin", "TenantAdmin"));
});
```

---

## ? Implementation Checklist

### Phase 1: SuperAdmin Access
- [ ] Update RolePermissionService with SuperAdmin checks
- [ ] Update TenantManagementService with SuperAdmin access
- [ ] Test SuperAdmin can access all tenants
- [ ] Test TenantAdmin access is limited to assigned tenants

### Phase 2: Database Multi-Tenancy
- [ ] Create migration to add TenantId to all tables
- [ ] Update all model classes with TenantId property
- [ ] Create proper foreign keys and indexes
- [ ] Update ApplicationDbContext relationships
- [ ] Run migration and verify database structure

### Phase 3: Query Filtering
- [ ] Implement global query filters
- [ ] Update DataService with tenant filtering
- [ ] Update all service methods with tenant context
- [ ] Test query isolation

### Phase 4: Tenants Component
- [ ] Add SuperAdmin-only verification
- [ ] Update Tenants.razor with role management
- [ ] Add role assignment functionality
- [ ] Test role assignment

### Phase 5: Data Isolation
- [ ] Create TenantIsolationMiddleware
- [ ] Register middleware in Program.cs
- [ ] Test data isolation across requests
- [ ] Verify no cross-tenant data leakage

### Phase 6: Authorization
- [ ] Define all authorization policies
- [ ] Apply policies to components and services
- [ ] Test policy enforcement
- [ ] Verify role-based access control

---

## ?? Testing Strategy

### Unit Tests
```csharp
[TestFixture]
public class MultiTenancyTests
{
    [Test]
    public async Task SuperAdmin_CanAccessAllTenants()
    {
        // Arrange
        var superAdminUser = CreateSuperAdminUser();
        var tenants = await _context.Tenants.ToListAsync();

        // Act
        var result = await _rolePermissionService.GetAccessibleTenantsAsync();

        // Assert
        Assert.AreEqual(tenants.Count, result.Count);
    }

    [Test]
    public async Task TenantUser_CanOnlyAccessOwnTenant()
    {
        // Arrange
        var tenantUser = CreateTenantUser();
        var assignedTenantId = 1;

        // Act
        var result = await _rolePermissionService.GetAccessibleTenantsAsync();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(assignedTenantId, result[0]);
    }

    [Test]
    public async Task DataService_FiltersAssetsByTenant()
    {
        // Arrange
        var tenantId = 1;
        var assets = CreateTestAssets(tenantId);

        // Act
        var result = await _dataService.GetAssetsAsync();

        // Assert
        Assert.True(result.All(a => a.TenantId == tenantId));
    }
}
```

---

## ?? Deployment Checklist

- [ ] Backup database before migration
- [ ] Test migration on development database
- [ ] Review all model changes
- [ ] Verify all foreign keys are correct
- [ ] Test all services with new TenantId filtering
- [ ] Run full integration tests
- [ ] Test SuperAdmin access to all tenants
- [ ] Test TenantAdmin access limitations
- [ ] Verify no data leakage between tenants
- [ ] Deploy to staging
- [ ] Run UAT with test data
- [ ] Deploy to production
- [ ] Monitor for errors

---

## ?? Success Criteria

? **SuperAdmin Access**
- SuperAdmin can access all tenants
- SuperAdmin can assign all roles
- SuperAdmin can view/edit all data

? **Data Isolation**
- No cross-tenant data visible
- All queries filtered by tenant
- Foreign keys enforce tenant relationships

? **Role-Based Access**
- TenantAdmin can only see assigned tenants
- Technician limited to assigned tenant
- Viewer has read-only access

? **Database Integrity**
- All tables have TenantId
- Foreign keys properly configured
- Indexes for performance
- No orphaned data

---

**This comprehensive plan ensures complete multi-tenancy with SuperAdmin access to all roles and full data isolation!** ??
