namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Logging abstraction for core services.
/// SMAPI adapter implements via <c>IMonitor</c>; tests use a capturing logger.
/// </summary>
public interface ILogger
{
    /// <summary>Log a message at the specified level with optional context.</summary>
    /// <param name="level">Severity level.</param>
    /// <param name="message">Human-readable log message.</param>
    /// <param name="context">Optional structured context (e.g., player name, date).</param>
    void Log(LogLevel level, string message, object? context = null);
}

/// <summary>Severity levels for log messages.</summary>
public enum LogLevel
{
    /// <summary>Verbose debugging information.</summary>
    Trace,
    /// <summary>Diagnostic information for developers.</summary>
    Debug,
    /// <summary>Informational messages about normal operation.</summary>
    Info,
    /// <summary>Potentially harmful situations.</summary>
    Warn,
    /// <summary>Error events that might still allow the mod to continue.</summary>
    Error
}
