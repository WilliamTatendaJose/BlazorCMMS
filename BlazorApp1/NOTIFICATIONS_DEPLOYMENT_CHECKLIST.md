# Notifications System - Deployment Checklist

## ?? Pre-Deployment (Development)

- [x] Database migration created
- [x] NotificationSettings model implemented
- [x] NotificationLog model implemented
- [x] NotificationService implemented (325+ lines)
- [x] Current user ID exposed in CurrentUserService
- [x] Service registered in Program.cs
- [x] Settings page updated with real saving
- [x] Email templates created (4 types)
- [x] Validation implemented
- [x] Error handling added
- [x] Build successful ?

## ?? Database Migration

- [ ] Review migration script: `20251220_AddNotificationTables.cs`
- [ ] Backup existing database
- [ ] Run `dotnet ef database update` (or automatic on app start)
- [ ] Verify tables created in SQL Server
- [ ] Test rollback if needed

**SQL to verify**:
```sql
-- Check tables exist
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('NotificationSettings', 'NotificationLogs');

-- Check indexes
SELECT * FROM sys.indexes 
WHERE object_id = OBJECT_ID('NotificationSettings');
```

## ?? Email Configuration

### For Development (No SMTP Required)
- [x] Leave `SmtpServer` empty in appsettings.json
- [x] Test by checking console output
- [ ] Test by navigating to Settings page
- [ ] Test by clicking Save Preferences
- [ ] Verify output in Application window

### For Production (Real Emails)

**Choose an SMTP Provider**:
- [ ] Gmail (easy, limited volume)
- [ ] SendGrid (professional, free tier available)
- [ ] AWS SES (cheap, requires AWS account)
- [ ] Office 365 (if using Microsoft)
- [ ] Custom SMTP server

**Get Credentials**:
- [ ] SMTP Server address
- [ ] SMTP Port (usually 587 or 465)
- [ ] Username/Email
- [ ] Password/API Key

**Update appsettings.json**:
```json
{
  "Email": {
    "SmtpServer": "smtp.your-provider.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-username",
    "SmtpPassword": "your-password-or-apikey",
    "FromEmail": "noreply@rbmcmms.com",
    "FromName": "RBM CMMS"
  }
}
```

**Use User Secrets (Development)**:
```bash
dotnet user-secrets set "Email:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "Email:SmtpUsername" "your-email@gmail.com"
dotnet user-secrets set "Email:SmtpPassword" "your-app-password"
```

**Use Key Vault (Production)**:
- [ ] Create Azure Key Vault
- [ ] Store SMTP credentials securely
- [ ] Update appsettings.json to reference vault
- [ ] Grant app managed identity access

## ?? Testing Phase 1: Development

- [ ] Create test admin user account
- [ ] Login as admin
- [ ] Navigate to `/rbm/settings`
- [ ] Locate "Notification Preferences" section
- [ ] Configure test settings:
  - [x] Critical alerts: ON
  - [x] Work order due: ON
  - [x] Maintenance: OFF
  - [x] Weekly report: OFF
  - [ ] Phone: (optional)
  - [ ] SMS: OFF
- [ ] Click "?? Save Preferences"
- [ ] Check "Last saved" timestamp updates
- [ ] Check console for email log message
- [ ] Verify message shows correct preferences

## ?? Testing Phase 2: Database Verification

**Check NotificationSettings**:
```sql
SELECT * FROM NotificationSettings 
WHERE UserId = 'test-user-id';

-- Expected: 1 row with your preferences
```

**Check NotificationLogs** (if any sent):
```sql
SELECT TOP 10 * FROM NotificationLogs 
ORDER BY CreatedDate DESC;

-- Should be empty initially
```

## ?? Testing Phase 3: Production Email (Optional)

- [ ] Configure real SMTP credentials
- [ ] Create test email address
- [ ] Create test user with that email
- [ ] Login as test user
- [ ] Go to Settings page
- [ ] Save notification preferences
- [ ] Check test email inbox
- [ ] Verify email received and formatted correctly
- [ ] Check NotificationLogs for "Sent" status
- [ ] Verify error message is empty

## ?? Testing Phase 4: Programmatic Notifications

**Test sending programmatically**:
```csharp
// In a test controller/component
var testUserId = "test-user-id";

// Test critical alert
await notificationService.SendCriticalAlertAsync(
    userId: testUserId,
    assetName: "Test-Asset",
    alertMessage: "This is a test critical alert",
    assetId: 1
);

// Verify in database
SELECT * FROM NotificationLogs 
WHERE UserId = 'test-user-id' 
ORDER BY CreatedDate DESC;
```

Expected results:
- [ ] Status: "Sent" (production) or "Pending" (dev)
- [ ] Channel: "Email"
- [ ] RecipientAddress: test user's email
- [ ] CreatedDate: recent timestamp

## ?? Security Checklist

- [ ] SMTP password not in source code
- [ ] Use User Secrets (dev) or Key Vault (prod)
- [ ] SSL/TLS enabled for SMTP (port 587 or 465)
- [ ] Admin-only access to Settings page ?
- [ ] Phone number validation working ?
- [ ] User can only modify their own settings
- [ ] SMTP credentials not logged
- [ ] Error messages don't expose sensitive info

## ?? Functional Testing

### Test Quiet Hours
```csharp
// Set quiet hours: 6 PM - 8 AM
var settings = await notificationService.GetNotificationSettingsAsync(userId);
settings.QuietHoursStart = new TimeOnly(18, 0);  // 6 PM
settings.QuietHoursEnd = new TimeOnly(8, 0);    // 8 AM
await notificationService.SaveNotificationSettingsAsync(settings);

// At 10 PM, send notification (should skip)
// Check NotificationLogs: status should be "Skipped"
```

### Test Disabled Notifications
```csharp
// Disable critical alerts
var settings = await notificationService.GetNotificationSettingsAsync(userId);
settings.EmailCriticalAlerts = false;
await notificationService.SaveNotificationSettingsAsync(settings);

// Send critical alert (should skip)
// Check NotificationLogs: status should be "Skipped"
```

### Test SMS Validation
- [ ] Valid: "+1-555-123-4567" ?
- [ ] Valid: "5551234567" ?
- [ ] Invalid: "555-1234" (too short) ?
- [ ] Invalid: "abcdefghij" (non-digits) ?

### Test All Notification Types
- [ ] Critical alert email
- [ ] Work order reminder email
- [ ] Maintenance schedule email
- [ ] Weekly report email
- [ ] SMS (if provider integrated)

## ?? Performance Testing

- [ ] Query NotificationLogs with 1000+ rows (< 100ms)
- [ ] Save notification preferences (< 50ms)
- [ ] Load Settings page (< 1s)
- [ ] Send notification (< 500ms)
- [ ] Indexes are used (check execution plan)

## ?? Documentation Review

- [ ] NOTIFICATIONS_PRODUCTION_READY.md reviewed
- [ ] NOTIFICATIONS_QUICK_REFERENCE.md reviewed
- [ ] NOTIFICATIONS_IMPLEMENTATION_SUMMARY.md reviewed
- [ ] Code comments clear and helpful
- [ ] Public methods documented
- [ ] Error cases documented

## ?? Monitoring & Alerts

- [ ] Monitor NotificationLogs for failures
- [ ] Alert on high failure rate (> 10%)
- [ ] Check SMTP connection daily
- [ ] Archive old logs monthly (6+ months)
- [ ] Monitor email delivery via provider's dashboard

## ?? Team Training

- [ ] Team knows how to access Settings page
- [ ] Team can configure notifications
- [ ] Team knows how to check NotificationLogs
- [ ] Team knows how to troubleshoot issues
- [ ] Team has documentation in wiki/SharePoint

## ?? Migration from Old System (if applicable)

- [ ] Backup existing notification preferences
- [ ] Data migration script (if needed)
- [ ] Verify data integrity after migration
- [ ] Archive old tables
- [ ] Update queries to use new tables

## ?? Production Deployment

### Pre-Deployment
- [ ] Backup production database
- [ ] Review change log
- [ ] Notify users of deployment window
- [ ] Test rollback procedure

### Deployment
- [ ] Deploy code to production
- [ ] Run migrations automatically or manually
- [ ] Verify all tables created
- [ ] Test one manual notification
- [ ] Monitor application logs for errors

### Post-Deployment
- [ ] Verify Settings page loads
- [ ] Test saving preferences
- [ ] Test notification sending
- [ ] Monitor NotificationLogs
- [ ] Check for SMTP errors
- [ ] Verify email delivery
- [ ] Communicate status to team

## ?? Rollback Plan

If issues occur:
1. Identify problem (check logs)
2. Revert code to previous version
3. Run `dotnet ef database update` with previous migration (optional)
4. Verify application works
5. Investigate root cause
6. Fix and retest
7. Redeploy

## ? Sign-Off

- [ ] Lead Developer: _______________  Date: ________
- [ ] QA Lead: _______________  Date: ________
- [ ] Operations: _______________  Date: ________
- [ ] Project Manager: _______________  Date: ________

## ?? Support Contacts

- **SMTP Issues**: Email provider support
- **Database Issues**: DBA team
- **Application Issues**: Development team
- **User Issues**: Help desk

## ?? Post-Deployment Review

**One Week After**:
- [ ] No critical errors in logs
- [ ] Email delivery rate > 95%
- [ ] Users satisfied with notifications
- [ ] Performance acceptable

**One Month After**:
- [ ] Archive old notification logs
- [ ] Review delivery metrics
- [ ] Check failure patterns
- [ ] Optimize if needed

---

## ?? Sign-Off Complete!

When all items are checked, the notification system is **production-ready and deployed**! ?

**Final Status**: Ready for Production ?
**Date Deployed**: _______________
**Deployed By**: _______________
