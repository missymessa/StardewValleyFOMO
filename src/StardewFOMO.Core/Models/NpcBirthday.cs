namespace StardewFOMO.Core.Models;

/// <summary>
/// Represents an NPC's birthday with their gift preferences.
/// </summary>
public sealed class NpcBirthday
{
    /// <summary>The NPC's display name.</summary>
    public string NpcName { get; init; } = string.Empty;

    /// <summary>The NPC's birthday date.</summary>
    public GameDate BirthdayDate { get; init; } = null!;

    /// <summary>Item IDs/names the NPC loves as gifts.</summary>
    public IReadOnlyList<string> LovedGifts { get; init; } = Array.Empty<string>();

    /// <summary>Item IDs/names the NPC likes as gifts.</summary>
    public IReadOnlyList<string> LikedGifts { get; init; } = Array.Empty<string>();
}
