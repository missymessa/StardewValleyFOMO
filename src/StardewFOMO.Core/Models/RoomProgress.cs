namespace StardewFOMO.Core.Models;

/// <summary>
/// Represents the completion progress of a Community Center room.
/// </summary>
public sealed class RoomProgress
{
    /// <summary>The display name of the room (e.g., "Crafts Room", "Pantry").</summary>
    public string RoomName { get; init; } = string.Empty;

    /// <summary>Total number of bundles in this room.</summary>
    public int TotalBundles { get; init; }

    /// <summary>Number of bundles that have been completed.</summary>
    public int CompletedBundles { get; init; }

    /// <summary>Completion percentage (0-100).</summary>
    public int PercentComplete => TotalBundles > 0 
        ? (int)Math.Round(CompletedBundles * 100.0 / TotalBundles) 
        : 0;

    /// <summary>Whether all bundles in this room are complete.</summary>
    public bool IsComplete => TotalBundles > 0 && CompletedBundles >= TotalBundles;
}
