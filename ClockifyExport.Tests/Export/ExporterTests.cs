using System.Globalization;
using System.Text.Json;
using ClockifyExport.Cli.Export;
using ClockifyExport.Cli.Processing;
using FluentAssertions;

namespace ClockifyExport.Tests.Export;

internal sealed class ExporterTests
{
    private static readonly GroupedTimeEntry[] TimeEntries =
    [
        new("2024-01-02", "Group 1", 1, $"Description A{Environment.NewLine}Description B"),
        new("2024-01-03", "Group 2", 0.5, $"Description C"),
    ];

    [Test]
    public void ExportsTimeEntriesToCsv()
    {
        var exporter = new CsvExporter();

        var csv = exporter.Export(TimeEntries);

        var expectedCsv = $"""
            Date,Group,Hours,Description
            {TimeEntries[0].Date},{TimeEntries[0].Group},{TimeEntries[0]
                .Hours.ToString(CultureInfo.InvariantCulture)},"{TimeEntries[0].Description}"
            {TimeEntries[1].Date},{TimeEntries[1].Group},{TimeEntries[1]
                .Hours.ToString(CultureInfo.InvariantCulture)},{TimeEntries[1].Description}
            
            """;
        csv.Should().Be(expectedCsv.ToString());
    }

    [Test]
    public void ExportsTimeEntriesToJson()
    {
        var exporter = new JsonExporter();

        var json = exporter.Export(TimeEntries);

        var expectedJson = $$"""
            [
              {
                "date": "{{TimeEntries[0].Date}}",
                "group": "{{TimeEntries[0].Group}}",
                "hours": {{TimeEntries[0].Hours.ToString(CultureInfo.InvariantCulture)}},
                "description": "{{JsonEncodedText.Encode(TimeEntries[0].Description)}}"
              },
              {
                "date": "{{TimeEntries[1].Date}}",
                "group": "{{TimeEntries[1].Group}}",
                "hours": {{TimeEntries[1].Hours.ToString(CultureInfo.InvariantCulture)}},
                "description": "{{TimeEntries[1].Description}}"
              }
            ]
            """;
        json.Should().Be(expectedJson.ToString());
    }

    [Test]
    [TestCase(ExportFormat.Csv)]
    [TestCase(ExportFormat.Json)]
    public void ProvidesRequestedExporter(ExportFormat format)
    {
        var exporters = new Dictionary<ExportFormat, IExporter>
        {
            { ExportFormat.Csv, new CsvExporter() },
            { ExportFormat.Json, new JsonExporter() },
        };
        var provider = new ExporterProvider(
            exporters[ExportFormat.Csv],
            exporters[ExportFormat.Json]
        );

        var exporter = provider.GetExporter(format);

        exporter.Should().Be(exporters[format]);
    }

    [Test]
    public void ThrowsForUnsupportedFormat()
    {
        var provider = new ExporterProvider(new CsvExporter(), new JsonExporter());

        Action action = () => provider.GetExporter((ExportFormat)42);

        var exception = action.Should().Throw<ArgumentOutOfRangeException>();
        exception.Which.ParamName.Should().Be("format");
        exception.Which.ActualValue.Should().Be((ExportFormat)42);
    }
}
