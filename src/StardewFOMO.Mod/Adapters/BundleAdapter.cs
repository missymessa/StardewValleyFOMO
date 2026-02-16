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

            // Value format: "BundleName//Item1 Qty1 Quality1/Item2 Qty2 Quality2/..."
            var valueParts = kvp.Value.Split('/');
            var bundleName = valueParts.Length > 0 ? valueParts[0] : "Unknown Bundle";

            var requiredItems = new List<BundleItem>();
            var completedItemIds = new HashSet<string>();

            // Parse required items (starting from index 2, each group of 3: itemId qty quality)
            for (int i = 2; i < valueParts.Length; i++)
            {
                var itemParts = valueParts[i].Trim().Split(' ');
                if (itemParts.Length >= 3 && int.TryParse(itemParts[0], out var itemId) && itemId > 0)
                {
                    var quantity = int.TryParse(itemParts[1], out var q) ? q : 1;
                    var quality = int.TryParse(itemParts[2], out var qual) ? qual : 0;
                    var itemName = Game1.objectData != null &&
                                   Game1.objectData.TryGetValue(itemId.ToString(), out var info)
                        ? info.DisplayName
                        : $"Item {itemId}";

                    requiredItems.Add(new BundleItem
                    {
                        ItemId = itemId.ToString(),
                        ItemName = itemName,
                        Quantity = quantity,
                        MinimumQuality = quality
                    });
                }
            }

            // Check completion state from the bundle rewards dictionary
            if (int.TryParse(keyParts.Length > 1 ? keyParts[1] : "", out var bundleIndex))
            {
                if (completedBundles.TryGetValue(bundleIndex, out var completed))
                {
                    for (int i = 0; i < completed.Length && i < requiredItems.Count; i++)
                    {
                        if (completed[i])
                            completedItemIds.Add(requiredItems[i].ItemId);
                    }
                }
            }

            bundles.Add(new BundleInfo
            {
                BundleName = bundleName,
                RoomName = roomName,
                RequiredItems = requiredItems.AsReadOnly(),
                CompletedItemIds = completedItemIds
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
}
