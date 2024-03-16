﻿using System.Globalization;
using Flurl;

namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Provides access to the Clockify API.
/// </summary>
public class ApiClient(HttpClient httpClient) : IApiClient
{
    private const string reportsBaseUrl = "https://reports.api.clockify.me";
    private const string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

    private static readonly TimeOnly dayStart = new(0, 0, 0);
    private static readonly TimeOnly dayEnd = new(23, 59, 59, 999);

    ///<inheritdoc />
    public async Task<string> GetSharedReportCsvAsync(
        string reportId,
        DateOnly startDate,
        DateOnly endDate,
        string apiKey
    )
    {
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        var url = BuildCsvSharedReportUrl(reportId, startDate, endDate);
        return await httpClient.GetStringAsync(url).ConfigureAwait(false);
    }

    private static Uri BuildCsvSharedReportUrl(
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
                startDate
                    .ToDateTime(dayStart, DateTimeKind.Utc)
                    .ToString(dateTimeFormat, CultureInfo.InvariantCulture)
            )
            .SetQueryParam(
                "dateRangeEnd",
                endDate
                    .ToDateTime(dayEnd, DateTimeKind.Utc)
                    .ToString(dateTimeFormat, CultureInfo.InvariantCulture)
            )
            .ToUri();
    }
}
