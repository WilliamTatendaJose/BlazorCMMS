# WhatsApp Business Cloud API Integration Guide

## Overview

RBM CMMS integrates with the **Official Meta WhatsApp Business Cloud API** to enable real-time communication with technicians directly through WhatsApp.

## Setup Requirements

### 1. Meta Business Account

1. Go to [Meta for Developers](https://developers.facebook.com/)
2. Create a new app or use an existing one
3. Add the **WhatsApp** product to your app
4. Complete business verification

### 2. WhatsApp Business API Access

1. In your Meta App dashboard, go to **WhatsApp > Getting Started**
2. Note down your:
   - **Phone Number ID**
   - **WhatsApp Business Account ID**
   - **Permanent Access Token** (generate from System Users in Business Settings)

### 3. Configure appsettings.json

```json
{
  "WhatsApp": {
    "Enabled": true,
    "UseLLM": true,
    "Meta": {
      "AccessToken": "YOUR_PERMANENT_ACCESS_TOKEN",
      "PhoneNumberId": "YOUR_PHONE_NUMBER_ID",
      "BusinessAccountId": "YOUR_BUSINESS_ACCOUNT_ID",
      "AppSecret": "YOUR_APP_SECRET",
      "WebhookVerifyToken": "YOUR_CUSTOM_VERIFY_TOKEN",
      "ApiVersion": "v18.0"
    }
  }
}
```

### 4. Configure Webhook

In your Meta App Dashboard:

1. Go to **WhatsApp > Configuration**
2. Set Webhook URL: `https://yourdomain.com/api/whatsappwebhook`
3. Set Verify Token: Same value as `WebhookVerifyToken` in appsettings
4. Subscribe to webhook fields:
   - `messages`
   - `message_deliveries`
   - `message_reads`

## API Features

### Text Messages

```csharp
await whatsAppService.SendTextMessageAsync(
    "1234567890",  // Phone number
    "Hello! This is a test message."
);
```

### Template Messages (Business-Initiated)

Required for messaging users outside the 24-hour window:

```csharp
var components = new List<MetaTemplateComponent>
{
    new() 
    { 
        Type = "body",
        Parameters = new List<MetaTemplateParameter>
        {
            new() { Type = "text", Text = "John Smith" },
            new() { Type = "text", Text = "WO-2024-001" }
        }
    }
};

await whatsAppService.SendTemplateMessageAsync(
    "1234567890",
    "work_order_notification",  // Template name
    "en",                       // Language code
    components
);
```

### Interactive Buttons

```csharp
var buttons = new List<MetaInteractiveButton>
{
    new() 
    { 
        Type = "reply",
        Reply = new MetaButtonReply { Id = "ack", Title = "Acknowledge" }
    },
    new() 
    { 
        Type = "reply",
        Reply = new MetaButtonReply { Id = "start", Title = "Start Work" }
    }
};

await whatsAppService.SendInteractiveButtonsAsync(
    "1234567890",
    "New work order assigned to you",
    "Reply to take action",
    buttons
);
```

### Interactive Lists

```csharp
var sections = new List<MetaInteractiveSection>
{
    new()
    {
        Title = "Work Orders",
        Rows = new List<MetaInteractiveRow>
        {
            new() { Id = "wo-001", Title = "WO-2024-001", Description = "Pump maintenance" },
            new() { Id = "wo-002", Title = "WO-2024-002", Description = "Motor repair" }
        }
    }
};

await whatsAppService.SendInteractiveListAsync(
    "1234567890",
    "Your Work Orders",
    "Select a work order to view details",
    "View Options",
    sections
);
```

### Media Messages

```csharp
// Send document
await whatsAppService.SendDocumentAsync(
    "1234567890",
    "https://example.com/manual.pdf",
    "Equipment_Manual.pdf",
    "Here is the equipment manual"
);

// Send image
await whatsAppService.SendImageAsync(
    "1234567890",
    "https://example.com/diagram.png",
    "Wiring diagram for reference"
);
```

## Technician Commands

| Command | Description |
|---------|-------------|
| `HELP` | Show available commands |
| `MY WORK` | List assigned work orders |
| `PENDING` | List pending acknowledgements |
| `STATUS` | Quick status summary |
| `ACK` | Acknowledge latest work order |
| `ACK WO-123` | Acknowledge specific work order |
| `START` | Start latest acknowledged work order |
| `START WO-123` | Start specific work order |
| `COMPLETE` | Complete current work order |
| `COMPLETE WO-123` | Complete specific work order |
| `DELAY [reason]` | Report delay on current work |
| `NOTE [text]` | Add note to current work order |
| `ESCALATE` | Request supervisor assistance |
| `RESPONDING` | Confirm response to alert |
| `CONFIRM` | Accept scheduled maintenance |

## Webhook Events

The system processes these webhook events:

### Incoming Messages
- Text messages
- Interactive button replies
- Interactive list selections
- Media messages (logged)

### Status Updates
- `sent` - Message sent to WhatsApp servers
- `delivered` - Message delivered to user's device
- `read` - Message read by user
- `failed` - Message delivery failed

## Message Templates

Create message templates in Meta Business Manager for:

1. **Work Order Assignment** - `work_order_assignment`
2. **Work Order Reminder** - `work_order_reminder`
3. **Critical Alert** - `critical_equipment_alert`
4. **Schedule Notification** - `maintenance_schedule`

Templates must be approved by Meta before use.

## Rate Limits

Meta WhatsApp API has rate limits:

| Tier | Messages/day |
|------|--------------|
| Tier 1 | 1,000 |
| Tier 2 | 10,000 |
| Tier 3 | 100,000 |
| Tier 4 | Unlimited |

Quality rating affects tier movement.

## Pricing

WhatsApp Business API charges per conversation:

| Type | Initiated By | Cost (varies by country) |
|------|--------------|--------------------------|
| Business-initiated | Your business | Higher |
| User-initiated | Customer | Lower |
| Service | Within 24h window | Free |

## Troubleshooting

### Messages Not Sending

1. Check `WhatsApp:Enabled` is `true`
2. Verify Access Token is valid and not expired
3. Check Phone Number ID is correct
4. Ensure phone number format is correct (no + prefix)

### Webhook Not Working

1. Verify webhook URL is publicly accessible
2. Check `WebhookVerifyToken` matches configuration
3. Ensure HTTPS is enabled
4. Check webhook subscriptions in Meta dashboard

### Template Messages Failing

1. Ensure template is approved
2. Check template name and language code
3. Verify all required parameters are provided
4. Check parameter types match template definition

## Security

### Webhook Signature Verification

Enable signature verification for production:

```csharp
// In appsettings.json
"AppSecret": "YOUR_APP_SECRET"
```

The controller verifies the `X-Hub-Signature-256` header.

### Access Token Management

- Use permanent access tokens from System Users
- Rotate tokens periodically
- Never expose tokens in client-side code

## Testing

### Test Endpoint

```bash
curl -X POST https://yourdomain.com/api/whatsappwebhook/test \
  -H "Content-Type: application/json" \
  -d '{"phone": "1234567890", "message": "Test message"}'
```

### Health Check

```bash
curl https://yourdomain.com/api/whatsappwebhook/health
```

Returns:
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "whatsAppEnabled": true,
  "apiVersion": "v18.0",
  "hasPhoneNumberId": true,
  "hasAccessToken": true
}
```

## Migration from Twilio

If migrating from Twilio:

1. Update `appsettings.json` with Meta configuration
2. Remove Twilio-specific settings
3. Update webhook URL in your deployment
4. Create and approve message templates
5. Test with a small group before full rollout

## Resources

- [Meta WhatsApp Business Platform](https://business.whatsapp.com/)
- [Cloud API Documentation](https://developers.facebook.com/docs/whatsapp/cloud-api)
- [Message Templates](https://developers.facebook.com/docs/whatsapp/message-templates)
- [Webhook Documentation](https://developers.facebook.com/docs/whatsapp/cloud-api/webhooks)
