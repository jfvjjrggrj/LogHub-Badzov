using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Observers;

public sealed class PipelineSubject
{
    private readonly List<ILogObserver> _observers = new();
    private LogEntry? _lastEntry;

    public IDisposable Subscribe(ILogObserver observer)
    {
        _observers.Add(observer);
        return new Subscription(() => _observers.Remove(observer));
    }

    public LogEntry? LastEntry => _lastEntry;

    public void Notify(LogEntry entry)
    {
        _lastEntry = entry;
        foreach (var observer in _observers.ToList())
        {
            try
            {
                observer.OnEntryProcessed(entry);
            }
            catch (Exception)
            {
                // Исключение в одном подписчике не мешает остальным
            }
        }
    }

    private sealed class Subscription : IDisposable
    {
        private readonly Action _onDispose;
        private bool _disposed;

        public Subscription(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _onDispose();
                _disposed = true;
            }
        }
    }
}