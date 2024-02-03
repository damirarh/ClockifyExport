using CsvHelper.Configuration.Attributes;

namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Represents a time entry from a Clockify shared report.
/// </summary>
public class ClockifyTimeEntry
{
    /// <summary>
    /// Date of the time entry. Non-parsed string as in the CSV.
    /// </summary>
    public string Date { get; set; } = null!;

    /// <summary>
    /// Task name.
    /// </summary>
    [Optional]
    public string? Task { get; set; }

    /// <summary>
    /// Project name.
    /// </summary>
    public string Project { get; set; } = null!;

    /// <summary>
    /// Client name.
    /// </summary>
    public string Client { get; set; } = null!;

    /// <summary>
    /// Description of the time entry.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Duration of the time entry.
    /// </summary>
    [Name("Time (h)")]
    public TimeSpan Time { get; set; }
}
