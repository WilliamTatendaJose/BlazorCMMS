# Units Selector - Quick Reference Guide

## Accessing the Units Feature

1. Navigate to **Condition Monitoring** page
2. Look for the **Units selector** in the top-right of the page header (next to "Add Reading" button)
3. Select your preferred unit system from the dropdown

## Available Unit Systems

| System | Temperature | Pressure | Flow Rate | Best For |
|--------|-------------|----------|-----------|----------|
| ???? Imperial | °F | PSI | GPM | USA-based operations |
| ?? Metric | °C | bar | L/min | Most countries |
| ?? SI | °C | Pa (Pascals) | m³/s | Scientific/Engineering |

## Quick Conversion Reference

### Temperature Conversions
```
85°F = 29.4°C
100°F = 37.8°C
68°F = 20°C (Room temperature)
```

### Pressure Conversions
```
50 PSI = 3.45 bar = 344,738 Pa
100 PSI = 6.89 bar = 689,476 Pa
14.7 PSI = 1 bar = 101,325 Pa (atmospheric)
```

### Flow Rate Conversions
```
50 GPM = 189.3 L/min = 0.00316 m³/s
100 GPM = 378.5 L/min = 0.00631 m³/s
```

## Where Units Are Applied

### 1. Recording Form
- Temperature input field label shows current unit
- Pressure input field label shows current unit
- Flow rate input field label shows current unit
- Placeholder text updates based on unit system

### 2. Recent Readings Display
- Temperature values in reading cards convert to selected unit
- Pressure values in reading cards convert to selected unit
- Unit symbols displayed after values

### 3. Condition Trends
- Average temperature/pressure statistics update
- Min/max values display in selected units
- Charts and statistics use selected units

### 4. Reading Details Modal
- All temperature, pressure, flow rate values convert
- Unit symbols display with converted values

## Best Practices

? **DO:**
- Use **Imperial** if you operate in the United States
- Use **Metric** for international operations (most common)
- Use **SI** for scientific documentation and research
- Switch units as needed for different reports
- Share reports with unit context (always mention units)

? **DON'T:**
- Assume others use your preferred unit system
- Mix unit systems in the same report
- Forget to mention units when communicating readings
- Change units mid-report without noting the conversion

## Common Workflows

### Scenario 1: Recording a Temperature Reading
1. Open Condition Monitoring
2. Select preferred unit system (e.g., Metric for °C)
3. Enter temperature in that unit (e.g., 29.7)
4. System stores as equivalent Fahrenheit (85°F)
5. All displays show in your selected unit

### Scenario 2: Reviewing Historical Data
1. Select your preferred unit system at the top
2. All existing readings display in that unit
3. Charts and trends automatically update
4. Switch units anytime to see same data differently

### Scenario 3: Exporting Data
1. Set unit system to desired output format
2. Click "Export CSV"
3. Data exports showing values in selected units
4. (Recommended: Include unit headers in CSV)

## FAQ

**Q: Will changing units affect my stored data?**
A: No. Your data is stored in Imperial units (°F, PSI, GPM) in the database. The unit system selector only changes the display format.

**Q: Can other users see my unit preference?**
A: Currently, unit preference is session-only. Each user can independently choose their preferred unit system.

**Q: What if I mix units when entering readings?**
A: Always enter readings in the unit shown in the label. The system assumes inputs match the displayed unit.

**Q: How do I report my readings?**
A: 
1. Set your preferred unit system
2. View the readings you want to report
3. Take screenshots or export as CSV
4. Always include unit symbols in your report

**Q: What about alert thresholds?**
A: Alert thresholds are currently in Imperial units. Future updates may auto-convert thresholds based on your unit system.

## Keyboard Shortcut

Currently no keyboard shortcuts for unit switching. Planned for future versions:
- `Alt + U` for quick unit system toggle

## Visual Indicators

- ???? = Imperial/US system
- ?? = Metric system (international)
- ?? = SI system (scientific)

## Need Help?

- See full documentation: `UNITS_SELECTOR_IMPLEMENTATION.md`
- Contact: System Administrator
- Report bugs: Include your unit system preference in the report

---

**Last Updated**: November 2024
**Feature Status**: ? Production Ready
**Version**: 1.0
