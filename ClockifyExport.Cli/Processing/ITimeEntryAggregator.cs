using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Processing.PostProcessors;

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
    IReadOnlyCollection<GroupedTimeEntry> Aggregate(
        IEnumerable<ClockifyTimeEntry> timeEntries,
        TimeEntryGrouping grouping
    );

    /// <summary>
    /// Adds a post-processor to be executed on time entries after grouping.
    /// </summary>
    /// <param name="postProcessor">Post-processor to be executed on each time entry.</param>
    void AddPostProcessor(IPostProcessor postProcessor);
}
