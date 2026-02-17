using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts player skill level data.
/// Implemented by SMAPI adapter reading from player object.
/// </summary>
public interface ISkillRepository
{
    /// <summary>Get all skills with current levels and XP progress.</summary>
    IReadOnlyList<SkillProgress> GetAllSkills();

    /// <summary>Get the level for a specific skill.</summary>
    int GetSkillLevel(string skillName);

    /// <summary>Get the XP required to reach the next level for a skill.</summary>
    int GetXpToNextLevel(string skillName);

    /// <summary>Check if all skills are at maximum level (10).</summary>
    bool AreAllSkillsMaxed();
}
