using ClockifyExport.Cli.Clockify;

namespace ClockifyExport.Cli.Processing;

/// <summary>
/// Groups time entries from Clockify shared reports.
/// </summary>
public interface ITimeEntryAggregator
{
    /// <summary>
    /// Groups time entries by date and either project or task.
    /// </summary>
    /// <param name="timeEntries">Time entries to group.</param>
    /// <param name="grouping">Indicates how time entries should be grouped.</param>
    /// <returns>Grouped time entries.</returns>
    List<GroupedTimeEntry> Aggregate(
        IEnumerable<ClockifyTimeEntry> timeEntries,
        TimeEntryGrouping grouping
    );
}
