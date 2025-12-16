using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("Documents")]
public class Document
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string DocumentNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty; // Manual, SOP, Drawing, Certificate, Report, Photo, etc.
    
    [MaxLength(100)]
    public string SubCategory { get; set; } = string.Empty; // Technical Manual, Safety SOP, Electrical Drawing, etc.
    
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string FileName { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string FileExtension { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string FileType { get; set; } = string.Empty;
    
    public long FileSizeBytes { get; set; } // in bytes
    
    public long FileSize { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Draft, Under Review, Approved, Active, Archived, Obsolete
    
    [MaxLength(50)]
    public string Version { get; set; } = "1.0";
    
    public int RevisionNumber { get; set; } = 1;
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    public DateTime? ModifiedDate { get; set; }
    
    [MaxLength(100)]
    public string ModifiedBy { get; set; } = string.Empty;
    
    public DateTime? ExpiryDate { get; set; }
    
    [MaxLength(500)]
    public string Tags { get; set; } = string.Empty; // Comma-separated tags for search
    
    [MaxLength(2000)]
    public string Notes { get; set; } = string.Empty; // Additional notes field
    
    public bool IsConfidential { get; set; }
    
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;
    
    public DateTime? EffectiveDate { get; set; }
    
    public DateTime? ReviewDate { get; set; }
    
    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;
    
    public DateTime? ApprovalDate { get; set; }
    
    [MaxLength(50)]
    public string AccessLevel { get; set; } = "Public"; // Default access level
    
    [MaxLength(500)]
    public string AllowedRoles { get; set; } = string.Empty; // Comma-separated roles
    
    public int ViewCount { get; set; }
    
    public int DownloadCount { get; set; }
    
    // References to other entities
    public int? AssetId { get; set; }
    
    public int? WorkOrderId { get; set; }
    
    public int? FailureModeId { get; set; }
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    // Navigation properties
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
    
    [ForeignKey("WorkOrderId")]
    public virtual WorkOrder? WorkOrder { get; set; }
    
    [ForeignKey("FailureModeId")]
    public virtual FailureMode? FailureMode { get; set; }
    
    public virtual ICollection<DocumentAccessLog> AccessLogs { get; set; } = new List<DocumentAccessLog>();
    
    // Computed properties
    [NotMapped]
    public string FileSizeFormatted
    {
        get
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = FileSizeBytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
    
    [NotMapped]
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now;
    
    [NotMapped]
    public bool NeedsReview => ReviewDate.HasValue && ReviewDate.Value < DateTime.Now;
}
