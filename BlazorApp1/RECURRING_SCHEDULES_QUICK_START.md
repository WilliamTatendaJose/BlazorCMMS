# Recurring Maintenance Schedules - Quick Start Guide

## What's New?

You can now automatically generate future maintenance schedules for repeating tasks. Create one schedule with a frequency, and the system generates multiple future occurrences automatically!

---

## Quick Start (30 Seconds)

### Step 1: Open Maintenance Planning
Navigate to `/rbm/maintenance-planning` or click **Maintenance Planning** in the menu.

### Step 2: Click "New Schedule"
Fill in the basic details:
- **Asset**: Select the equipment to maintain
- **Type**: Preventive, Corrective, or Inspection
- **Date/Time**: When to start
- **Technician**: Who will do the work
- **Duration**: How long it takes (hours)

### Step 3: Set Repeat Frequency
Scroll down in the modal and find "Repeat Frequency":
```
Choose one:
- Daily (every 1 day)
- Weekly (every 7 days)  
- BiWeekly (every 14 days)
- Monthly (every 30 days)
- Quarterly (every 90 days)
- Semi-Annual (every 180 days)
- Annually (every 365 days)
```

### Step 4: Specify How Many to Generate
"Generate Future Occurrences" field:
```
Examples:
- 12 = 12 months of monthly schedules
- 4 = 4 quarters of quarterly schedules  
- 52 = 1 year of weekly schedules
```

### Step 5: Save
Click **Save** button. System creates:
? The base schedule
? All future occurrences automatically
? Shows confirmation with count

---

## Real-World Examples

### Example 1: Monthly Oil Changes
```
Asset: Compressor A
Type: Preventive
Start: January 5, 2024
Frequency: Monthly
Generate: 12 occurrences
Technician: John Smith

Result: Oil changes scheduled for:
? Jan 5, Feb 5, Mar 5, Apr 5... Dec 5
```

### Example 2: Weekly Inspections
```
Asset: Safety Equipment
Type: Inspection  
Start: Monday, Jan 8, 2024
Frequency: Weekly
Generate: 26 occurrences
Technician: Mike Davis

Result: Inspections every Monday for 6 months
```

### Example 3: Annual Certification
```
Asset: Forklift
Type: Inspection
Start: Jan 15, 2024
Frequency: Annually
Generate: 5 occurrences
Technician: Sarah Jones

Result: Certifications scheduled:
? Jan 15, 2024
? Jan 15, 2025
? Jan 15, 2026
? Jan 15, 2027
? Jan 15, 2028
```

---

## View Recurring Schedule Info

Each recurring schedule has a ?? button to show:
- **Frequency**: How often it repeats
- **Next Due**: When the next occurrence is  
- **Days Until**: How many days away
- **Upcoming**: List of next 5 occurrences

Click the ?? button next to any recurring schedule in the table.

---

## How It Works Behind the Scenes

### Frequency Calculation
```
Base Schedule: January 15, 2024
Frequency: Monthly (30 days)

Generated Dates:
1. January 15, 2024 (day 0)
2. February 15, 2024 (day +30)
3. March 15, 2024 (day +60)
4. April 15, 2024 (day +90)
... and so on
```

### What Gets Copied
Each generated schedule inherits:
- ? Same asset
- ? Same maintenance type
- ? Same technician
- ? Same duration
- ? Similar description (with occurrence number)

### What's Different
Each occurrence:
- ? Has own database record
- ? Can be edited independently
- ? Can be rescheduled separately
- ? Has own status tracking

---

## Database Storage

All schedules stored in `MaintenanceSchedules` table:
```
ID | Asset    | ScheduledDate | Frequency | Status
1  | Motor A  | 2024-01-15    | Monthly   | Scheduled
2  | Motor A  | 2024-02-15    | Monthly   | Scheduled
3  | Motor A  | 2024-03-15    | Monthly   | Scheduled
...
```

Each is a separate, independent record.

---

## Frequency Guide

| Frequency | Days | When to Use |
|-----------|------|------------|
| **Daily** | 1 | Critical systems, emergency equipment |
| **Weekly** | 7 | Regular checks, filter inspections |
| **BiWeekly** | 14 | Lubrication, bearing checks |
| **Monthly** | 30 | Oil changes, calibration (MOST COMMON) |
| **Quarterly** | 90 | Major maintenance, deep cleaning |
| **Semi-Annual** | 180 | Comprehensive service, upgrades |
| **Annually** | 365 | Certifications, major overhauls |

### Recommended by Criticality

**Critical Assets** ? BiWeekly or Monthly
- Production-essential equipment
- Safety systems
- Backup components

**High Priority** ? Monthly
- Important machinery
- Support systems
- Auxiliary equipment

**Medium Priority** ? Quarterly
- General equipment
- Support machinery
- Infrequent use items

**Low Priority** ? Semi-Annual or Annual
- Storage equipment
- Rarely-used items
- Backup machinery

---

## Tips & Tricks

### ? DO

1. **Start Small**
   - Generate 3-6 months first
   - Add more later if needed
   - Avoid overwhelming the calendar

2. **Clear Descriptions**
   ```
   Good: "Monthly oil change - Valvoline 10W30"
   Bad: "Maintenance"
   ```

3. **Balanced Workload**
   - Check technician allocation
   - Distribute across team
   - Avoid overloading one person

4. **Set Reasonable Frequencies**
   - Monthly (30 days) for most work
   - Weekly for critical items
   - Quarterly for major tasks

### ? DON'T

1. **Excessive Occurrences**
   - Don't generate 52+ for monthly items
   - Stick to 12-24 typically

2. **Ignore Technician Load**
   - Check the Technician Allocation card
   - Reassign overloaded staff

3. **Vague Information**
   - Specify exactly what maintenance
   - Include part numbers or procedures

4. **Forget to Update**
   - Review schedules quarterly
   - Adjust frequency as needed
   - Add/remove as situations change

---

## Keyboard Shortcuts

| Action | Method |
|--------|--------|
| Create Schedule | Click "New Schedule" button |
| View Recurring Info | Click ?? button in Actions column |
| Edit Schedule | Click "Edit" button (if not completed) |
| Delete Schedule | Click "Delete" button |
| View Details | Click "View" button |
| Filter by Type | Use Type dropdown |
| Filter by Status | Use Status dropdown |
| Filter by Technician | Use Technician dropdown |

---

## Common Questions

### Q: Can I edit one occurrence without affecting others?
**A:** Yes! Each occurrence is independent. Edit one schedule without affecting others.

### Q: What if I need to skip a month?
**A:** Delete that specific schedule. Others remain unaffected.

### Q: Can I change the frequency after creating?
**A:** Yes, edit the base schedule's frequency. Consider regenerating future dates afterward.

### Q: How many should I generate?
**A:** Industry standard is 12 months (1 year). For critical items, do 24 months.

### Q: What if a technician is unavailable?
**A:** Edit schedules and reassign to another technician.

### Q: Can I see which dates are scheduled?
**A:** Click the ?? button to view next 5 occurrences, or check the table.

---

## Integration Points

### Links to Assets
When selecting an asset, the name auto-fills.

### Links to Technicians
Only "Technician" role users appear in assignment.

### Technician Allocation
View workload in the "Technician Allocation" section below the table.

### Auto-Schedule
"Auto-Schedule" button creates schedules for high-risk assets automatically.

### Exports
Export filtered schedules to Excel, Word, or PDF.

---

## For Administrators

### Enable/Disable Users
Only users with "Technician" role can be assigned.

### Change Frequencies
Review and update frequency patterns periodically.

### Monitor Workload  
Check the Technician Allocation card for balance.

### Archive Old Schedules
Completed schedules can be deleted to clean up old data.

---

## Support

### Need Help?
- Review the complete guide: `RECURRING_MAINTENANCE_SCHEDULES.md`
- Check specific examples in this guide
- Contact your system administrator

### Report Issues
- Build fails? Check `Program.cs` registration
- Schedules not generating? Verify frequency is set
- Can't see recurring button? Schedule needs frequency

---

## Summary

? **Key Features:**
- ? Set frequency (7 patterns)
- ? Auto-generate future dates
- ? View upcoming occurrences
- ? Edit individually
- ? Technician workload balancing
- ? Export capability
- ? Smart scheduling by risk

?? **Get Started:**
1. Go to Maintenance Planning
2. Click "New Schedule"
3. Fill details
4. Set frequency
5. Click Save
6. Done! All future schedules created automatically

**Happy scheduling!** ???
