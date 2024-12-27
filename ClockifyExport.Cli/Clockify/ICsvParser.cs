namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Parses CSV data from Clockify.
/// </summary>
internal interface ICsvParser
{
    /// <summary>
    /// Parses CSV data from a shared report.
    /// </summary>
    /// <param name="csv">Shared report in CSV format</param>
    /// <returns>Time entries from the shared report.</returns>
    IReadOnlyList<ClockifyTimeEntry> ParseSharedReportCsv(string csv);
}
