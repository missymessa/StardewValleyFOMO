namespace StardewFOMO.Core.Models;

/// <summary>Represents the collection/ownership status of an item.</summary>
public enum CollectionStatus
{
    /// <summary>The item has never been collected/obtained.</summary>
    NotCollected,
    /// <summary>The item is currently in the player's inventory or storage.</summary>
    InInventory,
    /// <summary>The item has been collected/completed at some point (collection tab).</summary>
    EverCollected
}
