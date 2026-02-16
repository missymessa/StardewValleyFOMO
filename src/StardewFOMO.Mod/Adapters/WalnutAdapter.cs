using StardewValley;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IWalnutRepository"/> by reading game world state.
/// </summary>
public sealed class WalnutAdapter : IWalnutRepository
{
    // Approximate walnut distribution by acquisition type
    private static readonly (string GroupType, string DisplayName, int Total)[] WalnutGroups =
    {
        (WalnutGroupType.Digging, "Digging/Hidden", 19),
        (WalnutGroupType.Puzzles, "Puzzles & Secrets", 35),
        (WalnutGroupType.Fishing, "Fishing", 13),
        (WalnutGroupType.NPCs, "NPC Tasks", 27),
        (WalnutGroupType.Combat, "Combat/Volcano", 20),
        (WalnutGroupType.Farming, "Farming & Shipping", 16)
    };

    /// <inheritdoc/>
    public IReadOnlyList<WalnutGroup> GetAllWalnutGroups()
    {
        // Note: Stardew Valley doesn't track walnuts by category in save data
        // We can only get total count, so we show it as one group for now
        var collected = GetCollectedCount();
        var total = GetTotalCount();

        var groups = new List<WalnutGroup>
        {
            new WalnutGroup
            {
                GroupType = "Total",
                DisplayName = "Golden Walnuts",
                CollectedCount = collected,
                TotalCount = total
            }
        };

        return groups.AsReadOnly();
    }

    /// <inheritdoc/>
    public int GetCollectedCount()
    {
        return Game1.netWorldState?.Value?.GoldenWalnutsFound ?? 0;
    }

    /// <inheritdoc/>
    public int GetTotalCount() => 130;

    /// <inheritdoc/>
    public bool IsGingerIslandUnlocked()
    {
        return Game1.MasterPlayer?.hasOrWillReceiveMail("Island_Turtle") ?? false;
    }
}
