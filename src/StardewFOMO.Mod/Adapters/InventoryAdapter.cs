using StardewValley;
using StardewValley.Objects;
using StardewFOMO.Core.Interfaces;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IInventoryProvider"/> by reading player inventory and chests.
/// </summary>
public sealed class InventoryAdapter : IInventoryProvider
{
    /// <inheritdoc/>
    public bool HasItemInInventoryOrStorage(string itemId)
    {
        return GetTotalItemCount(itemId) > 0;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetInventoryItemIds()
    {
        var ids = new List<string>();
        if (Game1.player?.Items == null)
            return ids.AsReadOnly();

        foreach (var item in Game1.player.Items)
        {
            if (item != null)
                ids.Add(item.ParentSheetIndex.ToString());
        }
        return ids.AsReadOnly();
    }

    /// <inheritdoc/>
    public int GetTotalItemCount(string itemId)
    {
        if (!int.TryParse(itemId, out var id))
            return 0;

        int total = 0;

        // Check player inventory
        if (Game1.player?.Items != null)
        {
            foreach (var item in Game1.player.Items)
            {
                if (item != null && item.ParentSheetIndex == id)
                    total += item.Stack;
            }
        }

        // Check all accessible chests in known locations
        foreach (var location in Game1.locations)
        {
            foreach (var obj in location.Objects.Values)
            {
                if (obj is Chest chest)
                {
                    foreach (var item in chest.Items)
                    {
                        if (item != null && item.ParentSheetIndex == id)
                            total += item.Stack;
                    }
                }
            }
        }

        return total;
    }
}
