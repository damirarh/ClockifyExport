using ClockifyExport.Cli;
using ClockifyExport.Cli.Clockify;
using ClockifyExport.Cli.Export;
using ClockifyExport.Cli.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder()
    .ConfigureServices(ConfigureServices)
    .RunCommandLineApplicationAsync<AppCommand>(args);

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddTransient<IClockifyService, ClockifyService>();
    services.AddTransient<IClockifyUrlBuilder, ClockifyUrlBuilder>();
    services.AddTransient<ITimeEntryAggregator, TimeEntryAggregator>();
    services.AddExporters();

    services.AddHttpClient<ClockifyService>();
}
