namespace StardewFOMO.Core.Models;

/// <summary>
/// Skill level tracking for perfection.
/// </summary>
public sealed class SkillProgress
{
    /// <summary>Skill name (Farming, Mining, Foraging, Fishing, Combat).</summary>
    public string SkillName { get; init; } = string.Empty;

    /// <summary>Current skill level (0-10).</summary>
    public int CurrentLevel { get; init; }

    /// <summary>Maximum skill level (10).</summary>
    public int MaxLevel { get; init; } = 10;

    /// <summary>Current XP in this skill.</summary>
    public int CurrentXp { get; init; }

    /// <summary>XP required to reach the next level (0 if maxed).</summary>
    public int XpToNextLevel { get; init; }

    /// <summary>Whether this skill is at maximum level.</summary>
    public bool IsMaxed => CurrentLevel >= MaxLevel;
}

/// <summary>
/// Skill name constants.
/// </summary>
public static class SkillName
{
    /// <summary>Farming skill.</summary>
    public const string Farming = "Farming";
    /// <summary>Mining skill.</summary>
    public const string Mining = "Mining";
    /// <summary>Foraging skill.</summary>
    public const string Foraging = "Foraging";
    /// <summary>Fishing skill.</summary>
    public const string Fishing = "Fishing";
    /// <summary>Combat skill.</summary>
    public const string Combat = "Combat";
}
