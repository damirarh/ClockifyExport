using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Processing;
using ClockifyExport.Cli.Processing.PostProcessors;
using ClockifyExport.Cli.Processing.PreProcessors;
using FluentAssertions;
using Moq;

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

    [Test]
    public void ExecutesAllPreProcessorsOnAllEntries()
    {
        var aggregator = new TimeEntryAggregator();

        var preProcessor1Mock = new Mock<IPreProcessor>();
        preProcessor1Mock
            .Setup(p => p.Process(It.IsAny<ClockifyTimeEntry>()))
            .Returns<ClockifyTimeEntry>(entry => entry with { Project = $"{entry.Project}-P" });
        aggregator.AddPreProcessor(preProcessor1Mock.Object);

        var preProcessor2Mock = new Mock<IPreProcessor>();
        preProcessor2Mock
            .Setup(p => p.Process(It.IsAny<ClockifyTimeEntry>()))
            .Returns<ClockifyTimeEntry>(
                entry => entry with { Description = $"{entry.Description}-P" }
            );
        aggregator.AddPreProcessor(preProcessor2Mock.Object);

        var groupedTimeEntries = aggregator.Aggregate(timeEntries, TimeEntryGrouping.ByProject);

        var expectedGroupedTimeEntries = new List<GroupedTimeEntry>
        {
            new(
                "2024-01-01",
                "P1-P",
                2,
                $"D1A1-P{Environment.NewLine}D1A2-P{Environment.NewLine}D1B1-P{Environment.NewLine}D1B2-P"
            ),
            new(
                "2024-01-01",
                "P2-P",
                2,
                $"D2A1-P{Environment.NewLine}D2A2-P{Environment.NewLine}D2C1-P{Environment.NewLine}D2C2-P"
            ),
            new("2024-01-02", "P1-P", 1, $"D1A-P{Environment.NewLine}D1B-P"),
            new("2024-01-02", "P2-P", 1, $"D2A-P{Environment.NewLine}D2C-P"),
            new("2024-01-03", "P3-P", 0.5, $"D3-P"),
        };
        groupedTimeEntries.Should().BeEquivalentTo(expectedGroupedTimeEntries);
        preProcessor1Mock.Verify(p => p.Process(It.IsAny<ClockifyTimeEntry>()), Times.Exactly(13));
        preProcessor2Mock.Verify(p => p.Process(It.IsAny<ClockifyTimeEntry>()), Times.Exactly(13));
    }

    [Test]
    public void ExecutesAllPostProcessorsOnAllGroupedEntries()
    {
        var aggregator = new TimeEntryAggregator();

        var postProcessor1Mock = new Mock<IPostProcessor>();
        postProcessor1Mock
            .Setup(p => p.Process(It.IsAny<GroupedTimeEntry>()))
            .Returns<GroupedTimeEntry>(entry => entry with { Group = $"{entry.Group}-P" });
        aggregator.AddPostProcessor(postProcessor1Mock.Object);

        var postProcessor2Mock = new Mock<IPostProcessor>();
        postProcessor2Mock
            .Setup(p => p.Process(It.IsAny<GroupedTimeEntry>()))
            .Returns<GroupedTimeEntry>(
                entry => entry with { Description = $"{entry.Description}-P" }
            );
        aggregator.AddPostProcessor(postProcessor2Mock.Object);

        var groupedTimeEntries = aggregator.Aggregate(timeEntries, TimeEntryGrouping.ByProject);

        var expectedGroupedTimeEntries = new List<GroupedTimeEntry>
        {
            new(
                "2024-01-01",
                "P1-P",
                2,
                $"D1A1{Environment.NewLine}D1A2{Environment.NewLine}D1B1{Environment.NewLine}D1B2-P"
            ),
            new(
                "2024-01-01",
                "P2-P",
                2,
                $"D2A1{Environment.NewLine}D2A2{Environment.NewLine}D2C1{Environment.NewLine}D2C2-P"
            ),
            new("2024-01-02", "P1-P", 1, $"D1A{Environment.NewLine}D1B-P"),
            new("2024-01-02", "P2-P", 1, $"D2A{Environment.NewLine}D2C-P"),
            new("2024-01-03", "P3-P", 0.5, $"D3-P"),
        };
        groupedTimeEntries.Should().BeEquivalentTo(expectedGroupedTimeEntries);
        postProcessor1Mock.Verify(p => p.Process(It.IsAny<GroupedTimeEntry>()), Times.Exactly(5));
        postProcessor2Mock.Verify(p => p.Process(It.IsAny<GroupedTimeEntry>()), Times.Exactly(5));
    }
}
