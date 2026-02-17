using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="PerfectionCalculatorService"/>.</summary>
public sealed class PerfectionCalculatorServiceTests
{
    private readonly FakePerfectionRepository _perfectionRepo;
    private readonly FakeCollectionRepository _collectionRepo;
    private readonly FakeShippingRepository _shippingRepo;
    private readonly FakeFishRepository _fishRepo;
    private readonly FakeRecipeRepository _recipeRepo;
    private readonly FakeFriendshipRepository _friendshipRepo;
    private readonly FakeBuildingRepository _buildingRepo;
    private readonly FakeMonsterRepository _monsterRepo;
    private readonly FakeStardropRepository _stardropRepo;
    private readonly FakeWalnutRepository _walnutRepo;
    private readonly FakeSkillRepository _skillRepo;
    private readonly TestLogger _logger;
    private readonly PerfectionCalculatorService _service;

    public PerfectionCalculatorServiceTests()
    {
        _perfectionRepo = new FakePerfectionRepository();
        _collectionRepo = new FakeCollectionRepository();
        _shippingRepo = new FakeShippingRepository();
        _fishRepo = new FakeFishRepository();
        _recipeRepo = new FakeRecipeRepository();
        _friendshipRepo = new FakeFriendshipRepository();
        _buildingRepo = new FakeBuildingRepository();
        _monsterRepo = new FakeMonsterRepository();
        _stardropRepo = new FakeStardropRepository();
        _walnutRepo = new FakeWalnutRepository();
        _skillRepo = new FakeSkillRepository();
        _logger = new TestLogger();

        // Create individual progress services
        var shippingService = new ShippingProgressService(_collectionRepo, _shippingRepo, _logger);
        var fishService = new FishProgressService(_collectionRepo, _fishRepo, _logger);
        var cookingService = new CookingProgressService(_collectionRepo, _recipeRepo, _logger);
        var craftingService = new CraftingProgressService(_collectionRepo, _recipeRepo, _logger);
        var friendshipService = new FriendshipProgressService(_friendshipRepo, _logger);
        var buildingService = new BuildingProgressService(_buildingRepo, _logger);
        var monsterService = new MonsterProgressService(_monsterRepo, _logger);
        var stardropService = new StardropProgressService(_stardropRepo, _logger);
        var walnutService = new WalnutProgressService(_walnutRepo, _perfectionRepo, _logger);
        var skillService = new SkillProgressService(_skillRepo, _logger);

        _service = new PerfectionCalculatorService(
            shippingService,
            fishService,
            cookingService,
            craftingService,
            friendshipService,
            buildingService,
            monsterService,
            stardropService,
            walnutService,
            skillService,
            _perfectionRepo,
            _logger);
    }

    [Fact]
    public void GetProgress_NoProgress_ReturnsZeroPercent()
    {
        // Set up empty repositories (but with items to track)
        SetupEmptyProgress();

        var progress = _service.GetProgress();

        Assert.Equal(0, progress.TotalPercentage);
    }

    [Fact]
    public void GetProgress_AllComplete_ReturnsHighPercentage()
    {
        // Set all categories that have dynamic counts to complete
        SetupCompleteProgress();

        var progress = _service.GetProgress();

        // The test sets up complete data for fixed-count categories (buildings, monsters, stardrops, skills, walnuts)
        // and partial completion for collection-based categories. Should be above 50%.
        Assert.True(progress.TotalPercentage >= 50.0, $"Expected at least 50%, got {progress.TotalPercentage}");
    }

    [Fact]
    public void GetProgress_ReturnsAllCategories()
    {
        var progress = _service.GetProgress();

        Assert.Equal(10, progress.Categories.Count);
    }

    [Fact]
    public void GetProgress_GingerIslandNotUnlocked_ReturnsUnlockedFalse()
    {
        _perfectionRepo.GingerIslandUnlocked = false;

        var progress = _service.GetProgress();

        Assert.False(progress.GingerIslandUnlocked);
    }

    [Fact]
    public void GetProgress_GingerIslandUnlocked_ReturnsUnlockedTrue()
    {
        _perfectionRepo.GingerIslandUnlocked = true;

        var progress = _service.GetProgress();

        Assert.True(progress.GingerIslandUnlocked);
    }

    [Fact]
    public void GetProgress_CategoryWeightsTotal100()
    {
        var progress = _service.GetProgress();

        var totalWeight = progress.Categories.Sum(c => c.Weight);
        Assert.Equal(100.0, totalWeight, 0.01);
    }

    private void SetupEmptyProgress()
    {
        // Add items to repositories but don't mark them complete
        _fishRepo.AddFish("Sardine");
        _fishRepo.AddFish("Anchovy");
        _recipeRepo.AddCookingRecipe("Fried Egg");
        _recipeRepo.AddCraftingRecipe("Chest");
        _friendshipRepo.SetFriendship("Alex", 0, 10);
        _buildingRepo.SetBuildingBuilt("Gold Clock", false);
        _monsterRepo.SetGoalComplete("Slimes", false);
        _stardropRepo.SetStardropCollected("Fair", false);
        _skillRepo.SetSkillLevel("Farming", 0);
    }

    private void SetupCompleteProgress()
    {
        // Set all repositories to complete state
        _buildingRepo.SetAllBuilt();
        _monsterRepo.SetAllGoalsComplete();
        _stardropRepo.SetAllCollected();
        _skillRepo.SetAllSkillsMaxed();
        _perfectionRepo.GoldenWalnutsCollected = 130;

        // For items that count from repos, mark as complete
        _fishRepo.AddFish("Sardine");
        _collectionRepo.AddCaughtFish("Sardine");
        _recipeRepo.AddCookingRecipe("Fried Egg");
        _collectionRepo.AddCookedRecipe("Fried Egg");
        _recipeRepo.AddCraftingRecipe("Chest");
        _collectionRepo.AddCraftedRecipe("Chest");
        _friendshipRepo.SetFriendship("Alex", 10, 10);
    }
}
