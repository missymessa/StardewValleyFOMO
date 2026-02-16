namespace StardewFOMO.Core.Models;

/// <summary>
/// Monster eradication goal for Adventurer's Guild tracking.
/// </summary>
public sealed class MonsterGoal
{
    /// <summary>Monster category name (e.g., "Slimes", "Void Spirits", "Bats").</summary>
    public string MonsterCategory { get; init; } = string.Empty;

    /// <summary>Display name for the eradication goal.</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Current number of kills for this category.</summary>
    public int CurrentKills { get; init; }

    /// <summary>Required number of kills to complete this goal.</summary>
    public int RequiredKills { get; init; }

    /// <summary>Whether this goal is complete.</summary>
    public bool IsComplete => CurrentKills >= RequiredKills;

    /// <summary>Percentage complete for this goal.</summary>
    public double PercentComplete => RequiredKills > 0 ? Math.Min(100.0, (double)CurrentKills / RequiredKills * 100.0) : 0.0;

    /// <summary>Locations where these monsters spawn.</summary>
    public IReadOnlyList<string> SpawnLocations { get; init; } = Array.Empty<string>();
}
