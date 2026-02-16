using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IStorageScanner"/> with in-memory item data.</summary>
public sealed class FakeStorageScanner : IStorageScanner
{
    private readonly List<OwnedItemInfo> _items = new();
    private bool _cacheInvalidated;

    /// <summary>Add an item to the fake storage.</summary>
    public void AddItem(OwnedItemInfo item) => _items.Add(item);

    /// <summary>Add an item to the fake storage with simplified parameters.</summary>
    public void AddItem(string itemId, string itemName, int quantity = 1, int quality = 0, 
        ItemLocation location = ItemLocation.Inventory, string? buildingName = null)
    {
        _items.Add(new OwnedItemInfo
        {
            ItemId = itemId,
            ItemName = itemName,
            Quantity = quantity,
            Quality = quality,
            Location = location,
            BuildingName = buildingName
        });
    }

    /// <summary>Clear all items.</summary>
    public void Clear() => _items.Clear();

    /// <summary>Check if cache was invalidated (for testing).</summary>
    public bool WasCacheInvalidated => _cacheInvalidated;

    /// <summary>Reset the invalidation flag.</summary>
    public void ResetInvalidationFlag() => _cacheInvalidated = false;

    /// <inheritdoc/>
    public IReadOnlyList<OwnedItemInfo> GetOwnedItems() => _items.AsReadOnly();

    /// <inheritdoc/>
    public void InvalidateCache() => _cacheInvalidated = true;

    /// <inheritdoc/>
    public bool HasItem(string itemId, int minQuality = 0) =>
        _items.Any(item => item.ItemId == itemId && item.Quality >= minQuality);

    /// <inheritdoc/>
    public IReadOnlyList<OwnedItemInfo> FindItem(string itemId, int minQuality = 0) =>
        _items
            .Where(item => item.ItemId == itemId && item.Quality >= minQuality)
            .ToList()
            .AsReadOnly();

    /// <inheritdoc/>
    public int GetTotalQuantity(string itemId, int minQuality = 0) =>
        _items
            .Where(item => item.ItemId == itemId && item.Quality >= minQuality)
            .Sum(item => item.Quantity);
}
