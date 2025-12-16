# Recurring Maintenance Schedules - Complete Guide

## Overview

The Recurring Maintenance Schedules feature automatically generates future maintenance schedules based on frequency patterns. Once you create a schedule with a repeat frequency, the system automatically generates multiple future occurrences.

---

## Features

### ? Automatic Schedule Generation
When you create a maintenance schedule with a frequency, future occurrences are automatically generated:
- Create one schedule
- Set repeat frequency (Daily, Weekly, Monthly, etc.)
- System generates up to 52 future schedules automatically
- All configured with the same details as the base schedule

### ? Flexible Frequency Options
Choose from predefined frequency patterns:
- **Daily** - Every 1 day
- **Weekly** - Every 7 days
- **BiWeekly** - Every 14 days
- **Monthly** - Every 30 days
- **Quarterly** - Every 90 days
- **Semi-Annual** - Every 180 days
- **Annually** - Every 365 days

### ? Smart Scheduling
- Next scheduled dates automatically calculated
- Duration preserved across all occurrences
- Technician assignments maintained
- Description includes occurrence number
- All schedules linked to parent schedule

### ? Schedule Information Display
View future occurrences for any recurring schedule:
- Next scheduled date
- Days until next maintenance
- Overdue detection
- List of upcoming 5 occurrences
- Estimated duration and technician

---

## How to Use

### Creating a Recurring Schedule

#### Step 1: Click "New Schedule"
```
Click the "New Schedule" button in the Maintenance Planning page
```

#### Step 2: Fill Basic Details
```
- Asset: Select the asset
- Type: Preventive, Corrective, or Inspection
- Start Date & Time: When to start
- End Date & Time: When to finish (optional)
- Estimated Duration: In hours
- Technician: Who will perform the maintenance
- Description: What work will be done
```

#### Step 3: Set Repeat Frequency
```
Repeat Frequency: Choose from:
  - Daily
  - Weekly (every 7 days)
  - BiWeekly (every 14 days)
  - Monthly (every 30 days)
  - Quarterly (every 90 days)
  - Semi-Annual (every 180 days)
  - Annually (every 365 days)
  - Or leave blank for no repeat
```

#### Step 4: Configure Future Occurrences
```
Generate Future Occurrences: Enter a number (0-52)
  - 0: No automatic generation (manual only)
  - 1-52: Number of future schedules to create automatically

Examples:
  - 12: Creates 12 months of schedules (good for monthly maintenance)
  - 4: Creates 4 future schedules (good for quarterly maintenance)
  - 52: Creates 1 year of schedules (good for weekly maintenance)
```

#### Step 5: Save
```
Click "Save" to:
1. Create the base schedule
2. Automatically generate all future occurrences
3. Show confirmation with number of schedules created
```

### Viewing Recurring Schedule Information

#### Method 1: Click the ?? Button
```
In the maintenance schedules table:
1. Find a schedule with a frequency (shows in Frequency column)
2. Click the ?? button in the Actions column
3. View:
   - Frequency pattern
   - Next scheduled date
   - Days until next
   - Up to 5 upcoming occurrences
```

#### Method 2: Schedule Details Modal
```
1. Click "View" button on any schedule
2. See:
   - Asset name and type
   - Status and technician
   - Scheduled dates
   - Duration
```

### Managing Recurring Schedules

#### View All Occurrences
```
All generated schedules appear in the main table:
- Frequency column shows the pattern
- Each occurrence listed separately
- Can view, edit, or delete individually
```

#### Edit a Specific Occurrence
```
1. Find the schedule in the table
2. Click "Edit"
3. Change:
   - Assigned technician
   - Date/time
   - Duration
   - Description
4. Click "Save"

Note: Editing one occurrence doesn't affect others
```

#### Change Frequency for All Future
```
1. Edit the base schedule
2. Change the "Repeat Frequency"
3. System recalculates next scheduled dates
4. Can regenerate future schedules
```

#### Delete a Schedule
```
1. Find the schedule
2. Click "Delete"
3. Confirm deletion
4. Only that specific occurrence is deleted (others remain)
```

---

## Technical Details

### Behind the Scenes

When you create a recurring schedule, the system:

1. **Creates the Base Schedule**
   ```csharp
   var baseSchedule = new MaintenanceSchedule
   {
       AssetId = selectedAsset.Id,
       ScheduledDate = selectedDate,
       Type = "Preventive",
       Frequency = "Monthly",
       NextScheduledDate = selectedDate.AddDays(30),
       // ... other properties
   };
   ```

2. **Generates Future Occurrences**
   ```csharp
   // Creates schedules for:
   // Day 30, Day 60, Day 90, ... (based on frequency)
   ```

3. **Calculates Next Dates**
   ```csharp
   var frequencyDays = GetFrequencyDays("Monthly"); // Returns 30
   var nextDate = currentDate.AddDays(frequencyDays);
   ```

4. **Preserves All Details**
   - Same asset
   - Same technician
   - Same type
   - Same duration
   - Similar description with occurrence number

### Database Storage

Each generated schedule is stored as a separate `MaintenanceSchedule` record:

```
MaintenanceSchedules Table:
??????????????????????????????????????????????????????
? ID  ? Asset ? ScheduledDate? Status ? Frequency    ?
??????????????????????????????????????????????????????
? 1   ? Motor ? 2024-01-15   ? Schd.  ? Monthly      ?
? 2   ? Motor ? 2024-02-15   ? Schd.  ? Monthly      ?
? 3   ? Motor ? 2024-03-15   ? Schd.  ? Monthly      ?
? 4   ? Motor ? 2024-04-15   ? Schd.  ? Monthly      ?
? ... ? ...   ? ...          ? ...    ? Monthly      ?
??????????????????????????????????????????????????????
```

### Calculation Examples

#### Monthly Maintenance
```
Start: January 15, 2024
Frequency: Monthly (30 days)
Occurrences to generate: 12

Generated Dates:
1. January 15, 2024
2. February 15, 2024
3. March 15, 2024
4. April 15, 2024
5. May 15, 2024
6. June 15, 2024
7. July 15, 2024
8. August 15, 2024
9. September 15, 2024
10. October 15, 2024
11. November 15, 2024
12. December 15, 2024
```

#### Quarterly Maintenance
```
Start: January 1, 2024
Frequency: Quarterly (90 days)
Occurrences: 4

Generated Dates:
1. January 1, 2024
2. April 1, 2024 (+ 90 days)
3. July 1, 2024 (+ 90 days)
4. October 1, 2024 (+ 90 days)
```

#### Weekly Maintenance
```
Start: Monday, January 8, 2024
Frequency: Weekly (7 days)
Occurrences: 4

Generated Dates:
1. Monday, January 8, 2024
2. Monday, January 15, 2024
3. Monday, January 22, 2024
4. Monday, January 29, 2024
```

---

## Frequency Reference

| Frequency | Days | Use Case | Examples |
|-----------|------|----------|----------|
| **Daily** | 1 | Very critical items needing checks | Emergency systems, safety equipment |
| **Weekly** | 7 | Regular maintenance needed | Filter changes, inspections |
| **BiWeekly** | 14 | Moderate maintenance interval | Pump checks, lubrication |
| **Monthly** | 30 | Standard preventive maintenance | Oil changes, calibration |
| **Quarterly** | 90 | Major maintenance | Bearing inspection, deep clean |
| **Semi-Annual** | 180 | Large overhauls | Comprehensive service |
| **Annually** | 365 | Minimal maintenance items | Annual certification |

---

## Smart Recommendations

### By Asset Criticality

The system recommends frequencies based on asset importance:

```
Critical Assets ? BiWeekly (14 days)
  - Production line motors
  - Critical conveyors
  - Essential pumps

High Priority Assets ? Monthly (30 days)
  - Backup equipment
  - Important motors
  - Support machinery

Medium Priority Assets ? Quarterly (90 days)
  - General equipment
  - Auxiliary machinery

Low Priority Assets ? Semi-Annual (180 days)
  - Storage equipment
  - Rarely-used items
```

### By Asset Health Score

Maintenance frequency can be adjusted based on current condition:

```
Poor Health (< 40%)  ? Weekly maintenance
Fair Health (40-70%) ? Monthly maintenance
Good Health (70-90%) ? Quarterly maintenance
Excellent (> 90%)    ? Annually
```

---

## Common Scenarios

### Scenario 1: Annual Equipment Certification
```
Asset: Forklift A
Type: Inspection
Frequency: Annually (365 days)
Occurrences: 5 (covers next 5 years)
Start Date: January 15, 2024

Automatically creates:
? Jan 15, 2024
? Jan 15, 2025
? Jan 15, 2026
? Jan 15, 2027
? Jan 15, 2028
```

### Scenario 2: Monthly Oil Changes
```
Asset: Compressor B
Type: Preventive
Frequency: Monthly (30 days)
Occurrences: 12 (one year)
Start Date: January 5, 2024

Automatically creates:
? Jan 5, 2024
? Feb 5, 2024
? Mar 5, 2024
... and 9 more through December
```

### Scenario 3: Weekly Filter Checks
```
Asset: Air Filter Unit C
Type: Inspection
Frequency: Weekly (7 days)
Occurrences: 26 (6 months)
Start Date: January 8, 2024 (Monday)

Automatically creates:
? Jan 8 (Mon)
? Jan 15 (Mon)
? Jan 22 (Mon)
... every Monday through June
```

---

## Advanced Features

### View Upcoming Occurrences

Click the ?? button to see:
```
Frequency: Monthly (30 days)
Next scheduled: Feb 15, 2024 14:00
Days until next: 18
Status: On Schedule

Upcoming occurrences:
  • Feb 15, 2024 (18 days from now)
  • Mar 15, 2024 (48 days from now)
  • Apr 15, 2024 (79 days from now)
  • May 15, 2024 (109 days from now)
  • Jun 15, 2024 (140 days from now)
```

### Technician Workload Balancing

The system shows technician allocation:
```
John Smith - 8 assigned schedules
Mike Davis - 6 assigned schedules
Sarah Jones - 7 assigned schedules

Helps identify:
- Who is overloaded
- Who has capacity
- When to reassign
```

### Filter and Search

Filter recurring schedules by:
- **Type**: Preventive, Corrective, Inspection
- **Status**: Scheduled, In Progress, Completed
- **Technician**: By assigned technician
- **Frequency**: See all recurring items

---

## Best Practices

### ? DO

1. **Set Frequency on Initial Creation**
   ```
   Create with frequency from the start
   Generates all future dates at once
   ```

2. **Choose Realistic Occurrences**
   ```
   12-24 for annual plans
   4-8 for quarterly plans
   52 for weekly plans
   ```

3. **Document in Description**
   ```
   "Monthly oil change - Valvoline 10W30"
   "Quarterly bearing inspection - Check temperature"
   ```

4. **Review Technician Load**
   ```
   Ensure balanced workload
   Avoid overloading one technician
   ```

5. **Use Recommended Frequencies**
   ```
   Monthly (30 days) for most items
   Quarterly (90 days) for major work
   Weekly (7 days) for critical items
   ```

### ? DON'T

1. **Don't Create Excessive Occurrences**
   ```
   ? 52+ occurrences for monthly items (too much)
   ? 12-24 occurrences is usually enough
   ```

2. **Don't Leave Dates Ambiguous**
   ```
   ? "Maintenance needed"
   ? "Replace air filter and check belts"
   ```

3. **Don't Ignore Technician Workload**
   ```
   ? Assign all to one person
   ? Distribute across team
   ```

4. **Don't Set Unrealistic Frequencies**
   ```
   ? Daily maintenance for non-critical items
   ? Monthly for most preventive work
   ```

5. **Don't Forget to Update**
   ```
   ? Set frequency once and forget
   ? Review quarterly and adjust as needed
   ```

---

## Integration Points

### With Work Orders
When a maintenance schedule is completed:
```csharp
// In future enhancement:
schedule.Status = "Completed";
schedule.CompletedDate = DateTime.Now;

// Auto-create work order
var workOrder = new WorkOrder
{
    AssetId = schedule.AssetId,
    Type = schedule.Type,
    Description = schedule.Description,
    // ... other details
};
```

### With Asset Health Scoring
Maintenance frequency affects asset health:
```csharp
// Schedule frequency impacts predicted health
if (asset.HealthScore < 60 && frequency > 90)
{
    // Too infrequent for poor health
    // Recommend increasing frequency
}
```

### With Notifications
Receive alerts for:
- Upcoming scheduled maintenance
- Overdue schedules
- Technician assignments
- Completion reminders

---

## Troubleshooting

### "No recurring schedules generated"
**Cause**: Frequency not set or occurrences set to 0
**Fix**: Set frequency AND occurrences > 0

### "Dates seem wrong"
**Cause**: Frequency days calculation
**Fix**: Check frequency type (Monthly = 30 days, Weekly = 7 days)

### "Technician is overloaded"
**Cause**: All recurring schedules assigned to one person
**Fix**: Edit some schedules and reassign to other technicians

### "Can't edit future occurrences together"
**Cause**: Design - each schedule is independent
**Fix**: Edit each occurrence separately or create new series

---

## API Reference

### RecurringMaintenanceScheduler Class

#### Generate Recurring Schedules
```csharp
var scheduler = new RecurringMaintenanceScheduler(contextFactory);

var generatedSchedules = scheduler.GenerateRecurringSchedules(
    baseSchedule,
    numberOfOccurrences: 12
);
```

#### Create and Save
```csharp
int created = await scheduler.CreateRecurringSchedulesAsync(
    baseSchedule,
    numberOfOccurrences: 12
);
```

#### Get Scheduling Info
```csharp
var info = scheduler.GetSchedulingInfo(schedule);
// Returns: Frequency, NextDate, DaysUntil, IsOverdue, etc.
```

#### Get Future Occurrences
```csharp
var occurrences = scheduler.GetFutureOccurrences(schedule, count: 5);
// Returns: List of next 5 scheduled dates
```

#### Calculate Next Date
```csharp
var nextDate = scheduler.CalculateNextScheduleDate(
    currentDate,
    frequency: "Monthly"
);
```

---

## Summary

The Recurring Maintenance Schedules feature:

? **Saves Time** - Create one schedule, get 12+ automatically
? **Maintains Consistency** - Same details across all occurrences
? **Smart Scheduling** - Automatic date calculation
? **Flexible** - Supports 7 standard frequencies
? **Trackable** - Each occurrence tracked independently
? **Scalable** - Handles large maintenance programs
? **Integrated** - Works with assets, technicians, work orders

---

## Next Steps

1. **Create Your First Recurring Schedule**
   - Choose a frequent maintenance task
   - Set appropriate frequency
   - Generate 12 months of schedules

2. **Review Technician Load**
   - Check allocation in Technician Allocation section
   - Reassign overloaded technicians

3. **Monitor Completion**
   - Track status in the main table
   - Update as schedules complete

4. **Adjust as Needed**
   - Change frequency based on results
   - Add/remove technicians as workload changes

---

**Happy scheduling!** ???
