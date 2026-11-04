using LogHub.Pipeline.Entries;
using LogHub.Pipeline.Observers;
using Xunit;

namespace LogHub.Tests.Pipeline.Observers;

public class ObserverTests
{
    [Fact]
    public void UnsubscribedObserver_DoesNotReceiveEvents()
    {
        var subject = new PipelineSubject();
        var counter = new CounterObserver();

        using var subscription = subject.Subscribe(counter);
        subject.Notify(new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "1" });
        Assert.Equal(1, counter.Count);

        subscription.Dispose();
        subject.Notify(new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "2" });
        Assert.Equal(1, counter.Count); // Не увеличился
    }

    [Fact]
    public void ExceptionInOneObserver_OthersStillWork()
    {
        var subject = new PipelineSubject();
        var counter = new CounterObserver();
        var failingObserver = new FailingObserver();

        subject.Subscribe(failingObserver);
        subject.Subscribe(counter);

        subject.Notify(new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "test" });

        Assert.Equal(1, counter.Count); // Счётчик всё равно сработал
    }

    [Fact]
    public void PushAndPull_GiveSameResult()
    {
        var subject = new PipelineSubject();
        var pushCounter = new CounterObserver();
        var pullCounter = new PullCounterObserver(subject);

        subject.Subscribe(pushCounter);
        subject.Subscribe(pullCounter);

        subject.Notify(new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "1" });
        subject.Notify(new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "2" });

        Assert.Equal(pushCounter.Count, pullCounter.Count);
    }

    [Fact]
    public void CriticalAlert_FiresOnlyOnCritical()
    {
        var subject = new PipelineSubject();
        var alert = new CriticalAlertObserver();

        subject.Subscribe(alert);

        subject.Notify(new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "info" });
        subject.Notify(new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Critical, Message = "critical" });

        Assert.Single(alert.Alerts);
        Assert.Contains("critical", alert.Alerts[0]);
    }
}

public class FailingObserver : ILogObserver
{
    public void OnEntryProcessed(LogEntry entry)
    {
        throw new InvalidOperationException("Test exception");
    }
}