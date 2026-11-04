using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Filters;

public sealed class LogFilterChain : ILogFilter
{
    private readonly IReadOnlyList<ILogFilter> _filters;

    public LogFilterChain(IEnumerable<ILogFilter> filters)
    {
        _filters = filters.ToList();
    }

    public LogEntry? Process(LogEntry entry)
    {
        LogEntry? current = entry;
        foreach (var filter in _filters)
        {
            current = filter.Process(current);
            if (current == null)
                return null;
        }
        return current;
    }
}