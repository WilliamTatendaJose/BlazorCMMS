namespace BlazorApp1.Models;

/// <summary>
/// Enumeration of notification types
/// </summary>
public enum NotificationType
{
    CriticalAlert,
    WorkOrderDue,
    MaintenanceSchedule,
    WeeklyReport,
    AssetFailure,
    ConditionWarning
}

/// <summary>
/// Enumeration of notification channels
/// </summary>
public enum NotificationChannel
{
    Email,
    SMS,
    InApp
}

/// <summary>
/// Enumeration of notification statuses
/// </summary>
public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Skipped,
    Read
}
