using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for calculating Community Center bundle and room progress.
/// </summary>
public sealed class BundleProgressService
{
    private readonly IBundleRepository _bundleRepo;
    private readonly ILogger _logger;

    /// <summary>Creates a new instance of the bundle progress service.</summary>
    public BundleProgressService(IBundleRepository bundleRepo, ILogger logger)
    {
        _bundleRepo = bundleRepo;
        _logger = logger;
    }

    /// <summary>Get overall Community Center completion progress.</summary>
    public RoomProgress GetOverallProgress()
    {
        var bundles = _bundleRepo.GetAllBundles();
        var completed = bundles.Count(b => b.IsComplete);

        _logger.Log(LogLevel.Debug, $"Overall CC progress: {completed}/{bundles.Count} bundles complete");

        return new RoomProgress
        {
            RoomName = "Community Center",
            TotalBundles = bundles.Count,
            CompletedBundles = completed
        };
    }

    /// <summary>Get progress for all rooms.</summary>
    public IReadOnlyList<RoomProgress> GetRoomProgressList()
    {
        var rooms = _bundleRepo.GetAllRooms();
        var result = new List<RoomProgress>();

        foreach (var roomName in rooms)
        {
            result.Add(_bundleRepo.GetRoomProgress(roomName));
        }

        return result.AsReadOnly();
    }

    /// <summary>Get bundle item counts for a specific room.</summary>
    public IReadOnlyList<BundleItemCount> GetBundleCountsForRoom(string roomName)
    {
        var bundles = _bundleRepo.GetBundlesByRoom(roomName);
        var result = new List<BundleItemCount>();

        foreach (var bundle in bundles)
        {
            var totalItems = bundle.SlotsRequired > 0 
                ? bundle.SlotsRequired 
                : bundle.RequiredItems.Count;
            var completedItems = bundle.SlotsFilled > 0 
                ? bundle.SlotsFilled 
                : bundle.CompletedItemIds.Count;

            result.Add(new BundleItemCount
            {
                BundleName = bundle.BundleName,
                RoomName = bundle.RoomName,
                TotalItems = totalItems,
                CompletedItems = completedItems,
                IsComplete = bundle.IsComplete
            });
        }

        return result.AsReadOnly();
    }

    /// <summary>Check if the entire Community Center is complete.</summary>
    public bool IsCommunityComplete() => _bundleRepo.IsCommunityComplete();

    /// <summary>Get detailed information about a specific bundle including remaining slots.</summary>
    /// <param name="bundleName">The name of the bundle to get details for.</param>
    /// <returns>Bundle details or null if not found.</returns>
    public BundleDetails? GetBundleDetails(string bundleName)
    {
        var bundle = _bundleRepo.GetAllBundles()
            .FirstOrDefault(b => b.BundleName.Equals(bundleName, StringComparison.OrdinalIgnoreCase));

        if (bundle == null)
        {
            _logger.Log(LogLevel.Debug, $"Bundle '{bundleName}' not found");
            return null;
        }

        var remainingSlots = bundle.GetRemainingSlots().ToList();

        return new BundleDetails
        {
            BundleName = bundle.BundleName,
            RoomName = bundle.RoomName,
            IsComplete = bundle.IsComplete,
            TotalSlots = bundle.Slots.Count,
            FilledSlots = bundle.SlotsFilled,
            RemainingSlots = remainingSlots.AsReadOnly()
        };
    }
}

/// <summary>Simple count of items in a bundle.</summary>
public sealed class BundleItemCount
{
    /// <summary>Name of the bundle.</summary>
    public string BundleName { get; init; } = string.Empty;

    /// <summary>Room the bundle belongs to.</summary>
    public string RoomName { get; init; } = string.Empty;

    /// <summary>Total items required.</summary>
    public int TotalItems { get; init; }

    /// <summary>Items already contributed.</summary>
    public int CompletedItems { get; init; }

    /// <summary>Whether the bundle is complete.</summary>
    public bool IsComplete { get; init; }

    /// <summary>Remaining items needed.</summary>
    public int RemainingItems => TotalItems - CompletedItems;
}

/// <summary>Detailed bundle information including remaining slots with item requirements.</summary>
public sealed class BundleDetails
{
    /// <summary>Name of the bundle.</summary>
    public string BundleName { get; init; } = string.Empty;

    /// <summary>Room the bundle belongs to.</summary>
    public string RoomName { get; init; } = string.Empty;

    /// <summary>Whether the bundle is complete.</summary>
    public bool IsComplete { get; init; }

    /// <summary>Total number of slots in the bundle.</summary>
    public int TotalSlots { get; init; }

    /// <summary>Number of slots that have been filled.</summary>
    public int FilledSlots { get; init; }

    /// <summary>Slots that still need to be filled, with item requirement details.</summary>
    public IReadOnlyList<BundleSlot> RemainingSlots { get; init; } = Array.Empty<BundleSlot>();
}
