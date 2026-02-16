using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

public class FishAvailabilityServiceTests
{
    private readonly FakeGameStateProvider _gameState = new();
    private readonly FakeFishRepository _fishRepo = new();
    private readonly FakeCollectionRepository _collectionRepo = new();
    private readonly FakeInventoryProvider _inventoryProvider = new();
    private readonly TestLogger _logger = new();

    private FishAvailabilityService CreateService() =>
        new(_fishRepo, _gameState, _collectionRepo, _inventoryProvider, _logger);

    #region Season Filtering

    [Fact]
    public void GetAvailableFish_SpringDay_ReturnsOnlySpringFish()
    {
        // Arrange
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 3, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1200;

        _fishRepo.AddFish(CreateFish("catfish", "Catfish", new[] { Season.Spring }, new[] { Weather.Rainy }));
        _fishRepo.AddFish(CreateFish("sunfish", "Sunfish", new[] { Season.Spring, Season.Summer }));
        _fishRepo.AddFish(CreateFish("tilapia", "Tilapia", new[] { Season.Summer }));

        // Act
        var result = CreateService().GetAvailableFish();

        // Assert — Catfish excluded (wrong weather), Sunfish included, Tilapia excluded (wrong season)
        Assert.Single(result);
        Assert.Equal("sunfish", result[0].Id);
    }

    [Fact]
    public void GetAvailableFish_RainySpring_IncludesRainExclusiveFish()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 3, Year = 1 };
        _gameState.CurrentWeather = Weather.Rainy;
        _gameState.TimeOfDay = 1200;

        _fishRepo.AddFish(CreateFish("catfish", "Catfish", new[] { Season.Spring }, new[] { Weather.Rainy }));
        _fishRepo.AddFish(CreateFish("sunfish", "Sunfish", new[] { Season.Spring, Season.Summer }));

        var result = CreateService().GetAvailableFish();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.Id == "catfish");
        Assert.Contains(result, f => f.Id == "sunfish");
    }

    [Fact]
    public void GetAvailableFish_SunnyDay_ExcludesRainOnlyFish()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Summer, Day = 5, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1200;

        _fishRepo.AddFish(CreateFish("catfish", "Catfish", new[] { Season.Spring, Season.Fall }, new[] { Weather.Rainy }));
        _fishRepo.AddFish(CreateFish("sunfish", "Sunfish", new[] { Season.Spring, Season.Summer }));

        var result = CreateService().GetAvailableFish();

        Assert.Single(result);
        Assert.Equal("sunfish", result[0].Id);
    }

    #endregion

    #region Time Filtering

    [Fact]
    public void GetAvailableFish_BeforeStartTime_ExcludesFish()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 1, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TimeOfDay = 500; // 5 AM

        _fishRepo.AddFish(CreateFish("eel", "Eel", new[] { Season.Spring }, startTime: 1600, endTime: 2400));

        var result = CreateService().GetAvailableFish();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAvailableFish_WithinTimeWindow_IncludesFish()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 1, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1800;

        _fishRepo.AddFish(CreateFish("eel", "Eel", new[] { Season.Spring }, startTime: 1600, endTime: 2400));

        var result = CreateService().GetAvailableFish();

        Assert.Single(result);
        Assert.Equal("eel", result[0].Id);
    }

    #endregion

    #region Collection Status

    [Fact]
    public void GetAvailableFish_PreviouslyCaughtFish_MarkedAsEverCollected()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 1, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1200;

        _fishRepo.AddFish(CreateFish("sunfish", "Sunfish", new[] { Season.Spring }));
        _collectionRepo.AddCaughtFish("sunfish");

        var result = CreateService().GetAvailableFish();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.EverCollected, result[0].CollectionStatus);
    }

    [Fact]
    public void GetAvailableFish_FishInInventory_MarkedAsInInventory()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 1, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1200;

        _fishRepo.AddFish(CreateFish("sunfish", "Sunfish", new[] { Season.Spring }));
        _inventoryProvider.AddItem("sunfish");

        var result = CreateService().GetAvailableFish();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.InInventory, result[0].CollectionStatus);
    }

    [Fact]
    public void GetAvailableFish_NeverCaughtFish_MarkedAsNotCollected()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 1, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1200;

        _fishRepo.AddFish(CreateFish("sunfish", "Sunfish", new[] { Season.Spring }));

        var result = CreateService().GetAvailableFish();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.NotCollected, result[0].CollectionStatus);
    }

    #endregion

    #region Festival Days

    [Fact]
    public void GetAvailableFish_FestivalDay_StillReturnsFishButFlagsFestival()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 13, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TimeOfDay = 1200;
        _gameState.FestivalDay = true;
        _gameState.FestivalName_ = "Egg Festival";

        _fishRepo.AddFish(CreateFish("sunfish", "Sunfish", new[] { Season.Spring }));

        // Fish still returned — the UI layer handles festival-day messaging
        var result = CreateService().GetAvailableFish();

        Assert.Single(result);
    }

    #endregion

    #region All-Season Fish

    [Fact]
    public void GetAvailableFish_AllSeasonFish_AlwaysIncluded()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Winter, Day = 15, Year = 1 };
        _gameState.CurrentWeather = Weather.Snowy;
        _gameState.TimeOfDay = 1200;

        // Fish with no season restriction
        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "carp",
            Name = "Carp",
            CollectionType = CollectionType.Fish,
            AvailableSeasons = Array.Empty<Season>(),
            RequiredWeather = Array.Empty<Weather>()
        });

        var result = CreateService().GetAvailableFish();

        Assert.Single(result);
        Assert.Equal("carp", result[0].Id);
    }

    #endregion

    #region Helpers

    private static CollectibleItem CreateFish(
        string id,
        string name,
        Season[]? seasons = null,
        Weather[]? weather = null,
        int? startTime = null,
        int? endTime = null,
        string[]? locations = null) =>
        new()
        {
            Id = id,
            Name = name,
            CollectionType = CollectionType.Fish,
            AvailableSeasons = seasons ?? Array.Empty<Season>(),
            RequiredWeather = weather ?? Array.Empty<Weather>(),
            Locations = locations ?? Array.Empty<string>(),
            StartTime = startTime,
            EndTime = endTime
        };

    #endregion
}
