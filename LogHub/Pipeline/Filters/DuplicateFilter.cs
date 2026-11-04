using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Filters;

public sealed class DuplicateFilter : ILogFilter
{
    private readonly HashSet<string> _seenHashes = new();

    public LogEntry? Process(LogEntry entry)
    {
        var hash = $"{entry.Timestamp:O}|{entry.Severity}|{entry.Message}";
        return _seenHashes.Add(hash) ? entry : null;
    }
}