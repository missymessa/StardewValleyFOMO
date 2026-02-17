using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts stardrop collection data.
/// Implemented by SMAPI adapter reading from player mail flags.
/// </summary>
public interface IStardropRepository
{
    /// <summary>Get all stardrops with collection status and acquisition hints.</summary>
    IReadOnlyList<StardropInfo> GetAllStardrops();

    /// <summary>Get the count of stardrops collected.</summary>
    int GetCollectedCount();

    /// <summary>Get the total number of stardrops (7).</summary>
    int GetTotalCount();

    /// <summary>Check if a specific stardrop has been collected.</summary>
    bool HasCollectedStardrop(string stardropSource);
}
