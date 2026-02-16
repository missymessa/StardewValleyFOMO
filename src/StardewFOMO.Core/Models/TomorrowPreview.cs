namespace StardewFOMO.Core.Models;

/// <summary>
/// A forward-looking summary for tomorrow: weather forecast, new collectibles, events.
/// </summary>
public sealed class TomorrowPreview
{
    /// <summary>Tomorrow's weather forecast.</summary>
    public Weather WeatherForecast { get; init; }

    /// <summary>Collectibles that become newly available tomorrow (e.g., rain-exclusive fish).</summary>
    public IReadOnlyList<CollectibleItem> NewCollectibles { get; init; } = Array.Empty<CollectibleItem>();

    /// <summary>Events or festivals occurring tomorrow.</summary>
    public IReadOnlyList<string> Events { get; init; } = Array.Empty<string>();

    /// <summary>Warning message if tomorrow starts a new season, with uncollected items at risk.</summary>
    public string? SeasonChangeWarning { get; init; }

    /// <summary>Items that will become unavailable after the season change.</summary>
    public IReadOnlyList<CollectibleItem> ExpiringItems { get; init; } = Array.Empty<CollectibleItem>();
}
