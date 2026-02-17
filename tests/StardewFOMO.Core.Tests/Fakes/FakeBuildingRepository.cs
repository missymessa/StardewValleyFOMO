using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IBuildingRepository"/> with configurable building status.</summary>
public sealed class FakeBuildingRepository : IBuildingRepository
{
    private readonly Dictionary<string, FarmBuilding> _buildings = new();

    public void SetBuilding(string buildingType, bool isBuilt, int cost = 0)
    {
        _buildings[buildingType] = new FarmBuilding
        {
            BuildingType = buildingType,
            DisplayName = buildingType,
            IsBuilt = isBuilt,
            Cost = cost
        };
    }

    public void SetBuildingBuilt(string buildingType, bool isBuilt) => SetBuilding(buildingType, isBuilt);

    public void SetAllBuilt()
    {
        var buildingTypes = new[] { "Earth Obelisk", "Water Obelisk", "Desert Obelisk", "Island Obelisk", "Gold Clock" };
        foreach (var type in buildingTypes)
        {
            SetBuilding(type, true, 0);
        }
    }

    public IReadOnlyList<FarmBuilding> GetAllPerfectionBuildings() => _buildings.Values.ToList().AsReadOnly();

    public bool HasBuilding(string buildingType) =>
        _buildings.TryGetValue(buildingType, out var building) && building.IsBuilt;

    public bool AreAllBuildingsComplete() => _buildings.Values.All(b => b.IsBuilt);

    public int GetBuiltCount() => _buildings.Values.Count(b => b.IsBuilt);

    public int GetTotalCount() => _buildings.Count > 0 ? _buildings.Count : 5;
}
