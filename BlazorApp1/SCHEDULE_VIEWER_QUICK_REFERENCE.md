# Maintenance Schedule Viewer - Quick Reference

## 🎨 Color Codes

| Type | Color | Code | When Used |
|------|-------|------|-----------|
| Preventive | 🟢 | #4CAF50 | Planned maintenance |
| Corrective | 🟠 | #FF9800 | Equipment fixes |
| Predictive | 🔵 | #2196F3 | Condition-based |
| Inspection | 🟣 | #9C27B0 | Quality checks |
| Emergency | 🔴 | #F44336 | Critical/urgent |
| Routine | 🔵 | #00BCD4 | Regular upkeep |

## 📅 Weekend Adjustment

```
Saturday → Friday (day before)
Sunday → Monday (day after)
Weekdays → No change
```

## 🚀 Quick Start

### 1. Add to Navigation
```html
<NavLink href="/rbm/maintenance-schedule-viewer">📅 Schedule Viewer</NavLink>
```

### 2. Use in Code
```csharp
@inject RecurringMaintenanceScheduler RecurringScheduler

var info = RecurringScheduler.GetSchedulingInfo(schedule);
var occurrences = RecurringScheduler.GetFutureOccurrences(schedule);
```

### 3. Display with Color
```html
<div style="color: @info.TaskTypeColor; font-weight: 600;">
    @info.TaskTypeColorName
</div>
```

## 📁 Files Created

| File | Purpose |
|------|---------|
| `MaintenanceScheduleViewer.razor` | Full-page viewer with all schedules |
| `RecurringScheduleModal.razor` | Modal for detailed recurring info |
| `SCHEDULE_VIEWER_INTEGRATION_GUIDE.md` | Full integration documentation |
| `SCHEDULING_IMPROVEMENTS.md` | Technical details of improvements |

## 🔧 Key Methods

### Get Color
```csharp
string color = RecurringScheduler.GetTaskTypeColor("Preventive");
// Returns: "#4CAF50"

string name = RecurringScheduler.GetTaskTypeColorName("Preventive");
// Returns: "Green (Preventive)"
```

### Get Schedule Info
```csharp
var info = RecurringScheduler.GetSchedulingInfo(schedule);
// Returns: SchedulingInfo with colors and dates
```

### Get Future Occurrences
```csharp
var occurrences = RecurringScheduler.GetFutureOccurrences(schedule, 10);
// Returns: List of next 10 occurrences with colors
```

### Adjust Weekends
```csharp
var adjusted = RecurringScheduler.AdjustToWeekday(dateTime);
// Returns: Date adjusted if it's Sat/Sun
```

## 🎯 Component Features

### MaintenanceScheduleViewer
✅ Color-coded schedule cards  
✅ Weekend adjustment indicators  
✅ Recurring schedule preview  
✅ Expandable future occurrences  
✅ Task distribution statistics  
✅ Responsive grid layout  

### RecurringScheduleModal
✅ Colorful header with gradient  
✅ Schedule overview metrics  
✅ 10 future occurrences  
✅ Weekend warnings  
✅ Edit integration  

## 📊 Status Indicators

| Status | Color | Used For |
|--------|-------|----------|
| Scheduled | 🔵 Blue | Upcoming tasks |
| Overdue | 🔴 Red | Past due dates |
| Completed | 🟢 Green | Finished tasks |
| In Progress | 🔵 Blue | Currently running |

## 🛠️ Integration Steps

### Into MaintenancePlanning.razor

**1. Add Modal State**
```csharp
private bool showRecurringModal = false;
private MaintenanceSchedule? selectedSchedule = null;
```

**2. Add Modal Component**
```html
@if (showRecurringModal && selectedSchedule != null)
{
    <RecurringScheduleModal 
        Schedule="selectedSchedule"
        OnClose="@(() => showRecurringModal = false)">
    </RecurringScheduleModal>
}
```

**3. Add Show Method**
```csharp
void ShowRecurringScheduleModal(MaintenanceSchedule schedule)
{
    selectedSchedule = schedule;
    showRecurringModal = true;
}
```

**4. Update Button**
```html
<button @onclick="() => ShowRecurringScheduleModal(schedule)">
    🔄 View Schedule
</button>
```

## 💡 Tips & Tricks

### Check for Weekend
```csharp
if (date.DayOfWeek == DayOfWeek.Saturday || 
    date.DayOfWeek == DayOfWeek.Sunday)
{
    // It's a weekend
}
```

### Display Days Until Next
```html
@if (info.DaysUntilNext > 0)
{
    <span>Next in @info.DaysUntilNext days</span>
}
else if (info.IsOverdue)
{
    <span style="color: red;">OVERDUE</span>
}
```

### Apply Dynamic Color
```html
<div style="background-color: @info.TaskTypeColor; color: white;">
    @info.TaskTypeColorName
</div>
```

### Create Timeline
```html
@foreach (var occ in occurrences)
{
    <div style="border-left: 4px solid @occ.TaskTypeColor; padding: 8px;">
        @occ.ScheduledDate.ToString("MMM dd, yyyy")
    </div>
}
```

## 🧪 Common Test Cases

### Weekend Adjustment
- Input: Saturday (12/21) → Output: Friday (12/20)
- Input: Sunday (12/22) → Output: Monday (12/23)
- Input: Wednesday (12/18) → Output: Wednesday (12/18)

### Color Mapping
- "Preventive" → #4CAF50 (Green)
- "Corrective" → #FF9800 (Orange)
- "Unknown Type" → #607D8B (Grey)

### Future Occurrences
- Every 7 days frequency → 7 days between each
- Weekend dates adjusted automatically
- Past dates skipped

## 📱 Responsive Behavior

- **Desktop (1200px+):** 4-column grid
- **Tablet (768px-1200px):** 2-3 column grid
- **Mobile (< 768px):** 1 column, stacked

## ⚡ Performance Notes

- Schedule loading: Uses async/await
- Color mapping: O(1) switch expression
- Weekend adjustment: Single date operation
- Future occurrences: Cached after first call

## 🔐 Access Control

```csharp
@attribute [Authorize]  // Required for both components

// In MaintenanceScheduleViewer
@if(CurrentUser.CanEdit)
{
    // Show edit buttons
}
```

## 📖 Documentation Files

- **SCHEDULE_VIEWER_INTEGRATION_GUIDE.md** - Full integration steps
- **SCHEDULING_IMPROVEMENTS.md** - Technical implementation details
- **RecurringMaintenanceScheduler.cs** - Service source code

## 🆘 Quick Troubleshooting

**Colors not showing?**
→ Check task `Type` matches mapping in `GetTaskTypeColor()`

**Weekend not adjusting?**
→ Ensure `AdjustToWeekday()` is called before saving

**Modal not appearing?**
→ Verify `showRecurringModal` bool is toggled true

**Occurrences empty?**
→ Check schedule has `Frequency` set and isn't too far in past

## 🎓 Learning Path

1. Read `SCHEDULING_IMPROVEMENTS.md` - Understand the changes
2. Review `MaintenanceScheduleViewer.razor` - See full implementation
3. Check `RecurringScheduleModal.razor` - See modal pattern
4. Follow `SCHEDULE_VIEWER_INTEGRATION_GUIDE.md` - Integrate into existing pages
5. Test with weekend dates - Verify adjustment works
6. Customize colors - Adapt to your branding

## 📞 Support

For issues:
1. Check console for errors (`F12`)
2. Verify services are registered in `Program.cs`
3. Ensure components are in correct directory
4. Review integration steps in guide document

---

**Version:** 1.0  
**Last Updated:** 2024  
**Status:** Production Ready ✅
