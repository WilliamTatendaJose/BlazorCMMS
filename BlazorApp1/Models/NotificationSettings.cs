using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

/// <summary>
/// Stores user notification preferences for email and SMS
/// </summary>
[Table("NotificationSettings")]
public class NotificationSettings
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    // Email Notification Settings
    [Required]
    public bool EmailCriticalAlerts { get; set; } = true;

    [Required]
    public bool EmailWorkOrderDue { get; set; } = true;

    [Required]
    public bool EmailMaintenanceSchedule { get; set; } = true;

    [Required]
    public bool EmailWeeklyReport { get; set; } = false;

    // SMS Notification Settings
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public bool SmsCriticalOnly { get; set; } = true;

    // System Settings
    [Required]
    public bool EnableEmailNotifications { get; set; } = true;

    [Required]
    public bool EnableSmsNotifications { get; set; } = false;

    // Quiet Hours (optional - don't send notifications during these hours)
    public TimeOnly? QuietHoursStart { get; set; }
    public TimeOnly? QuietHoursEnd { get; set; }

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Last modified timestamp
    /// </summary>
    public DateTime? ModifiedDate { get; set; }
}
