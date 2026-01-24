# Complete Razor Component Integration Example

## Overview
This file shows complete working examples of how to integrate the new maintenance schedule viewer components into your Blazor application.

## Example 1: Using MaintenanceScheduleViewer (Standalone Page)

**File:** `MaintenanceScheduleViewer.razor`  
**Route:** `/rbm/maintenance-schedule-viewer`  
**Already Created:** ✅ Ready to use

Simply navigate to the route to view all schedules with color coding.

### Access from Navigation
```razor
<NavLink href="/rbm/maintenance-schedule-viewer" Match="NavLinkMatch.All">
    📅 Schedule Viewer
</NavLink>
```

---

## Example 2: Integrating RecurringScheduleModal into MaintenancePlanning.razor

### Step 1: Add Modal State to Code Section

In `MaintenancePlanning.razor`, add these variables to your `@code` block:

```csharp
@code {
    // ... existing code ...

    // NEW - for recurring schedule modal
    private bool showRecurringModal = false;
    private MaintenanceSchedule? selectedRecurringSchedule = null;

    // Existing code continues...
    private List<MaintenanceSchedule> schedules = new();
    // ... rest of existing code ...
}
```

### Step 2: Add Modal Component Reference

Add this markup to your `MaintenancePlanning.razor` (at the end of the file, before `@code`):

```razor
@* Recurring Schedule Modal *@
@if (showRecurringModal && selectedRecurringSchedule != null)
{
    <RecurringScheduleModal 
        Schedule="selectedRecurringSchedule"
        OnClose="@(() => CloseRecurringModal())"
        OnEdit="@((schedule) => { EditSchedule(schedule); CloseRecurringModal(); })">
    </RecurringScheduleModal>
}
```

### Step 3: Add Methods to Show/Close Modal

In the `@code` section, add these methods:

```csharp
private void ShowRecurringScheduleModal(MaintenanceSchedule schedule)
{
    selectedRecurringSchedule = schedule;
    showRecurringModal = true;
}

private void CloseRecurringModal()
{
    showRecurringModal = false;
    selectedRecurringSchedule = null;
}
```

### Step 4: Update the Schedule Table

In the schedule table row where you have the recurring button, update it:

```razor
@foreach (var schedule in GetFilteredSchedules().OrderBy(s => s.ScheduledDate))
{
    <tr>
        <td><strong>@schedule.AssetName</strong></td>
        <td><span class="rbm-badge @GetTypeBadgeClass(schedule.Type)">@schedule.Type</span></td>
        <td>@schedule.ScheduledDate.ToString("MMM dd, yyyy HH:mm")</td>
        <td>@schedule.EstimatedDuration.ToString("F1") hrs</td>
        <td>@schedule.AssignedTechnician</td>
        <td>
            @if (!string.IsNullOrEmpty(schedule.Frequency))
            {
                <span style="color: var(--rbm-accent); font-weight: 500;">@schedule.Frequency</span>
            }
            else
            {
                <span style="color: var(--rbm-text-light);">—</span>
            }
        </td>
        <td><span class="rbm-badge @GetStatusBadgeClass(schedule.Status)">@schedule.Status</span></td>
        <td style="display: flex; gap: 4px;">
            <button class="rbm-btn rbm-btn-outline rbm-btn-sm" @onclick="() => ShowScheduleDetails(schedule)">View</button>
            
            @* NEW - Recurring Schedule Modal Button *@
            @if (!string.IsNullOrEmpty(schedule.Frequency))
            {
                <button class="rbm-btn rbm-btn-outline rbm-btn-sm" 
                        @onclick="() => ShowRecurringScheduleModal(schedule)" 
                        title="View detailed recurring schedule information"
                        style="background: #e3f2fd; color: #1565c0;">
                    🔄 Recurring
                </button>
            }
            
            @if (CurrentUser.CanEdit && schedule.Status != "Completed")
            {
                <button class="rbm-btn rbm-btn-outline rbm-btn-sm" @onclick="() => EditSchedule(schedule)">Edit</button>
            }
            @if (CurrentUser.CanEdit)
            {
                <button class="rbm-btn rbm-btn-outline rbm-btn-sm" @onclick="() => DeleteSchedule(schedule)">Delete</button>
            }
        </td>
    </tr>
}
```

---

## Example 3: Display Colors in a Custom List Component

Create a new component `ScheduleListWithColors.razor`:

```razor
@using BlazorApp1.Models
@using BlazorApp1.Services
@inject RecurringMaintenanceScheduler RecurringScheduler

<div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 16px;">
    @foreach (var schedule in Schedules)
    {
        var info = RecurringScheduler.GetSchedulingInfo(schedule);
        
        <div style="border: 1px solid #ddd; border-left: 5px solid @info.TaskTypeColor; 
                    padding: 16px; border-radius: 8px; background: white;">
            
            <!-- Type Badge with Dynamic Color -->
            <div style="margin-bottom: 12px;">
                <span style="background-color: @info.TaskTypeColor; color: white; 
                            padding: 6px 12px; border-radius: 20px; font-size: 12px; 
                            font-weight: 600; text-transform: uppercase;">
                    @info.TaskTypeColorName
                </span>
            </div>

            <!-- Asset Name -->
            <h3 style="margin: 8px 0; font-size: 18px; font-weight: 600;">
                @schedule.AssetName
            </h3>

            <!-- Schedule Date -->
            <div style="margin: 8px 0; color: #666; font-size: 14px;">
                📅 @schedule.ScheduledDate.ToString("dddd, MMMM dd, yyyy")
            </div>

            <!-- Technician -->
            <div style="margin: 8px 0; color: #666; font-size: 14px;">
                👤 @schedule.AssignedTechnician
            </div>

            <!-- Duration -->
            <div style="margin: 8px 0; color: #666; font-size: 14px;">
                ⏱️ @schedule.EstimatedDuration.ToString("F1") hours
            </div>

            <!-- Frequency if Recurring -->
            @if (!string.IsNullOrEmpty(schedule.Frequency))
            {
                <div style="margin: 12px 0; padding: 10px; background: #f0f7ff; 
                           border-radius: 6px; color: #1565c0; font-weight: 600;">
                    🔄 Repeats: @schedule.Frequency
                </div>
            }

            <!-- Status with Color -->
            <div style="margin-top: 12px; padding-top: 12px; border-top: 1px solid #eee;">
                <span style="background: @GetStatusColor(schedule.Status); color: white; 
                            padding: 4px 8px; border-radius: 4px; font-size: 12px; 
                            font-weight: 600;">
                    @schedule.Status
                </span>
            </div>
        </div>
    }
</div>

@code {
    [Parameter]
    public List<MaintenanceSchedule> Schedules { get; set; } = new();

    private string GetStatusColor(string status)
    {
        return status switch
        {
            "Scheduled" => "#2196f3",
            "In Progress" => "#ff9800",
            "Completed" => "#4caf50",
            "Cancelled" => "#757575",
            _ => "#999"
        };
    }
}
```

### Usage:
```razor
@inject DataService DataService

@code {
    private List<MaintenanceSchedule> schedules = new();

    protected override async Task OnInitializedAsync()
    {
        schedules = await DataService.GetSchedulesAsync();
    }
}

<ScheduleListWithColors Schedules="schedules" />
```

---

## Example 4: Display Future Occurrences Timeline in Dashboard

Create a new component `RecurringScheduleTimeline.razor`:

```razor
@using BlazorApp1.Models
@using BlazorApp1.Services
@inject RecurringMaintenanceScheduler RecurringScheduler

<div style="padding: 20px; background: white; border-radius: 8px;">
    <h3 style="margin-bottom: 20px; font-weight: 600;">📋 Upcoming Maintenance (Next 10 Occurrences)</h3>

    @if (Schedule != null)
    {
        var info = RecurringScheduler.GetSchedulingInfo(Schedule);
        var occurrences = RecurringScheduler.GetFutureOccurrences(Schedule, 10);

        @if (occurrences.Count > 0)
        {
            <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 12px;">
                @foreach (var occ in occurrences)
                {
                    <div style="padding: 16px; background: #f9f9f9; border-left: 4px solid @info.TaskTypeColor; 
                               border-radius: 6px; display: flex; gap: 12px;">
                        
                        <!-- Occurrence Number Circle -->
                        <div style="width: 40px; height: 40px; border-radius: 50%; 
                                   background: @info.TaskTypeColor; color: white; 
                                   display: flex; align-items: center; justify-content: center; 
                                   font-weight: 600; flex-shrink: 0;">
                            @occ.OccurrenceNumber
                        </div>

                        <!-- Occurrence Details -->
                        <div style="flex: 1;">
                            <div style="font-weight: 600; color: #333;">
                                @occ.ScheduledDate.ToString("ddd, MMM dd, yyyy")
                            </div>
                            <div style="font-size: 12px; color: #666; margin-top: 4px;">
                                @if (occ.DaysFromNow < 0)
                                {
                                    <span>@Math.Abs(occ.DaysFromNow) days ago</span>
                                }
                                else if (occ.DaysFromNow == 0)
                                {
                                    <span style="color: #f57c00; font-weight: 600;">Today! ⏰</span>
                                }
                                else if (occ.DaysFromNow == 1)
                                {
                                    <span style="color: #2196f3; font-weight: 600;">Tomorrow</span>
                                }
                                else
                                {
                                    <span>@occ.DaysFromNow days from now</span>
                                }
                            </div>
                        </div>

                        <!-- Status Badge -->
                        <div style="text-align: right;">
                            <span style="background: @GetStatusBg(occ.Status); color: @GetStatusColor(occ.Status); 
                                        padding: 4px 8px; border-radius: 4px; font-size: 11px; font-weight: 600;">
                                @occ.Status
                            </span>
                        </div>
                    </div>
                }
            </div>
        }
        else
        {
            <div style="padding: 20px; text-align: center; color: #999;">
                No upcoming occurrences scheduled
            </div>
        }
    }
    else
    {
        <div style="padding: 20px; text-align: center; color: #999;">
            No schedule selected
        </div>
    }
</div>

@code {
    [Parameter]
    public MaintenanceSchedule? Schedule { get; set; }

    private string GetStatusColor(string status)
    {
        return status switch
        {
            "Scheduled" => "#1565c0",
            "Overdue" => "#c62828",
            "Completed" => "#2e7d32",
            _ => "#666"
        };
    }

    private string GetStatusBg(string status)
    {
        return status switch
        {
            "Scheduled" => "#e3f2fd",
            "Overdue" => "#ffebee",
            "Completed" => "#e8f5e9",
            _ => "#f5f5f5"
        };
    }
}
```

### Usage:
```razor
<RecurringScheduleTimeline Schedule="selectedSchedule" />
```

---

## Example 5: Weekend Adjustment Validation

Add this to your schedule creation/edit component:

```csharp
@code {
    @inject RecurringMaintenanceScheduler RecurringScheduler

    private MaintenanceSchedule schedule = new();

    private async Task SaveSchedule()
    {
        // Adjust the date if it's a weekend
        var adjustedDate = RecurringScheduler.AdjustToWeekday(schedule.ScheduledDate);

        if (adjustedDate != schedule.ScheduledDate)
        {
            var originalDay = schedule.ScheduledDate.ToString("dddd");
            var adjustedDay = adjustedDate.ToString("dddd");
            
            // Show user notification
            Console.WriteLine($"⚠️ Schedule adjusted from {originalDay} to {adjustedDay}");
            
            // Update the schedule with adjusted date
            schedule.ScheduledDate = adjustedDate;
        }

        // Save the schedule
        await DataService.AddScheduleAsync(schedule);
    }
}
```

---

## Example 6: Dashboard Widget with Color Legend

Create `MaintenanceColorLegend.razor`:

```razor
@using BlazorApp1.Services
@inject RecurringMaintenanceScheduler RecurringScheduler

<div style="padding: 16px; background: #f9f9f9; border-radius: 8px;">
    <h4 style="margin-bottom: 12px; font-weight: 600;">Task Type Colors</h4>
    
    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 8px;">
        @foreach (var type in TaskTypes)
        {
            var color = RecurringScheduler.GetTaskTypeColor(type);
            var name = RecurringScheduler.GetTaskTypeColorName(type);
            
            <div style="display: flex; align-items: center; gap: 8px;">
                <div style="width: 20px; height: 20px; border-radius: 4px; background: @color;"></div>
                <span style="font-size: 12px;">@name</span>
            </div>
        }
    </div>
</div>

@code {
    private List<string> TaskTypes = new()
    {
        "Preventive",
        "Corrective",
        "Predictive",
        "Inspection",
        "Emergency",
        "Routine"
    };
}
```

---

## Complete Integration Checklist

- [ ] Add using statements to `_Imports.razor`:
  ```razor
  @using BlazorApp1.Models
  @using BlazorApp1.Services
  ```

- [ ] Place new components in correct directory:
  - `MaintenanceScheduleViewer.razor`
  - `RecurringScheduleModal.razor`

- [ ] Add modal state to `MaintenancePlanning.razor`:
  - `showRecurringModal` bool
  - `selectedRecurringSchedule` object

- [ ] Add modal methods:
  - `ShowRecurringScheduleModal()`
  - `CloseRecurringModal()`

- [ ] Add modal to page markup
- [ ] Update recurring button in schedule table
- [ ] Test navigation to new page
- [ ] Test modal display
- [ ] Test weekend adjustments
- [ ] Verify color coding appears

---

## Testing the Integration

### Test 1: View Schedule Viewer
1. Navigate to `/rbm/maintenance-schedule-viewer`
2. Verify schedules load with colors
3. Expand a recurring schedule to see occurrences

### Test 2: Open Modal
1. Go to Maintenance Planning page
2. Find a recurring schedule
3. Click "🔄 Recurring" button
4. Verify modal displays with 10 occurrences

### Test 3: Weekend Adjustment
1. Create a schedule for Saturday or Sunday
2. Verify it adjusts to Friday or Monday
3. Check the "Adjusted to Weekday" indicator

### Test 4: Color Mapping
1. View different task types
2. Verify each type has correct color
3. Check legend matches displayed colors

---

## Troubleshooting

### Modal doesn't appear
- Check `showRecurringModal` is true
- Verify `RecurringScheduleModal.razor` path is correct
- Check browser console for JS errors

### Colors not showing
- Verify `RecurringScheduler` is injected
- Check schedule.Type matches mappings
- Use browser inspector to verify inline styles

### Weekend not adjusting
- Call `AdjustToWeekday()` before saving
- Test with known weekend dates
- Check date is actually a weekend

### Components not found
- Verify files are in `Components/Pages/RBM/` directory
- Check `_Imports.razor` has required usings
- Rebuild solution

---

## Summary

You now have:
- ✅ 2 new production-ready Razor components
- ✅ 5 implementation examples
- ✅ Complete integration instructions
- ✅ Testing procedures
- ✅ Troubleshooting guide

All components are designed to work together and provide a cohesive user experience for viewing and managing maintenance schedules with color-coded task types and automatic weekend adjustments.

