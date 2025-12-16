# ?? Assets Module - Troubleshooting Guide

## Common Issues & Solutions

### ? Issue 1: Add Asset Button Not Responding
**Symptoms**: Button shows but clicking doesn't open modal

**Solutions**:
1. ? Clear browser cache (Ctrl+Shift+Delete)
2. ? Hard refresh page (Ctrl+F5)
3. ? Check console for errors (F12 ? Console)
4. ? Verify CurrentUser.CanEdit is true
5. ? Check if you're logged in

**Technical Check**:
- Verify `StateHasChanged()` is called in `ShowAddAssetModal()`
- Ensure `showAddModal = true` is set correctly
- Check browser console for JavaScript errors

---

### ? Issue 2: Assets Table Shows "No Assets Found" but Stats Show 6
**Symptoms**: Statistics cards show counts, but table is empty

**Solutions**:
1. ? Reload the page
2. ? Check if filters are applied (Clear Filters button visible?)
3. ? Verify database has seed data
4. ? Check if you're querying retired assets only

**Database Check**:
```sql
-- Verify assets exist in database
SELECT COUNT(*) as TotalAssets FROM Assets;
SELECT COUNT(*) as ActiveAssets FROM Assets WHERE IsRetired = 0;

-- If no results, run migration
-- If results are 0, seed data may not have run
```

**Code Check**:
- Verify `GetAssets()` returns non-empty list
- Check `LoadData()` is being called
- Ensure `filteredAssets = assets` is executed
- Verify `filteredAssets.Any()` condition in template

---

### ? Issue 3: Search/Filter Not Working
**Symptoms**: Typing search or selecting filters doesn't update table

**Solutions**:
1. ? Ensure filter has been applied (press Enter in search)
2. ? Check if filters are conflicting
3. ? Clear all filters and try again
4. ? Verify search term is valid

**Quick Fix**:
```
1. Click "? Clear Filters" button
2. Table should refresh to show all assets
3. Try filtering one field at a time
```

---

### ? Issue 4: Edit Modal Shows Empty
**Symptoms**: Edit modal opens but form fields are blank

**Solutions**:
1. ? Check if asset data loaded correctly
2. ? Verify asset object is not null
3. ? Ensure property names match exactly
4. ? Clear browser cache and reload

**Property Check**:
```csharp
// Verify these properties exist and are populated
editingAsset.AssetId
editingAsset.Name
editingAsset.Location
editingAsset.Criticality
editingAsset.HealthScore
// etc...
```

---

### ? Issue 5: Save Asset Shows Error
**Symptoms**: Clicking Create/Update shows error notification

**Error: "Asset ID and Name are required"**
- ? Fill in both Asset ID and Name fields
- ? Both fields have red * indicating required
- ? Check for extra spaces in fields

**Error: "Error: [specific error message]"**
- ? Check browser console (F12 ? Console)
- ? Look for SQL or database errors
- ? Verify all required fields are filled
- ? Check asset ID doesn't already exist

---

### ? Issue 6: Modal Won't Close
**Symptoms**: Close button or Cancel doesn't work

**Solutions**:
1. ? Press Escape key on keyboard
2. ? Click outside modal (on backdrop)
3. ? Refresh the page (Ctrl+F5)
4. ? Check console for JavaScript errors

**Code Check**:
```csharp
// Verify CloseModals method is called
private void CloseModals()
{
    showAddModal = false;
    showEditModal = false;
    StateHasChanged(); // This is critical!
}
```

---

### ? Issue 7: Statistics Show Wrong Numbers
**Symptoms**: Total/Active/Critical counts don't match table rows

**Solutions**:
1. ? Reload page to refresh statistics
2. ? Statistics exclude retired assets by default
3. ? Add a new asset to see count update
4. ? Check if assets are retired (hidden from default view)

**Verify**:
- Total count excludes retired assets
- Statistics recalculate after add/edit/delete
- Seed data has 5 active assets (not counting retired ones)

---

### ? Issue 8: Notifications Disappear Too Fast
**Symptoms**: Success message appears then vanishes immediately

**Solutions**:
1. ? This is normal behavior - messages auto-hide after 3 seconds
2. ? Check the notification appeared at all
3. ? Look in browser console to verify action worked

**Timing**:
- Success notifications: 3 second timeout
- Error notifications: 4 second timeout

---

## Database Troubleshooting

### Check if Seed Data Exists
```csharp
// Run this in your debug console
var context = /* your DbContext */;
var assetCount = context.Assets.Count();
Console.WriteLine($"Total assets in database: {assetCount}");
```

### Reset Database
```csharp
// In Program.cs or Startup
await context.Database.EnsureDeletedAsync(); // ?? CAREFUL - deletes all data
await context.Database.EnsureCreatedAsync();
await DbInitializer.SeedAsync(context);
```

### Verify Migrations Applied
```powershell
# In Package Manager Console
Get-Migration
# Should show: 20251205085714_Assets
```

---

## Performance Troubleshooting

### Page Loads Slowly
1. ? Clear browser cache
2. ? Check network tab (F12 ? Network)
3. ? Verify database connection is fast
4. ? Check if queries are efficient

### Table Takes Long to Display
1. ? Verify row count (should be ~5 assets)
2. ? Check if filtering is slow
3. ? Monitor database queries
4. ? Look for N+1 query problems

---

## Permission Troubleshooting

### Can't See Add Asset Button
- ? Verify you're logged in as admin or role with CanEdit
- ? Check CurrentUserService.CanEdit is true
- ? Verify role permissions are configured

### Can't See Edit/Delete Buttons
- ? Check CurrentUser.CanEdit permission
- ? Check CurrentUser.CanDelete permission
- ? Verify asset is not retired (Delete hidden for retired)

---

## Browser Console Debugging

### Enable Debug Logging
```javascript
// In browser console (F12)
localStorage.debug = '*';
window.location.reload();
```

### Check for Errors
```javascript
// Monitor for runtime errors
window.addEventListener('error', e => {
    console.error('Runtime error:', e);
});
```

---

## Quick Diagnostic Checklist

Run this checklist to diagnose issues:

- [ ] **Database**: Check seed data exists
  ```sql
  SELECT COUNT(*) FROM Assets;
  ```

- [ ] **Page Load**: Verify 6 total count shows
  - [ ] Page fully loads
  - [ ] Statistics card shows "6"

- [ ] **Table Display**: Verify assets show
  - [ ] Table renders
  - [ ] 6 rows visible (or filtered count)

- [ ] **Button Click**: Verify button works
  - [ ] Add Asset button visible
  - [ ] Clicking opens modal

- [ ] **Form**: Verify form works
  - [ ] Modal appears
  - [ ] Form fields present
  - [ ] Can type in fields

- [ ] **Save**: Verify save works
  - [ ] Click Create
  - [ ] Success notification shows
  - [ ] New asset in table

---

## Getting More Help

### Check the Logs
1. Browser Console (F12 ? Console)
2. Visual Studio Output Window
3. SQL Server logs
4. Application event logs

### Check Related Files
- `DataService.cs` - Business logic
- `Asset.cs` - Model definition
- `DbInitializer.cs` - Seed data
- `ApplicationDbContext.cs` - Database context

### Run Diagnostics
```csharp
// Add to component OnInitialized
Console.WriteLine($"Assets loaded: {assets.Count}");
Console.WriteLine($"Filtered assets: {filteredAssets.Count}");
Console.WriteLine($"Total stats: {totalAssets}");
```

---

## Emergency Reset

If everything is broken, try these steps:

```csharp
// 1. Clear database
await context.Database.EnsureDeletedAsync();

// 2. Recreate database
await context.Database.EnsureCreatedAsync();

// 3. Reseed data
await DbInitializer.SeedAsync(context);

// 4. Refresh browser
Ctrl+Shift+Delete (clear cache)
Ctrl+F5 (hard refresh)
```

---

## Contact Information

For persistent issues:
1. Check this troubleshooting guide first
2. Review ASSETS_BUG_FIX_SUMMARY.md
3. Review ASSETS_PRODUCTION_READY.md
4. Check browser console for error details
5. Review database logs

---

**Remember**: Most issues can be resolved by:
1. ? Clearing browser cache
2. ? Hard refreshing page (Ctrl+F5)
3. ? Restarting the application
4. ? Checking browser console (F12)
