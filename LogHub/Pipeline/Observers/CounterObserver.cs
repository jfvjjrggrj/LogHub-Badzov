using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Observers;

public sealed class CounterObserver : ILogObserver
{
    public int Count { get; private set; }

    public void OnEntryProcessed(LogEntry entry)
    {
        Count++;
    }
}