using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BlazorApp1.Data;

namespace BlazorApp1.Services;

public class EmailSender : IEmailSender<ApplicationUser>
{
    private readonly ILogger<EmailSender> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var subject = "Confirm your email - RBM CMMS";
        var htmlMessage = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <div style='background: linear-gradient(135deg, #0288d1 0%, #37474f 100%); padding: 40px; text-align: center; color: white;'>
                    <h1 style='margin: 0; font-size: 28px;'>RBM CMMS</h1>
                    <p style='margin: 8px 0 0 0; opacity: 0.9;'>Reliability-Based Maintenance System</p>
                </div>
                <div style='padding: 40px; background: white;'>
                    <h2 style='color: #37474f; margin-top: 0;'>Confirm Your Email</h2>
                    <p style='color: #607d8b; line-height: 1.6;'>
                        Hello {user.FullName ?? user.Email},
                    </p>
                    <p style='color: #607d8b; line-height: 1.6;'>
                        Thank you for registering with RBM CMMS. Please confirm your email address by clicking the button below:
                    </p>
                    <div style='text-align: center; margin: 32px 0;'>
                        <a href='{confirmationLink}' style='display: inline-block; background: #0288d1; color: white; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: 600;'>
                            Confirm Email Address
                        </a>
                    </div>
                    <p style='color: #607d8b; font-size: 14px;'>
                        If you didn't create an account, you can safely ignore this email.
                    </p>
                </div>
                <div style='background: #eceff1; padding: 20px; text-align: center; color: #607d8b; font-size: 13px;'>
                    <p>� 2024 RBM CMMS. All rights reserved.</p>
                </div>
            </div>";

        await SendEmailAsync(email, subject, htmlMessage);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var subject = "Reset your password - RBM CMMS";
        var htmlMessage = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <div style='background: linear-gradient(135deg, #0288d1 0%, #37474f 100%); padding: 40px; text-align: center; color: white;'>
                    <h1 style='margin: 0; font-size: 28px;'>RBM CMMS</h1>
                    <p style='margin: 8px 0 0 0; opacity: 0.9;'>Reliability-Based Maintenance System</p>
                </div>
                <div style='padding: 40px; background: white;'>
                    <h2 style='color: #37474f; margin-top: 0;'>Reset Your Password</h2>
                    <p style='color: #607d8b; line-height: 1.6;'>
                        Hello {user.FullName ?? user.Email},
                    </p>
                    <p style='color: #607d8b; line-height: 1.6;'>
                        We received a request to reset your password. Click the button below to choose a new password:
                    </p>
                    <div style='text-align: center; margin: 32px 0;'>
                        <a href='{resetLink}' style='display: inline-block; background: #0288d1; color: white; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: 600;'>
                            Reset Password
                        </a>
                    </div>
                    <p style='color: #607d8b; font-size: 14px;'>
                        If you didn't request a password reset, you can safely ignore this email. Your password will not be changed.
                    </p>
                    <p style='color: #e53935; font-size: 13px;'>
                        ?? This link will expire in 24 hours.
                    </p>
                </div>
                <div style='background: #eceff1; padding: 20px; text-align: center; color: #607d8b; font-size: 13px;'>
                    <p>� 2024 RBM CMMS. All rights reserved.</p>
                </div>
            </div>";

        await SendEmailAsync(email, subject, htmlMessage);
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        var subject = "Your password reset code - RBM CMMS";
        var htmlMessage = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <div style='background: linear-gradient(135deg, #0288d1 0%, #37474f 100%); padding: 40px; text-align: center; color: white;'>
                    <h1 style='margin: 0; font-size: 28px;'>RBM CMMS</h1>
                    <p style='margin: 8px 0 0 0; opacity: 0.9;'>Reliability-Based Maintenance System</p>
                </div>
                <div style='padding: 40px; background: white;'>
                    <h2 style='color: #37474f; margin-top: 0;'>Password Reset Code</h2>
                    <p style='color: #607d8b; line-height: 1.6;'>
                        Hello {user.FullName ?? user.Email},
                    </p>
                    <p style='color: #607d8b; line-height: 1.6;'>
                        Your password reset code is:
                    </p>
                    <div style='background: #eceff1; padding: 20px; text-align: center; margin: 24px 0; border-radius: 8px;'>
                        <span style='font-size: 32px; font-weight: 700; letter-spacing: 4px; color: #37474f; font-family: monospace;'>{resetCode}</span>
                    </div>
                    <p style='color: #607d8b; font-size: 14px;'>
                        Enter this code in the password reset form to continue.
                    </p>
                    <p style='color: #e53935; font-size: 13px;'>
                        ?? This code will expire in 15 minutes.
                    </p>
                </div>
                <div style='background: #eceff1; padding: 20px; text-align: center; color: #607d8b; font-size: 13px;'>
                    <p>� 2024 RBM CMMS. All rights reserved.</p>
                </div>
            </div>";

        await SendEmailAsync(email, subject, htmlMessage);
    }

    private async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Get email configuration from appsettings.json
        var provider = _configuration["Email:Provider"] ?? "Resend";
        var apiKey = _configuration["Email:ResendApiKey"];
        var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@rbmcmms.com";
        var fromName = _configuration["Email:FromName"] ?? "RBM CMMS";

        // For development without API key, just log the email
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogInformation(
                "Email would be sent to {Email} with subject '{Subject}'. Configure Email:ResendApiKey in appsettings.json to enable real email sending.",
                email, subject);
            _logger.LogDebug("Email body: {Body}", htmlMessage);
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.Timeout = TimeSpan.FromSeconds(30);

            var payload = new
            {
                from = string.IsNullOrEmpty(fromName) ? fromEmail : $"{fromName} <{fromEmail}>",
                to = new[] { email },
                subject = subject,
                html = htmlMessage
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Attempting to send email to {Email} via Resend API", email);
            var response = await client.PostAsync("https://api.resend.com/emails", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Email sent successfully to {Email}. Response: {Response}", email, responseBody);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Resend API error sending email to {Email}. Status: {Status}, Response: {Response}", 
                    email, response.StatusCode, errorBody);
                
                if (!_configuration.GetValue<bool>("Email:SkipOnError", true))
                {
                    throw new InvalidOperationException($"Failed to send email: {response.StatusCode} - {errorBody}");
                }
                _logger.LogWarning("Email sending failed but SkipOnError is enabled. User can still use the application.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", email);
            // If SkipOnError is true, log but don't throw - allows registration to continue
            if (_configuration.GetValue<bool>("Email:SkipOnError", true))
            {
                _logger.LogWarning("Email sending skipped due to error. User can still use the application.");
                return;
            }
            throw;
        }
    }
}
