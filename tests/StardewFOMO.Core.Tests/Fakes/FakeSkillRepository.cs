using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="ISkillRepository"/> with configurable skill levels.</summary>
public sealed class FakeSkillRepository : ISkillRepository
{
    private readonly Dictionary<string, SkillProgress> _skills = new();

    public void SetSkill(string skillName, int level, int xpToNext = 0)
    {
        _skills[skillName] = new SkillProgress
        {
            SkillName = skillName,
            CurrentLevel = level,
            MaxLevel = 10,
            XpToNextLevel = xpToNext
        };
    }

    public void SetSkillLevel(string skillName, int level) => SetSkill(skillName, level);

    public void SetAllSkillsMaxed()
    {
        var skillNames = new[] { "Farming", "Mining", "Foraging", "Fishing", "Combat" };
        foreach (var name in skillNames)
        {
            SetSkill(name, 10, 0);
        }
    }

    public IReadOnlyList<SkillProgress> GetAllSkills() => _skills.Values.ToList().AsReadOnly();

    public int GetSkillLevel(string skillName) =>
        _skills.TryGetValue(skillName, out var skill) ? skill.CurrentLevel : 0;

    public int GetXpToNextLevel(string skillName) =>
        _skills.TryGetValue(skillName, out var skill) ? skill.XpToNextLevel : 0;

    public bool AreAllSkillsMaxed() => _skills.Values.All(s => s.IsMaxed);
}
