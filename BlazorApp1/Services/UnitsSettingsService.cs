using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Service for managing user preferences and unit system settings
/// Used app-wide by ConditionMonitoring, Assets, and other components
/// </summary>
public class UnitsSettingsService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private UserSettings? _currentUserSettings;
    private string _currentUserId = string.Empty;

    public UnitsSettingsService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Initialize settings for a specific user
    /// </summary>
    public async Task InitializeAsync(string userId)
    {
        _currentUserId = userId;
        
        using var context = _contextFactory.CreateDbContext();
        
        var settings = await context.UserSettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings == null)
        {
            // Create default settings for new user
            settings = new UserSettings
            {
                UserId = userId,
                PreferredUnitSystem = "imperial"
            };
            
            context.UserSettings.Add(settings);
            await context.SaveChangesAsync();
        }

        // Create a detached copy to avoid tracking issues
        _currentUserSettings = new UserSettings
        {
            Id = settings.Id,
            UserId = settings.UserId,
            PreferredUnitSystem = settings.PreferredUnitSystem,
            TemperatureUnit = settings.TemperatureUnit,
            PressureUnit = settings.PressureUnit,
            FlowRateUnit = settings.FlowRateUnit,
            WeightUnit = settings.WeightUnit,
            LengthUnit = settings.LengthUnit,
            DistanceUnit = settings.DistanceUnit,
            ThemePreference = settings.ThemePreference,
            DateFormat = settings.DateFormat,
            TimeFormat = settings.TimeFormat,
            DecimalPlaces = settings.DecimalPlaces,
            EnableNotifications = settings.EnableNotifications,
            NotificationFrequency = settings.NotificationFrequency,
            CreatedDate = settings.CreatedDate,
            ModifiedDate = settings.ModifiedDate
        };
    }

    /// <summary>
    /// Get current user settings
    /// </summary>
    public UserSettings? GetCurrentSettings() => _currentUserSettings;

    /// <summary>
    /// Get settings for a specific user (admin only)
    /// </summary>
    public async Task<UserSettings?> GetUserSettingsAsync(string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserSettings
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    /// <summary>
    /// Update user unit system preference
    /// </summary>
    public async Task SetUnitSystemAsync(string userId, string unitSystem)
    {
        using var context = _contextFactory.CreateDbContext();
        
        var settings = await context.UserSettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings != null)
        {
            settings.PreferredUnitSystem = unitSystem;
            settings.ModifiedDate = DateTime.Now;
            await context.SaveChangesAsync();

            if (userId == _currentUserId)
            {
                _currentUserSettings!.PreferredUnitSystem = unitSystem;
                _currentUserSettings.ModifiedDate = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Get the current preferred unit system
    /// </summary>
    public string GetUnitSystem() => _currentUserSettings?.PreferredUnitSystem ?? "imperial";

    #region Temperature Unit Methods

    public string GetTemperatureUnit()
    {
        // Check for custom unit override first
        if (!string.IsNullOrEmpty(_currentUserSettings?.TemperatureUnit))
        {
            return _currentUserSettings.TemperatureUnit;
        }

        // Fall back to system preference
        return GetUnitSystem() switch
        {
            "metric" => "°C",
            "si" => "°C",
            _ => "°F"
        };
    }

    public double ConvertTemperature(double celsius, string? targetUnit = null)
    {
        targetUnit ??= GetTemperatureUnit();

        return targetUnit switch
        {
            "°F" => (celsius * 9 / 5) + 32,
            "°C" => celsius,
            "K" => celsius + 273.15,
            _ => celsius
        };
    }

    public double ConvertTemperatureToInternal(double value, string sourceUnit)
    {
        // Store everything internally as Celsius
        return sourceUnit switch
        {
            "°F" => (value - 32) * 5 / 9,
            "K" => value - 273.15,
            _ => value // Assume Celsius
        };
    }

    #endregion

    #region Pressure Unit Methods

    public string GetPressureUnit()
    {
        // Check for custom unit override first
        if (!string.IsNullOrEmpty(_currentUserSettings?.PressureUnit))
        {
            return _currentUserSettings.PressureUnit;
        }

        // Fall back to system preference
        return GetUnitSystem() switch
        {
            "metric" => "bar",
            "si" => "Pa",
            _ => "PSI"
        };
    }

    public double ConvertPressure(double pascal, string? targetUnit = null)
    {
        targetUnit ??= GetPressureUnit();

        return targetUnit switch
        {
            "PSI" => pascal * 0.000145038,
            "bar" => pascal * 0.00001,
            "Pa" => pascal,
            "atm" => pascal / 101325,
            "kPa" => pascal / 1000,
            _ => pascal
        };
    }

    public double ConvertPressureToInternal(double value, string sourceUnit)
    {
        // Store everything internally as Pascal
        return sourceUnit switch
        {
            "PSI" => value * 6894.76,
            "bar" => value * 100000,
            "atm" => value * 101325,
            "kPa" => value * 1000,
            _ => value // Assume Pascal
        };
    }

    #endregion

    #region Flow Rate Unit Methods

    public string GetFlowRateUnit()
    {
        // Check for custom unit override first
        if (!string.IsNullOrEmpty(_currentUserSettings?.FlowRateUnit))
        {
            return _currentUserSettings.FlowRateUnit;
        }

        // Fall back to system preference
        return GetUnitSystem() switch
        {
            "metric" => "L/min",
            "si" => "m³/s",
            _ => "GPM"
        };
    }

    public double ConvertFlowRate(double literPerMin, string? targetUnit = null)
    {
        targetUnit ??= GetFlowRateUnit();

        return targetUnit switch
        {
            "GPM" => literPerMin * 0.264172,
            "L/min" => literPerMin,
            "m³/s" => literPerMin * 0.0000166667,
            "m³/h" => literPerMin * 0.06,
            "L/s" => literPerMin * 0.0166667,
            _ => literPerMin
        };
    }

    public double ConvertFlowRateToInternal(double value, string sourceUnit)
    {
        // Store everything internally as L/min
        return sourceUnit switch
        {
            "GPM" => value * 3.78541,
            "m³/s" => value * 60000,
            "m³/h" => value * 16.6667,
            "L/s" => value * 60,
            _ => value // Assume L/min
        };
    }

    #endregion

    #region Weight Unit Methods

    public string GetWeightUnit()
    {
        // Check for custom unit override first
        if (!string.IsNullOrEmpty(_currentUserSettings?.WeightUnit))
        {
            return _currentUserSettings.WeightUnit;
        }

        // Fall back to system preference
        return GetUnitSystem() switch
        {
            "metric" => "kg",
            "si" => "kg",
            _ => "lb"
        };
    }

    public double ConvertWeight(double kilogram, string? targetUnit = null)
    {
        targetUnit ??= GetWeightUnit();

        return targetUnit switch
        {
            "lb" => kilogram * 2.20462,
            "kg" => kilogram,
            "g" => kilogram * 1000,
            "ton" => kilogram * 0.001,
            _ => kilogram
        };
    }

    public double ConvertWeightToInternal(double value, string sourceUnit)
    {
        // Store everything internally as kg
        return sourceUnit switch
        {
            "lb" => value * 0.453592,
            "g" => value / 1000,
            "ton" => value * 1000,
            _ => value // Assume kg
        };
    }

    #endregion

    #region Length Unit Methods

    public string GetLengthUnit()
    {
        // Check for custom unit override first
        if (!string.IsNullOrEmpty(_currentUserSettings?.LengthUnit))
        {
            return _currentUserSettings.LengthUnit;
        }

        // Fall back to system preference
        return GetUnitSystem() switch
        {
            "metric" => "mm",
            "si" => "m",
            _ => "in"
        };
    }

    public double ConvertLength(double millimeter, string? targetUnit = null)
    {
        targetUnit ??= GetLengthUnit();

        return targetUnit switch
        {
            "in" => millimeter * 0.0393701,
            "ft" => millimeter * 0.00328084,
            "m" => millimeter * 0.001,
            "mm" => millimeter,
            "cm" => millimeter * 0.1,
            _ => millimeter
        };
    }

    public double ConvertLengthToInternal(double value, string sourceUnit)
    {
        // Store everything internally as mm
        return sourceUnit switch
        {
            "in" => value * 25.4,
            "ft" => value * 304.8,
            "m" => value * 1000,
            "cm" => value * 10,
            _ => value // Assume mm
        };
    }

    #endregion

    #region Distance Unit Methods

    public string GetDistanceUnit()
    {
        // Check for custom unit override first
        if (!string.IsNullOrEmpty(_currentUserSettings?.DistanceUnit))
        {
            return _currentUserSettings.DistanceUnit;
        }

        // Fall back to system preference
        return GetUnitSystem() switch
        {
            "metric" => "km",
            "si" => "m",
            _ => "mi"
        };
    }

    public double ConvertDistance(double kilometer, string? targetUnit = null)
    {
        targetUnit ??= GetDistanceUnit();

        return targetUnit switch
        {
            "mi" => kilometer * 0.621371,
            "km" => kilometer,
            "m" => kilometer * 1000,
            _ => kilometer
        };
    }

    public double ConvertDistanceToInternal(double value, string sourceUnit)
    {
        // Store everything internally as km
        return sourceUnit switch
        {
            "mi" => value * 1.60934,
            "m" => value * 0.001,
            _ => value // Assume km
        };
    }

    #endregion

    #region Format Methods

    public string GetDateFormat() => _currentUserSettings?.DateFormat ?? "MM/dd/yyyy";

    public string GetTimeFormat() => _currentUserSettings?.TimeFormat ?? "12h";

    public int GetDecimalPlaces() => _currentUserSettings?.DecimalPlaces ?? 2;

    public string FormatMeasurement(double value, string unit)
    {
        var decimalPlaces = GetDecimalPlaces();
        return $"{value.ToString($"F{decimalPlaces}")}{unit}";
    }

    public string FormatDate(DateTime date)
    {
        return date.ToString(GetDateFormat());
    }

    public string FormatTime(DateTime dateTime)
    {
        var format = GetTimeFormat() == "24h" ? "HH:mm" : "hh:mm tt";
        return dateTime.ToString(format);
    }

    #endregion

    #region Update Methods

    public async Task UpdateSettingsAsync(UserSettings settings)
    {
        if (settings.UserId != _currentUserId)
        {
            throw new UnauthorizedAccessException("Cannot update settings for another user");
        }

        using var context = _contextFactory.CreateDbContext();
        
        settings.ModifiedDate = DateTime.Now;
        context.UserSettings.Update(settings);
        await context.SaveChangesAsync();
        
        _currentUserSettings = settings;
    }

    #endregion

    #region Placeholder Methods

    public string GetTemperaturePlaceholder()
    {
        return GetTemperatureUnit() switch
        {
            "°C" => "e.g., 29.7",
            "K" => "e.g., 303.0",
            _ => "e.g., 85.5"
        };
    }

    public string GetPressurePlaceholder()
    {
        return GetPressureUnit() switch
        {
            "bar" => "e.g., 3.4",
            "Pa" => "e.g., 344738",
            "atm" => "e.g., 0.94",
            "kPa" => "e.g., 344.7",
            _ => "e.g., 50"
        };
    }

    public string GetFlowRatePlaceholder()
    {
        return GetFlowRateUnit() switch
        {
            "L/min" => "e.g., 189",
            "m³/s" => "e.g., 0.003",
            "m³/h" => "e.g., 11.3",
            "L/s" => "e.g., 3.15",
            _ => "e.g., 50"
        };
    }

    #endregion
}
