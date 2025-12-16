using Microsoft.JSInterop;

namespace BlazorApp1.Services;

/// <summary>
/// Service for managing application theme (light/dark mode)
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private string _currentTheme = "light";
    private bool _initialized = false;
    
    public event Action? OnThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Get the current theme
    /// </summary>
    public string GetCurrentTheme() => _currentTheme;

    /// <summary>
    /// Initialize or re-apply theme from localStorage
    /// </summary>
    public async Task InitializeThemeAsync()
    {
        try
        {
            if (!_initialized)
            {
                var savedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "rbm-theme");
                _currentTheme = savedTheme ?? "light";
                _initialized = true;
            }
            // Always ensure DOM reflects the current theme
            await ApplyThemeAsync(_currentTheme);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing theme: {ex.Message}");
            _currentTheme = "light";
            _initialized = true;
        }
    }

    /// <summary>
    /// Set the theme to light or dark
    /// </summary>
    public async Task SetThemeAsync(string theme)
    {
        if (theme != "light" && theme != "dark")
        {
            throw new ArgumentException("Theme must be 'light' or 'dark'", nameof(theme));
        }

        _currentTheme = theme;
        await ApplyThemeAsync(theme);
        
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rbm-theme", theme);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving theme preference: {ex.Message}");
        }

        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Toggle between light and dark mode
    /// </summary>
    public async Task ToggleThemeAsync()
    {
        var newTheme = _currentTheme == "light" ? "dark" : "light";
        await SetThemeAsync(newTheme);
    }

    /// <summary>
    /// Apply theme by updating DOM
    /// </summary>
    private async Task ApplyThemeAsync(string theme)
    {
        try
        {
            // Prefer theme-manager if loaded
            await _jsRuntime.InvokeVoidAsync("themeManager.applyTheme", theme);
        }
        catch
        {
            // Fallback: manipulate DOM directly
            try
            {
                if (theme == "dark")
                {
                    await _jsRuntime.InvokeVoidAsync("eval",
                        "(function(){var h=document.documentElement;h.classList.add('dark-mode');h.setAttribute('data-theme','dark');localStorage.setItem('rbm-theme','dark');})();");
                }
                else
                {
                    await _jsRuntime.InvokeVoidAsync("eval",
                        "(function(){var h=document.documentElement;h.classList.remove('dark-mode');h.setAttribute('data-theme','light');localStorage.setItem('rbm-theme','light');})();");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying theme via fallback: {ex.Message}");
            }
        }
    }
}
