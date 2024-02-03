namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Provides access to the Clockify API.
/// </summary>
public interface IClockifyService
{
    /// <summary>
    /// Gets and parses a shared CSV report.
    /// </summary>
    /// <param name="reportId">Unique report ID.</param>
    /// <param name="startDate">Inclusive starting date of the report.</param>
    /// <param name="endDate">Inclusive starting date of the report.</param>
    /// <param name="apiKey">Workspace API key for the Clockify API.</param>
    /// <returns>Time entries from the shared report.</returns>
    Task<List<ClockifyTimeEntry>> GetSharedReport(
        string reportId,
        DateOnly startDate,
        DateOnly endDate,
        string apiKey
    );
}
