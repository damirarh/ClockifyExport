using System.Globalization;
using ClockifyExport.Cli.Clockify;
using FluentAssertions;

namespace ClockifyExport.Tests.Clockify;

internal sealed class CsvParserTests
{
    [Test]
    public void ParsesCsvWithTask()
    {
        var date = DateOnly
            .FromDateTime(DateTime.Today)
            .ToString("o", CultureInfo.InvariantCulture);
        var task = "My task";
        var project = "My project";
        var client = "My client";
        var description = "My description";
        var time = TimeSpan.FromMinutes(15);

        var csv = $"""
            "Date","Task","Project","Client","Description","Time (h)","Time (decimal)"
            "{date}","{task}","{project}","{client}","{description}","{time}","{time.TotalHours.ToString(
                CultureInfo.InvariantCulture
            )}"
            """;

        var csvParser = new CsvParser();

        var timeEntries = csvParser.ParseSharedReportCsv(csv);

        timeEntries.Should().HaveCount(1);
        timeEntries[0].Date.Should().Be(date);
        timeEntries[0].Task.Should().Be(task);
        timeEntries[0].Project.Should().Be(project);
        timeEntries[0].Client.Should().Be(client);
        timeEntries[0].Description.Should().Be(description);
        timeEntries[0].Time.Should().Be(time);
    }

    [Test]
    public void ParsesCsvWithoutTask()
    {
        var date = DateOnly
            .FromDateTime(DateTime.Today)
            .ToString("o", CultureInfo.InvariantCulture);
        var project = "My project";
        var client = "My client";
        var description = "My description";
        var time = TimeSpan.FromMinutes(15);

        var csv = $"""
            "Date","Project","Client","Description","Time (h)","Time (decimal)"
            "{date}","{project}","{client}","{description}","{time}","{time.TotalHours.ToString(
                CultureInfo.InvariantCulture
            )}"
            """;

        var csvParser = new CsvParser();

        var timeEntries = csvParser.ParseSharedReportCsv(csv);

        timeEntries.Should().HaveCount(1);
        timeEntries[0].Date.Should().Be(date);
        timeEntries[0].Project.Should().Be(project);
        timeEntries[0].Client.Should().Be(client);
        timeEntries[0].Description.Should().Be(description);
        timeEntries[0].Time.Should().Be(time);
    }
}
