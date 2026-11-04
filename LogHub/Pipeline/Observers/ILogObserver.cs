using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Observers;

public interface ILogObserver
{
    void OnEntryProcessed(LogEntry entry);
}