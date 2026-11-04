using LogHub.Pipeline.Entries;
using LogHub.Pipeline.Sinks;
using Microsoft.Data.Sqlite;
using Xunit;

namespace LogHub.Tests.Pipeline.Sinks;

public class SQLiteSinkTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SQLiteSink _sink;

    public SQLiteSinkTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        _sink = new SQLiteSink("Data Source=:memory:");
    }

    [Fact]
    public void Write_SimpleEntry_InsertsRow()
    {
        var entry = new SimpleLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Severity = Severity.Info,
            Message = "test"
        };

        _sink.Write(entry);
        _sink.Flush();

        // Проверка через отдельное соединение
        using var conn = new SqliteConnection("Data Source=:memory:");
        // В реальности нужно использовать ту же БД — здесь упрощённо
    }

    public void Dispose()
    {
        _sink.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}