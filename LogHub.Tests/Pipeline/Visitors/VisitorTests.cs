using LogHub.Pipeline.Entries;
using LogHub.Pipeline.Visitors;
using Xunit;

namespace LogHub.Tests.Pipeline.Visitors;

public class VisitorTests
{
    [Fact]
    public void TextFormatter_FormatsSimpleEntry()
    {
        var formatter = new TextFormatter();
        var entry = new SimpleLogEntry { Timestamp = new DateTime(2026, 1, 1), Severity = Severity.Info, Message = "test" };

        var result = entry.Accept(formatter);

        Assert.Contains("test", result);
        Assert.Contains("Info", result);
    }

    [Fact]
    public void TextFormatter_FormatsExceptionEntry_WithStackTrace()
    {
        var formatter = new TextFormatter();
        var entry = new ExceptionLogEntry { Timestamp = new DateTime(2026, 1, 1), Severity = Severity.Error, Message = "error", StackTrace = "stack" };

        var result = entry.Accept(formatter);

        Assert.Contains("stack", result);
    }

    [Fact]
    public void StatisticsCollector_CountsByType()
    {
        var collector = new StatisticsCollector();
        var entries = new LogEntry[]
        {
            new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "1" },
            new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "2" },
            new ExceptionLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Error, Message = "3" }
        };

        foreach (var entry in entries)
        {
            entry.Accept(collector);
        }

        var result = collector.GetResult();
        Assert.Equal(2, result[typeof(SimpleLogEntry)]);
        Assert.Equal(1, result[typeof(ExceptionLogEntry)]);
    }

    [Fact]
    public void NewVisitor_CanBeAddedWithoutChangingEntries()
    {
        // Доказательство OCP: добавляем нового посетителя без правки классов записей
        var jsonVisitor = new JsonVisitor();
        var entry = new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "test" };

        var result = entry.Accept(jsonVisitor);

        Assert.Contains("\"Message\":\"test\"", result);
    }
}

// Новый посетитель для демонстрации OCP
public class JsonVisitor : ILogVisitor<string>
{
    public string Visit(SimpleLogEntry entry) => $"{{\"Message\":\"{entry.Message}\"}}";
    public string Visit(ExceptionLogEntry entry) => $"{{\"Message\":\"{entry.Message}\",\"StackTrace\":\"{entry.StackTrace}\"}}";
    public string Visit(MetricLogEntry entry) => $"{{\"Message\":\"{entry.Message}\",\"Value\":{entry.Value}}}";
}