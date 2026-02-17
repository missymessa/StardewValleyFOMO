using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IMonsterRepository"/> with configurable monster goals.</summary>
public sealed class FakeMonsterRepository : IMonsterRepository
{
    private readonly List<MonsterGoal> _monsterGoals = new();

    public void AddMonsterGoal(MonsterGoal goal) => _monsterGoals.Add(goal);

    public void SetKillCount(string monsterCategory, int kills)
    {
        var goal = _monsterGoals.FirstOrDefault(g => g.MonsterCategory == monsterCategory);
        if (goal != null)
        {
            var index = _monsterGoals.IndexOf(goal);
            _monsterGoals[index] = new MonsterGoal
            {
                MonsterCategory = goal.MonsterCategory,
                DisplayName = goal.DisplayName,
                CurrentKills = kills,
                RequiredKills = goal.RequiredKills,
                SpawnLocations = goal.SpawnLocations
            };
        }
    }

    public void SetGoalComplete(string monsterCategory, bool complete)
    {
        var existing = _monsterGoals.FirstOrDefault(g => g.MonsterCategory == monsterCategory);
        if (existing != null)
        {
            _monsterGoals.Remove(existing);
        }
        _monsterGoals.Add(new MonsterGoal
        {
            MonsterCategory = monsterCategory,
            DisplayName = monsterCategory,
            CurrentKills = complete ? 100 : 50,
            RequiredKills = 100,
            SpawnLocations = new List<string> { "Mines" }
        });
    }

    public void SetAllGoalsComplete()
    {
        _monsterGoals.Clear();
        var categories = new[] { "Slimes", "Void Spirits", "Bats", "Skeletons", "Cave Insects",
                                  "Duggies", "Dust Sprites", "Rock Crabs", "Mummies", "Pepper Rex",
                                  "Serpents", "Magma Sprites" };
        foreach (var cat in categories)
        {
            _monsterGoals.Add(new MonsterGoal
            {
                MonsterCategory = cat,
                DisplayName = cat,
                CurrentKills = 1000,
                RequiredKills = 1000,
                SpawnLocations = new List<string> { "Mines" }
            });
        }
    }

    public IReadOnlyList<MonsterGoal> GetAllMonsterGoals() => _monsterGoals.AsReadOnly();

    public int GetKillCount(string monsterCategory) =>
        _monsterGoals.FirstOrDefault(g => g.MonsterCategory == monsterCategory)?.CurrentKills ?? 0;

    public bool AreAllGoalsComplete() => _monsterGoals.All(g => g.IsComplete);
}
