using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for matching player-owned items to bundle requirements.
/// Considers quality requirements and OR requirements (multiple valid items per slot).
/// </summary>
public sealed class BundleItemMatcherService
{
    private readonly IStorageScanner _storageScanner;
    private readonly IBundleRepository _bundleRepo;
    private readonly ILogger _logger;

    /// <summary>Creates a new instance of the bundle item matcher service.</summary>
    public BundleItemMatcherService(IStorageScanner storageScanner, IBundleRepository bundleRepo, ILogger logger)
    {
        _storageScanner = storageScanner;
        _bundleRepo = bundleRepo;
        _logger = logger;
    }

    /// <summary>Get all owned items that match requirements for a specific bundle.</summary>
    /// <param name="bundleName">The bundle to check items for.</param>
    /// <returns>List of owned items that satisfy unfilled bundle slots.</returns>
    public IReadOnlyList<OwnedItemInfo> GetOwnedBundleItems(string bundleName)
    {
        var bundle = _bundleRepo.GetAllBundles()
            .FirstOrDefault(b => b.BundleName.Equals(bundleName, StringComparison.OrdinalIgnoreCase));

        if (bundle == null)
        {
            _logger.Log(LogLevel.Debug, $"Bundle '{bundleName}' not found for item matching");
            return Array.Empty<OwnedItemInfo>();
        }

        var matchedItems = new List<OwnedItemInfo>();
        var remainingSlots = bundle.GetRemainingSlots();

        foreach (var slot in remainingSlots)
        {
            var matchingItem = FindMatchingOwnedItem(slot);
            if (matchingItem != null)
            {
                matchedItems.Add(matchingItem);
            }
        }

        _logger.Log(LogLevel.Debug, $"Found {matchedItems.Count} owned items for bundle '{bundleName}'");
        return matchedItems.AsReadOnly();
    }

    /// <summary>Get count of items ready to deliver for a bundle.</summary>
    /// <param name="bundleName">The bundle to check.</param>
    /// <returns>Tuple of (ready count, total needed).</returns>
    public (int Ready, int Total) GetReadyItemCount(string bundleName)
    {
        var bundle = _bundleRepo.GetAllBundles()
            .FirstOrDefault(b => b.BundleName.Equals(bundleName, StringComparison.OrdinalIgnoreCase));

        if (bundle == null)
            return (0, 0);

        var remainingSlots = bundle.GetRemainingSlots().ToList();
        var totalNeeded = remainingSlots.Count;
        var readyCount = 0;

        foreach (var slot in remainingSlots)
        {
            if (HasItemForSlot(slot))
            {
                readyCount++;
            }
        }

        return (readyCount, totalNeeded);
    }

    /// <summary>Check if the player has an item that satisfies a bundle slot.</summary>
    /// <param name="slot">The slot to check.</param>
    /// <returns>True if player has a matching item.</returns>
    public bool HasItemForSlot(BundleSlot slot)
    {
        return FindMatchingOwnedItem(slot) != null;
    }

    /// <summary>Find an owned item that matches a slot's requirements.</summary>
    private OwnedItemInfo? FindMatchingOwnedItem(BundleSlot slot)
    {
        foreach (var validItem in slot.ValidItems)
        {
            var matches = _storageScanner.FindItem(validItem.ItemId, validItem.MinimumQuality);
            if (matches.Count > 0)
            {
                return matches[0]; // Return first matching item
            }
        }
        return null;
    }

    /// <summary>Get match information for all slots in a bundle.</summary>
    /// <param name="bundleName">The bundle to check.</param>
    /// <returns>List of slot match info with ownership status.</returns>
    public IReadOnlyList<SlotMatchInfo> GetSlotMatchInfo(string bundleName)
    {
        var bundle = _bundleRepo.GetAllBundles()
            .FirstOrDefault(b => b.BundleName.Equals(bundleName, StringComparison.OrdinalIgnoreCase));

        if (bundle == null)
            return Array.Empty<SlotMatchInfo>();

        var result = new List<SlotMatchInfo>();

        foreach (var slot in bundle.Slots)
        {
            var matchingItem = slot.IsFilled ? null : FindMatchingOwnedItem(slot);
            result.Add(new SlotMatchInfo
            {
                Slot = slot,
                IsFilled = slot.IsFilled,
                HasOwnedItem = matchingItem != null,
                OwnedItem = matchingItem
            });
        }

        return result.AsReadOnly();
    }
}

/// <summary>Information about a bundle slot and whether the player owns a matching item.</summary>
public sealed class SlotMatchInfo
{
    /// <summary>The bundle slot.</summary>
    public BundleSlot Slot { get; init; } = null!;

    /// <summary>Whether this slot is already filled.</summary>
    public bool IsFilled { get; init; }

    /// <summary>Whether the player owns an item that can fill this slot.</summary>
    public bool HasOwnedItem { get; init; }

    /// <summary>The owned item that matches this slot, if any.</summary>
    public OwnedItemInfo? OwnedItem { get; init; }
}
