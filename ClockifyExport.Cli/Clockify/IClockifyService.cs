namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Provides access to the Clockify API.
/// </summary>
public interface IClockifyService
{
    /// <summary>
    /// Gets a shared report as CSV.
    /// </summary>
    /// <param name="reportId">Unique report ID.</param>
    /// <param name="startDate">Inclusive starting date of the report.</param>
    /// <param name="endDate">Inclusive starting date of the report.</param>
    /// <param name="apiKey">Workspace API key for the Clockify API.</param>
    /// <returns>Shared report as CSV string.</returns>
    Task<string> GetSharedReportAsCsv(
        string reportId,
        DateOnly startDate,
        DateOnly endDate,
        string apiKey
    );
}
