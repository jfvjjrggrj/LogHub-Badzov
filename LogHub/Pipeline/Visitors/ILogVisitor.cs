using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Visitors;

public interface ILogVisitor<T>
{
    T Visit(SimpleLogEntry entry);
    T Visit(ExceptionLogEntry entry);
    T Visit(MetricLogEntry entry);
}