using BlazorApp1.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers;

/// <summary>
/// Webhook controller for receiving WhatsApp messages from Twilio
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
    /// Twilio webhook endpoint for incoming WhatsApp messages
    /// POST /api/whatsappwebhook/incoming
    /// </summary>
    [HttpPost("incoming")]
    public async Task<IActionResult> IncomingMessage([FromForm] TwilioWhatsAppRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Incoming WhatsApp from {From}: {Body}", 
                request.From, 
                request.Body);

            // Validate Twilio signature (optional but recommended for production)
            if (!ValidateTwilioSignature(Request))
            {
                _logger.LogWarning("Invalid Twilio signature");
                // In production, return Unauthorized. For development, continue.
                // return Unauthorized();
            }

            // Process the message
            var response = await _whatsAppService.ProcessIncomingMessageAsync(
                request.From?.Replace("whatsapp:", "") ?? "",
                request.Body ?? "",
                request.MessageSid);

            // Return TwiML response
            var twiml = $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <Response>
                    <Message>{EscapeXml(response.Message)}</Message>
                </Response>
                """;

            return Content(twiml, "application/xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing WhatsApp webhook");
            
            var errorTwiml = """
                <?xml version="1.0" encoding="UTF-8"?>
                <Response>
                    <Message>Sorry, an error occurred. Please try again or contact support.</Message>
                </Response>
                """;

            return Content(errorTwiml, "application/xml");
        }
    }

    /// <summary>
    /// Status callback endpoint for delivery updates
    /// POST /api/whatsappwebhook/status
    /// </summary>
    [HttpPost("status")]
    public async Task<IActionResult> StatusCallback([FromForm] TwilioStatusCallback callback)
    {
        try
        {
            _logger.LogInformation(
                "WhatsApp status update - SID: {Sid}, Status: {Status}", 
                callback.MessageSid, 
                callback.MessageStatus);

            // You could update message status in database here
            // await _whatsAppService.UpdateMessageStatusAsync(callback.MessageSid, callback.MessageStatus);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing status callback");
            return Ok(); // Return OK to prevent Twilio retries
        }
    }

    /// <summary>
    /// Health check endpoint
    /// GET /api/whatsappwebhook/health
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    private bool ValidateTwilioSignature(HttpRequest request)
    {
        var authToken = _configuration["WhatsApp:TwilioAuthToken"];
        if (string.IsNullOrEmpty(authToken))
            return true; // Skip validation if not configured

        var signature = request.Headers["X-Twilio-Signature"].FirstOrDefault();
        if (string.IsNullOrEmpty(signature))
            return false;

        // Full signature validation would use Twilio's RequestValidator
        // For now, just check signature exists
        return !string.IsNullOrEmpty(signature);
    }

    private static string EscapeXml(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}

/// <summary>
/// Model for incoming Twilio WhatsApp webhook request
/// </summary>
public class TwilioWhatsAppRequest
{
    public string? MessageSid { get; set; }
    public string? AccountSid { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string? Body { get; set; }
    public int? NumMedia { get; set; }
    public string? MediaUrl0 { get; set; }
    public string? MediaContentType0 { get; set; }
    public string? ProfileName { get; set; }
    public string? WaId { get; set; }
}

/// <summary>
/// Model for Twilio status callback
/// </summary>
public class TwilioStatusCallback
{
    public string? MessageSid { get; set; }
    public string? MessageStatus { get; set; }
    public string? To { get; set; }
    public string? From { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}
