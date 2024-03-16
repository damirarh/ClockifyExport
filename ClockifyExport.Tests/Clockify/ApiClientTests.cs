using System.Net;
using ClockifyExport.Cli.Clockify;
using FluentAssertions;
using Flurl;
using Moq;
using Moq.Contrib.HttpClient;

namespace ClockifyExport.Tests.Clockify;

public class ApiClientTests
{
    [Test]
    public async Task BuildsCorrectSharedReportUrl()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = handlerMock.CreateClient();

        handlerMock
            .SetupRequest(
                HttpMethod.Get,
                request =>
                {
                    Url url = request.RequestUri;
                    return url.Scheme == "https"
                        && url.Host == "reports.api.clockify.me"
                        && url.Path == "/v1/shared-reports/61a710a20a923f0f3b446bdc"
                        && url.QueryParams.Count == 3
                        && url.QueryParams.FirstOrDefault("exportType").ToString() == "CSV"
                        && url.QueryParams.FirstOrDefault("dateRangeStart").ToString()
                            == "2023-11-01T00:00:00.000Z"
                        && url.QueryParams.FirstOrDefault("dateRangeEnd").ToString()
                            == "2023-11-30T23:59:59.999Z";
                }
            )
            .ReturnsResponse(HttpStatusCode.OK);

        var apiClient = new ApiClient(httpClient);
        await apiClient.GetSharedReportCsvAsync(
            "61a710a20a923f0f3b446bdc",
            new DateOnly(2023, 11, 1),
            new DateOnly(2023, 11, 30),
            "my-api-key"
        );

        handlerMock.VerifyAll();
    }

    [Test]
    public async Task AddsApiKeyToHeader()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = handlerMock.CreateClient();

        handlerMock
            .SetupRequest(
                HttpMethod.Get,
                request => request.Headers.GetValues("x-api-key").Single() == "my-api-key"
            )
            .ReturnsResponse(HttpStatusCode.OK);

        var apiClient = new ApiClient(httpClient);
        await apiClient.GetSharedReportCsvAsync(
            "61a710a20a923f0f3b446bdc",
            new DateOnly(2023, 11, 1),
            new DateOnly(2023, 11, 30),
            "my-api-key"
        );

        handlerMock.VerifyAll();
    }

    [Test]
    public async Task ReturnsHttpResponseAsString()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = handlerMock.CreateClient();

        var csv = "csv";
        handlerMock.SetupAnyRequest().ReturnsResponse(csv, "text/csv");

        var apiClient = new ApiClient(httpClient);
        var result = await apiClient.GetSharedReportCsvAsync(
            "61a710a20a923f0f3b446bdc",
            new DateOnly(2023, 11, 1),
            new DateOnly(2023, 11, 30),
            "my-api-key"
        );

        result.Should().Be(csv);
        handlerMock.VerifyAll();
    }
}
