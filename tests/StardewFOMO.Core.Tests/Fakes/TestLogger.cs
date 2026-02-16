using StardewFOMO.Core.Interfaces;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>
/// Test logger that captures log messages for assertion.
/// Implements <see cref="ILogger"/>.
/// </summary>
public sealed class TestLogger : ILogger
{
    private readonly List<LogEntry> _entries = new();

    /// <summary>All captured log entries.</summary>
    public IReadOnlyList<LogEntry> Entries => _entries.AsReadOnly();

    /// <summary>Log entries at a specific level.</summary>
    public IEnumerable<LogEntry> GetEntries(LogLevel level) => _entries.Where(e => e.Level == level);

    /// <summary>Whether any message contains the specified text.</summary>
    public bool HasMessage(string text) => _entries.Any(e => e.Message.Contains(text, StringComparison.OrdinalIgnoreCase));

    /// <summary>Whether any message at the specified level contains the text.</summary>
    public bool HasMessage(LogLevel level, string text) =>
        _entries.Any(e => e.Level == level && e.Message.Contains(text, StringComparison.OrdinalIgnoreCase));

    /// <summary>Clear all captured entries.</summary>
    public void Clear() => _entries.Clear();

    public void Log(LogLevel level, string message, object? context = null) =>
        _entries.Add(new LogEntry(level, message, context));

    /// <summary>A single captured log entry.</summary>
    public sealed record LogEntry(LogLevel Level, string Message, object? Context);
}
