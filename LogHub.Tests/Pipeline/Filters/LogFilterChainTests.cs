using LogHub.Pipeline.Entries;
using LogHub.Pipeline.Filters;
using Xunit;

namespace LogHub.Tests.Pipeline.Filters;

public class LogFilterChainTests
{
    [Fact]
    public void EmptyChain_PassesAllEntries()
    {
        var chain = new LogFilterChain(Array.Empty<ILogFilter>());
        var entry = new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "test" };

        var result = chain.Process(entry);

        Assert.NotNull(result);
        Assert.Same(entry, result);
    }

    [Fact]
    public void SeverityFilter_RejectsBelowThreshold()
    {
        var filter = new SeverityFilter(Severity.Warning);
        var entry = new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "test" };

        var result = filter.Process(entry);

        Assert.Null(result);
    }

    [Fact]
    public void SeverityFilter_PassesAboveThreshold()
    {
        var filter = new SeverityFilter(Severity.Warning);
        var entry = new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Error, Message = "test" };

        var result = filter.Process(entry);

        Assert.NotNull(result);
    }

    [Fact]
    public void AgeFilter_RejectsOldEntries()
    {
        var filter = new AgeFilter(TimeSpan.FromDays(1));
        var entry = new SimpleLogEntry { Timestamp = DateTime.UtcNow.AddDays(-2), Severity = Severity.Info, Message = "test" };

        var result = filter.Process(entry);

        Assert.Null(result);
    }

    [Fact]
    public void DuplicateFilter_RejectsDuplicates()
    {
        var filter = new DuplicateFilter();
        var entry = new SimpleLogEntry { Timestamp = DateTime.UtcNow, Severity = Severity.Info, Message = "test" };

        var first = filter.Process(entry);
        var second = filter.Process(entry);

        Assert.NotNull(first);
        Assert.Null(second);
    }

    [Fact]
    public void ChainOrder_AffectsResult()
    {
        var severityFilter = new SeverityFilter(Severity.Warning);
        var ageFilter = new AgeFilter(TimeSpan.FromDays(1));

        var chain1 = new LogFilterChain(new ILogFilter[] { severityFilter, ageFilter });
        var chain2 = new LogFilterChain(new ILogFilter[] { ageFilter, severityFilter });

        var oldWarningEntry = new SimpleLogEntry { Timestamp = DateTime.UtcNow.AddDays(-2), Severity = Severity.Warning, Message = "test" };

        Assert.Null(chain1.Process(oldWarningEntry)); // Отсекается по возрасту
        Assert.Null(chain2.Process(oldWarningEntry)); // Отсекается по возрасту
    }
}