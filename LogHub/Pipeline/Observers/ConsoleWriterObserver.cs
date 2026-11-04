using LogHub.Pipeline.Entries;
using LogHub.Pipeline.Visitors;

namespace LogHub.Pipeline.Observers;

public sealed class ConsoleWriterObserver : ILogObserver
{
    private readonly TextFormatter _formatter = new();
    public List<string> WrittenEntries { get; } = new();

    public void OnEntryProcessed(LogEntry entry)
    {
        var text = entry.Accept(_formatter);
        WrittenEntries.Add(text);
    }
}