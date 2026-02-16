using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts farm building data for perfection tracking (obelisks, golden clock).
/// Implemented by SMAPI adapter scanning farm buildings.
/// </summary>
public interface IBuildingRepository
{
    /// <summary>Get all perfection-related buildings with built status.</summary>
    IReadOnlyList<FarmBuilding> GetAllPerfectionBuildings();

    /// <summary>Check if a specific building type exists on the farm.</summary>
    bool HasBuilding(string buildingType);

    /// <summary>Check if all perfection buildings are constructed.</summary>
    bool AreAllBuildingsComplete();

    /// <summary>Get the count of perfection buildings that have been built.</summary>
    int GetBuiltCount();

    /// <summary>Get the total count of perfection buildings required (5).</summary>
    int GetTotalCount();
}
