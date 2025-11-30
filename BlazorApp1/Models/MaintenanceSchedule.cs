using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("MaintenanceSchedules")]
public class MaintenanceSchedule
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int AssetId { get; set; }
    
    [MaxLength(200)]
    public string AssetName { get; set; } = string.Empty;
    
    public DateTime ScheduledDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = "Preventive"; // Preventive, Predictive, Condition-Based
    
    [MaxLength(200)]
    public string AssignedTechnician { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, In Progress, Completed, Cancelled
    
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    public double EstimatedDuration { get; set; } // Hours
    
    public double? ActualDuration { get; set; } // Hours
    
    [MaxLength(50)]
    public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly, Quarterly, Annually
    
    public DateTime? NextScheduledDate { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    [MaxLength(200)]
    public string CreatedBy { get; set; } = string.Empty;
    
    public DateTime? CompletedDate { get; set; }
    
    [MaxLength(2000)]
    public string CompletionNotes { get; set; } = string.Empty;
    
    // Navigation properties
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
    
    public virtual ICollection<MaintenanceTask> MaintenanceTasks { get; set; } = new List<MaintenanceTask>();
}
