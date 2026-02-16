using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

public class TomorrowPreviewServiceTests
{
    private readonly FakeGameStateProvider _gameState = new();
    private readonly FakeFishRepository _fishRepo = new();
    private readonly FakeForageRepository _forageRepo = new();
    private readonly FakeCollectionRepository _collectionRepo = new();
    private readonly TestLogger _logger = new();

    private TomorrowPreviewService CreateService() =>
        new(_gameState, _fishRepo, _forageRepo, _collectionRepo, _logger);

    #region Weather Forecast

    [Fact]
    public void GetTomorrowPreview_IncludesWeatherForecast()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 5, Year = 1 };
        _gameState.TomorrowWeather = Weather.Rainy;

        var result = CreateService().GetTomorrowPreview();

        Assert.Equal(Weather.Rainy, result.WeatherForecast);
    }

    #endregion

    #region Rain-Exclusive Fish

    [Fact]
    public void GetTomorrowPreview_RainTomorrow_HighlightsRainExclusiveFish()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 5, Year = 1 };
        _gameState.CurrentWeather = Weather.Sunny;
        _gameState.TomorrowWeather = Weather.Rainy;

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "catfish",
            Name = "Catfish",
            CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring },
            RequiredWeather = new[] { Weather.Rainy }
        });

        var result = CreateService().GetTomorrowPreview();

        Assert.Contains(result.NewCollectibles, f => f.Id == "catfish");
    }

    [Fact]
    public void GetTomorrowPreview_SunnyTomorrow_ExcludesRainOnlyFish()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 5, Year = 1 };
        _gameState.CurrentWeather = Weather.Rainy;
        _gameState.TomorrowWeather = Weather.Sunny;

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "catfish",
            Name = "Catfish",
            CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring },
            RequiredWeather = new[] { Weather.Rainy }
        });

        var result = CreateService().GetTomorrowPreview();

        // Catfish won't be newly available tomorrow if it's sunny
        Assert.DoesNotContain(result.NewCollectibles, f => f.Id == "catfish");
    }

    #endregion

    #region Season-End Warning

    [Fact]
    public void GetTomorrowPreview_LastDayOfSeason_IncludesSeasonChangeWarning()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 28, Year = 1 };
        _gameState.TomorrowWeather = Weather.Sunny;

        _fishRepo.AddFish(new CollectibleItem
        {
            Id = "catfish",
            Name = "Catfish",
            CollectionType = CollectionType.Fish,
            AvailableSeasons = new[] { Season.Spring },
            RequiredWeather = new[] { Weather.Rainy }
        });

        var result = CreateService().GetTomorrowPreview();

        Assert.NotNull(result.SeasonChangeWarning);
        Assert.Contains("Spring", result.SeasonChangeWarning!);
    }

    [Fact]
    public void GetTomorrowPreview_NotLastDay_NoSeasonChangeWarning()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 15, Year = 1 };
        _gameState.TomorrowWeather = Weather.Sunny;

        var result = CreateService().GetTomorrowPreview();

        Assert.Null(result.SeasonChangeWarning);
    }

    #endregion

    #region Festival Day

    [Fact]
    public void GetTomorrowPreview_FestivalTomorrow_IncludesFestivalEvent()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 12, Year = 1 };
        _gameState.TomorrowWeather = Weather.Sunny;
        _gameState.TomorrowFestivalDay = true;
        _gameState.TomorrowFestivalName_ = "Egg Festival";

        var result = CreateService().GetTomorrowPreview();

        Assert.Contains(result.Events, e => e.Contains("Egg Festival"));
    }

    #endregion
}
