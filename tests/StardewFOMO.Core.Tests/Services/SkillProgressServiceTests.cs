using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="SkillProgressService"/>.</summary>
public sealed class SkillProgressServiceTests
{
    private readonly FakeSkillRepository _skillRepo;
    private readonly TestLogger _logger;
    private readonly SkillProgressService _service;

    public SkillProgressServiceTests()
    {
        _skillRepo = new FakeSkillRepository();
        _logger = new TestLogger();
        _service = new SkillProgressService(_skillRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoSkillProgress_ReturnsZeroCount()
    {
        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_SomeSkillsMaxed_ReturnsCorrectCount()
    {
        _skillRepo.SetSkillLevel("Farming", 10);
        _skillRepo.SetSkillLevel("Mining", 5);

        var progress = _service.GetProgress();

        Assert.Equal(1, progress.CurrentCount);  // Only Farming at max
    }

    [Fact]
    public void GetProgress_AllSkillsMaxed_ReturnsComplete()
    {
        _skillRepo.SetAllSkillsMaxed();

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
    public void GetProgress_CategoryName_ReturnsSkills()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Skills", progress.CategoryName);
    }
}
