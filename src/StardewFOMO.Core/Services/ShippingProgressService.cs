using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking shipping collection progress toward perfection.
/// </summary>
public sealed class ShippingProgressService
{
    private readonly ICollectionRepository _collectionRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (15%)
    private const double PerfectionWeight = 15.0;

    // Total shippable items for perfection (vanilla count)
    private const int TotalShippableItems = 145;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShippingProgressService"/> class.
    /// </summary>
    /// <param name="collectionRepo">Repository for collection data.</param>
    /// <param name="logger">Logger instance.</param>
    public ShippingProgressService(ICollectionRepository collectionRepo, ILogger logger)
    {
        _collectionRepo = collectionRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the shipping progress category for perfection tracking.
    /// </summary>
    /// <returns>Shipping progress category with completion stats.</returns>
    public PerfectionCategory GetProgress()
    {
        var shippedIds = _collectionRepo.GetAllShippedItemIds();
        var shippedCount = shippedIds.Count;

        _logger.Log(LogLevel.Trace, $"Shipping progress: {shippedCount}/{TotalShippableItems}");

        return new PerfectionCategory
        {
            CategoryName = "Shipping",
            CurrentCount = shippedCount,
            TotalCount = TotalShippableItems,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets items that have not yet been shipped.
    /// </summary>
    /// <returns>List of unshipped items.</returns>
    public IReadOnlyList<ShippingItem> GetUnshippedItems()
    {
        var shippedIds = _collectionRepo.GetAllShippedItemIds();
        var unshipped = new List<ShippingItem>();

        // Note: In actual implementation, we would enumerate all shippable items
        // and filter out the shipped ones. For now, return empty list.
        // This would require access to game item data through an interface.

        _logger.Log(LogLevel.Trace, $"Found {unshipped.Count} unshipped items");

        return unshipped.AsReadOnly();
    }
}
