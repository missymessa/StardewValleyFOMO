namespace StardewFOMO.Core.Models;

/// <summary>
/// Represents a single slot within a bundle that may accept one or more valid items.
/// Supports "OR" requirements where multiple items can satisfy the same slot.
/// </summary>
public sealed class BundleSlot
{
    /// <summary>The index of this slot within the bundle (0-based).</summary>
    public int SlotIndex { get; init; }

    /// <summary>List of items that can satisfy this slot (OR requirement).</summary>
    public IReadOnlyList<BundleItem> ValidItems { get; init; } = Array.Empty<BundleItem>();

    /// <summary>Whether this slot has been filled with an item.</summary>
    public bool IsFilled { get; init; }

    /// <summary>The item ID that was used to fill this slot, if filled.</summary>
    public string? FilledWithItemId { get; init; }

    /// <summary>Get the first valid item (convenience for single-item slots).</summary>
    public BundleItem? PrimaryItem => ValidItems.Count > 0 ? ValidItems[0] : null;

    /// <summary>Whether this slot accepts multiple different items.</summary>
    public bool HasOrRequirement => ValidItems.Count > 1;
}
