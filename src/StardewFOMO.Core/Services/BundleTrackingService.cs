using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Maps available collectible items to incomplete Community Center bundles.
/// Annotates items with bundle names when needed.
/// </summary>
public sealed class BundleTrackingService
{
    private readonly IBundleRepository _bundleRepository;
    private readonly IInventoryProvider _inventoryProvider;
    private readonly ILogger _logger;

    /// <summary>Initializes a new instance of <see cref="BundleTrackingService"/>.</summary>
    public BundleTrackingService(
        IBundleRepository bundleRepository,
        IInventoryProvider inventoryProvider,
        ILogger logger)
    {
        _bundleRepository = bundleRepository;
        _inventoryProvider = inventoryProvider;
        _logger = logger;
    }

    /// <summary>
    /// Annotate a list of collectible items with bundle information.
    /// Each item's <see cref="CollectibleItem.BundleNames"/> is populated with
    /// names of incomplete bundles that still need that item.
    /// Returns a new list with cloned, annotated items.
    /// </summary>
    public IReadOnlyList<CollectibleItem> AnnotateWithBundleInfo(IReadOnlyList<CollectibleItem> items)
    {
        if (!_bundleRepository.IsCommunityCenterActive())
        {
            _logger.Log(Interfaces.LogLevel.Info, "Joja route active — skipping bundle annotations");
            // Return items as-is, no bundle names
            return items.Select(CloneItem).ToList().AsReadOnly();
        }

        var result = new List<CollectibleItem>(items.Count);
        foreach (var item in items)
        {
            var bundleNames = _bundleRepository.GetBundleNamesNeedingItem(item.Id);
            var annotated = CloneItem(item);
            foreach (var name in bundleNames)
            {
                annotated.BundleNames.Add(name);
            }
            result.Add(annotated);
        }

        var bundleNeededCount = result.Count(i => i.IsNeededForBundle);
        _logger.Log(Interfaces.LogLevel.Info,
            $"Annotated {items.Count} items: {bundleNeededCount} needed for bundles",
            new { TotalItems = items.Count, BundleNeeded = bundleNeededCount });

        return result.AsReadOnly();
    }

    private static CollectibleItem CloneItem(CollectibleItem item) =>
        new()
        {
            Id = item.Id,
            Name = item.Name,
            CollectionType = item.CollectionType,
            AvailableSeasons = item.AvailableSeasons,
            RequiredWeather = item.RequiredWeather,
            Locations = item.Locations,
            StartTime = item.StartTime,
            EndTime = item.EndTime,
            CollectionStatus = item.CollectionStatus,
            BundleNames = new List<string>()
        };
}
