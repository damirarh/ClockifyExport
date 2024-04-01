using ClockifyExport.Cli.Clockify;
using Microsoft.Extensions.Logging;

namespace ClockifyExport.Cli.Logging;

/// <summary>
/// Strongly typed logger extensions.
/// </summary>
public static class LoggerExtensions
{
    private static readonly Action<
        ILogger,
        string,
        ClockifyTimeEntry,
        Exception?
    > timeEntryValidationError = LoggerMessage.Define<string, ClockifyTimeEntry>(
        LogLevel.Warning,
        new EventId(1, nameof(TimeEntryValidationError)),
        "Validation failed: {ValidationMessage}" + Environment.NewLine + "{TimeEntry}"
    );

    /// <summary>
    /// Logs a time entry validation error.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="validationMessage">Validation error message to log.</param>
    /// <param name="timeEntry">Time entry that failed validation.</param>
    public static void TimeEntryValidationError(
        this ILogger logger,
        string validationMessage,
        ClockifyTimeEntry timeEntry
    ) => timeEntryValidationError(logger, validationMessage, timeEntry, default);
}
