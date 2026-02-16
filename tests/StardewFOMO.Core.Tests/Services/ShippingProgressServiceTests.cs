using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="ShippingProgressService"/>.</summary>
public sealed class ShippingProgressServiceTests
{
    private readonly FakeCollectionRepository _collectionRepo;
    private readonly TestLogger _logger;
    private readonly ShippingProgressService _service;

    public ShippingProgressServiceTests()
    {
        _collectionRepo = new FakeCollectionRepository();
        _logger = new TestLogger();
        _service = new ShippingProgressService(_collectionRepo, _logger);
    }

    [Fact]
    public void GetProgress_NoItemsShipped_ReturnsZeroPercent()
    {
        var progress = _service.GetProgress();

        Assert.Equal(0, progress.CurrentCount);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void GetProgress_SomeItemsShipped_ReturnsCorrectCount()
    {
        _collectionRepo.AddShippedItem("Parsnip");
        _collectionRepo.AddShippedItem("Potato");

        var progress = _service.GetProgress();

        Assert.Equal(2, progress.CurrentCount);
    }

    [Fact]
    public void GetProgress_CategoryWeight_Returns15Percent()
    {
        var progress = _service.GetProgress();

        Assert.Equal(15.0, progress.Weight);
    }

    [Fact]
    public void GetProgress_CategoryName_ReturnsShipping()
    {
        var progress = _service.GetProgress();

        Assert.Equal("Shipping", progress.CategoryName);
    }

    [Fact]
    public void GetUnshippedItems_ReturnsItemsNotYetShipped()
    {
        // Simulate shipping some items
        _collectionRepo.AddShippedItem("Parsnip");

        var unshipped = _service.GetUnshippedItems();

        // Should not contain Parsnip
        Assert.DoesNotContain(unshipped, item => item.ItemId == "Parsnip");
    }
}
