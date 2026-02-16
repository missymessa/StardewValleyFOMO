using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking fish collection progress toward perfection.
/// </summary>
public sealed class FishProgressService
{
    private readonly ICollectionRepository _collectionRepo;
    private readonly IFishRepository _fishRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (10%)
    private const double PerfectionWeight = 10.0;

    // Total fish for perfection (vanilla count)
    private const int TotalFish = 67;

    /// <summary>
    /// Initializes a new instance of the <see cref="FishProgressService"/> class.
    /// </summary>
    public FishProgressService(ICollectionRepository collectionRepo, IFishRepository fishRepo, ILogger logger)
    {
        _collectionRepo = collectionRepo;
        _fishRepo = fishRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the fish collection progress category for perfection tracking.
    /// </summary>
    public PerfectionCategory GetProgress()
    {
        var caughtCount = CountCaughtFish();

        _logger.Log(LogLevel.Trace, $"Fish progress: {caughtCount}/{TotalFish}");

        return new PerfectionCategory
        {
            CategoryName = "Fish",
            CurrentCount = caughtCount,
            TotalCount = TotalFish,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets fish that have not yet been caught.
    /// </summary>
    public IReadOnlyList<PerfectionItem> GetUncaughtFish()
    {
        var allFish = _fishRepo.GetAllFish();
        var uncaught = new List<PerfectionItem>();

        foreach (var fish in allFish)
        {
            if (!_collectionRepo.HasCaughtFish(fish.Id))
            {
                uncaught.Add(new PerfectionItem
                {
                    ItemId = fish.Id,
                    DisplayName = fish.Name,
                    IsComplete = false,
                    AcquisitionHint = GetFishHint(fish)
                });
            }
        }

        return uncaught.AsReadOnly();
    }

    private static string GetFishHint(CollectibleItem fish)
    {
        var hints = new List<string>();
        if (fish.Locations.Count > 0)
            hints.Add(string.Join(", ", fish.Locations));
        if (fish.AvailableSeasons.Count > 0 && fish.AvailableSeasons.Count < 4)
            hints.Add(string.Join("/", fish.AvailableSeasons));
        return hints.Count > 0 ? string.Join(" - ", hints) : "Unknown";
    }

    private int CountCaughtFish()
    {
        var allFish = _fishRepo.GetAllFish();
        return allFish.Count(f => _collectionRepo.HasCaughtFish(f.Id));
    }
}
