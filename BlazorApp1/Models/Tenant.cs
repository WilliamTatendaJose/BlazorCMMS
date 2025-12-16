using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlazorApp1.Data;

namespace BlazorApp1.Models;

[Table("Tenants")]
public class Tenant
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string TenantCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string ContactPerson { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string ContactPhone { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string ContactEmail { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Active, Inactive, Suspended, Archived
    
    public bool IsActive { get; set; } = true;
    
    public int MaxUsers { get; set; } = 50;
    
    public int MaxAssets { get; set; } = 1000;
    
    public int MaxDocuments { get; set; } = 10000;
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime? ModifiedDate { get; set; }
    
    [MaxLength(200)]
    public string CreatedBy { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string ModifiedBy { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
