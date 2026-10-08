using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Logging;
using ClockifyExport.Cli.Processing.PostProcessors;
using ClockifyExport.Cli.Processing.PreProcessors;
using Microsoft.Extensions.Logging;

namespace ClockifyExport.Cli.Processing;

/// <summary>
/// Groups time entries from Clockify shared reports.
/// </summary>
/// <param name="logger">Injected <see cref="ILogger"/> instance.</param>
internal sealed class TimeEntryAggregator(ILogger<TimeEntryAggregator> logger)
    : ITimeEntryAggregator
{
    private readonly List<IPreProcessor> _preProcessors = [];
    private readonly List<IPostProcessor> _postProcessors = [];

    /// <inheritdoc />
    public IReadOnlyCollection<GroupedTimeEntry> Aggregate(
        IEnumerable<ClockifyTimeEntry> timeEntries,
        TimeEntryGrouping grouping
    )
    {
        var groupingSelector = GetGroupingSelector(grouping);
        return timeEntries
            .Select(ExecutePreProcessors)
            .GroupBy(timeEntry =>
            {
                var group = groupingSelector(timeEntry);
                if (string.IsNullOrWhiteSpace(group))
                {
                    logger.TimeEntryValidationError(
                        $"Empty group value. Grouping: {grouping}",
                        timeEntry
                    );
                }
                return new { timeEntry.Date, Group = group };
            })
            .Select(grouped => new GroupedTimeEntry(
                grouped.Key.Date,
                grouped.Key.Group,
                grouped.Sum(timeEntry => timeEntry.Time.TotalHours),
                string.Join(Environment.NewLine, grouped.Select(timeEntry => timeEntry.Description))
            ))
            .Select(ExecutePostProcessors)
            .ToList();
    }

    /// <inheritdoc/>
    public void AddPreProcessor(IPreProcessor preProcessor) => _preProcessors.Add(preProcessor);

    /// <inheritdoc/>
    public void AddPostProcessor(IPostProcessor postProcessor) =>
        _postProcessors.Add(postProcessor);

    private static Func<ClockifyTimeEntry, string> GetGroupingSelector(TimeEntryGrouping grouping)
    {
        return grouping switch
        {
            TimeEntryGrouping.ByTask => timeEntry => timeEntry.Task ?? string.Empty,
            TimeEntryGrouping.ByProject => timeEntry => timeEntry.Project,
            _ => throw new ArgumentException($"Unknown grouping: {grouping}", nameof(grouping)),
        };
    }

    private ClockifyTimeEntry ExecutePreProcessors(ClockifyTimeEntry entry)
    {
        foreach (var preProcessor in _preProcessors)
        {
            entry = preProcessor.Process(entry, out var validationError);
            if (validationError != null)
            {
                logger.TimeEntryValidationError(validationError, entry);
            }
        }
        return entry;
    }

    private GroupedTimeEntry ExecutePostProcessors(GroupedTimeEntry entry)
    {
        foreach (var postProcessor in _postProcessors)
        {
            entry = postProcessor.Process(entry);
        }
        return entry;
    }
}
