using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("WorkOrders")]
public class WorkOrder
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string WorkOrderId { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string Priority { get; set; } = "Medium";
    
    [StringLength(50)]
    public string Status { get; set; } = "Open";
    
    [StringLength(50)]
    public string Type { get; set; } = "Corrective";
    
    public int AssetId { get; set; }
    
    [StringLength(100)]
    public string RequestedBy { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string AssignedTo { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime? RequestedDate { get; set; }
    
    public DateTime? ScheduledDate { get; set; }
    
    public DateTime? CompletedDate { get; set; }
    
    public DateTime? StartedDate { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public DateTime? TimeSubmitted { get; set; }
    
    public DateTime? TimeDone { get; set; }
    
    public DateTime? TimeCompleted { get; set; }
    
    public decimal EstimatedCost { get; set; }
    
    public decimal? ActualCost { get; set; }
    
    public int EstimatedDowntime { get; set; }
    
    public double ActualDowntime { get; set; }
    
    public double? LaborHours { get; set; }
    
    [StringLength(1000)]
    public string CompletionNotes { get; set; } = string.Empty;
    
    [StringLength(2000)]
    public string DetailsOfWorkCarriedOut { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string CorrectiveAction { get; set; } = string.Empty;
    
    [StringLength(2000)]
    public string AnyOtherDetails { get; set; } = string.Empty;
    
    // Approval tracking
    [StringLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;
    
    public DateTime? ApprovedDate { get; set; }
    
    [StringLength(500)]
    public string ApprovalNotes { get; set; } = string.Empty;
    
    // Rejection tracking
    [StringLength(100)]
    public string RejectedBy { get; set; } = string.Empty;
    
    public DateTime? RejectedDate { get; set; }
    
    [StringLength(500)]
    public string RejectionReason { get; set; } = string.Empty;
    
    // Acknowledgement tracking
    public bool IsAcknowledged { get; set; }
    
    [StringLength(100)]
    public string AcknowledgedBy { get; set; } = string.Empty;
    
    public DateTime? AcknowledgedDate { get; set; }
    
    // Modification tracking
    [StringLength(100)]
    public string LastModifiedBy { get; set; } = string.Empty;
    
    public DateTime? LastModifiedDate { get; set; }
    
    // Location and categorization
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Building { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string Floor { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string SubCategory { get; set; } = string.Empty;
    
    // Work type flags
    public bool IsMechanical { get; set; }
    
    public bool IsElectrical { get; set; }
    
    [StringLength(100)]
    public string JobType { get; set; } = string.Empty;
    
    // Scheduling
    public DateTime? ScheduledStartDate { get; set; }
    
    public DateTime? ScheduledEndDate { get; set; }
    
    public bool RequiresShutdown { get; set; }
    
    // Additional fields for work order management
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string ContactPerson { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string ContactPhone { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string ContactEmail { get; set; } = string.Empty;
    
    public bool LockOutRequired { get; set; }
    
    public bool RequiresSafetyPermit { get; set; }
    
    [StringLength(100)]
    public string SafetyPermitNumber { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string ArtisanName { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string PartsUsed { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string JobNumber { get; set; } = string.Empty;
    
    public bool HousekeepingAffected { get; set; }
    
    [StringLength(500)]
    public string HousekeepingNotes { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string RequestReason { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Originator { get; set; } = string.Empty;
    
    public DateTime? FaultDate { get; set; }
    
   
    public DateTime FaultTime { get; set; } 
    
    // Recurrence fields
    public bool IsRecurring { get; set; }
    
    [StringLength(50)]
    public string RecurrencePattern { get; set; } = string.Empty;
    
    public int RecurrenceInterval { get; set; }
    
    public int? ParentWorkOrderId { get; set; }
    
    // Verification and sign-off fields
    [StringLength(100)]
    public string ArtisanSignature { get; set; } = string.Empty;
    
    [StringLength(2000)]
    public string DetailsOfRequest { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string OriginatorVerification { get; set; } = string.Empty;
    
    public DateTime? OriginatorVerificationDate { get; set; }
    
    [StringLength(100)]
    public string EngineeringForemanVerification { get; set; } = string.Empty;
    
    public DateTime? EngineeringForemanVerificationDate { get; set; }
    
    [StringLength(100)]
    public string HODVerification { get; set; } = string.Empty;
    
    public DateTime? HODVerificationDate { get; set; }

    
    public DateTime HandOverTime { get; set; }
    
    [StringLength(100)]
    public string BreakdownType { get; set; } = string.Empty;

    // Multi-tenancy support
    public int? TenantId { get; set; }
    
    // Computed property for AssetName (read-only)
    [NotMapped]
    public string AssetName => Asset?.Name ?? "";
    
    // Computed properties for UI
    [NotMapped]
    public string StatusColor => Status switch
    {
        "Open" => "#2196f3",
        "In Progress" => "#ff9800",
        "On Hold" => "#9e9e9e",
        "Completed" => "#4caf50",
        "Cancelled" => "#f44336",
        "Pending Approval" => "#fbc02d",
        _ => "#757575"
    };
    
    [NotMapped]
    public string PriorityColor => Priority switch
    {
        "Critical" => "#d32f2f",
        "High" => "#f57c00",
        "Medium" => "#fbc02d",
        "Low" => "#4caf50",
        _ => "#757575"
    };
    
    [NotMapped]
    public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now && Status != "Completed";
    
    [NotMapped]
    public int DaysUntilDue => DueDate.HasValue 
        ? (DueDate.Value - DateTime.Now).Days 
        : 0;
    
    [NotMapped]
    public double? TotalWorkTime => LaborHours;
    
    // Navigation properties
    public virtual Asset? Asset { get; set; }
    public virtual ICollection<MaintenanceTask> MaintenanceTasks { get; set; } = new List<MaintenanceTask>();
    public virtual ICollection<AssetDowntime> DowntimeRecords { get; set; } = new List<AssetDowntime>();
    public virtual ICollection<WorkOrderSpareUsed> SparesUsed { get; set; } = new List<WorkOrderSpareUsed>();
}

// Model for tracking spares used in a work order
[Table("WorkOrderSparesUsed")]
public class WorkOrderSpareUsed
{
    [Key]
    public int Id { get; set; }
    
    public int WorkOrderId { get; set; }
    
    [MaxLength(100)]
    public string RequisitionNumber { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string ItemDescription { get; set; } = string.Empty;
    
    public int Quantity { get; set; } = 1;
    
    public int? SparePartId { get; set; }
    
    [ForeignKey("WorkOrderId")]
    public virtual WorkOrder? WorkOrder { get; set; }
    
    [ForeignKey("SparePartId")]
    public virtual SparePart? SparePart { get; set; }
}
