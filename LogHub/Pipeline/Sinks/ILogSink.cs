using LogHub.Pipeline.Entries;

namespace LogHub.Pipeline.Sinks;

public interface ILogSink
{
    void Write(LogEntry entry);
    void Flush();
}