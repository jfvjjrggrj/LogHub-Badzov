using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Filters;

public interface ILogFilter
{
    LogEntry? Process(LogEntry entry);
}