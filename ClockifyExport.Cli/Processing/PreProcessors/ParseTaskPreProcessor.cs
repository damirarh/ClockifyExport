using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ClockifyExport.Cli.Clockify;

namespace ClockifyExport.Cli.Processing.PreProcessors;

/// <summary>
/// Pre-processor for time entries that parses the task id from the task using a regular expression.
/// </summary>
/// <param name="regexPattern">
/// Regular expression to use for parsing.
/// Full match should be the task id to parse.
/// </param>
public class ParseTaskPreProcessor([StringSyntax("Regex")] string regexPattern) : IPreProcessor
{
    private readonly Regex regex = new(regexPattern, RegexOptions.Compiled);

    /// <inheritdoc/>
    public ClockifyTimeEntry Process(ClockifyTimeEntry entry, out string? validationError)
    {
        ArgumentNullException.ThrowIfNull(entry, nameof(entry));

        if (!string.IsNullOrEmpty(entry.Task))
        {
            var match = regex.Match(entry.Task);
            if (match.Success)
            {
                validationError = null;
                return entry with { Task = match.Value };
            }
        }

        validationError = $"Couldn't parse task Id from: {entry.Task}";
        return entry;
    }
}
