using LogHub.Pipeline.Entries;
using LogHub.Pipeline.Visitors;

namespace LogHub.Pipeline.Sinks;

public sealed class ConsoleSink : ILogSink
{
    private readonly ILogVisitor<string> _formatter;

    public ConsoleSink(ILogVisitor<string> formatter)
    {
        _formatter = formatter;
    }

    public void Write(LogEntry entry)
    {
        Console.WriteLine(entry.Accept(_formatter));
    }

    public void Flush() { }
}