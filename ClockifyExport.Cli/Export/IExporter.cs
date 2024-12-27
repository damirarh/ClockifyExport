using ClockifyExport.Cli.Processing;

namespace ClockifyExport.Cli.Export;

/// <summary>
/// Exports data to a string.
/// </summary>
internal interface IExporter
{
    /// <summary>
    /// Exports the given time entries to a string.
    /// </summary>
    /// <param name="timeEntries">Time entries export.</param>
    /// <returns>String with exported time entries.</returns>
    string Export(IEnumerable<GroupedTimeEntry> timeEntries);
}
