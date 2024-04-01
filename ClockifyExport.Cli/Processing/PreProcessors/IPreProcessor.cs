﻿using ClockifyExport.Cli.Clockify;

namespace ClockifyExport.Cli.Processing.PreProcessors;

/// <summary>
/// Pre-processor for time entries.
/// </summary>
public interface IPreProcessor
{
    /// <summary>
    /// Processes a time entry before it is grouped.
    /// </summary>
    /// <param name="entry">Time entry to process.</param>
    /// <returns>Processed time entry.</returns>
    ClockifyTimeEntry Process(ClockifyTimeEntry entry);
}
