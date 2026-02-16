using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

public class SeasonAlertServiceTests
{
    private readonly FakeGameStateProvider _gameState = new();
    private readonly FakeFishRepository _fishRepo = new();
    private readonly FakeForageRepository _forageRepo = new();
    private readonly FakeCollectionRepository _collectionRepo = new();
    private readonly TestLogger _logger = new();

    private SeasonAlertService CreateService(int alertDays = 3) =>
        new(_gameState, _fishRepo, _forageRepo, _collectionRepo, _logger, alertDays);

    #region Alert Window

    [Fact]
    public void GetLastChanceAlerts_WithinAlertWindow_ReturnsUncollectedItems()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 26, Year = 1 };
        _gameState.CurrentWeather = Weather.Rainy;

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "catfish",
            Name = "Catfish",
            CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring },
            RequiredWeather = new[] { Weather.Rainy }
        });

        var result = CreateService().GetLastChanceAlerts();

        Assert.Single(result);
        Assert.Equal("catfish", result[0].Id);
    }

    [Fact]
    public void GetLastChanceAlerts_OutsideAlertWindow_ReturnsEmpty()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 15, Year = 1 };

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "catfish",
            Name = "Catfish",
            CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring },
            RequiredWeather = new[] { Weather.Rainy }
        });

        var result = CreateService().GetLastChanceAlerts();

        Assert.Empty(result);
    }

    #endregion

    #region All Collected

    [Fact]
    public void GetLastChanceAlerts_AllCollected_ReturnsEmpty()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 27, Year = 1 };

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "catfish",
            Name = "Catfish",
            CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring },
            RequiredWeather = new[] { Weather.Rainy }
        });
        _collectionRepo.AddCaughtFish("catfish");

        _forageRepo.AddForageable(new CollectibleItem
        {
            Id = "leek",
            Name = "Leek",
            CollectionType = CollectionType.Forage,
            AvailableSeasons = new[] { Season.Spring },
            Locations = new[] { "Forest" }
        });
        _collectionRepo.AddShippedItem("leek");

        var result = CreateService().GetLastChanceAlerts();

        Assert.Empty(result);
    }

    #endregion

    #region Weather Notes

    [Fact]
    public void GetLastChanceAlerts_WeatherDependentFish_IncludesWeatherNote()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 28, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "catfish",
            Name = "Catfish",
            CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring },
            RequiredWeather = new[] { Weather.Rainy }
        });

        var result = CreateService().GetLastChanceAlerts();

        Assert.Single(result);
        Assert.True(result[0].IsWeatherDependent);
    }

    #endregion

    #region Multi-Season Fish Excluded

    [Fact]
    public void GetLastChanceAlerts_MultiSeasonFish_NotIncluded()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 27, Year = 1 };

        // Available in Spring AND Summer — not season-exclusive
        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "sunfish",
            Name = "Sunfish",
            CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring, Season.Summer }
        });

        var result = CreateService().GetLastChanceAlerts();

        Assert.Empty(result); // Sunfish is available next season too
    }

    #endregion

    #region Forageables

    [Fact]
    public void GetLastChanceAlerts_IncludesUncollectedForageables()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 26, Year = 1 };

        _forageRepo.AddForageable(new CollectibleItem
        {
            Id = "leek",
            Name = "Leek",
            CollectionType = CollectionType.Forage,
            AvailableSeasons = new[] { Season.Spring },
            Locations = new[] { "Forest" }
        });

        var result = CreateService().GetLastChanceAlerts();

        Assert.Single(result);
        Assert.Equal("leek", result[0].Id);
    }

    #endregion
}
