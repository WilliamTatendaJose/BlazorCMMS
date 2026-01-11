# WhatsApp Integration for RBM CMMS

## Overview

This feature enables two-way WhatsApp communication between technicians and the RBM CMMS system, allowing field technicians to manage work orders, receive alerts, and update task status directly from their mobile phones.

## Architecture

```
???????????????????     ???????????????????     ???????????????????
?   Technician    ???????  Twilio/Meta    ???????   RBM CMMS      ?
?   WhatsApp      ???????  WhatsApp API   ???????   Server        ?
???????????????????     ???????????????????     ???????????????????
                               ?
                               ?
                        ???????????????????
                        ?    Webhook      ?
                        ?  /api/whatsapp  ?
                        ?   webhook/      ?
                        ?   incoming      ?
                        ???????????????????
```

## Quick Start

### 1. Get Twilio Credentials

1. Sign up at [twilio.com](https://www.twilio.com)
2. Go to Console ? Account Info
3. Copy your **Account SID** and **Auth Token**
4. Navigate to Messaging ? Try it out ? Send a WhatsApp message
5. Note the sandbox number (e.g., +14155238886)

### 2. Configure appsettings.json

```json
{
  "WhatsApp": {
    "Enabled": true,
    "TwilioAccountSid": "ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "TwilioAuthToken": "your_auth_token_here",
    "FromNumber": "+14155238886"
  }
}
```

### 3. Set Up Webhook

In Twilio Console:
1. Go to Messaging ? Settings ? WhatsApp sandbox settings
2. Set "When a message comes in" to: `https://your-domain.com/api/whatsappwebhook/incoming`
3. Set "Status callback URL" to: `https://your-domain.com/api/whatsappwebhook/status`

### 4. Run Database Migration

Execute the SQL migration script:
```sql
-- Run BlazorApp1/Migrations/ADD_WHATSAPP_SUPPORT.sql
```

### 5. Link Technician Phone Numbers

Ensure all technicians have their phone numbers (with country code) in their user profiles.

## Technician Commands

### Work Order Management

| Command | Description | Example |
|---------|-------------|---------|
| `MY WORK` | View all assigned work orders | `MY WORK` |
| `PENDING` | View pending tasks requiring action | `PENDING` |
| `STATUS` | Quick summary of work order counts | `STATUS` |
| `ACK` | Acknowledge latest work order | `ACK` |
| `ACK WO-123` | Acknowledge specific work order | `ACK WO-2024-001` |
| `START` | Start latest acknowledged work order | `START` |
| `START WO-123` | Start specific work order | `START WO-2024-001` |
| `COMPLETE` | Complete current in-progress work | `COMPLETE` |
| `COMPLETE WO-123` | Complete specific work order | `COMPLETE WO-2024-001` |

### Updates & Notes

| Command | Description | Example |
|---------|-------------|---------|
| `NOTE {text}` | Add note to current work order | `NOTE Replaced bearing, needs alignment` |
| `DELAY {reason}` | Report delay with reason | `DELAY Waiting for spare parts` |
| `ESCALATE` | Escalate to supervisor | `ESCALATE` |

### Alerts & Confirmations

| Command | Description | Example |
|---------|-------------|---------|
| `RESPONDING` | Confirm responding to critical alert | `RESPONDING` |
| `CONFIRM` | Accept scheduled maintenance | `CONFIRM` |
| `HELP` | Show all available commands | `HELP` |

## Automatic Notifications

The system automatically sends WhatsApp messages for:

### Work Order Assignment
```
?? *New Work Order Assigned*

?? *WO#:* WO-2024-001
?? *Title:* Replace Motor Bearing
?? *Asset:* Conveyor Belt A-01
? *Priority:* High
?? *Due:* Dec 25, 2024

Bearing showing signs of wear, needs replacement.

Reply with:
• *ACK* - Acknowledge receipt
• *START* - Start work
• *HELP* - Request assistance
```

### Due Date Reminders
```
?? *Work Order Reminder*

?? *WO#:* WO-2024-001
?? *Title:* Replace Motor Bearing
? *Due in:* 2 day(s)
?? *Due Date:* Dec 25, 2024

Reply with:
• *STATUS* - Update status
• *DELAY {reason}* - Report delay
• *COMPLETE* - Mark as done
```

### Critical Alerts
```
?? *CRITICAL EQUIPMENT ALERT*

?? *Asset:* Pump Station P-01
?? *Alert:* Temperature exceeds 95°C threshold
?? *Time:* 14:30 Dec 22

Immediate attention required!

Reply with:
• *RESPONDING* - On my way
• *ESCALATE* - Need supervisor
```

## API Endpoints

### Webhook Endpoint (Twilio)
```
POST /api/whatsappwebhook/incoming
```
Receives incoming WhatsApp messages from Twilio and processes commands.

### Status Callback
```
POST /api/whatsappwebhook/status
```
Receives delivery status updates from Twilio.

### Health Check
```
GET /api/whatsappwebhook/health
```
Returns health status for monitoring.

## Integration with Existing Features

### Work Order Service
- Automatically sends WhatsApp when work orders are assigned
- Updates work order status based on WhatsApp commands

### Notification Service
- WhatsApp notifications complement email/SMS
- Respects user notification preferences

### Condition Monitoring
- Sends critical alerts when thresholds are exceeded
- Tracks technician response times

## Security Considerations

1. **Signature Validation**: Enable Twilio signature validation in production
2. **Phone Number Verification**: Only registered phone numbers can execute commands
3. **Tenant Isolation**: Messages are logged per tenant
4. **Audit Trail**: All messages are logged with timestamps

## Troubleshooting

### Messages Not Sending

1. Check `WhatsApp:Enabled` is `true` in config
2. Verify Twilio credentials are correct
3. Ensure phone number format includes country code (+1...)
4. Check Twilio console for error logs

### Messages Not Received

1. Verify webhook URL is publicly accessible
2. Check webhook URL is configured in Twilio console
3. Ensure HTTPS certificate is valid
4. Check application logs for webhook errors

### Commands Not Working

1. Verify technician's phone is linked to their account
2. Check phone number format matches (with/without +)
3. Review command spelling (case-insensitive)

## Production Deployment

### For Twilio Sandbox (Testing)
- Technicians must join sandbox by sending code to Twilio number
- Limited to sandbox-approved numbers

### For Twilio Production
1. Apply for Twilio WhatsApp Business Profile
2. Get approved message templates from Meta
3. Use your business phone number
4. Update configuration with production credentials

### For Meta WhatsApp Cloud API (Alternative)
If you prefer direct Meta integration:
1. Create Meta Business Account
2. Set up WhatsApp Business API
3. Modify `WhatsAppService.cs` to use Meta's API endpoints

## Files Created

| File | Description |
|------|-------------|
| `Services/WhatsAppService.cs` | Core WhatsApp messaging service |
| `Models/WhatsAppMessageLog.cs` | Message logging model |
| `Controllers/WhatsAppWebhookController.cs` | Webhook endpoint |
| `Components/Pages/RBM/WhatsAppSettings.razor` | Admin settings UI |
| `Migrations/ADD_WHATSAPP_SUPPORT.sql` | Database migration |

## Next Steps

1. ? Configure Twilio credentials
2. ? Run database migration
3. ? Set up webhook URL in Twilio
4. ? Link technician phone numbers
5. ? Test with sandbox
6. ?? Apply for WhatsApp Business approval (production)
7. ?? Create approved message templates

## Support

For issues or questions, check:
- Twilio Console logs
- Application logs
- `/api/whatsappwebhook/health` endpoint
- WhatsApp Settings page in admin panel

---

*WhatsApp Integration v1.0 - RBM CMMS*
