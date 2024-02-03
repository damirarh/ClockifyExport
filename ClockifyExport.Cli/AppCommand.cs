using System.ComponentModel.DataAnnotations;
using ClockifyExport.Cli.Clockify;
using McMaster.Extensions.CommandLineUtils;

namespace ClockifyExport.Cli;

/// <summary>
/// Implements the root/only command of the application.
/// </summary>
/// <param name="clockifyService">Injected <see cref="IClockifyService"/> instance.</param>
public class AppCommand(IClockifyService clockifyService)
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
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Report end date (inclusive).
    /// </summary>
    [Required]
    [Option(Description = "Report end date (inclusive).")]
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Called when the command is invoked.
    /// </summary>
    /// <returns>0 on success, non-0 on failure.</returns>
    public async Task<int> OnExecuteAsync()
    {
        var csv = await clockifyService.GetSharedReportAsCsv(ReportId, StartDate, EndDate, ApiKey);

        return 0;
    }
}
