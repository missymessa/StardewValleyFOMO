namespace StardewFOMO.Core.Models;

/// <summary>The type of collectible item for categorization.</summary>
public enum CollectionType
{
    /// <summary>A catchable fish.</summary>
    Fish,
    /// <summary>A forageable item.</summary>
    Forage,
    /// <summary>An artifact (museum donation).</summary>
    Artifact,
    /// <summary>A mineral (museum donation).</summary>
    Mineral,
    /// <summary>An item that can be shipped.</summary>
    ShippableItem,
    /// <summary>A cooking recipe.</summary>
    CookingRecipe,
    /// <summary>A crafting recipe.</summary>
    CraftingRecipe
}

/// <summary>
/// Represents any trackable collectible item across all collection types.
/// </summary>
public sealed class CollectibleItem
{
    /// <summary>Unique item identifier.</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Display name of the item.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>The type of collection this item belongs to.</summary>
    public CollectionType CollectionType { get; init; }

    /// <summary>Seasons when this item is available (empty = all seasons).</summary>
    public IReadOnlyList<Season> AvailableSeasons { get; init; } = Array.Empty<Season>();

    /// <summary>Weather conditions required (empty = any weather).</summary>
    public IReadOnlyList<Weather> RequiredWeather { get; init; } = Array.Empty<Weather>();

    /// <summary>Locations where this item can be found.</summary>
    public IReadOnlyList<string> Locations { get; init; } = Array.Empty<string>();

    /// <summary>Earliest time of day this item is available (e.g., 600). Null = any time.</summary>
    public int? StartTime { get; init; }

    /// <summary>Latest time of day this item is available (e.g., 2000). Null = any time.</summary>
    public int? EndTime { get; init; }

    /// <summary>Current collection status for this item.</summary>
    public CollectionStatus CollectionStatus { get; set; }

    /// <summary>Bundle names that need this item (empty if not needed by any bundle).</summary>
    public IList<string> BundleNames { get; set; } = new List<string>();

    /// <summary>Whether this item is needed for at least one incomplete bundle.</summary>
    public bool IsNeededForBundle => BundleNames.Count > 0;

    /// <summary>Whether this item is exclusive to a specific season.</summary>
    public bool IsSeasonExclusive => AvailableSeasons.Count == 1;

    /// <summary>Whether this item requires specific weather conditions.</summary>
    public bool IsWeatherDependent => RequiredWeather.Count > 0;
}
