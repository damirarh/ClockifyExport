using System.Text.Json;
using ClockifyExport.Cli.Processing;

namespace ClockifyExport.Cli.Export;

/// <summary>
/// Exports data to a JSON string.
/// </summary>
public class JsonExporter : IExporter
{
    private static readonly JsonSerializerOptions options =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    /// <inheritdoc />
    public string Export(IEnumerable<GroupedTimeEntry> timeEntries)
    {
        return JsonSerializer.Serialize(timeEntries, options);
    }
}
