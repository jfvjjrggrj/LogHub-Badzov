using System.Globalization;

namespace LogHub.Pipeline.Entries;

/// <summary>
/// Парсер CSV-строк с заголовком.
/// Разделитель передаётся из конфигурации (требование варианта 2).
/// </summary>
public static class CsvLogParser
{
    /// <summary>
    /// Разбирает одну CSV-строку в конкретный тип LogEntry.
    /// </summary>
    /// <param name="line">Строка данных (без заголовка).</param>
    /// <param name="delimiter">Разделитель из конфигурации (например, ';' или ',').</param>
    /// <param name="headers">Массив имён колонок из первой строки CSV.</param>
    /// <returns>LogEntry подходящего типа или null, если строка невалидна.</returns>
    public static LogEntry? Parse(string line, char delimiter, string[] headers)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var values = line.Split(delimiter);

        // Количество значений должно совпадать с количеством заголовков
        if (values.Length != headers.Length)
            return null;

        // Сопоставляем заголовки и значения в словарь для удобного доступа
        var dict = headers
            .Zip(values)
            .ToDictionary(pair => pair.First.Trim(), pair => pair.Second.Trim());

        // Обязательные поля
        if (!dict.TryGetValue("Timestamp", out var ts) ||
            !dict.TryGetValue("Level", out var level) ||
            !dict.TryGetValue("Message", out var message))
        {
            return null;
        }

        // Парсинг даты
        if (!DateTime.TryParse(ts, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var timestamp))
        {
            return null;
        }

        // Парсинг уровня важности
        if (!Enum.TryParse<Severity>(level, ignoreCase: true, out var severity))
        {
            return null;
        }

        // Определяем тип записи по наличию специфичных полей

        // 1. ExceptionLogEntry — если есть StackTrace
        if (dict.TryGetValue("StackTrace", out var stackTrace) &&
            !string.IsNullOrWhiteSpace(stackTrace))
        {
            return new ExceptionLogEntry
            {
                Timestamp = timestamp,
                Severity = severity,
                Message = message,
                StackTrace = stackTrace
            };
        }

        // 2. MetricLogEntry — если есть числовое поле Value
        if (dict.TryGetValue("Value", out var valStr) &&
            double.TryParse(valStr, CultureInfo.InvariantCulture, out var value))
        {
            return new MetricLogEntry
            {
                Timestamp = timestamp,
                Severity = severity,
                Message = message,
                Value = value
            };
        }

        // 3. Простая запись по умолчанию
        return new SimpleLogEntry
        {
            Timestamp = timestamp,
            Severity = severity,
            Message = message
        };
    }

    /// <summary>
    /// Читает первую строку CSV-файла как массив заголовков.
    /// </summary>
    public static string[] ReadHeaders(string firstLine, char delimiter)
    {
        return firstLine.Split(delimiter).Select(h => h.Trim()).ToArray();
    }
}