# PHASES 3-6 IMPLEMENTATION GUIDE

## Overview

After Phase 2 (SQL migration) is complete, you'll proceed with:
- Phase 3: Query Filtering (1.5 hours)
- Phase 4: Service Updates (1.5 hours)
- Phase 5: Testing (1.5 hours)
- Phase 6: Deployment (1 hour)

**Total remaining: ~5.5 hours**

---

## PHASE 3: Query Filtering in DataService.cs (1.5 hours)

### Objective
Implement automatic tenant filtering in all data service queries.

### Pattern to Follow

**Before (Current - No Filtering):**
```csharp
public async Task<List<Asset>> GetAssetsAsync()
{
    return await _context.Assets.ToListAsync();
}
```

**After (With Filtering):**
```csharp
public async Task<List<Asset>> GetAssetsAsync()
{
    var isSuperAdmin = await _rolePermissionService.IsSuperAdminAsync();
    var tenantId = await GetCurrentTenantIdAsync();

    var query = _context.Assets.AsQueryable();

    if (!isSuperAdmin)
    {
        query = query.Where(a => a.TenantId == tenantId);
    }

    return await query.ToListAsync();
}
```

### Methods to Update in DataService.cs

```csharp
// List of methods that need filtering:
- GetAssetsAsync()
- GetWorkOrdersAsync()
- GetConditionReadingsAsync()
- GetFailureModesAsync()
- GetReliabilityMetricsAsync()
- GetAssetDowntimeAsync()
- GetMaintenanceSchedulesAsync()
- GetMaintenanceTasksAsync()
- GetSparePartsAsync()
- GetSparePartTransactionsAsync()
- GetDocumentsAsync()
- GetDocumentAccessLogsAsync()
// And any others that return business data
```

### Helper Methods to Add

```csharp
private async Task<int> GetCurrentTenantIdAsync()
{
    var userId = _httpContextAccessor.HttpContext?.User
        ?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    
    if (string.IsNullOrEmpty(userId))
        throw new InvalidOperationException("User not found");

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    return user?.PrimaryTenantId ?? 0;
}

private async Task<bool> IsSuperAdminAsync()
{
    var user = _httpContextAccessor.HttpContext?.User;
    return user?.IsInRole("SuperAdmin") ?? false;
}
```

### Implementation Steps

1. **Open DataService.cs**
2. **Add IHttpContextAccessor and RolePermissionService**
   ```csharp
   private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly RolePermissionService _rolePermissionService;
   ```

3. **Add helper methods** (above)

4. **Update each method** to apply filtering

5. **Test after each update**

---

## PHASE 4: Service Updates (1.5 hours)

### Services to Update

1. **WorkOrderService.cs**
   - Implement filtering in GetWorkOrdersAsync()
   - Implement filtering in CreateWorkOrderAsync()
   - Implement filtering in UpdateWorkOrderAsync()
   - Implement filtering in DeleteWorkOrderAsync()

2. **TenantManagementService.cs**
   - Implement tenant access verification
   - Implement user tenant assignment verification
   - Add CanUserAccessTenantAsync() method

3. **Other Services**
   - Apply same filtering pattern
   - Check for any hardcoded data access
   - Implement where needed

### Pattern for Service Updates

**Before:**
```csharp
public async Task<WorkOrder?> GetWorkOrderAsync(int workOrderId)
{
    return await _context.WorkOrders
        .FirstOrDefaultAsync(wo => wo.Id == workOrderId);
}
```

**After:**
```csharp
public async Task<WorkOrder?> GetWorkOrderAsync(int workOrderId)
{
    var isSuperAdmin = await _rolePermissionService.IsSuperAdminAsync();
    var tenantId = await GetCurrentTenantIdAsync();

    var query = _context.WorkOrders.AsQueryable();

    if (!isSuperAdmin)
    {
        query = query.Where(wo => wo.TenantId == tenantId);
    }

    return await query.FirstOrDefaultAsync(wo => wo.Id == workOrderId);
}
```

### Implementation Steps

1. **WorkOrderService.cs** - Add filtering (30 min)
2. **TenantManagementService.cs** - Add verification (20 min)
3. **Other critical services** - Add filtering (40 min)
4. **Test all changes** - Verify data isolation (10 min)

---

## PHASE 5: Testing (1.5 hours)

### Unit Tests

**Test 1: SuperAdmin Access**
```csharp
[Test]
public async Task SuperAdmin_CanAccessAllData()
{
    // Create SuperAdmin user
    // Call GetAssetsAsync()
    // Assert: Returns all assets from all tenants
}
```

**Test 2: TenantUser Access**
```csharp
[Test]
public async Task TenantUser_CanOnlyAccessOwnTenantData()
{
    // Create TenantUser in Tenant #1
    // Call GetAssetsAsync()
    // Assert: Returns only Tenant #1 assets
}
```

**Test 3: Data Isolation**
```csharp
[Test]
public async Task CrossTenantDataNotVisible()
{
    // User in Tenant #1 tries to access Tenant #2 asset
    // Assert: Returns nothing or throws unauthorized
}
```

### Manual Tests

1. **Login as SuperAdmin**
   - Verify can see all tenants
   - Verify can see all assets
   - Verify can see all work orders
   - Verify can manage all data

2. **Login as TenantAdmin (Tenant #1)**
   - Verify can only see Tenant #1
   - Verify cannot see Tenant #2 data
   - Verify can manage Tenant #1 data

3. **Login as Technician (Tenant #2)**
   - Verify can only see Tenant #2
   - Verify cannot see Tenant #1 data
   - Verify can view but cannot delete

4. **Cross-Tenant Security**
   - Try to access other tenant's URL directly
   - Try to modify other tenant's data
   - Verify all attempts fail

### SQL Verification

```sql
-- Check data distribution by tenant
SELECT TenantId, COUNT(*) as Count FROM Assets GROUP BY TenantId
SELECT TenantId, COUNT(*) as Count FROM WorkOrders GROUP BY TenantId
SELECT TenantId, COUNT(*) as Count FROM Documents GROUP BY TenantId

-- Verify no NULL TenantId in new data
SELECT COUNT(*) FROM Assets WHERE TenantId IS NULL
SELECT COUNT(*) FROM WorkOrders WHERE TenantId IS NULL
```

---

## PHASE 6: Deployment (1 hour)

### Pre-Deployment Checklist

- [ ] All code changes complete
- [ ] All tests passing
- [ ] No compilation errors
- [ ] No runtime warnings
- [ ] Database migration tested
- [ ] Rollback procedure documented
- [ ] Team notified
- [ ] Change request approved

### Deployment Steps

1. **Backup Production Database** (10 min)
   ```sql
   BACKUP DATABASE [RBM_CMMS]
   TO DISK = 'C:\Backups\RBM_CMMS_$(DATE).bak'
   ```

2. **Deploy to Staging** (10 min)
   - Copy new build
   - Run on staging environment
   - Verify application starts

3. **Run Tests on Staging** (15 min)
   - Unit tests
   - Integration tests
   - Manual tests

4. **Deploy to Production** (10 min)
   - Copy build files
   - Update database connection if needed
   - Verify application starts

5. **Smoke Test Production** (10 min)
   - Login as SuperAdmin
   - Login as TenantAdmin
   - Verify data access correct
   - Check error logs

6. **Monitor** (5 min)
   - Watch logs for errors
   - Check performance
   - Monitor user activity

### Rollback Procedure

**If issues occur:**
1. Restore from backup
2. Revert code to previous version
3. Investigate issue
4. Fix and test thoroughly
5. Try again

---

## ?? Quick Reference

### Key Methods to Remember

**RolePermissionService:**
- `IsSuperAdminAsync()`
- `CanAccessTenantAsync(tenantId)`
- `CanManageRoleAsync(role)`

**DataService Pattern:**
```csharp
var isSuperAdmin = await _rolePermissionService.IsSuperAdminAsync();
var tenantId = await GetCurrentTenantIdAsync();
if (!isSuperAdmin) query = query.Where(x => x.TenantId == tenantId);
```

**Database:**
- All tables have TenantId column
- All have FK to Tenants table
- All have indexes for performance
- SuperAdmin sees all, others filtered

---

## ?? Success Criteria

After all 6 phases:

? SuperAdmin can access all roles and tenants  
? SuperAdmin can view/edit all data  
? TenantAdmin limited to assigned tenants  
? Technician limited to assigned tenant  
? Viewer has read-only access  
? No cross-tenant data visible  
? All queries filtered by tenant  
? Database enforces relationships  
? Tests passing (100% isolation verified)  
? Production deployment successful  

---

## ?? Support

**If you get stuck:**
1. Check SUPERADMIN_IMPLEMENTATION_ACTION_PLAN.md for detailed steps
2. Reference MULTI_TENANCY_COMPLETE_ENFORCEMENT.md for patterns
3. Check code examples in documentation
4. Review working code in Phase 1-2

---

## Timeline Summary

```
Phase 1: 30 min ?
Phase 2: 5 min ? (SQL execution)
Phase 3: 1.5 hours ?
Phase 4: 1.5 hours ?
Phase 5: 1.5 hours ?
Phase 6: 1 hour ?
?????????????????????
TOTAL: 6 hours (Phase 1 done, 5.5 remaining)
```

---

**You're doing great!** Phase 1 is complete, Phase 2 is ready to execute. ??

After Phase 2, you'll have SQL infrastructure in place and can begin Phase 3 for application-level filtering.
