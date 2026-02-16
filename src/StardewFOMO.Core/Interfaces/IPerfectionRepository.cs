namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Aggregates all perfection data sources for overall progress calculation.
/// Implemented by SMAPI adapter that delegates to category-specific adapters.
/// </summary>
public interface IPerfectionRepository
{
    /// <summary>Check if Ginger Island has been unlocked by the player.</summary>
    bool IsGingerIslandUnlocked();

    /// <summary>Get the total number of golden walnuts collected.</summary>
    int GetGoldenWalnutsCollected();

    /// <summary>Get the total number of golden walnuts available (130).</summary>
    int GetGoldenWalnutsTotal();
}
