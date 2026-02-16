using StardewValley;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="ISkillRepository"/> by reading player skill levels.
/// </summary>
public sealed class SkillAdapter : ISkillRepository
{
    // XP required for each skill level (cumulative)
    private static readonly int[] LevelXp = { 0, 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };

    /// <inheritdoc/>
    public IReadOnlyList<SkillProgress> GetAllSkills()
    {
        var skills = new List<SkillProgress>
        {
            CreateSkillProgress(SkillName.Farming, Farmer.farmingSkill),
            CreateSkillProgress(SkillName.Mining, Farmer.miningSkill),
            CreateSkillProgress(SkillName.Foraging, Farmer.foragingSkill),
            CreateSkillProgress(SkillName.Fishing, Farmer.fishingSkill),
            CreateSkillProgress(SkillName.Combat, Farmer.combatSkill)
        };

        return skills.AsReadOnly();
    }

    /// <inheritdoc/>
    public int GetSkillLevel(string skillName)
    {
        var skillId = GetSkillId(skillName);
        return Game1.player?.GetSkillLevel(skillId) ?? 0;
    }

    /// <inheritdoc/>
    public int GetXpToNextLevel(string skillName)
    {
        var skillId = GetSkillId(skillName);
        var currentLevel = Game1.player?.GetSkillLevel(skillId) ?? 0;

        if (currentLevel >= 10)
            return 0;

        var currentXp = Game1.player?.experiencePoints[skillId] ?? 0;
        var nextLevelXp = LevelXp[currentLevel + 1];
        return Math.Max(0, nextLevelXp - currentXp);
    }

    /// <inheritdoc/>
    public bool AreAllSkillsMaxed()
    {
        if (Game1.player == null)
            return false;

        return Game1.player.GetSkillLevel(Farmer.farmingSkill) >= 10
            && Game1.player.GetSkillLevel(Farmer.miningSkill) >= 10
            && Game1.player.GetSkillLevel(Farmer.foragingSkill) >= 10
            && Game1.player.GetSkillLevel(Farmer.fishingSkill) >= 10
            && Game1.player.GetSkillLevel(Farmer.combatSkill) >= 10;
    }

    private SkillProgress CreateSkillProgress(string skillName, int skillId)
    {
        var level = Game1.player?.GetSkillLevel(skillId) ?? 0;
        var currentXp = Game1.player?.experiencePoints[skillId] ?? 0;
        var xpToNext = level >= 10 ? 0 : Math.Max(0, LevelXp[level + 1] - currentXp);

        return new SkillProgress
        {
            SkillName = skillName,
            CurrentLevel = level,
            MaxLevel = 10,
            CurrentXp = currentXp,
            XpToNextLevel = xpToNext
        };
    }

    private static int GetSkillId(string skillName)
    {
        return skillName switch
        {
            SkillName.Farming => Farmer.farmingSkill,
            SkillName.Mining => Farmer.miningSkill,
            SkillName.Foraging => Farmer.foragingSkill,
            SkillName.Fishing => Farmer.fishingSkill,
            SkillName.Combat => Farmer.combatSkill,
            _ => 0
        };
    }
}
