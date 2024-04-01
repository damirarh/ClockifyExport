using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Processing.PreProcessors;
using FluentAssertions;

namespace ClockifyExport.Tests.Processing.PreProcessors;

public class ParseTaskPreProcessorTests
{
    [Test]
    [TestCase("JIRA-1234: Do something", "JIRA-1234")]
    [TestCase("JIRA-1234 Do something", "JIRA-1234")]
    [TestCase("JIRA-1234", "JIRA-1234")]
    [TestCase("Do something", "Do something")]
    [TestCase("", "")]
    [TestCase(null, null)]
    public void ParsesJiraTaskIdOrLeaveTaskUnchanged(string? task, string? expectedTaskId)
    {
        var preProcessor = new ParseTaskPreProcessor(@"[A-Z\d]+\-\d+");
        var entry = new ClockifyTimeEntry
        {
            Date = "2024-04-01",
            Task = task,
            Project = "Project",
            Client = "Client",
            Description = "Description",
            Time = TimeSpan.FromHours(1)
        };

        var result = preProcessor.Process(entry);

        result.Task.Should().Be(expectedTaskId);
    }

    [Test]
    public void DoesNotModifyAnythingButTask()
    {
        var preProcessor = new ParseTaskPreProcessor(@"[A-Z\d]+\-\d+");
        var entry = new ClockifyTimeEntry
        {
            Date = "2024-04-01",
            Task = "JIRA-1234: Do something",
            Project = "Project",
            Client = "Client",
            Description = "Description",
            Time = TimeSpan.FromHours(1)
        };

        var result = preProcessor.Process(entry);

        result.Date.Should().Be(entry.Date);
        result.Task.Should().NotBe(entry.Task);
        result.Project.Should().Be(entry.Project);
        result.Client.Should().Be(entry.Client);
        result.Description.Should().Be(entry.Description);
        result.Time.Should().Be(entry.Time);
    }
}
