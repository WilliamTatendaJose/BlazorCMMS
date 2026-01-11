# WhatsApp AI Assistant Integration

## Overview

The WhatsApp integration now includes an optional AI Assistant powered by Azure OpenAI that allows technicians to communicate naturally instead of using rigid commands.

## Architecture

```
???????????????????     ???????????????????     ???????????????????
?   Technician    ???????     Twilio      ???????   RBM CMMS      ?
?   WhatsApp      ???????  WhatsApp API   ???????   Webhook       ?
???????????????????     ???????????????????     ???????????????????
                                                         ?
                                         ?????????????????????????????????
                                         ?                               ?
                                   ?????????????                 ?????????????????
                                   ?  Command  ?                 ?  AI Assistant ?
                                   ?  Parser   ?                 ? (Azure OpenAI)?
                                   ?????????????                 ?????????????????
                                         ?                               ?
                                         ?????????????????????????????????
                                                         ?
                                               ?????????????????????
                                               ?   Work Order      ?
                                               ?   Operations      ?
                                               ?????????????????????
```

## Features

### Natural Language Understanding

| Without AI | With AI |
|------------|---------|
| `ACK WO-2024-001` | "I acknowledge the pump repair work order" |
| `START` | "Okay, I'm starting work now" |
| `NOTE replaced bearing` | "Just finished replacing the bearing, it was worn out" |
| `DELAY waiting for parts` | "I need to wait for the spare parts to arrive" |

### Contextual Responses

The AI assistant has access to:
- User's assigned work orders
- Current work order status
- Upcoming maintenance schedules
- Due dates and priorities

### Actions the AI Can Execute

| Action | JSON Format | Description |
|--------|-------------|-------------|
| Acknowledge | `{"action": "acknowledge", "workOrderId": "WO-XXX"}` | Mark work order as acknowledged |
| Start | `{"action": "start", "workOrderId": "WO-XXX"}` | Start work timer |
| Complete | `{"action": "complete", "workOrderId": "WO-XXX"}` | Complete work order |
| Add Note | `{"action": "add_note", "workOrderId": "WO-XXX", "note": "text"}` | Add work notes |
| Report Delay | `{"action": "report_delay", "workOrderId": "WO-XXX", "reason": "text"}` | Report delay |
| Escalate | `{"action": "escalate", "workOrderId": "WO-XXX"}` | Escalate to supervisor |

## Configuration

### appsettings.json

```json
{
  "WhatsApp": {
    "Enabled": true,
    "UseLLM": true,
    "TwilioAccountSid": "YOUR_SID",
    "TwilioAuthToken": "YOUR_TOKEN",
    "FromNumber": "+14155238886"
  },
  "AzureOpenAI": {
    "Enabled": true,
    "Endpoint": "https://YOUR_RESOURCE.openai.azure.com",
    "ApiKey": "YOUR_API_KEY",
    "DeploymentName": "gpt-4o-mini"
  }
}
```

### Configuration Options

| Setting | Description | Default |
|---------|-------------|---------|
| `WhatsApp:Enabled` | Enable WhatsApp messaging | `false` |
| `WhatsApp:UseLLM` | Use AI for message processing | `true` |
| `AzureOpenAI:Enabled` | Enable Azure OpenAI | `false` |
| `AzureOpenAI:Endpoint` | Azure OpenAI endpoint URL | - |
| `AzureOpenAI:ApiKey` | API key | - |
| `AzureOpenAI:DeploymentName` | Model deployment name | `gpt-4o-mini` |

## Setup Azure OpenAI

### 1. Create Azure OpenAI Resource

1. Go to [Azure Portal](https://portal.azure.com)
2. Create a new "Azure OpenAI" resource
3. Wait for deployment to complete

### 2. Deploy a Model

1. Go to Azure OpenAI Studio
2. Navigate to Deployments
3. Create new deployment:
   - **Model**: `gpt-4o-mini` (recommended) or `gpt-4o`
   - **Deployment name**: Choose a name (use in config)
   - **Tokens per minute**: Start with 10,000

### 3. Get Credentials

1. In Azure Portal, go to your OpenAI resource
2. Copy the **Endpoint** (e.g., `https://myresource.openai.azure.com`)
3. Go to **Keys and Endpoint**
4. Copy **Key 1** or **Key 2**

## How It Works

### Message Flow

1. **Incoming Message**: Twilio receives WhatsApp message
2. **Webhook**: Message forwarded to `/api/whatsappwebhook/incoming`
3. **User Lookup**: Find user by phone number
4. **Mode Check**:
   - If direct command (HELP, STATUS, etc.) ? Command parser
   - If `UseLLM: true` ? AI Assistant
   - If `UseLLM: false` ? Command parser (fallback for unknown)
5. **Context Building**: Fetch user's work orders and status
6. **AI Processing**: Generate response with Azure OpenAI
7. **Action Extraction**: Parse any actions from AI response
8. **Execute Actions**: Update database if actions present
9. **Response**: Send formatted response back via WhatsApp

### System Prompt

The AI receives context including:
- Current date/time
- Technician name
- Active work orders list
- Current in-progress work order
- Pending work orders
- Upcoming maintenance schedules

### Example Conversation

```
User: Hi, what do I need to work on today?

AI: ?? Good morning! Here's your workload for today:

?? **Active Work Orders:**
• WO-2024-001 - Replace Motor Bearing (?? Critical) - Due today
• WO-2024-003 - Quarterly Pump Inspection (?? Medium) - Due Dec 25

You have 1 critical item that needs attention first. 
Would you like me to give you details on WO-2024-001?

---

User: Yes, tell me about the motor bearing job

AI: ?? **WO-2024-001: Replace Motor Bearing**

?? Asset: Pump Station P-01
? Priority: Critical
?? Due: Today (Dec 22)

?? Description:
Motor bearing showing signs of wear during last inspection. 
Vibration readings elevated. Needs replacement before failure.

The work order is currently pending acknowledgement. 
Would you like me to acknowledge it for you?

---

User: Yes, acknowledge it and I'm heading there now

AI: ? Perfect! I've acknowledged WO-2024-001 for you.

?? When you arrive and start working, just let me know 
and I'll start the timer. Good luck! ??

? Work order WO-2024-001 acknowledged.

---

User: Ok I'm there, starting now

AI: ?? Started WO-2024-001. Timer is running!

?? Started at: 09:45 AM

Tips for this job:
• Check bearing alignment after installation
• Log any additional findings

Let me know when you're done or if you need to add notes!

?? Work order WO-2024-001 started. Timer running.
```

## Fallback Behavior

When AI is unavailable or for direct commands, the system falls back to command-based processing:

| Command | Description |
|---------|-------------|
| `HELP` | Show available commands |
| `MY WORK` | List work orders |
| `STATUS` | Quick status summary |
| `ACK` | Acknowledge latest WO |
| `START` | Start latest WO |
| `COMPLETE` | Complete current WO |
| `NOTE {text}` | Add note |
| `DELAY {reason}` | Report delay |
| `ESCALATE` | Escalate to supervisor |

## Cost Considerations

### Azure OpenAI Pricing (Approximate)

| Model | Input (1K tokens) | Output (1K tokens) |
|-------|-------------------|-------------------|
| gpt-4o-mini | $0.00015 | $0.0006 |
| gpt-4o | $0.005 | $0.015 |

### Estimated Usage

- Average message: ~500 tokens (context) + ~100 tokens (response)
- 100 messages/day × 30 days = 3,000 messages/month
- Estimated cost: ~$5-15/month with gpt-4o-mini

## Security

1. **Phone Verification**: Only registered phone numbers can interact
2. **Action Confirmation**: AI confirms before executing actions
3. **Audit Trail**: All messages logged with timestamps
4. **No Sensitive Data**: AI doesn't expose sensitive business data

## Files

| File | Description |
|------|-------------|
| `Services/WhatsAppLLMService.cs` | AI assistant service |
| `Services/WhatsAppService.cs` | Main WhatsApp service (updated) |
| `Components/Pages/RBM/WhatsAppSettings.razor` | Admin settings (updated) |

## Troubleshooting

### AI Not Responding

1. Check `AzureOpenAI:Enabled` is `true`
2. Verify endpoint URL format
3. Confirm API key is valid
4. Check deployment name matches exactly

### Falling Back to Commands

1. Verify `WhatsApp:UseLLM` is `true`
2. Check Azure OpenAI logs for errors
3. Verify model deployment is active

### Actions Not Executing

1. Check AI response contains valid JSON in ```action``` block
2. Verify work order ID format (WO-XXXX)
3. Check database for work order existence

## Best Practices

1. **Start Simple**: Test with command mode first
2. **Monitor Costs**: Set up Azure cost alerts
3. **Rate Limiting**: Consider adding rate limits for AI calls
4. **Feedback Loop**: Monitor AI responses for quality
5. **Hybrid Mode**: Keep commands available as fallback
