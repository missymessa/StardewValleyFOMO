using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="FriendshipProgressService"/>.</summary>
public sealed class FriendshipProgressServiceTests
{
    private readonly FakeFriendshipRepository _friendshipRepo;
    private readonly TestLogger _logger;
    private readonly FriendshipProgressService _service;

    public FriendshipProgressServiceTests()
    {
        _friendshipRepo = new FakeFriendshipRepository();
        _logger = new TestLogger();
        _service = new FriendshipProgressService(_friendshipRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoFriendships_ReturnsZeroCount()
    {
        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.Equal(0, progress.TotalCount);
    }

    [Fact]
    public void GetProgress_SomeMaxFriendships_ReturnsCorrectCount()
    {
        _friendshipRepo.SetFriendship("Alex", 8, 10);
        _friendshipRepo.SetFriendship("Emily", 10, 10);  // Max

        var progress = _service.GetProgress();

        Assert.Equal(1, progress.CurrentCount);  // Only Emily at max
        Assert.Equal(2, progress.TotalCount);
    }

    [Fact]
    public void GetProgress_AllMaxFriendships_ReturnsComplete()
    {
        _friendshipRepo.SetFriendship("Alex", 10, 10);
        _friendshipRepo.SetFriendship("Emily", 10, 10);

        var progress = _service.GetProgress();

        Assert.True(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_CategoryWeight_Returns11Percent()
    {
        var progress = _service.GetProgress();

        Assert.Equal(11.0, progress.Weight);
    }

    [Fact]
    public void GetProgress_CategoryName_ReturnsFriendship()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Friendship", progress.CategoryName);
    }
}
