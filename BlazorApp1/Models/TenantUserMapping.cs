using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlazorApp1.Data;
using BlazorApp1.Models;

namespace BlazorApp1.Models;

public class TenantUserMapping
{
    [Key]
    public int Id { get; set; }
    
    public int TenantId { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    
    public bool IsTenantAdmin { get; set; } = false;
    
    public DateTime AssignedDate { get; set; } = DateTime.Now;
    
    public DateTime? RemovedDate { get; set; }
    
    // Navigation properties
    public virtual Tenant? Tenant { get; set; }
    public virtual ApplicationUser? User { get; set; }
}
