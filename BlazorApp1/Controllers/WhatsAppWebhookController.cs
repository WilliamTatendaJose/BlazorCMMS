using BlazorApp1.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BlazorApp1.Controllers;

/// <summary>
/// Webhook controller for receiving WhatsApp messages from Meta WhatsApp Business Cloud API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly WhatsAppService _whatsAppService;
    private readonly ILogger<WhatsAppWebhookController> _logger;
    private readonly IConfiguration _configuration;

    public WhatsAppWebhookController(
        WhatsAppService whatsAppService,
        ILogger<WhatsAppWebhookController> logger,
        IConfiguration configuration)
    {
        _whatsAppService = whatsAppService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Webhook verification endpoint (GET) - Required by Meta for initial webhook setup
    /// GET /api/whatsappwebhook
    /// </summary>
    [HttpGet]
    public IActionResult VerifyWebhook(
        [FromQuery(Name = "hub.mode")] string? mode,
        [FromQuery(Name = "hub.verify_token")] string? token,
        [FromQuery(Name = "hub.challenge")] string? challenge)
    {
        _logger.LogInformation("Webhook verification request - Mode: {Mode}", mode);

        if (_whatsAppService.VerifyWebhook(mode ?? "", token ?? "", challenge ?? "", out var response))
        {
            return Ok(response);
        }

        return Forbidden();
    }

    /// <summary>
    /// Webhook endpoint for incoming messages and status updates (POST)
    /// POST /api/whatsappwebhook
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook([FromBody] MetaWebhookPayload payload)
    {
        try
        {
            // Verify the webhook signature (optional but recommended for production)
            if (!VerifyWebhookSignature(Request))
            {
                _logger.LogWarning("Invalid webhook signature");
                // For production, return Unauthorized()
                // return Unauthorized();
            }

            _logger.LogInformation("Received webhook: {Object}", payload.Object);

            if (payload.Object != "whatsapp_business_account")
            {
                return Ok(); // Not a WhatsApp webhook, ignore
            }

            // Process the webhook
            var result = await _whatsAppService.ProcessWebhookAsync(payload);
            
            _logger.LogInformation("Webhook processed: {Success}", result.Success);
            
            // Always return 200 to acknowledge receipt
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing WhatsApp webhook");
            // Return 200 to prevent Meta from retrying
            return Ok();
        }
    }

    /// <summary>
    /// Alternative incoming message endpoint (for backward compatibility or custom routing)
    /// POST /api/whatsappwebhook/incoming
    /// </summary>
    [HttpPost("incoming")]
    public async Task<IActionResult> IncomingMessage([FromBody] MetaWebhookPayload payload)
    {
        return await ReceiveWebhook(payload);
    }

    /// <summary>
    /// Status callback endpoint
    /// POST /api/whatsappwebhook/status
    /// </summary>
    [HttpPost("status")]
    public async Task<IActionResult> StatusCallback([FromBody] MetaWebhookPayload payload)
    {
        return await ReceiveWebhook(payload);
    }

    /// <summary>
    /// Health check endpoint
    /// GET /api/whatsappwebhook/health
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        var config = new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            whatsAppEnabled = _configuration.GetValue<bool>("WhatsApp:Enabled"),
            apiVersion = _configuration["WhatsApp:Meta:ApiVersion"] ?? "v18.0",
            hasPhoneNumberId = !string.IsNullOrEmpty(_configuration["WhatsApp:Meta:PhoneNumberId"]),
            hasAccessToken = !string.IsNullOrEmpty(_configuration["WhatsApp:Meta:AccessToken"])
        };

        return Ok(config);
    }

    /// <summary>
    /// Test endpoint to send a message (for development/testing)
    /// POST /api/whatsappwebhook/test
    /// </summary>
    [HttpPost("test")]
    public async Task<IActionResult> TestMessage([FromBody] TestMessageRequest request)
    {
        if (string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.Message))
        {
            return BadRequest(new { error = "Phone and message are required" });
        }

        var result = await _whatsAppService.SendTextMessageAsync(request.Phone, request.Message);
        
        return Ok(new { success = result, phone = request.Phone });
    }

    /// <summary>
    /// Verify the webhook signature from Meta
    /// </summary>
    private bool VerifyWebhookSignature(HttpRequest request)
    {
        var appSecret = _configuration["WhatsApp:Meta:AppSecret"];
        if (string.IsNullOrEmpty(appSecret))
            return true; // Skip validation if not configured

        var signature = request.Headers["X-Hub-Signature-256"].FirstOrDefault();
        if (string.IsNullOrEmpty(signature))
        {
            _logger.LogWarning("Missing X-Hub-Signature-256 header");
            return false;
        }

        // In production, implement proper HMAC-SHA256 signature verification
        // For now, just check that signature header exists
        return signature.StartsWith("sha256=");
    }

    private IActionResult Forbidden()
    {
        return StatusCode(403, new { error = "Forbidden" });
    }
}

/// <summary>
/// Request model for test endpoint
/// </summary>
public class TestMessageRequest
{
    public string Phone { get; set; } = "";
    public string Message { get; set; } = "";
}
