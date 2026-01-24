# 🎨 Color Coding - Complete Visual Guide

## System Overview

```
┌──────────────────────────────────────────────────────────────┐
│                    MAINTENANCE PLANNING                      │
│                    Color-Coded System                        │
└──────────────────────────────────────────────────────────────┘

     RecurringMaintenanceScheduler Service
              (Color Mapping)
                    │
        ┌───────────┼───────────┐
        │           │           │
        ▼           ▼           ▼
   ┌────────┐  ┌────────┐  ┌──────────┐
   │Calendar│  │ Gantt  │  │ Schedule │
   │ View   │  │ Chart  │  │ Viewer   │
   └────────┘  └────────┘  └──────────┘
        │           │           │
        └───────────┴───────────┘
              All Color-Coded
```

---

## Color Palette

```
┌─────────────────────────────────────────────────────────────┐
│                    TASK TYPE COLORS                         │
├─────────────────────────────────────────────────────────────┤
│ 🟢 #4CAF50   GREEN       - Preventive Maintenance          │
│ 🟠 #FF9800   ORANGE      - Corrective Maintenance          │
│ 🔵 #2196F3   BLUE        - Predictive Maintenance          │
│ 🟣 #9C27B0   PURPLE      - Inspection                      │
│ 🔴 #F44336   RED         - Emergency                       │
│ 🔵 #00BCD4   CYAN        - Routine Maintenance             │
│ 🟠 #FF5722   DEEP ORANGE - Unscheduled                     │
│ ⚫ #607D8B    GREY        - Default/Other                  │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                    STATUS COLORS                            │
├─────────────────────────────────────────────────────────────┤
│ 🔵 #1565c0   BLUE        - Scheduled                       │
│ 🔴 #c62828   RED         - Overdue                         │
│ 🟢 #2e7d32   GREEN       - Completed                       │
│ 🟠 #FF9800   ORANGE      - In Progress                     │
└─────────────────────────────────────────────────────────────┘
```

---

## View Layouts

### 1. Calendar View

```
JANUARY 2025
┌─────────────────────────────────────────────────────────┐
│ Sun      Mon        Tue         Wed        Thu     Fri Sa│
├─────────────────────────────────────────────────────────┤
│  1                                                        │
│  2    🟢 Asset   🟠 Asset     🔵 Asset   🟣 Asset       │
│       Pump       Motor        Valve       Comp    +1more│
│  3                                                        │
│  4    🔴 Asset   🔵 Asset     🟢 Asset   🟠 Asset       │
│       Emerg      Rout         Prev        Corr           │
│  5    🟣 Asset   ⬜           ⬜          🔴 Asset       │
│       Insp                               Emerg           │
│                                                         │
│           [← Previous]  [Next →]                        │
└─────────────────────────────────────────────────────────┘
```

**Features**:
- Color-coded schedule boxes
- Shows 3 schedules per day
- White text for contrast
- Today has yellow background
- Month navigation

---

### 2. Gantt Chart View

```
┌──────────────┬──────────────────────────────────────┐
│ ASSET        │ D01 02 03 04 05 06 07 08 09 10 ... 30│
├──────────────┼──────────────────────────────────────┤
│ Pump-001     │ 🟢  ⬜  🟠  ⬜  🔵  ⬜  🟣  ⬜  🔴  ⬜│
│              │                                      │
│ Motor-002    │ ⬜  🟢  ⬜  🟠  ⬜  🔵  ⬜  🟣  ⬜  🟢│
│              │                                      │
│ Valve-003    │ 🔴  ⬜  🟡  ⬜  🟢  ⬜  ⬜  🟠  ⬜  ⬜│
│              │                                      │
│ Pump-004     │ ⬜  🟣  ⬜  ⬜  🔴  ⬜  🟢  ⬜  🟡  ⬜│
└──────────────┴──────────────────────────────────────┘

Legend:
🟢 Preventive   🟠 Corrective   🔵 Predictive   🟣 Inspection
🔴 Emergency    🟡 Routine      ⬜ No Schedule
```

**Features**:
- One row per asset
- One column per day
- 30-day timeline
- Color shows task type
- Hover for details

---

### 3. Schedule Viewer Page

```
┌────────────────────────────────────────────────────────┐
│  MAINTENANCE SCHEDULE VIEWER - Color Codes             │
├────────────────────────────────────────────────────────┤
│                  TASK TYPE COLORS                      │
│  🟢 Green (Preventive)    🟠 Orange (Corrective)      │
│  🔵 Blue (Predictive)     🟣 Purple (Inspection)      │
│  🔴 Red (Emergency)       🔵 Cyan (Routine)           │
└────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────┐
│ Schedule Cards (Color-Coded by Type)     │
├──────────────────────────────────────────┤
│                                          │
│  ┌─────────────────────────────────┐   │
│  │█ 🟢 PREVENTIVE    ✓ SCHEDULED   │   │
│  │                                  │   │
│  │  Pump Maintenance                │   │
│  │  📅 Mon, Jan 06, 2025 10:00     │   │
│  │  👤 John Smith      ⏱️ 4.5 hrs   │   │
│  │                                  │   │
│  │  🔄 Recurring: Weekly            │   │
│  │     Next in 7 days               │   │
│  │                                  │   │
│  │  [▶ View Next 5 Occurrences]    │   │
│  └─────────────────────────────────┘   │
│                                          │
│  ┌─────────────────────────────────┐   │
│  │█ 🟠 CORRECTIVE    ✓ SCHEDULED   │   │
│  │  Motor Repair                    │   │
│  │  📅 Tue, Jan 07, 2025 14:00     │   │
│  │  👤 Jane Doe       ⏱️ 6.0 hrs    │   │
│  └─────────────────────────────────┘   │
│                                          │
│  ┌─────────────────────────────────┐   │
│  │█ 🔵 PREDICTIVE    ✓ SCHEDULED   │   │
│  │  Vibration Analysis              │   │
│  │  📅 Wed, Jan 08, 2025 09:00     │   │
│  │  👤 Bob Wilson     ⏱️ 2.0 hrs    │   │
│  └─────────────────────────────────┘   │
│                                          │
└──────────────────────────────────────────┘
```

**Features**:
- Color-coded cards
- Left border = task type
- Badge = task type color
- Expandable timeline
- Statistics dashboard

---

### 4. Recurring Schedule Modal

```
┌──────────────────────────────────────────┐
│  RECURRING SCHEDULE: Pump Maintenance    │  🟢 Green gradient
├──────────────────────────────────────────┤
│                                          │
│  🟢 PREVENTIVE  │ Every 7 days  │ 4.5h  │
│  Next in 7 days                         │
│                                          │
│  Asset: Pump-001       Technician: John │
│  Last: Jan 06, 2025    Next: Jan 13     │
│                                          │
│  Description:                            │
│  Routine preventive maintenance of       │
│  primary circulation pump                │
│                                          │
│  ═══════════════════════════════════    │
│  📋 NEXT 10 OCCURRENCES                 │
│                                          │
│  ┌─────────────────────────────┐        │
│  │ ⊚ Jan 13, 2025             │        │ 🟢
│  │ 1 scheduled (7 days)        │        │
│  └─────────────────────────────┘        │
│                                          │
│  ┌─────────────────────────────┐        │
│  │ ⊚ Jan 20, 2025             │        │ 🟢
│  │ 2 scheduled (14 days)       │        │
│  └─────────────────────────────┘        │
│                                          │
│  ┌─────────────────────────────┐        │
│  │ ⊚ Jan 27, 2025             │        │ 🟢
│  │ 3 scheduled (21 days)       │        │
│  └─────────────────────────────┘        │
│                                          │
│           [Close]      [Edit]           │
└──────────────────────────────────────────┘
```

**Features**:
- Colorful gradient header
- Numbered occurrences
- Status indicators
- Color legend
- 10 occurrences shown

---

## Color Usage Patterns

### Pattern 1: Task Type Identification
```
At a glance, you can see:
🟢 Preventive tasks    (routine, planned)
🟠 Corrective tasks    (fixes needed)
🔵 Predictive tasks    (condition-based)
🟣 Inspections         (checkups)
🔴 Emergencies         (urgent!)
```

### Pattern 2: Workload Distribution
```
GREEN-heavy schedule   = More preventive work (good!)
RED-heavy schedule     = More emergencies (bad!)
MIXED colors           = Balanced workload (normal)
CLUSTERED colors       = Similar tasks together
SCATTERED colors       = Diverse task types
```

### Pattern 3: Planning Insights
```
All ORANGE on Monday   = Corrective backlog
Mostly GREEN spread out = Good preventive maintenance
Multiple RED           = Need emergency resources
Color patterns repeat  = Recurring schedules visible
```

---

## Implementation Examples

### How to Add Color to New Component

**Step 1: Inject the service**
```csharp
@inject RecurringMaintenanceScheduler RecurringScheduler
```

**Step 2: Get the color**
```csharp
var color = RecurringScheduler.GetTaskTypeColor(schedule.Type);
```

**Step 3: Apply to HTML**
```html
<div style="background: @color; color: white;">
    @schedule.Type
</div>
```

**Done!** The color is automatically applied.

---

## Customization Examples

### Example 1: Change Green to Teal
```csharp
// In RecurringMaintenanceScheduler.cs
public string GetTaskTypeColor(string taskType)
{
    return taskType?.ToLower() switch
    {
        "preventive" => "#008080",  // Changed from #4CAF50 to teal
        // ... rest unchanged
    };
}
```
**Result**: All green boxes become teal everywhere

### Example 2: Show More Days in Gantt
```csharp
// In MaintenancePlanning.razor
@for (int i = 0; i < 60; i++)  // Changed from 30 to 60
```
**Result**: Gantt shows 60 days instead of 30

### Example 3: Show More Schedules in Calendar
```csharp
// In MaintenancePlanning.razor
.Take(5)  // Changed from 3 to 5
```
**Result**: Calendar shows 5 schedules per day instead of 3

---

## User Experience Flow

```
User Opens Maintenance Planning Page
         │
         ├─ Clicks "Calendar"
         │  └─ Sees color-coded schedules by task type
         │     ├─ Green = Preventive (routine)
         │     ├─ Orange = Corrective (needs fix)
         │     ├─ Blue = Predictive (condition-based)
         │     └─ Red = Emergency (urgent!)
         │
         ├─ Clicks "Gantt"
         │  └─ Sees timeline with colored bars
         │     ├─ One bar per schedule
         │     ├─ Color shows task type
         │     ├─ Hover for details
         │     └─ Easily spots task patterns
         │
         └─ Navigates to "Schedule Viewer"
            └─ Sees detailed color-coded cards
               ├─ Each card bordered with task color
               ├─ Type badge matches color
               ├─ Expands to show occurrences
               └─ Color legend at top
```

---

## Benefits Summary

```
FOR USERS
┌─────────────────────────────────────┐
│ ✓ Quickly see task types           │
│ ✓ Spot patterns visually           │
│ ✓ Better planning decisions        │
│ ✓ Professional appearance          │
│ ✓ Easier to communicate            │
└─────────────────────────────────────┘

FOR MANAGERS
┌─────────────────────────────────────┐
│ ✓ Visual workload trends          │
│ ✓ Identify task type patterns     │
│ ✓ Better resource planning        │
│ ✓ Improved reporting              │
│ ✓ Professional dashboards         │
└─────────────────────────────────────┘

FOR DEVELOPERS
┌─────────────────────────────────────┐
│ ✓ Consistent colors everywhere    │
│ ✓ Easy to maintain & update       │
│ ✓ No code duplication             │
│ ✓ Reusable service                │
│ ✓ Simple customization            │
└─────────────────────────────────────┘
```

---

## Quick Reference Card

```
┌──────────────────────────────────────────┐
│           COLOR QUICK REFERENCE          │
├──────────────────────────────────────────┤
│ Task Type        Color    Hex    Meaning │
├──────────────────────────────────────────┤
│ Preventive       🟢      #4CAF50  Routine│
│ Corrective       🟠      #FF9800  Needs  │
│ Predictive       🔵      #2196F3  Smart  │
│ Inspection       🟣      #9C27B0  Check  │
│ Emergency        🔴      #F44336  URGENT │
│ Routine          🔵      #00BCD4  Normal │
│ Unscheduled      🟠      #FF5722  Unplan │
└──────────────────────────────────────────┘

┌──────────────────────────────────────────┐
│           VIEW QUICK REFERENCE           │
├──────────────────────────────────────────┤
│ Location         Feature      Color      │
├──────────────────────────────────────────┤
│ Calendar View    Boxes        Task Type  │
│ Gantt Chart      Bars         Task Type  │
│ Schedule Viewer  Cards        Task Type  │
│ Modal Dialog     Circles      Task Type  │
│ Status Badges    Chips        Status    │
└──────────────────────────────────────────┘
```

---

**Status**: ✅ COMPLETE & PRODUCTION READY

