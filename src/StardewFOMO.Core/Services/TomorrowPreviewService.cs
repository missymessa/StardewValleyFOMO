using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Builds tomorrow's preview: weather forecast, newly available collectibles,
/// events/festivals, and season-change warnings.
/// </summary>
public sealed class TomorrowPreviewService
{
    private readonly IGameStateProvider _gameStateProvider;
    private readonly IFishRepository _fishRepository;
    private readonly IForageRepository _forageRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ILogger _logger;

    /// <summary>Initializes a new instance of <see cref="TomorrowPreviewService"/>.</summary>
    public TomorrowPreviewService(
        IGameStateProvider gameStateProvider,
        IFishRepository fishRepository,
        IForageRepository forageRepository,
        ICollectionRepository collectionRepository,
        ILogger logger)
    {
        _gameStateProvider = gameStateProvider;
        _fishRepository = fishRepository;
        _forageRepository = forageRepository;
        _collectionRepository = collectionRepository;
        _logger = logger;
    }

    /// <summary>Build tomorrow's preview.</summary>
    public TomorrowPreview GetTomorrowPreview()
    {
        var today = _gameStateProvider.GetCurrentDate();
        var todayWeather = _gameStateProvider.GetCurrentWeather();
        var tomorrowWeather = _gameStateProvider.GetTomorrowWeather();
        var tomorrow = today.GetTomorrow();

        _logger.Log(Interfaces.LogLevel.Debug,
            $"Building tomorrow preview: {tomorrow}, weather={tomorrowWeather}",
            new { Tomorrow = tomorrow.ToString(), tomorrowWeather });

        var newCollectibles = GetNewlyAvailableCollectibles(today, todayWeather, tomorrow, tomorrowWeather);
        var events = GetTomorrowEvents();
        var (seasonWarning, expiringItems) = GetSeasonChangeInfo(today);

        return new TomorrowPreview
        {
            WeatherForecast = tomorrowWeather,
            NewCollectibles = newCollectibles,
            Events = events,
            SeasonChangeWarning = seasonWarning,
            ExpiringItems = expiringItems
        };
    }

    private IReadOnlyList<CollectibleItem> GetNewlyAvailableCollectibles(
        GameDate today, Weather todayWeather, GameDate tomorrow, Weather tomorrowWeather)
    {
        var newItems = new List<CollectibleItem>();

        // Fish that become available due to weather change
        var todayFish = _fishRepository.GetFishBySeasonAndWeather(today.Season, todayWeather);
        var tomorrowFish = _fishRepository.GetFishBySeasonAndWeather(tomorrow.Season, tomorrowWeather);

        var todayFishIds = todayFish.Select(f => f.Id).ToHashSet();
        foreach (var fish in tomorrowFish)
        {
            if (!todayFishIds.Contains(fish.Id))
                newItems.Add(fish);
        }

        // Forageables that become available due to season change
        if (today.IsTomorrowNewSeason)
        {
            var tomorrowForageables = _forageRepository.GetForageablesBySeason(tomorrow.Season);
            var todayForageables = _forageRepository.GetForageablesBySeason(today.Season);
            var todayForageIds = todayForageables.Select(f => f.Id).ToHashSet();

            foreach (var forage in tomorrowForageables)
            {
                if (!todayForageIds.Contains(forage.Id))
                    newItems.Add(forage);
            }
        }

        return newItems.AsReadOnly();
    }

    private IReadOnlyList<string> GetTomorrowEvents()
    {
        var events = new List<string>();

        // Add festival if tomorrow is a festival day
        if (_gameStateProvider.IsTomorrowFestivalDay())
        {
            var festivalName = _gameStateProvider.GetTomorrowFestivalName();
            if (festivalName != null)
                events.Add($"🎉 Festival: {festivalName}");
        }

        // Add other events from the game state provider
        var otherEvents = _gameStateProvider.GetTomorrowEvents();
        events.AddRange(otherEvents);

        return events.AsReadOnly();
    }

    private (string? Warning, IReadOnlyList<CollectibleItem> ExpiringItems) GetSeasonChangeInfo(GameDate today)
    {
        if (!today.IsTomorrowNewSeason)
            return (null, Array.Empty<CollectibleItem>());

        var expiringFish = _fishRepository.GetSeasonExclusiveFish(today.Season)
            .Where(f => !_collectionRepository.HasCaughtFish(f.Id))
            .ToList();

        var expiringForage = _forageRepository.GetSeasonExclusiveForageables(today.Season)
            .Where(f => !_collectionRepository.HasShippedItem(f.Id))
            .ToList();

        var allExpiring = expiringFish.Concat(expiringForage).ToList();

        var warning = allExpiring.Count > 0
            ? $"{today.Season} is ending! {allExpiring.Count} uncollected season-exclusive item(s) will become unavailable."
            : $"{today.Season} is ending tomorrow. All season-exclusive items have been collected!";

        _logger.Log(Interfaces.LogLevel.Warn,
            $"Season change warning: {warning}",
            new { today.Season, ExpiringCount = allExpiring.Count });

        return (warning, allExpiring.AsReadOnly());
    }
}
