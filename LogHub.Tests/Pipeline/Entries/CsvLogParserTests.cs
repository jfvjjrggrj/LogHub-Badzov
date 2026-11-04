using LogHub.Pipeline.Entries;
using Xunit;

namespace LogHub.Tests.Pipeline.Entries;

public class CsvLogParserTests
{
    private const char Delimiter = ';';
    private static readonly string[] Headers = { "Timestamp", "Level", "Message", "StackTrace", "Value" };

    [Fact]
    public void Parse_SimpleEntry_ReturnsSimpleLogEntry()
    {
        var line = "2026-11-03T10:00:00Z;INFO;Application started;;";
        var entry = CsvLogParser.Parse(line, Delimiter, Headers);

        Assert.NotNull(entry);
        Assert.IsType<SimpleLogEntry>(entry);
        Assert.Equal(Severity.Info, entry!.Severity);
        Assert.Equal("Application started", entry.Message);
    }

    [Fact]
    public void Parse_ExceptionEntry_ReturnsExceptionLogEntry()
    {
        var line = "2026-11-03T10:00:00Z;ERROR;NullRef;at Program.Main;";
        var entry = CsvLogParser.Parse(line, Delimiter, Headers);

        Assert.NotNull(entry);
        Assert.IsType<ExceptionLogEntry>(entry);
        Assert.Contains("at Program.Main", ((ExceptionLogEntry)entry!).StackTrace);
    }

    [Fact]
    public void Parse_MetricEntry_ReturnsMetricLogEntry()
    {
        var line = "2026-11-03T10:00:00Z;INFO;CPU load;;42.5";
        var entry = CsvLogParser.Parse(line, Delimiter, Headers);

        Assert.NotNull(entry);
        Assert.IsType<MetricLogEntry>(entry);
        Assert.Equal(42.5, ((MetricLogEntry)entry!).Value);
    }

    [Fact]
    public void Parse_WrongColumnCount_ReturnsNull()
    {
        var line = "2026-11-03T10:00:00Z;INFO";
        var entry = CsvLogParser.Parse(line, Delimiter, Headers);
        Assert.Null(entry);
    }

    [Fact]
    public void Parse_TooManyColumns_ReturnsNull()
    {
        var line = "2026-11-03T10:00:00Z;INFO;msg;;;extra";
        var entry = CsvLogParser.Parse(line, Delimiter, Headers);
        Assert.Null(entry);
    }

    [Fact]
    public void Parse_InvalidTimestamp_ReturnsNull()
    {
        var line = "not-a-date;INFO;msg;;";
        var entry = CsvLogParser.Parse(line, Delimiter, Headers);
        Assert.Null(entry);
    }

    [Fact]
    public void Parse_InvalidLevel_ReturnsNull()
    {
        var line = "2026-11-03T10:00:00Z;UNKNOWN_LEVEL;msg;;";
        var entry = CsvLogParser.Parse(line, Delimiter, Headers);
        Assert.Null(entry);
    }

    [Fact]
    public void Parse_EmptyLine_ReturnsNull()
    {
        var entry = CsvLogParser.Parse("", Delimiter, Headers);
        Assert.Null(entry);
    }

    [Fact]
    public void Parse_CommaDelimiter_WorksWhenConfigured()
    {
        var commaHeaders = new[] { "Timestamp", "Level", "Message" };
        var line = "2026-11-03T10:00:00Z,INFO,msg";
        var entry = CsvLogParser.Parse(line, ',', commaHeaders);

        Assert.NotNull(entry);
        Assert.Equal(Severity.Info, entry!.Severity);
    }

    [Fact]
    public void Parse_TabDelimiter_WorksWhenConfigured()
    {
        var tabHeaders = new[] { "Timestamp", "Level", "Message" };
        var line = "2026-11-03T10:00:00Z\tWARNING\tmsg";
        var entry = CsvLogParser.Parse(line, '\t', tabHeaders);

        Assert.NotNull(entry);
        Assert.Equal(Severity.Warning, entry!.Severity);
    }

    [Fact]
    public void Parse_MissingRequiredField_ReturnsNull()
    {
        var headers = new[] { "Timestamp", "Level" };
        var line = "2026-11-03T10:00:00Z;INFO";
        var entry = CsvLogParser.Parse(line, Delimiter, headers);
        Assert.Null(entry);
    }

    [Fact]
    public void ReadHeaders_ReturnsTrimmedHeaders()
    {
        var firstLine = " Timestamp ; Level ; Message ";
        var headers = CsvLogParser.ReadHeaders(firstLine, Delimiter);

        Assert.Equal(3, headers.Length);
        Assert.Equal("Timestamp", headers[0]);
        Assert.Equal("Level", headers[1]);
        Assert.Equal("Message", headers[2]);
    }
}