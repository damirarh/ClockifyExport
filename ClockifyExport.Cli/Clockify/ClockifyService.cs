using System.Globalization;
using CsvHelper;

namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Provides access to the Clockify API.
/// </summary>
/// <param name="httpClient">Injected typed <see cref="HttpClient"/> instance.</param>
/// <param name="clockifyUrlBuilder">Injected <see cref="IClockifyUrlBuilder"/> instance.</param>
public class ClockifyService(HttpClient httpClient, IClockifyUrlBuilder clockifyUrlBuilder)
    : IClockifyService
{
    ///<inheritdoc />
    public async Task<List<ClockifyTimeEntry>> GetSharedReport(
        string reportId,
        DateOnly startDate,
        DateOnly endDate,
        string apiKey
    )
    {
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        var url = clockifyUrlBuilder.BuildCsvSharedReportUrl(reportId, startDate, endDate);
        var csv = await httpClient.GetStringAsync(url);
        using var csvReader = new CsvReader(new StringReader(csv), CultureInfo.InvariantCulture);
        return csvReader.GetRecords<ClockifyTimeEntry>().ToList();
    }
}
