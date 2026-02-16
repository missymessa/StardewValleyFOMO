namespace StardewFOMO.Core.Models;

/// <summary>
/// Where an owned item is located.
/// </summary>
public enum ItemLocation
{
    /// <summary>In the player's inventory.</summary>
    Inventory,

    /// <summary>In the farmhouse.</summary>
    Farmhouse,

    /// <summary>In a farm building (shed, barn, coop, etc.).</summary>
    FarmBuilding
}

/// <summary>
/// Represents an item the player owns, including its location.
/// </summary>
public sealed class OwnedItemInfo
{
    /// <summary>The item identifier.</summary>
    public string ItemId { get; init; } = string.Empty;

    /// <summary>The display name of the item.</summary>
    public string ItemName { get; init; } = string.Empty;

    /// <summary>Quantity of this item at this location.</summary>
    public int Quantity { get; init; } = 1;

    /// <summary>Quality level (0 = normal, 1 = silver, 2 = gold, 3 = iridium).</summary>
    public int Quality { get; init; }

    /// <summary>Where this item is located.</summary>
    public ItemLocation Location { get; init; } = ItemLocation.Inventory;

    /// <summary>Name of the building if Location is FarmBuilding (e.g., "Shed", "Barn").</summary>
    public string? BuildingName { get; init; }

    /// <summary>Gets a display string for the location.</summary>
    public string LocationDisplay => Location switch
    {
        ItemLocation.Inventory => "Inventory",
        ItemLocation.Farmhouse => "Farmhouse",
        ItemLocation.FarmBuilding => BuildingName ?? "Farm Building",
        _ => "Unknown"
    };
}
