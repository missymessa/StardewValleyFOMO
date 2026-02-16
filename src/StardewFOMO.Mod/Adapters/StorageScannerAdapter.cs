using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IStorageScanner"/> by scanning
/// player inventory, farmhouse, and farm building chests.
/// Results are cached and invalidated on day start or inventory changes.
/// </summary>
public sealed class StorageScannerAdapter : IStorageScanner
{
    private List<OwnedItemInfo>? _cachedItems;

    /// <inheritdoc/>
    public IReadOnlyList<OwnedItemInfo> GetOwnedItems()
    {
        if (_cachedItems != null)
            return _cachedItems.AsReadOnly();

        _cachedItems = new List<OwnedItemInfo>();
        
        // Scan player inventory
        ScanInventory();
        
        // Scan farmhouse
        ScanFarmhouse();
        
        // Scan farm buildings
        ScanFarmBuildings();

        return _cachedItems.AsReadOnly();
    }

    /// <inheritdoc/>
    public void InvalidateCache()
    {
        _cachedItems = null;
    }

    /// <inheritdoc/>
    public bool HasItem(string itemId, int minQuality = 0)
    {
        return GetOwnedItems().Any(item => 
            item.ItemId == itemId && item.Quality >= minQuality);
    }

    /// <inheritdoc/>
    public IReadOnlyList<OwnedItemInfo> FindItem(string itemId, int minQuality = 0)
    {
        return GetOwnedItems()
            .Where(item => item.ItemId == itemId && item.Quality >= minQuality)
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc/>
    public int GetTotalQuantity(string itemId, int minQuality = 0)
    {
        return GetOwnedItems()
            .Where(item => item.ItemId == itemId && item.Quality >= minQuality)
            .Sum(item => item.Quantity);
    }

    private void ScanInventory()
    {
        var player = Game1.player;
        if (player?.Items == null) return;

        foreach (var item in player.Items)
        {
            if (item is StardewValley.Object obj)
            {
                AddItem(obj, ItemLocation.Inventory, null);
            }
        }
    }

    private void ScanFarmhouse()
    {
        var farmhouse = Game1.getLocationFromName("FarmHouse");
        if (farmhouse == null) return;

        foreach (var obj in farmhouse.Objects.Values)
        {
            if (obj is Chest chest)
            {
                ScanChest(chest, ItemLocation.Farmhouse, null);
            }
        }
    }

    private void ScanFarmBuildings()
    {
        var farm = Game1.getFarm();
        if (farm == null) return;

        foreach (var building in farm.buildings)
        {
            var interior = building.indoors.Value;
            if (interior == null) continue;

            var buildingName = building.buildingType.Value ?? "Building";

            foreach (var obj in interior.Objects.Values)
            {
                if (obj is Chest chest)
                {
                    ScanChest(chest, ItemLocation.FarmBuilding, buildingName);
                }
            }
        }
    }

    private void ScanChest(Chest chest, ItemLocation location, string? buildingName)
    {
        foreach (var item in chest.Items)
        {
            if (item is StardewValley.Object obj)
            {
                AddItem(obj, location, buildingName);
            }
        }
    }

    private void AddItem(StardewValley.Object obj, ItemLocation location, string? buildingName)
    {
        if (_cachedItems == null) return;

        var itemId = obj.ItemId ?? obj.ParentSheetIndex.ToString();
        var itemName = obj.DisplayName ?? $"Item {itemId}";

        _cachedItems.Add(new OwnedItemInfo
        {
            ItemId = itemId,
            ItemName = itemName,
            Quantity = obj.Stack,
            Quality = obj.Quality,
            Location = location,
            BuildingName = buildingName
        });
    }
}
