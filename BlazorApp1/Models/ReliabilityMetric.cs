using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("ReliabilityMetrics")]
public class ReliabilityMetric
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int AssetId { get; set; }
    
    public DateTime MetricDate { get; set; }
    
    /// <summary>
    /// Mean Time Between Failures (hours)
    /// </summary>
    public double MTBF { get; set; }
    
    /// <summary>
    /// Mean Time To Repair (hours)
    /// </summary>
    public double MTTR { get; set; }
    
    /// <summary>
    /// Mean Time To Failure (hours)
    /// </summary>
    public double MTTF { get; set; }
    
    /// <summary>
    /// Availability percentage
    /// </summary>
    public double Availability { get; set; }
    
    /// <summary>
    /// Reliability percentage
    /// </summary>
    public double Reliability { get; set; }
    
    /// <summary>
    /// Overall Equipment Effectiveness
    /// </summary>
    public double OEE { get; set; }
    
    public int FailureCount { get; set; }
    
    public double TotalDowntimeHours { get; set; }
    
    public double TotalUptimeHours { get; set; }
    
    [MaxLength(50)]
    public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly, Yearly
    
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    
    public DateTime CalculatedDate { get; set; } = DateTime.Now;
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    // Navigation property
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
}
