using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

/// <summary>Tests for <see cref="BundleProgressService"/>.</summary>
public sealed class BundleProgressServiceTests
{
    private readonly FakeBundleRepository _bundleRepo;
    private readonly TestLogger _logger;
    private readonly BundleProgressService _service;

    public BundleProgressServiceTests()
    {
        _bundleRepo = new FakeBundleRepository();
        _logger = new TestLogger();
        _service = new BundleProgressService(_bundleRepo, _logger);
    }

    [Fact]
    public void GetOverallProgress_EmptyBundles_ReturnsZero()
    {
        var progress = _service.GetOverallProgress();

        Assert.Equal(0, progress.PercentComplete);
        Assert.Equal(0, progress.TotalBundles);
        Assert.Equal(0, progress.CompletedBundles);
    }

    [Fact]
    public void GetOverallProgress_AllComplete_Returns100Percent()
    {
        _bundleRepo.AddBundle(CreateBundle("Bundle1", "Crafts Room", isComplete: true));
        _bundleRepo.AddBundle(CreateBundle("Bundle2", "Pantry", isComplete: true));

        var progress = _service.GetOverallProgress();

        Assert.Equal(100, progress.PercentComplete);
        Assert.Equal(2, progress.TotalBundles);
        Assert.Equal(2, progress.CompletedBundles);
    }

    [Fact]
    public void GetOverallProgress_PartialComplete_ReturnsCorrectPercentage()
    {
        _bundleRepo.AddBundle(CreateBundle("Bundle1", "Crafts Room", isComplete: true));
        _bundleRepo.AddBundle(CreateBundle("Bundle2", "Pantry", isComplete: false));

        var progress = _service.GetOverallProgress();

        Assert.Equal(50, progress.PercentComplete);
        Assert.Equal(2, progress.TotalBundles);
        Assert.Equal(1, progress.CompletedBundles);
    }

    [Fact]
    public void GetRoomProgressList_ReturnsAllRoomsWithProgress()
    {
        _bundleRepo.AddBundle(CreateBundle("Bundle1", "Crafts Room", isComplete: true));
        _bundleRepo.AddBundle(CreateBundle("Bundle2", "Crafts Room", isComplete: false));
        _bundleRepo.AddBundle(CreateBundle("Bundle3", "Pantry", isComplete: true));

        var rooms = _service.GetRoomProgressList();

        Assert.Equal(2, rooms.Count);
        
        var craftsRoom = rooms.First(r => r.RoomName == "Crafts Room");
        Assert.Equal(2, craftsRoom.TotalBundles);
        Assert.Equal(1, craftsRoom.CompletedBundles);
        Assert.Equal(50, craftsRoom.PercentComplete);

        var pantry = rooms.First(r => r.RoomName == "Pantry");
        Assert.Equal(1, pantry.TotalBundles);
        Assert.Equal(1, pantry.CompletedBundles);
        Assert.Equal(100, pantry.PercentComplete);
    }

    [Fact]
    public void GetBundleCountsForRoom_ReturnsCorrectCounts()
    {
        _bundleRepo.AddBundle(CreateBundleWithItems("Bundle1", "Crafts Room", 3, 1));
        _bundleRepo.AddBundle(CreateBundleWithItems("Bundle2", "Crafts Room", 5, 5));

        var bundles = _service.GetBundleCountsForRoom("Crafts Room");

        Assert.Equal(2, bundles.Count);
        
        var bundle1 = bundles.First(b => b.BundleName == "Bundle1");
        Assert.Equal(3, bundle1.TotalItems);
        Assert.Equal(1, bundle1.CompletedItems);
        Assert.False(bundle1.IsComplete);

        var bundle2 = bundles.First(b => b.BundleName == "Bundle2");
        Assert.Equal(5, bundle2.TotalItems);
        Assert.Equal(5, bundle2.CompletedItems);
        Assert.True(bundle2.IsComplete);
    }

    [Fact]
    public void IsCommunityComplete_AllBundlesComplete_ReturnsTrue()
    {
        _bundleRepo.AddBundle(CreateBundle("Bundle1", "Crafts Room", isComplete: true));
        _bundleRepo.AddBundle(CreateBundle("Bundle2", "Pantry", isComplete: true));

        Assert.True(_service.IsCommunityComplete());
    }

    [Fact]
    public void IsCommunityComplete_SomeBundlesIncomplete_ReturnsFalse()
    {
        _bundleRepo.AddBundle(CreateBundle("Bundle1", "Crafts Room", isComplete: true));
        _bundleRepo.AddBundle(CreateBundle("Bundle2", "Pantry", isComplete: false));

        Assert.False(_service.IsCommunityComplete());
    }

    // US2 Tests - Item requirements with quality and OR requirements

    [Fact]
    public void GetBundleDetails_ReturnsRemainingSlots()
    {
        _bundleRepo.AddBundle(CreateBundleWithItems("Test Bundle", "Crafts Room", 3, 1));

        var details = _service.GetBundleDetails("Test Bundle");

        Assert.NotNull(details);
        Assert.Equal("Test Bundle", details.BundleName);
        Assert.Equal(2, details.RemainingSlots.Count); // 3 total - 1 filled = 2 remaining
    }

    [Fact]
    public void GetBundleDetails_IncludesQualityRequirements()
    {
        var bundle = CreateBundleWithQuality("Quality Bundle", "Pantry", 2); // Gold quality
        _bundleRepo.AddBundle(bundle);

        var details = _service.GetBundleDetails("Quality Bundle");

        Assert.NotNull(details);
        Assert.Single(details.RemainingSlots);
        var slot = details.RemainingSlots[0];
        Assert.Equal(2, slot.PrimaryItem?.MinimumQuality); // Gold quality
    }

    [Fact]
    public void GetBundleDetails_IncludesOrRequirements()
    {
        var bundle = CreateBundleWithOrRequirement("Spring Crops", "Pantry");
        _bundleRepo.AddBundle(bundle);

        var details = _service.GetBundleDetails("Spring Crops");

        Assert.NotNull(details);
        Assert.Single(details.RemainingSlots);
        var slot = details.RemainingSlots[0];
        Assert.True(slot.HasOrRequirement);
        Assert.Equal(3, slot.ValidItems.Count); // Parsnip, Potato, or Cauliflower
    }

    [Fact]
    public void GetBundleDetails_NonExistentBundle_ReturnsNull()
    {
        var details = _service.GetBundleDetails("Non-Existent Bundle");

        Assert.Null(details);
    }

    private static BundleInfo CreateBundleWithQuality(string name, string room, int quality)
    {
        var items = new List<BundleItem>
        {
            new() { ItemId = "24", ItemName = "Parsnip", Quantity = 5, MinimumQuality = quality }
        };
        var slots = new List<BundleSlot>
        {
            new() 
            { 
                SlotIndex = 0, 
                ValidItems = items.AsReadOnly(), 
                IsFilled = false
            }
        };
        return new BundleInfo
        {
            BundleName = name,
            RoomName = room,
            RequiredItems = items.AsReadOnly(),
            CompletedItemIds = new HashSet<string>(),
            Slots = slots.AsReadOnly(),
            SlotsRequired = 1
        };
    }

    private static BundleInfo CreateBundleWithOrRequirement(string name, string room)
    {
        // Create an OR requirement slot - any of these items satisfies the slot
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
            RoomName = room,
            RequiredItems = orItems.AsReadOnly(),
            CompletedItemIds = new HashSet<string>(),
            Slots = slots.AsReadOnly(),
            SlotsRequired = 1
        };
    }

    private static BundleInfo CreateBundle(string name, string room, bool isComplete)
    {
        var items = new List<BundleItem>
        {
            new() { ItemId = "1", ItemName = "Item 1", Quantity = 1 }
        };
        var completed = isComplete ? new HashSet<string> { "1" } : new HashSet<string>();
        
        // Create slot to track completion via Slots property
        var slots = new List<BundleSlot>
        {
            new() 
            { 
                SlotIndex = 0, 
                ValidItems = items.AsReadOnly(), 
                IsFilled = isComplete, 
                FilledWithItemId = isComplete ? "1" : null 
            }
        };
        
        return new BundleInfo
        {
            BundleName = name,
            RoomName = room,
            RequiredItems = items.AsReadOnly(),
            CompletedItemIds = completed,
            Slots = slots.AsReadOnly(),
            SlotsRequired = 1
        };
    }

    private static BundleInfo CreateBundleWithItems(string name, string room, int totalItems, int completedItems)
    {
        var items = Enumerable.Range(1, totalItems)
            .Select(i => new BundleItem { ItemId = i.ToString(), ItemName = $"Item {i}", Quantity = 1 })
            .ToList();
        
        var completed = Enumerable.Range(1, completedItems)
            .Select(i => i.ToString())
            .ToHashSet();

        // Create slots with proper filled state
        var slots = Enumerable.Range(1, totalItems)
            .Select(i => new BundleSlot
            {
                SlotIndex = i - 1,
                ValidItems = new List<BundleItem> { items[i - 1] }.AsReadOnly(),
                IsFilled = i <= completedItems,
                FilledWithItemId = i <= completedItems ? i.ToString() : null
            })
            .ToList();

        return new BundleInfo
        {
            BundleName = name,
            RoomName = room,
            RequiredItems = items.AsReadOnly(),
            CompletedItemIds = completed,
            Slots = slots.AsReadOnly(),
            SlotsRequired = totalItems
        };
    }
}
