using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("AssetAttachments")]
public class AssetAttachment
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int AssetId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string FilePath { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string FileType { get; set; } = string.Empty; // PDF, Image, Document, etc.
    
    public long FileSize { get; set; } // in bytes
    
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // Manual, Photo, Schematic, Warranty, Invoice, etc.
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime UploadedDate { get; set; } = DateTime.Now;
    
    [MaxLength(200)]
    public string UploadedBy { get; set; } = string.Empty;
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    // Navigation property
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
}
