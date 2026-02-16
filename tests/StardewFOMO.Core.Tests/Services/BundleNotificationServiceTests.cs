using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="BundleNotificationService"/>.</summary>
public sealed class BundleNotificationServiceTests
{
    private readonly FakeBundleRepository _bundleRepo;
    private readonly TestLogger _logger;
    private readonly BundleNotificationService _service;

    public BundleNotificationServiceTests()
    {
        _bundleRepo = new FakeBundleRepository();
        _logger = new TestLogger();
        _service = new BundleNotificationService(_bundleRepo, _logger);
    }

    [Fact]
    public void CheckForBundleItem_MatchingItem_ReturnsNotification()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Spring Bundle", new[] { ("24", "Parsnip") }));

        var result = _service.CheckForBundleItem("24", "Parsnip");

        Assert.NotNull(result);
        Assert.Contains("Parsnip", result);
        Assert.Contains("Spring Bundle", result);
    }

    [Fact]
    public void CheckForBundleItem_NonBundleItem_ReturnsNull()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Spring Bundle", new[] { ("24", "Parsnip") }));

        var result = _service.CheckForBundleItem("999", "Random Item");

        Assert.Null(result);
    }

    [Fact]
    public void CheckForBundleItem_SameItemTwice_SuppressesDuplicate()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Spring Bundle", new[] { ("24", "Parsnip") }));

        var first = _service.CheckForBundleItem("24", "Parsnip");
        var second = _service.CheckForBundleItem("24", "Parsnip");

        Assert.NotNull(first);
        Assert.Null(second); // Duplicate suppressed
    }

    [Fact]
    public void CheckForBundleItem_MultipleMatchingBundles_IncludesAll()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Bundle A", new[] { ("24", "Parsnip") }));
        _bundleRepo.AddBundle(CreateSimpleBundle("Bundle B", new[] { ("24", "Parsnip") }));

        var result = _service.CheckForBundleItem("24", "Parsnip");

        Assert.NotNull(result);
        // Message should include bundle info
        Assert.Contains("Parsnip", result);
    }

    [Fact]
    public void ResetSession_ClearsDuplicateTracking()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Spring Bundle", new[] { ("24", "Parsnip") }));

        var first = _service.CheckForBundleItem("24", "Parsnip");
        Assert.NotNull(first);

        _service.ResetSession();

        var afterReset = _service.CheckForBundleItem("24", "Parsnip");
        Assert.NotNull(afterReset); // Should notify again after reset
    }

    [Fact]
    public void CheckForBundleItem_CompletedBundle_DoesNotNotify()
    {
        var bundle = CreateCompletedBundle("Complete Bundle", new[] { ("24", "Parsnip") });
        _bundleRepo.AddBundle(bundle);

        var result = _service.CheckForBundleItem("24", "Parsnip");

        Assert.Null(result); // Bundle is complete, no notification
    }

    private static BundleInfo CreateSimpleBundle(string name, (string itemId, string itemName)[] items)
    {
        var bundleItems = items.Select((item, idx) => new BundleItem
        {
            ItemId = item.itemId,
            ItemName = item.itemName,
            Quantity = 1
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

    private static BundleInfo CreateCompletedBundle(string name, (string itemId, string itemName)[] items)
    {
        var bundleItems = items.Select((item, idx) => new BundleItem
        {
            ItemId = item.itemId,
            ItemName = item.itemName,
            Quantity = 1
        }).ToList();

        var slots = bundleItems.Select((item, idx) => new BundleSlot
        {
            SlotIndex = idx,
            ValidItems = new List<BundleItem> { item }.AsReadOnly(),
            IsFilled = true, // Already filled
            FilledWithItemId = item.ItemId
        }).ToList();

        return new BundleInfo
        {
            BundleName = name,
            RoomName = "Test Room",
            RequiredItems = bundleItems.AsReadOnly(),
            CompletedItemIds = items.Select(i => i.itemId).ToHashSet(),
            Slots = slots.AsReadOnly(),
            SlotsRequired = items.Length
        };
    }

    // Edge case tests for T036

    [Fact]
    public void CheckForBundleItem_NoBundles_ReturnsNull()
    {
        // No bundles added
        var result = _service.CheckForBundleItem("24", "Parsnip");

        Assert.Null(result);
    }

    [Fact]
    public void CheckForBundleItem_OrRequirementBundle_ReturnsNotification()
    {
        // Remixed bundle with OR requirements (any of the items can fill the slot)
        var bundle = CreateOrRequirementBundle("Remixed Bundle", 
            new[] { ("24", "Parsnip"), ("192", "Potato"), ("190", "Cauliflower") });
        _bundleRepo.AddBundle(bundle);

        // Any of the OR items should trigger notification
        var result = _service.CheckForBundleItem("192", "Potato");

        Assert.NotNull(result);
        Assert.Contains("Potato", result);
        Assert.Contains("Remixed Bundle", result);
    }

    [Fact]
    public void HasBeenNotified_AfterNotification_ReturnsTrue()
    {
        _bundleRepo.AddBundle(CreateSimpleBundle("Test Bundle", new[] { ("24", "Parsnip") }));

        Assert.False(_service.HasBeenNotified("24"));
        _service.CheckForBundleItem("24", "Parsnip");
        Assert.True(_service.HasBeenNotified("24"));
    }

    private static BundleInfo CreateOrRequirementBundle(string name, (string itemId, string itemName)[] orItems)
    {
        var bundleItems = orItems.Select((item, idx) => new BundleItem
        {
            ItemId = item.itemId,
            ItemName = item.itemName,
            Quantity = 1
        }).ToList();

        // Single slot that can be filled by any of the items (OR requirement)
        var slots = new List<BundleSlot>
        {
            new() 
            { 
                SlotIndex = 0, 
                ValidItems = bundleItems.AsReadOnly(), 
                IsFilled = false 
            }
        };

        return new BundleInfo
        {
            BundleName = name,
            RoomName = "Test Room",
            RequiredItems = bundleItems.AsReadOnly(),
            CompletedItemIds = new HashSet<string>(),
            Slots = slots.AsReadOnly(),
            SlotsRequired = 1
        };
    }
}
