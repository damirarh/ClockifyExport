using System.Globalization;
using System.Text.Json;
using ClockifyExport.Cli.Export;
using ClockifyExport.Cli.Processing;
using FluentAssertions;

namespace ClockifyExport.Tests.Export;

public class ExporterTests
{
    private static readonly GroupedTimeEntry[] timeEntries =
    [
        new("2024-01-02", "Group 1", 1, $"Description A{Environment.NewLine}Description B"),
        new("2024-01-03", "Group 2", 0.5, $"Description C"),
    ];

    [Test]
    public void ExportsTimeEntriesToCsv()
    {
        var exporter = new CsvExporter();

        var csv = exporter.Export(timeEntries);

        var expectedCsv = $"""
            Date,Group,Hours,Description
            {timeEntries[0].Date},{timeEntries[0].Group},{timeEntries[0].Hours.ToString(CultureInfo.InvariantCulture)},"{timeEntries[0].Description}"
            {timeEntries[1].Date},{timeEntries[1].Group},{timeEntries[1].Hours.ToString(CultureInfo.InvariantCulture)},{timeEntries[1].Description}

            """;
        csv.Should().Be(expectedCsv.ToString());
    }

    [Test]
    public void ExportsTimeEntriesToJson()
    {
        var exporter = new JsonExporter();

        var json = exporter.Export(timeEntries);

        var expectedJson = $$"""
            [
              {
                "date": "{{timeEntries[0].Date}}",
                "group": "{{timeEntries[0].Group}}",
                "hours": {{timeEntries[0].Hours.ToString(CultureInfo.InvariantCulture)}},
                "description": "{{JsonEncodedText.Encode(timeEntries[0].Description)}}"
              },
              {
                "date": "{{timeEntries[1].Date}}",
                "group": "{{timeEntries[1].Group}}",
                "hours": {{timeEntries[1].Hours.ToString(CultureInfo.InvariantCulture)}},
                "description": "{{timeEntries[1].Description}}"
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
