using Flurl;

namespace ClockifyExport.Cli.Clockify;

public class ClockifyUrlBuilder
{
    private static readonly string reportsBaseUrl = "https://reports.api.clockify.me";
    private static readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    private static readonly TimeOnly dayStart = new(0, 0, 0);
    private static readonly TimeOnly dayEnd = new(23, 59, 59, 999);

    /// <summary>
    /// Builds URL for a Clockify shared report in CSV format.
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns>URL for a Clockify shared report in CSV format.</returns>
    public static string BuildCsvSharedReportUrl(
        string reportId,
        DateOnly startDate,
        DateOnly endDate
    )
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
