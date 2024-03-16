using System.Globalization;
using CsvHelper;

namespace ClockifyExport.Cli.Clockify;

/// <summary>
/// Parses CSV data from Clockify.
/// </summary>
public class CsvParser : ICsvParser
{
    ///<inheritdoc />
    public List<ClockifyTimeEntry> ParseSharedReportCsv(string csv)
    {
        using var csvReader = new CsvReader(new StringReader(csv), CultureInfo.InvariantCulture);
        return csvReader.GetRecords<ClockifyTimeEntry>().ToList();
    }
}
