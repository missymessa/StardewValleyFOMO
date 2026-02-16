using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="BundleAvailabilityService"/>.</summary>
public sealed class BundleAvailabilityServiceTests
{
    private readonly FakeBundleRepository _bundleRepo;
    private readonly FakeItemAvailabilityService _availabilityService;
    private readonly TestLogger _logger;
    private readonly BundleAvailabilityService _service;

    public BundleAvailabilityServiceTests()
    {
        _bundleRepo = new FakeBundleRepository();
        _availabilityService = new FakeItemAvailabilityService();
        _logger = new TestLogger();
        _service = new BundleAvailabilityService(_bundleRepo, _availabilityService, _logger);
    }

    [Fact]
    public void GetAvailableBundles_NoItems_ReturnsEmpty()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Test Bundle", new[] { "1" }));
        // No items set as available

        var result = _service.GetAvailableBundles();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAvailableBundles_HasAvailableItem_ReturnsBundle()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Spring Bundle", new[] { "24" })); // Parsnip
        _availabilityService.SetAvailable("24", "Parsnip");

        var result = _service.GetAvailableBundles();

        Assert.Single(result);
        Assert.Equal("Spring Bundle", result[0].BundleName);
    }

    [Fact]
    public void GetAvailableBundles_WrongSeason_ExcludesBundle()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Winter Bundle", new[] { "418" })); // Crocus
        _availabilityService.SetUnavailable("418", "Crocus", AvailabilityReason.WrongSeason, "Available in Winter");

        var result = _service.GetAvailableBundles();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAvailableBundles_MultipleItems_IncludesIfAnyAvailable()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Mixed Bundle", new[] { "24", "192", "418" }));
        _availabilityService.SetAvailable("24", "Parsnip");
        _availabilityService.SetUnavailable("192", "Potato", AvailabilityReason.WrongWeather);
        _availabilityService.SetUnavailable("418", "Crocus", AvailabilityReason.WrongSeason);

        var result = _service.GetAvailableBundles();

        // Should include because at least one item (Parsnip) is available
        Assert.Single(result);
    }

    [Fact]
    public void GetItemAvailabilityForBundle_ReturnsAllItems()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Test Bundle", new[] { "24", "192" }));
        _availabilityService.SetAvailable("24", "Parsnip");
        _availabilityService.SetUnavailable("192", "Potato", AvailabilityReason.WrongWeather);

        var result = _service.GetItemAvailabilityForBundle("Test Bundle");

        Assert.Equal(2, result.Count);
        Assert.True(result[0].IsAvailableToday);
        Assert.False(result[1].IsAvailableToday);
    }

    [Fact]
    public void IsBundleAvailableToday_AnyItemAvailable_ReturnsTrue()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Test Bundle", new[] { "24", "192" }));
        _availabilityService.SetAvailable("24", "Parsnip");
        // 192 not set - defaults to unavailable

        Assert.True(_service.IsBundleAvailableToday("Test Bundle"));
    }

    [Fact]
    public void IsBundleAvailableToday_NoItemsAvailable_ReturnsFalse()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Test Bundle", new[] { "24", "192" }));
        // No items set as available

        Assert.False(_service.IsBundleAvailableToday("Test Bundle"));
    }

    private static BundleInfo CreateSimpleBundle(string name, string[] itemIds)
    {
        var items = itemIds.Select((id, idx) => new BundleItem
        {
            ItemId = id,
            ItemName = $"Item {id}",
            Quantity = 1
        }).ToList();

        var slots = items.Select((item, idx) => new BundleSlot
        {
            SlotIndex = idx,
            ValidItems = new List<BundleItem> { item }.AsReadOnly(),
            IsFilled = false
        }).ToList();

        return new BundleInfo
        {
            BundleName = name,
            RoomName = "Test Room",
            RequiredItems = items.AsReadOnly(),
            CompletedItemIds = new HashSet<string>(),
            Slots = slots.AsReadOnly(),
            SlotsRequired = itemIds.Length
        };
    }
}
