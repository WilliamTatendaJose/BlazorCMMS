# Units Selector Implementation for Condition Monitoring

## Overview

The Condition Monitoring page now includes a **Units Selector** that allows users to choose between three different measurement unit systems:
- **Imperial (US)**: °F, PSI, GPM
- **Metric**: °C, bar, L/min
- **SI**: °C, Pa, m³/s

## Features

### 1. Unit System Selection
- **Location**: Top-right of the page header, next to the "Add Reading" button
- **Options**:
  - ???? Imperial (°F, PSI) - Default
  - ?? Metric (°C, Bar)
  - ?? SI (°C, Pa)

### 2. Dynamic Unit Display
The following measurements are converted based on the selected unit system:

#### Temperature
- **Stored in**: Fahrenheit (°F)
- **Conversions**:
  - Metric: °C = (°F - 32) × 5/9
  - SI: °C = (°F - 32) × 5/9
  - Imperial: °F (no conversion)

#### Pressure
- **Stored in**: PSI
- **Conversions**:
  - Metric: bar = PSI × 0.0689476
  - SI: Pa = PSI × 6894.76
  - Imperial: PSI (no conversion)

#### Flow Rate
- **Stored in**: GPM
- **Conversions**:
  - Metric: L/min = GPM × 3.78541
  - SI: m³/s = GPM × 0.0000631
  - Imperial: GPM (no conversion)

### 3. Affected UI Elements

All temperature, pressure, and flow rate displays are converted:

- **Record Reading Form**: Input field labels show the current unit
- **Recent Readings**: Display converted values in the reading cards
- **Condition Trends**: Statistics show min/max/average in selected units
- **Reading Details Modal**: Displays all converted values
- **Insights Section**: Average temperature shows in selected unit

### 4. Implementation Details

#### Unit Conversion Methods

```csharp
// Temperature Conversion
private double ConvertTemperature(double fahrenheit)
{
    return selectedUnitSystem switch
    {
        "metric" => (fahrenheit - 32) * 5 / 9,
        "si" => (fahrenheit - 32) * 5 / 9,
        _ => fahrenheit
    };
}

// Pressure Conversion
private double ConvertPressure(double psi)
{
    return selectedUnitSystem switch
    {
        "metric" => psi * 0.0689476,
        "si" => psi * 6894.76,
        _ => psi
    };
}

// Flow Rate Conversion
private double ConvertFlowRate(double gpm)
{
    return selectedUnitSystem switch
    {
        "metric" => gpm * 3.78541,
        "si" => gpm * 0.0000631,
        _ => gpm
    };
}
```

#### Unit Symbol Methods

```csharp
private string GetTemperatureUnit() => selectedUnitSystem switch
{
    "metric" => "°C",
    "si" => "°C",
    _ => "°F"
};

private string GetPressureUnit() => selectedUnitSystem switch
{
    "metric" => "bar",
    "si" => "Pa",
    _ => "PSI"
};

private string GetFlowRateUnit() => selectedUnitSystem switch
{
    "metric" => "L/min",
    "si" => "m³/s",
    _ => "GPM"
};
```

#### Placeholder Methods

```csharp
private string GetTemperaturePlaceholder() => selectedUnitSystem switch
{
    "metric" => "e.g., 29.7",
    "si" => "e.g., 29.7",
    _ => "e.g., 85.5"
};

private string GetPressurePlaceholder() => selectedUnitSystem switch
{
    "metric" => "e.g., 3.4",
    "si" => "e.g., 344738",
    _ => "e.g., 50"
};

private string GetFlowRatePlaceholder() => selectedUnitSystem switch
{
    "metric" => "e.g., 189",
    "si" => "e.g., 0.003",
    _ => "e.g., 50"
};
```

## Data Storage

**Important**: All readings are stored in the database using **Imperial units** (Fahrenheit, PSI, GPM). The unit conversion happens only at the display layer, ensuring:
- Consistent data storage
- Easy addition of new unit systems in the future
- No data loss when switching units
- Historical data remains consistent

## User Preference Storage (Future Enhancement)

Currently, the unit preference defaults to "Imperial" for all users. To implement persistent user preferences:

1. **Update CurrentUserService**: Add `UnitPreference` property
2. **Save to Database**: Store preference in AspNetUsers table
3. **Load on Init**: Retrieve user's saved preference in `OnInitializedAsync`

```csharp
// In OnInitializedAsync
selectedUnitSystem = await GetUserUnitPreference() ?? "imperial";

private async Task<string?> GetUserUnitPreference()
{
    // TODO: Load from database based on CurrentUser.UserId
    return await UserService.GetUserUnitPreferenceAsync(CurrentUser.UserId);
}
```

## Examples

### Converting 85°F to Metric
Input: 85°F
Output: (85 - 32) × 5/9 = 29.4°C

### Converting 50 PSI to Metric
Input: 50 PSI
Output: 50 × 0.0689476 = 3.47 bar

### Converting 50 GPM to Metric
Input: 50 GPM
Output: 50 × 3.78541 = 189.3 L/min

## Testing Checklist

- [x] Unit selector displays in page header
- [x] All three unit systems available
- [x] Temperature values convert correctly
- [x] Pressure values convert correctly
- [x] Flow rate values convert correctly
- [x] Input placeholders update based on unit system
- [x] Recent readings display converted values
- [x] Condition trends statistics show converted values
- [x] Reading details modal shows converted values
- [x] Insights section shows converted values
- [ ] User preferences persist across sessions (future)

## Maintenance Notes

1. **Conversion Accuracy**: All conversions use industry-standard factors
2. **Precision**: Displayed values use 1 decimal place (F1 format)
3. **Storage**: Always store original Imperial values in database
4. **Validation**: Input validation remains unchanged (validates numeric input)
5. **Performance**: Conversions are O(1) and occur only during rendering

## Future Enhancements

1. **User Preferences**: Save unit preference per user in database
2. **Default System**: Allow admin to set system-wide default unit system
3. **More Units**: Add Kelvin, atm, in-H2O, etc.
4. **Threshold Conversions**: Auto-convert alert thresholds based on unit system
5. **Localization**: Tie unit system to user's locale
6. **API Defaults**: Accept/return units based on user preference in REST APIs

## Troubleshooting

### Unit selector not appearing
- Check that the rbm-action-bar div has proper flex styling
- Verify rbm-form-select CSS class is loaded

### Conversions not updating
- Ensure StateHasChanged() is called in OnUnitSystemChanged()
- Check that all @bind directives are properly connected

### Incorrect conversion values
- Verify conversion factors in the conversion methods
- Test with known values (e.g., 32°F = 0°C)

## Files Modified

- `BlazorApp1/Components/Pages/RBM/ConditionMonitoring.razor`
  - Added unit selector to header
  - Added conversion methods
  - Updated all unit-dependent displays
  - Added GetTemperatureUnit(), GetPressureUnit(), GetFlowRateUnit() methods
  - Added ConvertTemperature(), ConvertPressure(), ConvertFlowRate() methods
  - Added placeholder getter methods

## Related Documentation

- Condition Monitoring Feature: `README_CONDITION_MONITORING.md`
- Data Models: See `ConditionReading` model in Models folder
- Services: See `DataService.cs` for reading persistence
