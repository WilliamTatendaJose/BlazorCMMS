# Quick Reference - Login, Email & Logout

## ?? Login Page

**URL:** `/Account/Login`

**Features:**
- RBM CMMS branded design
- Demo credentials displayed
- Mobile responsive
- No default layout

**Demo Accounts:**
```
Admin:      admin@company.com / Admin123!
Engineer:   sarah.johnson@company.com / Sarah123!
Planner:    emily.brown@company.com / Emily123!
Technician: john.smith@company.com / John123!
```

---

## ?? Email Configuration

### Development (No SMTP)
Leave `SmtpServer` empty in `appsettings.json`:
```json
{
  "Email": {
    "SmtpServer": "",
    "SmtpPort": "587",
    "SmtpUsername": "",
    "SmtpPassword": "",
    "FromEmail": "noreply@rbmcmms.com",
    "FromName": "RBM CMMS"
  }
}
```
Emails will be **logged to console** instead of sent.

### Production (With SMTP)
Configure real SMTP server:
```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@rbmcmms.com",
    "FromName": "RBM CMMS"
  }
}
```

### Email Types Sent:
- Email confirmation (on registration)
- Password reset link
- Password reset code

---

## ?? Logout

### Desktop
- **Location**: Top-right corner of topbar
- **Appearance**: Red-bordered button with ?? icon
- **Text**: "Logout"

### Mobile
- **Location**: Mobile header (next to profile)
- **Appearance**: Icon-only button ??
- **Touch-friendly**: 36px minimum

### How It Works:
```html
<form action="/Account/Logout" method="post">
    <AntiforgeryToken />
    <input type="hidden" name="ReturnUrl" value="/" />
    <button type="submit">?? Logout</button>
</form>
```

---

## ?? Files Modified

1. **Login.razor** - Redesigned UI
2. **AccountLayout.razor** - Created (no nav)
3. **EmailSender.cs** - Created (custom emails)
4. **RBMLayout.razor** - Added logout buttons
5. **Program.cs** - Registered EmailSender
6. **appsettings.json** - Added email config
7. **rbm-styles.css** - Added logout button styles

---

## ?? Quick Test

### Test Login:
```
1. Go to /Account/Login
2. Use admin@company.com / Admin123!
3. Should redirect to /rbm
```

### Test Logout:
```
1. Login first
2. Click "?? Logout" (top-right)
3. Should redirect to home
4. Should be logged out
```

### Test Email (Dev):
```
1. Try to register
2. Check console for email log
3. Should see "Email would be sent to..."
```

---

## ?? Colors Used

- Header gradient: #0288d1 to #37474f
- Button primary: #0288d1
- Logout button: #e53935 (red)
- Background: #eceff1

---

## ? Checklist

- [x] Login page redesigned
- [x] Demo credentials visible
- [x] Email sender implemented
- [x] Logout button on desktop
- [x] Logout button on mobile
- [x] AccountLayout created
- [x] Mobile responsive
- [x] Email config in appsettings.json

---

**Everything is ready to use!** ??
