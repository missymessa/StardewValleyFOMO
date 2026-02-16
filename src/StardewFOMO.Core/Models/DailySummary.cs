namespace StardewFOMO.Core.Models;

/// <summary>
/// Compiled planner data for a single in-game day.
/// Built by <c>DailySummaryService</c> orchestrating all sub-services.
/// </summary>
public sealed class DailySummary
{
    /// <summary>The current in-game date.</summary>
    public GameDate Date { get; init; } = null!;

    /// <summary>Current weather.</summary>
    public Weather CurrentWeather { get; init; }

    /// <summary>Fish available today with collection status.</summary>
    public IReadOnlyList<CollectibleItem> AvailableFish { get; init; } = Array.Empty<CollectibleItem>();

    /// <summary>Forageable items available today with collection status, grouped by location.</summary>
    public IReadOnlyList<CollectibleItem> AvailableForageables { get; init; } = Array.Empty<CollectibleItem>();

    /// <summary>Items from today's availability that are needed for incomplete bundles.</summary>
    public IReadOnlyList<CollectibleItem> BundleNeededItems { get; init; } = Array.Empty<CollectibleItem>();

    /// <summary>NPC birthdays occurring today.</summary>
    public IReadOnlyList<NpcBirthday> TodayBirthdays { get; init; } = Array.Empty<NpcBirthday>();

    /// <summary>NPC birthdays occurring within the lookahead window.</summary>
    public IReadOnlyList<NpcBirthday> UpcomingBirthdays { get; init; } = Array.Empty<NpcBirthday>();

    /// <summary>Preview of tomorrow's conditions.</summary>
    public TomorrowPreview? TomorrowPreview { get; init; }

    /// <summary>Last-chance alerts for season-exclusive items about to expire.</summary>
    public IReadOnlyList<CollectibleItem> LastChanceAlerts { get; init; } = Array.Empty<CollectibleItem>();

    /// <summary>All collection items for the "Show All" view (shipping, museum, cooking, crafting, etc.).</summary>
    public IReadOnlyList<CollectibleItem> AllCollectionItems { get; init; } = Array.Empty<CollectibleItem>();

    /// <summary>Whether the festival day limits normal foraging/fishing activities.</summary>
    public bool IsFestivalDay { get; init; }

    /// <summary>Festival name if applicable.</summary>
    public string? FestivalName { get; init; }
}
