using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;
using BlazorApp1.Data;

namespace BlazorApp1.Services;

public class EmailSender : IEmailSender<ApplicationUser>
{
    private readonly ILogger<EmailSender> _logger;
    private readonly IConfiguration _configuration;

    public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
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
                    <p>© 2024 RBM CMMS. All rights reserved.</p>
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
                    <p>© 2024 RBM CMMS. All rights reserved.</p>
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
                    <p>© 2024 RBM CMMS. All rights reserved.</p>
                </div>
            </div>";

        await SendEmailAsync(email, subject, htmlMessage);
    }

    private async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Get email configuration from appsettings.json
        var smtpServer = _configuration["Email:SmtpServer"];
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var smtpUsername = _configuration["Email:SmtpUsername"];
        var smtpPassword = _configuration["Email:SmtpPassword"];
        var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@rbmcmms.com";
        var fromName = _configuration["Email:FromName"] ?? "RBM CMMS";

        // For development, just log the email
        if (string.IsNullOrEmpty(smtpServer))
        {
            _logger.LogInformation(
                "Email would be sent to {Email} with subject '{Subject}'. Configure Email settings in appsettings.json to enable real email sending.",
                email, subject);
            _logger.LogInformation("Email body: {Body}", htmlMessage);
            return;
        }

        try
        {
            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", email);
            throw;
        }
    }
}
