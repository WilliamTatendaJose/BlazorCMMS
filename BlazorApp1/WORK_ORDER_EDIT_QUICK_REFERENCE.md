# Work Order Edit - Quick Reference

## Access the Edit Feature

1. Go to any Work Order detail page
2. Click the **?? Edit** button (top right)
3. Modal opens with editable form

## Who Can Edit?

| Role | Can Edit | Restrictions |
|------|----------|--------------|
| **Admin** | ? Any work order | None - all fields editable |
| **Engineer** | ? Any work order | Can edit type, job card fields |
| **Planner** | ? Any work order | Focused on scheduling/costs |
| **Supervisor** | ? Any work order | Same as Engineer |
| **Technician** | ?? Only own assignments | Cannot edit type |

## What Can Be Edited

### All Users Can Edit:
- Priority (Low/Medium/High/Critical)
- Description
- Due Date
- Location details (Building, Floor, Location)
- Contact information
- Estimated Cost

### Technicians Only:
- Work carried out details
- Completion notes
- Labor hours & actual downtime
- Artisan information
- Parts used

### Engineers/Supervisors/Admins:
- Work Order Type (affects classification)
- Job card fields
- Mechanical/Electrical flags
- Job type classification

### Admins Only:
- Any field without restrictions

## Edit Form Sections

1. **?? Basic Information** - Type, priority, category
2. **?? Scheduling** - Dates and downtime estimates
3. **?? Location & Contact** - Where and who
4. **?? Cost & Time** - Financial tracking
5. **?? Safety & Requirements** - Safety flags and permits
6. **?? Work Details** - Details of work done (tech/engineer)
7. **?? Job Card** - Engineering info (engineer/admin)

## Step-by-Step Usage

### To Edit a Work Order:

```
1. Navigate to Work Order Details
2. Click "?? Edit" button
3. Scroll through form sections
4. Update desired fields
5. Click "?? Save Changes"
6. See success message
7. Automatic refresh of display
```

### To Cancel:

```
1. Click "Cancel" button
2. Modal closes
3. No changes are saved
```

## What Happens When You Save?

- ? All changes saved to database
- ? `LastModifiedBy` updated with your name
- ? `LastModifiedDate` updated with current time
- ? Success notification appears
- ? Work order display automatically refreshes
- ? Edit modal closes

## Permission Error?

If you see: *"You don't have permission to edit this work order"*

- **Technician?** You can only edit work orders assigned to you
- **Admin?** You should be able to edit any work order
- **Check:** Is your role correct in the system?

## Tips & Tricks

? **Mobile Friendly** - Form adapts to phone/tablet screens  
? **Keyboard Navigation** - Use Tab to move between fields  
? **Auto-Focus** - First field automatically focused  
? **Clear Feedback** - Success/error messages auto-dismiss in 3 seconds  

## Form Field Tips

| Field Type | Notes |
|------------|-------|
| Text Input | Simple text entry |
| Number Input | Decimals allowed with steps |
| Date/Time | Calendar picker opens on click |
| Dropdown | Scroll to select from list |
| Checkbox | Click to toggle on/off |
| Textarea | Multi-line text, grows as you type |
| Money Fields | Shows $ prefix for clarity |

## Common Scenarios

### Scenario 1: Technician Completing Work
1. Click Edit on your assigned work order
2. Scroll to "?? Work Details" section
3. Fill in:
   - Details of Work Carried Out
   - Corrective Action (if any)
   - Actual labor hours
   - Artisan name
4. Save

### Scenario 2: Engineer Classifying Work
1. Click Edit on any work order
2. Scroll to "?? Basic Information"
3. Set:
   - Work Order Type
   - Category/Sub-Category
4. Scroll to "?? Job Card"
5. Mark:
   - Mechanical/Electrical work flags
   - Job type
6. Save

### Scenario 3: Planner Rescheduling
1. Click Edit on work order
2. Scroll to "?? Scheduling"
3. Update:
   - Due Date
   - Scheduled Start/End dates
   - Estimated Downtime
4. Scroll to "?? Cost & Time"
5. Update:
   - Estimated Cost
6. Save

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Tab` | Move to next field |
| `Shift+Tab` | Move to previous field |
| `Enter` | In textarea: New line; In form: Save (if focused on Save button) |
| `Escape` | Close modal |

## Troubleshooting

### Edit button is grayed out or missing
- You don't have permission for this work order
- Technicians: Make sure you're assigned to this work order
- Contact admin if you should have access

### Form won't save
- Check all required fields are filled
- Check internet connection
- Try refreshing the page and editing again
- Contact IT if problem persists

### Changes not showing
- Page auto-refreshes after save
- If not refreshed, refresh manually (F5)
- Make sure you clicked "Save Changes" (not "Cancel")

### Fields are disabled/grayed out
- Your role doesn't have permission for those fields
- Contact your supervisor if you need access

## Field Requirements

Required fields (must be filled before save):
- Most text fields have sensible defaults
- Date fields use current/future dates by default
- Cost fields default to 0

## Data Validation

- **Dates**: Must be valid dates (calendar picker enforces this)
- **Numbers**: Must be positive or zero
- **Text**: No special length restrictions
- **Email**: Must be valid email format if filled

## Data Privacy

- Only you and admins can see your edits
- All changes tracked with your username
- Previous values not visible in edit form (only current values)
- Use Notes field to explain why changes were made

## Limitations

?? **Cannot do in Edit Modal:**
- Cannot change work order status (use workflow buttons instead)
- Cannot add new spare parts (use separate Add Parts feature)
- Cannot see edit history (not implemented yet)
- Cannot undo changes (save is permanent)

## Best Practices

1. ? Save frequently as you work
2. ? Provide details in work descriptions
3. ? Fill in completion fields before saving
4. ? Update times accurately for labor tracking
5. ? Use contact info if assigning to someone
6. ?? Don't leave required fields empty
7. ?? Double-check before clicking Save

## Support

- ?? Email: [IT Support]
- ?? Phone: [IT Help Desk]
- ?? Report Bugs: [Bug Report Form]
- ? Questions: Ask your supervisor or trainer

---

**Last Updated:** December 2024  
**Version:** 1.0  
**Status:** Active
