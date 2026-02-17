using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="FishProgressService"/>.</summary>
public sealed class FishProgressServiceTests
{
    private readonly FakeCollectionRepository _collectionRepo;
    private readonly FakeFishRepository _fishRepo;
    private readonly TestLogger _logger;
    private readonly FishProgressService _service;

    public FishProgressServiceTests()
    {
        _collectionRepo = new FakeCollectionRepository();
        _fishRepo = new FakeFishRepository();
        _logger = new TestLogger();
        _service = new FishProgressService(_collectionRepo, _fishRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoFishCaught_ReturnsZeroCount()
    {
        // Add fish to the repository but don't mark them as caught
        _fishRepo.AddFish("Sardine");
        _fishRepo.AddFish("Anchovy");

        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_SomeFishCaught_ReturnsCorrectCount()
    {
        // Add fish to the repository
        _fishRepo.AddFish("Sardine");
        _fishRepo.AddFish("Anchovy");
        _fishRepo.AddFish("Pufferfish");

        // Mark some as caught
        _collectionRepo.AddCaughtFish("Sardine");
        _collectionRepo.AddCaughtFish("Anchovy");

        var progress = _service.GetProgress();

        Assert.Equal(2, progress.CurrentCount);
    }

    [Fact]
    public void GetProgress_CategoryWeight_Returns10Percent()
    {
        var progress = _service.GetProgress();

        Assert.Equal(10.0, progress.Weight);
    }

    [Fact]
    public void GetProgress_CategoryName_ReturnsFish()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Fish", progress.CategoryName);
    }
}
