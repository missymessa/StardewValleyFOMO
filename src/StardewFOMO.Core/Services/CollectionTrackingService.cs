using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Tracks collection completion status for shipping, museum, cooking, and crafting tabs.
/// Provides the "Show All" expanded view data beyond the priority fish/forage view.
/// </summary>
public sealed class CollectionTrackingService
{
    private readonly ICollectionRepository _collectionRepo;
    private readonly IInventoryProvider _inventoryProvider;
    private readonly IRecipeRepository _recipeRepo;
    private readonly ILogger _logger;

    /// <summary>Initializes a new instance of <see cref="CollectionTrackingService"/>.</summary>
    public CollectionTrackingService(
        ICollectionRepository collectionRepo,
        IInventoryProvider inventoryProvider,
        IRecipeRepository recipeRepo,
        ILogger logger)
    {
        _collectionRepo = collectionRepo;
        _inventoryProvider = inventoryProvider;
        _recipeRepo = recipeRepo;
        _logger = logger;
    }

    /// <summary>
    /// Returns copies of the provided items with shipping collection status applied.
    /// Items that have been shipped are marked <see cref="CollectionStatus.EverCollected"/>;
    /// items in inventory are marked <see cref="CollectionStatus.InInventory"/>;
    /// otherwise <see cref="CollectionStatus.NotCollected"/>.
    /// </summary>
    public IReadOnlyList<CollectibleItem> GetShippingCollectionStatus(IReadOnlyList<CollectibleItem> items)
    {
        _logger.Log(LogLevel.Debug, "Evaluating shipping status for {0} items", items.Count);

        var result = new List<CollectibleItem>(items.Count);

        foreach (var item in items)
        {
            var clone = CloneItem(item);

            if (_collectionRepo.HasShippedItem(item.Id))
                clone.CollectionStatus = CollectionStatus.EverCollected;
            else if (_inventoryProvider.HasItemInInventoryOrStorage(item.Id))
                clone.CollectionStatus = CollectionStatus.InInventory;
            else
                clone.CollectionStatus = CollectionStatus.NotCollected;

            result.Add(clone);
        }

        _logger.Log(LogLevel.Trace,
            $"Shipping status: {result.Count(i => i.CollectionStatus == CollectionStatus.EverCollected)} shipped, " +
            $"{result.Count(i => i.CollectionStatus == CollectionStatus.InInventory)} in inventory, " +
            $"{result.Count(i => i.CollectionStatus == CollectionStatus.NotCollected)} not collected");

        return result;
    }

    /// <summary>
    /// Returns copies of the provided items with museum donation status applied.
    /// </summary>
    public IReadOnlyList<CollectibleItem> GetMuseumDonationStatus(IReadOnlyList<CollectibleItem> items)
    {
        _logger.Log(LogLevel.Debug, "Evaluating museum donation status for {0} items", items.Count);

        var result = new List<CollectibleItem>(items.Count);

        foreach (var item in items)
        {
            var clone = CloneItem(item);

            if (_collectionRepo.HasDonatedToMuseum(item.Id))
                clone.CollectionStatus = CollectionStatus.EverCollected;
            else if (_inventoryProvider.HasItemInInventoryOrStorage(item.Id))
                clone.CollectionStatus = CollectionStatus.InInventory;
            else
                clone.CollectionStatus = CollectionStatus.NotCollected;

            result.Add(clone);
        }

        _logger.Log(LogLevel.Trace,
            $"Museum status: {result.Count(i => i.CollectionStatus == CollectionStatus.EverCollected)} donated, " +
            $"{result.Count(i => i.CollectionStatus == CollectionStatus.InInventory)} in inventory, " +
            $"{result.Count(i => i.CollectionStatus == CollectionStatus.NotCollected)} not collected");

        return result;
    }

    /// <summary>
    /// Returns cooking recipe collectible items with status based on whether the recipe
    /// has been cooked, the player knows it and has ingredients, or doesn't know it yet.
    /// </summary>
    public IReadOnlyList<CollectibleItem> GetCookingRecipeStatus()
    {
        var recipes = _recipeRepo.GetAllCookingRecipes();
        _logger.Log(LogLevel.Debug, "Evaluating cooking recipe status for {0} recipes", recipes.Count);

        var result = new List<CollectibleItem>(recipes.Count);

        foreach (var recipe in recipes)
        {
            if (!_collectionRepo.KnowsCookingRecipe(recipe.RecipeId))
                continue; // Player hasn't learned this recipe yet

            var item = new CollectibleItem
            {
                Id = recipe.RecipeId,
                Name = recipe.Name,
                CollectionType = CollectionType.CookingRecipe
            };

            if (_collectionRepo.HasCookedRecipe(recipe.RecipeId))
            {
                item.CollectionStatus = CollectionStatus.EverCollected;
            }
            else if (HasAllIngredients(recipe))
            {
                item.CollectionStatus = CollectionStatus.InInventory;
            }
            else
            {
                item.CollectionStatus = CollectionStatus.NotCollected;
            }

            result.Add(item);
        }

        _logger.Log(LogLevel.Trace,
            $"Cooking recipes: {result.Count(i => i.CollectionStatus == CollectionStatus.EverCollected)} cooked, " +
            $"{result.Count(i => i.CollectionStatus == CollectionStatus.InInventory)} craftable, " +
            $"{result.Count(i => i.CollectionStatus == CollectionStatus.NotCollected)} missing ingredients");

        return result;
    }

    /// <summary>
    /// Returns crafting recipe collectible items with status based on whether the recipe
    /// has been crafted, the player knows it and has materials, or doesn't know it yet.
    /// </summary>
    public IReadOnlyList<CollectibleItem> GetCraftingRecipeStatus()
    {
        var recipes = _recipeRepo.GetAllCraftingRecipes();
        _logger.Log(LogLevel.Debug, "Evaluating crafting recipe status for {0} recipes", recipes.Count);

        var result = new List<CollectibleItem>(recipes.Count);

        foreach (var recipe in recipes)
        {
            if (!_collectionRepo.KnowsCraftingRecipe(recipe.RecipeId))
                continue; // Player hasn't learned this recipe yet

            var item = new CollectibleItem
            {
                Id = recipe.RecipeId,
                Name = recipe.Name,
                CollectionType = CollectionType.CraftingRecipe
            };

            if (_collectionRepo.HasCraftedRecipe(recipe.RecipeId))
            {
                item.CollectionStatus = CollectionStatus.EverCollected;
            }
            else if (HasAllIngredients(recipe))
            {
                item.CollectionStatus = CollectionStatus.InInventory;
            }
            else
            {
                item.CollectionStatus = CollectionStatus.NotCollected;
            }

            result.Add(item);
        }

        _logger.Log(LogLevel.Trace,
            $"Crafting recipes: {result.Count(i => i.CollectionStatus == CollectionStatus.EverCollected)} crafted, " +
            $"{result.Count(i => i.CollectionStatus == CollectionStatus.InInventory)} craftable, " +
            $"{result.Count(i => i.CollectionStatus == CollectionStatus.NotCollected)} missing materials");

        return result;
    }

    private bool HasAllIngredients(RecipeInfo recipe)
    {
        foreach (var ingredient in recipe.Ingredients)
        {
            if (_inventoryProvider.GetTotalItemCount(ingredient.ItemId) < ingredient.Quantity)
                return false;
        }
        return true;
    }

    private static CollectibleItem CloneItem(CollectibleItem source) => new()
    {
        Id = source.Id,
        Name = source.Name,
        CollectionType = source.CollectionType,
        AvailableSeasons = source.AvailableSeasons,
        RequiredWeather = source.RequiredWeather,
        Locations = source.Locations,
        StartTime = source.StartTime,
        EndTime = source.EndTime,
        CollectionStatus = source.CollectionStatus,
        BundleNames = new List<string>(source.BundleNames)
    };
}
