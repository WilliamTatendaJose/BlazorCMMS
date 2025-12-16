# Units Selector - Testing Guide

## Test Environment Setup

1. Start the application
2. Navigate to `/rbm/condition-monitoring`
3. Ensure you have at least one asset with readings

## Unit Test Cases

### Test 1: Unit Selector Visibility
**Objective**: Verify unit selector displays in page header

**Steps**:
1. Navigate to Condition Monitoring page
2. Look at top-right area of page header

**Expected Result**:
- Unit selector dropdown visible next to "Add Reading" button
- Three options visible: Imperial, Metric, SI
- Imperial selected by default

**Status**: ? Pass / ? Fail

---

### Test 2: Temperature Conversion - Imperial to Metric
**Objective**: Verify temperature conversion from °F to °C

**Steps**:
1. Add a reading with Temperature = 85°F
2. Switch unit system to Metric
3. View the recorded reading

**Expected Result**:
- Temperature displays as ~29.4°C
- Calculation: (85 - 32) × 5/9 = 29.4

**Test Values**:
| Input °F | Expected °C | Formula |
|----------|------------|---------|
| 32 | 0 | (32-32)×5/9 = 0 |
| 68 | 20 | (68-32)×5/9 = 20 |
| 85 | 29.4 | (85-32)×5/9 = 29.4 |
| 100 | 37.8 | (100-32)×5/9 = 37.8 |

**Status**: ? Pass / ? Fail

---

### Test 3: Temperature Conversion - Metric to SI
**Objective**: Verify Metric and SI both use Celsius

**Steps**:
1. Add reading with Temperature = 85°F
2. Switch to Metric - note the °C value
3. Switch to SI - verify same °C value

**Expected Result**:
- Both systems show same temperature in °C
- Metric label: "°C"
- SI label: "°C"

**Status**: ? Pass / ? Fail

---

### Test 4: Pressure Conversion - PSI to bar
**Objective**: Verify pressure conversion from PSI to bar

**Steps**:
1. Add reading with Pressure = 50 PSI
2. Switch to Metric
3. View recent readings

**Expected Result**:
- Pressure displays as ~3.45 bar
- Calculation: 50 × 0.0689476 = 3.447

**Test Values**:
| Input PSI | Expected bar | Formula |
|-----------|-------------|---------|
| 50 | 3.45 | 50 × 0.0689476 = 3.45 |
| 100 | 6.89 | 100 × 0.0689476 = 6.89 |
| 14.7 | 1.01 | 14.7 × 0.0689476 = 1.01 |

**Status**: ? Pass / ? Fail

---

### Test 5: Pressure Conversion - PSI to Pa
**Objective**: Verify pressure conversion from PSI to Pascals

**Steps**:
1. Add reading with Pressure = 50 PSI
2. Switch to SI
3. View recent readings

**Expected Result**:
- Pressure displays as ~344,738 Pa
- Calculation: 50 × 6894.76 = 344,738

**Test Values**:
| Input PSI | Expected Pa | Formula |
|-----------|------------|---------|
| 50 | 344,738 | 50 × 6894.76 = 344,738 |
| 100 | 689,476 | 100 × 6894.76 = 689,476 |
| 1 | 6,894.76 | 1 × 6894.76 = 6,894.76 |

**Status**: ? Pass / ? Fail

---

### Test 6: Flow Rate Conversion - GPM to L/min
**Objective**: Verify flow rate conversion from GPM to Liters/minute

**Steps**:
1. Add reading with Flow Rate = 50 GPM
2. Switch to Metric
3. View condition trends

**Expected Result**:
- Flow rate displays as ~189.3 L/min
- Calculation: 50 × 3.78541 = 189.27

**Test Values**:
| Input GPM | Expected L/min | Formula |
|-----------|----------------|---------|
| 50 | 189.3 | 50 × 3.78541 = 189.27 |
| 100 | 378.5 | 100 × 3.78541 = 378.54 |
| 10 | 37.9 | 10 × 3.78541 = 37.854 |

**Status**: ? Pass / ? Fail

---

### Test 7: Flow Rate Conversion - GPM to m³/s
**Objective**: Verify flow rate conversion from GPM to cubic meters/second

**Steps**:
1. Add reading with Flow Rate = 50 GPM
2. Switch to SI
3. View condition trends

**Expected Result**:
- Flow rate displays as ~0.00316 m³/s
- Calculation: 50 × 0.0000631 = 0.003155

**Status**: ? Pass / ? Fail

---

### Test 8: Form Input Placeholders
**Objective**: Verify input placeholders update based on unit system

**Steps**:
1. Switch unit system to Imperial
2. Check Temperature field placeholder
3. Switch to Metric
4. Check Temperature field placeholder again
5. Repeat for Pressure and Flow Rate fields

**Expected Results**:
| Field | Imperial | Metric | SI |
|-------|----------|--------|-----|
| Temperature | "e.g., 85.5" | "e.g., 29.7" | "e.g., 29.7" |
| Pressure | "e.g., 50" | "e.g., 3.4" | "e.g., 344738" |
| Flow Rate | "e.g., 50" | "e.g., 189" | "e.g., 0.003" |

**Status**: ? Pass / ? Fail

---

### Test 9: Recent Readings Display
**Objective**: Verify all readings in list show converted values

**Steps**:
1. Create 5 readings with various temperatures/pressures
2. Switch between unit systems
3. Verify each reading updates

**Expected Result**:
- All temperature values update when unit system changes
- All pressure values update when unit system changes
- Unit symbols change correctly
- Values remain consistent for same reading

**Status**: ? Pass / ? Fail

---

### Test 10: Condition Trends Statistics
**Objective**: Verify trend statistics update with unit conversions

**Steps**:
1. Create readings: 70°F, 75°F, 80°F
2. Switch to Metric
3. Verify average, min, max calculations

**Expected Result**:
- Average displays correctly: (21.1 + 23.9 + 26.7) / 3 = 23.9°C
- Min displays correctly: 21.1°C
- Max displays correctly: 26.7°C

**Test Data**:
| Reading | °F | °C |
|---------|-----|------|
| 1 | 70 | 21.1 |
| 2 | 75 | 23.9 |
| 3 | 80 | 26.7 |
| Average | 75 | 23.9 |

**Status**: ? Pass / ? Fail

---

### Test 11: Reading Details Modal
**Objective**: Verify modal displays all converted values

**Steps**:
1. Create a reading with all parameters filled
2. Switch unit system to Metric
3. Click on reading to open details modal
4. Verify all conversions in modal

**Expected Result**:
- Temperature shows in °C
- Pressure shows in bar
- Flow Rate shows in L/min
- All other values unchanged
- Modal title unchanged

**Status**: ? Pass / ? Fail

---

### Test 12: Insights Section
**Objective**: Verify insights show correct unit averages

**Steps**:
1. Create 3+ readings for same asset
2. Switch between unit systems
3. Check "Avg Temp" in insights section

**Expected Result**:
- Average temperature shows in current unit
- Value matches calculation
- Unit symbol updates correctly

**Status**: ? Pass / ? Fail

---

### Test 13: Unit Persistence During Session
**Objective**: Verify unit selection persists when navigating

**Steps**:
1. Set unit system to Metric
2. Switch to different page (e.g., Assets)
3. Return to Condition Monitoring

**Expected Result**:
- Unit system remains Metric
- All values still display in Metric

**Status**: ? Pass / ? Fail

---

### Test 14: Data Consistency Check
**Objective**: Verify stored data isn't affected by unit conversions

**Steps**:
1. Add reading with Temperature 85°F in Imperial
2. Switch to Metric (displays ~29.4°C)
3. Switch back to Imperial

**Expected Result**:
- Original value 85°F appears
- No rounding errors
- Conversion is reversible

**Status**: ? Pass / ? Fail

---

### Test 15: Multiple Users
**Objective**: Verify units are per-session, not global

**Steps**:
1. Open two browser windows/tabs
2. In window 1: Set to Metric, add reading
3. In window 2: Keep Imperial
4. Verify readings show different units

**Expected Result**:
- Window 1 shows reading in Metric
- Window 2 shows reading in Imperial
- Both windows show same stored data

**Status**: ? Pass / ? Fail

---

## Regression Tests

### Test R1: Data Entry Still Works
**Objective**: Verify no breaking changes to data entry

**Steps**:
1. Switch through all unit systems
2. Enter reading in each system
3. Verify all save successfully

**Expected Result**: All readings save without errors

**Status**: ? Pass / ? Fail

---

### Test R2: Export Still Works
**Objective**: Verify export functionality unchanged

**Steps**:
1. Switch to Metric
2. Click "Export CSV"
3. Verify data exports

**Expected Result**: Export completes without error

**Status**: ? Pass / ? Fail

---

### Test R3: Alerts Still Function
**Objective**: Verify alert thresholds unaffected

**Steps**:
1. Create critical reading
2. Switch unit systems
3. Verify alert still appears

**Expected Result**: Alerts display regardless of unit system

**Status**: ? Pass / ? Fail

---

### Test R4: Asset Selection Still Works
**Objective**: Verify asset dropdown unaffected

**Steps**:
1. Switch unit systems
2. Select different assets
3. Verify readings update

**Expected Result**: Asset selection works in all unit systems

**Status**: ? Pass / ? Fail

---

## Performance Tests

### Test P1: Conversion Speed
**Objective**: Verify conversions don't slow UI

**Steps**:
1. Load 50+ readings
2. Switch unit systems
3. Measure page response time

**Expected Result**: No noticeable lag when switching units

**Status**: ? Pass / ? Fail

---

### Test P2: Memory Usage
**Objective**: Verify no memory leaks

**Steps**:
1. Switch units 20+ times
2. Check browser memory usage
3. Compare initial vs final memory

**Expected Result**: No significant memory increase

**Status**: ? Pass / ? Fail

---

## Browser Compatibility Tests

| Browser | Version | Status | Notes |
|---------|---------|--------|-------|
| Chrome | Latest | ? Pass | Test on desktop and mobile |
| Firefox | Latest | ? Pass | Test on desktop and mobile |
| Safari | Latest | ? Pass | Test on desktop and mobile |
| Edge | Latest | ? Pass | Test on desktop and mobile |

---

## Test Summary

**Total Tests**: 20
**Passed**: ___
**Failed**: ___
**Blocked**: ___

**Overall Status**: ? Ready for Production / ? Need Fixes

**Sign-Off**:
- Tested By: _______________
- Date: _______________
- Approved By: _______________

---

## Known Issues

(To be filled after testing)

---

## Follow-up Actions

- [ ] Fix any failing tests
- [ ] Document known limitations
- [ ] Update user documentation
- [ ] Plan next enhancement sprint
