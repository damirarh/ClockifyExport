using ClockifyExport.Cli.Clockify;
using Microsoft.Extensions.Logging;

namespace ClockifyExport.Cli.Logging;

/// <summary>
/// Strongly typed logger extensions.
/// </summary>
public static partial class LoggerExtensions
{
    /// <summary>
    /// Logs a time entry validation error.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="validationMessage">Validation error message to log.</param>
    /// <param name="timeEntry">Time entry that failed validation.</param>
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Warning,
        Message = """
        Validation failed: {ValidationMessage}
        {TimeEntry}
        """
    )]
    public static partial void TimeEntryValidationError(
        this ILogger logger,
        string validationMessage,
        ClockifyTimeEntry timeEntry
    );
}
