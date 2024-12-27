using Microsoft.Extensions.DependencyInjection;

namespace ClockifyExport.Cli.Export;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to add exporters.
/// </summary>
internal static class ExportServiceCollectionExtensions
{
    /// <summary>
    /// Adds all classes required by exporters to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the exporters to.</param>
    /// <returns>The service collection with the exporters added.</returns>
    public static IServiceCollection AddExporters(this IServiceCollection services)
    {
        services.AddKeyedTransient<IExporter, CsvExporter>(ExportFormat.Csv);
        services.AddKeyedTransient<IExporter, JsonExporter>(ExportFormat.Json);
        services.AddTransient<ExporterProvider>();
        return services;
    }
}
