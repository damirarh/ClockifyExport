# ClockifyExport

The tool exports data from a Clockify [shared report](https://clockify.me/help/reports/sharing-reports), groups the entries for the same day, and saves it as JSON or CSV with the following fields:

- `Date`: Date of the grouped time entry. Non-parsed string as it appears in the Clockify shared report CSV.
- `Group`: Project or task of the grouped time entry.
- `Hours`: Sum of time for the grouped time entry, expressed as total hours (with decimal part).
- `Description`: All descriptions for the grouped time entry, each in its own line.

You need the following information from Clockify to invoke the tool:

- report ID: it's the final part of the URL when viewing the shared report in the browser, e.g. https://app.clockify.me/shared/YourReportId
- API key: you can generate it on the Advanced tab of your [Clockify Preferences page](https://app.clockify.me/user/preferences#advanced) (don't share it with anyone as it gives access to all your data in Clockify)

## Installation

You can install the latest version as a global .NET tool from the NuGet Gallery with the following command:

```shell
dotnet tool install -g DamirsCorner.ClockifyExport
```

Alternatively, you can download the NuGet package from the [GitHub Releases](https://github.com/damirarh/ClockifyExport/releases) page and install that package with the following command:

```shell
dotnet tool install -g DamirsCorner.ClockifyExport --add-source "."
```

The command assumes the downloaded package is located in the current directory. If not, change the `--add-source` value to point at the directory containing the package.

## Usage

```text
Usage: clockifyexport [options]

Options:
  -a|--api-key <API_KEY>              Clockify API key.
  -i|--report-id <REPORT_ID>          Clockify shared report ID.
  -s|--start-date <START_DATE>        Report start date (inclusive).
  -e|--end-date <END_DATE>            Report end date (inclusive).
  -g|--grouping <GROUPING>            Column to group by time entries within a day.
                                      Allowed values are: ByTask, ByProject.
  -r|--round-up-to <ROUND_UP_TO>      Number of minutes to round duration up to after grouping.
  -t|--task-id-regex <TASK_ID_REGEX>  Regex to use for task id parsing.
  -f|--format <FORMAT>                Export format.
                                      Allowed values are: Csv, Json.
  -o|--output <OUTPUT>                Output file.
  -b|--base-url <BASE_URL>            Base URL of the Clockify Reports API.
                                      Default value is: https://reports.api.clockify.me/.
  -?|-h|--help                        Show help information.
```

Sample call:

```shell
clockifyexport --api-key YourApiKey --report-id YourReportId --start-date 2024-01-01 --end-date 2024-01-31 --grouping ByTask --round-up-to 15 --format Csv --output out.csv --task-id-regex [A-Z\\d]+\\-\\d+
```

## Troubleshooting

In the project repository (in the `.bruno` folder), you can find a [Bruno](https://www.usebruno.com/) collection with a few API calls. Use them to test the API key and report ID values you are providing to the tool.

- Get user: Returns your user data. Use it to confirm that your API key (`apiKey` secret in Bruno environment) is working.
- Get workspaces: Returns a list of your workspaces.
- Get shared reports: Returns a list of shared reports in a workspace (`workspaceId` secret in Bruno environment: set its value to `id` from the _Get workspaces_ call).
- Generate shared report: Returns a generated shared report in CSV file (`reportId` secret in Bruno environment: set its value to `id` from the _Get shared reports_ call). Set the date range to your liking. The same call is used by the tool.
