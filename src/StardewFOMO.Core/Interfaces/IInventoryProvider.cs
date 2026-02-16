namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts player inventory and storage access.
/// Implemented by SMAPI adapter reading player inventory and chests.
/// </summary>
public interface IInventoryProvider
{
    /// <summary>Check whether the player has a specific item in inventory or any storage.</summary>
    bool HasItemInInventoryOrStorage(string itemId);

    /// <summary>Get all item IDs currently in the player's inventory.</summary>
    IReadOnlyList<string> GetInventoryItemIds();

    /// <summary>Get the total quantity of a specific item across inventory and all storage.</summary>
    int GetTotalItemCount(string itemId);
}
