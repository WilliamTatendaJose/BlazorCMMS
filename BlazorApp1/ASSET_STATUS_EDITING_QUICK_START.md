# ?? Asset Status Editing - Quick Reference

## What Changed?

? Users can now **manually edit asset status** when editing existing assets!

---

## Status Options

| Status | Icon | When to Use |
|--------|------|-----------|
| Healthy | ? | Normal operation |
| Warning | ?? | Degradation detected |
| Critical | ?? | Urgent attention needed |
| Maintenance | ?? | Currently being serviced |
| Retired | ?? | No longer in use |

---

## How to Edit Status

### Step 1: Open Asset
```
Navigate to Assets ? Click View (???)
```

### Step 2: Click Edit
```
Asset Details ? Click Edit (??)
```

### Step 3: Change Status
```
Find "Status" field in the form
?
Click dropdown to see all options
?
Select new status
```

### Step 4: Save
```
Click "Update" button
? Status saved immediately
```

---

## Creating vs Editing

### ?? Creating New Asset
- Status auto-calculates from Health Score
- Cannot manually override
- Example: Health Score 85% ? Auto Status "Healthy"

### ?? Editing Existing Asset
- Status shows current value
- Can manually change to any option
- Manual status persists even if Health Score changes

---

## Examples

### Example 1: Asset Needs Maintenance
```
Current: Healthy (Health: 85%)
Action: Edit ? Select "Maintenance" ? Save
Result: Status shows "Maintenance" ??
```

### Example 2: Asset Recovery
```
Current: Critical (Health: 45%)
Action: Edit ? Select "Warning" ? Save
Result: Status shows "Warning" ??
```

### Example 3: Retire Old Asset
```
Current: Warning (Health: 65%)
Action: Edit ? Select "Retired" ? Save
Result: Asset marked as "Retired" ??
```

---

## Status Colors

- **Healthy** ? ?? Green
- **Warning** ? ?? Orange
- **Critical** ? ?? Red
- **Maintenance** ? ?? Blue
- **Retired** ? ? Gray

---

## Key Features

? Manual status selection when editing  
? Auto-calculation for new assets  
? 5 logical status options  
? Easy dropdown selection  
? Immediate save on update  

---

## Tips

?? Use "Maintenance" when asset is being serviced  
?? Use "Retired" for assets no longer in operation  
?? Status change doesn't affect Health Score  
?? All changes saved to database  

---

## Quick Action

| What You Need | How To Do It |
|---------------|-------------|
| Edit status | Asset Detail ? Edit (??) ? Status dropdown ? Update |
| View status | Asset List or Asset Detail page |
| Filter by status | Asset List ? Status filter dropdown |
| Revert status | Edit again and select different option |

---

**Status:** ? Ready to Use  
**Build:** ? Successful  
**Deployment:** ? Ready  
