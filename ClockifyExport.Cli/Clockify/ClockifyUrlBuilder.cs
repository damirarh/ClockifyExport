using Flurl;

namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Builds URLs for the Clockify API.
/// </summary>
public class ClockifyUrlBuilder : IClockifyUrlBuilder
{
    private static readonly string reportsBaseUrl = "https://reports.api.clockify.me";
    private static readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    private static readonly TimeOnly dayStart = new(0, 0, 0);
    private static readonly TimeOnly dayEnd = new(23, 59, 59, 999);

    /// <inheritdoc/>
    public string BuildCsvSharedReportUrl(string reportId, DateOnly startDate, DateOnly endDate)
    {
        return reportsBaseUrl
            .AppendPathSegments("v1", "shared-reports", reportId)
            .SetQueryParam("exportType", "CSV")
            .SetQueryParam(
                "dateRangeStart",
                startDate.ToDateTime(dayStart, DateTimeKind.Utc).ToString(dateTimeFormat)
            )
            .SetQueryParam(
                "dateRangeEnd",
                endDate.ToDateTime(dayEnd, DateTimeKind.Utc).ToString(dateTimeFormat)
            )
            .ToString();
    }
}
