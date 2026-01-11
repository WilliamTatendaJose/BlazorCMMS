using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BlazorApp1.Services;

/// <summary>
/// WhatsApp communication service for technician notifications and interactions
/// Uses Twilio WhatsApp Business API
/// </summary>
public class WhatsAppService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<WhatsAppService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly WhatsAppLLMService? _llmService;
    
    private string AccountSid => _configuration["WhatsApp:TwilioAccountSid"] ?? "";
    private string AuthToken => _configuration["WhatsApp:TwilioAuthToken"] ?? "";
    private string WhatsAppNumber => _configuration["WhatsApp:FromNumber"] ?? "";
    private bool IsEnabled => _configuration.GetValue<bool>("WhatsApp:Enabled");
    private bool UseLLM => _configuration.GetValue<bool>("WhatsApp:UseLLM");

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
        _httpClient = httpClientFactory.CreateClient("TwilioWhatsApp");
        _llmService = llmService;
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

        return await SendWhatsAppMessageAsync(technicianPhone, message, workOrder.Id, WhatsAppMessageType.WorkOrderAssignment);
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
            • *DELAY {"{reason}"}* - Report delay
            • *COMPLETE* - Mark as done
            """;

        return await SendWhatsAppMessageAsync(technicianPhone, message, workOrder.Id, WhatsAppMessageType.Reminder);
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

        return await SendWhatsAppMessageAsync(technicianPhone, message, assetId, WhatsAppMessageType.CriticalAlert);
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

        return await SendWhatsAppMessageAsync(technicianPhone, message, schedule.Id, WhatsAppMessageType.MaintenanceSchedule);
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

        // Log incoming message
        await LogMessageAsync(normalizedPhone, messageBody, WhatsAppMessageDirection.Incoming, messageId);

        // Find user by phone number - check ApplicationUser (Identity) first
        var identityUser = await FindUserByPhoneAsync(normalizedPhone, fromPhone);
        
        if (identityUser == null)
        {
            return new WhatsAppResponse
            {
                Success = false,
                Message = "Phone number not registered. Please contact your administrator to link your phone to your account."
            };
        }

        var userId = identityUser.Id;
        var userName = identityUser.FullName ?? identityUser.UserName ?? "";

        // Check if LLM mode is enabled and this is not a direct command
        if (UseLLM && _llmService != null && !IsDirectCommand(command))
        {
            // Use LLM for natural language processing
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

        // Fallback to command-based processing
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
                    Message = "Unknown command. Reply *HELP* for available commands."
                }
        };
    }

    /// <summary>
    /// Check if the message is a direct command (should bypass LLM)
    /// </summary>
    private bool IsDirectCommand(string command)
    {
        var directCommands = new[] 
        { 
            "HELP", "?", "STATUS", "MY WORK", "MYWORK", "PENDING", 
            "ACK", "START", "COMPLETE", "RESPONDING", "ESCALATE", "CONFIRM"
        };
        
        return directCommands.Any(c => command == c || command.StartsWith(c + " "));
    }

    /// <summary>
    /// Process message with LLM as fallback
    /// </summary>
    private async Task<WhatsAppResponse> ProcessWithLLMFallbackAsync(string message, string userName, string userId)
    {
        if (_llmService == null)
        {
            return new WhatsAppResponse
            {
                Success = false,
                Message = "Unknown command. Reply *HELP* for available commands."
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
        // Try to find by normalized phone
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

    /// <summary>
    /// Verify that a work order belongs to the specified technician
    /// This ensures technicians can only access their own assigned work orders
    /// </summary>
    private async Task<(bool IsAuthorized, WorkOrder? WorkOrder)> VerifyWorkOrderOwnershipAsync(string workOrderId, string userName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var workOrder = await context.Set<WorkOrder>()
            .FirstOrDefaultAsync(w => w.WorkOrderId == workOrderId);

        if (workOrder == null)
        {
            return (false, null);
        }

        // Check if work order is assigned to this technician
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

    /// <summary>
    /// Get work orders that belong to the specified technician only
    /// </summary>
    private async Task<List<WorkOrder>> GetTechnicianWorkOrdersAsync(string userName, bool activeOnly = true)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var query = context.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName);

        if (activeOnly)
        {
            query = query.Where(w => w.Status != "Completed" && w.Status != "Cancelled");
        }

        return await query
            .OrderByDescending(w => w.Priority == "Critical")
            .ThenByDescending(w => w.Priority == "High")
            .ThenBy(w => w.DueDate)
            .ToListAsync();
    }

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
            • DELAY {reason} - Report delay
            • NOTE {text} - Add note to latest WO
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
        // Verify the technician owns this work order
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
                Message = $"?? Work Order *{woId}* is not assigned to you. You can only manage your own work orders." 
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
        
        // Only get work orders assigned to this technician
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
            Message = $"?? Work Order *{workOrder.WorkOrderId}* started.\n?? Timer running.\n\nReply *COMPLETE* when finished or *NOTE {"{details}"}* to add notes."
        };
    }

    private async Task<WhatsAppResponse> StartWorkOrderAsync(string woId, string userName)
    {
        // Verify the technician owns this work order
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
                Message = $"?? Work Order *{woId}* is not assigned to you. You can only manage your own work orders." 
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
        // Verify the technician owns this work order
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
                Message = $"?? Work Order *{woId}* is not assigned to you. You can only manage your own work orders." 
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

        // Add delay note
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
        // This would integrate with your alert system
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

    #region Core Messaging

    /// <summary>
    /// Send WhatsApp message via Twilio API
    /// </summary>
    private async Task<bool> SendWhatsAppMessageAsync(string toPhone, string message, int? relatedEntityId, WhatsAppMessageType messageType)
    {
        if (!IsEnabled)
        {
            _logger.LogInformation("WhatsApp disabled. Would send to {Phone}: {Message}", toPhone, message);
            return true;
        }

        try
        {
            var normalizedPhone = NormalizePhoneNumber(toPhone);
            
            // Twilio API endpoint
            var url = $"https://api.twilio.com/2010-04-01/Accounts/{AccountSid}/Messages.json";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["From"] = $"whatsapp:{WhatsAppNumber}",
                ["To"] = $"whatsapp:{normalizedPhone}",
                ["Body"] = message
            });

            // Add Basic Auth
            var authBytes = System.Text.Encoding.ASCII.GetBytes($"{AccountSid}:{AuthToken}");
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

            var response = await _httpClient.PostAsync(url, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var messageData = JsonSerializer.Deserialize<JsonElement>(responseBody);
                var messageSid = messageData.GetProperty("sid").GetString();
                
                await LogMessageAsync(normalizedPhone, message, WhatsAppMessageDirection.Outgoing, messageSid, messageType, relatedEntityId);
                
                _logger.LogInformation("WhatsApp message sent to {Phone}, SID: {Sid}", normalizedPhone, messageSid);
                return true;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send WhatsApp message: {StatusCode} - {Error}", response.StatusCode, errorBody);
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
    /// Send template message (for business-initiated conversations)
    /// </summary>
    public async Task<bool> SendTemplateMessageAsync(string toPhone, string templateName, Dictionary<string, string> parameters)
    {
        if (!IsEnabled)
            return false;

        try
        {
            var normalizedPhone = NormalizePhoneNumber(toPhone);
            var url = $"https://api.twilio.com/2010-04-01/Accounts/{AccountSid}/Messages.json";
            
            // Build content SID for template
            var contentVariables = JsonSerializer.Serialize(parameters);
            
            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["From"] = $"whatsapp:{WhatsAppNumber}",
                ["To"] = $"whatsapp:{normalizedPhone}",
                ["ContentSid"] = templateName,
                ["ContentVariables"] = contentVariables
            });

            var authBytes = System.Text.Encoding.ASCII.GetBytes($"{AccountSid}:{AuthToken}");
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

            var response = await _httpClient.PostAsync(url, formContent);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending template message to {Phone}", toPhone);
            return false;
        }
    }

    #endregion

    #region Logging & Utilities

    private async Task LogMessageAsync(string phone, string message, WhatsAppMessageDirection direction, 
        string? externalId = null, WhatsAppMessageType? messageType = null, int? relatedEntityId = null)
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
                Status = WhatsAppMessageStatus.Sent
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
        
        // Ensure it starts with country code (assume +1 if not present)
        if (digits.Length == 10)
            digits = "1" + digits;
        
        return "+" + digits;
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

/// <summary>
/// Response from WhatsApp message processing
/// </summary>
public class WhatsAppResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public Dictionary<string, object>? Data { get; set; }
}
