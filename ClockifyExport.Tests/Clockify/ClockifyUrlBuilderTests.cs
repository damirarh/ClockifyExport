using ClockifyExport.Cli.Clockify;
using FluentAssertions;
using Flurl;

namespace ClockifyExport.Tests.Clockify;

public class ClockifyUrlBuilderTests
{
    [Test]
    public void BuildsCsvSharedReportUrl()
    {
        var urlBuilder = new ClockifyUrlBuilder();
        var url = urlBuilder.BuildCsvSharedReportUrl(
            "61a710a20a923f0f3b446bdc",
            new DateOnly(2023, 11, 1),
            new DateOnly(2023, 11, 30)
        );

        var parsedUrl = new Url(url);

        parsedUrl.Scheme.Should().Be("https");
        parsedUrl.Host.Should().Be("reports.api.clockify.me");
        parsedUrl.Path.Should().Be("/v1/shared-reports/61a710a20a923f0f3b446bdc");
        parsedUrl.QueryParams.Should().HaveCount(3);
        parsedUrl.QueryParams.FirstOrDefault("exportType").Should().Be("CSV");
        parsedUrl
            .QueryParams.FirstOrDefault("dateRangeStart")
            .Should()
            .Be("2023-11-01T00:00:00.000Z");
        parsedUrl
            .QueryParams.FirstOrDefault("dateRangeEnd")
            .Should()
            .Be("2023-11-30T23:59:59.999Z");
    }
}
