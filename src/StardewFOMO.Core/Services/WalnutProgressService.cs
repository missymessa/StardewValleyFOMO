using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking Golden Walnut collection toward perfection.
/// </summary>
public sealed class WalnutProgressService
{
    private readonly IWalnutRepository _walnutRepo;
    private readonly IPerfectionRepository _perfectionRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (5%)
    private const double PerfectionWeight = 5.0;

    // Total golden walnuts for perfection
    private const int TotalWalnuts = 130;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalnutProgressService"/> class.
    /// </summary>
    public WalnutProgressService(IWalnutRepository walnutRepo, IPerfectionRepository perfectionRepo, ILogger logger)
    {
        _walnutRepo = walnutRepo;
        _perfectionRepo = perfectionRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the golden walnut progress category for perfection tracking.
    /// </summary>
    public PerfectionCategory GetProgress()
    {
        var foundCount = _perfectionRepo.GetGoldenWalnutsCollected();

        _logger.Log(LogLevel.Trace, $"Golden Walnut progress: {foundCount}/{TotalWalnuts}");

        return new PerfectionCategory
        {
            CategoryName = "Golden Walnuts",
            CurrentCount = foundCount,
            TotalCount = TotalWalnuts,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets walnut groups with unobtained walnuts.
    /// </summary>
    public IReadOnlyList<WalnutGroup> GetRemainingWalnutGroups()
    {
        return _walnutRepo.GetAllWalnutGroups()
            .Where(g => g.TotalCount > g.CollectedCount)
            .OrderByDescending(g => g.TotalCount - g.CollectedCount)
            .ToList()
            .AsReadOnly();
    }
}
