namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Provides access to the Clockify API.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Gets a shared report in CSV format.
    /// </summary>
    /// <param name="reportsBaseUrl">Base URL for the Clockify reports API.</param>
    /// <param name="reportId">Unique report ID.</param>
    /// <param name="startDate">Inclusive starting date of the report.</param>
    /// <param name="endDate">Inclusive starting date of the report.</param>
    /// <param name="apiKey">Workspace API key for the Clockify API.</param>
    /// <returns>Shared report in CSV format.</returns>
    Task<string> GetSharedReportCsvAsync(
        Uri reportsBaseUrl,
        string reportId,
        DateOnly startDate,
        DateOnly endDate,
        string apiKey
    );
}
