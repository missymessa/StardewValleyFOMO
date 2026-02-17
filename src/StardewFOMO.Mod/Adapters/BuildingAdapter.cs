using StardewValley;
using StardewValley.Buildings;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IBuildingRepository"/> by scanning farm buildings.
/// </summary>
public sealed class BuildingAdapter : IBuildingRepository
{
    // Perfection buildings with their internal names and costs
    private static readonly (string Type, string InternalName, string DisplayName, int Cost)[] PerfectionBuildings =
    {
        (PerfectionBuildingType.EarthObelisk, "Earth Obelisk", "Earth Obelisk", 500000),
        (PerfectionBuildingType.WaterObelisk, "Water Obelisk", "Water Obelisk", 500000),
        (PerfectionBuildingType.DesertObelisk, "Desert Obelisk", "Desert Obelisk", 1000000),
        (PerfectionBuildingType.IslandObelisk, "Island Obelisk", "Island Obelisk", 1000000),
        (PerfectionBuildingType.GoldenClock, "Gold Clock", "Gold Clock", 10000000)
    };

    /// <inheritdoc/>
    public IReadOnlyList<FarmBuilding> GetAllPerfectionBuildings()
    {
        var builtBuildings = GetBuiltBuildingNames();
        var buildings = new List<FarmBuilding>();

        foreach (var (type, internalName, displayName, cost) in PerfectionBuildings)
        {
            buildings.Add(new FarmBuilding
            {
                BuildingType = type,
                DisplayName = displayName,
                IsBuilt = builtBuildings.Contains(internalName),
                Cost = cost
            });
        }

        return buildings.AsReadOnly();
    }

    /// <inheritdoc/>
    public bool HasBuilding(string buildingType)
    {
        var builtBuildings = GetBuiltBuildingNames();
        var definition = PerfectionBuildings.FirstOrDefault(b => b.Type == buildingType);
        return definition.InternalName != null && builtBuildings.Contains(definition.InternalName);
    }

    /// <inheritdoc/>
    public bool AreAllBuildingsComplete()
    {
        var builtBuildings = GetBuiltBuildingNames();
        return PerfectionBuildings.All(b => builtBuildings.Contains(b.InternalName));
    }

    /// <inheritdoc/>
    public int GetBuiltCount()
    {
        var builtBuildings = GetBuiltBuildingNames();
        return PerfectionBuildings.Count(b => builtBuildings.Contains(b.InternalName));
    }

    /// <inheritdoc/>
    public int GetTotalCount() => PerfectionBuildings.Length;

    private static HashSet<string> GetBuiltBuildingNames()
    {
        var builtNames = new HashSet<string>();

        var farm = Game1.getFarm();
        if (farm == null)
            return builtNames;

        foreach (var building in farm.buildings)
        {
            if (!string.IsNullOrEmpty(building.buildingType.Value))
            {
                builtNames.Add(building.buildingType.Value);
            }
        }

        return builtNames;
    }
}
