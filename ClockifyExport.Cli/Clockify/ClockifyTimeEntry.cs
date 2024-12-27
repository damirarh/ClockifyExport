using CsvHelper.Configuration.Attributes;

namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Represents a time entry from a Clockify shared report.
/// </summary>
internal sealed record class ClockifyTimeEntry
{
    /// <summary>
    /// Date of the time entry. Non-parsed string as in the CSV.
    /// </summary>
    public required string Date { get; init; }

    /// <summary>
    /// Task name.
    /// </summary>
    [Optional]
    public string? Task { get; init; }

    /// <summary>
    /// Project name.
    /// </summary>
    public required string Project { get; init; }

    /// <summary>
    /// Client name.
    /// </summary>
    public required string Client { get; init; }

    /// <summary>
    /// Description of the time entry.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Duration of the time entry.
    /// </summary>
    [Name("Time (h)")]
    public required TimeSpan Time { get; init; }
}
