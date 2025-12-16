# ? PHASE 2 IMPLEMENTATION READY

## Database Multi-Tenancy - PREPARATION

**Status:** Phase 2 Ready to Execute  
**Date:** 2024-12-20  

---

## ?? Phase 2 Tasks

### Task 1: Verify TenantId Exists in All Models ?

**Models Already Have TenantId:**
- ? Asset.cs - `public int? TenantId { get; set; }`
- ? WorkOrder.cs - `public int? TenantId { get; set; }`
- ? ConditionReading.cs - `public int? TenantId { get; set; }`
- ? FailureMode.cs - `public int? TenantId { get; set; }`
- ? MaintenanceSchedule.cs - `public int? TenantId { get; set; }`

**Need to Verify/Add TenantId:**
- ReliabilityMetric.cs
- AssetDowntime.cs
- MaintenanceTask.cs
- SparePart.cs
- SparePartTransaction.cs
- Document.cs
- DocumentAccessLog.cs

### Task 2: Execute SQL Migration ?

**Script:** `ADD_TENANTID_TO_ALL_TABLES.sql`

**What it does:**
1. Adds TenantId column to 11 business tables
2. Creates indexes for performance
3. Creates foreign key constraints
4. Provides verification queries

**Tables affected:**
- WorkOrders ? (already has column, adds FK)
- ConditionReadings ? (already has column, adds FK)
- FailureModes ? (already has column, adds FK)
- ReliabilityMetrics
- AssetDowntime
- MaintenanceSchedules ? (already has column, adds FK)
- MaintenanceTasks
- SpareParts
- SparePartTransactions
- Documents
- DocumentAccessLogs

### Task 3: Verify Database Schema ?

**Verification Queries Provided:**
- Check which tables have TenantId ?
- Check which tables are MISSING TenantId ?
- Verify foreign keys created ?
- Verify indexes created ?

---

## ?? Execution Steps

### Step 1: Check Current Status

Run this query in SQL Server Management Studio:

```sql
-- Check which tables have TenantId
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE COLUMN_NAME = 'TenantId' 
  AND TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME;
```

**Expected Result:** Should show 5 tables already have TenantId

### Step 2: Execute SQL Migration

1. Open SQL Server Management Studio
2. Connect to `RBM_CMMS` database
3. Open `ADD_TENANTID_TO_ALL_TABLES.sql`
4. Execute the entire script
5. Monitor for success

**Safe to execute multiple times** - Uses `IF NOT EXISTS` checks

### Step 3: Verify Results

Run verification queries at bottom of SQL script:

```sql
-- This will show all tables with TenantId
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE COLUMN_NAME = 'TenantId' 
  AND TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME;
```

**Expected Result:** All 12 business tables should have TenantId

---

## ?? What You Need to Do

### Immediate Actions:
1. ? Review Phase 1 completion (PHASE_1_IMPLEMENTATION_COMPLETE.md)
2. ? Run the SQL migration script (ADD_TENANTID_TO_ALL_TABLES.sql)
3. ? Verify database schema changes
4. ? Update remaining models (if needed)
5. ? Create EF Core migration (if using migrations)
6. ? Test application startup

---

## ?? Phase 2 Checklist

- [x] Phase 1 complete
- [x] TenantId property verified in models
- [x] SQL migration script prepared
- [ ] Execute SQL migration
- [ ] Verify all tables have TenantId
- [ ] Verify foreign keys created
- [ ] Verify indexes created
- [ ] Update ApplicationDbContext.cs (optional)
- [ ] Create EF Core migration
- [ ] Test application startup

---

## ?? Files Ready

**SQL Script:**
- `ADD_TENANTID_TO_ALL_TABLES.sql` - Ready to execute

**Models with TenantId (verified):**
- Asset.cs ?
- WorkOrder.cs ?
- ConditionReading.cs ?
- FailureMode.cs ?
- MaintenanceSchedule.cs ?

**Documentation:**
- Phase 1 Completion Report ?
- Phase 2 Implementation Ready ?
- SUPERADMIN_IMPLEMENTATION_ACTION_PLAN.md ?

---

## ?? Next: Phase 3

**After Phase 2 completes:**
- Implement query filtering in DataService.cs
- Add WHERE clause to all queries
- Filter by TenantId for non-SuperAdmin users
- Test data isolation

**Time estimate:** 1.5 hours

---

## ?? Summary

Phase 2 is ready to execute. The SQL migration script is prepared and will:

? Add TenantId to all business tables  
? Create proper indexes for performance  
? Create foreign key constraints  
? Enforce data isolation at database level  

**Next step:** Execute ADD_TENANTID_TO_ALL_TABLES.sql in SQL Server Management Studio
