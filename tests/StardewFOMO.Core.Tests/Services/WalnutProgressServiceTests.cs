using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="WalnutProgressService"/>.</summary>
public sealed class WalnutProgressServiceTests
{
    private readonly FakeWalnutRepository _walnutRepo;
    private readonly FakePerfectionRepository _perfectionRepo;
    private readonly TestLogger _logger;
    private readonly WalnutProgressService _service;

    public WalnutProgressServiceTests()
    {
        _walnutRepo = new FakeWalnutRepository();
        _perfectionRepo = new FakePerfectionRepository();
        _logger = new TestLogger();
        _service = new WalnutProgressService(_walnutRepo, _perfectionRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoWalnutsFound_ReturnsZeroCount()
    {
        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_SomeWalnutsFound_ReturnsCorrectCount()
    {
        _perfectionRepo.SetWalnutCount(50);

        var progress = _service.GetProgress();

        Assert.Equal(50, progress.CurrentCount);
    }

    [Fact]
    public void GetProgress_AllWalnutsFound_ReturnsComplete()
    {
        _perfectionRepo.SetWalnutCount(130);

        var progress = _service.GetProgress();

        Assert.True(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_CategoryWeight_Returns5Percent()
    {
        var progress = _service.GetProgress();

        Assert.Equal(5.0, progress.Weight);
    }

    [Fact]
    public void GetProgress_CategoryName_ReturnsGoldenWalnuts()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Golden Walnuts", progress.CategoryName);
    }
}
