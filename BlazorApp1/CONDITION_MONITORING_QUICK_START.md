# ?? CONDITION MONITORING - QUICK START GUIDE

## ?? Getting Started

### Access the Page
```
URL: /rbm/condition-monitoring
Permission: Authorize (any authenticated user)
Edit Permission: Users with CanEdit role
```

---

## ?? How to Record a Reading

### 1?? Select Asset
```
1. Click the "Asset" dropdown
2. Choose from active equipment
3. Example: "PMP-001 - Hydraulic Pump K"
```

### 2?? Set Reading Time
```
1. Modify "Reading Date" if needed
2. Default: Current date/time
3. Format: YYYY-MM-DD HH:mm
```

### 3?? Enter Parameters
```
Temperature (°F)
?? ? Normal: 40-130°F
?? ?? Warning: <40 or >130
?? Example: 85.5

Vibration (mm/s)
?? ? Normal: 0-5
?? ?? Warning: 5-10
?? ?? Critical: >10
?? Example: 3.2

Pressure (PSI)
?? ? Normal: 20-100
?? ?? Warning: <20 or >100
?? Example: 50

Oil Analysis
?? Normal ?
?? Warning ??
?? Critical ??

Electrical Parameters (Optional)
?? Current (Amps): 15.5
?? Noise Level (dB): 75

Additional Info
?? Notes: Free text observations
```

### 4?? Save Reading
```
1. Click "?? Save Reading" button
2. System auto-calculates status
3. Confirmation message appears
4. Form resets for next entry
```

---

## ?? Dashboard Metrics Explained

### Top 5 Cards

| Card | Meaning | Example |
|------|---------|---------|
| **Total Readings** | All readings across all assets | 1,245 |
| **Active Alerts** | Critical condition alerts | 3 |
| **Monitored Assets** | Active equipment being tracked | 12 |
| **Critical Status** | Equipment in critical condition | 1 |
| **Today's Readings** | Readings recorded today | 18 |

---

## ?? Understanding Asset Health

### Health Score Display
```
Color    Score      Status
?????????????????????????????
?? Green  80-100%    Healthy ?
?? Orange 60-79%     Warning ??
?? Red    <60%       Critical ??
```

### Recommended Actions

**?? Healthy (80-100%)**
```
? Asset Operating Normally
Continue routine monitoring.
No immediate action required.
```

**?? Warning (60-79%)**
```
?? Preventive Maintenance Recommended
Schedule maintenance within 7 days.
Monitor equipment closely.
```

**?? Critical (<60%)**
```
?? Immediate Action Required
Schedule emergency maintenance now.
Asset requires urgent attention.
```

---

## ?? Reading Trends

### What You'll See

```
Temperature Trend
?? Average temperature across readings
?? Color: ?? Blue
?? Example: 85.3°F average

Vibration Trend
?? Average vibration level
?? Maximum recorded value
?? Color: ?? Green
?? Example: 3.1 mm/s average, 4.2 max

Pressure Trend
?? Average pressure
?? Pressure range (min-max)
?? Color: ?? Red
?? Example: 49.8 PSI avg, 45-52 range
```

### How to Read Trends
1. **Select an asset** from dropdown
2. **Trends update automatically** with latest data
3. **Scroll through readings** to see history
4. **Export data** for detailed analysis

---

## ?? Alert System

### Alert Levels

```
?? CRITICAL
?? Color: Red background
?? Immediate action required
?? Examples: High vibration, Oil Critical

?? WARNING
?? Color: Orange background
?? Investigate within 24 hours
?? Examples: Temperature trending up
```

### Active Alerts Section
- Shows up to 5 most recent alerts
- Displays alert type and description
- Timestamp for reference
- Color-coded severity

---

## ?? Exporting Data

### Export to CSV
```
1. Select an asset
2. Add at least one reading
3. Click "?? Export" button
4. CSV file data prepares
5. Ready for Excel analysis
```

### CSV Format
```
Date,Temperature,Vibration,Pressure,Oil Analysis,Current,Noise,Notes,Status
2024-12-05 14:30,85.5,3.2,50.0,Normal,15.5,72.0,"Running normal",Normal
2024-12-05 12:00,84.2,3.1,49.0,Normal,15.3,71.5,"Good condition",Normal
```

---

## ? Checklist for Recording

Before saving, verify:

```
?? Asset selected (not "-- Select Asset --")
?? Reading date is reasonable
?? At least one parameter entered
?? Values are within expected ranges
?? Notes add useful context (if needed)
?? No error messages showing
```

---

## ? Common Issues

### Problem: "Please select an asset"
```
Solution: Click asset dropdown and select equipment
```

### Problem: Parameter shows ?? Warning
```
Solution: Review the parameter value
         Adjust if needed or investigate cause
```

### Problem: "Error saving reading"
```
Solution: Check internet connection
         Try again or contact administrator
```

### Problem: Trends not updating
```
Solution: Ensure asset is selected
         Verify reading saved successfully
         Refresh page if needed
```

---

## ?? Best Practices

### Recording Data
? Record readings at consistent times  
? Include relevant notes with context  
? Note any maintenance performed  
? Record environmental factors  
? Be consistent with parameter units  

### Parameter Thresholds
? Temperature: 40-130°F normal range  
? Vibration: <5 mm/s is healthy  
? Pressure: 20-100 PSI typical  
? Oil Analysis: Monitor changes  

### Alerts
? Respond promptly to alerts  
? Investigate root causes  
? Document corrective actions  
? Track maintenance history  

---

## ?? Support

### For Issues:
1. Check error message details
2. Verify all required fields filled
3. Review common issues section
4. Contact system administrator
5. Check documentation

---

## ?? Key Buttons

| Button | Purpose | Action |
|--------|---------|--------|
| ?? Add Reading | Open recording form | Show/focus form |
| ?? Save Reading | Submit new reading | Save to database |
| ?? Export | Download data | Prepare CSV |
| ?? Recent Readings | View history | Scroll list |
| ?? Condition Trends | Analyze patterns | Calculate stats |

---

## ?? Mobile Tips

- Use portrait orientation for best layout
- Scroll horizontally for all columns
- Tap carefully on small buttons
- Use date picker for reading time
- Allow extra time for calculations

---

**Quick Reference:** Print this guide for your equipment room!

Version: 1.0 | Date: December 5, 2024 | Status: ? Ready
