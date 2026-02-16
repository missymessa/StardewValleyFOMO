using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts NPC birthday and gift preference data.
/// Implemented by SMAPI adapter reading NPC data from game content.
/// </summary>
public interface INpcRepository
{
    /// <summary>Get all NPC birthdays as a list of (NPC name, birthday date) pairs.</summary>
    IReadOnlyList<NpcBirthday> GetAllBirthdays();

    /// <summary>Get the list of loved gift item IDs for a specific NPC.</summary>
    IReadOnlyList<string> GetLovedGifts(string npcName);

    /// <summary>Get the list of liked gift item IDs for a specific NPC.</summary>
    IReadOnlyList<string> GetLikedGifts(string npcName);
}
