namespace ClockifyExport.Cli.Processing.PostProcessors;

/// <summary>
/// Post-processor for grouped time entries.
/// </summary>
public interface IPostProcessor
{
    /// <summary>
    /// Processes a grouped time entry.
    /// </summary>
    /// <param name="entry">Grouped time entry to process.</param>
    /// <returns>Processed grouped time entry.</returns>
    GroupedTimeEntry Process(GroupedTimeEntry entry);
}
