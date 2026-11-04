using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Filters;

public sealed class AgeFilter : ILogFilter
{
    private readonly TimeSpan _maxAge;

    public AgeFilter(TimeSpan maxAge)
    {
        _maxAge = maxAge;
    }

    public LogEntry? Process(LogEntry entry)
    {
        var age = DateTime.UtcNow - entry.Timestamp;
        return age <= _maxAge ? entry : null;
    }
}