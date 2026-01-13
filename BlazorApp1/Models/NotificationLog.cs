using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

/// <summary>
/// Represents a notification event (email or SMS)
/// </summary>
[Table("NotificationLogs")]
public class NotificationLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NotificationType { get; set; } = string.Empty; // CriticalAlert, WorkOrderDue, MaintenanceSchedule, WeeklyReport

    [Required]
    [MaxLength(20)]
    public string Channel { get; set; } = string.Empty; // Email, SMS, InApp

    [MaxLength(250)]
    public string RecipientAddress { get; set; } = string.Empty; // Email or phone number

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Sent, Failed, Skipped

    public string? ErrorMessage { get; set; }

    [MaxLength(50)]
    public string? ExternalMessageId { get; set; } // For tracking with external services

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? SentDate { get; set; }
    public int? RelatedAssetId { get; set; }
    public int? RelatedWorkOrderId { get; set; }
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
}
