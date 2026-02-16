using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts forage availability data.
/// Implemented by SMAPI adapter reading forage data from game content.
/// </summary>
public interface IForageRepository
{
    /// <summary>Get all forageable items known to the game.</summary>
    IReadOnlyList<CollectibleItem> GetAllForageables();

    /// <summary>Get forageable items available in the given season, grouped by location.</summary>
    IReadOnlyList<CollectibleItem> GetForageablesBySeason(Season season);

    /// <summary>Get all forageables exclusive to a specific season.</summary>
    IReadOnlyList<CollectibleItem> GetSeasonExclusiveForageables(Season season);
}
