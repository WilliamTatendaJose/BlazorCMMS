using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

/// <summary>
/// Legacy Users table - DEPRECATED
/// This table is kept for backward compatibility with work order assignments.
/// New functionality should use ApplicationUser (AspNetUsers) via UserManagementService.
/// 
/// This table is synchronized with AspNetUsers during seeding.
/// </summary>
[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty; // Admin, Reliability Engineer, Planner, Technician
    
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;
    
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime? LastLoginDate { get; set; }
    
    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    /// <summary>
    /// Links to the corresponding ApplicationUser (AspNetUsers) record
    /// </summary>
    [MaxLength(450)]
    public string? AspNetUserId { get; set; }
}
