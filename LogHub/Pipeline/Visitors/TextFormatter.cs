using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Visitors;

public sealed class TextFormatter : ILogVisitor<string>
{
    public string Visit(SimpleLogEntry entry)
    {
        return $"[{entry.Timestamp:O}] {entry.Severity} {entry.Message}";
    }

    public string Visit(ExceptionLogEntry entry)
    {
        return $"[{entry.Timestamp:O}] {entry.Severity} {entry.Message}\n{entry.StackTrace}";
    }

    public string Visit(MetricLogEntry entry)
    {
        return $"[{entry.Timestamp:O}] {entry.Severity} {entry.Message} value={entry.Value}";
    }
}