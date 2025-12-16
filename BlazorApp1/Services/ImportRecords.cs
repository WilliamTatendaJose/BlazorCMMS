namespace BlazorApp1.Services;

/// <summary>
/// Asset import record for CSV parsing
/// </summary>
public class AssetImportRecord
{
    public string? AssetId { get; set; }
    public string? Name { get; set; }
    public string? ModelNumber { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Location { get; set; }
    public string? Department { get; set; }
    public string? Criticality { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// Work order import record for CSV parsing
/// </summary>
public class WorkOrderImportRecord
{
    public string? WorkOrderId { get; set; }
    public string? AssetId { get; set; }
    public string? Type { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime DueDate { get; set; }
    public string? Description { get; set; }
    public string? CompletedDate { get; set; }
}

/// <summary>
/// Spare part import record for CSV parsing
/// </summary>
public class SparePartImportRecord
{
    public string? PartNumber { get; set; }
    public string? Description { get; set; }
    public string? Manufacturer { get; set; }
    public int QuantityInStock { get; set; }
    public int ReorderPoint { get; set; }
    public decimal UnitCost { get; set; }
    public string? Status { get; set; }
}
