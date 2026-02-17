using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking crafting recipe progress toward perfection.
/// </summary>
public sealed class CraftingProgressService
{
    private readonly ICollectionRepository _collectionRepo;
    private readonly IRecipeRepository _recipeRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (10%)
    private const double PerfectionWeight = 10.0;

    // Total crafting recipes for perfection (vanilla count)
    private const int TotalCraftingRecipes = 129;

    /// <summary>
    /// Initializes a new instance of the <see cref="CraftingProgressService"/> class.
    /// </summary>
    public CraftingProgressService(ICollectionRepository collectionRepo, IRecipeRepository recipeRepo, ILogger logger)
    {
        _collectionRepo = collectionRepo;
        _recipeRepo = recipeRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the crafting progress category for perfection tracking.
    /// </summary>
    public PerfectionCategory GetProgress()
    {
        var craftedCount = CountCraftedRecipes();

        _logger.Log(LogLevel.Trace, $"Crafting progress: {craftedCount}/{TotalCraftingRecipes}");

        return new PerfectionCategory
        {
            CategoryName = "Crafting",
            CurrentCount = craftedCount,
            TotalCount = TotalCraftingRecipes,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets recipes that have not yet been crafted.
    /// </summary>
    public IReadOnlyList<PerfectionItem> GetUncraftedRecipes()
    {
        var allRecipes = _recipeRepo.GetAllCraftingRecipes();
        var uncrafted = new List<PerfectionItem>();

        foreach (var recipe in allRecipes)
        {
            if (!_collectionRepo.HasCraftedRecipe(recipe.RecipeId))
            {
                var isKnown = _collectionRepo.KnowsCraftingRecipe(recipe.RecipeId);
                uncrafted.Add(new PerfectionItem
                {
                    ItemId = recipe.RecipeId,
                    DisplayName = recipe.Name,
                    IsComplete = false,
                    AcquisitionHint = isKnown ? "Known - ready to craft" : "Recipe not yet learned"
                });
            }
        }

        return uncrafted.AsReadOnly();
    }

    private int CountCraftedRecipes()
    {
        var allRecipes = _recipeRepo.GetAllCraftingRecipes();
        return allRecipes.Count(r => _collectionRepo.HasCraftedRecipe(r.RecipeId));
    }
}
