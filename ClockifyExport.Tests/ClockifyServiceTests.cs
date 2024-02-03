using System.Globalization;
using ClockifyExport.Cli.Clockify;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Moq.Contrib.HttpClient;

namespace ClockifyExport.Tests;

public class ClockifyServiceTests
{
    private void SetupMocks(
        AutoMocker mocker,
        string reportId,
        DateOnly startDate,
        DateOnly endDate,
        string apiKey,
        string csvResponse
    )
    {
        var url = $"http://{Guid.NewGuid()}";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .SetupRequest(
                HttpMethod.Get,
                url,
                request => request.Headers.GetValues("x-api-key").Single() == apiKey
            )
            .ReturnsResponse(csvResponse, "text/csv");
        mocker.Use(handlerMock.CreateClient());

        var urlBuilderMock = new Mock<IClockifyUrlBuilder>();
        urlBuilderMock
            .Setup(x => x.BuildCsvSharedReportUrl(reportId, startDate, endDate))
            .Returns(url);
        mocker.Use(urlBuilderMock);
    }

    [Test]
    public async Task ParsesCsvWithTask()
    {
        var reportId = Guid.NewGuid().ToString();
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        var startDate = endDate.AddDays(-7);
        var apiKey = Guid.NewGuid().ToString();

        var date = startDate.ToString();
        var task = "My task";
        var project = "My project";
        var client = "My client";
        var description = "My description";
        var time = TimeSpan.FromMinutes(15);

        var csvResponse = $"""
        "Date","Task","Project","Client","Description","Time (h)","Time (decimal)"
        "{date}","{task}","{project}","{client}","{description}","{time}","{time.TotalHours.ToString(CultureInfo.InvariantCulture)}"
        """;

        var mocker = new AutoMocker();
        SetupMocks(mocker, reportId, startDate, endDate, apiKey, csvResponse);
        var clockifyService = mocker.CreateInstance<ClockifyService>();

        var timeEntries = await clockifyService.GetSharedReport(
            reportId,
            startDate,
            endDate,
            apiKey
        );

        timeEntries.Should().HaveCount(1);
        timeEntries[0].Date.Should().Be(date);
        timeEntries[0].Task.Should().Be(task);
        timeEntries[0].Project.Should().Be(project);
        timeEntries[0].Client.Should().Be(client);
        timeEntries[0].Description.Should().Be(description);
        timeEntries[0].Time.Should().Be(time);
    }

    [Test]
    public async Task ParsesCsvWithoutTask()
    {
        var reportId = Guid.NewGuid().ToString();
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        var startDate = endDate.AddDays(-7);
        var apiKey = Guid.NewGuid().ToString();

        var date = startDate.ToString();
        var project = "My project";
        var client = "My client";
        var description = "My description";
        var time = TimeSpan.FromMinutes(15);

        var csvResponse = $"""
        "Date","Project","Client","Description","Time (h)","Time (decimal)"
        "{date}","{project}","{client}","{description}","{time}","{time.TotalHours.ToString(CultureInfo.InvariantCulture)}"
        """;

        var mocker = new AutoMocker();
        SetupMocks(mocker, reportId, startDate, endDate, apiKey, csvResponse);
        var clockifyService = mocker.CreateInstance<ClockifyService>();

        var timeEntries = await clockifyService.GetSharedReport(
            reportId,
            startDate,
            endDate,
            apiKey
        );

        timeEntries.Should().HaveCount(1);
        timeEntries[0].Date.Should().Be(date);
        timeEntries[0].Project.Should().Be(project);
        timeEntries[0].Client.Should().Be(client);
        timeEntries[0].Description.Should().Be(description);
        timeEntries[0].Time.Should().Be(time);
    }
}
