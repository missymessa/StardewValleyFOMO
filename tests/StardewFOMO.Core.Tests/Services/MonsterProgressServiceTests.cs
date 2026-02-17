using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="MonsterProgressService"/>.</summary>
public sealed class MonsterProgressServiceTests
{
    private readonly FakeMonsterRepository _monsterRepo;
    private readonly TestLogger _logger;
    private readonly MonsterProgressService _service;

    public MonsterProgressServiceTests()
    {
        _monsterRepo = new FakeMonsterRepository();
        _logger = new TestLogger();
        _service = new MonsterProgressService(_monsterRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoMonstersSlain_ReturnsZeroCount()
    {
        // Add monster goals to the repository but don't complete them
        _monsterRepo.SetGoalComplete("Slimes", false);
        _monsterRepo.SetGoalComplete("Bats", false);
        _monsterRepo.SetGoalComplete("Skeletons", false);

        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_SomeGoalsComplete_ReturnsCorrectCount()
    {
        _monsterRepo.SetGoalComplete("Slimes", true);
        _monsterRepo.SetGoalComplete("Bats", false);
        _monsterRepo.SetGoalComplete("Skeletons", false);

        var progress = _service.GetProgress();

        Assert.Equal(1, progress.CurrentCount);
    }

    [Fact]
    public void GetProgress_AllGoalsComplete_ReturnsComplete()
    {
        _monsterRepo.SetAllGoalsComplete();

        var progress = _service.GetProgress();

        Assert.True(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_CategoryWeight_Returns10Percent()
    {
        var progress = _service.GetProgress();

        Assert.Equal(10.0, progress.Weight);
    }

    [Fact]
    public void GetProgress_CategoryName_ReturnsMonsterSlayer()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Monster Slayer", progress.CategoryName);
    }
}
