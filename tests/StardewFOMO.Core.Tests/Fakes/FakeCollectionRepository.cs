using StardewFOMO.Core.Interfaces;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="ICollectionRepository"/> with in-memory collection data.</summary>
public sealed class FakeCollectionRepository : ICollectionRepository
{
    private readonly HashSet<string> _caughtFish = new();
    private readonly HashSet<string> _shippedItems = new();
    private readonly HashSet<string> _donatedItems = new();
    private readonly HashSet<string> _cookedRecipes = new();
    private readonly HashSet<string> _craftedRecipes = new();
    private readonly HashSet<string> _knownCookingRecipes = new();
    private readonly HashSet<string> _knownCraftingRecipes = new();

    public void AddCaughtFish(string fishId) => _caughtFish.Add(fishId);
    public void AddShippedItem(string itemId) => _shippedItems.Add(itemId);
    public void AddDonatedItem(string itemId) => _donatedItems.Add(itemId);
    public void AddCookedRecipe(string recipeId) => _cookedRecipes.Add(recipeId);
    public void AddCraftedRecipe(string recipeId) => _craftedRecipes.Add(recipeId);
    public void AddKnownCookingRecipe(string recipeId) => _knownCookingRecipes.Add(recipeId);
    public void AddKnownCraftingRecipe(string recipeId) => _knownCraftingRecipes.Add(recipeId);

    public bool HasCaughtFish(string fishId) => _caughtFish.Contains(fishId);
    public bool HasShippedItem(string itemId) => _shippedItems.Contains(itemId);
    public bool HasDonatedToMuseum(string itemId) => _donatedItems.Contains(itemId);
    public bool HasCookedRecipe(string recipeId) => _cookedRecipes.Contains(recipeId);
    public bool HasCraftedRecipe(string recipeId) => _craftedRecipes.Contains(recipeId);
    public bool KnowsCookingRecipe(string recipeId) => _knownCookingRecipes.Contains(recipeId);
    public bool KnowsCraftingRecipe(string recipeId) => _knownCraftingRecipes.Contains(recipeId);
    public IReadOnlySet<string> GetAllShippedItemIds() => _shippedItems;
    public IReadOnlySet<string> GetAllDonatedItemIds() => _donatedItems;
}
