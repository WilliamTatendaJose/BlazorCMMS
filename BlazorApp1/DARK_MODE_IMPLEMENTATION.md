# Dark Mode Implementation - Complete Guide

## ?? Overview

Dark mode has been fully implemented across the RBM CMMS application. Users can toggle between light and dark themes from the Settings page or using the theme toggle button in the topbar.

## ? Features

- **System-wide Theme Support**: All UI components automatically adapt to the selected theme
- **Persistent Storage**: Theme preference is saved to localStorage and restored on next visit
- **Smooth Transitions**: CSS transitions provide smooth color changes when switching themes
- **Quick Toggle**: Theme toggle button in the topbar for instant switching
- **Settings Page**: Dedicated theme selector in the Settings & Integrations page

## ??? Architecture

### Files Modified/Created

#### New Files
1. **`Services/ThemeService.cs`** - Core theme management service
2. **`Components/Shared/ThemeToggle.razor`** - Quick theme toggle button component
3. **`wwwroot/js/theme-manager.js`** - JavaScript theme initialization

#### Modified Files
1. **`wwwroot/css/rbm-styles.css`** - Added dark mode CSS variables and transitions
2. **`Components/App.razor`** - Added theme-manager.js script
3. **`Components/Layout/RBMLayout.razor`** - Added ThemeToggle component to topbar
4. **`Components/Pages/RBM/Settings.razor`** - Added theme selector dropdown
5. **`Program.cs`** - Registered ThemeService in DI container

## ?? How It Works

### Dark Mode CSS Variables

All colors are defined as CSS custom properties that change based on theme:

```css
:root {
    --rbm-primary: #37474f;           /* Light mode primary */
    --rbm-bg: #eceff1;                /* Light mode background */
    --rbm-card-bg: #ffffff;           /* Light mode card background */
    --rbm-text: #263238;              /* Light mode text */
}

html.dark-mode {
    --rbm-primary: #1a1a1a;           /* Dark mode primary */
    --rbm-bg: #121212;                /* Dark mode background */
    --rbm-card-bg: #1e1e1e;           /* Dark mode card background */
    --rbm-text: #e0e0e0;              /* Dark mode text */
}
```

### ThemeService

The `ThemeService` handles all theme operations:

```csharp
public class ThemeService
{
    // Initialize theme from localStorage
    public async Task InitializeThemeAsync()

    // Set theme to light or dark
    public async Task SetThemeAsync(string theme)

    // Toggle between themes
    public async Task ToggleThemeAsync()

    // Get current theme
    public string GetCurrentTheme()
}
```

### Theme Toggle Flow

1. User clicks theme toggle button or selects theme in Settings
2. `ThemeService.SetThemeAsync()` is called
3. JavaScript applies `dark-mode` class to `<html>` element
4. CSS variables automatically update all colors
5. Theme preference is saved to localStorage

## ?? Usage

### For Users

#### Quick Toggle (Topbar)
1. Click the theme icon (??/??) in the topbar
2. Page instantly updates to the new theme

#### Settings Page
1. Navigate to Settings & Integrations
2. Scroll to "Appearance" section
3. Select desired theme from dropdown
4. Click "Apply Theme"

### For Developers

#### Inject ThemeService

```csharp
@inject BlazorApp1.Services.ThemeService ThemeService

protected override async Task OnInitializedAsync()
{
    // Initialize theme on page load
    await ThemeService.InitializeThemeAsync();
    
    // Get current theme
    var currentTheme = ThemeService.GetCurrentTheme();
    
    // Set theme
    await ThemeService.SetThemeAsync("dark");
    
    // Toggle theme
    await ThemeService.ToggleThemeAsync();
}
```

#### Listen to Theme Changes

```csharp
protected override async Task OnInitializedAsync()
{
    // Subscribe to theme change events
    ThemeService.OnThemeChanged += OnThemeChanged;
}

private void OnThemeChanged()
{
    // React to theme changes
    StateHasChanged();
}
```

## ?? Color Scheme

### Light Mode
- **Primary**: #37474f (Blue-Grey)
- **Accent**: #0288d1 (Bright Blue)
- **Background**: #eceff1 (Light Grey)
- **Text**: #263238 (Dark Grey)

### Dark Mode
- **Primary**: #1a1a1a (Almost Black)
- **Accent**: #42a5f5 (Lighter Blue)
- **Background**: #121212 (Pure Black)
- **Text**: #e0e0e0 (Light Grey)

## ?? Transitions

All color changes include smooth CSS transitions:

```css
body {
    transition: background-color 0.3s ease, color 0.3s ease;
}

.rbm-card {
    transition: background-color 0.3s ease, box-shadow 0.3s ease;
}
```

## ?? Mobile Support

Theme toggle works seamlessly on mobile devices:
- Topbar theme button available on all screen sizes
- Settings page responsive on mobile
- Theme preference synced across all devices (via localStorage)

## ?? Storage

Theme preference is stored in browser localStorage:

```javascript
localStorage.setItem('rbm-theme', 'dark');  // Save
var theme = localStorage.getItem('rbm-theme');  // Retrieve
```

## ?? Testing Dark Mode

1. **Toggle Button**: Click moon icon (??) in topbar ? page goes dark
2. **Settings Page**: Select "Dark" theme ? click Apply
3. **Persistence**: Refresh page ? theme should remain dark
4. **All Components**: Verify all cards, buttons, inputs look good in dark mode
5. **Modals**: Open any modal ? check dark mode styling

## ?? Troubleshooting

### Theme Not Applying
- Check browser console for JavaScript errors
- Clear localStorage and refresh: `localStorage.removeItem('rbm-theme')`
- Verify `dark-mode` class is present on `<html>` element

### Colors Look Wrong
- Ensure all CSS uses `var(--rbm-*)` variables
- Check media query for dark mode is correct: `html.dark-mode`
- Verify CSS transitions are not conflicting

### Theme Doesn't Persist
- Check localStorage is enabled in browser
- Verify `theme-manager.js` is loaded
- Check Network tab in DevTools for script loading errors

## ?? CSS Color Reference

| Element | Light | Dark |
|---------|-------|------|
| Primary Background | #eceff1 | #121212 |
| Card Background | #ffffff | #1e1e1e |
| Text Primary | #263238 | #e0e0e0 |
| Text Secondary | #607d8b | #b0b0b0 |
| Borders | #cfd8dc | #333333 |
| Success | #43a047 | #66bb6a |
| Warning | #fb8c00 | #ffa726 |
| Danger | #e53935 | #ef5350 |

## ? Checklist

- [x] Dark mode CSS variables defined
- [x] ThemeService implemented
- [x] Theme toggle component created
- [x] Settings page integration
- [x] localStorage persistence
- [x] JavaScript theme initialization
- [x] Smooth transitions added
- [x] Mobile responsive
- [x] All components tested in dark mode
- [x] Documentation complete

## ?? Next Steps

1. **User Preferences**: Store theme preference in user database
2. **System Preference**: Detect OS dark mode preference
3. **Custom Themes**: Allow users to create custom color schemes
4. **Scheduled Switching**: Auto-switch themes based on time of day
5. **Analytics**: Track which theme users prefer

## ?? Version History

**v1.0.0** - Initial dark mode implementation
- Basic light/dark theme support
- localStorage persistence
- Settings page integration
- Quick toggle button in topbar

---

**Last Updated**: December 2024
**Status**: ? Production Ready
