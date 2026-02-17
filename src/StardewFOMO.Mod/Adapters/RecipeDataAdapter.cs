using StardewValley;
using StardewValley.TokenizableStrings;
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

        // Get output item ID to determine display name
        // Crafting: field[1] has output item ID (possibly with quantity like "456 2")
        // Cooking: field[2] has output item ID
        string? outputItemId = null;
        if (!isCooking && fields.Length > 1)
        {
            var outputParts = fields[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (outputParts.Length > 0)
                outputItemId = outputParts[0];
        }
        else if (isCooking && fields.Length > 2)
        {
            var outputParts = fields[2].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (outputParts.Length > 0)
                outputItemId = outputParts[0];
        }

        // Look up display name from item data, fall back to recipe name
        string displayName = recipeName;
        if (!string.IsNullOrEmpty(outputItemId))
        {
            // Check regular objects
            if (Game1.objectData?.TryGetValue(outputItemId, out var objInfo) == true && 
                !string.IsNullOrEmpty(objInfo.DisplayName))
            {
                displayName = TokenParser.ParseText(objInfo.DisplayName) ?? recipeName;
            }
            // Check big craftables
            else if (Game1.bigCraftableData?.TryGetValue(outputItemId, out var bigInfo) == true && 
                     !string.IsNullOrEmpty(bigInfo.DisplayName))
            {
                displayName = TokenParser.ParseText(bigInfo.DisplayName) ?? recipeName;
            }
        }

        return new RecipeInfo
        {
            RecipeId = recipeName,
            Name = displayName,
            IsCookingRecipe = isCooking,
            Ingredients = ingredients.AsReadOnly()
        };
    }
}
