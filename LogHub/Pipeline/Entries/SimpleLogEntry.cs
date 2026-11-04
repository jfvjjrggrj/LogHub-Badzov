using LogHub.Pipeline.Visitors;

namespace LogHub.Pipeline.Entries;

public sealed class SimpleLogEntry : LogEntry
{
    public override T Accept<T>(ILogVisitor<T> visitor)
        => visitor.Visit(this);
}