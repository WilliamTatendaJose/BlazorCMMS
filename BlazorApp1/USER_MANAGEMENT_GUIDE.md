# User Management & Profile System

## Overview

This implementation provides a production-ready user management and profile system using ASP.NET Core Identity with the following features:

- **User Management (Admin)** - Full CRUD operations for users
- **Profile Management** - Self-service profile editing
- **Password Management** - Change password, reset password
- **Passkeys** - Passwordless authentication support
- **Two-Factor Authentication** - Extra security layer
- **Role-Based Access Control** - 4 predefined roles with permissions

---

## Features

### 1. User Management (Admin Only)

**Location:** `/rbm/users`

**Capabilities:**
- ? Create new users with role assignment
- ?? Edit user profiles and roles
- ?? Lock/unlock user accounts
- ?? Reset passwords (generate new or send email)
- ??? Delete users
- ?? View user statistics

**Statistics Dashboard:**
- Total users count
- Active vs inactive users
- Locked accounts
- Users with 2FA enabled

### 2. User Profile (Self-Service)

**Locations:**
- `/rbm/profile` - RBM-styled profile page
- `/Account/Manage` - Standard Identity profile

**Features:**
- View profile information
- Edit full name, department, phone
- Access security settings
- View permissions based on role

### 3. Password Management

**Change Password:** `/Account/Manage/ChangePassword`
- Current password validation
- Password strength indicator
- Show/hide password toggles
- Real-time password match checking

**Password Reset (Admin):**
- Generate random secure password
- Send password reset email
- Invalidate current sessions

### 4. Passkeys (Passwordless Login)

**Location:** `/Account/Manage/Passkeys`

**Features:**
- Register multiple passkeys (up to 100)
- Support for:
  - ??? Fingerprint
  - ?? Face recognition
  - ?? Device PIN
  - ?? Security keys (USB/NFC)
- Rename and delete passkeys
- View registration dates

### 5. Two-Factor Authentication

**Location:** `/Account/Manage/TwoFactorAuthentication`

**Options:**
- Authenticator app setup
- Recovery codes
- Enable/disable 2FA

---

## Roles & Permissions

| Role | View | Edit | Delete | Users | FMEA | Work Orders |
|------|------|------|--------|-------|------|-------------|
| **Admin** | ? | ? | ? | ? | ? | ? |
| **Reliability Engineer** | ? | ? | ? | ? | ? | ? |
| **Planner** | ? | ? | ? | ? | ? | ? |
| **Technician** | ? | Limited | ? | ? | ? | ? |

---

## Services

### UserManagementService

```csharp
// Get all users
var users = await UserService.GetAllUsersAsync();

// Create user
var (success, errors, userId) = await UserService.CreateUserAsync(model);

// Update user
var (success, errors) = await UserService.UpdateUserAsync(model);

// Delete user
var (success, errors) = await UserService.DeleteUserAsync(userId);

// Reset password
var (success, errors, newPassword) = await UserService.AdminResetPasswordAsync(userId);

// Lock/Unlock
await UserService.LockUserAsync(userId);
await UserService.UnlockUserAsync(userId);

// Get statistics
var stats = await UserService.GetUserStatisticsAsync();
```

### RolePermissionService

```csharp
// Get current user's role
var role = await RolePermissionService.GetCurrentUserRoleAsync();

// Check permissions
var canEdit = await RolePermissionService.CanEditAsync();
var canDelete = await RolePermissionService.CanDeleteAsync();
var canManageUsers = await RolePermissionService.CanManageUsersAsync();
```

---

## Email Configuration

Configure SMTP in `appsettings.json`:

```json
{
  "Email": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-username",
    "SmtpPassword": "your-password",
    "FromEmail": "noreply@rbmcmms.com",
    "FromName": "RBM CMMS"
  }
}
```

**Note:** If SMTP is not configured, emails are logged to console (development mode).

---

## Security Settings

### Password Policy
- Minimum 6 characters
- Requires uppercase letter
- Requires lowercase letter
- Requires digit
- Special characters optional

### Account Lockout
- 5 failed attempts triggers lockout
- 15-minute lockout duration
- Admins can manually lock/unlock

### Session Security
- Automatic session refresh on profile changes
- Security stamp validation
- Cookie-based authentication

---

## Default Users

Created by `IdentityDataSeeder`:

| Email | Password | Role |
|-------|----------|------|
| admin@company.com | Admin123! | Admin |
| sarah.johnson@company.com | Sarah123! | Reliability Engineer |
| emily.brown@company.com | Emily123! | Planner |
| john.smith@company.com | John123! | Technician |
| mike.davis@company.com | Mike123! | Technician |

---

## Navigation

### Sidebar Links
- ?? My Profile ? `/rbm/profile`
- ?? User Management ? `/rbm/users` (Admin only)

### Profile Quick Actions
- ?? Change Password
- ?? Email Settings
- ??? Two-Factor Auth
- ?? Passkeys
- ?? Personal Data

---

## Files Created/Modified

### New Files
- `Services/UserManagementService.cs` - User management operations
- `Components/Pages/RBM/MyProfile.razor` - RBM profile page

### Modified Files
- `Services/RolePermissionService.cs` - Fixed role detection
- `Components/Pages/RBM/UserManagement.razor` - Enhanced with Identity
- `Components/Account/Pages/Manage/Index.razor` - Enhanced UI
- `Components/Account/Pages/Manage/ChangePassword.razor` - Enhanced UI
- `Components/Account/Pages/Manage/Passkeys.razor` - Enhanced UI
- `Components/Layout/RBMLayout.razor` - Added profile link
- `Program.cs` - Registered UserManagementService

---

## Usage Examples

### Create a New User (Admin)
1. Navigate to `/rbm/users`
2. Click "? Add User"
3. Fill in: Name, Email, Password, Role, Department
4. Click "Create User"

### Reset User Password (Admin)
1. Navigate to `/rbm/users`
2. Find user in list
3. Click ?? (key icon)
4. Choose: Generate new password OR Send reset email

### Add a Passkey (User)
1. Navigate to `/rbm/profile` or `/Account/Manage`
2. Click "Passkeys"
3. Click "Add a New Passkey"
4. Follow browser prompts (fingerprint, face, etc.)
5. Name your passkey

### Change Password (User)
1. Navigate to profile
2. Click "Change Password"
3. Enter current password
4. Enter new password (check strength indicator)
5. Confirm new password
6. Click "Update Password"

---

## Best Practices

1. **Always use HTTPS** in production
2. **Configure real SMTP** for password resets
3. **Encourage 2FA** for admin users
4. **Regular password rotation** for service accounts
5. **Monitor failed login attempts**
6. **Review user access** periodically

---

## Troubleshooting

### User Can't Login
1. Check if account is locked
2. Verify email is confirmed
3. Check IsActive status
4. Review failed access count

### Password Reset Not Working
1. Check SMTP configuration
2. Verify user email exists
3. Check email logs in development

### Passkey Not Working
1. Ensure HTTPS is enabled
2. Check browser compatibility
3. Verify device supports WebAuthn

---

**Your user management system is production-ready!** ??
