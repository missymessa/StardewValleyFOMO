using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Determines which forageable items are available today based on season.
/// Annotates each item with its collection status and supports grouping by location.
/// </summary>
public sealed class ForageAvailabilityService
{
    private readonly IForageRepository _forageRepository;
    private readonly IGameStateProvider _gameStateProvider;
    private readonly ICollectionRepository _collectionRepository;
    private readonly IInventoryProvider _inventoryProvider;
    private readonly ILogger _logger;

    /// <summary>Initializes a new instance of <see cref="ForageAvailabilityService"/>.</summary>
    public ForageAvailabilityService(
        IForageRepository forageRepository,
        IGameStateProvider gameStateProvider,
        ICollectionRepository collectionRepository,
        IInventoryProvider inventoryProvider,
        ILogger logger)
    {
        _forageRepository = forageRepository;
        _gameStateProvider = gameStateProvider;
        _collectionRepository = collectionRepository;
        _inventoryProvider = inventoryProvider;
        _logger = logger;
    }

    /// <summary>
    /// Get all forageable items available today, with collection status set.
    /// </summary>
    public IReadOnlyList<CollectibleItem> GetAvailableForageables()
    {
        var date = _gameStateProvider.GetCurrentDate();

        _logger.Log(Interfaces.LogLevel.Debug,
            $"Getting available forageables for {date}",
            new { date.Season, date.Day, date.Year });

        var available = _forageRepository.GetForageablesBySeason(date.Season);

        var result = new List<CollectibleItem>(available.Count);
        foreach (var item in available)
        {
            result.Add(CloneWithStatus(item));
        }

        _logger.Log(Interfaces.LogLevel.Info,
            $"Found {result.Count} available forageables for {date}",
            new { Count = result.Count, date.Season });

        return result.AsReadOnly();
    }

    /// <summary>
    /// Get available forageables grouped by location.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<CollectibleItem>> GetAvailableForageablesGroupedByLocation()
    {
        var allAvailable = GetAvailableForageables();

        var grouped = new Dictionary<string, List<CollectibleItem>>();
        foreach (var item in allAvailable)
        {
            foreach (var location in item.Locations)
            {
                if (!grouped.ContainsKey(location))
                    grouped[location] = new List<CollectibleItem>();
                grouped[location].Add(item);
            }
        }

        return grouped.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<CollectibleItem>)kvp.Value.AsReadOnly());
    }

    private CollectibleItem CloneWithStatus(CollectibleItem item)
    {
        var status = DetermineCollectionStatus(item.Id);
        return new CollectibleItem
        {
            Id = item.Id,
            Name = item.Name,
            CollectionType = item.CollectionType,
            AvailableSeasons = item.AvailableSeasons,
            RequiredWeather = item.RequiredWeather,
            Locations = item.Locations,
            StartTime = item.StartTime,
            EndTime = item.EndTime,
            CollectionStatus = status,
            BundleNames = item.BundleNames.ToList()
        };
    }

    private CollectionStatus DetermineCollectionStatus(string itemId)
    {
        if (_inventoryProvider.HasItemInInventoryOrStorage(itemId))
            return CollectionStatus.InInventory;

        if (_collectionRepository.HasShippedItem(itemId))
            return CollectionStatus.EverCollected;

        return CollectionStatus.NotCollected;
    }
}
