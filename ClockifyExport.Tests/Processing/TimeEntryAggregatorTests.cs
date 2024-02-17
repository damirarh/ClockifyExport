using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Processing;
using FluentAssertions;

namespace ClockifyExport.Tests.Processing;

public class TimeEntryAggregatorTests
{
    private static readonly List<ClockifyTimeEntry> timeEntries =
    [
        CreateClockifyTimeEntry("2024-01-01", "TA", "P1", "C1", "D1A1", 0.5),
        CreateClockifyTimeEntry("2024-01-01", "TA", "P1", "C1", "D1A2", 0.5),
        CreateClockifyTimeEntry("2024-01-01", "TB", "P1", "C1", "D1B1", 0.5),
        CreateClockifyTimeEntry("2024-01-01", "TB", "P1", "C1", "D1B2", 0.5),
        CreateClockifyTimeEntry("2024-01-01", "TA", "P2", "C1", "D2A1", 0.5),
        CreateClockifyTimeEntry("2024-01-01", "TA", "P2", "C1", "D2A2", 0.5),
        CreateClockifyTimeEntry("2024-01-01", "TC", "P2", "C1", "D2C1", 0.5),
        CreateClockifyTimeEntry("2024-01-01", "TC", "P2", "C1", "D2C2", 0.5),
        CreateClockifyTimeEntry("2024-01-02", "TA", "P1", "C1", "D1A", 0.5),
        CreateClockifyTimeEntry("2024-01-02", "TB", "P1", "C1", "D1B", 0.5),
        CreateClockifyTimeEntry("2024-01-02", "TA", "P2", "C1", "D2A", 0.5),
        CreateClockifyTimeEntry("2024-01-02", "TC", "P2", "C1", "D2C", 0.5),
        CreateClockifyTimeEntry("2024-01-03", null, "P3", "C1", "D3", 0.5),
    ];

    private static ClockifyTimeEntry CreateClockifyTimeEntry(
        string date,
        string? task,
        string project,
        string client,
        string description,
        double hours
    ) =>
        new()
        {
            Date = date,
            Task = task,
            Project = project,
            Client = client,
            Description = description,
            Time = TimeSpan.FromHours(hours)
        };

    [Test]
    public void AggregatesTimeEntriesByProject()
    {
        var aggregator = new TimeEntryAggregator();

        var groupedTimeEntries = aggregator.Aggregate(timeEntries, TimeEntryGrouping.ByProject);

        var expectedGroupedTimeEntries = new List<GroupedTimeEntry>
        {
            new(
                "2024-01-01",
                "P1",
                2,
                $"D1A1{Environment.NewLine}D1A2{Environment.NewLine}D1B1{Environment.NewLine}D1B2"
            ),
            new(
                "2024-01-01",
                "P2",
                2,
                $"D2A1{Environment.NewLine}D2A2{Environment.NewLine}D2C1{Environment.NewLine}D2C2"
            ),
            new("2024-01-02", "P1", 1, $"D1A{Environment.NewLine}D1B"),
            new("2024-01-02", "P2", 1, $"D2A{Environment.NewLine}D2C"),
            new("2024-01-03", "P3", 0.5, $"D3"),
        };
        groupedTimeEntries.Should().BeEquivalentTo(expectedGroupedTimeEntries);
    }

    [Test]
    public void AggregatesTimeEntriesByTask()
    {
        var aggregator = new TimeEntryAggregator();

        var groupedTimeEntries = aggregator.Aggregate(timeEntries, TimeEntryGrouping.ByTask);

        var expectedGroupedTimeEntries = new List<GroupedTimeEntry>
        {
            new(
                "2024-01-01",
                "TA",
                2,
                $"D1A1{Environment.NewLine}D1A2{Environment.NewLine}D2A1{Environment.NewLine}D2A2"
            ),
            new("2024-01-01", "TB", 1, $"D1B1{Environment.NewLine}D1B2"),
            new("2024-01-01", "TC", 1, $"D2C1{Environment.NewLine}D2C2"),
            new("2024-01-02", "TA", 1, $"D1A{Environment.NewLine}D2A"),
            new("2024-01-02", "TB", 0.5, $"D1B"),
            new("2024-01-02", "TC", 0.5, $"D2C"),
            new("2024-01-03", string.Empty, 0.5, $"D3"),
        };
        groupedTimeEntries.Should().BeEquivalentTo(expectedGroupedTimeEntries);
    }

    [Test]
    public void ThrowsForUnknownGrouping()
    {
        var aggregator = new TimeEntryAggregator();

        Action action = () => aggregator.Aggregate(timeEntries, (TimeEntryGrouping)42);

        action
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Unknown grouping: 42 (Parameter 'grouping')")
            .WithParameterName("grouping");
    }
}
