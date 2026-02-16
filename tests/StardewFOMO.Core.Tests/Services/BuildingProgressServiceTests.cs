using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="BuildingProgressService"/>.</summary>
public sealed class BuildingProgressServiceTests
{
    private readonly FakeBuildingRepository _buildingRepo;
    private readonly TestLogger _logger;
    private readonly BuildingProgressService _service;

    public BuildingProgressServiceTests()
    {
        _buildingRepo = new FakeBuildingRepository();
        _logger = new TestLogger();
        _service = new BuildingProgressService(_buildingRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoBuildingsBuilt_ReturnsZeroCount()
    {
        // Add buildings to the repository but don't mark them as built
        _buildingRepo.SetBuildingBuilt("Earth Obelisk", false);
        _buildingRepo.SetBuildingBuilt("Water Obelisk", false);
        _buildingRepo.SetBuildingBuilt("Gold Clock", false);

        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_SomeBuildingsBuilt_ReturnsCorrectCount()
    {
        _buildingRepo.SetBuildingBuilt("Earth Obelisk", true);
        _buildingRepo.SetBuildingBuilt("Water Obelisk", false);
        _buildingRepo.SetBuildingBuilt("Gold Clock", false);

        var progress = _service.GetProgress();

        Assert.Equal(1, progress.CurrentCount);
    }

    [Fact]
    public void GetProgress_AllBuildingsBuilt_ReturnsComplete()
    {
        _buildingRepo.SetAllBuilt();

        var progress = _service.GetProgress();

        Assert.True(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_CategoryWeight_Returns25Percent()
    {
        var progress = _service.GetProgress();

        Assert.Equal(25.0, progress.Weight);
    }

    [Fact]
    public void GetProgress_CategoryName_ReturnsBuildings()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Buildings", progress.CategoryName);
    }
}
