using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("MaintenanceTasks")]
public class MaintenanceTask
{
    [Key]
    public int Id { get; set; }
    
    public int? WorkOrderId { get; set; }
    
    public int? ScheduleId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string TaskName { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed, Skipped
    
    public int Sequence { get; set; } // Order of execution
    
    public double EstimatedDuration { get; set; } // Hours
    
    public double? ActualDuration { get; set; } // Hours
    
    [MaxLength(200)]
    public string AssignedTo { get; set; } = string.Empty;
    
    public DateTime? StartedDate { get; set; }
    
    public DateTime? CompletedDate { get; set; }
    
    [MaxLength(1000)]
    public string ToolsRequired { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string PartsRequired { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string SafetyLevel { get; set; } = string.Empty; // Low, Medium, High
    
    [MaxLength(2000)]
    public string SafetyPrecautions { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string CompletionNotes { get; set; } = string.Empty;
    
    public bool IsCompleted { get; set; }
    
    [MaxLength(200)]
    public string CompletedBy { get; set; } = string.Empty;
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    // Navigation properties
    [ForeignKey("WorkOrderId")]
    public virtual WorkOrder? WorkOrder { get; set; }
    
    [ForeignKey("ScheduleId")]
    public virtual MaintenanceSchedule? Schedule { get; set; }
}
