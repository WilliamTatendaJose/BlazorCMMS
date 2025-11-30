using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("AssetDowntime")]
public class AssetDowntime
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int AssetId { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
    
    /// <summary>
    /// Duration in hours (calculated property)
    /// </summary>
    [NotMapped]
    public double DurationHours => EndTime.HasValue 
        ? (EndTime.Value - StartTime).TotalHours 
        : (DateTime.Now - StartTime).TotalHours;
    
    [Required]
    [MaxLength(100)]
    public string Reason { get; set; } = string.Empty; // Breakdown, Maintenance, Setup, etc.
    
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // Planned, Unplanned
    
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    public int? RelatedWorkOrderId { get; set; }
    
    public decimal ProductionLoss { get; set; } // Units not produced
    
    public decimal FinancialImpact { get; set; } // Estimated cost
    
    [MaxLength(200)]
    public string RecordedBy { get; set; } = string.Empty;
    
    public DateTime RecordedDate { get; set; } = DateTime.Now;
    
    // Navigation properties
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
    
    [ForeignKey("RelatedWorkOrderId")]
    public virtual WorkOrder? RelatedWorkOrder { get; set; }
}
