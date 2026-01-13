using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

/// <summary>
/// Stores user-specific application settings including preferred units
/// </summary>
[Table("UserSettings")]
public class UserSettings
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Unit system preference: "imperial", "metric", or "si"
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string PreferredUnitSystem { get; set; } = "imperial";

    /// <summary>
    /// Individual unit preferences (can override system preference)
    /// </summary>
    [MaxLength(10)]
    public string? TemperatureUnit { get; set; } = null; // "F", "C", or null for system default

    [MaxLength(10)]
    public string? PressureUnit { get; set; } = null; // "PSI", "bar", "Pa", or null for system default

    [MaxLength(10)]
    public string? FlowRateUnit { get; set; } = null; // "GPM", "L/min", "m3/s", or null for system default

    [MaxLength(10)]
    public string? WeightUnit { get; set; } = null; // "lb", "kg", or null for system default

    [MaxLength(10)]
    public string? LengthUnit { get; set; } = null; // "in", "ft", "m", "mm", or null for system default

    [MaxLength(10)]
    public string? DistanceUnit { get; set; } = null; // "mi", "km", or null for system default

    /// <summary>
    /// Theme preference: "light", "dark", "auto"
    /// </summary>
    [MaxLength(10)]
    public string ThemePreference { get; set; } = "auto";

    /// <summary>
    /// Date format preference: "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd"
    /// </summary>
    [MaxLength(20)]
    public string DateFormat { get; set; } = "MM/dd/yyyy";

    /// <summary>
    /// Time format preference: "12h" or "24h"
    /// </summary>
    [MaxLength(10)]
    public string TimeFormat { get; set; } = "12h";

    /// <summary>
    /// Number of decimal places for measurements
    /// </summary>
    public int DecimalPlaces { get; set; } = 2;

    /// <summary>
    /// Enable notifications
    /// </summary>
    public bool EnableNotifications { get; set; } = true;

    /// <summary>
    /// Notification frequency: "immediate", "hourly", "daily"
    /// </summary>
    [MaxLength(20)]
    public string NotificationFrequency { get; set; } = "immediate";

    /// <summary>
    /// Created timestamp
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Last modified timestamp
    /// </summary>
    public DateTime? ModifiedDate { get; set; }
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
}
