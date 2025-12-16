# ?? PHASE 5 TESTING IMPLEMENTATION GUIDE

## Overview

**Phase 5** implements comprehensive testing for the multi-tenancy system to ensure:
- ? Data isolation between tenants
- ? SuperAdmin access works correctly
- ? TenantAdmin has proper restrictions
- ? Role-based access control
- ? Exception handling for unauthorized access

---

## TESTING STRATEGY

### Test Categories

1. **Unit Tests** - Test individual methods
2. **Integration Tests** - Test interactions between services
3. **Security Tests** - Test access control
4. **SQL Verification** - Test database constraints

---

## TEST PLAN

### Test Suite 1: DataService Tenant Filtering

**Test Cases:**
```
1. SuperAdmin can access all assets
2. TenantUser sees only own tenant assets
3. Cross-tenant asset access throws exception
4. GetAssetsAsync filters by tenant
5. GetAssetByIdAsync verifies access
6. CreateAssetAsync assigns correct tenant
```

### Test Suite 2: WorkOrderService Tenant Filtering

**Test Cases:**
```
1. SuperAdmin views all work orders
2. TenantUser views own tenant only
3. Cross-tenant WO access throws exception
4. GetWorkOrderAsync verifies access
5. CreateWorkOrderAsync assigns tenant
6. ApproveWorkOrderAsync requires tenant access
7. Statistics are tenant-specific
```

### Test Suite 3: TenantManagementService Access Control

**Test Cases:**
```
1. Only SuperAdmin can create tenant
2. Only SuperAdmin can update tenant
3. Only SuperAdmin can delete tenant
4. SuperAdmin can view all tenants
5. TenantAdmin cannot create tenant
6. TenantAdmin can add users to own tenant
7. TenantAdmin cannot add to other tenants
```

### Test Suite 4: Role-Based Access

**Test Cases:**
```
1. SuperAdmin has full access
2. TenantAdmin has tenant-level access
3. Technician has limited access
4. Viewer has read-only access
5. Unauthorized operations throw exception
```

### Test Suite 5: SQL-Level Verification

**Test Cases:**
```
1. TenantId is NOT NULL on all business tables
2. Foreign keys are properly set
3. Performance indexes exist
4. Data isolation at database level
```

---

## UNIT TEST EXAMPLES

### DataService Tests

```csharp
[TestClass]
public class DataServiceTenantFilteringTests
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly DataService _dataService;
    private readonly RolePermissionService _rolePermissionService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    [TestInitialize]
    public void Setup()
    {
        // Initialize services and mocks
    }

    [TestMethod]
    public async Task GetAssetsAsync_SuperAdmin_ReturnsAllAssets()
    {
        // Arrange
        // Mock: IsSuperAdminAsync returns true

        // Act
        var assets = await _dataService.GetAssetsAsync();

        // Assert
        Assert.IsTrue(assets.Count > 0);
        // Should include assets from all tenants
    }

    [TestMethod]
    public async Task GetAssetsAsync_TenantUser_ReturnsOwnTenantOnly()
    {
        // Arrange
        // Mock: IsSuperAdminAsync returns false
        // Mock: GetCurrentTenantIdAsync returns tenant 1

        // Act
        var assets = await _dataService.GetAssetsAsync();

        // Assert
        Assert.IsTrue(assets.All(a => a.TenantId == 1));
    }

    [TestMethod]
    public async Task GetAssetAsync_CrossTenantAccess_ThrowsException()
    {
        // Arrange
        var assetId = 1; // From tenant 2
        // Mock: GetCurrentTenantIdAsync returns tenant 1

        // Act & Assert
        await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            async () => await _dataService.GetAssetAsync(assetId)
        );
    }
}
```

### WorkOrderService Tests

```csharp
[TestClass]
public class WorkOrderServiceTenantFilteringTests
{
    [TestMethod]
    public async Task GetAllWorkOrdersAsync_SuperAdmin_ReturnsAll()
    {
        // Arrange
        // Mock: IsSuperAdminAsync returns true

        // Act
        var orders = await _workOrderService.GetAllWorkOrdersAsync();

        // Assert
        Assert.IsTrue(orders.Count > 0);
    }

    [TestMethod]
    public async Task GetWorkOrderAsync_CrossTenantAccess_ThrowsException()
    {
        // Arrange
        var workOrderId = 1; // From tenant 2
        // Mock: GetCurrentTenantIdAsync returns tenant 1

        // Act & Assert
        await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            async () => await _workOrderService.GetWorkOrderAsync(workOrderId)
        );
    }

    [TestMethod]
    public async Task GetStatisticsAsync_TenantUser_ReturnsTenantStats()
    {
        // Arrange
        // Mock: GetCurrentTenantIdAsync returns tenant 1
        // Create work orders in tenant 1 and tenant 2

        // Act
        var stats = await _workOrderService.GetStatisticsAsync();

        // Assert
        Assert.AreEqual(X, stats.Total); // Only tenant 1 work orders
    }
}
```

### TenantManagementService Tests

```csharp
[TestClass]
public class TenantManagementServiceAccessControlTests
{
    [TestMethod]
    public async Task CreateTenantAsync_NonSuperAdmin_ThrowsException()
    {
        // Arrange
        // Mock: IsSuperAdminAsync returns false

        // Act & Assert
        await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            async () => await _tenantService.CreateTenantAsync("CODE", "Name", "user")
        );
    }

    [TestMethod]
    public async Task CreateTenantAsync_SuperAdmin_Succeeds()
    {
        // Arrange
        // Mock: IsSuperAdminAsync returns true

        // Act
        var tenant = await _tenantService.CreateTenantAsync("CODE", "Name", "admin");

        // Assert
        Assert.IsNotNull(tenant);
        Assert.AreEqual("CODE", tenant.TenantCode);
    }

    [TestMethod]
    public async Task AddUserToTenantAsync_TenantAdmin_OwnTenant_Succeeds()
    {
        // Arrange
        var tenantId = 1;
        // Mock: GetCurrentTenantIdAsync returns tenant 1

        // Act
        var result = await _tenantService.AddUserToTenantAsync(tenantId, "userId", false);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task AddUserToTenantAsync_TenantAdmin_OtherTenant_ThrowsException()
    {
        // Arrange
        var tenantId = 2; // Other tenant
        // Mock: GetCurrentTenantIdAsync returns tenant 1

        // Act & Assert
        await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            async () => await _tenantService.AddUserToTenantAsync(tenantId, "userId", false)
        );
    }
}
```

---

## SQL VERIFICATION QUERIES

### Verify TenantId Columns

```sql
-- Check TenantId is NOT NULL on all business tables
SELECT TABLE_NAME, COLUMN_NAME, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN (
    'Assets', 'WorkOrders', 'ConditionReadings', 'FailureModes',
    'MaintenanceSchedules', 'Documents', 'SpareParts', 'DowntimeRecords'
)
AND COLUMN_NAME = 'TenantId'
ORDER BY TABLE_NAME;

-- Result should show IS_NULLABLE = 'NO' for all
```

### Verify Foreign Key Constraints

```sql
-- Check foreign key constraints exist
SELECT CONSTRAINT_NAME, TABLE_NAME, COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE REFERENCED_TABLE_NAME = 'Tenants'
AND COLUMN_NAME = 'TenantId'
ORDER BY TABLE_NAME;
```

### Verify Data Isolation

```sql
-- Verify no assets are shared between tenants
SELECT TenantId, COUNT(*) as AssetCount
FROM Assets
GROUP BY TenantId
ORDER BY TenantId;

-- Each TenantId should have distinct asset sets
```

### Performance Index Verification

```sql
-- Check indexes for TenantId columns
SELECT TABLE_NAME, INDEX_NAME, COLUMN_NAME
FROM INFORMATION_SCHEMA.STATISTICS
WHERE COLUMN_NAME = 'TenantId'
ORDER BY TABLE_NAME;
```

---

## MANUAL TESTING SCENARIOS

### Scenario 1: SuperAdmin Access

**Steps:**
1. Login as SuperAdmin user
2. Navigate to Assets page
3. Verify seeing assets from multiple tenants
4. Navigate to Tenants page
5. Verify seeing all tenants

**Expected Result:** ? SuperAdmin sees all data

---

### Scenario 2: TenantUser Access

**Steps:**
1. Login as TenantUser (Tenant 1)
2. Navigate to Assets page
3. Verify seeing only Tenant 1 assets
4. Try accessing Tenant 2 asset directly (URL manipulation)
5. Verify getting "Access Denied" or similar

**Expected Result:** ? TenantUser sees only own tenant data

---

### Scenario 3: Cross-Tenant Prevention

**Steps:**
1. Get WorkOrder ID from Tenant 2
2. Login as TenantUser (Tenant 1)
3. Try to approve/edit that WorkOrder
4. Verify UnauthorizedAccessException

**Expected Result:** ? Cross-tenant access prevented

---

### Scenario 4: TenantAdmin User Management

**Steps:**
1. Login as TenantAdmin (Tenant 1)
2. Go to Tenant Users page
3. Add user to Tenant 1 ?
4. Try adding user to Tenant 2 (if visible)
5. Verify access denied

**Expected Result:** ? TenantAdmin can only manage own tenant

---

## TESTING CHECKLIST

### Phase 5 Testing Tasks

- [ ] Create unit test project (if not exists)
- [ ] Write DataService tenant filtering tests
- [ ] Write WorkOrderService tenant filtering tests
- [ ] Write TenantManagementService access control tests
- [ ] Write role-based access tests
- [ ] Write SQL verification tests
- [ ] Run all unit tests
- [ ] Run all integration tests
- [ ] Perform manual testing scenarios
- [ ] Verify all exceptions are thrown properly
- [ ] Check no data leaks between tenants
- [ ] Verify SuperAdmin bypass works
- [ ] Test with multiple concurrent tenants
- [ ] Document test results

---

## EXPECTED TEST OUTCOMES

### Unit Tests
? All DataService methods filter by tenant  
? All WorkOrderService methods check access  
? All TenantManagementService operations verify permission  
? All unauthorized operations throw exception  

### Integration Tests
? Multi-tenant scenarios work correctly  
? Role-based operations work correctly  
? Statistics are tenant-specific  
? Cross-tenant prevention works  

### Security Tests
? SuperAdmin has full access  
? TenantAdmin has tenant-level access  
? TenantUser cannot access other tenants  
? No data leaks between tenants  

### SQL Verification
? TenantId present on all tables  
? Foreign key constraints exist  
? Performance indexes present  
? Data properly isolated at DB level  

---

## TIME ESTIMATE

| Task | Time |
|------|------|
| Setup test project | 15 min |
| Write unit tests | 30 min |
| Write integration tests | 25 min |
| Manual testing | 20 min |
| Bug fixes (if needed) | 15 min |
| Documentation | 10 min |
| **TOTAL** | **1.5 hours** |

---

## SUCCESS CRITERIA

? All unit tests pass  
? All integration tests pass  
? No cross-tenant data access  
? All unauthorized operations throw exception  
? SuperAdmin bypass works correctly  
? TenantAdmin restrictions work correctly  
? Zero security vulnerabilities  
? Zero data leaks between tenants  

---

## NEXT STEPS AFTER PHASE 5

Once testing is complete:
1. Review and fix any failures
2. Document test results
3. Proceed to Phase 6: Deployment

---

**Phase 5 is critical for ensuring production readiness!** ??
