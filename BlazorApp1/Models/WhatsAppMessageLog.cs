using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

/// <summary>
/// WhatsApp message log for tracking communication history
/// </summary>
[Table("WhatsAppMessageLogs")]
public class WhatsAppMessageLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = "";

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = "";

    public WhatsAppMessageDirection Direction { get; set; }

    public WhatsAppMessageType MessageType { get; set; }

    public WhatsAppMessageStatus Status { get; set; }

    [MaxLength(100)]
    public string? ExternalMessageId { get; set; }

    public int? RelatedEntityId { get; set; }

    [MaxLength(50)]
    public string? RelatedEntityType { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public DateTime? DeliveredAt { get; set; }

    public DateTime? ReadAt { get; set; }

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }

    // Multi-tenancy support
    public int? TenantId { get; set; }
}

/// <summary>
/// WhatsApp message direction
/// </summary>
public enum WhatsAppMessageDirection
{
    Incoming,
    Outgoing
}

/// <summary>
/// Types of WhatsApp messages
/// </summary>
public enum WhatsAppMessageType
{
    General,
    WorkOrderAssignment,
    Reminder,
    CriticalAlert,
    MaintenanceSchedule,
    StatusUpdate,
    Escalation,
    Confirmation
}

/// <summary>
/// WhatsApp message delivery status
/// </summary>
public enum WhatsAppMessageStatus
{
    Pending,
    Sent,
    Delivered,
    Read,
    Failed
}
