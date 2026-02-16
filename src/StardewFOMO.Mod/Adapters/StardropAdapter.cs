using StardewValley;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IStardropRepository"/> by reading player mail flags.
/// </summary>
public sealed class StardropAdapter : IStardropRepository
{
    // Stardrop mail flags and their acquisition hints
    private static readonly (string Source, string MailFlag, string DisplayName, string Hint)[] StardropDefinitions =
    {
        (StardropSource.Fair, "CF_Fair", "Stardew Valley Fair", "Buy for 2,000 star tokens at the Stardew Valley Fair"),
        (StardropSource.Mine, "CF_Mines", "Mine Level 100", "Reach level 100 of The Mines"),
        (StardropSource.Museum, "CF_Spouse", "Museum Donation", "Donate all 95 items to the Museum"),
        (StardropSource.Spouse, "CF_Spouse", "Spouse Gift", "Reach 12.5 hearts with your spouse"),
        (StardropSource.MasterAngler, "CF_Fish", "Master Angler", "Catch every fish (Master Angler achievement)"),
        (StardropSource.OldMasterCannoli, "CF_Statue", "Old Master Cannoli", "Give a Sweet Gem Berry to the statue in the Secret Woods"),
        (StardropSource.Krobus, "CF_Sewer", "Krobus/Sewers", "Purchase from Krobus in the Sewers for 20,000g")
    };

    /// <inheritdoc/>
    public IReadOnlyList<StardropInfo> GetAllStardrops()
    {
        var stardrops = new List<StardropInfo>();

        foreach (var (source, mailFlag, displayName, hint) in StardropDefinitions)
        {
            stardrops.Add(new StardropInfo
            {
                Source = source,
                DisplayName = displayName,
                IsCollected = HasMailFlag(mailFlag),
                AcquisitionHint = hint
            });
        }

        return stardrops.AsReadOnly();
    }

    /// <inheritdoc/>
    public int GetCollectedCount()
    {
        return StardropDefinitions.Count(d => HasMailFlag(d.MailFlag));
    }

    /// <inheritdoc/>
    public int GetTotalCount() => StardropDefinitions.Length;

    /// <inheritdoc/>
    public bool HasCollectedStardrop(string stardropSource)
    {
        var definition = StardropDefinitions.FirstOrDefault(d => d.Source == stardropSource);
        return definition.MailFlag != null && HasMailFlag(definition.MailFlag);
    }

    private static bool HasMailFlag(string mailFlag)
    {
        return Game1.player?.mailReceived?.Contains(mailFlag) ?? false;
    }
}
