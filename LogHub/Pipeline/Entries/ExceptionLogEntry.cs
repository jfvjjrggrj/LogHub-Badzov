using LogHub.Pipeline.Visitors;

namespace LogHub.Pipeline.Entries;

public sealed class ExceptionLogEntry : LogEntry
{
    public string StackTrace { get; init; } = "";

    public override T Accept<T>(ILogVisitor<T> visitor)
        => visitor.Visit(this);
}