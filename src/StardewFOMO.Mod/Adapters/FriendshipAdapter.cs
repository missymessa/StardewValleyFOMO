using StardewValley;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IFriendshipRepository"/> by reading player relationship data.
/// </summary>
public sealed class FriendshipAdapter : IFriendshipRepository
{
    // NPCs that count toward perfection (all villagers except some exclusions)
    private static readonly HashSet<string> DateableNpcs = new()
    {
        "Alex", "Elliott", "Harvey", "Sam", "Sebastian", "Shane",
        "Abigail", "Emily", "Haley", "Leah", "Maru", "Penny"
    };

    // NPCs excluded from perfection tracking
    private static readonly HashSet<string> ExcludedNpcs = new()
    {
        "Dwarf", "Krobus", "Sandy", "Leo"  // Some special characters with different requirements
    };

    /// <inheritdoc/>
    public IReadOnlyList<FriendshipInfo> GetAllFriendships()
    {
        var friendships = new List<FriendshipInfo>();

        if (Game1.player?.friendshipData == null)
            return friendships.AsReadOnly();

        foreach (var kvp in Game1.player.friendshipData.Pairs)
        {
            var npcName = kvp.Key;
            var friendship = kvp.Value;

            // Skip excluded NPCs
            if (ExcludedNpcs.Contains(npcName))
                continue;

            var isDateable = DateableNpcs.Contains(npcName);
            var isDating = friendship.IsDating();
            var isMarried = friendship.IsMarried();

            // Max hearts: 8 for non-dateables, 10 for dateables, 14 for spouse
            int maxHearts;
            if (isMarried)
                maxHearts = 14;
            else if (isDateable && isDating)
                maxHearts = 10;
            else if (isDateable)
                maxHearts = 8; // Until dating starts
            else
                maxHearts = 10;

            var currentHearts = friendship.Points / 250;

            friendships.Add(new FriendshipInfo
            {
                NpcName = npcName,
                CurrentHearts = Math.Min(currentHearts, maxHearts),
                MaxHearts = maxHearts,
                IsDateable = isDateable,
                IsDating = isDating,
                IsMarried = isMarried
            });
        }

        // Sort by hearts remaining (most needed first)
        return friendships
            .OrderByDescending(f => f.HeartsRemaining)
            .ThenBy(f => f.NpcName)
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc/>
    public FriendshipInfo? GetFriendship(string npcName)
    {
        return GetAllFriendships().FirstOrDefault(f => f.NpcName == npcName);
    }

    /// <inheritdoc/>
    public int GetMaxFriendshipCount()
    {
        return GetAllFriendships().Count(f => f.IsMaxFriendship);
    }

    /// <inheritdoc/>
    public int GetTotalNpcCount()
    {
        return GetAllFriendships().Count;
    }

    /// <inheritdoc/>
    public bool AreAllMaxFriendship()
    {
        var friendships = GetAllFriendships();
        return friendships.Count > 0 && friendships.All(f => f.IsMaxFriendship);
    }
}
