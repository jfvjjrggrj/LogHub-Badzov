using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Visitors;

public sealed class StatisticsCollector : ILogVisitor<Dictionary<Type, int>>
{
    private readonly Dictionary<Type, int> _counts = new();

    public Dictionary<Type, int> Visit(SimpleLogEntry entry)
    {
        return Count(entry.GetType());
    }

    public Dictionary<Type, int> Visit(ExceptionLogEntry entry)
    {
        return Count(entry.GetType());
    }

    public Dictionary<Type, int> Visit(MetricLogEntry entry)
    {
        return Count(entry.GetType());
    }

    public Dictionary<Type, int> GetResult() => new(_counts);

    private Dictionary<Type, int> Count(Type type)
    {
        _counts[type] = _counts.GetValueOrDefault(type) + 1;
        return _counts;
    }
}