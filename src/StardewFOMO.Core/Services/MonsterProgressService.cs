using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking Monster Slayer Hero goals toward perfection.
/// </summary>
public sealed class MonsterProgressService
{
    private readonly IMonsterRepository _monsterRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (10%)
    private const double PerfectionWeight = 10.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonsterProgressService"/> class.
    /// </summary>
    public MonsterProgressService(IMonsterRepository monsterRepo, ILogger logger)
    {
        _monsterRepo = monsterRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the monster slayer progress category for perfection tracking.
    /// </summary>
    public PerfectionCategory GetProgress()
    {
        var goals = _monsterRepo.GetAllMonsterGoals();
        var completedCount = goals.Count(g => g.IsComplete);
        var totalCount = goals.Count;

        _logger.Log(LogLevel.Trace, $"Monster Slayer progress: {completedCount}/{totalCount}");

        return new PerfectionCategory
        {
            CategoryName = "Monster Slayer",
            CurrentCount = completedCount,
            TotalCount = totalCount,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets monster goals that are not yet complete.
    /// </summary>
    public IReadOnlyList<MonsterGoal> GetIncompleteGoals()
    {
        return _monsterRepo.GetAllMonsterGoals()
            .Where(g => !g.IsComplete)
            .OrderByDescending(g => g.CurrentKills)
            .ToList()
            .AsReadOnly();
    }
}
