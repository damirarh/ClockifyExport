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
        using var csvReader = CsvReader.Create(new StringReader(csv));
        return csvReader.GetRecords<ClockifyTimeEntry>().ToList();
    }
}
