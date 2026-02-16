namespace StardewFOMO.Core.Models;

/// <summary>
/// Stardrop collection status with acquisition hint.
/// </summary>
public sealed class StardropInfo
{
    /// <summary>Source identifier for the stardrop.</summary>
    public string Source { get; init; } = string.Empty;

    /// <summary>Display name for the stardrop source.</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Whether this stardrop has been collected.</summary>
    public bool IsCollected { get; init; }

    /// <summary>Hint about how to obtain this stardrop.</summary>
    public string AcquisitionHint { get; init; } = string.Empty;
}

/// <summary>
/// Stardrop source constants matching mail flags.
/// </summary>
public static class StardropSource
{
    /// <summary>Stardew Valley Fair stardrop.</summary>
    public const string Fair = "Fair";
    /// <summary>Mine level 100 stardrop.</summary>
    public const string Mine = "Mine";
    /// <summary>Museum completion stardrop.</summary>
    public const string Museum = "Museum";
    /// <summary>Spouse/relationship stardrop.</summary>
    public const string Spouse = "Spouse";
    /// <summary>Master Angler achievement stardrop.</summary>
    public const string MasterAngler = "MasterAngler";
    /// <summary>Old Master Cannoli (Secret Woods statue) stardrop.</summary>
    public const string OldMasterCannoli = "OldMasterCannoli";
    /// <summary>Krobus relationship stardrop.</summary>
    public const string Krobus = "Krobus";
}
