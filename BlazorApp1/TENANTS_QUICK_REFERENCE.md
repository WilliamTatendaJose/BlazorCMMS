# Tenants Feature - Quick Reference Guide ??

## Feature Overview

The **Tenants Management** page is a SuperAdmin-only feature for managing multi-tenant configurations in the RBM CMMS system.

**URL:** `/rbm/tenants`  
**Access:** SuperAdmin role required  
**Status:** ? Production Ready

---

## Key Features

### 1. View All Tenants
- Card-based grid layout
- Shows tenant name and code
- Displays active/inactive status
- Shows contact information
- Displays user count and limits
- Creation date shown

### 2. Create New Tenant
- Unique tenant code (cannot be changed)
- Tenant name (required)
- Contact information (person, email, phone)
- Address information (optional)
- Resource limits (users, assets, documents)
- Default limits: 10 users, 100 assets, 500 documents

### 3. Edit Tenant
- Update all tenant details except tenant code
- Modify contact information
- Update resource limits
- Change description

### 4. Manage Users
- Navigate to tenant users page
- Add/remove users from tenant
- Set tenant admin status

### 5. Deactivate/Activate Tenant
- Toggle tenant status
- Users cannot access deactivated tenants
- Can reactivate anytime

### 6. Delete Tenant
- Permanent deletion
- Removes all associated data
- Confirmation required

---

## User Interface

### Page Layout
```
Header
??? Title: "Tenant Management"
??? Create New Tenant button
?
Alerts (if any)
??? Error messages (red)
??? Success messages (green)
?
Content
??? Loading spinner (if loading)
??? Tenant cards (if tenants exist)
??? Empty state (if no tenants)
```

### Tenant Card
```
Card
??? Header (Blue background)
?   ??? Tenant name
?   ??? Tenant code
?   ??? Status badge (Active/Inactive)
?
??? Body
?   ??? Contact Person
?   ??? Contact Email
?   ??? User count (current/max)
?   ??? Max assets
?   ??? Max documents
?   ??? Created date
?
??? Footer (Action buttons)
    ??? Edit
    ??? Users
    ??? Deactivate/Activate
    ??? Delete
```

---

## Common Tasks

### Create a New Tenant
1. Click "Create New Tenant" button
2. Fill in required fields:
   - Tenant Code (unique identifier)
   - Tenant Name
   - Max Users, Assets, Documents
3. Add optional contact information
4. Click "Save Tenant"

### Edit Tenant Details
1. Click "Edit" button on tenant card
2. Update desired fields
3. Note: Tenant code cannot be changed
4. Click "Save Tenant"

### Add Users to Tenant
1. Click "Users" button on tenant card
2. Navigate to tenant users page
3. Add users to tenant
4. Optionally set admin status

### Deactivate a Tenant
1. Click the deactivate button (warning icon)
2. Confirm action in dialog
3. Users will not be able to access
4. Can reactivate later

### Delete a Tenant
1. Click "Delete" button on tenant card
2. Confirm in deletion dialog
3. Action is permanent
4. All associated data is removed

---

## Validation Rules

### Required Fields
- **Tenant Code:** Must be unique, max 50 chars
- **Tenant Name:** Required, max 200 chars
- **Max Users:** Min 1, max 10,000
- **Max Assets:** Min 1, max 100,000
- **Max Documents:** Min 1, max 1,000,000

### Optional Fields
- Contact Person (max 100 chars)
- Contact Email (must be valid email format)
- Contact Phone (max 20 chars)
- Address, City, Country, Postal Code
- Description (max 500 chars)

---

## Error Messages

### Common Errors

**"Failed to load tenants"**
- Database connection issue
- Check network connectivity
- Try refreshing the page

**"Tenant Code is required"**
- Enter a unique tenant code
- Cannot be changed after creation

**"Tenant Name is required"**
- Enter a valid tenant name

**"The tenant code may already exist"**
- Choose a different unique code
- Tenant codes must be unique

**"Failed to create tenant"**
- Check all required fields are filled
- Verify database is accessible

**"Failed to delete tenant"**
- Tenant may still have users
- Try removing users first
- Check database connectivity

---

## Best Practices

### For SuperAdmins

1. **Resource Limits**
   - Set realistic limits based on needs
   - Account for growth
   - Monitor usage regularly

2. **Tenant Codes**
   - Use descriptive codes (e.g., ACME_2024)
   - Follow company naming convention
   - Cannot be changed later

3. **Contact Information**
   - Keep contact info up-to-date
   - Include backup contact
   - Verify email addresses

4. **User Management**
   - Review users quarterly
   - Remove inactive users
   - Update admin roles as needed

5. **Deactivation**
   - Deactivate instead of delete if possible
   - Preserves historical data
   - Can reactivate if needed

6. **Monitoring**
   - Monitor resource usage
   - Alert tenants approaching limits
   - Plan capacity ahead

---

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `Tab` | Navigate between buttons |
| `Enter` | Activate focused button |
| `Escape` | Close modal (if open) |

---

## Accessibility Features

? Keyboard navigation (Tab, Enter, Escape)  
? Color + text status indicators  
? ARIA labels on buttons  
? Semantic HTML structure  
? Readable font sizes  
? Good color contrast

---

## Troubleshooting

### Page Won't Load
1. Verify you're logged in as SuperAdmin
2. Check browser console (F12) for errors
3. Try refreshing the page
4. Check network connectivity

### Modal Won't Close
1. Click outside the modal
2. Click the X button
3. Click Cancel button
4. Refresh page if stuck

### Button Disabled During Save
1. This is normal - wait for operation to complete
2. Spinner shows progress
3. Button re-enables when done
4. Check for error message

### Tenant Data Not Updating
1. Check for error messages
2. Verify all required fields filled
3. Try saving again
4. Refresh page to reload data

### Can't Delete Tenant
1. Some tenants may have dependent data
2. Try removing users first
3. Check error message for details
4. Contact support if persistent

---

## Performance Tips

- Page loads all tenants at once
- For large lists, add filtering
- Bulk operations available via API
- Use keyboard shortcuts to navigate faster

---

## Security Reminders

?? Only SuperAdmins can access this page  
?? Tenant codes are unique and permanent  
?? Deletions are permanent and cannot be undone  
?? Deactivated tenants' users lose access immediately  

---

## Related Features

- **Tenant Users Page:** `/rbm/tenant-users/:tenantId`
  - Manage users in a specific tenant
  - Add/remove users
  - Set admin roles

- **Multi-Tenancy System**
  - Role-based access control
  - Tenant data isolation
  - User-tenant mapping

---

## Support

**Need Help?** See the full documentation:
- Technical Guide: TENANTS_PRODUCTION_READY.md
- Architecture: MULTI_TENANCY_ARCHITECTURE.md
- Troubleshooting: TENANT_MANAGEMENT_TROUBLESHOOTING.md

---

## Feature Status

```
? Create Tenants:       Working
? Edit Tenants:         Working
? Delete Tenants:       Working
? Activate/Deactivate:  Working
? User Management:      Working
? Form Validation:      Working
? Error Handling:       Working
? Loading States:       Working
? Confirmation Dialogs: Working
```

---

**Status:** ? **PRODUCTION READY**  
**Last Updated:** 2024-12-20  
**Version:** 1.0.0
