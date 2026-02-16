using StardewValley;
using StardewValley.Locations;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IBundleRepository"/> by reading Community Center data.
/// </summary>
public sealed class BundleAdapter : IBundleRepository
{

    /// <inheritdoc/>
    public IReadOnlyList<BundleInfo> GetAllBundles()
    {
        var bundles = new List<BundleInfo>();
        var bundleData = Game1.netWorldState?.Value?.BundleData;
        if (bundleData == null)
            return bundles.AsReadOnly();

        var completedBundles = Game1.netWorldState!.Value!.Bundles;

        foreach (var kvp in bundleData)
        {
            // Key format: "RoomName/BundleIndex"
            var keyParts = kvp.Key.Split('/');
            var roomName = keyParts.Length > 0 ? keyParts[0] : "Unknown";

            // Value format: "BundleName/Reward/AllItemsInOneString/Color///DisplayName"
            // Where AllItemsInOneString is like "724 1 0 259 1 0 430 1 0" (triplets: itemId qty quality)
            var valueParts = kvp.Value.Split('/');
            var bundleName = valueParts.Length > 0 ? valueParts[0] : "Unknown Bundle";

            var requiredItems = new List<BundleItem>();
            var completedItemIds = new HashSet<string>();
            var slots = new List<BundleSlot>();
            int slotsRequired = 0;

            // Parse items from index 2 - ALL items are in this single field separated by spaces
            // Format: "itemId1 qty1 quality1 itemId2 qty2 quality2 ..."
            if (valueParts.Length > 2)
            {
                var itemsString = valueParts[2].Trim();
                var itemTokens = itemsString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                int slotIndex = 0;
                // Parse in groups of 3 (itemId, qty, quality)
                for (int i = 0; i + 2 < itemTokens.Length; i += 3)
                {
                    if (int.TryParse(itemTokens[i], out var itemId) && itemId > 0)
                    {
                        var quantity = int.TryParse(itemTokens[i + 1], out var q) ? q : 1;
                        var quality = int.TryParse(itemTokens[i + 2], out var qual) ? qual : 0;
                        var itemName = Game1.objectData != null &&
                                       Game1.objectData.TryGetValue(itemId.ToString(), out var info)
                            ? info.DisplayName
                            : $"Item {itemId}";

                        var bundleItem = new BundleItem
                        {
                            ItemId = itemId.ToString(),
                            ItemName = itemName,
                            Quantity = quantity,
                            MinimumQuality = quality
                        };
                        
                        requiredItems.Add(bundleItem);
                        
                        slots.Add(new BundleSlot
                        {
                            SlotIndex = slotIndex,
                            ValidItems = new List<BundleItem> { bundleItem }.AsReadOnly(),
                            IsFilled = false,
                            FilledWithItemId = null
                        });
                        
                        slotIndex++;
                    }
                }
            }

            // Default slotsRequired to all items (standard bundles require all items)
            slotsRequired = requiredItems.Count;

            // Check completion state from the bundle rewards dictionary
            if (int.TryParse(keyParts.Length > 1 ? keyParts[1] : "", out var bundleIndex))
            {
                if (completedBundles.TryGetValue(bundleIndex, out var completed))
                {
                    for (int i = 0; i < completed.Length && i < requiredItems.Count; i++)
                    {
                        if (completed[i])
                        {
                            completedItemIds.Add(requiredItems[i].ItemId);
                            
                            // Mark the slot as filled
                            if (i < slots.Count)
                            {
                                slots[i] = new BundleSlot
                                {
                                    SlotIndex = slots[i].SlotIndex,
                                    ValidItems = slots[i].ValidItems,
                                    IsFilled = true,
                                    FilledWithItemId = requiredItems[i].ItemId
                                };
                            }
                        }
                    }
                }
            }

            bundles.Add(new BundleInfo
            {
                BundleName = bundleName,
                RoomName = roomName,
                RequiredItems = requiredItems.AsReadOnly(),
                CompletedItemIds = completedItemIds,
                Slots = slots.AsReadOnly(),
                SlotsRequired = slotsRequired
            });
        }

        return bundles.AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<BundleInfo> GetIncompleteBundles() =>
        GetAllBundles().Where(b => !b.IsComplete).ToList().AsReadOnly();

    /// <inheritdoc/>
    public IReadOnlyList<string> GetBundleNamesNeedingItem(string itemId) =>
        GetIncompleteBundles()
            .Where(b => b.GetRemainingItems().Any(r => r.ItemId == itemId))
            .Select(b => b.BundleName)
            .ToList()
            .AsReadOnly();

    /// <inheritdoc/>
    public bool IsCommunityCenterActive()
    {
        // Check if player chose Joja route
        if (Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
            return false;

        // Check if community center is still active (not completed)
        var cc = Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
        return cc != null;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetAllRooms()
    {
        var rooms = GetAllBundles()
            .Select(b => b.RoomName)
            .Distinct()
            .OrderBy(r => GetRoomSortOrder(r))
            .ToList();
        return rooms.AsReadOnly();
    }

    /// <inheritdoc/>
    public RoomProgress GetRoomProgress(string roomName)
    {
        var bundles = GetBundlesByRoom(roomName);
        return new RoomProgress
        {
            RoomName = roomName,
            TotalBundles = bundles.Count,
            CompletedBundles = bundles.Count(b => b.IsComplete)
        };
    }

    /// <inheritdoc/>
    public IReadOnlyList<BundleInfo> GetBundlesByRoom(string roomName) =>
        GetAllBundles()
            .Where(b => b.RoomName.Equals(roomName, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

    /// <inheritdoc/>
    public bool IsCommunityComplete()
    {
        if (!IsCommunityCenterActive())
            return false;

        var allBundles = GetAllBundles();
        return allBundles.Count > 0 && allBundles.All(b => b.IsComplete);
    }

    /// <summary>Get sort order for rooms to display in canonical order.</summary>
    private static int GetRoomSortOrder(string roomName) => roomName.ToLowerInvariant() switch
    {
        "crafts room" => 0,
        "pantry" => 1,
        "fish tank" => 2,
        "boiler room" => 3,
        "bulletin board" => 4,
        "vault" => 5,
        _ => 99
    };
}
