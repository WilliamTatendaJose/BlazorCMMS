// Console Program to Delete MaintenanceSchedules
// Run this as: dotnet run --project BlazorApp1 -- delete-schedules

using BlazorApp1.Data;
using BlazorApp1.Scripts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp1.ConsoleTools;

/// <summary>
/// Console application entry point for maintenance operations
/// </summary>
public class MaintenanceScriptRunner
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     Maintenance Schedules Management Tool                  ║");
        Console.WriteLine("║     ⚠️  CAUTION: This tool modifies database               ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // Setup dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            var command = args[0].ToLower();

            switch (command)
            {
                case "delete-all":
                    await DeleteAllSchedules(serviceProvider);
                    break;

                case "delete-by-status":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("❌ Please provide status: delete-by-status <status>");
                        Console.WriteLine("   Example: delete-by-status Completed");
                        return;
                    }
                    await DeleteByStatus(serviceProvider, args[1]);
                    break;

                case "delete-by-asset":
                    if (args.Length < 2 || !int.TryParse(args[1], out var assetId))
                    {
                        Console.WriteLine("❌ Please provide asset ID: delete-by-asset <id>");
                        Console.WriteLine("   Example: delete-by-asset 5");
                        return;
                    }
                    await DeleteByAsset(serviceProvider, assetId);
                    break;

                case "delete-before-date":
                    if (args.Length < 2 || !DateTime.TryParse(args[1], out var cutoffDate))
                    {
                        Console.WriteLine("❌ Please provide date: delete-before-date <date>");
                        Console.WriteLine("   Example: delete-before-date 2024-01-01");
                        return;
                    }
                    await DeleteBeforeDate(serviceProvider, cutoffDate);
                    break;

                case "count":
                    await ShowCount(serviceProvider);
                    break;

                case "count-by-status":
                    await ShowCountByStatus(serviceProvider);
                    break;

                case "help":
                case "-h":
                case "--help":
                    ShowHelp();
                    break;

                default:
                    Console.WriteLine($"❌ Unknown command: {command}");
                    Console.WriteLine();
                    ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    ERROR OCCURRED                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"❌ {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Inner Exception:");
            Console.WriteLine(ex.InnerException?.Message);
            Environment.Exit(1);
        }
    }

    private static async Task DeleteAllSchedules(ServiceProvider serviceProvider)
    {
        var script = serviceProvider.GetRequiredService<DeleteMaintenanceSchedulesScript>();

        Console.WriteLine("⚠️  WARNING: This will delete ALL MaintenanceSchedules records");
        Console.WriteLine("    This action cannot be undone without a database backup!");
        Console.WriteLine();

        // Show count first
        var count = await script.GetScheduleCountAsync();
        Console.WriteLine($"📊 Current schedules: {count}");
        Console.WriteLine();

        // Confirm deletion
        Console.Write("Are you SURE you want to delete all {0} schedules? (yes/NO): ", count);
        var response = Console.ReadLine()?.ToLower() ?? "no";

        if (response != "yes")
        {
            Console.WriteLine("❌ Deletion cancelled.");
            return;
        }

        Console.Write("Type 'DELETE' to confirm: ");
        var confirmation = Console.ReadLine();

        if (confirmation != "DELETE")
        {
            Console.WriteLine("❌ Deletion cancelled.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("⏳ Deleting all schedules...");
        await script.DeleteAllSchedulesAsync();

        // Verify
        var remaining = await script.GetScheduleCountAsync();
        Console.WriteLine($"✅ Verification: {remaining} schedules remaining");
    }

    private static async Task DeleteByStatus(ServiceProvider serviceProvider, string status)
    {
        var script = serviceProvider.GetRequiredService<DeleteMaintenanceSchedulesScript>();

        Console.WriteLine($"⚠️  WARNING: This will delete all schedules with status '{status}'");
        Console.WriteLine();

        // Show count by status
        var countByStatus = await script.GetScheduleCountByStatusAsync();
        if (!countByStatus.ContainsKey(status))
        {
            Console.WriteLine($"❌ No schedules found with status: {status}");
            Console.WriteLine($"Available statuses: {string.Join(", ", countByStatus.Keys)}");
            return;
        }

        var count = countByStatus[status];
        Console.WriteLine($"📊 Schedules with status '{status}': {count}");
        Console.WriteLine();

        Console.Write($"Are you sure? (yes/NO): ");
        var response = Console.ReadLine()?.ToLower() ?? "no";

        if (response != "yes")
        {
            Console.WriteLine("❌ Deletion cancelled.");
            return;
        }

        await script.DeleteSchedulesByStatusAsync(status);
    }

    private static async Task DeleteByAsset(ServiceProvider serviceProvider, int assetId)
    {
        var script = serviceProvider.GetRequiredService<DeleteMaintenanceSchedulesScript>();

        Console.WriteLine($"⚠️  WARNING: This will delete all schedules for Asset ID {assetId}");
        Console.WriteLine();

        Console.Write("Are you sure? (yes/NO): ");
        var response = Console.ReadLine()?.ToLower() ?? "no";

        if (response != "yes")
        {
            Console.WriteLine("❌ Deletion cancelled.");
            return;
        }

        await script.DeleteSchedulesByAssetAsync(assetId);
    }

    private static async Task DeleteBeforeDate(ServiceProvider serviceProvider, DateTime cutoffDate)
    {
        var script = serviceProvider.GetRequiredService<DeleteMaintenanceSchedulesScript>();

        Console.WriteLine($"⚠️  WARNING: This will delete all schedules before {cutoffDate:yyyy-MM-dd}");
        Console.WriteLine();

        Console.Write("Are you sure? (yes/NO): ");
        var response = Console.ReadLine()?.ToLower() ?? "no";

        if (response != "yes")
        {
            Console.WriteLine("❌ Deletion cancelled.");
            return;
        }

        await script.DeleteSchedulesBeforeDateAsync(cutoffDate);
    }

    private static async Task ShowCount(ServiceProvider serviceProvider)
    {
        var script = serviceProvider.GetRequiredService<DeleteMaintenanceSchedulesScript>();
        var count = await script.GetScheduleCountAsync();

        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              MaintenanceSchedules Count Report              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"📊 Total schedules: {count}");
    }

    private static async Task ShowCountByStatus(ServiceProvider serviceProvider)
    {
        var script = serviceProvider.GetRequiredService<DeleteMaintenanceSchedulesScript>();
        var countByStatus = await script.GetScheduleCountByStatusAsync();

        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║           MaintenanceSchedules Count by Status              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        if (countByStatus.Count == 0)
        {
            Console.WriteLine("No schedules found.");
            return;
        }

        var total = countByStatus.Values.Sum();
        foreach (var kvp in countByStatus.OrderByDescending(x => x.Value))
        {
            var percentage = (kvp.Value * 100) / total;
            var bar = string.Concat(Enumerable.Repeat("█", kvp.Value / 5));
            Console.WriteLine($"{kvp.Key,-15} : {kvp.Value,4} ({percentage,3}%) {bar}");
        }

        Console.WriteLine();
        var paddedTotal = "Total".PadRight(15);
        Console.WriteLine($"{paddedTotal} : {total,4}");
    }

    private static void ShowHelp()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    AVAILABLE COMMANDS                      ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("📋 Information Commands:");
        Console.WriteLine("  count               Show total number of schedules");
        Console.WriteLine("  count-by-status     Show count grouped by status");
        Console.WriteLine();
        Console.WriteLine("🗑️  Deletion Commands (⚠️ DESTRUCTIVE):");
        Console.WriteLine("  delete-all          Delete ALL schedules");
        Console.WriteLine("  delete-by-status    Delete schedules by status");
        Console.WriteLine("                      Usage: delete-by-status <status>");
        Console.WriteLine("                      Example: delete-by-status Completed");
        Console.WriteLine();
        Console.WriteLine("  delete-by-asset     Delete schedules for specific asset");
        Console.WriteLine("                      Usage: delete-by-asset <assetId>");
        Console.WriteLine("                      Example: delete-by-asset 5");
        Console.WriteLine();
        Console.WriteLine("  delete-before-date  Delete schedules before date");
        Console.WriteLine("                      Usage: delete-before-date <date>");
        Console.WriteLine("                      Example: delete-before-date 2024-01-01");
        Console.WriteLine();
        Console.WriteLine("ℹ️  Other:");
        Console.WriteLine("  help                Show this help message");
        Console.WriteLine();
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     EXAMPLE USAGE                         ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("# Preview without deleting:");
        Console.WriteLine("  dotnet run count");
        Console.WriteLine("  dotnet run count-by-status");
        Console.WriteLine();
        Console.WriteLine("# Delete data:");
        Console.WriteLine("  dotnet run delete-all");
        Console.WriteLine("  dotnet run delete-by-status Completed");
        Console.WriteLine("  dotnet run delete-by-asset 5");
        Console.WriteLine("  dotnet run delete-before-date 2024-01-01");
        Console.WriteLine();
        Console.WriteLine("⚠️  IMPORTANT: Always backup your database before deletion!");
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Add DbContext
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=BlazorApp1;Trusted_Connection=true;";
        
        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Add deletion script service
        services.AddScoped<DeleteMaintenanceSchedulesScript>();
    }
}
