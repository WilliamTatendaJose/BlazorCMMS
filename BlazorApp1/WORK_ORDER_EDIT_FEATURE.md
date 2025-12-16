# Work Order Edit Functionality - Implementation Guide

## Overview

The Work Order Edit feature allows users with appropriate permissions to edit work orders directly from the Work Order Detail page. The functionality includes role-based access control that restricts editing capabilities based on user roles.

## Features

### 1. **Edit Modal**
- Comprehensive form with multiple sections
- Real-time validation
- Responsive design for desktop and mobile
- Grouped form fields by category

### 2. **Role-Based Permissions**

#### Technician
- Can only edit work orders **assigned to them**
- Can update:
  - Work order status and priority
  - Description and details
  - Assigned location and contact information
  - Work carried out details
  - Completion information (labor hours, actual cost, downtime)
  - Parts used
  - Artisan information

#### Reliability Engineer
- Can edit **any work order**
- Can update:
  - All technician fields
  - Work order type (Preventive, Corrective, Predictive, Emergency)
  - Engineering job card fields
  - Job type and classification
  - Mechanical/Electrical work flags

#### Planner
- Can edit **any work order**
- Can update:
  - Scheduling information
  - Priority
  - Estimated costs and downtime
  - Resource assignment

#### Supervisor
- Can edit **any work order**
- Has same permissions as Reliability Engineer

#### Admin
- Can edit **any work order**
- Can update **all fields** without restrictions

### 3. **Edit Form Sections**

#### Basic Information
- Priority (Low, Medium, High, Critical)
- Type (Preventive, Corrective, Predictive, Emergency)
- Category and Sub-Category
- Description

#### Scheduling
- Due Date
- Estimated Downtime (hours)
- Scheduled Start/End Dates

#### Location & Contact
- Location details (Location, Building, Floor)
- Assigned Technician (dropdown filtered list)
- Department
- Contact Person, Phone, Email

#### Cost & Time
- Estimated Cost
- Actual Cost
- Actual Downtime (technician/engineer only)
- Labor Hours (technician/engineer only)

#### Safety & Requirements
- Requires Shutdown (checkbox)
- Lock Out Required (checkbox)
- Requires Safety Permit (checkbox)
- Safety Permit Number (conditional)

#### Work Details (Technician & Engineer Only)
- Details of Work Carried Out
- Corrective Action
- Completion Notes
- Artisan Name
- Parts Used

#### Job Card Information (Engineer & Admin Only)
- Job Number
- Job Type
- Mechanical Work (checkbox)
- Electrical Work (checkbox)
- Housekeeping Affected (checkbox)
- Housekeeping Notes (conditional)

## How to Use

### Opening the Edit Modal

1. Navigate to a Work Order Detail page
2. Click the **?? Edit** button in the top right
   - Button is only visible if user has edit permissions for that work order
   - For technicians, button appears only on work orders assigned to them

### Editing the Work Order

1. Modify the desired fields
2. Fields will vary based on your role
3. Some fields may be disabled for certain roles
4. Modified data is stored in component state (not yet saved)

### Saving Changes

1. Click **?? Save Changes** button
2. System will:
   - Update the `LastModifiedBy` field with current user name
   - Update the `LastModifiedDate` with current timestamp
   - Save to database
   - Show success message
   - Reload work order display

### Canceling

Click **Cancel** button to close the modal without saving changes.

## Permission Checking

The system performs permission checks at two levels:

### 1. UI Level
- Edit button only shows if user can edit
- Modal only opens if user has permission
- Error message shown if permission denied

### 2. Backend Level (Recommended)
- Add validation in `DataService.UpdateWorkOrder()` to verify permissions
- Prevents unauthorized edits via direct API calls

### Permission Logic in Code

```csharp
private bool CanUserEditWorkOrder()
{
    if (workOrder == null)
        return false;

    // Admin can always edit
    if (CurrentUser.IsAdmin)
        return true;

    // Technicians can only edit assigned work orders
    if (CurrentUser.Role == "Technician")
    {
        return workOrder.AssignedTo == CurrentUser.UserName;
    }

    // Engineers, Planners, Supervisors can edit any work order
    if (CurrentUser.Role == "Reliability Engineer" || 
        CurrentUser.Role == "Planner" || 
        CurrentUser.Role == "Supervisor")
    {
        return true;
    }

    return false;
}
```

## Component Structure

### Files Modified/Created

1. **BlazorApp1/Components/Pages/RBM/WorkOrderEditModal.razor**
   - Main edit form component
   - Handles form rendering and data binding

2. **BlazorApp1/Components/Pages/RBM/WorkOrderEditModal.razor.css**
   - Styling for edit modal
   - Form controls and layout styles

3. **BlazorApp1/Components/Pages/RBM/WorkOrderDetail.razor**
   - Updated to include edit modal integration
   - Added permission checking method
   - Added save handler

## Events and Callbacks

### OnSave Callback
Triggered when user clicks "Save Changes":
- Receives edited `WorkOrder` object
- Updates `LastModifiedBy` and `LastModifiedDate`
- Calls `DataService.UpdateWorkOrder()`
- Shows success message
- Reloads work order data

### OnCancel Callback
Triggered when user clicks "Cancel":
- Closes modal without saving
- Discards any changes

## Error Handling

The system includes error handling for:
- Permission denied (shows error message)
- Database save errors (shows error message with details)
- Missing data validation

Error messages display in a toast notification for 3 seconds.

## Field Validation

### Current Implementation
- Basic HTML5 validation
- Date/number fields use appropriate input types
- Required fields (marked with labels)

### Recommended Enhancements
- Add Blazor EditForm for comprehensive validation
- Implement custom validators for business logic
- Show validation errors in real-time
- Prevent save if validation fails

## Technical Details

### Data Binding
- Uses `@bind` directive for two-way data binding
- Creates copy of work order to avoid modifying original until save
- All properties properly copied to avoid reference issues

### State Management
- `showEditModal` - Controls modal visibility
- `editWorkOrder` - Working copy of work order being edited
- `technicians` - Filtered list of technician users
- `isSaving` - Prevents multiple submissions

### Responsive Design
- Mobile-optimized form layout
- Grid system adapts from 3 columns ? 2 columns ? 1 column
- Scrollable modal body for long forms
- Touch-friendly button sizes

## User Experience Features

1. **Loading State**
   - "Saving..." text while submitting
   - Button disabled during save

2. **Success Feedback**
   - Success toast message
   - Auto-dismiss after 3 seconds
   - Work order data reloaded automatically

3. **Error Feedback**
   - Error messages with details
   - Persistent until dismissed

4. **Field Organization**
   - Logically grouped sections
   - Clear section headers with icons
   - Related fields grouped together

## Security Considerations

1. **Role-Based Access**
   - Permissions checked before showing UI
   - Should also validate on backend

2. **Data Integrity**
   - `LastModifiedBy` and `LastModifiedDate` automatically tracked
   - Audit trail available in database

3. **Input Validation**
   - HTML5 validation on client
   - Should add server-side validation

## Future Enhancements

1. **Audit Logging**
   - Log all field changes
   - Track who changed what and when
   - Display change history

2. **Field-Level Permissions**
   - Different roles can edit different fields
   - More granular control

3. **Concurrent Edit Prevention**
   - Lock work order while being edited
   - Warn if another user is editing
   - Show last modifier info

4. **Change Summary**
   - Show what changed before saving
   - Allow user to review changes
   - Selective save (pick which fields to save)

5. **Workflow Integration**
   - Validate edits against current workflow state
   - Prevent invalid status transitions
   - Enforce required fields for each status

## Testing

### Test Scenarios

1. **Technician Tests**
   - ? Can edit own assigned work orders
   - ? Cannot edit other technician's work orders
   - ? Work details fields are editable
   - ? Type field is disabled

2. **Engineer Tests**
   - ? Can edit any work order
   - ? Type field is editable
   - ? Job card fields are editable
   - ? All sections visible

3. **Admin Tests**
   - ? Can edit any work order
   - ? All fields are editable and enabled
   - ? No permission restrictions

4. **Permission Tests**
   - ? Edit button hidden for unauthorized users
   - ? Error message shown when access denied
   - ? Modal doesn't open without permission

5. **Save Tests**
   - ? Changes are saved to database
   - ? LastModifiedBy updated with current user
   - ? LastModifiedDate updated with current time
   - ? Success message displays
   - ? Work order display updates

## Support and Maintenance

### Common Issues

**Issue**: Edit button not appearing
- **Solution**: Check user role and work order assignment

**Issue**: Modal won't save
- **Solution**: Check browser console for errors, verify database connectivity

**Issue**: Fields not updating
- **Solution**: Check for binding errors, verify property names

### Debugging

1. Check browser console for JavaScript errors
2. Check Visual Studio output for C# exceptions
3. Verify database connection and record existence
4. Check user permissions in CurrentUserService

## API Reference

### WorkOrderEditModal Component

```csharp
<WorkOrderEditModal 
    OriginalWorkOrder="workOrder"
    Technicians="technicianList"
    UserRole="@CurrentUser.Role"
    OnSave="SaveWorkOrderChanges"
    OnCancel="CancelEdit" />
```

**Parameters:**
- `OriginalWorkOrder` - The WorkOrder object to edit
- `Technicians` - List of User objects with Technician role
- `UserRole` - Current user's role (for permissions)
- `OnSave` - Callback when save clicked (WorkOrder)
- `OnCancel` - Callback when cancel clicked

### Permission Methods

```csharp
// Check if user can edit work order
bool CanUserEditWorkOrder()

// Save work order with modifications
Task SaveWorkOrderChanges(WorkOrder editedWorkOrder)

// Cancel edit without saving
void CancelEdit()
```

---

## Summary

The Work Order Edit functionality provides a secure, user-friendly way for technicians, engineers, and administrators to update work order information with:

- ? Role-based access control
- ? Comprehensive editing interface
- ? Real-time feedback
- ? Audit trail via LastModified fields
- ? Responsive design
- ? Error handling
- ? Permission validation

This implementation significantly improves the usability of the CMMS system by allowing field personnel to quickly update their assigned work orders without navigating through multiple pages.
