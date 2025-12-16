# ?? Dark Mode Persistence - Troubleshooting & Testing Guide

## ?? What Was Fixed

### Issue: Theme Reverts to Light When Navigating Away
**Root Cause**: The theme initialization wasn't properly persisting across page navigation.

### Solutions Applied:

1. **ThemeService.cs** - Improved robustness
   - Added `_initialized` flag to prevent re-initialization
   - Enhanced DOM manipulation with IIFE (Immediately Invoked Function Expression)
   - Better error handling

2. **theme-manager.js** - Enhanced JavaScript
   - Wrapped in IIFE to avoid global scope pollution
   - Added console logging for debugging
   - Added cross-tab synchronization (storage events)
   - Initializes immediately on script load and on DOMContentLoaded

3. **RBMLayout.razor** - Global initialization
   - Theme loads once when layout initializes
   - Applies to all pages using RBMLayout

4. **ThemeToggle.razor** - Better state management
   - Updates button immediately after toggle
   - Properly handles theme change events

## ?? Testing Checklist

### Test 1: Initial Load (Most Important)
```
1. Close the browser completely
2. Reopen the application
3. Expected: If you had dark mode on before, it should load in dark mode
   ? PASS: Dark theme loads automatically
   ? FAIL: Reverts to light theme
```

**If FAIL, check:**
- Browser DevTools ? Application ? Storage ? Local Storage
- Look for key: `rbm-theme`
- Check if value is `dark` or `light`

### Test 2: Theme Toggle in Settings
```
1. Go to Settings & Integrations
2. Select "Dark" theme
3. Click "Apply Theme"
4. Expected: Page immediately turns dark
   ? PASS: Page turns dark instantly
   ? FAIL: Nothing happens or page doesn't change
```

### Test 3: Navigate Away and Back
```
1. With dark mode ON, go to Settings
2. Click "Apply Theme" to activate dark mode
3. Navigate to Dashboard
4. Expected: Dashboard shows dark theme
   ? PASS: Dashboard is dark
   ? FAIL: Dashboard is light
```

### Test 4: Page Navigation
```
1. With dark mode ON
2. Navigate through multiple pages:
   - Dashboard ? Assets ? Work Orders ? Documents
3. Expected: All pages stay dark
   ? PASS: All pages are dark
   ? FAIL: Some pages switch to light
```

### Test 5: Browser Refresh
```
1. Activate dark mode
2. Press F5 (refresh page)
3. Expected: Page stays dark after refresh
   ? PASS: Page is still dark
   ? FAIL: Reverts to light
```

### Test 6: Close and Reopen
```
1. Activate dark mode
2. Close the browser completely
3. Reopen and go to application
4. Expected: Dark mode loads automatically
   ? PASS: Dark mode loads
   ? FAIL: Reverts to light
```

## ?? Debugging Steps

### Enable Console Logging
1. Open DevTools (F12)
2. Go to Console tab
3. Look for messages like: "Theme applied: dark"

### Check localStorage
1. DevTools ? Application tab
2. Local Storage ? Your domain
3. Look for key `rbm-theme`
4. Value should be `dark` or `light`

### Check DOM
1. DevTools ? Inspector tab
2. Right-click on `<html>` element
3. Check if `dark-mode` class exists
4. Check if `data-theme` attribute is set

```html
<!-- Expected for dark mode -->
<html class="dark-mode" data-theme="dark">

<!-- Expected for light mode -->
<html data-theme="light">  <!-- no dark-mode class -->
```

### Check Network
1. DevTools ? Network tab
2. Look for `theme-manager.js`
3. Should show status 200 (loaded successfully)

## ?? Common Issues & Fixes

### Issue 1: Theme Always Resets to Light
**Symptom**: No matter what you do, theme keeps resetting to light

**Solutions**:
- Clear browser cache and localStorage
- Check if JavaScript is enabled
- Verify `theme-manager.js` is loaded

**Clear localStorage in DevTools Console**:
```javascript
localStorage.removeItem('rbm-theme');
location.reload();
```

### Issue 2: Dark Mode Works on Settings Only
**Symptom**: Dark mode applies on Settings page but not other pages

**Solutions**:
- Check that RBMLayout is injecting ThemeService
- Verify `OnInitializedAsync` is being called
- Check browser console for errors

### Issue 3: Theme Toggle Button Not Working
**Symptom**: Clicking the ?? button does nothing

**Solutions**:
- Check if ThemeToggle component is rendering
- Verify ThemeService is registered in DI
- Check console for JavaScript errors

### Issue 4: CSS Variables Not Applying
**Symptom**: Theme changes but colors don't update

**Solutions**:
- Check if CSS variables are defined in `rbm-styles.css`
- Verify `html.dark-mode` rules are present
- Hard refresh with Ctrl+Shift+R

## ?? Diagnostic Command (Browser Console)

Run this in DevTools Console to diagnose issues:

```javascript
console.log('=== Theme Diagnostic ===');
console.log('Current Theme:', localStorage.getItem('rbm-theme'));
console.log('Dark Mode Class:', document.documentElement.classList.contains('dark-mode'));
console.log('Data Theme Attribute:', document.documentElement.getAttribute('data-theme'));
console.log('CSS Variable (--rbm-bg):', getComputedStyle(document.documentElement).getPropertyValue('--rbm-bg'));
console.log('Theme Manager Available:', typeof window.themeManager !== 'undefined');
console.log('Trying theme-manager.applyTheme("dark")...');
if (window.themeManager) window.themeManager.applyTheme('dark');
```

## ?? Expected Behavior

### Correct Flow:
```
Application Start
    ?
theme-manager.js loads immediately
    ?
Gets theme from localStorage (if exists)
    ?
Applies 'dark-mode' class to <html>
    ?
Blazor loads
    ?
RBMLayout initializes
    ?
ThemeService.InitializeThemeAsync() called
    ?
Confirms theme from localStorage
    ?
Page renders with correct theme
    ?
All pages using RBMLayout show correct theme ?
```

### Navigation:
```
User on Dashboard (dark mode)
    ?
Navigate to Assets
    ?
RBMLayout reused (not recreated)
    ?
Theme persists ?
    ?
New page content loads in dark mode ?
```

## ? All Tests Passing Indicators

When everything works correctly, you should see:

1. ? localStorage has `rbm-theme: dark` (when dark mode is on)
2. ? `<html>` element has `class="dark-mode"`
3. ? `<html>` element has `data-theme="dark"`
4. ? Console shows "Theme applied: dark"
5. ? CSS variables are updated (check --rbm-bg, --rbm-text, etc.)
6. ? All page elements are styled correctly
7. ? Theme persists across pages
8. ? Theme persists after refresh
9. ? Theme persists after close/reopen

## ?? Support

If tests are still failing:

1. **Clear everything and start fresh**:
   ```javascript
   localStorage.clear();
   location.reload();
   ```

2. **Check browser compatibility**:
   - Chrome 49+: ?
   - Firefox 31+: ?
   - Safari 9.1+: ?
   - Edge 15+: ?

3. **Check if localStorage is enabled**:
   ```javascript
   try {
       localStorage.setItem('test', 'test');
       localStorage.removeItem('test');
       console.log('localStorage: ENABLED');
   } catch(e) {
       console.log('localStorage: DISABLED');
   }
   ```

---

**Version**: 1.0.2 (Enhanced Persistence Fix)
**Build Status**: ? Successful
**Next Step**: Run all tests and report results
