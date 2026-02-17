using StardewModdingAPI;
using StardewValley;
using StardewFOMO.Core.Interfaces;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// Aggregates all perfection-related adapters for integration with Ginger Island and overall perfection tracking.
/// Implements <see cref="IPerfectionRepository"/>.
/// </summary>
public sealed class PerfectionAdapter : IPerfectionRepository
{
    /// <summary>
    /// Gets whether Ginger Island has been unlocked by checking if the boat has been repaired.
    /// </summary>
    public bool IsGingerIslandUnlocked()
    {
        if (!Context.IsWorldReady || Game1.player == null)
            return false;

        // Check if the boat to Ginger Island has been repaired
        return Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed");
    }

    /// <summary>
    /// Gets the total number of golden walnuts collected.
    /// </summary>
    public int GetGoldenWalnutsCollected()
    {
        if (!Context.IsWorldReady)
            return 0;

        // Golden walnuts are stored in a special world state variable
        return Game1.netWorldState.Value.GoldenWalnutsFound;
    }

    /// <summary>
    /// Gets the total number of golden walnuts available (always 130 in vanilla).
    /// </summary>
    public int GetGoldenWalnutsTotal()
    {
        return 130;
    }
}
