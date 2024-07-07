using System.Globalization;
using ClockifyExport.Cli.Processing;
using CsvHelper;
using CsvHelper.Configuration;

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
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine,
        };
        using var csv = new CsvWriter(writer, csvConfig);
        csv.WriteRecords(timeEntries);
        return writer.ToString();
    }
}
