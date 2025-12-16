# Work Order Edit Feature - Testing Guide

## Overview

This guide provides comprehensive testing procedures for the Work Order Edit feature. Follow these tests to verify all functionality works correctly.

## Test Environment Setup

### Prerequisites
- Access to Blazor application
- Valid user accounts with different roles:
  - Admin account
  - Reliability Engineer account
  - Planner account
  - Supervisor account
  - Technician account (assigned to work orders)
  - Technician account (NOT assigned)
- Sample work orders in system
- Browser developer tools (optional but helpful)

### Browser Testing
Test on multiple browsers:
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (if available)
- Mobile browser (iOS/Android)

### Test Data
Create test work orders with:
- Different statuses (Open, In Progress, Completed)
- Different priorities
- Different types (Preventive, Corrective, etc.)
- Assigned and unassigned states

---

## Test Suite 1: Permission & Access Control

### Test 1.1: Admin Can See Edit Button
**Precondition:** Logged in as Admin user
**Steps:**
1. Navigate to Work Order detail page
2. Look for "?? Edit" button in action buttons
3. Click Edit button

**Expected Result:**
- ? Edit button is visible
- ? Edit button is enabled (not grayed out)
- ? Modal opens without error
- ? All form sections visible
- ? All fields are enabled (not grayed out)

**Failed?** ? Check CurrentUser.IsAdmin flag

---

### Test 1.2: Engineer Can See Edit Button
**Precondition:** Logged in as Reliability Engineer
**Steps:**
1. Navigate to any work order detail page
2. Look for Edit button
3. Click it

**Expected Result:**
- ? Edit button is visible
- ? Modal opens
- ? Type field is ENABLED
- ? Job Card section is visible
- ? Can edit any work order (not restricted by assignment)

**Failed?** ? Check role permissions in CanUserEditWorkOrder()

---

### Test 1.3: Technician Can Edit Own Work Order
**Precondition:** Logged in as Technician, viewing work order assigned to you
**Steps:**
1. Navigate to work order detail page
2. Verify "Assigned To" shows your name
3. Look for Edit button

**Expected Result:**
- ? Edit button is visible
- ? Modal opens
- ? Type field is DISABLED
- ? Job Card section is NOT visible
- ? Work Details section is visible
- ? Can edit work fields

**Failed?** ? Check technician role restrictions

---

### Test 1.4: Technician CANNOT Edit Others' Work Order
**Precondition:** Logged in as Technician, viewing work order assigned to someone else
**Steps:**
1. Navigate to work order detail page
2. Verify "Assigned To" shows different person
3. Look for Edit button

**Expected Result:**
- ? Edit button is NOT visible OR
- ? Edit button is grayed out OR
- ? Clicking it shows error: "You don't have permission"
- ? Modal does NOT open

**Failed?** ? Check assignment verification in CanUserEditWorkOrder()

---

### Test 1.5: Planner Can Edit Any Work Order
**Precondition:** Logged in as Planner
**Steps:**
1. Navigate to any work order detail page
2. Click Edit button

**Expected Result:**
- ? Edit button visible and enabled
- ? Modal opens
- ? Can edit scheduling/costs
- ? All planning fields accessible

**Failed?** ? Check Planner role in permissions

---

### Test 1.6: Supervisor Can Edit Any Work Order
**Precondition:** Logged in as Supervisor
**Steps:**
1. Navigate to any work order detail page
2. Click Edit button

**Expected Result:**
- ? Edit button visible
- ? Modal opens
- ? Same permissions as Engineer

**Failed?** ? Check Supervisor role setup

---

## Test Suite 2: Form Fields & Visibility

### Test 2.1: Technician - Field Visibility
**Precondition:** Technician logged in, modal open, assigned to work order
**Steps:**
1. Open edit modal
2. Check each section

**Expected Result:**

| Section | Visible | Notes |
|---------|---------|-------|
| Basic Info | ? | Type field disabled |
| Scheduling | ? | All editable |
| Location & Contact | ? | All editable |
| Cost & Time | ? | Actual fields enabled |
| Safety | ? | Checkboxes editable |
| Work Details | ? | ALL fields visible |
| Job Card | ? | Completely hidden |

**Failed?** ? Check role-based visibility in modal

---

### Test 2.2: Engineer - Field Visibility  
**Precondition:** Engineer logged in, modal open
**Steps:**
1. Check each section
2. Verify Type field is enabled
3. Verify Job Card section exists

**Expected Result:**

| Section | Visible | Notes |
|---------|---------|-------|
| Basic Info | ? | Type enabled |
| Scheduling | ? | All editable |
| Location & Contact | ? | All editable |
| Cost & Time | ? | All editable |
| Safety | ? | All editable |
| Work Details | ? | All fields visible |
| Job Card | ? | All fields visible |

**Failed?** ? Check role-based visibility

---

### Test 2.3: Admin - Field Visibility
**Precondition:** Admin logged in, modal open
**Steps:**
1. Open modal
2. Check every field is enabled

**Expected Result:**
- ? All sections visible
- ? No disabled fields
- ? All checkboxes functional
- ? All dropdowns functional
- ? All text inputs functional

**Failed?** ? Check admin role has no restrictions

---

### Test 2.4: Conditional Field Display - Safety Permit
**Precondition:** Modal open for any role
**Steps:**
1. Uncheck "Requires Safety Permit" checkbox
2. Notice Safety Permit Number field
3. Check the checkbox

**Expected Result:**
- ? When unchecked: Safety Permit Number field HIDDEN
- ? When checked: Safety Permit Number field VISIBLE
- ? Can enter permit number when visible

**Failed?** ? Check conditional rendering logic

---

### Test 2.5: Conditional Field Display - Housekeeping
**Precondition:** Modal open for Engineer or Admin
**Steps:**
1. Uncheck "Housekeeping Affected" checkbox
2. Notice Housekeeping Notes field
3. Check the checkbox

**Expected Result:**
- ? When unchecked: Notes field HIDDEN
- ? When checked: Notes field VISIBLE
- ? Can enter notes when visible

**Failed?** ? Check conditional housekeeping logic

---

## Test Suite 3: Data Binding & Input

### Test 3.1: Text Field Input
**Precondition:** Modal open
**Steps:**
1. Go to Description field
2. Clear existing text
3. Type: "Test description 12345"
4. Move to another field

**Expected Result:**
- ? Text appears as you type
- ? Text remains after leaving field
- ? Text displays correctly

**Failed?** ? Check @bind directive

---

### Test 3.2: Number Field Input
**Precondition:** Modal open
**Steps:**
1. Go to "Estimated Downtime" field
2. Clear it
3. Type: "5.5"
4. Tab to next field

**Expected Result:**
- ? Number appears
- ? Decimal accepted
- ? Value retained

**Failed?** ? Check number input binding

---

### Test 3.3: Date Field Input
**Precondition:** Modal open
**Steps:**
1. Click on Due Date field
2. Calendar picker opens
3. Select a date 7 days in future

**Expected Result:**
- ? Calendar opens
- ? Can select date
- ? Date appears in correct format
- ? Modal doesn't close

**Failed?** ? Check date input binding

---

### Test 3.4: Dropdown Selection
**Precondition:** Modal open, Assigned To field visible
**Steps:**
1. Click Assigned To dropdown
2. Select a technician from list
3. Click outside dropdown

**Expected Result:**
- ? Dropdown opens
- ? Shows list of technicians
- ? Selection appears in field
- ? Can't select non-technicians

**Failed?** ? Check dropdown filtering

---

### Test 3.5: Checkbox Toggle
**Precondition:** Modal open
**Steps:**
1. Find "Requires Shutdown" checkbox
2. Click it (toggle on)
3. Click it again (toggle off)

**Expected Result:**
- ? Checkbox toggles
- ? State persists
- ? Can toggle multiple times

**Failed?** ? Check checkbox binding

---

### Test 3.6: Textarea Input
**Precondition:** Modal open
**Steps:**
1. Go to Description or Work Details textarea
2. Type multi-line text:
   ```
   Line 1
   Line 2
   Line 3
   ```
3. Tab away

**Expected Result:**
- ? All lines entered
- ? Line breaks preserved
- ? Text retained after focus loss

**Failed?** ? Check textarea binding

---

## Test Suite 4: Save Functionality

### Test 4.1: Save Single Field Change
**Precondition:** Modal open with unsaved changes
**Steps:**
1. Change Priority to "Critical"
2. Leave other fields unchanged
3. Click "?? Save Changes"

**Expected Result:**
- ? Button shows "Saving..." temporarily
- ? Modal closes
- ? Success message appears: "Work order updated successfully"
- ? Work order detail updates showing new priority
- ? Success message disappears after 3 seconds

**Failed?** ? Check save handler

---

### Test 4.2: Save Multiple Field Changes
**Precondition:** Modal open
**Steps:**
1. Change:
   - Description: "Updated description"
   - Priority: "High"
   - Due Date: 5 days from now
   - Estimated Cost: 500
2. Click "?? Save Changes"

**Expected Result:**
- ? All changes saved
- ? Database updated
- ? Detail page shows new values
- ? Success message shown

**Failed?** ? Check save logic

---

### Test 4.3: Verify LastModified Fields
**Precondition:** Work order just saved
**Steps:**
1. Refresh page to see saved data
2. Check if possible to view LastModifiedBy and LastModifiedDate
3. Open modal again

**Expected Result:**
- ? LastModifiedBy shows current user name
- ? LastModifiedDate shows recent timestamp
- ? Previous values shown in modal

**Failed?** ? Check LastModified assignment

---

### Test 4.4: Save with Disabled Fields
**Precondition:** Technician with Type field disabled
**Steps:**
1. Open modal (Type field disabled)
2. Try to change other fields
3. Save

**Expected Result:**
- ? Can save without touching Type field
- ? Type field unchanged
- ? Other changes saved

**Failed?** ? Check disabled field handling

---

### Test 4.5: Multiple Sequential Saves
**Precondition:** Modal open
**Steps:**
1. Change Description
2. Save
3. Modal opens with new data
4. Change Priority
5. Save
6. Repeat 2 more times

**Expected Result:**
- ? Each save works
- ? No data loss
- ? Each change accumulates
- ? No errors on repeated saves

**Failed?** ? Check save reliability

---

## Test Suite 5: Cancel Functionality

### Test 5.1: Cancel Discards Changes
**Precondition:** Modal open with changes made
**Steps:**
1. Change Description to "Test"
2. Change Priority to "Critical"
3. Click Cancel

**Expected Result:**
- ? Modal closes
- ? No save message
- ? Work order detail unchanged
- ? Changes not applied

**Failed?** ? Check cancel handler

---

### Test 5.2: Cancel Multiple Times
**Precondition:** Modal open
**Steps:**
1. Edit fields
2. Cancel
3. Click Edit again
4. Edit different fields
5. Cancel

**Expected Result:**
- ? Modal opens each time
- ? Shows original data
- ? No accumulated changes
- ? Modal closes cleanly

**Failed?** ? Check modal reset

---

## Test Suite 6: UI/UX

### Test 6.1: Modal Positioning
**Precondition:** Modal open
**Steps:**
1. Check modal appears centered
2. Check modal doesn't go off-screen
3. Check backdrop behind modal is dark

**Expected Result:**
- ? Modal centered on screen
- ? Readable on all screen sizes
- ? Modal is on top (z-index correct)
- ? Can't click through backdrop

**Failed?** ? Check CSS z-index and positioning

---

### Test 6.2: Form Scrolling
**Precondition:** Modal open on small screen
**Steps:**
1. Scroll down through form sections
2. Scroll back up
3. Scroll to bottom

**Expected Result:**
- ? All fields accessible by scrolling
- ? Footer (Save/Cancel buttons) always visible
- ? Scroll is smooth
- ? Header stays at top

**Failed?** ? Check CSS overflow and height

---

### Test 6.3: Button States
**Precondition:** Modal open
**Steps:**
1. Look at Cancel button (should be white/outline style)
2. Look at Save button (should be blue/primary style)
3. Hover over each

**Expected Result:**
- ? Buttons have correct colors
- ? Buttons change on hover
- ? Buttons are clickable
- ? Clear visual distinction

**Failed?** ? Check button CSS

---

### Test 6.4: Loading State
**Precondition:** Modal open with changes
**Steps:**
1. Click Save
2. Observe button immediately

**Expected Result:**
- ? Button text changes to "Saving..."
- ? Button is disabled (grayed out)
- ? Can't click again
- ? Button re-enables after save

**Failed?** ? Check loading state logic

---

### Test 6.5: Focus Management
**Precondition:** Modal just opened
**Steps:**
1. Look which field has focus (should show outline)
2. Press Tab multiple times
3. Check all fields are reachable

**Expected Result:**
- ? First field auto-focused
- ? Tab moves through fields in order
- ? Tab reaches buttons at end
- ? Shift+Tab goes backward

**Failed?** ? Check focus and keyboard navigation

---

## Test Suite 7: Responsive Design

### Test 7.1: Desktop View (1200px+)
**Steps:**
1. Open modal on large screen
2. Check layout

**Expected Result:**
- ? Form shows 3 columns where applicable
- ? Buttons side-by-side
- ? All content visible without scrolling (except down)
- ? Nice spacing

**Failed?** ? Check desktop media queries

---

### Test 7.2: Tablet View (768px - 1199px)
**Steps:**
1. Resize browser to tablet size
2. Open modal
3. Check layout

**Expected Result:**
- ? Form shows 2 columns
- ? Still readable
- ? Buttons may stack
- ? Touch-friendly sizes

**Failed?** ? Check tablet media queries

---

### Test 7.3: Mobile View (<768px)
**Steps:**
1. Resize to mobile size or test on phone
2. Open modal
3. Scroll through form

**Expected Result:**
- ? Form shows 1 column
- ? Full width inputs
- ? Large touch targets
- ? Buttons stacked vertically
- ? All content accessible by scrolling

**Failed?** ? Check mobile media queries

---

### Test 7.4: Mobile Keyboard
**Precondition:** On mobile device
**Steps:**
1. Open modal
2. Tap on text field
3. Type with on-screen keyboard
4. Tab to next field

**Expected Result:**
- ? Keyboard appears
- ? Can type normally
- ? Field scrolls into view
- ? Navigation works

**Failed?** ? Check mobile browser behavior

---

## Test Suite 8: Error Handling

### Test 8.1: Network Error During Save
**Precondition:** Modal open with changes
**Steps:**
1. Disconnect network (or use dev tools)
2. Click Save
3. Wait for timeout

**Expected Result:**
- ? Error message appears
- ? Error is user-friendly
- ? Suggests retry or contact support
- ? Save button becomes re-clickable

**Failed?** ? Check error handler

---

### Test 8.2: Permission Denied Message
**Precondition:** Technician on other's work order OR unassigned technician
**Steps:**
1. Try to click Edit
2. If button shows error on click

**Expected Result:**
- ? Clear error message
- ? Message says "don't have permission"
- ? Modal doesn't open
- ? User understands they can't edit

**Failed?** ? Check permission error message

---

### Test 8.3: Invalid Data Error
**Precondition:** If form validation exists
**Steps:**
1. Try to save with invalid data
2. Observe response

**Expected Result:**
- ? Form shows validation errors (if implemented)
- ? Save prevented
- ? User knows what to fix
- ? Can correct and retry

**Note:** Current implementation doesn't have validation - this is for future enhancement

---

## Test Suite 9: Integration

### Test 9.1: Works with Existing Buttons
**Precondition:** Modal open, work order in Progress
**Steps:**
1. Save changes via Edit modal
2. Close modal
3. Try other workflow buttons (Complete, Hold, etc.)

**Expected Result:**
- ? Other buttons still work
- ? Status transitions work
- ? No conflicts
- ? Data is consistent

**Failed?** ? Check integration with workflow

---

### Test 9.2: Edit After Approval
**Precondition:** Work order just approved
**Steps:**
1. View work order detail
2. Click Edit
3. Make changes
4. Save

**Expected Result:**
- ? Can edit after approval (if role allows)
- ? Approval status preserved
- ? Changes applied correctly

**Failed?** ? Check workflow state handling

---

### Test 9.3: Edit Multiple Work Orders Sequentially
**Steps:**
1. Open work order #1, edit, save
2. Go back
3. Open work order #2, edit, save
4. Go back
5. Verify both changes persisted

**Expected Result:**
- ? Both work orders updated
- ? No data cross-contamination
- ? Correct data in each

**Failed?** ? Check data isolation

---

## Test Suite 10: Browser Compatibility

### Test 10.1: Chrome/Edge
**Steps:**
1. Test all above with Chrome/Edge

**Expected Result:**
- ? All tests pass

---

### Test 10.2: Firefox
**Steps:**
1. Test key scenarios with Firefox

**Expected Result:**
- ? Modal displays correctly
- ? Form works as expected
- ? Save completes

---

### Test 10.3: Safari
**Steps:**
1. Test on Safari if available

**Expected Result:**
- ? Modal functions correctly
- ? Date pickers work
- ? No layout issues

---

## Test Results Summary

Create a table like this to track results:

```
| Test ID | Test Name | Status | Notes | Date |
|---------|-----------|--------|-------|------|
| 1.1 | Admin Edit | PASS | | 12/1 |
| 1.2 | Engineer Edit | PASS | | 12/1 |
| 2.1 | Tech Fields | PASS | | 12/1 |
| ... | ... | ... | ... | ... |
```

---

## Known Issues

*[Leave empty until testing finds issues]*

---

## Sign-Off

- **Tested By:** [Name]
- **Test Date:** [Date]
- **Browser:** [List tested]
- **Overall Result:** ? PASS / ? FAIL
- **Ready for Deployment:** YES / NO

---

## Notes

- Test on multiple browsers for best results
- Test on mobile devices if possible
- Try to break things (negative testing)
- Document any unexpected behavior
- Report bugs clearly with steps to reproduce

---

**Good luck with testing!** ???
