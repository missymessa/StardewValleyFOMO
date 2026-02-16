using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="BundleItemMatcherService"/>.</summary>
public sealed class BundleItemMatcherServiceTests
{
    private readonly FakeStorageScanner _storageScanner;
    private readonly FakeBundleRepository _bundleRepo;
    private readonly TestLogger _logger;
    private readonly BundleItemMatcherService _service;

    public BundleItemMatcherServiceTests()
    {
        _storageScanner = new FakeStorageScanner();
        _bundleRepo = new FakeBundleRepository();
        _logger = new TestLogger();
        _service = new BundleItemMatcherService(_storageScanner, _bundleRepo, _logger);
    }

    [Fact]
    public void GetOwnedBundleItems_NoItems_ReturnsEmpty()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Test Bundle", new[] { ("1", "Parsnip", 0) }));

        var result = _service.GetOwnedBundleItems("Test Bundle");

        Assert.Empty(result);
    }

    [Fact]
    public void GetOwnedBundleItems_HasMatchingItem_ReturnsMatch()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Test Bundle", new[] { ("24", "Parsnip", 0) }));
        _storageScanner.AddItem("24", "Parsnip", quantity: 5, quality: 0, ItemLocation.Inventory);

        var result = _service.GetOwnedBundleItems("Test Bundle");

        Assert.Single(result);
        Assert.Equal("24", result[0].ItemId);
        Assert.Equal(ItemLocation.Inventory, result[0].Location);
    }

    [Fact]
    public void GetOwnedBundleItems_QualityRequirement_MatchesHigherQuality()
    {
        // Requires Gold quality (2)
        _bundleRepo.AddBundle(CreateSimpleBundle("Quality Bundle", new[] { ("24", "Parsnip", 2) }));
        // Has Iridium quality (3) - should match
        _storageScanner.AddItem("24", "Parsnip", quantity: 1, quality: 3, ItemLocation.Inventory);

        var result = _service.GetOwnedBundleItems("Quality Bundle");

        Assert.Single(result);
        Assert.Equal(3, result[0].Quality);
    }

    [Fact]
    public void GetOwnedBundleItems_QualityRequirement_DoesNotMatchLowerQuality()
    {
        // Requires Gold quality (2)
        _bundleRepo.AddBundle(CreateSimpleBundle("Quality Bundle", new[] { ("24", "Parsnip", 2) }));
        // Has Silver quality (1) - should NOT match
        _storageScanner.AddItem("24", "Parsnip", quantity: 1, quality: 1, ItemLocation.Inventory);

        var result = _service.GetOwnedBundleItems("Quality Bundle");

        Assert.Empty(result);
    }

    [Fact]
    public void GetOwnedBundleItems_OrRequirement_MatchesAnyValidItem()
    {
        var bundle = CreateBundleWithOrRequirement("Spring Crops");
        _bundleRepo.AddBundle(bundle);
        // Has Potato (192) - one of the valid OR items
        _storageScanner.AddItem("192", "Potato", quantity: 1, quality: 0, ItemLocation.Farmhouse);

        var result = _service.GetOwnedBundleItems("Spring Crops");

        Assert.Single(result);
        Assert.Equal("192", result[0].ItemId);
        Assert.Equal(ItemLocation.Farmhouse, result[0].Location);
    }

    [Fact]
    public void GetReadyItemCount_ReturnsCorrectCounts()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Test Bundle", new[] 
        { 
            ("24", "Parsnip", 0),
            ("192", "Potato", 0),
            ("190", "Cauliflower", 0)
        }));
        _storageScanner.AddItem("24", "Parsnip", quantity: 5, location: ItemLocation.Inventory);
        _storageScanner.AddItem("190", "Cauliflower", quantity: 1, location: ItemLocation.Farmhouse);

        var (ready, total) = _service.GetReadyItemCount("Test Bundle");

        Assert.Equal(2, ready); // Have Parsnip and Cauliflower
        Assert.Equal(3, total); // Need 3 items total
    }

    [Fact]
    public void HasItemForSlot_ReturnsTrueWhenItemOwned()
    {
        _storageScanner.AddItem("24", "Parsnip", quantity: 5, quality: 2, ItemLocation.Inventory);

        var slot = new BundleSlot
        {
            SlotIndex = 0,
            ValidItems = new List<BundleItem>
            {
                new() { ItemId = "24", ItemName = "Parsnip", Quantity = 1, MinimumQuality = 2 }
            }.AsReadOnly(),
            IsFilled = false
        };

        Assert.True(_service.HasItemForSlot(slot));
    }

    [Fact]
    public void HasItemForSlot_ReturnsFalseWhenItemNotOwned()
    {
        // Storage is empty

        var slot = new BundleSlot
        {
            SlotIndex = 0,
            ValidItems = new List<BundleItem>
            {
                new() { ItemId = "24", ItemName = "Parsnip", Quantity = 1, MinimumQuality = 0 }
            }.AsReadOnly(),
            IsFilled = false
        };

        Assert.False(_service.HasItemForSlot(slot));
    }

    private static BundleInfo CreateSimpleBundle(string name, (string itemId, string itemName, int quality)[] items)
    {
        var bundleItems = items.Select((item, idx) => new BundleItem 
        { 
            ItemId = item.itemId, 
            ItemName = item.itemName, 
            Quantity = 1, 
            MinimumQuality = item.quality 
        }).ToList();

        var slots = bundleItems.Select((item, idx) => new BundleSlot
        {
            SlotIndex = idx,
            ValidItems = new List<BundleItem> { item }.AsReadOnly(),
            IsFilled = false
        }).ToList();

        return new BundleInfo
        {
            BundleName = name,
            RoomName = "Test Room",
            RequiredItems = bundleItems.AsReadOnly(),
            CompletedItemIds = new HashSet<string>(),
            Slots = slots.AsReadOnly(),
            SlotsRequired = items.Length
        };
    }

    private static BundleInfo CreateBundleWithOrRequirement(string name)
    {
        var orItems = new List<BundleItem>
        {
            new() { ItemId = "24", ItemName = "Parsnip", Quantity = 1 },
            new() { ItemId = "192", ItemName = "Potato", Quantity = 1 },
            new() { ItemId = "190", ItemName = "Cauliflower", Quantity = 1 }
        };

        var slots = new List<BundleSlot>
        {
            new()
            {
                SlotIndex = 0,
                ValidItems = orItems.AsReadOnly(),
                IsFilled = false
            }
        };

        return new BundleInfo
        {
            BundleName = name,
            RoomName = "Pantry",
            RequiredItems = orItems.AsReadOnly(),
            CompletedItemIds = new HashSet<string>(),
            Slots = slots.AsReadOnly(),
            SlotsRequired = 1
        };
    }
}
