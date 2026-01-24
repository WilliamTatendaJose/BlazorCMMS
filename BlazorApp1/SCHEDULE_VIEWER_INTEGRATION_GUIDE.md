# Maintenance Schedule Viewer - Integration Guide

## Overview
This guide explains how to use the new **MaintenanceScheduleViewer.razor** and **RecurringScheduleModal.razor** components with the enhanced `RecurringMaintenanceScheduler` service that includes:
- ✅ Automatic weekend date adjustment (Sat→Fri, Sun→Mon)
- ✅ Color-coded task types for visual distinction
- ✅ Recurring schedule visualization with future occurrences

## Components Created

### 1. MaintenanceScheduleViewer.razor
**Location:** `BlazorApp1/Components/Pages/RBM/MaintenanceScheduleViewer.razor`

A full-page component displaying all maintenance schedules with:
- Color-coded task type badges
- Weekend adjustment indicators
- Recurring schedule preview
- Task type distribution statistics
- Responsive grid layout

**Route:** `/rbm/maintenance-schedule-viewer`

**Features:**
- Task type color legend (Preventive, Corrective, Predictive, Inspection, Emergency, Routine)
- Schedule cards with:
  - Color-coded left border
  - Task type badge with color
  - Scheduled date and time
  - Technician assignment
  - Duration estimate
  - Frequency information (if recurring)
  - Description
  - Expandable future occurrences timeline
- Statistics cards showing:
  - Total scheduled tasks
  - In-progress tasks
  - One-time vs recurring tasks
- Task type distribution bar chart

### 2. RecurringScheduleModal.razor
**Location:** `BlazorApp1/Components/Pages/RBM/RecurringScheduleModal.razor`

A modal dialog component for viewing detailed recurring schedule information:
- Colorful header with task type color gradient
- Schedule overview with key metrics
- Frequency breakdown
- Future 10 occurrences displayed as cards
- Weekend adjustment warnings
- Edit action callback

## Integration with MaintenancePlanning.razor

### Step 1: Add the Modal Reference

Add this to your `MaintenancePlanning.razor` component:

```razor
@if (showRecurringModal && selectedSchedule != null)
{
    <RecurringScheduleModal 
        Schedule="selectedSchedule"
        OnClose="@(() => { showRecurringModal = false; selectedSchedule = null; })"
        OnEdit="@((schedule) => { EditSchedule(schedule); })">
    </RecurringScheduleModal>
}
```

### Step 2: Add Modal State Variables

```csharp
private bool showRecurringModal = false;
private MaintenanceSchedule? selectedSchedule = null;
```

### Step 3: Add Method to Show Modal

```csharp
private void ShowRecurringScheduleModal(MaintenanceSchedule schedule)
{
    selectedSchedule = schedule;
    showRecurringModal = true;
}
```

### Step 4: Update Table to Use Modal

Replace the recurring schedule button in `MaintenancePlanning.razor`:

```razor
@if (!string.IsNullOrEmpty(schedule.Frequency))
{
    <button class="rbm-btn rbm-btn-outline rbm-btn-sm" 
            @onclick="() => ShowRecurringScheduleModal(schedule)" 
            title="View recurring schedule info">
        🔄 View Schedule
    </button>
}
```

## Color Mapping Reference

The `RecurringMaintenanceScheduler` maps task types to colors:

| Task Type | Color | Hex Code | Use Case |
|-----------|-------|----------|----------|
| Preventive | 🟢 Green | #4CAF50 | Planned maintenance |
| Corrective | 🟠 Orange | #FF9800 | Equipment fixes |
| Predictive | 🔵 Blue | #2196F3 | Condition-based |
| Inspection | 🟣 Purple | #9C27B0 | Quality checks |
| Emergency | 🔴 Red | #F44336 | Critical issues |
| Routine | 🔵 Cyan | #00BCD4 | Regular tasks |
| Unscheduled | 🟠 Deep Orange | #FF5722 | Ad-hoc work |
| Breakdown | 🔴 Red | #F44336 | System failures |

### Using Colors in Custom Components

```csharp
// Get color for a schedule
var color = RecurringScheduler.GetTaskTypeColor(schedule.Type);

// Get friendly name with color
var colorName = RecurringScheduler.GetTaskTypeColorName(schedule.Type);
// Example output: "Green (Preventive)"
```

## Weekend Adjustment System

The scheduler automatically adjusts dates that fall on weekends:

- **Saturday → Friday** (previous day)
- **Sunday → Monday** (next day)

### Checking for Weekends

```csharp
private bool IsWeekend(DateTime date)
{
    return date.DayOfWeek == DayOfWeek.Saturday || 
           date.DayOfWeek == DayOfWeek.Sunday;
}
```

### In UI

Schedules show a weekend badge if the original calculated date fell on a weekend:

```razor
@if (IsWeekend(schedule.ScheduledDate))
{
    <span class="weekend-badge">⚠️ Adjusted to Weekday</span>
}
```

## CSS Classes Reference

### Schedule Cards
```css
.schedule-card          /* Main schedule card with colored left border */
.task-type-badge       /* Color-coded type badge */
.weekend-badge         /* Warning indicator for weekend dates */
.schedule-date         /* Styled date display */
```

### Occurrence Timeline
```css
.occurrence-timeline   /* Container for future occurrences */
.occurrence-item       /* Individual occurrence card */
.occurrence-indicator  /* Numbered circle for occurrence */
```

### Status Indicators
```css
.status-chip           /* Status badge (Scheduled, Overdue, Completed) */
.status-scheduled      /* Blue - scheduled status */
.status-overdue        /* Red - overdue status */
.status-completed      /* Green - completed status */
```

### Color Legend
```css
.color-legend          /* Grid of color definitions */
.legend-item           /* Individual legend entry */
.legend-color          /* Color box in legend */
```

## Data Flow Example

```
User navigates to /rbm/maintenance-schedule-viewer
    ↓
MaintenanceScheduleViewer loads schedules
    ↓
For each schedule:
  - RecurringScheduler.GetSchedulingInfo() gets colors and dates
  - RecurringScheduler.GetTaskTypeColor() returns hex color
  - Check if weekend with IsWeekend()
  - Display in color-coded card
    ↓
User clicks "View Next 5 Occurrences"
    ↓
RecurringScheduler.GetFutureOccurrences() returns 5 items
    ↓
Each occurrence displays with:
  - Adjusted date (if it was weekend)
  - Color indicator
  - Days from now
  - Weekend warning if applicable
```

## Methods Reference

### RecurringMaintenanceScheduler Methods

```csharp
// Get hex color for task type
public string GetTaskTypeColor(string taskType)

// Get friendly color name
public string GetTaskTypeColorName(string taskType)

// Adjust date to weekday if needed
public DateTime AdjustToWeekday(DateTime date)

// Get complete scheduling info
public SchedulingInfo GetSchedulingInfo(MaintenanceSchedule schedule)

// Get future occurrences (with colors and weekend adjustment)
public List<ScheduleOccurrence> GetFutureOccurrences(
    MaintenanceSchedule schedule, 
    int numberOfOccurrences = 5)
```

### SchedulingInfo Properties

```csharp
public int CurrentScheduleId { get; set; }
public string Frequency { get; set; }
public int FrequencyDays { get; set; }
public DateTime LastScheduledDate { get; set; }
public DateTime NextScheduledDate { get; set; }
public int DaysUntilNext { get; set; }
public bool IsOverdue { get; set; }
public double EstimatedDuration { get; set; }
public string TechnicianName { get; set; }
public string TaskType { get; set; }
public string TaskTypeColor { get; set; }      // Hex color
public string TaskTypeColorName { get; set; }  // Friendly name
```

### ScheduleOccurrence Properties

```csharp
public int OccurrenceNumber { get; set; }
public DateTime ScheduledDate { get; set; }
public int DaysFromNow { get; set; }
public string Status { get; set; }
public string TaskType { get; set; }
public string TaskTypeColor { get; set; }      // Hex color
public string TaskTypeColorName { get; set; }  // Friendly name
```

## Usage Examples

### Example 1: Display a Schedule with Color

```razor
@inject RecurringMaintenanceScheduler RecurringScheduler

@foreach (var schedule in schedules)
{
    var info = RecurringScheduler.GetSchedulingInfo(schedule);
    
    <div style="border-left: 5px solid @info.TaskTypeColor; padding: 16px;">
        <span style="background-color: @info.TaskTypeColor; color: white; padding: 4px 12px; border-radius: 4px;">
            @info.TaskTypeColorName
        </span>
        <h3>@schedule.AssetName</h3>
        <p>@schedule.ScheduledDate.ToString("MMM dd, yyyy")</p>
    </div>
}
```

### Example 2: Show Future Occurrences with Timeline

```razor
@{
    var occurrences = RecurringScheduler.GetFutureOccurrences(schedule, 10);
}

@foreach (var occ in occurrences)
{
    <div style="border-left: 4px solid @occ.TaskTypeColor;">
        <strong>@occ.ScheduledDate.ToString("dddd, MMM dd")</strong>
        <span style="color: @occ.TaskTypeColor;">@occ.TaskTypeColorName</span>
        <small>@occ.DaysFromNow days from now</small>
    </div>
}
```

### Example 3: Weekend Check in Validation

```csharp
public async Task ValidateScheduleDate(MaintenanceSchedule schedule)
{
    var adjustedDate = RecurringScheduler.AdjustToWeekday(schedule.ScheduledDate);
    
    if (adjustedDate != schedule.ScheduledDate)
    {
        Console.WriteLine($"Original: {schedule.ScheduledDate:dddd}");
        Console.WriteLine($"Adjusted: {adjustedDate:dddd}");
    }
}
```

## Styling Customization

### Change Color Scheme

Edit the color mappings in `RecurringMaintenanceScheduler.cs`:

```csharp
public string GetTaskTypeColor(string taskType)
{
    return taskType?.ToLower() switch
    {
        "preventive" => "#YourColorHere",  // Change hex code
        // ... other types
    };
}
```

### Modify CSS

All component styles use CSS custom properties for easy theming:

```css
:root {
    --card-color: var(--task-color);  /* Dynamic color from inline style */
    --rbm-accent: #0288d1;
    --rbm-text-light: #999;
    /* ... other variables */
}
```

## Testing

### Test Weekend Adjustment

```csharp
// Saturday test
var sat = new DateTime(2024, 12, 21);  // A Saturday
var adjusted = RecurringScheduler.AdjustToWeekday(sat);
// Result: 2024-12-20 (Friday)

// Sunday test
var sun = new DateTime(2024, 12, 22);  // A Sunday
var adjusted = RecurringScheduler.AdjustToWeekday(sun);
// Result: 2024-12-23 (Monday)

// Weekday test
var wed = new DateTime(2024, 12, 18);  // A Wednesday
var adjusted = RecurringScheduler.AdjustToWeekday(wed);
// Result: 2024-12-18 (unchanged)
```

### Test Color Mapping

```csharp
var color = RecurringScheduler.GetTaskTypeColor("Preventive");
// Result: "#4CAF50"

var name = RecurringScheduler.GetTaskTypeColorName("Preventive");
// Result: "Green (Preventive)"
```

## Navigation Links

Add these navigation links to your app menu:

```razor
<NavLink href="/rbm/maintenance-schedule-viewer" 
         Match="NavLinkMatch.All" 
         class="nav-item">
    📅 Schedule Viewer
</NavLink>
```

## Troubleshooting

### Colors Not Showing
- Ensure `RecurringMaintenanceScheduler` is registered in DI
- Check that schedule `Type` matches one of the defined mappings
- Verify `@inject RecurringMaintenanceScheduler` is declared

### Weekend Adjustment Not Working
- Check that dates are in correct format (DateTime)
- Verify `AdjustToWeekday()` is being called before saving
- Test with known weekend dates (Sat/Sun)

### Modal Not Appearing
- Ensure `RecurringScheduleModal.razor` is in the correct directory
- Check `OnClose` callback is properly declared
- Verify `showRecurringModal` state is being toggled

## Support & Future Enhancements

Current features:
- ✅ Color-coded task types
- ✅ Weekend date adjustment
- ✅ Recurring schedule preview
- ✅ Future occurrence timeline

Potential enhancements:
- [ ] Holiday calendar integration
- [ ] Custom business day configuration per tenant
- [ ] Color customization in user settings
- [ ] Export schedules as calendar (.ics)
- [ ] Integration with calendar sync services
- [ ] Drag-and-drop schedule adjustment
