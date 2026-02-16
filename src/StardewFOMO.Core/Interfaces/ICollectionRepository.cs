namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts player collection tab data (fish, shipping, museum, cooking, crafting).
/// Implemented by SMAPI adapter reading from player's collection tabs.
/// </summary>
public interface ICollectionRepository
{
    /// <summary>Check whether the player has ever caught a specific fish.</summary>
    bool HasCaughtFish(string fishId);

    /// <summary>Check whether the player has ever shipped a specific item.</summary>
    bool HasShippedItem(string itemId);

    /// <summary>Check whether the player has donated a specific item to the museum.</summary>
    bool HasDonatedToMuseum(string itemId);

    /// <summary>Check whether the player has ever cooked a specific recipe.</summary>
    bool HasCookedRecipe(string recipeId);

    /// <summary>Check whether the player has ever crafted a specific recipe.</summary>
    bool HasCraftedRecipe(string recipeId);

    /// <summary>Check whether the player knows (has learned) a specific cooking recipe.</summary>
    bool KnowsCookingRecipe(string recipeId);

    /// <summary>Check whether the player knows (has learned) a specific crafting recipe.</summary>
    bool KnowsCraftingRecipe(string recipeId);

    /// <summary>Get all item IDs that have been shipped.</summary>
    IReadOnlySet<string> GetAllShippedItemIds();

    /// <summary>Get all item IDs that have been donated to the museum.</summary>
    IReadOnlySet<string> GetAllDonatedItemIds();
}
