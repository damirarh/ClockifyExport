using System.ComponentModel.DataAnnotations;
using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Export;
using ClockifyExport.Cli.Processing;
using ClockifyExport.Cli.Processing.PostProcessors;
using ClockifyExport.Cli.Processing.PreProcessors;
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
internal sealed class AppCommand(
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
    public required string ApiKey { get; init; }

    /// <summary>
    /// Clockify shared report ID.
    /// </summary>
    [Required]
    [Option(Description = "Clockify shared report ID.", ShortName = "i")]
    public required string ReportId { get; init; }

    /// <summary>
    /// Report start date (inclusive).
    /// </summary>
    [Required]
    [Option(Description = "Report start date (inclusive).")]
    public required DateOnly? StartDate { get; init; }

    /// <summary>
    /// Report end date (inclusive).
    /// </summary>
    [Required]
    [Option(Description = "Report end date (inclusive).")]
    public required DateOnly? EndDate { get; init; }

    /// <summary>
    /// Column to group by time entries within a day.
    /// </summary>
    [Required]
    [Option(Description = "Column to group by time entries within a day.")]
    public required TimeEntryGrouping? Grouping { get; init; }

    /// <summary>
    /// Number of minutes to round duration up to after grouping.
    /// </summary>
    [FactorOf(60)]
    [Option(Description = "Number of minutes to round duration up to after grouping.")]
    public required int? RoundUpTo { get; init; }

    /// <summary>
    /// Regex to use for task id parsing.
    /// </summary>
    [Option(Description = "Regex to use for task id parsing.")]
    public required string? TaskIdRegex { get; init; }

    /// <summary>
    /// Export format.
    /// </summary>
    [Required]
    [Option(Description = "Export format.")]
    public required ExportFormat? Format { get; init; }

    /// <summary>
    /// Output file.
    /// </summary>
    [Required]
    [Option(Description = "Output file.")]
    public required string Output { get; init; }

    /// <summary>
    /// Base URL of the Clockify Reports API.
    /// </summary>
    [Option(Description = "Base URL of the Clockify Reports API.")]
    public required Uri BaseUrl { get; init; } = new("https://reports.api.clockify.me");

    /// <summary>
    /// Called when the command is invoked.
    /// </summary>
    /// <returns>0 on success, non-0 on failure.</returns>
    public async Task<int> OnExecuteAsync()
    {
        var csv = await apiClient
            .GetSharedReportCsvAsync(
                BaseUrl,
                ReportId,
                StartDate!.Value, // [Required] ensures that this is not null
                EndDate!.Value, // [Required] ensures that this is not null
                ApiKey
            )
            .ConfigureAwait(false);

        var timeEntries = csvParser.ParseSharedReportCsv(csv);

        if (RoundUpTo.HasValue)
        {
            timeEntryAggregator.AddPostProcessor(new RoundingPostProcessor(RoundUpTo.Value));
        }

        if (!string.IsNullOrEmpty(TaskIdRegex))
        {
            timeEntryAggregator.AddPreProcessor(new ParseTaskPreProcessor(TaskIdRegex));
        }

        var groupedTimeEntries = timeEntryAggregator.Aggregate(
            timeEntries,
            Grouping!.Value // [Required] ensures that this is not null
        );

        var exporter = exporterProvider.GetExporter(
            Format!.Value // [Required] ensures that this is not null
        );
        var exportedTimeEntries = exporter.Export(groupedTimeEntries);
        await File.WriteAllTextAsync(Output, exportedTimeEntries).ConfigureAwait(false);

        return 0;
    }
}
