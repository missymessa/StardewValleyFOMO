using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts NPC friendship data.
/// Implemented by SMAPI adapter reading from player relationship data.
/// </summary>
public interface IFriendshipRepository
{
    /// <summary>Get friendship info for all NPCs that count toward perfection.</summary>
    IReadOnlyList<FriendshipInfo> GetAllFriendships();

    /// <summary>Get friendship info for a specific NPC.</summary>
    FriendshipInfo? GetFriendship(string npcName);

    /// <summary>Get the count of NPCs at maximum friendship.</summary>
    int GetMaxFriendshipCount();

    /// <summary>Get the total count of NPCs that count toward perfection.</summary>
    int GetTotalNpcCount();

    /// <summary>Check if all NPCs are at maximum friendship.</summary>
    bool AreAllMaxFriendship();
}
