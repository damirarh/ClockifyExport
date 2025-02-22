﻿namespace ClockifyExport.Cli.Processing.PostProcessors;

/// <summary>
/// Rounds up the hours of a grouped time entry to the nearest multiple of a given number of minutes.
/// </summary>
/// <param name="roundUpToMinutes">The number of minutes to round up to.</param>
internal sealed class RoundingPostProcessor(int roundUpToMinutes) : IPostProcessor
{
    /// <inheritdoc/>
    public GroupedTimeEntry Process(GroupedTimeEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry, nameof(entry));
        return entry with
        {
            Hours = Math.Ceiling(entry.Hours * 60 / roundUpToMinutes) * roundUpToMinutes / 60
        };
    }
}
