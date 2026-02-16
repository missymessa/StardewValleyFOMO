using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for filtering bundles by item availability today.
/// Considers season, weather, time of day, and location unlock status.
/// </summary>
public sealed class BundleAvailabilityService
{
    private readonly IBundleRepository _bundleRepo;
    private readonly IItemAvailabilityService _availabilityService;
    private readonly ILogger _logger;

    /// <summary>Creates a new instance of the bundle availability service.</summary>
    public BundleAvailabilityService(
        IBundleRepository bundleRepo, 
        IItemAvailabilityService availabilityService, 
        ILogger logger)
    {
        _bundleRepo = bundleRepo;
        _availabilityService = availabilityService;
        _logger = logger;
    }

    /// <summary>Get all bundles that have at least one obtainable item today.</summary>
    public IReadOnlyList<BundleInfo> GetAvailableBundles()
    {
        var incompleteBundles = _bundleRepo.GetIncompleteBundles();
        var availableBundles = new List<BundleInfo>();

        foreach (var bundle in incompleteBundles)
        {
            if (HasAnyAvailableItem(bundle))
            {
                availableBundles.Add(bundle);
            }
        }

        _logger.Log(LogLevel.Debug, $"Found {availableBundles.Count} bundles with items available today");
        return availableBundles.AsReadOnly();
    }

    /// <summary>Check if any item in a bundle is available today.</summary>
    public bool IsBundleAvailableToday(string bundleName)
    {
        var bundle = _bundleRepo.GetAllBundles()
            .FirstOrDefault(b => b.BundleName.Equals(bundleName, StringComparison.OrdinalIgnoreCase));

        return bundle != null && HasAnyAvailableItem(bundle);
    }

    /// <summary>Get availability status for all items in a bundle.</summary>
    public IReadOnlyList<ItemAvailability> GetItemAvailabilityForBundle(string bundleName)
    {
        var bundle = _bundleRepo.GetAllBundles()
            .FirstOrDefault(b => b.BundleName.Equals(bundleName, StringComparison.OrdinalIgnoreCase));

        if (bundle == null)
            return Array.Empty<ItemAvailability>();

        var result = new List<ItemAvailability>();
        var remainingSlots = bundle.GetRemainingSlots();

        foreach (var slot in remainingSlots)
        {
            // For OR requirements, check the primary item (or first valid)
            var primaryItem = slot.PrimaryItem;
            if (primaryItem != null)
            {
                result.Add(_availabilityService.GetAvailability(primaryItem.ItemId));
            }
        }

        return result.AsReadOnly();
    }

    /// <summary>Get bundles grouped by availability status.</summary>
    public BundleAvailabilitySummary GetAvailabilitySummary()
    {
        var incompleteBundles = _bundleRepo.GetIncompleteBundles();
        var available = new List<BundleInfo>();
        var unavailable = new List<BundleInfo>();

        foreach (var bundle in incompleteBundles)
        {
            if (HasAnyAvailableItem(bundle))
                available.Add(bundle);
            else
                unavailable.Add(bundle);
        }

        return new BundleAvailabilitySummary
        {
            AvailableBundles = available.AsReadOnly(),
            UnavailableBundles = unavailable.AsReadOnly()
        };
    }

    private bool HasAnyAvailableItem(BundleInfo bundle)
    {
        foreach (var slot in bundle.GetRemainingSlots())
        {
            foreach (var item in slot.ValidItems)
            {
                if (_availabilityService.IsAvailableToday(item.ItemId))
                    return true;
            }
        }
        return false;
    }
}

/// <summary>Summary of bundle availability.</summary>
public sealed class BundleAvailabilitySummary
{
    /// <summary>Bundles with at least one item available today.</summary>
    public IReadOnlyList<BundleInfo> AvailableBundles { get; init; } = Array.Empty<BundleInfo>();

    /// <summary>Bundles with no items available today.</summary>
    public IReadOnlyList<BundleInfo> UnavailableBundles { get; init; } = Array.Empty<BundleInfo>();
}
