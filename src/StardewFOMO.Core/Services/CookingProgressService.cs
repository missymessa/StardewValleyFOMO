using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking cooking recipe progress toward perfection.
/// </summary>
public sealed class CookingProgressService
{
    private readonly ICollectionRepository _collectionRepo;
    private readonly IRecipeRepository _recipeRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (10%)
    private const double PerfectionWeight = 10.0;

    // Total cooking recipes for perfection (vanilla count)
    private const int TotalCookingRecipes = 80;

    /// <summary>
    /// Initializes a new instance of the <see cref="CookingProgressService"/> class.
    /// </summary>
    public CookingProgressService(ICollectionRepository collectionRepo, IRecipeRepository recipeRepo, ILogger logger)
    {
        _collectionRepo = collectionRepo;
        _recipeRepo = recipeRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the cooking progress category for perfection tracking.
    /// </summary>
    public PerfectionCategory GetProgress()
    {
        var cookedCount = CountCookedRecipes();

        _logger.Log(LogLevel.Trace, $"Cooking progress: {cookedCount}/{TotalCookingRecipes}");

        return new PerfectionCategory
        {
            CategoryName = "Cooking",
            CurrentCount = cookedCount,
            TotalCount = TotalCookingRecipes,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets recipes that have not yet been cooked.
    /// </summary>
    public IReadOnlyList<PerfectionItem> GetUncookedRecipes()
    {
        var allRecipes = _recipeRepo.GetAllCookingRecipes();
        var uncooked = new List<PerfectionItem>();

        foreach (var recipe in allRecipes)
        {
            if (!_collectionRepo.HasCookedRecipe(recipe.RecipeId))
            {
                var isKnown = _collectionRepo.KnowsCookingRecipe(recipe.RecipeId);
                uncooked.Add(new PerfectionItem
                {
                    ItemId = recipe.RecipeId,
                    DisplayName = recipe.Name,
                    IsComplete = false,
                    AcquisitionHint = isKnown ? "Known - ready to cook" : "Recipe not yet learned"
                });
            }
        }

        return uncooked.AsReadOnly();
    }

    private int CountCookedRecipes()
    {
        var allRecipes = _recipeRepo.GetAllCookingRecipes();
        return allRecipes.Count(r => _collectionRepo.HasCookedRecipe(r.RecipeId));
    }
}
