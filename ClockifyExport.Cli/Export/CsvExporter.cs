using ClockifyExport.Cli.Processing;
using CsvHelper;

namespace ClockifyExport.Cli.Export;

/// <summary>
/// Exports data to a CSV string.
/// </summary>
internal sealed class CsvExporter : IExporter
{
    /// <inheritdoc />
    public string Export(IEnumerable<GroupedTimeEntry> timeEntries)
    {
        using var writer = new StringWriter();
        var options = new CsvWriterOptions
        {
            NewLine = Environment.NewLine,
        };
        using (var csv = CsvWriter.Create(writer, options))
        {
            csv.WriteRecords(timeEntries);
        }
        return writer.ToString();
    }
}
