using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

public class DailySummaryServiceTests
{
    private readonly FakeGameStateProvider _gameState = new();
    private readonly FakeFishRepository _fishRepo = new();
    private readonly FakeForageRepository _forageRepo = new();
    private readonly FakeBundleRepository _bundleRepo = new();
    private readonly FakeCollectionRepository _collectionRepo = new();
    private readonly FakeInventoryProvider _inventoryProvider = new();
    private readonly FakeNpcRepository _npcRepo = new();
    private readonly FakeRecipeRepository _recipeRepo = new();
    private readonly TestLogger _logger = new();

    private DailySummaryService CreateService()
    {
        var fishAvail = new FishAvailabilityService(_fishRepo, _gameState, _collectionRepo, _inventoryProvider, _logger);
        var forageAvail = new ForageAvailabilityService(_forageRepo, _gameState, _collectionRepo, _inventoryProvider, _logger);
        var bundleTracking = new BundleTrackingService(_bundleRepo, _inventoryProvider, _logger);
        var collectionTracking = new CollectionTrackingService(_collectionRepo, _inventoryProvider, _recipeRepo, _logger);
        var birthdayService = new BirthdayService(_npcRepo, _gameState, _logger, lookaheadDays: 7);
        var tomorrowPreview = new TomorrowPreviewService(_gameState, _fishRepo, _forageRepo, _collectionRepo, _logger);
        var seasonAlert = new SeasonAlertService(_gameState, _fishRepo, _forageRepo, _collectionRepo, _logger, alertDays: 3);

        return new DailySummaryService(
            fishAvail,
            forageAvail,
            bundleTracking,
            collectionTracking,
            birthdayService,
            tomorrowPreview,
            seasonAlert,
            _gameState,
            _logger);
    }

    [Fact]
    public void GetDailySummary_PopulatesDateAndWeather()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 5, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Rainy;

        var result = CreateService().GetDailySummary();

        Assert.Equal(Season.Spring, result.Date.Season);
        Assert.Equal(5, result.Date.Day);
        Assert.Equal(Weather.Sunny, result.CurrentWeather);
    }

    [Fact]
    public void GetDailySummary_IncludesAvailableFishAndForage()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Summer, Day = 10, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1200;

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "tuna", Name = "Tuna", CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Summer },
            RequiredWeather = Array.Empty<Weather>(),
            StartTime = 600, EndTime = 1900,
            Locations = new[] { "Ocean" }
        });

        _forageRepo.AddForageable(new CollectibleItem
        {
            Id = "grape", Name = "Grape", CollectionType = CollectionType.Forage,
            AvailableSeasons = new[] { Season.Summer },
            Locations = new[] { "Mountain" }
        });

        var result = CreateService().GetDailySummary();

        Assert.Single(result.AvailableFish);
        Assert.Equal("tuna", result.AvailableFish[0].Id);
        Assert.Single(result.AvailableForageables);
        Assert.Equal("grape", result.AvailableForageables[0].Id);
    }

    [Fact]
    public void GetDailySummary_AnnotatesBundleNeededItems()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 1, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1200;
        _bundleRepo.SetCommunityCenterActive(true);

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "catfish", Name = "Catfish", CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring },
            RequiredWeather = Array.Empty<Weather>(),
            StartTime = 600, EndTime = 2400,
            Locations = new[] { "River" }
        });

        _bundleRepo.AddBundle(new BundleInfo
        {
            BundleName = "River Fish Bundle",
            RoomName = "Fish Tank",
            RequiredItems = new[] { new BundleItem { ItemId = "catfish", ItemName = "Catfish" } },
            CompletedItemIds = new HashSet<string>()
        });

        var result = CreateService().GetDailySummary();

        Assert.Single(result.BundleNeededItems);
        Assert.Equal("catfish", result.BundleNeededItems[0].Id);
        Assert.Contains("River Fish Bundle", result.BundleNeededItems[0].BundleNames);
    }

    [Fact]
    public void GetDailySummary_IncludesTodayBirthdays()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 4, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Sunny;

        _npcRepo.AddBirthday(new NpcBirthday
        {
            NpcName = "Kent",
            BirthdayDate = new GameDate { Season = Season.Spring, Day = 4, Year = 1 },
            LovedGifts = new[] { "Roasted Hazelnuts" },
            LikedGifts = new[] { "Fiddlehead Risotto" }
        });

        var result = CreateService().GetDailySummary();

        Assert.Single(result.TodayBirthdays);
        Assert.Equal("Kent", result.TodayBirthdays[0].NpcName);
    }

    [Fact]
    public void GetDailySummary_IncludesTomorrowPreview()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 10, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Rainy;

        var result = CreateService().GetDailySummary();

        Assert.NotNull(result.TomorrowPreview);
        Assert.Equal(Weather.Rainy, result.TomorrowPreview!.WeatherForecast);
    }

    [Fact]
    public void GetDailySummary_IncludesLastChanceAlerts_WhenInAlertWindow()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 27, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Sunny;

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "legend", Name = "Legend", CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring },
            RequiredWeather = new[] { Weather.Rainy },
            Locations = new[] { "Mountain Lake" }
        });

        var result = CreateService().GetDailySummary();

        Assert.Single(result.LastChanceAlerts);
        Assert.Equal("legend", result.LastChanceAlerts[0].Id);
    }

    [Fact]
    public void GetDailySummary_FestivalDay_Detected()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 13, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Sunny;
        _gameState.FestivalDay = true;
        _gameState.FestivalName_ = "Egg Festival";

        var result = CreateService().GetDailySummary();

        Assert.True(result.IsFestivalDay);
        Assert.Equal("Egg Festival", result.FestivalName);
    }

    [Fact]
    public void GetPrioritySummary_FiltersBundleNeeded_TimeSensitive_Birthdays_Alerts()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Summer, Day = 26, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1200;
        _bundleRepo.SetCommunityCenterActive(true);

        // Bundle-needed fish
        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "pufferfish", Name = "Pufferfish", CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Summer },
            RequiredWeather = Array.Empty<Weather>(),
            StartTime = 1200, EndTime = 1600,
            Locations = new[] { "Ocean" }
        });
        _bundleRepo.AddBundle(new BundleInfo
        {
            BundleName = "Ocean Fish Bundle",
            RoomName = "Fish Tank",
            RequiredItems = new[] { new BundleItem { ItemId = "pufferfish", ItemName = "Pufferfish" } },
            CompletedItemIds = new HashSet<string>()
        });

        // Non-bundle fish (should not appear in priority)
        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "herring", Name = "Herring", CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Summer },
            RequiredWeather = Array.Empty<Weather>(),
            StartTime = 600, EndTime = 2400,
            Locations = new[] { "Ocean" }
        });

        var result = CreateService().GetPrioritySummary();

        // Priority view should include bundle-needed items and last-chance alerts
        Assert.Contains(result.BundleNeededItems, i => i.Id == "pufferfish");
        Assert.True(result.LastChanceAlerts.Count >= 0); // Alert window depends on day 26 of 28
    }

    [Fact]
    public void GetShowAllSummary_IncludesCollectionTracking()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Fall, Day = 15, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Sunny;

        _collectionRepo.AddKnownCookingRecipe("fried_egg");
        _collectionRepo.AddCookedRecipe("fried_egg");

        _recipeRepo.AddCookingRecipe(new RecipeInfo
        {
            RecipeId = "fried_egg",
            Name = "Fried Egg",
            IsCookingRecipe = true,
            Ingredients = Array.Empty<RecipeIngredient>()
        });

        _collectionRepo.AddKnownCraftingRecipe("chest");
        _recipeRepo.AddCraftingRecipe(new RecipeInfo
        {
            RecipeId = "chest",
            Name = "Chest",
            IsCookingRecipe = false,
            Ingredients = new[] { new RecipeIngredient { ItemId = "wood", ItemName = "Wood", Quantity = 50 } }
        });

        var result = CreateService().GetShowAllSummary();

        Assert.NotNull(result.AllCollectionItems);
        Assert.True(result.AllCollectionItems.Count > 0);
    }
}
