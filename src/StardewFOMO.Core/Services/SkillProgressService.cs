using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking skill progress toward perfection.
/// </summary>
public sealed class SkillProgressService
{
    private readonly ISkillRepository _skillRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (2%)
    private const double PerfectionWeight = 2.0;

    // Standard vanilla skills
    private const int TotalSkills = 5;
    private const int MaxSkillLevel = 10;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillProgressService"/> class.
    /// </summary>
    public SkillProgressService(ISkillRepository skillRepo, ILogger logger)
    {
        _skillRepo = skillRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the skill progress category for perfection tracking.
    /// </summary>
    public PerfectionCategory GetProgress()
    {
        var skills = _skillRepo.GetAllSkills();
        var maxedCount = skills.Count(s => s.CurrentLevel >= MaxSkillLevel);
        var totalCount = skills.Count > 0 ? skills.Count : TotalSkills;

        _logger.Log(LogLevel.Trace, $"Skill progress: {maxedCount}/{totalCount} skills at level 10");

        return new PerfectionCategory
        {
            CategoryName = "Skills",
            CurrentCount = maxedCount,
            TotalCount = totalCount,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets skills that are not yet at level 10.
    /// </summary>
    public IReadOnlyList<SkillProgress> GetIncompleteSkills()
    {
        return _skillRepo.GetAllSkills()
            .Where(s => s.CurrentLevel < MaxSkillLevel)
            .OrderByDescending(s => s.CurrentLevel)
            .ToList()
            .AsReadOnly();
    }
}
