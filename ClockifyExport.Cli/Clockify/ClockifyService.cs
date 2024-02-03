namespace ClockifyExport.Cli.Clockify;

/// <inheritdoc />
/// <param name="httpClient">Injected typed <see cref="HttpClient"/> instance.</param>
public class ClockifyService(HttpClient httpClient) : IClockifyService
{
    ///<inheritdoc />
    public async Task<string> GetSharedReportAsCsv(
        string reportId,
        DateOnly startDate,
        DateOnly endDate,
        string apiKey
    )
    {
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        var url = ClockifyUrlBuilder.BuildCsvSharedReportUrl(reportId, startDate, endDate);
        return await httpClient.GetStringAsync(url);
    }
}
