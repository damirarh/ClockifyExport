using ClockifyExport.Cli;
using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Export;
using ClockifyExport.Cli.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder()
    .ConfigureServices(ConfigureServices)
    .RunCommandLineApplicationAsync<AppCommand>(args)
    .ConfigureAwait(false);

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddTransient<IApiClient, ApiClient>();
    services.AddTransient<ICsvParser, CsvParser>();
    services.AddTransient<ITimeEntryAggregator, TimeEntryAggregator>();
    services.AddExporters();

    services.AddHttpClient<ApiClient>();
}
