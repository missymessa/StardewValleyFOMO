using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts shipping item data for perfection tracking.
/// Implemented by SMAPI adapter reading from game content data.
/// </summary>
public interface IShippingRepository
{
    /// <summary>Get all items that count toward shipping perfection.</summary>
    IReadOnlyList<ShippingItem> GetAllShippableItems();

    /// <summary>Get items that have not yet been shipped.</summary>
    IReadOnlyList<ShippingItem> GetUnshippedItems(IReadOnlySet<string> shippedItemIds);
}
