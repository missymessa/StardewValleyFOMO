using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IFriendshipRepository"/> with configurable friendship data.</summary>
public sealed class FakeFriendshipRepository : IFriendshipRepository
{
    private readonly Dictionary<string, FriendshipInfo> _friendships = new();

    public void SetFriendship(string npcName, int currentHearts, int maxHearts, bool isDateable = false, bool isDating = false, bool isMarried = false)
    {
        _friendships[npcName] = new FriendshipInfo
        {
            NpcName = npcName,
            CurrentHearts = currentHearts,
            MaxHearts = maxHearts,
            IsDateable = isDateable,
            IsDating = isDating,
            IsMarried = isMarried
        };
    }

    public IReadOnlyList<FriendshipInfo> GetAllFriendships() => _friendships.Values.ToList().AsReadOnly();

    public FriendshipInfo? GetFriendship(string npcName) =>
        _friendships.TryGetValue(npcName, out var friendship) ? friendship : null;

    public int GetMaxFriendshipCount() => _friendships.Values.Count(f => f.IsMaxFriendship);

    public int GetTotalNpcCount() => _friendships.Count;

    public bool AreAllMaxFriendship() => _friendships.Values.All(f => f.IsMaxFriendship);
}
