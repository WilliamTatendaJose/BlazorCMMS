# Maintenance Scheduling Improvements

## Overview
Enhanced the `RecurringMaintenanceScheduler` service with automatic weekend date adjustment and visual color coding for task types.

## Changes Made

### 1. Weekend Date Adjustment
Added a new `AdjustToWeekday()` method that automatically shifts weekend-scheduled tasks to weekdays:
- **Saturday → Friday** (previous day)
- **Sunday → Monday** (next day)
- Weekdays remain unchanged

#### Where It's Applied
- `GenerateRecurringSchedules()` - Applies to all generated future schedules
- `ProcessRecurringSchedulesAsync()` - Applies when processing completed recurring tasks
- `GetFutureOccurrences()` - Applies when previewing future schedule occurrences

### 2. Color Coding for Task Types
Added two color-related methods to visually distinguish maintenance tasks:

#### `GetTaskTypeColor(string taskType)` 
Returns hex color codes:
- **#4CAF50** - Green: Preventive/Preventative maintenance
- **#FF9800** - Orange: Corrective/Corrective maintenance
- **#2196F3** - Blue: Predictive/Predictive maintenance
- **#9C27B0** - Purple: Inspection
- **#F44336** - Red: Emergency/Emergency maintenance/Breakdown
- **#00BCD4** - Cyan: Routine
- **#FF5722** - Deep Orange: Unscheduled
- **#607D8B** - Blue Grey: Default/Other

#### `GetTaskTypeColorName(string taskType)`
Returns user-friendly labels like:
- "Green (Preventive)"
- "Orange (Corrective)"
- "Blue (Predictive)"
- etc.

### 3. Updated Data Models

#### `SchedulingInfo` Class
Added three new properties:
```csharp
public string TaskType { get; set; } = string.Empty;
public string TaskTypeColor { get; set; } = "#607D8B";      // Hex color
public string TaskTypeColorName { get; set; } = "Grey (Other)"; // Label
```

#### `ScheduleOccurrence` Class
Added three new properties:
```csharp
public string TaskType { get; set; } = string.Empty;
public string TaskTypeColor { get; set; } = "#607D8B";      // Hex color
public string TaskTypeColorName { get; set; } = "Grey (Other)"; // Label
```

### 4. Updated Methods

#### `GetSchedulingInfo()`
Now populates color information:
```csharp
TaskType = schedule.Type,
TaskTypeColor = GetTaskTypeColor(schedule.Type),
TaskTypeColorName = GetTaskTypeColorName(schedule.Type)
```

#### `GetFutureOccurrences()`
- Now applies `AdjustToWeekday()` to all generated dates
- Populates color information for each occurrence

## Usage Examples

### In Blazor Components
```csharp
@inject RecurringMaintenanceScheduler ScheduleService

// Get scheduling info with color
var info = ScheduleService.GetSchedulingInfo(schedule);

// Apply color in UI
<div style="background-color: @info.TaskTypeColor; padding: 10px; border-radius: 4px;">
    @info.TaskTypeColorName - @info.TechnicianName
</div>

// Get future occurrences with weekend adjustment
var future = ScheduleService.GetFutureOccurrences(schedule);

// Each occurrence has color coding
@foreach (var occ in future)
{
    <tr style="border-left: 5px solid @occ.TaskTypeColor;">
        <td>@occ.ScheduledDate.ToShortDateString()</td>
        <td>@occ.TaskTypeColorName</td>
    </tr>
}
```

### In CSS/Tailwind
```css
/* Use the color property directly */
.schedule-card {
    border-left: 4px solid var(--task-color);
    background: linear-gradient(to right, var(--task-color)33, white);
}
```

## Benefits
1. **Better User Experience**: Tasks automatically scheduled on business days
2. **Visual Clarity**: Color-coded tasks are easier to scan and identify
3. **Compliance**: Respects typical business hours (Mon-Fri)
4. **Flexibility**: Easy to customize colors or weekday rules
5. **Backward Compatible**: Existing code continues to work without changes

## Testing Checklist
- [ ] Test weekend date adjustment (schedule for Saturday → verify Friday)
- [ ] Test Sunday adjustment (schedule for Sunday → verify Monday)
- [ ] Verify colors display correctly in UI
- [ ] Test with various task types
- [ ] Verify existing schedules still work
- [ ] Check recurring schedule generation
- [ ] Verify ProcessRecurringSchedulesAsync() applies adjustments

## Future Enhancements
- Add configurable weekend rules (e.g., different business days per tenant)
- Add color customization per tenant
- Add task type color preferences in user settings
- Add color legend/indicator to UI
- Support holiday calendar integration to skip holidays
