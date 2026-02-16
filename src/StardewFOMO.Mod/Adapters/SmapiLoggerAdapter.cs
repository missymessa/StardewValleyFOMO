using StardewModdingAPI;
using StardewFOMO.Core.Interfaces;
using CoreLogLevel = StardewFOMO.Core.Interfaces.LogLevel;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter that forwards core <see cref="ILogger"/> calls to SMAPI's <see cref="IMonitor"/>.
/// </summary>
public sealed class SmapiLoggerAdapter : ILogger
{
    private readonly IMonitor _monitor;

    /// <summary>Initializes a new instance wrapping the SMAPI monitor.</summary>
    public SmapiLoggerAdapter(IMonitor monitor)
    {
        _monitor = monitor;
    }

    /// <inheritdoc/>
    public void Log(CoreLogLevel level, string message, object? context = null)
    {
        var smapiLevel = level switch
        {
            CoreLogLevel.Trace => StardewModdingAPI.LogLevel.Trace,
            CoreLogLevel.Debug => StardewModdingAPI.LogLevel.Debug,
            CoreLogLevel.Info => StardewModdingAPI.LogLevel.Info,
            CoreLogLevel.Warn => StardewModdingAPI.LogLevel.Warn,
            CoreLogLevel.Error => StardewModdingAPI.LogLevel.Error,
            _ => StardewModdingAPI.LogLevel.Trace
        };

        _monitor.Log(message, smapiLevel);
    }
}
