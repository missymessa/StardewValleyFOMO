using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts golden walnut collection data.
/// Implemented by SMAPI adapter reading from game world state.
/// </summary>
public interface IWalnutRepository
{
    /// <summary>Get all walnut groups with collection counts per acquisition type.</summary>
    IReadOnlyList<WalnutGroup> GetAllWalnutGroups();

    /// <summary>Get the total number of golden walnuts collected.</summary>
    int GetCollectedCount();

    /// <summary>Get the total number of golden walnuts available (130).</summary>
    int GetTotalCount();

    /// <summary>Check if Ginger Island has been unlocked.</summary>
    bool IsGingerIslandUnlocked();
}
