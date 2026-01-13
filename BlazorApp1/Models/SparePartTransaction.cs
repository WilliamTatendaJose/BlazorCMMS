using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("SparePartTransactions")]
public class SparePartTransaction
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int SparePartId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty; // Issue, Return, Restock, Adjustment, Transfer
    
    public int Quantity { get; set; }
    
    public int StockBefore { get; set; }
    
    public int StockAfter { get; set; }
    
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    
    [MaxLength(200)]
    public string IssuedTo { get; set; } = string.Empty; // User or technician
    
    public int? WorkOrderId { get; set; }
    
    public int? AssetId { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? UnitCostAtTransaction { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalCost { get; set; }
    
    [MaxLength(200)]
    public string TransactionBy { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string ReferenceNumber { get; set; } = string.Empty; // PO number, invoice number, etc.
    
    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    // Navigation properties
    [ForeignKey("SparePartId")]
    public virtual SparePart? SparePart { get; set; }
    
    [ForeignKey("WorkOrderId")]
    public virtual WorkOrder? WorkOrder { get; set; }
    
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
}
