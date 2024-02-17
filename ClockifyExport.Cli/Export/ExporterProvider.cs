using Microsoft.Extensions.DependencyInjection;

namespace ClockifyExport.Cli.Export;

/// <summary>
/// Provides an exporter based on the requested format.
/// </summary>
/// <param name="csvExporter">Injected CSV <see cref="IExporter"/> instance.</param>
/// <param name="jsonExporter">Injected JSON <see cref="IExporter"/> instance.</param>
public class ExporterProvider(
    [FromKeyedServices(ExportFormat.Csv)] IExporter csvExporter,
    [FromKeyedServices(ExportFormat.Json)] IExporter jsonExporter
)
{
    /// <summary>
    /// Gets an exporter based on the requested format.
    /// </summary>
    /// <param name="format">Export format.</param>
    /// <returns>Exporter for the requested format.</returns>
    public IExporter GetExporter(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Csv => csvExporter,
            ExportFormat.Json => jsonExporter,
            _
                => throw new ArgumentOutOfRangeException(
                    nameof(format),
                    format,
                    $"Unsupported format."
                )
        };
    }
}
