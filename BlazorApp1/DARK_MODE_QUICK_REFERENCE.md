# Dark Mode - Quick Reference

## ?? Quick Start

### Toggle Dark Mode
1. Click the ?? icon in the topbar (any page)
2. Page instantly switches to dark mode

### Via Settings
1. Go to **Settings & Integrations** (?? icon)
2. Scroll to **Appearance** section
3. Select "?? Dark (Modern)" from dropdown
4. Click "Apply Theme"

## ?? Key Files

| File | Purpose |
|------|---------|
| `Services/ThemeService.cs` | Theme management logic |
| `Components/Shared/ThemeToggle.razor` | Quick toggle button |
| `wwwroot/css/rbm-styles.css` | Dark mode CSS variables |
| `wwwroot/js/theme-manager.js` | Early initialization |

## ?? Available Themes

- **Light**: ?? Industrial Blue-Grey (Default)
- **Dark**: ?? Modern Black & Blue

## ?? For Developers

### Inject ThemeService
```csharp
@inject ThemeService ThemeService

// In code section
var currentTheme = ThemeService.GetCurrentTheme();
await ThemeService.SetThemeAsync("dark");
await ThemeService.ToggleThemeAsync();
```

### CSS Variables
```css
/* All colors use CSS variables */
background: var(--rbm-bg);
color: var(--rbm-text);
border-color: var(--rbm-border);
```

### Add Theme Toggle to Component
```razor
<ThemeToggle />
```

## ?? Platform Support

- ? Desktop (all modern browsers)
- ? Mobile & Tablet
- ? Browser dark mode detection (coming soon)
- ? Cross-device sync (via localStorage)

## ?? How It Works

1. **Initialization**: `theme-manager.js` loads saved theme
2. **Switching**: Click toggle ? `ThemeService` updates class
3. **Storage**: Preference saved to localStorage
4. **Styling**: CSS variables automatically update colors

## ? FAQ

**Q: Where is my theme preference saved?**
A: Browser localStorage under key `rbm-theme`

**Q: Can I create custom themes?**
A: Currently no, but Light and Dark are fully customizable via CSS

**Q: Does theme change across devices?**
A: No, it's stored locally per device/browser

**Q: Can I detect user's system dark mode?**
A: Not yet, but this feature is planned

## ?? Common Issues

| Issue | Solution |
|-------|----------|
| Theme not saving | Clear localStorage & refresh |
| Colors look off | Hard refresh (Ctrl+F5) |
| Theme doesn't apply | Check browser console for errors |

## ? Features

- ? Instant switching (no page reload)
- ?? Persistent (survives browser restart)
- ?? Smooth transitions (0.3s)
- ?? Mobile responsive
- ? Accessible color contrasts

## ?? Support

For dark mode issues:
1. Check console for JavaScript errors
2. Try clearing localStorage
3. Hard refresh the page (Ctrl+Shift+R)

---

**Status**: ? Fully Implemented & Ready to Use
