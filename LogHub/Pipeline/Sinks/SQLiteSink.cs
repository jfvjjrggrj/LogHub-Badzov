using LogHub.Pipeline.Entries;
using Microsoft.Data.Sqlite;

namespace LogHub.Pipeline.Sinks;

public sealed class SQLiteSink : ILogSink, IDisposable
{
    private readonly SqliteConnection _connection;
    private bool _disposed;

    public SQLiteSink(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        EnsureTable();
    }

    private void EnsureTable()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Logs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Timestamp TEXT NOT NULL,
                Severity TEXT NOT NULL,
                Message TEXT NOT NULL,
                StackTrace TEXT,
                Value REAL
            )";
        cmd.ExecuteNonQuery();
    }

    public void Write(LogEntry entry)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Logs (Timestamp, Severity, Message, StackTrace, Value) VALUES (@ts, @sev, @msg, @st, @val)";
        cmd.Parameters.AddWithValue("@ts", entry.Timestamp.ToString("O"));
        cmd.Parameters.AddWithValue("@sev", entry.Severity.ToString());
        cmd.Parameters.AddWithValue("@msg", entry.Message);
        cmd.Parameters.AddWithValue("@st", (entry as ExceptionLogEntry)?.StackTrace ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@val", (entry as MetricLogEntry)?.Value ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Flush() { }

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection.Close();
            _connection.Dispose();
            _disposed = true;
        }
    }
}