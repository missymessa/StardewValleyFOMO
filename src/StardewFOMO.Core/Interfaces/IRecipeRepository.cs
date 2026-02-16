using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts cooking and crafting recipe data.
/// Implemented by SMAPI adapter reading recipe data from game content.
/// </summary>
public interface IRecipeRepository
{
    /// <summary>Get all cooking recipes known to the game.</summary>
    IReadOnlyList<RecipeInfo> GetAllCookingRecipes();

    /// <summary>Get all crafting recipes known to the game.</summary>
    IReadOnlyList<RecipeInfo> GetAllCraftingRecipes();

    /// <summary>Get the ingredient/material requirements for a specific recipe.</summary>
    IReadOnlyList<RecipeIngredient> GetRecipeIngredients(string recipeId);
}
