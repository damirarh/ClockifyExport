using System.Reflection;
using ClockifyExport.Cli;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ClockifyExport.Tests.E2E;

internal sealed class IntegrationTests
{
    private WireMockServer server;

    [SetUp]
    public void Setup()
    {
        server = WireMockServer.Start();
    }

    [Test]
    public void AppRunsWhenApiCallSucceeds()
    {
        var reportId = "report-id";
        server
            .Given(Request.Create().WithPath($"/v1/shared-reports/{reportId}").UsingGet())
            .RespondWith(
                Response.Create().WithStatusCode(200).WithBodyFromFile("Inputs/Clockify.csv")
            );

        var result = InvokeApp(reportId, server.Urls[0]);
        result.Should().Be(0);
    }

    [Test]
    public void AppFailsWhenApiCallFails()
    {
        var reportId = "report-id";
        server
            .Given(Request.Create().WithPath($"/v1/shared-reports/{reportId}").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(401));

        var action = () => InvokeApp(reportId, server.Urls[0]);
        action
            .Should()
            .Throw<TargetInvocationException>()
            .Which.InnerException.Should()
            .BeOfType<HttpRequestException>();
    }

    private static object? InvokeApp(string reportId, string baseUrl)
    {
        string[] args =
        [
            "--api-key",
            "api-key",
            "--report-id",
            reportId,
            "--start-date",
            "2024-01-01",
            "--end-date",
            "2024-01-31",
            "--grouping",
            "ByTask",
            "--round-up-to",
            "15",
            "--task-id-regex",
            "[A-Z\\d]+\\-\\d+",
            "--format",
            "Csv",
            "--output",
            "out.csv",
            "--base-url",
            baseUrl,
        ];
        var entryPoint = typeof(AppCommand).Assembly.EntryPoint!;
        return entryPoint.Invoke(null, [args]);
    }

    [TearDown]
    public void TearDown()
    {
        server.Stop();
        server.Dispose();
    }
}
