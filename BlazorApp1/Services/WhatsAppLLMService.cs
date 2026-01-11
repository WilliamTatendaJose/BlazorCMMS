using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace BlazorApp1.Services;

/// <summary>
/// LLM-powered assistant for WhatsApp interactions
/// Supports multiple providers: Groq (FREE), Azure OpenAI, OpenAI, Gemini
/// </summary>
public class WhatsAppLLMService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<WhatsAppLLMService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    // Provider selection
    private string Provider => _configuration["LLM:Provider"] ?? "Groq";
    
    // Groq configuration (FREE!)
    private string GroqApiKey => _configuration["LLM:Groq:ApiKey"] ?? "";
    private string GroqModel => _configuration["LLM:Groq:Model"] ?? "llama-3.1-8b-instant";
    private bool IsGroqEnabled => _configuration.GetValue<bool>("LLM:Groq:Enabled");
    
    // Azure OpenAI configuration
    private string AzureOpenAIEndpoint => _configuration["LLM:AzureOpenAI:Endpoint"] ?? "";
    private string AzureOpenAIKey => _configuration["LLM:AzureOpenAI:ApiKey"] ?? "";
    private string AzureDeploymentName => _configuration["LLM:AzureOpenAI:DeploymentName"] ?? "gpt-4o-mini";
    private bool IsAzureEnabled => _configuration.GetValue<bool>("LLM:AzureOpenAI:Enabled");
    
    // OpenAI configuration
    private string OpenAIKey => _configuration["LLM:OpenAI:ApiKey"] ?? "";
    private string OpenAIModel => _configuration["LLM:OpenAI:Model"] ?? "gpt-4o-mini";
    private bool IsOpenAIEnabled => _configuration.GetValue<bool>("LLM:OpenAI:Enabled");
    
    // Gemini configuration
    private string GeminiKey => _configuration["LLM:Gemini:ApiKey"] ?? "";
    private string GeminiModel => _configuration["LLM:Gemini:Model"] ?? "gemini-1.5-flash";
    private bool IsGeminiEnabled => _configuration.GetValue<bool>("LLM:Gemini:Enabled");

    private bool IsLLMEnabled => IsGroqEnabled || IsAzureEnabled || IsOpenAIEnabled || IsGeminiEnabled;

    public WhatsAppLLMService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        UserManager<ApplicationUser> userManager,
        ILogger<WhatsAppLLMService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("LLMClient");
    }

    /// <summary>
    /// Process a natural language message from a technician via WhatsApp
    /// </summary>
    public async Task<LLMResponse> ProcessMessageAsync(string userMessage, string userName, string userId)
    {
        if (!IsLLMEnabled)
        {
            _logger.LogWarning("No LLM provider is enabled. Falling back to command-based processing.");
            return new LLMResponse
            {
                Success = false,
                Message = "AI assistant is not enabled. Please use commands like HELP, MY WORK, ACK, etc."
            };
        }

        try
        {
            // Get user's current context (work orders, etc.)
            var context = await BuildUserContextAsync(userName);

            // Build the system prompt with context
            var systemPrompt = BuildSystemPrompt(userName, context);

            // Call the configured LLM provider
            var response = await CallLLMAsync(systemPrompt, userMessage);

            // Parse the response and execute any actions
            var result = await ProcessLLMResponseAsync(response, userName, userId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing LLM message for user {UserName}", userName);
            return new LLMResponse
            {
                Success = false,
                Message = "Sorry, I encountered an error processing your request. Please try again or use a command like HELP."
            };
        }
    }

    /// <summary>
    /// Route to the appropriate LLM provider
    /// </summary>
    private async Task<string> CallLLMAsync(string systemPrompt, string userMessage)
    {
        // Priority: Groq (free) > Gemini (cheapest) > OpenAI > Azure OpenAI
        if (IsGroqEnabled && !string.IsNullOrEmpty(GroqApiKey))
        {
            _logger.LogInformation("Using Groq LLM (FREE tier)");
            return await CallGroqAsync(systemPrompt, userMessage);
        }
        
        if (IsGeminiEnabled && !string.IsNullOrEmpty(GeminiKey))
        {
            _logger.LogInformation("Using Google Gemini");
            return await CallGeminiAsync(systemPrompt, userMessage);
        }
        
        if (IsOpenAIEnabled && !string.IsNullOrEmpty(OpenAIKey))
        {
            _logger.LogInformation("Using OpenAI");
            return await CallOpenAIAsync(systemPrompt, userMessage);
        }
        
        if (IsAzureEnabled && !string.IsNullOrEmpty(AzureOpenAIKey))
        {
            _logger.LogInformation("Using Azure OpenAI");
            return await CallAzureOpenAIAsync(systemPrompt, userMessage);
        }

        throw new InvalidOperationException("No LLM provider is properly configured.");
    }

    /// <summary>
    /// Call Groq API (FREE!)
    /// </summary>
    private async Task<string> CallGroqAsync(string systemPrompt, string userMessage)
    {
        var endpoint = "https://api.groq.com/openai/v1/chat/completions";

        var requestBody = new
        {
            model = GroqModel,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            max_tokens = 500,
            temperature = 0.7
        };

        return await SendChatCompletionRequest(endpoint, requestBody, GroqApiKey, "Bearer");
    }

    /// <summary>
    /// Call Google Gemini API
    /// </summary>
    private async Task<string> CallGeminiAsync(string systemPrompt, string userMessage)
    {
        var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{GeminiModel}:generateContent?key={GeminiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = $"{systemPrompt}\n\nUser: {userMessage}" }
                    }
                }
            },
            generationConfig = new
            {
                maxOutputTokens = 500,
                temperature = 0.7
            }
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, httpContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Gemini API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
            throw new Exception($"Gemini API error: {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);

        var text = responseJson
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return text ?? "I apologize, but I couldn't generate a response.";
    }

    /// <summary>
    /// Call OpenAI API
    /// </summary>
    private async Task<string> CallOpenAIAsync(string systemPrompt, string userMessage)
    {
        var endpoint = "https://api.openai.com/v1/chat/completions";

        var requestBody = new
        {
            model = OpenAIModel,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            max_tokens = 500,
            temperature = 0.7
        };

        return await SendChatCompletionRequest(endpoint, requestBody, OpenAIKey, "Bearer");
    }

    /// <summary>
    /// Call Azure OpenAI API
    /// </summary>
    private async Task<string> CallAzureOpenAIAsync(string systemPrompt, string userMessage)
    {
        var endpoint = $"{AzureOpenAIEndpoint.TrimEnd('/')}/openai/deployments/{AzureDeploymentName}/chat/completions?api-version=2024-08-01-preview";

        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            max_tokens = 500,
            temperature = 0.7
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("api-key", AzureOpenAIKey);

        var response = await _httpClient.PostAsync(endpoint, httpContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Azure OpenAI API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
            throw new Exception($"Azure OpenAI API error: {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return ExtractChatCompletionResponse(responseContent);
    }

    /// <summary>
    /// Generic method for OpenAI-compatible APIs (OpenAI, Groq)
    /// </summary>
    private async Task<string> SendChatCompletionRequest(string endpoint, object requestBody, string apiKey, string authType)
    {
        var jsonContent = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"{authType} {apiKey}");

        var response = await _httpClient.PostAsync(endpoint, httpContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("LLM API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
            throw new Exception($"LLM API error: {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return ExtractChatCompletionResponse(responseContent);
    }

    private string ExtractChatCompletionResponse(string responseContent)
    {
        var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);

        var assistantMessage = responseJson
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return assistantMessage ?? "I apologize, but I couldn't generate a response.";
    }

    /// <summary>
    /// Process the LLM response and execute any embedded actions
    /// </summary>
    private async Task<LLMResponse> ProcessLLMResponseAsync(string llmResponse, string userName, string userId)
    {
        var result = new LLMResponse { Success = true };

        // Extract action JSON if present
        var actionMatch = System.Text.RegularExpressions.Regex.Match(
            llmResponse,
            @"```action\s*({.*?})\s*```",
            System.Text.RegularExpressions.RegexOptions.Singleline);

        if (actionMatch.Success)
        {
            try
            {
                var actionJson = actionMatch.Groups[1].Value;
                var action = JsonSerializer.Deserialize<WorkOrderAction>(actionJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (action != null)
                {
                    var actionResult = await ExecuteActionAsync(action, userName);
                    result.ActionExecuted = true;
                    result.ActionResult = actionResult;
                }

                // Remove the action JSON from the displayed message
                result.Message = llmResponse
                    .Replace(actionMatch.Value, "")
                    .Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse/execute action from LLM response");
                result.Message = llmResponse;
            }
        }
        else
        {
            result.Message = llmResponse;
        }

        return result;
    }

    /// <summary>
    /// Execute a work order action with ownership verification
    /// </summary>
    private async Task<string> ExecuteActionAsync(WorkOrderAction action, string userName)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();

        var workOrder = await dbContext.Set<WorkOrder>()
            .FirstOrDefaultAsync(w => w.WorkOrderId == action.WorkOrderId);

        if (workOrder == null)
        {
            return $"Work order {action.WorkOrderId} not found.";
        }

        // AUTHORIZATION CHECK: Verify the technician owns this work order
        if (workOrder.AssignedTo != userName && 
            !string.Equals(workOrder.AssignedTo, userName, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Unauthorized action attempt via LLM: User {UserName} tried to {Action} work order {WorkOrderId} assigned to {AssignedTo}",
                userName, action.Action, action.WorkOrderId, workOrder.AssignedTo);
            
            return $"?? Work Order {action.WorkOrderId} is not assigned to you. You can only manage your own work orders.";
        }

        switch (action.Action?.ToLower())
        {
            case "acknowledge":
                workOrder.IsAcknowledged = true;
                workOrder.AcknowledgedBy = userName;
                workOrder.AcknowledgedDate = DateTime.Now;
                await dbContext.SaveChangesAsync();
                return $"? Work order {action.WorkOrderId} acknowledged.";

            case "start":
                if (!workOrder.IsAcknowledged)
                {
                    workOrder.IsAcknowledged = true;
                    workOrder.AcknowledgedBy = userName;
                    workOrder.AcknowledgedDate = DateTime.Now;
                }
                workOrder.Status = "In Progress";
                workOrder.StartedDate = DateTime.Now;
                await dbContext.SaveChangesAsync();
                return $"?? Work order {action.WorkOrderId} started. Timer running.";

            case "complete":
                workOrder.Status = "Completed";
                workOrder.CompletedDate = DateTime.Now;
                workOrder.TimeCompleted = DateTime.Now;
                if (workOrder.StartedDate.HasValue)
                {
                    workOrder.LaborHours = (DateTime.Now - workOrder.StartedDate.Value).TotalHours;
                }
                await dbContext.SaveChangesAsync();
                return $"? Work order {action.WorkOrderId} completed!";

            case "add_note":
                var noteText = action.Note ?? "";
                workOrder.DetailsOfWorkCarriedOut = string.IsNullOrEmpty(workOrder.DetailsOfWorkCarriedOut)
                    ? $"[{DateTime.Now:MMM dd HH:mm}] {noteText}"
                    : $"{workOrder.DetailsOfWorkCarriedOut}\n[{DateTime.Now:MMM dd HH:mm}] {noteText}";
                await dbContext.SaveChangesAsync();
                return $"?? Note added to {action.WorkOrderId}.";

            case "report_delay":
                var delayReason = action.Reason ?? "No reason provided";
                workOrder.CompletionNotes = string.IsNullOrEmpty(workOrder.CompletionNotes)
                    ? $"[{DateTime.Now:MMM dd HH:mm}] DELAY: {delayReason}"
                    : $"{workOrder.CompletionNotes}\n[{DateTime.Now:MMM dd HH:mm}] DELAY: {delayReason}";
                await dbContext.SaveChangesAsync();
                return $"?? Delay reported for {action.WorkOrderId}. Supervisor notified.";

            case "escalate":
                workOrder.Priority = "Critical";
                workOrder.CompletionNotes = string.IsNullOrEmpty(workOrder.CompletionNotes)
                    ? $"[{DateTime.Now:MMM dd HH:mm}] ESCALATED by {userName}"
                    : $"{workOrder.CompletionNotes}\n[{DateTime.Now:MMM dd HH:mm}] ESCALATED by {userName}";
                await dbContext.SaveChangesAsync();
                return $"?? Work order {action.WorkOrderId} escalated to supervisor.";

            default:
                return $"Unknown action: {action.Action}";
        }
    }

    /// <summary>
    /// Get conversation history for a user (for context in multi-turn conversations)
    /// </summary>
    public async Task<List<ConversationMessage>> GetRecentConversationAsync(string phoneNumber, int messageCount = 10)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();

        var messages = await dbContext.Set<WhatsAppMessageLog>()
            .Where(m => m.PhoneNumber == phoneNumber)
            .OrderByDescending(m => m.Timestamp)
            .Take(messageCount)
            .OrderBy(m => m.Timestamp)
            .Select(m => new ConversationMessage
            {
                Role = m.Direction == WhatsAppMessageDirection.Incoming ? "user" : "assistant",
                Content = m.Message,
                Timestamp = m.Timestamp
            })
            .ToListAsync();

        return messages;
    }

    /// <summary>
    /// Build context about the user's current work situation
    /// </summary>
    private async Task<UserWorkContext> BuildUserContextAsync(string userName)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();

        var workOrders = await dbContext.Set<WorkOrder>()
            .Where(w => w.AssignedTo == userName)
            .Where(w => w.Status != "Completed" && w.Status != "Cancelled")
            .OrderByDescending(w => w.Priority == "Critical")
            .ThenBy(w => w.DueDate)
            .Take(10)
            .ToListAsync();

        var pendingCount = workOrders.Count(w => w.Status == "Open" && !w.IsAcknowledged);
        var inProgressCount = workOrders.Count(w => w.Status == "In Progress");
        var acknowledgedCount = workOrders.Count(w => w.IsAcknowledged && w.Status == "Open");

        var currentWorkOrder = workOrders
            .Where(w => w.Status == "In Progress")
            .OrderByDescending(w => w.StartedDate)
            .FirstOrDefault();

        var latestPending = workOrders
            .Where(w => w.Status == "Open" && !w.IsAcknowledged)
            .OrderByDescending(w => w.CreatedDate)
            .FirstOrDefault();

        // Get recent maintenance schedules
        var schedules = await dbContext.Set<MaintenanceSchedule>()
            .Where(s => s.AssignedTechnician == userName && s.Status == "Scheduled")
            .OrderBy(s => s.ScheduledDate)
            .Take(5)
            .ToListAsync();

        return new UserWorkContext
        {
            TotalActiveWorkOrders = workOrders.Count,
            PendingAcknowledgement = pendingCount,
            InProgress = inProgressCount,
            Acknowledged = acknowledgedCount,
            WorkOrders = workOrders,
            CurrentWorkOrder = currentWorkOrder,
            LatestPendingWorkOrder = latestPending,
            UpcomingSchedules = schedules,
            CurrentDateTime = DateTime.Now
        };
    }

    /// <summary>
    /// Build the system prompt for the LLM with tools definition
    /// </summary>
    private string BuildSystemPrompt(string userName, UserWorkContext context)
    {
        var contextSummary = new StringBuilder();
        contextSummary.AppendLine($"Current Date/Time: {context.CurrentDateTime:dddd, MMMM dd, yyyy HH:mm}");
        contextSummary.AppendLine($"Technician: {userName}");
        contextSummary.AppendLine();
        contextSummary.AppendLine("=== WORK STATUS ===");
        contextSummary.AppendLine($"Total Active Work Orders: {context.TotalActiveWorkOrders}");
        contextSummary.AppendLine($"Pending Acknowledgement: {context.PendingAcknowledgement}");
        contextSummary.AppendLine($"In Progress: {context.InProgress}");
        contextSummary.AppendLine($"Acknowledged (not started): {context.Acknowledged}");

        if (context.CurrentWorkOrder != null)
        {
            contextSummary.AppendLine();
            contextSummary.AppendLine("=== CURRENT WORK ORDER (In Progress) ===");
            contextSummary.AppendLine($"WO#: {context.CurrentWorkOrder.WorkOrderId}");
            contextSummary.AppendLine($"Title: {context.CurrentWorkOrder.Title}");
            contextSummary.AppendLine($"Priority: {context.CurrentWorkOrder.Priority}");
            contextSummary.AppendLine($"Started: {context.CurrentWorkOrder.StartedDate:MMM dd HH:mm}");
            contextSummary.AppendLine($"Due: {context.CurrentWorkOrder.DueDate?.ToString("MMM dd") ?? "Not set"}");
        }

        if (context.LatestPendingWorkOrder != null)
        {
            contextSummary.AppendLine();
            contextSummary.AppendLine("=== LATEST PENDING WORK ORDER ===");
            contextSummary.AppendLine($"WO#: {context.LatestPendingWorkOrder.WorkOrderId}");
            contextSummary.AppendLine($"Title: {context.LatestPendingWorkOrder.Title}");
            contextSummary.AppendLine($"Priority: {context.LatestPendingWorkOrder.Priority}");
            contextSummary.AppendLine($"Due: {context.LatestPendingWorkOrder.DueDate?.ToString("MMM dd") ?? "Not set"}");
        }

        if (context.WorkOrders.Any())
        {
            contextSummary.AppendLine();
            contextSummary.AppendLine("=== ALL ACTIVE WORK ORDERS ===");
            foreach (var wo in context.WorkOrders)
            {
                var status = wo.IsAcknowledged ? (wo.Status == "In Progress" ? "?? In Progress" : "? Acknowledged") : "? Pending";
                contextSummary.AppendLine($"• {wo.WorkOrderId} | {wo.Title} | {wo.Priority} | {status} | Due: {wo.DueDate?.ToString("MMM dd") ?? "N/A"}");
            }
        }

        if (context.UpcomingSchedules.Any())
        {
            contextSummary.AppendLine();
            contextSummary.AppendLine("=== UPCOMING SCHEDULED MAINTENANCE ===");
            foreach (var schedule in context.UpcomingSchedules)
            {
                contextSummary.AppendLine($"• {schedule.AssetName} | {schedule.Description} | {schedule.ScheduledDate:MMM dd HH:mm}");
            }
        }

        return $$"""
            You are an AI assistant for RBM CMMS (Computerized Maintenance Management System), helping technicians manage their work via WhatsApp.
            
            Your capabilities:
            1. Answer questions about work orders, maintenance schedules, and assets
            2. Help technicians understand their tasks and priorities
            3. Execute actions like acknowledging, starting, or completing work orders
            4. Add notes to work orders
            5. Report delays and escalate issues
            6. Provide guidance on maintenance procedures
            
            IMPORTANT RULES:
            - Be concise - responses are sent via WhatsApp (character limits apply)
            - Use emojis to make responses scannable ???????
            - Always confirm before executing actions
            - If unsure, ask for clarification
            - Format work order IDs as WO-XXXX
            - Be friendly but professional
            
            AVAILABLE ACTIONS (return as JSON in your response when user confirms):
            - {"action": "acknowledge", "workOrderId": "WO-XXXX"}
            - {"action": "start", "workOrderId": "WO-XXXX"}
            - {"action": "complete", "workOrderId": "WO-XXXX"}
            - {"action": "add_note", "workOrderId": "WO-XXXX", "note": "text"}
            - {"action": "report_delay", "workOrderId": "WO-XXXX", "reason": "text"}
            - {"action": "escalate", "workOrderId": "WO-XXXX"}
            
            When you need to execute an action, include the JSON at the END of your response wrapped in ```action tags.
            
            CURRENT CONTEXT FOR {{userName}}:
            {{contextSummary}}
            
            Respond naturally to the technician's message. If they ask to do something, confirm what you'll do and include the action JSON.
            """;
    }
}

#region Models

/// <summary>
/// Response from LLM processing
/// </summary>
public class LLMResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public bool ActionExecuted { get; set; }
    public string? ActionResult { get; set; }
}

/// <summary>
/// User's current work context
/// </summary>
public class UserWorkContext
{
    public int TotalActiveWorkOrders { get; set; }
    public int PendingAcknowledgement { get; set; }
    public int InProgress { get; set; }
    public int Acknowledged { get; set; }
    public List<WorkOrder> WorkOrders { get; set; } = new();
    public WorkOrder? CurrentWorkOrder { get; set; }
    public WorkOrder? LatestPendingWorkOrder { get; set; }
    public List<MaintenanceSchedule> UpcomingSchedules { get; set; } = new();
    public DateTime CurrentDateTime { get; set; }
}

/// <summary>
/// Action to execute from LLM response
/// </summary>
public class WorkOrderAction
{
    public string? Action { get; set; }
    public string? WorkOrderId { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// Conversation message for history
/// </summary>
public class ConversationMessage
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime Timestamp { get; set; }
}

#endregion
