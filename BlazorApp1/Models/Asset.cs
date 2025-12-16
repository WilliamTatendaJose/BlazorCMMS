using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("Assets")]
public class Asset
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string AssetId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string AssetType { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Manufacturer { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string EquipmentManufacturer { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string SerialNumber { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string Status { get; set; } = "Operational";
    
    [StringLength(50)]
    public string Criticality { get; set; } = "Medium";
    
    public DateTime? InstallationDate { get; set; }
    
    public DateTime? LastMaintenanceDate { get; set; }
    
    public DateTime? NextMaintenanceDate { get; set; }
    
    // Restore missing properties for compatibility
    [StringLength(100)]
    public string ModelNumber { get; set; } = string.Empty;
    
    public double HealthScore { get; set; } = 100.0;
    public double Uptime { get; set; } = 100.0;
    public double Downtime { get; set; } = 0.0;
    
    public DateTime? LastMaintenance { get; set; }
    public DateTime? NextScheduledMaintenance { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool IsRetired { get; set; } = false;
    public DateTime? RetirementDate { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? ModifiedDate { get; set; }
    
    public DateTime? ManufactureDate { get; set; }
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    // Computed field
    [NotMapped]
    public string AssetTag => $"{AssetId} - {Name}";
    
    // Computed properties for UI
    [NotMapped]
    public string CriticalityColor => Criticality switch
    {
        "Critical" => "#d32f2f",
        "High" => "#f57c00",
        "Medium" => "#fbc02d",
        "Low" => "#388e3c",
        _ => "#757575"
    };
    
    [NotMapped]
    public string StatusColor => Status switch
    {
        "Healthy" or "Operational" => "#4caf50",
        "Warning" => "#ff9800",
        "Critical" => "#f44336",
        "Retired" => "#9e9e9e",
        _ => "#2196f3"
    };
    
    [NotMapped]
    public bool IsOverdue => NextScheduledMaintenance.HasValue && NextScheduledMaintenance.Value < DateTime.Now;
    
    [NotMapped]
    public int DaysSinceLastMaintenance => LastMaintenance.HasValue 
        ? (DateTime.Now - LastMaintenance.Value).Days 
        : -1;
    
    [NotMapped]
    public int DaysUntilMaintenance => NextScheduledMaintenance.HasValue 
        ? (NextScheduledMaintenance.Value - DateTime.Now).Days 
        : -1;
    
    // Navigation properties
    public virtual ICollection<AssetAttachment> Attachments { get; set; } = new List<AssetAttachment>();
    public virtual ICollection<AssetDowntime> DowntimeRecords { get; set; } = new List<AssetDowntime>();
    public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public virtual ICollection<ConditionReading> ConditionReadings { get; set; } = new List<ConditionReading>();
    public virtual ICollection<FailureMode> FailureModes { get; set; } = new List<FailureMode>();
    public virtual ICollection<ReliabilityMetric> ReliabilityMetrics { get; set; } = new List<ReliabilityMetric>();
}
