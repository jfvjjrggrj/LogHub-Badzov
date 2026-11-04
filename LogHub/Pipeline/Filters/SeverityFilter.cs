using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Filters;

public sealed class SeverityFilter : ILogFilter
{
    private readonly Severity _minSeverity;

    public SeverityFilter(Severity minSeverity)
    {
        _minSeverity = minSeverity;
    }

    public LogEntry? Process(LogEntry entry)
    {
        return entry.Severity >= _minSeverity ? entry : null;
    }
}