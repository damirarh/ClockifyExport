using System.ComponentModel.DataAnnotations;
using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Export;
using ClockifyExport.Cli.Processing;
using ClockifyExport.Cli.Processing.PostProcessors;
using ClockifyExport.Cli.Validation;
using McMaster.Extensions.CommandLineUtils;

namespace ClockifyExport.Cli;

/// <summary>
/// Implements the root/only command of the application.
/// </summary>
/// <param name="apiClient">Injected <see cref="IApiClient"/> instance.</param>
/// <param name="csvParser">Injected <see cref="ICsvParser"/> instance.</param>
/// <param name="timeEntryAggregator">Injected <see cref="ITimeEntryAggregator"/> instance.</param>
/// <param name="exporterProvider">Injected <see cref="ExporterProvider"/> instance.</param>
public class AppCommand(
    IApiClient apiClient,
    ICsvParser csvParser,
    ITimeEntryAggregator timeEntryAggregator,
    ExporterProvider exporterProvider
)
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
    [Option(Description = "Clockify shared report ID.", ShortName = "i")]
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
    /// Number of minutes to round duration up to after grouping.
    /// </summary>
    [FactorOf(60)]
    [Option(Description = "Number of minutes to round duration up to after grouping.")]
    public int? RoundUpTo { get; set; }

    /// <summary>
    /// Export format.
    /// </summary>
    [Required]
    [Option(Description = "Export format.")]
    public ExportFormat? Format { get; set; }

    /// <summary>
    /// Output file.
    /// </summary>
    [Required]
    [Option(Description = "Output file.")]
    public string Output { get; set; } = null!;

    /// <summary>
    /// Called when the command is invoked.
    /// </summary>
    /// <returns>0 on success, non-0 on failure.</returns>
    public async Task<int> OnExecuteAsync()
    {
        var csv = await apiClient.GetSharedReportCsvAsync(
            ReportId,
            StartDate!.Value, // [Required] ensures that this is not null
            EndDate!.Value, // [Required] ensures that this is not null
            ApiKey
        );

        var timeEntries = csvParser.ParseSharedReportCsv(csv);

        if (RoundUpTo.HasValue)
        {
            timeEntryAggregator.AddPostProcessor(new RoundingPostProcessor(RoundUpTo.Value));
        }

        var groupedTimeEntries = timeEntryAggregator.Aggregate(
            timeEntries,
            Grouping!.Value // [Required] ensures that this is not null
        );

        var exporter = exporterProvider.GetExporter(
            Format!.Value // [Required] ensures that this is not null
        );
        var exportedTimeEntries = exporter.Export(groupedTimeEntries);
        await File.WriteAllTextAsync(Output, exportedTimeEntries);

        return 0;
    }
}
