namespace StardewFOMO.Core.Models;

/// <summary>
/// Individual trackable item within a perfection category.
/// </summary>
public class PerfectionItem
{
    /// <summary>Unique identifier for the item.</summary>
    public string ItemId { get; init; } = string.Empty;

    /// <summary>Display name of the item.</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Whether this item has been completed (shipped, caught, cooked, etc.).</summary>
    public bool IsComplete { get; init; }

    /// <summary>Whether the player currently owns this item in inventory or storage.</summary>
    public bool IsOwned { get; init; }

    /// <summary>Whether this item is obtainable today (correct season, weather, time).</summary>
    public bool IsAvailableToday { get; init; }

    /// <summary>Hint about how to acquire this item.</summary>
    public string AcquisitionHint { get; init; } = string.Empty;
}
