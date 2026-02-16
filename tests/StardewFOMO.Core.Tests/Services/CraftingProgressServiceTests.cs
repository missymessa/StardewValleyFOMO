using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="CraftingProgressService"/>.</summary>
public sealed class CraftingProgressServiceTests
{
    private readonly FakeCollectionRepository _collectionRepo;
    private readonly FakeRecipeRepository _recipeRepo;
    private readonly TestLogger _logger;
    private readonly CraftingProgressService _service;

    public CraftingProgressServiceTests()
    {
        _collectionRepo = new FakeCollectionRepository();
        _recipeRepo = new FakeRecipeRepository();
        _logger = new TestLogger();
        _service = new CraftingProgressService(_collectionRepo, _recipeRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoRecipesCrafted_ReturnsZeroCount()
    {
        // Add recipes to the repository but don't mark them as crafted
        _recipeRepo.AddCraftingRecipe("Chest");
        _recipeRepo.AddCraftingRecipe("Furnace");

        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_SomeRecipesCrafted_ReturnsCorrectCount()
    {
        // Add recipes to the repository
        _recipeRepo.AddCraftingRecipe("Chest");
        _recipeRepo.AddCraftingRecipe("Furnace");
        _recipeRepo.AddCraftingRecipe("Scarecrow");

        // Mark some as crafted
        _collectionRepo.AddCraftedRecipe("Chest");
        _collectionRepo.AddCraftedRecipe("Furnace");

        var progress = _service.GetProgress();

        Assert.Equal(2, progress.CurrentCount);
    }

    [Fact]
    public void GetProgress_CategoryWeight_Returns10Percent()
    {
        var progress = _service.GetProgress();

        Assert.Equal(10.0, progress.Weight);
    }

    [Fact]
    public void GetProgress_CategoryName_ReturnsCrafting()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Crafting", progress.CategoryName);
    }
}
