using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("Assets")]
public class Asset
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string AssetId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Criticality { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    public double HealthScore { get; set; }
    
    public DateTime? LastMaintenance { get; set; }
    
    public double Uptime { get; set; }
    
    public double Downtime { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "Healthy"; // Healthy, Warning, Critical
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime? ModifiedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<AssetAttachment> Attachments { get; set; } = new List<AssetAttachment>();
    public virtual ICollection<AssetDowntime> DowntimeRecords { get; set; } = new List<AssetDowntime>();
    public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public virtual ICollection<ConditionReading> ConditionReadings { get; set; } = new List<ConditionReading>();
    public virtual ICollection<FailureMode> FailureModes { get; set; } = new List<FailureMode>();
    public virtual ICollection<ReliabilityMetric> ReliabilityMetrics { get; set; } = new List<ReliabilityMetric>();
}
