using StardewValley;
using StardewFOMO.Core.Interfaces;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="ICollectionRepository"/> by reading player collection tabs.
/// </summary>
public sealed class CollectionAdapter : ICollectionRepository
{
    /// <inheritdoc/>
    public bool HasCaughtFish(string fishId)
    {
        // SDV 1.6 uses qualified IDs like "(O)128" in fishCaught
        // But Data\Fish uses unqualified IDs like "128"
        if (Game1.player.fishCaught.ContainsKey(fishId))
            return true;
        
        // Try with qualified object ID prefix
        var qualifiedId = fishId.StartsWith("(O)") ? fishId : $"(O){fishId}";
        return Game1.player.fishCaught.ContainsKey(qualifiedId);
    }

    /// <inheritdoc/>
    public bool HasShippedItem(string itemId)
    {
        // Same pattern for shipping - try both qualified and unqualified
        if (Game1.player.basicShipped.ContainsKey(itemId))
            return true;
        
        var qualifiedId = itemId.StartsWith("(O)") ? itemId : $"(O){itemId}";
        return Game1.player.basicShipped.ContainsKey(qualifiedId);
    }

    /// <inheritdoc/>
    public bool HasDonatedToMuseum(string itemId)
    {
        var museum = Game1.getLocationFromName("ArchaeologyHouse") as StardewValley.Locations.LibraryMuseum;
        if (museum?.museumPieces == null)
            return false;
        return museum.museumPieces.Values.Contains(itemId);
    }

    /// <inheritdoc/>
    public bool HasCookedRecipe(string recipeId)
    {
        return Game1.player.recipesCooked.ContainsKey(recipeId);
    }

    /// <inheritdoc/>
    public bool HasCraftedRecipe(string recipeId)
    {
        return Game1.player.craftingRecipes.ContainsKey(recipeId)
               && Game1.player.craftingRecipes[recipeId] > 0;
    }

    /// <inheritdoc/>
    public bool KnowsCookingRecipe(string recipeId)
    {
        return Game1.player.cookingRecipes.ContainsKey(recipeId);
    }

    /// <inheritdoc/>
    public bool KnowsCraftingRecipe(string recipeId)
    {
        return Game1.player.craftingRecipes.ContainsKey(recipeId);
    }

    /// <inheritdoc/>
    public IReadOnlySet<string> GetAllShippedItemIds()
    {
        var result = new HashSet<string>();
        foreach (var id in Game1.player.basicShipped.Keys)
            result.Add(id.ToString());
        return result;
    }

    /// <inheritdoc/>
    public IReadOnlySet<string> GetAllDonatedItemIds()
    {
        var result = new HashSet<string>();
        var museum = Game1.getLocationFromName("ArchaeologyHouse") as StardewValley.Locations.LibraryMuseum;
        if (museum?.museumPieces != null)
        {
            foreach (var id in museum.museumPieces.Values)
                result.Add(id.ToString());
        }
        return result;
    }
}
