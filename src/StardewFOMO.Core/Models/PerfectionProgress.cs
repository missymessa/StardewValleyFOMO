namespace StardewFOMO.Core.Models;

/// <summary>
/// Overall perfection tracking state containing all categories and computed total percentage.
/// </summary>
public sealed class PerfectionProgress
{
    /// <summary>Overall perfection percentage (0-100).</summary>
    public double TotalPercentage { get; init; }

    /// <summary>All perfection categories with their individual progress.</summary>
    public IReadOnlyList<PerfectionCategory> Categories { get; init; } = Array.Empty<PerfectionCategory>();

    /// <summary>Whether 100% perfection has been achieved.</summary>
    public bool IsComplete => TotalPercentage >= 100.0;

    /// <summary>Whether Ginger Island has been unlocked.</summary>
    public bool GingerIslandUnlocked { get; init; }
}
