using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IRecipeRepository"/> with in-memory recipe data.</summary>
public sealed class FakeRecipeRepository : IRecipeRepository
{
    private readonly List<RecipeInfo> _cookingRecipes = new();
    private readonly List<RecipeInfo> _craftingRecipes = new();

    public void AddCookingRecipe(RecipeInfo recipe) => _cookingRecipes.Add(recipe);
    public void AddCraftingRecipe(RecipeInfo recipe) => _craftingRecipes.Add(recipe);

    /// <summary>Add a simple cooking recipe with just an ID for testing.</summary>
    public void AddCookingRecipe(string recipeId) => _cookingRecipes.Add(new RecipeInfo
    {
        RecipeId = recipeId,
        Name = recipeId,
        IsCookingRecipe = true
    });

    /// <summary>Add a simple crafting recipe with just an ID for testing.</summary>
    public void AddCraftingRecipe(string recipeId) => _craftingRecipes.Add(new RecipeInfo
    {
        RecipeId = recipeId,
        Name = recipeId,
        IsCookingRecipe = false
    });

    public IReadOnlyList<RecipeInfo> GetAllCookingRecipes() => _cookingRecipes.AsReadOnly();
    public IReadOnlyList<RecipeInfo> GetAllCraftingRecipes() => _craftingRecipes.AsReadOnly();

    public IReadOnlyList<RecipeIngredient> GetRecipeIngredients(string recipeId)
    {
        var recipe = _cookingRecipes.FirstOrDefault(r => r.RecipeId == recipeId)
                     ?? _craftingRecipes.FirstOrDefault(r => r.RecipeId == recipeId);
        return recipe?.Ingredients ?? Array.Empty<RecipeIngredient>();
    }
}
