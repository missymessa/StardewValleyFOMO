namespace StardewFOMO.Core.Models;

/// <summary>
/// Farm building status for perfection tracking (obelisks and golden clock).
/// </summary>
public sealed class FarmBuilding
{
    /// <summary>Building type identifier.</summary>
    public string BuildingType { get; init; } = string.Empty;

    /// <summary>Display name for the building.</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Whether this building has been constructed.</summary>
    public bool IsBuilt { get; init; }

    /// <summary>Gold cost to construct this building.</summary>
    public int Cost { get; init; }
}

/// <summary>
/// Perfection building type constants.
/// </summary>
public static class PerfectionBuildingType
{
    /// <summary>Earth Obelisk building.</summary>
    public const string EarthObelisk = "Earth Obelisk";
    /// <summary>Water Obelisk building.</summary>
    public const string WaterObelisk = "Water Obelisk";
    /// <summary>Desert Obelisk building.</summary>
    public const string DesertObelisk = "Desert Obelisk";
    /// <summary>Island Obelisk building.</summary>
    public const string IslandObelisk = "Island Obelisk";
    /// <summary>Gold Clock building.</summary>
    public const string GoldenClock = "Gold Clock";
}
