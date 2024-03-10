using ClockifyExport.Cli.Processing;
using ClockifyExport.Cli.Processing.PostProcessors;
using FluentAssertions;

namespace ClockifyExport.Tests.Processing.PostProcessors;

public class RoundingPostProcessorTests
{
    [Test]
    [TestCase(0.01, 15)]
    [TestCase(14.99, 15)]
    [TestCase(15, 15)]
    [TestCase(16, 30)]
    [TestCase(30, 30)]
    [TestCase(60, 60)]
    [TestCase(61, 75)]
    public void RoundsUpTo15Minutes(double actualMinutes, double expectedMinutes)
    {
        var processor = new RoundingPostProcessor(15);
        var entry = new GroupedTimeEntry("2024-03-10", "Group", actualMinutes / 60, "Description");

        var result = processor.Process(entry);

        result.Hours.Should().Be(expectedMinutes / 60);
    }

    [Test]
    public void DoesNotModifyAnythingButHours()
    {
        var processor = new RoundingPostProcessor(15);
        var entry = new GroupedTimeEntry("2024-03-10", "Group", 1.01, "Description");

        var result = processor.Process(entry);

        result.Date.Should().Be(entry.Date);
        result.Group.Should().Be(entry.Group);
        result.Description.Should().Be(entry.Description);
    }
}
