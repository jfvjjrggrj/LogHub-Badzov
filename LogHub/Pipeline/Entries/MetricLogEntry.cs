using LogHub.Pipeline.Visitors;

namespace LogHub.Pipeline.Entries;

public sealed class MetricLogEntry : LogEntry
{
    public double Value { get; init; }

    public override T Accept<T>(ILogVisitor<T> visitor)
        => visitor.Visit(this);
}