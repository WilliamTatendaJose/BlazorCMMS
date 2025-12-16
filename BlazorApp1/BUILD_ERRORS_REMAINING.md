# BUILD ERRORS - REMAINING WORK

## ? PROGRESS SO FAR

Fixed:
- ? CurrentUserService - User.Id type mismatch
- ? WorkOrder missing properties (20+ properties added)
- ? Asset missing EquipmentManufacturer
- ? Document missing Notes property

Build errors reduced from **500+ to 260**!

---

## ?? REMAINING ISSUES

### Category 1: Read-Only Properties (Easy Fix)
These are computed properties that should NOT be assigned in DbInitializer:

1. **WorkOrder.AssetName** - ? Cannot assign (it's computed from Asset.Name)
   - Fix: Remove from DbInitializer, let it compute automatically

2. **Document.AllowedRoles** - Missing property
   - Fix: Add to Document model

### Category 2: Asset Missing Computed Properties
Asset model needs these computed/additional properties:

- `CriticalityColor` - Computed property for UI colors
- `StatusColor` - Computed property for UI colors
- `IsOverdue` - Computed property (NextMaintenance < Today)
- `DaysSinceLastMaintenance` - Computed property
- `DaysUntilMaintenance` - Computed property
- `ManufactureDate` - DateTime? property

### Category 3: WorkOrder Missing Computed Properties
WorkOrder model needs:

- `StatusColor` - Computed property
- `PriorityColor` - Computed property  
- `IsOverdue` - Computed property
- `DaysUntilDue` - Computed property
- `Location`, `Building`, `Floor` - String properties
- `Category`, `SubCategory` - String properties
- `IsMechanical`, `IsElectrical` - Bool properties
- `JobType` - String property
- `ScheduledStartDate`, `ScheduledEndDate` - DateTime? properties
- `RequiresShutdown` - Bool property

### Category 4: Type Mismatches
- `ActualDowntime` in WorkOrder is `int` but service passes `double`
  - Fix: Change to `double` OR cast in service

- `ActualCost.HasValue` - ActualCost is `decimal` not `decimal?`
  - Fix: Change to `decimal?` OR fix the query

---

## ?? RECOMMENDED FIX ORDER

### Priority 1: DbInitializer Quick Fix (1 minute)
Remove `AssetName` assignments - they're computed properties

### Priority 2: Add Missing Simple Properties (5 minutes)
Add all missing string/bool/DateTime properties to WorkOrder and Asset

### Priority 3: Add Computed Properties (10 minutes)
Add all `[NotMapped]` computed properties for colors, dates, etc.

###  Priority 4: Fix Type Mismatches (2 minutes)
Change `ActualCost` to nullable OR fix the query

---

## ?? QUICK FIXES TO APPLY NOW

### Fix 1: DbInitializer - Remove AssetName
```csharp
// Line 35-36 in DbInitializer.cs
// REMOVE: AssetName = assets[0].Name,
// It's a computed property!
```

### Fix 2: Add to Document.cs
```csharp
[StringLength(500)]
public string AllowedRoles { get; set; } = string.Empty;
```

### Fix 3: Change ActualCost to nullable
```csharp
// In WorkOrder.cs
public decimal? ActualCost { get; set; }
```

### Fix 4: Change ActualDowntime type
```csharp
// In WorkOrder.cs  
public double ActualDowntime { get; set; }
```

---

## ?? TIME ESTIMATE

- DbInitializer fixes: 1 minute
- Document AllowedRoles: 1 minute
- Type fixes (ActualCost, ActualDowntime): 1 minute
- Add missing WorkOrder properties: 5 minutes
- Add missing Asset properties: 3 minutes
- Add computed properties: 10 minutes

**Total: ~20 minutes** to fix all remaining errors

---

## ?? NEXT STEP

I'll fix these in batches to get the build working faster.

