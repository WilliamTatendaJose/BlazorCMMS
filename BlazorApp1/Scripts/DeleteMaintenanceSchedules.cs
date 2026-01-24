// Script to Delete All MaintenanceSchedules Data
// Location: BlazorApp1/Scripts/DeleteMaintenanceSchedules.cs

using BlazorApp1.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Scripts;

/// <summary>
/// Script to safely delete all MaintenanceSchedules data
/// IMPORTANT: Make a backup before running!
/// </summary>
public class DeleteMaintenanceSchedulesScript
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DeleteMaintenanceSchedulesScript(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Delete all MaintenanceSchedules records
    /// </summary>
    public async Task DeleteAllSchedulesAsync()
    {
        using var context = _contextFactory.CreateDbContext();

        try
        {
            // Get count before deletion
            var count = await context.MaintenanceSchedules.CountAsync();
            Console.WriteLine($"Found {count} MaintenanceSchedules to delete...");

            if (count == 0)
            {
                Console.WriteLine("No schedules to delete.");
                return;
            }

            // Delete all schedules
            await context.MaintenanceSchedules.ExecuteDeleteAsync();

            Console.WriteLine($"✅ Successfully deleted {count} MaintenanceSchedules.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting schedules: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Delete schedules for a specific asset only
    /// </summary>
    public async Task DeleteSchedulesByAssetAsync(int assetId)
    {
        using var context = _contextFactory.CreateDbContext();

        try
        {
            var count = await context.MaintenanceSchedules
                .Where(s => s.AssetId == assetId)
                .ExecuteDeleteAsync();

            Console.WriteLine($"✅ Deleted {count} schedules for Asset ID: {assetId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Delete schedules by status
    /// </summary>
    public async Task DeleteSchedulesByStatusAsync(string status)
    {
        using var context = _contextFactory.CreateDbContext();

        try
        {
            var count = await context.MaintenanceSchedules
                .Where(s => s.Status == status)
                .ExecuteDeleteAsync();

            Console.WriteLine($"✅ Deleted {count} schedules with status: {status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Delete schedules older than specified date
    /// </summary>
    public async Task DeleteSchedulesBeforeDateAsync(DateTime cutoffDate)
    {
        using var context = _contextFactory.CreateDbContext();

        try
        {
            var count = await context.MaintenanceSchedules
                .Where(s => s.ScheduledDate < cutoffDate)
                .ExecuteDeleteAsync();

            Console.WriteLine($"✅ Deleted {count} schedules before {cutoffDate:yyyy-MM-dd}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Get count without deleting (safe preview)
    /// </summary>
    public async Task<int> GetScheduleCountAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.MaintenanceSchedules.CountAsync();
    }

    /// <summary>
    /// Get count by status
    /// </summary>
    public async Task<Dictionary<string, int>> GetScheduleCountByStatusAsync()
    {
        using var context = _contextFactory.CreateDbContext();

        var counts = await context.MaintenanceSchedules
            .GroupBy(s => s.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return counts.ToDictionary(x => x.Status, x => x.Count);
    }
}
