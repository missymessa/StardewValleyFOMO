using StardewValley;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IMonsterRepository"/> by reading player monster kill stats.
/// </summary>
public sealed class MonsterAdapter : IMonsterRepository
{
    // Monster eradication goal definitions (category -> required kills)
    private static readonly Dictionary<string, (string DisplayName, int Required, string[] Locations)> MonsterGoals = new()
    {
        ["Slimes"] = ("Slimes", 1000, new[] { "The Mines", "Skull Cavern", "Secret Woods" }),
        ["Void Spirits"] = ("Void Spirits", 150, new[] { "The Mines (80-120)", "Skull Cavern" }),
        ["Bats"] = ("Bats", 200, new[] { "The Mines (40-80)", "Skull Cavern" }),
        ["Skeletons"] = ("Skeletons", 50, new[] { "The Mines (70-79)", "Skull Cavern" }),
        ["Cave Insects"] = ("Cave Insects", 125, new[] { "The Mines (10-30)" }),
        ["Duggies"] = ("Duggies", 30, new[] { "The Mines (6-30)" }),
        ["Dust Sprites"] = ("Dust Sprites", 500, new[] { "The Mines (40-79)" }),
        ["Rock Crabs"] = ("Rock Crabs", 60, new[] { "The Mines (1-30)", "Beach" }),
        ["Mummies"] = ("Mummies", 100, new[] { "Skull Cavern" }),
        ["Pepper Rex"] = ("Pepper Rex", 50, new[] { "Skull Cavern (prehistoric floors)" }),
        ["Serpents"] = ("Serpents", 250, new[] { "Skull Cavern" }),
        ["Magma Sprites"] = ("Magma Sprites", 150, new[] { "Volcano Dungeon" })
    };

    // Monster name mappings to categories
    private static readonly Dictionary<string, string> MonsterToCategory = new()
    {
        ["Green Slime"] = "Slimes",
        ["Frost Jelly"] = "Slimes",
        ["Sludge"] = "Slimes",
        ["Tiger Slime"] = "Slimes",
        ["Shadow Brute"] = "Void Spirits",
        ["Shadow Shaman"] = "Void Spirits",
        ["Shadow Sniper"] = "Void Spirits",
        ["Bat"] = "Bats",
        ["Frost Bat"] = "Bats",
        ["Lava Bat"] = "Bats",
        ["Iridium Bat"] = "Bats",
        ["Skeleton"] = "Skeletons",
        ["Skeleton Mage"] = "Skeletons",
        ["Bug"] = "Cave Insects",
        ["Fly"] = "Cave Insects",
        ["Grub"] = "Cave Insects",
        ["Duggy"] = "Duggies",
        ["Magma Duggy"] = "Duggies",
        ["Dust Spirit"] = "Dust Sprites",
        ["Rock Crab"] = "Rock Crabs",
        ["Lava Crab"] = "Rock Crabs",
        ["Iridium Crab"] = "Rock Crabs",
        ["Mummy"] = "Mummies",
        ["Pepper Rex"] = "Pepper Rex",
        ["Serpent"] = "Serpents",
        ["Royal Serpent"] = "Serpents",
        ["Magma Sprite"] = "Magma Sprites",
        ["Magma Sparker"] = "Magma Sprites"
    };

    /// <inheritdoc/>
    public IReadOnlyList<MonsterGoal> GetAllMonsterGoals()
    {
        var goals = new List<MonsterGoal>();
        var killCounts = GetCategoryKillCounts();

        foreach (var (category, info) in MonsterGoals)
        {
            var kills = killCounts.TryGetValue(category, out var count) ? count : 0;
            goals.Add(new MonsterGoal
            {
                MonsterCategory = category,
                DisplayName = info.DisplayName,
                CurrentKills = kills,
                RequiredKills = info.Required,
                SpawnLocations = info.Locations.ToList().AsReadOnly()
            });
        }

        return goals.AsReadOnly();
    }

    /// <inheritdoc/>
    public int GetKillCount(string monsterCategory)
    {
        var killCounts = GetCategoryKillCounts();
        return killCounts.TryGetValue(monsterCategory, out var count) ? count : 0;
    }

    /// <inheritdoc/>
    public bool AreAllGoalsComplete()
    {
        var killCounts = GetCategoryKillCounts();
        return MonsterGoals.All(kvp =>
            killCounts.TryGetValue(kvp.Key, out var count) && count >= kvp.Value.Required);
    }

    private Dictionary<string, int> GetCategoryKillCounts()
    {
        var categoryCounts = new Dictionary<string, int>();

        if (Game1.player?.stats?.specificMonstersKilled == null)
            return categoryCounts;

        foreach (var kvp in Game1.player.stats.specificMonstersKilled)
        {
            var monsterName = kvp.Key;
            var kills = kvp.Value;

            if (MonsterToCategory.TryGetValue(monsterName, out var category))
            {
                if (!categoryCounts.ContainsKey(category))
                    categoryCounts[category] = 0;
                categoryCounts[category] += kills;
            }
        }

        return categoryCounts;
    }
}
