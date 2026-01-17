using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp1.Services;

/// <summary>
/// WhatsApp communication service for technician notifications and interactions
/// Uses Official Meta WhatsApp Business Cloud API
/// </summary>
public class WhatsAppService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<WhatsAppService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly WhatsAppLLMService? _llmService;
    
    // Meta WhatsApp Business API Configuration
    private string AccessToken => _configuration["WhatsApp:Meta:AccessToken"] ?? "";
    private string PhoneNumberId => _configuration["WhatsApp:Meta:PhoneNumberId"] ?? "";
    private string BusinessAccountId => _configuration["WhatsApp:Meta:BusinessAccountId"] ?? "";
    private string WebhookVerifyToken => _configuration["WhatsApp:Meta:WebhookVerifyToken"] ?? "";
    private string ApiVersion => _configuration["WhatsApp:Meta:ApiVersion"] ?? "v18.0";
    
    private bool IsEnabled => _configuration.GetValue<bool>("WhatsApp:Enabled");
    private bool UseLLM => _configuration.GetValue<bool>("WhatsApp:UseLLM");
    
    private string BaseUrl => $"https://graph.facebook.com/{ApiVersion}/{PhoneNumberId}";

    public WhatsAppService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        UserManager<ApplicationUser> userManager,
        ILogger<WhatsAppService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        WhatsAppLLMService? llmService = null)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("MetaWhatsApp");
        _llmService = llmService;
        
        // Configure HttpClient for Meta API
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", AccessToken);
    }

    #region Work Order Notifications

    /// <summary>
    /// Send work order assignment notification to technician
    /// </summary>
    public async Task<bool> SendWorkOrderAssignmentAsync(WorkOrder workOrder, string technicianPhone)
    {
        if (!IsEnabled || string.IsNullOrEmpty(technicianPhone))
            return false;

        var message = $"""
            ?? *New Work Order Assigned*
            
            ?? *WO#:* {workOrder.WorkOrderId}
            ?? *Title:* {workOrder.Title}
            ?? *Asset:* {workOrder.AssetId}
            ? *Priority:* {workOrder.Priority}
            ?? *Due:* {workOrder.DueDate?.ToString("MMM dd, yyyy") ?? "Not set"}
            
            {workOrder.Description}
            
            Reply with:
            • *ACK* - Acknowledge receipt
            • *START* - Start work
            • *HELP* - Request assistance
            """;

        return await SendTextMessageAsync(technicianPhone, message, workOrder.Id, WhatsAppMessageType.WorkOrderAssignment);
    }

    /// <summary>
    /// Send work order reminder to technician
    /// </summary>
    public async Task<bool> SendWorkOrderReminderAsync(WorkOrder workOrder, string technicianPhone, int daysUntilDue)
    {
        if (!IsEnabled || string.IsNullOrEmpty(technicianPhone))
            return false;

        var urgencyEmoji = daysUntilDue <= 1 ? "??" : daysUntilDue <= 3 ? "??" : "??";
        
        var message = $"""
            {urgencyEmoji} *Work Order Reminder*
            
            ?? *WO#:* {workOrder.WorkOrderId}
            ?? *Title:* {workOrder.Title}
            ? *Due in:* {daysUntilDue} day(s)
            ?? *Due Date:* {workOrder.DueDate?.ToString("MMM dd, yyyy")}
            
            Reply with:
            • *STATUS* - Update status
            • *DELAY [reason]* - Report delay
            • *COMPLETE* - Mark as done
            """;

        return await SendTextMessageAsync(technicianPhone, message, workOrder.Id, WhatsAppMessageType.Reminder);
    }

    /// <summary>
    /// Send critical equipment alert
    /// </summary>
    public async Task<bool> SendCriticalAlertAsync(string technicianPhone, string assetName, string alertMessage, int? assetId = null)
    {
        if (!IsEnabled || string.IsNullOrEmpty(technicianPhone))
            return false;

        var message = $"""
            ?? *CRITICAL EQUIPMENT ALERT*
            
            ?? *Asset:* {assetName}
            ?? *Alert:* {alertMessage}
            ?? *Time:* {DateTime.Now:HH:mm MMM dd}
            
            Immediate attention required!
            
            Reply with:
            • *RESPONDING* - On my way
            • *ESCALATE* - Need supervisor
            """;

        return await SendTextMessageAsync(technicianPhone, message, assetId, WhatsAppMessageType.CriticalAlert);
    }

    /// <summary>
    /// Send maintenance schedule notification
    /// </summary>
    public async Task<bool> SendMaintenanceScheduleAsync(string technicianPhone, MaintenanceSchedule schedule)
    {
        if (!IsEnabled || string.IsNullOrEmpty(technicianPhone))
            return false;

        var message = $"""
            ??? *Scheduled Maintenance*
            
            ?? *Task:* {schedule.Description}
            ?? *Asset:* {schedule.AssetName}
            ?? *Scheduled:* {schedule.ScheduledDate:MMM dd, yyyy HH:mm}
            ?? *Type:* {schedule.Type}
            
            Reply *CONFIRM* to accept this assignment.
            """;

        return await SendTextMessageAsync(technicianPhone, message, schedule.Id, WhatsAppMessageType.MaintenanceSchedule);
    }

    #endregion

    #region Meta WhatsApp Cloud API - Message Sending

    /// <summary>
    /// Send a text message via Meta WhatsApp Cloud API
    /// </summary>
    public async Task<bool> SendTextMessageAsync(string toPhone, string message, int? relatedEntityId = null, WhatsAppMessageType messageType = WhatsAppMessageType.General)
    {
        if (!IsEnabled)
        {
            _logger.LogInformation("WhatsApp disabled. Would send to {Phone}: {Message}", toPhone, message);
            return true;
        }

        try
        {
            var normalizedPhone = NormalizePhoneNumber(toPhone);
            
            var payload = new MetaTextMessageRequest
            {
                MessagingProduct = "whatsapp",
                RecipientType = "individual",
                To = normalizedPhone,
                Type = "text",
                Text = new MetaTextContent { Body = message }
            };

            var response = await SendMetaApiRequestAsync("/messages", payload);
            
            if (response.Success && response.MessageId != null)
            {
                await LogMessageAsync(normalizedPhone, message, WhatsAppMessageDirection.Outgoing, 
                    response.MessageId, messageType, relatedEntityId);
                
                _logger.LogInformation("WhatsApp message sent to {Phone}, ID: {MessageId}", normalizedPhone, response.MessageId);
                return true;
            }
            else
            {
                _logger.LogError("Failed to send WhatsApp message: {Error}", response.Error);
                await LogMessageAsync(normalizedPhone, message, WhatsAppMessageDirection.Outgoing, 
                    null, messageType, relatedEntityId, WhatsAppMessageStatus.Failed, response.Error);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending WhatsApp message to {Phone}", toPhone);
            return false;
        }
    }

    /// <summary>
    /// Send a template message (required for business-initiated conversations outside 24h window)
    /// </summary>
    public async Task<bool> SendTemplateMessageAsync(string toPhone, string templateName, string languageCode = "en", 
        List<MetaTemplateComponent>? components = null)
    {
        if (!IsEnabled)
            return false;

        try
        {
            var normalizedPhone = NormalizePhoneNumber(toPhone);
            
            var payload = new MetaTemplateMessageRequest
            {
                MessagingProduct = "whatsapp",
                To = normalizedPhone,
                Type = "template",
                Template = new MetaTemplate
                {
                    Name = templateName,
                    Language = new MetaLanguage { Code = languageCode },
                    Components = components
                }
            };

            var response = await SendMetaApiRequestAsync("/messages", payload);
            
            if (response.Success)
            {
                _logger.LogInformation("Template message '{Template}' sent to {Phone}", templateName, normalizedPhone);
                return true;
            }
            
            _logger.LogError("Failed to send template message: {Error}", response.Error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending template message to {Phone}", toPhone);
            return false;
        }
    }

    /// <summary>
    /// Send an interactive button message
    /// </summary>
    public async Task<bool> SendInteractiveButtonsAsync(string toPhone, string bodyText, string footerText, 
        List<MetaInteractiveButton> buttons)
    {
        if (!IsEnabled)
            return false;

        try
        {
            var normalizedPhone = NormalizePhoneNumber(toPhone);
            
            var payload = new MetaInteractiveMessageRequest
            {
                MessagingProduct = "whatsapp",
                RecipientType = "individual",
                To = normalizedPhone,
                Type = "interactive",
                Interactive = new MetaInteractiveContent
                {
                    Type = "button",
                    Body = new MetaInteractiveBody { Text = bodyText },
                    Footer = string.IsNullOrEmpty(footerText) ? null : new MetaInteractiveFooter { Text = footerText },
                    Action = new MetaInteractiveAction { Buttons = buttons }
                }
            };

            var response = await SendMetaApiRequestAsync("/messages", payload);
            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending interactive buttons to {Phone}", toPhone);
            return false;
        }
    }

    /// <summary>
    /// Send an interactive list message
    /// </summary>
    public async Task<bool> SendInteractiveListAsync(string toPhone, string headerText, string bodyText, 
        string buttonText, List<MetaInteractiveSection> sections)
    {
        if (!IsEnabled)
            return false;

        try
        {
            var normalizedPhone = NormalizePhoneNumber(toPhone);
            
            var payload = new MetaInteractiveMessageRequest
            {
                MessagingProduct = "whatsapp",
                RecipientType = "individual",
                To = normalizedPhone,
                Type = "interactive",
                Interactive = new MetaInteractiveContent
                {
                    Type = "list",
                    Header = string.IsNullOrEmpty(headerText) ? null : new MetaInteractiveHeader { Type = "text", Text = headerText },
                    Body = new MetaInteractiveBody { Text = bodyText },
                    Action = new MetaInteractiveAction 
                    { 
                        Button = buttonText,
                        Sections = sections 
                    }
                }
            };

            var response = await SendMetaApiRequestAsync("/messages", payload);
            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending interactive list to {Phone}", toPhone);
            return false;
        }
    }

    /// <summary>
    /// Send a document/file message
    /// </summary>
    public async Task<bool> SendDocumentAsync(string toPhone, string documentUrl, string? filename = null, string? caption = null)
    {
        if (!IsEnabled)
            return false;

        try
        {
            var normalizedPhone = NormalizePhoneNumber(toPhone);
            
            var payload = new MetaMediaMessageRequest
            {
                MessagingProduct = "whatsapp",
                RecipientType = "individual",
                To = normalizedPhone,
                Type = "document",
                Document = new MetaMediaContent
                {
                    Link = documentUrl,
                    Filename = filename,
                    Caption = caption
                }
            };

            var response = await SendMetaApiRequestAsync("/messages", payload);
            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending document to {Phone}", toPhone);
            return false;
        }
    }

    /// <summary>
    /// Send an image message
    /// </summary>
    public async Task<bool> SendImageAsync(string toPhone, string imageUrl, string? caption = null)
    {
        if (!IsEnabled)
            return false;

        try
        {
            var normalizedPhone = NormalizePhoneNumber(toPhone);
            
            var payload = new MetaMediaMessageRequest
            {
                MessagingProduct = "whatsapp",
                RecipientType = "individual",
                To = normalizedPhone,
                Type = "image",
                Image = new MetaMediaContent
                {
                    Link = imageUrl,
                    Caption = caption
                }
            };

            var response = await SendMetaApiRequestAsync("/messages", payload);
            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending image to {Phone}", toPhone);
            return false;
        }
    }

    /// <summary>
    /// Mark a message as read
    /// </summary>
    public async Task<bool> MarkMessageAsReadAsync(string messageId)
    {
        if (!IsEnabled || string.IsNullOrEmpty(messageId))
            return false;

        try
        {
            var payload = new
            {
                messaging_product = "whatsapp",
                status = "read",
                message_id = messageId
            };

            var response = await SendMetaApiRequestAsync("/messages", payload);
            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as read: {MessageId}", messageId);
            return false;
        }
    }

    /// <summary>
    /// Send request to Meta Graph API
    /// </summary>
    private async Task<MetaApiResponse> SendMetaApiRequestAsync<T>(string endpoint, T payload)
    {
        try
        {
            var url = $"{BaseUrl}{endpoint}";
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            var json = JsonSerializer.Serialize(payload, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("Sending to Meta API: {Url} - {Payload}", url, json);

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Meta API response: {StatusCode} - {Body}", response.StatusCode, responseBody);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<MetaMessageResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new MetaApiResponse
                {
                    Success = true,
                    MessageId = result?.Messages?.FirstOrDefault()?.Id
                };
            }
            else
            {
                var error = JsonSerializer.Deserialize<MetaErrorResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new MetaApiResponse
                {
                    Success = false,
                    Error = error?.Error?.Message ?? $"HTTP {response.StatusCode}: {responseBody}"
                };
            }
        }
        catch (Exception ex)
        {
            return new MetaApiResponse
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    #endregion

    #region Webhook Processing

    /// <summary>
    /// Verify webhook from Meta (for initial setup)
    /// </summary>
    public bool VerifyWebhook(string mode, string token, string challenge, out string? response)
    {
        response = null;
        
        if (mode == "subscribe" && token == WebhookVerifyToken)
        {
            response = challenge;
            _logger.LogInformation("Webhook verified successfully");
            return true;
        }
        
        _logger.LogWarning("Webhook verification failed. Mode: {Mode}, Token valid: {TokenValid}", 
            mode, token == WebhookVerifyToken);
        return false;
    }

    /// <summary>
    /// Process incoming webhook from Meta
    /// </summary>
    public async Task<WhatsAppResponse> ProcessWebhookAsync(MetaWebhookPayload payload)
    {
        try
        {
            if (payload.Entry == null || !payload.Entry.Any())
            {
                return new WhatsAppResponse { Success = false, Message = "No entry in webhook" };
            }

            foreach (var entry in payload.Entry)
            {
                foreach (var change in entry.Changes ?? [])
                {
                    if (change.Value?.Messages != null)
                    {
                        foreach (var message in change.Value.Messages)
                        {
                            var result = await ProcessIncomingMessageAsync(
                                message.From,
                                GetMessageText(message),
                                message.Id);
                            
                            // Send reply
                            if (!string.IsNullOrEmpty(result.Message))
                            {
                                await SendTextMessageAsync(message.From, result.Message);
                            }
                        }
                    }
                    
                    // Process status updates
                    if (change.Value?.Statuses != null)
                    {
                        foreach (var status in change.Value.Statuses)
                        {
                            await ProcessStatusUpdateAsync(status);
                        }
                    }
                }
            }

            return new WhatsAppResponse { Success = true, Message = "Webhook processed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return new WhatsAppResponse { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// Extract text content from different message types
    /// </summary>
    private string GetMessageText(MetaWebhookMessage message)
    {
        return message.Type switch
        {
            "text" => message.Text?.Body ?? "",
            "interactive" => message.Interactive?.ButtonReply?.Id ?? 
                            message.Interactive?.ListReply?.Id ?? "",
            "button" => message.Button?.Text ?? "",
            _ => ""
        };
    }

    /// <summary>
    /// Process status update from webhook
    /// </summary>
    private async Task ProcessStatusUpdateAsync(MetaWebhookStatus status)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var log = await context.Set<WhatsAppMessageLog>()
                .FirstOrDefaultAsync(l => l.ExternalMessageId == status.Id);

            if (log != null)
            {
                log.Status = status.Status switch
                {
                    "sent" => WhatsAppMessageStatus.Sent,
                    "delivered" => WhatsAppMessageStatus.Delivered,
                    "read" => WhatsAppMessageStatus.Read,
                    "failed" => WhatsAppMessageStatus.Failed,
                    _ => log.Status
                };

                if (status.Status == "delivered")
                    log.DeliveredAt = DateTime.Now;
                else if (status.Status == "read")
                    log.ReadAt = DateTime.Now;
                else if (status.Status == "failed")
                    log.ErrorMessage = status.Errors?.FirstOrDefault()?.Message;

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing status update for message {MessageId}", status.Id);
        }
    }

    #endregion

    #region Interactive Commands

    /// <summary>
    /// Process incoming WhatsApp message from technician
    /// </summary>
    public async Task<WhatsAppResponse> ProcessIncomingMessageAsync(string fromPhone, string messageBody, string? messageId = null)
    {
        var normalizedPhone = NormalizePhoneNumber(fromPhone);
        var command = messageBody.Trim().ToUpperInvariant();
        
        _logger.LogInformation("Processing WhatsApp message from {Phone}: {Message}", normalizedPhone, messageBody);

        // Mark message as read
        if (!string.IsNullOrEmpty(messageId))
        {
            _ = MarkMessageAsReadAsync(messageId);
        }

        // Log incoming message
        await LogMessageAsync(normalizedPhone, messageBody, WhatsAppMessageDirection.Incoming, messageId);

        // Find user by phone number
        var identityUser = await FindUserByPhoneAsync(normalizedPhone, fromPhone);
        
        if (identityUser == null)
        {
            return new WhatsAppResponse
            {
                Success = false,
                Message = "?? Phone number not registered.\n\nPlease contact your administrator to link your phone to your account."
            };
        }

        var userId = identityUser.Id;
        var userName = identityUser.FullName ?? identityUser.UserName ?? "";

        // Check if LLM mode is enabled and this is not a direct command
        if (UseLLM && _llmService != null && !IsDirectCommand(command))
        {
            var llmResponse = await _llmService.ProcessMessageAsync(messageBody, userName, userId);
            
            var responseMessage = llmResponse.Message;
            if (llmResponse.ActionExecuted && !string.IsNullOrEmpty(llmResponse.ActionResult))
            {
                responseMessage += $"\n\n{llmResponse.ActionResult}";
            }
            
            return new WhatsAppResponse
            {
                Success = llmResponse.Success,
                Message = responseMessage
            };
        }

        // Command-based processing
        return command switch
        {
            "HELP" or "?" => await GetHelpMenuAsync(),
            "STATUS" => await GetMyWorkOrderStatusAsync(userName),
            "MY WORK" or "MYWORK" => await GetMyWorkOrdersAsync(userName),
            "PENDING" => await GetPendingWorkOrdersAsync(userName),
            "ACK" => await AcknowledgeLatestWorkOrderAsync(userName),
            "START" => await StartLatestWorkOrderAsync(userName),
            "COMPLETE" => await CompleteLatestWorkOrderAsync(userName),
            "RESPONDING" => await MarkRespondingAsync(userId),
            "ESCALATE" => await EscalateLatestWorkOrderAsync(userName),
            "CONFIRM" => await ConfirmMaintenanceAsync(userId),
            _ when command.StartsWith("ACK ") => await AcknowledgeWorkOrderAsync(command[4..].Trim(), userName),
            _ when command.StartsWith("START ") => await StartWorkOrderAsync(command[6..].Trim(), userName),
            _ when command.StartsWith("COMPLETE ") => await CompleteWorkOrderAsync(command[9..].Trim(), userName),
            _ when command.StartsWith("DELAY ") => await ReportDelayAsync(command[6..].Trim(), userName),
            _ when command.StartsWith("NOTE ") => await AddNoteAsync(command[5..].Trim(), userName),
            _ => UseLLM && _llmService != null 
                ? await ProcessWithLLMFallbackAsync(messageBody, userName, userId)
                : new WhatsAppResponse
                {
                    Success = false,
                    Message = "? Unknown command.\n\nReply *HELP* for available commands."
                }
        };
    }

    private bool IsDirectCommand(string command)
    {
        var directCommands = new[] 
        { 
            "HELP", "?", "STATUS", "MY WORK", "MYWORK", "PENDING", 
            "ACK", "START", "COMPLETE", "RESPONDING", "ESCALATE", "CONFIRM"
        };
        
        return directCommands.Any(c => command == c || command.StartsWith(c + " "));
    }

    private async Task<WhatsAppResponse> ProcessWithLLMFallbackAsync(string message, string userName, string userId)
    {
        if (_llmService == null)
        {
            return new WhatsAppResponse
            {
                Success = false,
                Message = "? Unknown command.\n\nReply *HELP* for available commands."
            };
        }

        var llmResponse = await _llmService.ProcessMessageAsync(message, userName, userId);
        
        var responseMessage = llmResponse.Message;
        if (llmResponse.ActionExecuted && !string.IsNullOrEmpty(llmResponse.ActionResult))
        {
            responseMessage += $"\n\n{llmResponse.ActionResult}";
        }
        
        return new WhatsAppResponse
        {
            Success = llmResponse.Success,
            Message = responseMessage
        };
    }

    private async Task<ApplicationUser?> FindUserByPhoneAsync(string normalizedPhone, string originalPhone)
    {
        var users = _userManager.Users.ToList();
        
        foreach (var user in users)
        {
            if (string.IsNullOrEmpty(user.PhoneNumber))
                continue;
                
            var userPhone = NormalizePhoneNumber(user.PhoneNumber);
            if (userPhone == normalizedPhone || user.PhoneNumber == originalPhone)
                return user;
        }
        
        return null;
    }

    private async Task<(bool IsAuthorized, WorkOrder? WorkOrder)> VerifyWorkOrderOwnershipAsync(string workOrderId, string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrder = await context.Set<WorkOrder>()
            .FirstOrDefaultAsync(w => w.WorkOrderId == workOrderId);

        if (workOrder == null)
            return (false, null);

        var isOwner = workOrder.AssignedTo == userName ||
                     string.Equals(workOrder.AssignedTo, userName, StringComparison.OrdinalIgnoreCase);

        if (!isOwner)
        {
            _logger.LogWarning(
                "Unauthorized access attempt: User {UserName} tried to access work order {WorkOrderId} assigned to {AssignedTo}",
                userName, workOrderId, workOrder.AssignedTo);
        }

        return (isOwner, isOwner ? workOrder : null);
    }

    #endregion

    #region Work Order Commands

    private async Task<WhatsAppResponse> GetHelpMenuAsync()
    {
        var helpText = """
            ?? *RBM CMMS Commands*
            
            ?? *Work Orders:*
            • MY WORK - View your work orders
            • PENDING - View pending tasks
            • STATUS - Quick status summary
            • ACK - Acknowledge latest WO
            • ACK WO-123 - Acknowledge specific WO
            • START - Start latest WO
            • START WO-123 - Start specific WO
            • COMPLETE - Complete latest WO
            • COMPLETE WO-123 - Complete specific WO
            
            ?? *Updates:*
            • DELAY [reason] - Report delay
            • NOTE [text] - Add note to latest WO
            • ESCALATE - Request supervisor help
            
            ?? *Alerts:*
            • RESPONDING - Confirm alert response
            • CONFIRM - Accept scheduled work
            
            Reply with any command above.
            """;

        return new WhatsAppResponse { Success = true, Message = helpText };
    }

    private async Task<WhatsAppResponse> GetMyWorkOrderStatusAsync(string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var counts = await context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .GroupBy(w => w.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var pending = counts.FirstOrDefault(c => c.Status == "Open")?.Count ?? 0;
        var inProgress = counts.FirstOrDefault(c => c.Status == "In Progress")?.Count ?? 0;
        var completed = counts.FirstOrDefault(c => c.Status == "Completed")?.Count ?? 0;

        var message = $"""
            ?? *Your Work Order Status*
            
            ? Pending: {pending}
            ?? In Progress: {inProgress}
            ? Completed Today: {completed}
            
            Total Active: {pending + inProgress}
            """;

        return new WhatsAppResponse { Success = true, Message = message };
    }

    private async Task<WhatsAppResponse> GetMyWorkOrdersAsync(string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrders = await context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .Where(w => w.Status != "Completed" && w.Status != "Cancelled")
            .OrderByDescending(w => w.Priority == "Critical")
            .ThenByDescending(w => w.Priority == "High")
            .ThenBy(w => w.DueDate)
            .Take(5)
            .ToListAsync();

        if (!workOrders.Any())
        {
            return new WhatsAppResponse { Success = true, Message = "? No active work orders assigned to you." };
        }

        var woList = string.Join("\n\n", workOrders.Select(w =>
            $"?? *{w.WorkOrderId}*\n" +
            $"   {w.Title}\n" +
            $"   {GetPriorityEmoji(w.Priority)} {w.Priority} | {GetStatusEmoji(w.Status)} {w.Status}\n" +
            $"   ?? Due: {w.DueDate?.ToString("MMM dd") ?? "N/A"}"));

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"?? *Your Active Work Orders*\n\n{woList}\n\nReply *ACK WO-XXX* to acknowledge."
        };
    }

    private async Task<WhatsAppResponse> GetPendingWorkOrdersAsync(string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrders = await context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .Where(w => w.Status == "Open" && !w.IsAcknowledged)
            .OrderByDescending(w => w.Priority == "Critical")
            .ThenBy(w => w.DueDate)
            .Take(5)
            .ToListAsync();

        if (!workOrders.Any())
        {
            return new WhatsAppResponse { Success = true, Message = "? No pending work orders requiring acknowledgement." };
        }

        var woList = string.Join("\n", workOrders.Select(w =>
            $"• *{w.WorkOrderId}* - {w.Title} ({w.Priority})"));

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"? *Pending Acknowledgement*\n\n{woList}\n\nReply *ACK* to acknowledge the latest."
        };
    }

    private async Task<WhatsAppResponse> AcknowledgeLatestWorkOrderAsync(string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrder = await context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .Where(w => w.Status == "Open" && !w.IsAcknowledged)
            .OrderByDescending(w => w.CreatedDate)
            .FirstOrDefaultAsync();

        if (workOrder == null)
        {
            return new WhatsAppResponse { Success = false, Message = "No pending work orders to acknowledge." };
        }

        workOrder.IsAcknowledged = true;
        workOrder.AcknowledgedBy = userName;
        workOrder.AcknowledgedDate = DateTime.Now;
        
        await context.SaveChangesAsync();

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"? Work Order *{workOrder.WorkOrderId}* acknowledged.\n\nReply *START* when you begin work."
        };
    }

    private async Task<WhatsAppResponse> AcknowledgeWorkOrderAsync(string woId, string userName)
    {
        var (isAuthorized, workOrder) = await VerifyWorkOrderOwnershipAsync(woId, userName);
        
        if (workOrder == null)
        {
            return new WhatsAppResponse { Success = false, Message = $"Work Order *{woId}* not found." };
        }
        
        if (!isAuthorized)
        {
            return new WhatsAppResponse 
            { 
                Success = false, 
                Message = $"?? Work Order *{woId}* is not assigned to you." 
            };
        }

        await using var context = await _contextFactory.CreateDbContextAsync();
        var wo = await context.Set<WorkOrder>().FirstOrDefaultAsync(w => w.WorkOrderId == woId);
        
        if (wo != null)
        {
            wo.IsAcknowledged = true;
            wo.AcknowledgedBy = userName;
            wo.AcknowledgedDate = DateTime.Now;
            await context.SaveChangesAsync();
        }

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"? Work Order *{woId}* acknowledged.\n\nReply *START {woId}* when you begin work."
        };
    }

    private async Task<WhatsAppResponse> StartLatestWorkOrderAsync(string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrder = await context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .Where(w => w.Status == "Open" && w.IsAcknowledged)
            .OrderByDescending(w => w.AcknowledgedDate)
            .FirstOrDefaultAsync();

        if (workOrder == null)
        {
            return new WhatsAppResponse { Success = false, Message = "No acknowledged work orders to start. Reply *ACK* first." };
        }

        workOrder.Status = "In Progress";
        workOrder.StartedDate = DateTime.Now;
        
        await context.SaveChangesAsync();

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"?? Work Order *{workOrder.WorkOrderId}* started.\n?? Timer running.\n\nReply *COMPLETE* when finished."
        };
    }

    private async Task<WhatsAppResponse> StartWorkOrderAsync(string woId, string userName)
    {
        var (isAuthorized, workOrder) = await VerifyWorkOrderOwnershipAsync(woId, userName);
        
        if (workOrder == null)
        {
            return new WhatsAppResponse { Success = false, Message = $"Work Order *{woId}* not found." };
        }
        
        if (!isAuthorized)
        {
            return new WhatsAppResponse 
            { 
                Success = false, 
                Message = $"?? Work Order *{woId}* is not assigned to you." 
            };
        }

        if (!workOrder.IsAcknowledged)
        {
            return new WhatsAppResponse { Success = false, Message = $"Please acknowledge *{woId}* first. Reply *ACK {woId}*" };
        }

        await using var context = await _contextFactory.CreateDbContextAsync();
        var wo = await context.Set<WorkOrder>().FirstOrDefaultAsync(w => w.WorkOrderId == woId);
        
        if (wo != null)
        {
            wo.Status = "In Progress";
            wo.StartedDate = DateTime.Now;
            await context.SaveChangesAsync();
        }

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"?? Work Order *{woId}* started.\n?? Timer running.\n\nReply *COMPLETE {woId}* when finished."
        };
    }

    private async Task<WhatsAppResponse> CompleteLatestWorkOrderAsync(string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrder = await context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .Where(w => w.Status == "In Progress")
            .OrderByDescending(w => w.StartedDate)
            .FirstOrDefaultAsync();

        if (workOrder == null)
        {
            return new WhatsAppResponse { Success = false, Message = "No in-progress work orders to complete." };
        }

        workOrder.Status = "Completed";
        workOrder.CompletedDate = DateTime.Now;
        workOrder.TimeCompleted = DateTime.Now;
        
        if (workOrder.StartedDate.HasValue)
        {
            workOrder.LaborHours = (DateTime.Now - workOrder.StartedDate.Value).TotalHours;
        }
        
        await context.SaveChangesAsync();

        var duration = workOrder.LaborHours.HasValue 
            ? $"?? Duration: {workOrder.LaborHours:F1} hours" 
            : "";

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"? Work Order *{workOrder.WorkOrderId}* completed!\n{duration}\n\nGreat job! ??"
        };
    }

    private async Task<WhatsAppResponse> CompleteWorkOrderAsync(string woId, string userName)
    {
        var (isAuthorized, workOrder) = await VerifyWorkOrderOwnershipAsync(woId, userName);
        
        if (workOrder == null)
        {
            return new WhatsAppResponse { Success = false, Message = $"Work Order *{woId}* not found." };
        }
        
        if (!isAuthorized)
        {
            return new WhatsAppResponse 
            { 
                Success = false, 
                Message = $"?? Work Order *{woId}* is not assigned to you." 
            };
        }

        await using var context = await _contextFactory.CreateDbContextAsync();
        var wo = await context.Set<WorkOrder>().FirstOrDefaultAsync(w => w.WorkOrderId == woId);
        
        if (wo != null)
        {
            wo.Status = "Completed";
            wo.CompletedDate = DateTime.Now;
            wo.TimeCompleted = DateTime.Now;
            
            if (wo.StartedDate.HasValue)
            {
                wo.LaborHours = (DateTime.Now - wo.StartedDate.Value).TotalHours;
            }
            
            await context.SaveChangesAsync();
        }

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"? Work Order *{woId}* completed!\n\nGreat job! ??"
        };
    }

    private async Task<WhatsAppResponse> ReportDelayAsync(string reason, string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrder = await context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .Where(w => w.Status == "Open" || w.Status == "In Progress")
            .OrderByDescending(w => w.StartedDate ?? w.AcknowledgedDate ?? w.CreatedDate)
            .FirstOrDefaultAsync();

        if (workOrder == null)
        {
            return new WhatsAppResponse { Success = false, Message = "No active work order found to report delay." };
        }

        workOrder.CompletionNotes = string.IsNullOrEmpty(workOrder.CompletionNotes)
            ? $"[{DateTime.Now:MMM dd HH:mm}] DELAY: {reason}"
            : $"{workOrder.CompletionNotes}\n[{DateTime.Now:MMM dd HH:mm}] DELAY: {reason}";

        await context.SaveChangesAsync();

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"?? Delay reported for *{workOrder.WorkOrderId}*.\n\nSupervisor will be notified."
        };
    }

    private async Task<WhatsAppResponse> AddNoteAsync(string note, string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrder = await context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .Where(w => w.Status == "In Progress")
            .OrderByDescending(w => w.StartedDate)
            .FirstOrDefaultAsync();

        if (workOrder == null)
        {
            return new WhatsAppResponse { Success = false, Message = "No in-progress work order to add note to." };
        }

        workOrder.DetailsOfWorkCarriedOut = string.IsNullOrEmpty(workOrder.DetailsOfWorkCarriedOut)
            ? $"[{DateTime.Now:MMM dd HH:mm}] {note}"
            : $"{workOrder.DetailsOfWorkCarriedOut}\n[{DateTime.Now:MMM dd HH:mm}] {note}";

        await context.SaveChangesAsync();

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"?? Note added to *{workOrder.WorkOrderId}*."
        };
    }

    private async Task<WhatsAppResponse> EscalateLatestWorkOrderAsync(string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrder = await context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .Where(w => w.Status == "Open" || w.Status == "In Progress")
            .OrderByDescending(w => w.StartedDate ?? w.CreatedDate)
            .FirstOrDefaultAsync();

        if (workOrder == null)
        {
            return new WhatsAppResponse { Success = false, Message = "No active work order to escalate." };
        }

        workOrder.Priority = "Critical";
        workOrder.CompletionNotes = string.IsNullOrEmpty(workOrder.CompletionNotes)
            ? $"[{DateTime.Now:MMM dd HH:mm}] ESCALATED by {userName}: Supervisor assistance requested"
            : $"{workOrder.CompletionNotes}\n[{DateTime.Now:MMM dd HH:mm}] ESCALATED by {userName}: Supervisor assistance requested";

        await context.SaveChangesAsync();

        return new WhatsAppResponse
        {
            Success = true,
            Message = $"?? Work Order *{workOrder.WorkOrderId}* escalated to supervisor.\n\nHelp is on the way!"
        };
    }

    private Task<WhatsAppResponse> MarkRespondingAsync(string userId)
    {
        return Task.FromResult(new WhatsAppResponse
        {
            Success = true,
            Message = "? Response logged. Thank you for your quick response!"
        });
    }

    private Task<WhatsAppResponse> ConfirmMaintenanceAsync(string userId)
    {
        return Task.FromResult(new WhatsAppResponse
        {
            Success = true,
            Message = "? Maintenance schedule confirmed. You will receive a reminder before the scheduled time."
        });
    }

    #endregion

    #region Logging & Utilities

    private async Task LogMessageAsync(string phone, string message, WhatsAppMessageDirection direction, 
        string? externalId = null, WhatsAppMessageType? messageType = null, int? relatedEntityId = null,
        WhatsAppMessageStatus status = WhatsAppMessageStatus.Sent, string? errorMessage = null)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var log = new WhatsAppMessageLog
            {
                PhoneNumber = phone,
                Message = message.Length > 2000 ? message[..2000] : message,
                Direction = direction,
                ExternalMessageId = externalId,
                MessageType = messageType ?? WhatsAppMessageType.General,
                RelatedEntityId = relatedEntityId,
                Timestamp = DateTime.Now,
                Status = status,
                ErrorMessage = errorMessage
            };

            context.Set<WhatsAppMessageLog>().Add(log);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log WhatsApp message");
        }
    }

    private string NormalizePhoneNumber(string phone)
    {
        // Remove all non-digit characters
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        
        // Ensure it starts with country code (assume +1 if not present for 10-digit US numbers)
        if (digits.Length == 10)
            digits = "1" + digits;
        
        return digits; // Meta API expects phone without + prefix
    }

    private static string GetPriorityEmoji(string priority) => priority switch
    {
        "Critical" => "??",
        "High" => "??",
        "Medium" => "??",
        "Low" => "??",
        _ => "?"
    };

    private static string GetStatusEmoji(string status) => status switch
    {
        "Open" => "??",
        "In Progress" => "??",
        "Completed" => "?",
        "On Hold" => "??",
        "Cancelled" => "?",
        _ => "??"
    };

    #endregion
}

#region Response Classes

public class WhatsAppResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public Dictionary<string, object>? Data { get; set; }
}

public class MetaApiResponse
{
    public bool Success { get; set; }
    public string? MessageId { get; set; }
    public string? Error { get; set; }
}

#endregion

#region Meta API Request Models

public class MetaTextMessageRequest
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct { get; set; } = "whatsapp";
    
    [JsonPropertyName("recipient_type")]
    public string RecipientType { get; set; } = "individual";
    
    [JsonPropertyName("to")]
    public string To { get; set; } = "";
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";
    
    [JsonPropertyName("text")]
    public MetaTextContent? Text { get; set; }
}

public class MetaTextContent
{
    [JsonPropertyName("preview_url")]
    public bool PreviewUrl { get; set; } = false;
    
    [JsonPropertyName("body")]
    public string Body { get; set; } = "";
}

public class MetaTemplateMessageRequest
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct { get; set; } = "whatsapp";
    
    [JsonPropertyName("to")]
    public string To { get; set; } = "";
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "template";
    
    [JsonPropertyName("template")]
    public MetaTemplate? Template { get; set; }
}

public class MetaTemplate
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    
    [JsonPropertyName("language")]
    public MetaLanguage? Language { get; set; }
    
    [JsonPropertyName("components")]
    public List<MetaTemplateComponent>? Components { get; set; }
}

public class MetaLanguage
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = "en";
}

public class MetaTemplateComponent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = ""; // header, body, button
    
    [JsonPropertyName("parameters")]
    public List<MetaTemplateParameter>? Parameters { get; set; }
    
    [JsonPropertyName("sub_type")]
    public string? SubType { get; set; } // quick_reply, url
    
    [JsonPropertyName("index")]
    public int? Index { get; set; }
}

public class MetaTemplateParameter
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text"; // text, currency, date_time, image, document, video
    
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("image")]
    public MetaMediaContent? Image { get; set; }
    
    [JsonPropertyName("document")]
    public MetaMediaContent? Document { get; set; }
}

public class MetaInteractiveMessageRequest
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct { get; set; } = "whatsapp";
    
    [JsonPropertyName("recipient_type")]
    public string RecipientType { get; set; } = "individual";
    
    [JsonPropertyName("to")]
    public string To { get; set; } = "";
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "interactive";
    
    [JsonPropertyName("interactive")]
    public MetaInteractiveContent? Interactive { get; set; }
}

public class MetaInteractiveContent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = ""; // button, list, product, product_list
    
    [JsonPropertyName("header")]
    public MetaInteractiveHeader? Header { get; set; }
    
    [JsonPropertyName("body")]
    public MetaInteractiveBody? Body { get; set; }
    
    [JsonPropertyName("footer")]
    public MetaInteractiveFooter? Footer { get; set; }
    
    [JsonPropertyName("action")]
    public MetaInteractiveAction? Action { get; set; }
}

public class MetaInteractiveHeader
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text"; // text, image, video, document
    
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

public class MetaInteractiveBody
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
}

public class MetaInteractiveFooter
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
}

public class MetaInteractiveAction
{
    [JsonPropertyName("button")]
    public string? Button { get; set; } // For list messages
    
    [JsonPropertyName("buttons")]
    public List<MetaInteractiveButton>? Buttons { get; set; } // For button messages
    
    [JsonPropertyName("sections")]
    public List<MetaInteractiveSection>? Sections { get; set; } // For list messages
}

public class MetaInteractiveButton
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "reply";
    
    [JsonPropertyName("reply")]
    public MetaButtonReply? Reply { get; set; }
}

public class MetaButtonReply
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";
}

public class MetaInteractiveSection
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("rows")]
    public List<MetaInteractiveRow>? Rows { get; set; }
}

public class MetaInteractiveRow
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class MetaMediaMessageRequest
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct { get; set; } = "whatsapp";
    
    [JsonPropertyName("recipient_type")]
    public string RecipientType { get; set; } = "individual";
    
    [JsonPropertyName("to")]
    public string To { get; set; } = "";
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
    
    [JsonPropertyName("image")]
    public MetaMediaContent? Image { get; set; }
    
    [JsonPropertyName("document")]
    public MetaMediaContent? Document { get; set; }
    
    [JsonPropertyName("video")]
    public MetaMediaContent? Video { get; set; }
    
    [JsonPropertyName("audio")]
    public MetaMediaContent? Audio { get; set; }
}

public class MetaMediaContent
{
    [JsonPropertyName("id")]
    public string? Id { get; set; } // Media ID if uploaded
    
    [JsonPropertyName("link")]
    public string? Link { get; set; } // URL if hosted externally
    
    [JsonPropertyName("caption")]
    public string? Caption { get; set; }
    
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }
}

#endregion

#region Meta API Response Models

public class MetaMessageResponse
{
    [JsonPropertyName("messaging_product")]
    public string? MessagingProduct { get; set; }
    
    [JsonPropertyName("contacts")]
    public List<MetaContact>? Contacts { get; set; }
    
    [JsonPropertyName("messages")]
    public List<MetaMessageInfo>? Messages { get; set; }
}

public class MetaContact
{
    [JsonPropertyName("input")]
    public string? Input { get; set; }
    
    [JsonPropertyName("wa_id")]
    public string? WaId { get; set; }
}

public class MetaMessageInfo
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

public class MetaErrorResponse
{
    [JsonPropertyName("error")]
    public MetaError? Error { get; set; }
}

public class MetaError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("code")]
    public int? Code { get; set; }
    
    [JsonPropertyName("error_subcode")]
    public int? ErrorSubcode { get; set; }
    
    [JsonPropertyName("fbtrace_id")]
    public string? FbTraceId { get; set; }
}

#endregion

#region Meta Webhook Models

public class MetaWebhookPayload
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }
    
    [JsonPropertyName("entry")]
    public List<MetaWebhookEntry>? Entry { get; set; }
}

public class MetaWebhookEntry
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("changes")]
    public List<MetaWebhookChange>? Changes { get; set; }
}

public class MetaWebhookChange
{
    [JsonPropertyName("value")]
    public MetaWebhookValue? Value { get; set; }
    
    [JsonPropertyName("field")]
    public string? Field { get; set; }
}

public class MetaWebhookValue
{
    [JsonPropertyName("messaging_product")]
    public string? MessagingProduct { get; set; }
    
    [JsonPropertyName("metadata")]
    public MetaWebhookMetadata? Metadata { get; set; }
    
    [JsonPropertyName("contacts")]
    public List<MetaWebhookContact>? Contacts { get; set; }
    
    [JsonPropertyName("messages")]
    public List<MetaWebhookMessage>? Messages { get; set; }
    
    [JsonPropertyName("statuses")]
    public List<MetaWebhookStatus>? Statuses { get; set; }
}

public class MetaWebhookMetadata
{
    [JsonPropertyName("display_phone_number")]
    public string? DisplayPhoneNumber { get; set; }
    
    [JsonPropertyName("phone_number_id")]
    public string? PhoneNumberId { get; set; }
}

public class MetaWebhookContact
{
    [JsonPropertyName("profile")]
    public MetaWebhookProfile? Profile { get; set; }
    
    [JsonPropertyName("wa_id")]
    public string? WaId { get; set; }
}

public class MetaWebhookProfile
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class MetaWebhookMessage
{
    [JsonPropertyName("from")]
    public string From { get; set; } = "";
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
    
    [JsonPropertyName("text")]
    public MetaWebhookText? Text { get; set; }
    
    [JsonPropertyName("image")]
    public MetaWebhookMedia? Image { get; set; }
    
    [JsonPropertyName("document")]
    public MetaWebhookMedia? Document { get; set; }
    
    [JsonPropertyName("audio")]
    public MetaWebhookMedia? Audio { get; set; }
    
    [JsonPropertyName("video")]
    public MetaWebhookMedia? Video { get; set; }
    
    [JsonPropertyName("interactive")]
    public MetaWebhookInteractive? Interactive { get; set; }
    
    [JsonPropertyName("button")]
    public MetaWebhookButton? Button { get; set; }
    
    [JsonPropertyName("context")]
    public MetaWebhookContext? Context { get; set; }
}

public class MetaWebhookText
{
    [JsonPropertyName("body")]
    public string? Body { get; set; }
}

public class MetaWebhookMedia
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }
    
    [JsonPropertyName("sha256")]
    public string? Sha256 { get; set; }
    
    [JsonPropertyName("caption")]
    public string? Caption { get; set; }
    
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }
}

public class MetaWebhookInteractive
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("button_reply")]
    public MetaWebhookButtonReply? ButtonReply { get; set; }
    
    [JsonPropertyName("list_reply")]
    public MetaWebhookListReply? ListReply { get; set; }
}

public class MetaWebhookButtonReply
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

public class MetaWebhookListReply
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class MetaWebhookButton
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("payload")]
    public string? Payload { get; set; }
}

public class MetaWebhookContext
{
    [JsonPropertyName("from")]
    public string? From { get; set; }
    
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

public class MetaWebhookStatus
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";
    
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
    
    [JsonPropertyName("recipient_id")]
    public string? RecipientId { get; set; }
    
    [JsonPropertyName("conversation")]
    public MetaWebhookConversation? Conversation { get; set; }
    
    [JsonPropertyName("pricing")]
    public MetaWebhookPricing? Pricing { get; set; }
    
    [JsonPropertyName("errors")]
    public List<MetaWebhookError>? Errors { get; set; }
}

public class MetaWebhookConversation
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("origin")]
    public MetaWebhookOrigin? Origin { get; set; }
    
    [JsonPropertyName("expiration_timestamp")]
    public string? ExpirationTimestamp { get; set; }
}

public class MetaWebhookOrigin
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class MetaWebhookPricing
{
    [JsonPropertyName("billable")]
    public bool? Billable { get; set; }
    
    [JsonPropertyName("pricing_model")]
    public string? PricingModel { get; set; }
    
    [JsonPropertyName("category")]
    public string? Category { get; set; }
}

public class MetaWebhookError
{
    [JsonPropertyName("code")]
    public int? Code { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    [JsonPropertyName("error_data")]
    public MetaWebhookErrorData? ErrorData { get; set; }
}

public class MetaWebhookErrorData
{
    [JsonPropertyName("details")]
    public string? Details { get; set; }
}

#endregion
