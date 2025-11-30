using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("WorkOrders")]
public class WorkOrder
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string WorkOrderId { get; set; } = $"WO-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
    
    [Required]
    public int AssetId { get; set; }
    
    [MaxLength(200)]
    public string AssetName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical
    
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = "Corrective"; // Preventive, Corrective, Predictive
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Open"; // Open, In Progress, Completed, Cancelled
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    
    public DateTime? StartedDate { get; set; }
    
    public DateTime? CompletedDate { get; set; }
    
    [MaxLength(200)]
    public string AssignedTo { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    public double EstimatedDowntime { get; set; } // Hours
    
    public double? ActualDowntime { get; set; } // Hours
    
    public decimal EstimatedCost { get; set; }
    
    public decimal? ActualCost { get; set; }
    
    [MaxLength(2000)]
    public string CompletionNotes { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string PartsUsed { get; set; } = string.Empty;
    
    public double? LaborHours { get; set; }
    
    // Navigation properties
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
    
    public virtual ICollection<MaintenanceTask> MaintenanceTasks { get; set; } = new List<MaintenanceTask>();
    public virtual ICollection<AssetDowntime> DowntimeRecords { get; set; } = new List<AssetDowntime>();
}
