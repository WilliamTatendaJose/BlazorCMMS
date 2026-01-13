using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("DocumentAccessLogs")]
public class DocumentAccessLog
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int DocumentId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ActionType { get; set; } = string.Empty; // View, Download, Edit, Delete, Share
    
    public DateTime AccessDate { get; set; } = DateTime.Now;
    
    [MaxLength(200)]
    public string AccessedBy { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string UserRole { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string IpAddress { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string UserAgent { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    // Navigation property
    [ForeignKey("DocumentId")]
    public virtual Document? Document { get; set; }
}
