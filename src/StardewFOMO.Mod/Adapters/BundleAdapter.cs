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

            // Value format: "BundleName//Item1 Qty1 Quality1/Item2 Qty2 Quality2/.../Color:SlotsRequired"
            var valueParts = kvp.Value.Split('/');
            var bundleName = valueParts.Length > 0 ? valueParts[0] : "Unknown Bundle";

            var requiredItems = new List<BundleItem>();
            var completedItemIds = new HashSet<string>();
            var slots = new List<BundleSlot>();
            int slotsRequired = 0;

            // Parse required items (starting from index 2, each group of 3: itemId qty quality)
            int slotIndex = 0;
            for (int i = 2; i < valueParts.Length; i++)
            {
                var itemData = valueParts[i].Trim();
                
                // Skip empty parts
                if (string.IsNullOrWhiteSpace(itemData))
                    continue;
                
                // Check if this is the color/slots required field (e.g., "4:6" means color 4, 6 slots required)
                if (itemData.Contains(':') || (itemData.Length <= 3 && int.TryParse(itemData, out _) && i == valueParts.Length - 1))
                {
                    // Parse slots required from color field "ColorIndex:SlotsRequired"
                    var colorParts = itemData.Split(':');
                    if (colorParts.Length >= 2 && int.TryParse(colorParts[1], out var reqSlots))
                    {
                        slotsRequired = reqSlots;
                    }
                    continue;
                }

                var itemParts = itemData.Split(' ');
                if (itemParts.Length >= 3 && int.TryParse(itemParts[0], out var itemId) && itemId > 0)
                {
                    var quantity = int.TryParse(itemParts[1], out var q) ? q : 1;
                    var quality = int.TryParse(itemParts[2], out var qual) ? qual : 0;
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
                    
                    // Create a slot for this item
                    slots.Add(new BundleSlot
                    {
                        SlotIndex = slotIndex,
                        ValidItems = new List<BundleItem> { bundleItem }.AsReadOnly(),
                        IsFilled = false, // Will be set below
                        FilledWithItemId = null
                    });
                    
                    slotIndex++;
                }
            }

            // If no slotsRequired was specified, default to all items required
            if (slotsRequired == 0)
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
