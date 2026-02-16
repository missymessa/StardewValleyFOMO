using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking required buildings toward perfection.
/// </summary>
public sealed class BuildingProgressService
{
    private readonly IBuildingRepository _buildingRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (25%)
    private const double PerfectionWeight = 25.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingProgressService"/> class.
    /// </summary>
    public BuildingProgressService(IBuildingRepository buildingRepo, ILogger logger)
    {
        _buildingRepo = buildingRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the building progress category for perfection tracking.
    /// </summary>
    public PerfectionCategory GetProgress()
    {
        var buildings = _buildingRepo.GetAllPerfectionBuildings();
        var builtCount = buildings.Count(b => b.IsBuilt);
        var totalCount = buildings.Count;

        _logger.Log(LogLevel.Trace, $"Building progress: {builtCount}/{totalCount}");

        return new PerfectionCategory
        {
            CategoryName = "Buildings",
            CurrentCount = builtCount,
            TotalCount = totalCount,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets buildings that have not yet been built.
    /// </summary>
    public IReadOnlyList<FarmBuilding> GetUnbuiltBuildings()
    {
        return _buildingRepo.GetAllPerfectionBuildings()
            .Where(b => !b.IsBuilt)
            .ToList()
            .AsReadOnly();
    }
}
