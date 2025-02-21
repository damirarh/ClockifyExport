﻿namespace ClockifyExport.Cli.Processing;

/// <summary>
/// Indicates how time entries should be grouped.
/// </summary>
internal enum TimeEntryGrouping
{
    /// <summary>
    /// Group time entries by task.
    /// </summary>
    ByTask,

    /// <summary>
    /// Group time entries by project.
    /// </summary>
    ByProject,
}
