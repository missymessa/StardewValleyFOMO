using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts monster eradication goal data.
/// Implemented by SMAPI adapter reading from player stats.
/// </summary>
public interface IMonsterRepository
{
    /// <summary>Get all monster eradication goals with current kill counts.</summary>
    IReadOnlyList<MonsterGoal> GetAllMonsterGoals();

    /// <summary>Get the kill count for a specific monster category.</summary>
    int GetKillCount(string monsterCategory);

    /// <summary>Check if all monster eradication goals are complete.</summary>
    bool AreAllGoalsComplete();
}
