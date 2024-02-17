using System.Globalization;
using ClockifyExport.Cli.Processing;
using CsvHelper;

namespace ClockifyExport.Cli.Export;

/// <summary>
/// Exports data to a CSV string.
/// </summary>
public class CsvExporter : IExporter
{
    /// <inheritdoc />
    public string Export(IEnumerable<GroupedTimeEntry> timeEntries)
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(timeEntries);
        return writer.ToString();
    }
}
