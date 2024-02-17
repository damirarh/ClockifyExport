using ClockifyExport.Cli.Clockify;

namespace ClockifyExport.Cli.Processing;

/// <summary>
/// Groups time entries from Clockify shared reports.
/// </summary>
public class TimeEntryAggregator : ITimeEntryAggregator
{
    /// <inheritdoc />
    public List<GroupedTimeEntry> Aggregate(
        IEnumerable<ClockifyTimeEntry> timeEntries,
        TimeEntryGrouping grouping
    )
    {
        var groupingSelector = GetGroupingSelector(grouping);
        return timeEntries
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
            .ToList();
    }

    private static Func<ClockifyTimeEntry, string> GetGroupingSelector(TimeEntryGrouping grouping)
    {
        return grouping switch
        {
            TimeEntryGrouping.ByTask => timeEntry => timeEntry.Task ?? string.Empty,
            TimeEntryGrouping.ByProject => timeEntry => timeEntry.Project,
            _ => throw new ArgumentException($"Unknown grouping: {grouping}", nameof(grouping))
        };
    }
}
