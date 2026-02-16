using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="CookingProgressService"/>.</summary>
public sealed class CookingProgressServiceTests
{
    private readonly FakeCollectionRepository _collectionRepo;
    private readonly FakeRecipeRepository _recipeRepo;
    private readonly TestLogger _logger;
    private readonly CookingProgressService _service;

    public CookingProgressServiceTests()
    {
        _collectionRepo = new FakeCollectionRepository();
        _recipeRepo = new FakeRecipeRepository();
        _logger = new TestLogger();
        _service = new CookingProgressService(_collectionRepo, _recipeRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoRecipesCooked_ReturnsZeroCount()
    {
        // Add recipes to the repository but don't mark them as cooked
        _recipeRepo.AddCookingRecipe("Fried Egg");
        _recipeRepo.AddCookingRecipe("Omelet");

        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_SomeRecipesCooked_ReturnsCorrectCount()
    {
        // Add recipes to the repository
        _recipeRepo.AddCookingRecipe("Fried Egg");
        _recipeRepo.AddCookingRecipe("Omelet");
        _recipeRepo.AddCookingRecipe("Pancakes");

        // Mark some as cooked
        _collectionRepo.AddCookedRecipe("Fried Egg");
        _collectionRepo.AddCookedRecipe("Omelet");

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
    public void GetProgress_CategoryName_ReturnsCooking()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Cooking", progress.CategoryName);
    }
}
