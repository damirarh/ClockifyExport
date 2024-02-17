using Microsoft.Extensions.DependencyInjection;

namespace ClockifyExport.Cli.Export;

public static class ExportServiceCollectionExtensions
{
    public static IServiceCollection AddExporters(this IServiceCollection services)
    {
        services.AddKeyedTransient<IExporter, CsvExporter>(ExportFormat.Csv);
        services.AddKeyedTransient<IExporter, JsonExporter>(ExportFormat.Json);
        services.AddTransient<ExporterProvider>();
        return services;
    }
}
