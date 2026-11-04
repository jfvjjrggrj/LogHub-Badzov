using LogHub.Pipeline.Visitors;

namespace LogHub.Pipeline.Entries;

public abstract class LogEntry
{
    public DateTime Timestamp { get; init; }
    public Severity Severity { get; init; }
    public string Message { get; init; } = "";

    // Двойная диспетчеризация: Accept переопределяется в каждом наследнике!
    public abstract T Accept<T>(ILogVisitor<T> visitor);
}