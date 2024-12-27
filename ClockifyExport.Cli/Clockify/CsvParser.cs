using System.Globalization;
using CsvHelper;

namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Parses CSV data from Clockify.
/// </summary>
internal sealed class CsvParser : ICsvParser
{
    ///<inheritdoc />
    public IReadOnlyList<ClockifyTimeEntry> ParseSharedReportCsv(string csv)
    {
        using var csvReader = new CsvReader(new StringReader(csv), CultureInfo.InvariantCulture);
        return csvReader.GetRecords<ClockifyTimeEntry>().ToList();
    }
}
