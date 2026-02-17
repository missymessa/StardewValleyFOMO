using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking Stardrop collection toward perfection.
/// </summary>
public sealed class StardropProgressService
{
    private readonly IStardropRepository _stardropRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (2%)
    private const double PerfectionWeight = 2.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="StardropProgressService"/> class.
    /// </summary>
    public StardropProgressService(IStardropRepository stardropRepo, ILogger logger)
    {
        _stardropRepo = stardropRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the stardrop collection progress category for perfection tracking.
    /// </summary>
    public PerfectionCategory GetProgress()
    {
        var stardrops = _stardropRepo.GetAllStardrops();
        var collectedCount = stardrops.Count(s => s.IsCollected);
        var totalCount = stardrops.Count;

        _logger.Log(LogLevel.Trace, $"Stardrop progress: {collectedCount}/{totalCount}");

        return new PerfectionCategory
        {
            CategoryName = "Stardrops",
            CurrentCount = collectedCount,
            TotalCount = totalCount,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets stardrops that have not yet been collected.
    /// </summary>
    public IReadOnlyList<StardropInfo> GetUncollectedStardrops()
    {
        return _stardropRepo.GetAllStardrops()
            .Where(s => !s.IsCollected)
            .ToList()
            .AsReadOnly();
    }
}
