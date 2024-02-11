using System.ComponentModel.DataAnnotations;
using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Processing;
using McMaster.Extensions.CommandLineUtils;

namespace ClockifyExport.Cli;

/// <summary>
/// Implements the root/only command of the application.
/// </summary>
/// <param name="clockifyService">Injected <see cref="IClockifyService"/> instance.</param>
public class AppCommand(IClockifyService clockifyService, TimeEntryAggregator timeEntryAggregator)
{
    /// <summary>
    /// Clockify API key.
    /// </summary>
    [Required]
    [Option(Description = "Clockify API key.")]
    public string ApiKey { get; set; } = null!;

    /// <summary>
    /// Clockify shared report ID.
    /// </summary>
    [Required]
    [Option(Description = "Clockify shared report ID.")]
    public string ReportId { get; set; } = null!;

    /// <summary>
    /// Report start date (inclusive).
    /// </summary>
    [Required]
    [Option(Description = "Report start date (inclusive).")]
    public DateOnly? StartDate { get; set; } = null!;

    /// <summary>
    /// Report end date (inclusive).
    /// </summary>
    [Required]
    [Option(Description = "Report end date (inclusive).")]
    public DateOnly? EndDate { get; set; } = null!;

    /// <summary>
    /// Column to group by time entries within a day.
    /// </summary>
    [Required]
    [Option(Description = "Column to group by time entries within a day.")]
    public TimeEntryGrouping? Grouping { get; set; }

    /// <summary>
    /// Called when the command is invoked.
    /// </summary>
    /// <returns>0 on success, non-0 on failure.</returns>
    public async Task<int> OnExecuteAsync()
    {
        var timeEntries = await clockifyService.GetSharedReport(
            ReportId,
            StartDate!.Value, // [Required] ensures that this is not null
            EndDate!.Value, // [Required] ensures that this is not null
            ApiKey
        );

        var groupedTimeEntries = timeEntryAggregator.Aggregate(
            timeEntries,
            Grouping!.Value // [Required] ensures that this is not null
        );

        return 0;
    }
}
