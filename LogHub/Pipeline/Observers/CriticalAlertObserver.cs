using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Observers;

public sealed class CriticalAlertObserver : ILogObserver
{
    public List<string> Alerts { get; } = new();

    public void OnEntryProcessed(LogEntry entry)
    {
        if (entry.Severity == Severity.Critical)
        {
            Alerts.Add($"CRITICAL: {entry.Message}");
        }
    }
}