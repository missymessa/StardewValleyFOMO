using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Generates "Last Chance" alerts during the final days of a season
/// for uncollected season-exclusive items.
/// </summary>
public sealed class SeasonAlertService
{
    private readonly IGameStateProvider _gameStateProvider;
    private readonly IFishRepository _fishRepository;
    private readonly IForageRepository _forageRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ILogger _logger;
    private readonly int _alertDays;

    /// <summary>Initializes a new instance of <see cref="SeasonAlertService"/>.</summary>
    public SeasonAlertService(
        IGameStateProvider gameStateProvider,
        IFishRepository fishRepository,
        IForageRepository forageRepository,
        ICollectionRepository collectionRepository,
        ILogger logger,
        int alertDays = 3)
    {
        _gameStateProvider = gameStateProvider;
        _fishRepository = fishRepository;
        _forageRepository = forageRepository;
        _collectionRepository = collectionRepository;
        _logger = logger;
        _alertDays = alertDays;
    }

    /// <summary>
    /// Get last-chance alerts for season-exclusive items.
    /// Only returns results during the final N days of the season.
    /// </summary>
    public IReadOnlyList<CollectibleItem> GetLastChanceAlerts()
    {
        var today = _gameStateProvider.GetCurrentDate();

        if (!today.IsWithinLastDays(_alertDays))
        {
            _logger.Log(Interfaces.LogLevel.Trace,
                $"Not within last {_alertDays} days of {today.Season} (Day {today.Day}) — no alerts");
            return Array.Empty<CollectibleItem>();
        }

        _logger.Log(Interfaces.LogLevel.Info,
            $"Last-chance alert window active: {today.Season} Day {today.Day} (final {_alertDays} days)",
            new { today.Season, today.Day, AlertDays = _alertDays });

        var alerts = new List<CollectibleItem>();

        // Season-exclusive uncaught fish
        var exclusiveFish = _fishRepository.GetSeasonExclusiveFish(today.Season);
        foreach (var fish in exclusiveFish)
        {
            if (!_collectionRepository.HasCaughtFish(fish.Id))
            {
                alerts.Add(fish);
            }
        }

        // Season-exclusive uncollected forageables
        var exclusiveForage = _forageRepository.GetSeasonExclusiveForageables(today.Season);
        foreach (var forage in exclusiveForage)
        {
            if (!_collectionRepository.HasShippedItem(forage.Id))
            {
                alerts.Add(forage);
            }
        }

        _logger.Log(Interfaces.LogLevel.Warn,
            $"Last-chance alerts: {alerts.Count} uncollected season-exclusive item(s) for {today.Season}",
            new { Count = alerts.Count, today.Season });

        return alerts.AsReadOnly();
    }
}
