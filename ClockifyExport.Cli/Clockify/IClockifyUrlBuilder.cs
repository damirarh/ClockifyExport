namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Builds URLs for the Clockify API.
/// </summary>
public interface IClockifyUrlBuilder
{
    /// <summary>
    /// Builds URL for a Clockify shared report in CSV format.
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns>URL for a Clockify shared report in CSV format.</returns>
    string BuildCsvSharedReportUrl(string reportId, DateOnly startDate, DateOnly endDate);
}
