using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Services;

/// <summary>
/// Service for managing recurring maintenance schedules.
/// Automatically generates future maintenance schedules based on frequency patterns.
/// </summary>
public class RecurringMaintenanceScheduler
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public RecurringMaintenanceScheduler(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Frequency types supported for recurring schedules
    /// </summary>
    public enum FrequencyType
    {
        Daily,      // Every day
        Weekly,     // Every 7 days
        BiWeekly,   // Every 14 days
        Monthly,    // Every 30 days
        Quarterly,  // Every 90 days
        SemiAnnual, // Every 180 days
        Annually,   // Every 365 days
        Custom      // Custom number of days
    }

    /// <summary>
    /// Gets the number of days for a given frequency type
    /// </summary>
    public int GetFrequencyDays(string frequency, int? customDays = null)
    {
        return frequency switch
        {
            "Daily" => 1,
            "Weekly" => 7,
            "BiWeekly" => 14,
            "Monthly" => 30,
            "Quarterly" => 90,
            "SemiAnnual" => 180,
            "Annually" => 365,
            "Custom" => customDays ?? 30,
            _ => 30 // Default to monthly
        };
    }

    /// <summary>
    /// Generates future schedules for a repeating maintenance task
    /// </summary>
    /// <param name="baseSchedule">The original schedule to use as a template</param>
    /// <param name="numberOfOccurrences">How many future schedules to generate</param>
    /// <returns>List of generated MaintenanceSchedule objects</returns>
    public List<MaintenanceSchedule> GenerateRecurringSchedules(
        MaintenanceSchedule baseSchedule,
        int numberOfOccurrences = 12)
    {
        var generatedSchedules = new List<MaintenanceSchedule>();

        if (string.IsNullOrEmpty(baseSchedule.Frequency) || numberOfOccurrences <= 0)
        {
            return generatedSchedules;
        }

        var frequencyDays = GetFrequencyDays(baseSchedule.Frequency);
        var currentDate = baseSchedule.NextScheduledDate ?? baseSchedule.ScheduledDate;

        for (int i = 1; i <= numberOfOccurrences; i++)
        {
            var nextDate = currentDate.AddDays(frequencyDays * i);

            // Skip if in the past
            if (nextDate < DateTime.Now)
            {
                continue;
            }

            var newSchedule = new MaintenanceSchedule
            {
                AssetId = baseSchedule.AssetId,
                AssetName = baseSchedule.AssetName,
                ScheduledDate = nextDate,
                EndDate = baseSchedule.EndDate.HasValue 
                    ? nextDate.Add(baseSchedule.EndDate.Value - baseSchedule.ScheduledDate) 
                    : null,
                Type = baseSchedule.Type,
                AssignedTechnician = baseSchedule.AssignedTechnician,
                Status = "Scheduled",
                Description = baseSchedule.Description.Length > 0 
                    ? $"{baseSchedule.Description} (Recurring - Occurrence {i})"
                    : $"Recurring {baseSchedule.Frequency.ToLower()} maintenance",
                EstimatedDuration = baseSchedule.EstimatedDuration,
                Frequency = baseSchedule.Frequency,
                NextScheduledDate = nextDate.AddDays(frequencyDays),
                CreatedDate = DateTime.Now,
                CreatedBy = baseSchedule.CreatedBy,
                TenantId = baseSchedule.TenantId
            };

            generatedSchedules.Add(newSchedule);
        }

        return generatedSchedules;
    }

    /// <summary>
    /// Automatically adds generated recurring schedules to the database
    /// </summary>
    /// <param name="baseSchedule">The original schedule to use as a template</param>
    /// <param name="numberOfOccurrences">How many future schedules to generate</param>
    /// <returns>Number of schedules created</returns>
    public async Task<int> CreateRecurringSchedulesAsync(
        MaintenanceSchedule baseSchedule,
        int numberOfOccurrences = 12)
    {
        if (string.IsNullOrEmpty(baseSchedule.Frequency))
        {
            return 0; // No frequency set, don't create recurring schedules
        }

        using var context = _contextFactory.CreateDbContext();
        var generatedSchedules = GenerateRecurringSchedules(baseSchedule, numberOfOccurrences);

        if (generatedSchedules.Count == 0)
        {
            return 0;
        }

        // Add all generated schedules
        context.MaintenanceSchedules.AddRange(generatedSchedules);
        
        // Update the base schedule's next scheduled date
        if (baseSchedule.NextScheduledDate == null)
        {
            var frequencyDays = GetFrequencyDays(baseSchedule.Frequency);
            baseSchedule.NextScheduledDate = baseSchedule.ScheduledDate.AddDays(frequencyDays);
            context.MaintenanceSchedules.Update(baseSchedule);
        }

        await context.SaveChangesAsync();
        return generatedSchedules.Count;
    }

    /// <summary>
    /// Gets all schedules that need recurring schedules generated
    /// </summary>
    /// <returns>List of schedules with frequency patterns</returns>
    public async Task<List<MaintenanceSchedule>> GetRecurringSchedulesAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.MaintenanceSchedules
            .Where(s => !string.IsNullOrEmpty(s.Frequency) && s.Frequency != "")
            .OrderBy(s => s.ScheduledDate)
            .ToListAsync();
    }

    /// <summary>
    /// Processes all overdue recurring schedules and generates next occurrences
    /// Should be called by a background job/timer
    /// </summary>
    /// <returns>Number of new schedules created</returns>
    public async Task<int> ProcessRecurringSchedulesAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var recurringSchedules = await context.MaintenanceSchedules
            .Where(s => !string.IsNullOrEmpty(s.Frequency) && 
                       s.Status == "Completed" &&
                       s.NextScheduledDate.HasValue)
            .ToListAsync();

        int totalCreated = 0;

        foreach (var schedule in recurringSchedules)
        {
            // Check if next scheduled date has arrived
            if (schedule.NextScheduledDate.Value <= DateTime.Now)
            {
                var frequencyDays = GetFrequencyDays(schedule.Frequency);
                
                // Create new schedule for the next occurrence
                var newSchedule = new MaintenanceSchedule
                {
                    AssetId = schedule.AssetId,
                    AssetName = schedule.AssetName,
                    ScheduledDate = schedule.NextScheduledDate.Value,
                    EndDate = schedule.EndDate.HasValue
                        ? schedule.NextScheduledDate.Value.Add(schedule.EndDate.Value - schedule.ScheduledDate)
                        : null,
                    Type = schedule.Type,
                    AssignedTechnician = schedule.AssignedTechnician,
                    Status = "Scheduled",
                    Description = schedule.Description,
                    EstimatedDuration = schedule.EstimatedDuration,
                    Frequency = schedule.Frequency,
                    NextScheduledDate = schedule.NextScheduledDate.Value.AddDays(frequencyDays),
                    CreatedDate = DateTime.Now,
                    CreatedBy = schedule.CreatedBy,
                    TenantId = schedule.TenantId
                };

                context.MaintenanceSchedules.Add(newSchedule);
                totalCreated++;

                // Update the parent schedule's next scheduled date
                schedule.NextScheduledDate = schedule.NextScheduledDate.Value.AddDays(frequencyDays);
                context.MaintenanceSchedules.Update(schedule);
            }
        }

        if (totalCreated > 0)
        {
            await context.SaveChangesAsync();
        }

        return totalCreated;
    }

    /// <summary>
    /// Calculates the next scheduled date for a maintenance item
    /// </summary>
    public DateTime CalculateNextScheduleDate(
        DateTime currentDate,
        string frequency,
        int? customDays = null)
    {
        var frequencyDays = GetFrequencyDays(frequency, customDays);
        return currentDate.AddDays(frequencyDays);
    }

    /// <summary>
    /// Gets scheduling information for a recurring schedule
    /// </summary>
    public SchedulingInfo GetSchedulingInfo(MaintenanceSchedule schedule)
    {
        var frequencyDays = GetFrequencyDays(schedule.Frequency);
        var nextDate = schedule.NextScheduledDate ?? schedule.ScheduledDate.AddDays(frequencyDays);

        return new SchedulingInfo
        {
            CurrentScheduleId = schedule.Id,
            Frequency = schedule.Frequency,
            FrequencyDays = frequencyDays,
            LastScheduledDate = schedule.ScheduledDate,
            NextScheduledDate = nextDate,
            DaysUntilNext = (int)(nextDate - DateTime.Now).TotalDays,
            IsOverdue = nextDate <= DateTime.Now,
            EstimatedDuration = schedule.EstimatedDuration,
            TechnicianName = schedule.AssignedTechnician
        };
    }

    /// <summary>
    /// Gets future occurrences for a schedule
    /// </summary>
    public List<ScheduleOccurrence> GetFutureOccurrences(
        MaintenanceSchedule schedule,
        int numberOfOccurrences = 5)
    {
        var occurrences = new List<ScheduleOccurrence>();
        var frequencyDays = GetFrequencyDays(schedule.Frequency);
        var currentDate = schedule.NextScheduledDate ?? schedule.ScheduledDate;

        for (int i = 1; i <= numberOfOccurrences; i++)
        {
            var nextDate = currentDate.AddDays(frequencyDays * i);

            if (nextDate >= DateTime.Now)
            {
                occurrences.Add(new ScheduleOccurrence
                {
                    OccurrenceNumber = i,
                    ScheduledDate = nextDate,
                    DaysFromNow = (int)(nextDate - DateTime.Now).TotalDays,
                    Status = nextDate <= DateTime.Now ? "Overdue" : "Scheduled"
                });
            }
        }

        return occurrences;
    }

    /// <summary>
    /// Updates frequency for a schedule and regenerates future dates
    /// </summary>
    public async Task UpdateScheduleFrequencyAsync(
        int scheduleId,
        string newFrequency,
        int numberOfOccurrences = 12)
    {
        using var context = _contextFactory.CreateDbContext();
        var schedule = await context.MaintenanceSchedules.FindAsync(scheduleId);

        if (schedule != null)
        {
            schedule.Frequency = newFrequency;
            var frequencyDays = GetFrequencyDays(newFrequency);
            schedule.NextScheduledDate = schedule.ScheduledDate.AddDays(frequencyDays);

            context.MaintenanceSchedules.Update(schedule);
            await context.SaveChangesAsync();

            // Generate new recurring schedules with updated frequency
            await CreateRecurringSchedulesAsync(schedule, numberOfOccurrences);
        }
    }

    /// <summary>
    /// Generates smart schedule based on asset criticality
    /// </summary>
    public int GetRecommendedFrequencyDays(Asset asset)
    {
        // More critical assets get more frequent maintenance
        if (asset.Criticality == "Critical")
            return 14; // BiWeekly

        if (asset.Criticality == "High")
            return 30; // Monthly

        if (asset.Criticality == "Medium")
            return 90; // Quarterly

        return 180; // SemiAnnual for low criticality
    }

    /// <summary>
    /// Gets recommended maintenance frequency based on asset health
    /// </summary>
    public string GetRecommendedFrequency(Asset asset)
    {
        if (asset.HealthScore < 40)
            return "Weekly"; // Very poor health - frequent maintenance

        if (asset.HealthScore < 60)
            return "BiWeekly"; // Poor health

        if (asset.HealthScore < 80)
            return "Monthly"; // Fair health

        if (asset.HealthScore < 90)
            return "Quarterly"; // Good health

        return "Annually"; // Excellent health - minimal maintenance
    }
}

/// <summary>
/// Information about a schedule's recurrence pattern
/// </summary>
public class SchedulingInfo
{
    public int CurrentScheduleId { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public int FrequencyDays { get; set; }
    public DateTime LastScheduledDate { get; set; }
    public DateTime NextScheduledDate { get; set; }
    public int DaysUntilNext { get; set; }
    public bool IsOverdue { get; set; }
    public double EstimatedDuration { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
}

/// <summary>
/// Details about a specific occurrence in a recurring schedule
/// </summary>
public class ScheduleOccurrence
{
    public int OccurrenceNumber { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int DaysFromNow { get; set; }
    public string Status { get; set; } = string.Empty; // "Scheduled", "Overdue", "Completed"
}
