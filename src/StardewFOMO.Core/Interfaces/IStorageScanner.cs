using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts storage scanning for player items across inventory and chests.
/// Implemented by SMAPI adapter with caching for performance.
/// </summary>
public interface IStorageScanner
{
    /// <summary>Get all items the player owns across inventory and storage.</summary>
    /// <returns>List of owned items with their locations.</returns>
    IReadOnlyList<OwnedItemInfo> GetOwnedItems();

    /// <summary>Invalidate the item cache, forcing a rescan on next access.</summary>
    void InvalidateCache();

    /// <summary>Check whether the player has a specific item meeting quality requirements.</summary>
    /// <param name="itemId">The item identifier to check.</param>
    /// <param name="minQuality">Minimum quality required (0 = normal, 1 = silver, 2 = gold, 3 = iridium).</param>
    /// <returns>True if the player has the item at or above the specified quality.</returns>
    bool HasItem(string itemId, int minQuality = 0);

    /// <summary>Get all locations where a specific item is stored.</summary>
    /// <param name="itemId">The item identifier to find.</param>
    /// <param name="minQuality">Minimum quality required.</param>
    /// <returns>List of owned item entries matching the criteria.</returns>
    IReadOnlyList<OwnedItemInfo> FindItem(string itemId, int minQuality = 0);

    /// <summary>Get the total quantity of an item across all storage.</summary>
    /// <param name="itemId">The item identifier to count.</param>
    /// <param name="minQuality">Minimum quality required.</param>
    int GetTotalQuantity(string itemId, int minQuality = 0);
}
