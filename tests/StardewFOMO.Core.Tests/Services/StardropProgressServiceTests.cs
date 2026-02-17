using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="StardropProgressService"/>.</summary>
public sealed class StardropProgressServiceTests
{
    private readonly FakeStardropRepository _stardropRepo;
    private readonly TestLogger _logger;
    private readonly StardropProgressService _service;

    public StardropProgressServiceTests()
    {
        _stardropRepo = new FakeStardropRepository();
        _logger = new TestLogger();
        _service = new StardropProgressService(_stardropRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoStardropsCollected_ReturnsZeroCount()
    {
        // Add stardrops to the repository but don't mark them as collected
        _stardropRepo.SetStardropCollected("Fair", false);
        _stardropRepo.SetStardropCollected("Mines", false);
        _stardropRepo.SetStardropCollected("Krobus", false);

        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_SomeStardropsCollected_ReturnsCorrectCount()
    {
        // Add stardrops to the repo first
        _stardropRepo.SetStardropCollected("Fair", true);
        _stardropRepo.SetStardropCollected("Mines", true);
        _stardropRepo.SetStardropCollected("Krobus", false);

        var progress = _service.GetProgress();

        Assert.Equal(2, progress.CurrentCount);
    }

    [Fact]
    public void GetProgress_AllStardropsCollected_ReturnsComplete()
    {
        _stardropRepo.SetAllCollected();

        var progress = _service.GetProgress();

        Assert.True(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_CategoryWeight_Returns2Percent()
    {
        var progress = _service.GetProgress();

        Assert.Equal(2.0, progress.Weight);
    }

    [Fact]
    public void GetProgress_CategoryName_ReturnsStardrops()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Stardrops", progress.CategoryName);
    }
}
