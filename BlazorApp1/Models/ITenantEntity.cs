namespace BlazorApp1.Models;

/// <summary>
/// Interface for entities that support multi-tenancy.
/// All tenant-scoped entities should implement this interface.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// The tenant identifier. Null for SuperAdmin-created or system entities.
    /// </summary>
    int? TenantId { get; set; }
}
