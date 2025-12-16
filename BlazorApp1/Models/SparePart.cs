using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("SpareParts")]
public class SparePart
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string PartNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty; // Bearings, Motors, Seals, Electrical, Hydraulic, etc.
    
    [MaxLength(100)]
    public string Manufacturer { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string ManufacturerPartNumber { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Supplier { get; set; } = string.Empty;
    
    public int QuantityInStock { get; set; } = 0;
    
    public int MinimumStockLevel { get; set; } = 0;
    
    public int ReorderPoint { get; set; } = 0;
    
    public int ReorderQuantity { get; set; } = 0;
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitCost { get; set; }
    
    [MaxLength(50)]
    public string Unit { get; set; } = "Each"; // Each, Box, Meter, Liter, etc.
    
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty; // Warehouse location
    
    // Asset-specific or generic
    public bool IsGeneric { get; set; } = true;
    
    // If not generic, link to specific asset
    public int? AssetId { get; set; }
    
    [MaxLength(500)]
    public string CompatibleAssets { get; set; } = string.Empty; // Comma-separated list or description
    
    [MaxLength(500)]
    public string Specifications { get; set; } = string.Empty;
    
    public DateTime? LastRestockDate { get; set; }
    
    public DateTime? LastUsedDate { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "In Stock"; // In Stock, Low Stock, Out of Stock, On Order
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime? ModifiedDate { get; set; }
    
    [MaxLength(200)]
    public string CreatedBy { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    
    // Navigation properties
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
    
    public virtual ICollection<SparePartTransaction> Transactions { get; set; } = new List<SparePartTransaction>();
    
    // Computed property for stock status
    [NotMapped]
    public string StockStatus
    {
        get
        {
            if (QuantityInStock == 0) return "Out of Stock";
            if (QuantityInStock <= MinimumStockLevel) return "Low Stock";
            if (QuantityInStock <= ReorderPoint) return "Reorder Soon";
            return "In Stock";
        }
    }
    
    [NotMapped]
    public bool NeedsReorder => QuantityInStock <= ReorderPoint;
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
}
