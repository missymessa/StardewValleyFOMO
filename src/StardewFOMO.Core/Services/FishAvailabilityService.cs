using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Determines which fish are available today based on season, weather, and time.
/// Annotates each fish with its collection status.
/// </summary>
public sealed class FishAvailabilityService
{
    private readonly IFishRepository _fishRepository;
    private readonly IGameStateProvider _gameStateProvider;
    private readonly ICollectionRepository _collectionRepository;
    private readonly IInventoryProvider _inventoryProvider;
    private readonly ILogger _logger;

    /// <summary>Initializes a new instance of <see cref="FishAvailabilityService"/>.</summary>
    public FishAvailabilityService(
        IFishRepository fishRepository,
        IGameStateProvider gameStateProvider,
        ICollectionRepository collectionRepository,
        IInventoryProvider inventoryProvider,
        ILogger logger)
    {
        _fishRepository = fishRepository;
        _gameStateProvider = gameStateProvider;
        _collectionRepository = collectionRepository;
        _inventoryProvider = inventoryProvider;
        _logger = logger;
    }

    /// <summary>
    /// Get all fish available right now, with collection status set.
    /// </summary>
    public IReadOnlyList<CollectibleItem> GetAvailableFish()
    {
        var date = _gameStateProvider.GetCurrentDate();
        var weather = _gameStateProvider.GetCurrentWeather();
        var time = _gameStateProvider.GetTimeOfDay();

        _logger.Log(Interfaces.LogLevel.Debug,
            $"Getting available fish for {date}, weather={weather}, time={time}",
            new { date.Season, date.Day, date.Year, weather, time });

        var availableFish = _fishRepository.GetAvailableFish(date.Season, weather, time);

        var result = new List<CollectibleItem>(availableFish.Count);
        foreach (var fish in availableFish)
        {
            var annotated = CloneWithStatus(fish);
            result.Add(annotated);
        }

        _logger.Log(Interfaces.LogLevel.Info,
            $"Found {result.Count} available fish for {date}",
            new { Count = result.Count, date.Season });

        return result.AsReadOnly();
    }

    private CollectibleItem CloneWithStatus(CollectibleItem fish)
    {
        var status = DetermineCollectionStatus(fish.Id);
        return new CollectibleItem
        {
            Id = fish.Id,
            Name = fish.Name,
            CollectionType = fish.CollectionType,
            AvailableSeasons = fish.AvailableSeasons,
            RequiredWeather = fish.RequiredWeather,
            Locations = fish.Locations,
            StartTime = fish.StartTime,
            EndTime = fish.EndTime,
            CollectionStatus = status,
            BundleNames = fish.BundleNames.ToList()
        };
    }

    private CollectionStatus DetermineCollectionStatus(string fishId)
    {
        if (_inventoryProvider.HasItemInInventoryOrStorage(fishId))
            return CollectionStatus.InInventory;

        if (_collectionRepository.HasCaughtFish(fishId))
            return CollectionStatus.EverCollected;

        return CollectionStatus.NotCollected;
    }
}
