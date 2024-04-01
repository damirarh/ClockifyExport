using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Processing.PostProcessors;
using ClockifyExport.Cli.Processing.PreProcessors;

namespace ClockifyExport.Cli.Processing;

/// <summary>
/// Groups time entries from Clockify shared reports.
/// </summary>
public class TimeEntryAggregator : ITimeEntryAggregator
{
    private readonly List<IPreProcessor> preProcessors = [];
    private readonly List<IPostProcessor> postProcessors = [];

    /// <inheritdoc />
    public IReadOnlyCollection<GroupedTimeEntry> Aggregate(
        IEnumerable<ClockifyTimeEntry> timeEntries,
        TimeEntryGrouping grouping
    )
    {
        var groupingSelector = GetGroupingSelector(grouping);
        return timeEntries
            .Select(ExecutePreProcessors)
            .GroupBy(timeEntry => new { timeEntry.Date, Group = groupingSelector(timeEntry) })
            .Select(
                grouping =>
                    new GroupedTimeEntry(
                        grouping.Key.Date,
                        grouping.Key.Group,
                        grouping.Sum(timeEntry => timeEntry.Time.TotalHours),
                        string.Join(
                            Environment.NewLine,
                            grouping.Select(timeEntry => timeEntry.Description)
                        )
                    )
            )
            .Select(ExecutePostProcessors)
            .ToList();
    }

    /// <inheritdoc/>
    public void AddPreProcessor(IPreProcessor preProcessor) => preProcessors.Add(preProcessor);

    /// <inheritdoc/>
    public void AddPostProcessor(IPostProcessor postProcessor) => postProcessors.Add(postProcessor);

    private static Func<ClockifyTimeEntry, string> GetGroupingSelector(TimeEntryGrouping grouping)
    {
        return grouping switch
        {
            TimeEntryGrouping.ByTask => timeEntry => timeEntry.Task ?? string.Empty,
            TimeEntryGrouping.ByProject => timeEntry => timeEntry.Project,
            _ => throw new ArgumentException($"Unknown grouping: {grouping}", nameof(grouping))
        };
    }

    private ClockifyTimeEntry ExecutePreProcessors(ClockifyTimeEntry entry)
    {
        foreach (var preProcessor in preProcessors)
        {
            entry = preProcessor.Process(entry);
        }
        return entry;
    }

    private GroupedTimeEntry ExecutePostProcessors(GroupedTimeEntry entry)
    {
        foreach (var postProcessor in postProcessors)
        {
            entry = postProcessor.Process(entry);
        }
        return entry;
    }
}
