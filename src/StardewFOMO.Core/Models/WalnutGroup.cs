namespace StardewFOMO.Core.Models;

/// <summary>
/// Golden walnut collection grouped by acquisition type.
/// </summary>
public sealed class WalnutGroup
{
    /// <summary>Type of acquisition method.</summary>
    public string GroupType { get; init; } = string.Empty;

    /// <summary>Display name for this group.</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Number of walnuts collected in this group.</summary>
    public int CollectedCount { get; init; }

    /// <summary>Total number of walnuts available in this group.</summary>
    public int TotalCount { get; init; }

    /// <summary>Whether all walnuts in this group have been collected.</summary>
    public bool IsComplete => CollectedCount >= TotalCount;

    /// <summary>Percentage complete for this group.</summary>
    public double PercentComplete => TotalCount > 0 ? (double)CollectedCount / TotalCount * 100.0 : 0.0;
}

/// <summary>
/// Walnut group type constants.
/// </summary>
public static class WalnutGroupType
{
    /// <summary>Walnuts found by digging.</summary>
    public const string Digging = "Digging";
    /// <summary>Walnuts from puzzle solutions.</summary>
    public const string Puzzles = "Puzzles";
    /// <summary>Walnuts from fishing.</summary>
    public const string Fishing = "Fishing";
    /// <summary>Walnuts from NPC interactions.</summary>
    public const string NPCs = "NPCs";
    /// <summary>Walnuts from combat/volcano.</summary>
    public const string Combat = "Combat";
    /// <summary>Walnuts from farming activities.</summary>
    public const string Farming = "Farming";
    /// <summary>Other miscellaneous walnuts.</summary>
    public const string Other = "Other";
}
