namespace ClockifyExport.Cli.Processing;

/// <summary>
/// Represents a time entry from a Clockify shared report after being grouped by date and either project or task.
/// </summary>
/// <param name="Date">Date of the grouped time entry. Non-parsed string as it appears in the Clockify shared report CSV.</param>
/// <param name="Group">Project or task of the grouped time entry.</param>
/// <param name="Hours">Sum of time for the grouped time entry, expressed as total hours (with decimal part).</param>
/// <param name="Description">All descriptions for the grouped time entry, each in its own line.</param>
internal sealed record class GroupedTimeEntry(
    string Date,
    string Group,
    double Hours,
    string Description
) { }
