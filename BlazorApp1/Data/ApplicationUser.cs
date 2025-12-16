using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlazorApp1.Models;

namespace BlazorApp1.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [MaxLength(200)]
    public string? FullName { get; set; }
    
    [MaxLength(100)]
    public string? Department { get; set; }
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime? LastLoginDate { get; set; }

    // Multi-tenancy support
    [ForeignKey("Tenant")]
    public int? PrimaryTenantId { get; set; }
    
    public bool IsSuperAdmin { get; set; } = false;
    
    // Navigation properties
    public virtual Tenant? Tenant { get; set; }
    public virtual ICollection<TenantUserMapping> TenantMappings { get; set; } = new List<TenantUserMapping>();
}

