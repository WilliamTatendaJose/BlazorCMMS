using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("FailureModes")]
public class FailureMode
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int AssetId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Mode { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Cause { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Effect { get; set; } = string.Empty;
    
    [Range(1, 10)]
    public int Severity { get; set; } = 5;
    
    [Range(1, 10)]
    public int Occurrence { get; set; } = 5;
    
    [Range(1, 10)]
    public int Detection { get; set; } = 5;
    
    [NotMapped]
    public int RPN => Severity * Occurrence * Detection;
    
    [MaxLength(1000)]
    public string CurrentControls { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string RecommendedActions { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string ResponsiblePerson { get; set; } = string.Empty;
    
    public DateTime? TargetCompletionDate { get; set; }
    
    [MaxLength(2000)]
    public string ActionsTaken { get; set; } = string.Empty;
    
    public int? RevisedSeverity { get; set; }
    
    public int? RevisedOccurrence { get; set; }
    
    public int? RevisedDetection { get; set; }
    
    [NotMapped]
    public int? RevisedRPN => RevisedSeverity.HasValue && RevisedOccurrence.HasValue && RevisedDetection.HasValue
        ? RevisedSeverity.Value * RevisedOccurrence.Value * RevisedDetection.Value
        : null;
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime? ModifiedDate { get; set; }
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    // Navigation property
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
}
