using StardewValley;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IRecipeRepository"/> by reading recipe data from game content.
/// Parses <c>Data\CookingRecipes</c> and <c>Data\CraftingRecipes</c>.
/// </summary>
public sealed class RecipeDataAdapter : IRecipeRepository
{
    /// <inheritdoc/>
    public IReadOnlyList<RecipeInfo> GetAllCookingRecipes()
    {
        var recipes = new List<RecipeInfo>();
        var recipeData = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");

        foreach (var kvp in recipeData)
        {
            var recipe = ParseRecipe(kvp.Key, kvp.Value, isCooking: true);
            if (recipe != null)
                recipes.Add(recipe);
        }

        return recipes.AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<RecipeInfo> GetAllCraftingRecipes()
    {
        var recipes = new List<RecipeInfo>();
        var recipeData = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");

        foreach (var kvp in recipeData)
        {
            var recipe = ParseRecipe(kvp.Key, kvp.Value, isCooking: false);
            if (recipe != null)
                recipes.Add(recipe);
        }

        return recipes.AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<RecipeIngredient> GetRecipeIngredients(string recipeId)
    {
        // Try cooking first, then crafting
        var cooking = GetAllCookingRecipes().FirstOrDefault(r => r.RecipeId == recipeId);
        if (cooking != null)
            return cooking.Ingredients;

        var crafting = GetAllCraftingRecipes().FirstOrDefault(r => r.RecipeId == recipeId);
        return crafting?.Ingredients ?? Array.Empty<RecipeIngredient>();
    }

    private static RecipeInfo? ParseRecipe(string recipeName, string recipeData, bool isCooking)
    {
        var fields = recipeData.Split('/');
        if (fields.Length < 1)
            return null;

        // Ingredient format: "itemId qty itemId qty ..."
        var ingredientStr = fields[0].Trim();
        var ingredients = new List<RecipeIngredient>();

        if (!string.IsNullOrEmpty(ingredientStr))
        {
            var parts = ingredientStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i + 1 < parts.Length; i += 2)
            {
                if (int.TryParse(parts[i], out var itemId) && int.TryParse(parts[i + 1], out var qty))
                {
                    var itemName = Game1.objectData != null &&
                                   Game1.objectData.TryGetValue(itemId.ToString(), out var info)
                        ? info.DisplayName
                        : $"Item {itemId}";

                    ingredients.Add(new RecipeIngredient
                    {
                        ItemId = itemId.ToString(),
                        ItemName = itemName,
                        Quantity = qty
                    });
                }
            }
        }

        // Display name: for cooking, field index varies; use recipe name as fallback
        var displayName = recipeName;
        if (isCooking && fields.Length > 4)
            displayName = fields[4].Trim();
        else if (!isCooking && fields.Length > 4)
            displayName = fields[4].Trim();

        if (string.IsNullOrEmpty(displayName))
            displayName = recipeName;

        return new RecipeInfo
        {
            RecipeId = recipeName,
            Name = displayName,
            IsCookingRecipe = isCooking,
            Ingredients = ingredients.AsReadOnly()
        };
    }
}
