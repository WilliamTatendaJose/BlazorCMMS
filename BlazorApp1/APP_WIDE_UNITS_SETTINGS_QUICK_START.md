# ?? APP-WIDE UNITS SETTINGS - QUICK START

## ? 5-Minute Setup

### Step 1: Create Database Migration

```bash
cd BlazorApp1
dotnet ef migrations add AddUserSettings
dotnet ef database update
```

**What it does:**
- Creates `UserSettings` table in database
- Adds columns for unit preferences, formats, and settings
- Sets up foreign key to `AspNetUsers`

### Step 2: Verify Files Are In Place

? Check these files exist:
- `Models/UserSettings.cs` - Data model
- `Services/UnitsSettingsService.cs` - Main service
- `Components/Pages/RBM/UnitsSettingsComponent.razor` - UI component
- `Components/Pages/RBM/MyProfile.razor` - Updated with component

? Check these are updated:
- `Program.cs` - Contains `builder.Services.AddScoped<UnitsSettingsService>();`
- `ApplicationDbContext.cs` - Contains `public DbSet<UserSettings> UserSettings { get; set; }`

### Step 3: Test in Browser

1. Navigate to **My Profile** page (`/rbm/profile`)
2. Scroll down to "?? Units & Measurements" section
3. Select a unit system (Imperial/Metric/SI)
4. See message: "? Settings saved successfully"
5. Refresh page - settings should persist

---

## ?? Using in Your Components

### Example: Condition Monitoring Integration

```csharp
@page "/rbm/condition-monitoring"
@using BlazorApp1.Services
@using BlazorApp1.Models
@inject UnitsSettingsService UnitsSettings

@code {
    protected override async Task OnInitializedAsync()
    {
        // Initialize with current user ID
        var userId = "user-id-here";  // Get from CurrentUserService
        await UnitsSettings.InitializeAsync(userId);
        
        // Now you can use these methods:
        var tempUnit = UnitsSettings.GetTemperatureUnit();      // "°F", "°C", or "K"
        var pressureUnit = UnitsSettings.GetPressureUnit();     // "PSI", "bar", etc.
    }
    
    private void DisplayReading(double celsiusTemp)
    {
        // Convert Celsius (internal storage) to user's preferred unit
        var displayValue = UnitsSettings.ConvertTemperature(celsiusTemp);
        var unit = UnitsSettings.GetTemperatureUnit();
        Console.WriteLine($"Temperature: {displayValue:F2}{unit}");
    }
    
    private void SaveUserInput(double inputValue, string userEnteredUnit)
    {
        // Convert user's input to internal storage format (Celsius)
        var internalValue = UnitsSettings.ConvertTemperatureToInternal(inputValue, userEnteredUnit);
        // Save internalValue to database
    }
}
```

### Display Conversions in Razor

```razor
@{
    var tempInCelsius = 29.7;  // From database
    var displayTemp = UnitsSettings.ConvertTemperature(tempInCelsius);
    var unit = UnitsSettings.GetTemperatureUnit();
}

<div>Temperature: @displayTemp.ToString("F1")@unit</div>
<!-- Output: "Temperature: 85.5°F" (if imperial) -->
<!-- Output: "Temperature: 29.7°C" (if metric) -->
```

---

## ?? Internal Storage Convention

| Data Type | Store As | Convert At Input | Convert At Output |
|---|---|---|---|
| Temperature | Celsius | ? User input ? Celsius | ? Celsius ? User display unit |
| Pressure | Pascal (Pa) | ? User input ? Pa | ? Pa ? User display unit |
| Flow Rate | L/min | ? User input ? L/min | ? L/min ? User display unit |

**Why?** Consistency - always store in SI units, convert for display.

---

## ?? Available Methods

### Temperature
```csharp
UnitsSettings.GetTemperatureUnit()              // Returns: "°F", "°C", or "K"
UnitsSettings.ConvertTemperature(celsiusValue) // Converts from Celsius
UnitsSettings.ConvertTemperatureToInternal(value, sourceUnit)
UnitsSettings.GetTemperaturePlaceholder()       // "e.g., 85.5"
```

### Pressure
```csharp
UnitsSettings.GetPressureUnit()                 // Returns: "PSI", "bar", "Pa", etc.
UnitsSettings.ConvertPressure(pascalValue)
UnitsSettings.ConvertPressureToInternal(value, sourceUnit)
UnitsSettings.GetPressurePlaceholder()
```

### Flow Rate
```csharp
UnitsSettings.GetFlowRateUnit()                 // Returns: "GPM", "L/min", "m³/s"
UnitsSettings.ConvertFlowRate(literPerMinValue)
UnitsSettings.ConvertFlowRateToInternal(value, sourceUnit)
UnitsSettings.GetFlowRatePlaceholder()
```

### Weight, Length, Distance
Similar methods for weight, length, and distance.

### Formatting
```csharp
UnitsSettings.FormatMeasurement(value, unit)   // "85.50°F"
UnitsSettings.FormatDate(dateTime)             // Formats by user's preference
UnitsSettings.FormatTime(dateTime)             // 12h or 24h format
```

---

## ? Testing Checklist

- [ ] Database migration ran successfully
- [ ] My Profile page loads without errors
- [ ] Units & Measurements section visible
- [ ] Can select Imperial/Metric/SI
- [ ] Settings save with success message
- [ ] Settings persist after page reload
- [ ] Can override individual units
- [ ] Format options work
- [ ] Notification settings work
- [ ] Component loads in other pages

---

## ?? Troubleshooting

### Issue: "UserSettings table not found"
**Solution:**
```bash
dotnet ef migrations add AddUserSettings
dotnet ef database update
```

### Issue: "UnitsSettingsService not injected"
**Solution:** Check `Program.cs` has:
```csharp
builder.Services.AddScoped<UnitsSettingsService>();
```

### Issue: Settings don't save
**Solution:** Check browser console for errors. Verify:
1. User is authenticated
2. Database connection is working
3. Service initialization happened

### Issue: Conversions are wrong
**Solution:** Remember:
- Store in SI units (Celsius, Pascal, L/min)
- Convert on input (user input ? SI)
- Convert on output (SI ? user display)

---

## ?? Unit System Quick Reference

### Imperial (????)
- Temperature: °F (Fahrenheit)
- Pressure: PSI (pounds per square inch)
- Flow: GPM (gallons per minute)
- Weight: lb (pounds)
- Length: in (inches)
- Distance: mi (miles)

### Metric (??)
- Temperature: °C (Celsius)
- Pressure: bar
- Flow: L/min (liters per minute)
- Weight: kg (kilograms)
- Length: mm (millimeters)
- Distance: km (kilometers)

### SI (??)
- Temperature: °C (Celsius)
- Pressure: Pa (Pascals)
- Flow: m³/s (cubic meters per second)
- Weight: kg (kilograms)
- Length: m (meters)
- Distance: m (meters)

---

## ??? Common Implementation Patterns

### Pattern 1: Display with Automatic Conversion
```razor
@{
    var reading = conditionReadings.First();
    var tempDisplay = UnitsSettings.ConvertTemperature(reading.Temperature);
}

<span>@tempDisplay.ToString("F2")@UnitsSettings.GetTemperatureUnit()</span>
```

### Pattern 2: Form Input with Conversion
```razor
<input type="number" @bind="inputValue" placeholder="@UnitsSettings.GetTemperaturePlaceholder()">

@code {
    private double inputValue;
    
    private void SaveReading()
    {
        // Convert user input to internal format
        var internalValue = UnitsSettings.ConvertTemperatureToInternal(
            inputValue, 
            UnitsSettings.GetTemperatureUnit()
        );
        
        // Save to database
        reading.Temperature = internalValue;
    }
}
```

### Pattern 3: Formatted Output
```razor
<div>@UnitsSettings.FormatMeasurement(29.7, UnitsSettings.GetTemperatureUnit())</div>
<!-- Output: "29.70°C" or "85.46°F" depending on user preference -->
```

---

## ?? Next Steps

1. **Run migration** (see Step 1 above)
2. **Test in My Profile**
3. **Integrate with Condition Monitoring**
4. **Extend to other components**
5. **Monitor performance**

---

## ?? Need Help?

1. Check the comprehensive documentation: `APP_WIDE_UNITS_SETTINGS_COMPLETE.md`
2. Review the service: `Services/UnitsSettingsService.cs`
3. Check example usage: `Components/Pages/RBM/UnitsSettingsComponent.razor`
4. See integration: `Components/Pages/RBM/MyProfile.razor`

---

**Version**: 1.0  
**Status**: ? Production Ready  
**Created**: December 16, 2024
