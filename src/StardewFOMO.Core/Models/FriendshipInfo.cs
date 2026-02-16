namespace StardewFOMO.Core.Models;

/// <summary>
/// NPC friendship state for perfection tracking.
/// </summary>
public sealed class FriendshipInfo
{
    /// <summary>NPC name.</summary>
    public string NpcName { get; init; } = string.Empty;

    /// <summary>Current friendship hearts (0-14 for dateables, 0-10 for non-dateables).</summary>
    public int CurrentHearts { get; init; }

    /// <summary>Maximum possible hearts for this NPC.</summary>
    public int MaxHearts { get; init; }

    /// <summary>Whether this NPC is a dateable character.</summary>
    public bool IsDateable { get; init; }

    /// <summary>Whether the player is currently dating this NPC.</summary>
    public bool IsDating { get; init; }

    /// <summary>Whether the player is married to this NPC.</summary>
    public bool IsMarried { get; init; }

    /// <summary>Whether this NPC is at maximum friendship.</summary>
    public bool IsMaxFriendship => CurrentHearts >= MaxHearts;

    /// <summary>Hearts remaining to reach maximum.</summary>
    public int HeartsRemaining => Math.Max(0, MaxHearts - CurrentHearts);
}
