using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Observers;

public sealed class PullCounterObserver : ILogObserver
{
    private readonly PipelineSubject _subject;
    public int Count { get; private set; }

    public PullCounterObserver(PipelineSubject subject)
    {
        _subject = subject;
    }

    public void OnEntryProcessed(LogEntry entry)
    {
        // Pull-модель: игнорируем переданный entry, сами берём из subject
        if (_subject.LastEntry != null)
        {
            Count++;
        }
    }
}