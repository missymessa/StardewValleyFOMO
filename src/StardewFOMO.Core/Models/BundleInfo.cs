namespace StardewFOMO.Core.Models;

/// <summary>
/// Represents a Community Center bundle with its name, room, required items, and completion state.
/// </summary>
public sealed class BundleInfo
{
    /// <summary>The display name of the bundle.</summary>
    public string BundleName { get; init; } = string.Empty;

    /// <summary>The Community Center room this bundle belongs to.</summary>
    public string RoomName { get; init; } = string.Empty;

    /// <summary>Item IDs required to complete this bundle.</summary>
    public IReadOnlyList<BundleItem> RequiredItems { get; init; } = Array.Empty<BundleItem>();

    /// <summary>Item IDs that have already been contributed to this bundle.</summary>
    public IReadOnlySet<string> CompletedItemIds { get; init; } = new HashSet<string>();

    /// <summary>Whether the entire bundle is complete.</summary>
    public bool IsComplete => RequiredItems.All(r => CompletedItemIds.Contains(r.ItemId));

    /// <summary>Get the items still needed for this bundle.</summary>
    public IEnumerable<BundleItem> GetRemainingItems() =>
        RequiredItems.Where(r => !CompletedItemIds.Contains(r.ItemId));
}

/// <summary>An individual item requirement within a bundle.</summary>
public sealed class BundleItem
{
    /// <summary>The item identifier.</summary>
    public string ItemId { get; init; } = string.Empty;

    /// <summary>The display name of the item.</summary>
    public string ItemName { get; init; } = string.Empty;

    /// <summary>Quantity required.</summary>
    public int Quantity { get; init; } = 1;

    /// <summary>Minimum quality required (0 = normal, 1 = silver, 2 = gold, 3 = iridium).</summary>
    public int MinimumQuality { get; init; }
}
