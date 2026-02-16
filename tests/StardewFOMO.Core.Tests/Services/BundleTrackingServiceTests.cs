using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

public class BundleTrackingServiceTests
{
    private readonly FakeBundleRepository _bundleRepo = new();
    private readonly FakeInventoryProvider _inventoryProvider = new();
    private readonly TestLogger _logger = new();

    private BundleTrackingService CreateService() =>
        new(_bundleRepo, _inventoryProvider, _logger);

    #region Basic Bundle Matching

    [Fact]
    public void AnnotateWithBundleInfo_ItemNeededForBundle_BundleNameAdded()
    {
        _bundleRepo.AddBundle(new BundleInfo
        {
            BundleName = "Spring Foraging Bundle",
            RoomName = "Crafts Room",
            RequiredItems = new[] { new BundleItem { ItemId = "leek", ItemName = "Leek" } },
            CompletedItemIds = new HashSet<string>()
        });

        var items = new List<CollectibleItem>
        {
            new() { Id = "leek", Name = "Leek", CollectionType = CollectionType.Forage }
        };

        var result = CreateService().AnnotateWithBundleInfo(items);

        Assert.Single(result);
        Assert.Contains("Spring Foraging Bundle", result[0].BundleNames);
    }

    [Fact]
    public void AnnotateWithBundleInfo_ItemNotNeededForAnyBundle_NoBundleNames()
    {
        _bundleRepo.AddBundle(new BundleInfo
        {
            BundleName = "Spring Foraging Bundle",
            RoomName = "Crafts Room",
            RequiredItems = new[] { new BundleItem { ItemId = "leek", ItemName = "Leek" } },
            CompletedItemIds = new HashSet<string>()
        });

        var items = new List<CollectibleItem>
        {
            new() { Id = "sunfish", Name = "Sunfish", CollectionType = CollectionType.Fish }
        };

        var result = CreateService().AnnotateWithBundleInfo(items);

        Assert.Single(result);
        Assert.Empty(result[0].BundleNames);
    }

    #endregion

    #region Multiple Bundles

    [Fact]
    public void AnnotateWithBundleInfo_MultipleBundlesNeedSameItem_AllBundleNamesShown()
    {
        _bundleRepo.AddBundle(new BundleInfo
        {
            BundleName = "Quality Crops Bundle",
            RoomName = "Pantry",
            RequiredItems = new[] { new BundleItem { ItemId = "pumpkin", ItemName = "Pumpkin" } },
            CompletedItemIds = new HashSet<string>()
        });
        _bundleRepo.AddBundle(new BundleInfo
        {
            BundleName = "Fall Crops Bundle",
            RoomName = "Pantry",
            RequiredItems = new[] { new BundleItem { ItemId = "pumpkin", ItemName = "Pumpkin" } },
            CompletedItemIds = new HashSet<string>()
        });

        var items = new List<CollectibleItem>
        {
            new() { Id = "pumpkin", Name = "Pumpkin", CollectionType = CollectionType.ShippableItem }
        };

        var result = CreateService().AnnotateWithBundleInfo(items);

        Assert.Single(result);
        Assert.Equal(2, result[0].BundleNames.Count);
        Assert.Contains("Quality Crops Bundle", result[0].BundleNames);
        Assert.Contains("Fall Crops Bundle", result[0].BundleNames);
    }

    #endregion

    #region Completed Bundles

    [Fact]
    public void AnnotateWithBundleInfo_BundleAlreadyComplete_ItemNotFlagged()
    {
        _bundleRepo.AddBundle(new BundleInfo
        {
            BundleName = "Spring Foraging Bundle",
            RoomName = "Crafts Room",
            RequiredItems = new[] { new BundleItem { ItemId = "leek", ItemName = "Leek" } },
            CompletedItemIds = new HashSet<string> { "leek" } // Already completed
        });

        var items = new List<CollectibleItem>
        {
            new() { Id = "leek", Name = "Leek", CollectionType = CollectionType.Forage }
        };

        var result = CreateService().AnnotateWithBundleInfo(items);

        Assert.Single(result);
        Assert.Empty(result[0].BundleNames);
    }

    #endregion

    #region Joja Route

    [Fact]
    public void AnnotateWithBundleInfo_JojaRoute_NoBundleAnnotations()
    {
        _bundleRepo.SetCommunityCenterActive(false);

        _bundleRepo.AddBundle(new BundleInfo
        {
            BundleName = "Spring Foraging Bundle",
            RoomName = "Crafts Room",
            RequiredItems = new[] { new BundleItem { ItemId = "leek", ItemName = "Leek" } },
            CompletedItemIds = new HashSet<string>()
        });

        var items = new List<CollectibleItem>
        {
            new() { Id = "leek", Name = "Leek", CollectionType = CollectionType.Forage }
        };

        var result = CreateService().AnnotateWithBundleInfo(items);

        Assert.Single(result);
        Assert.Empty(result[0].BundleNames);
    }

    #endregion

    #region Bundle-Needed Items Filtering

    [Fact]
    public void GetBundleNeededItems_ReturnsOnlyItemsNeededForBundles()
    {
        _bundleRepo.AddBundle(new BundleInfo
        {
            BundleName = "Spring Foraging Bundle",
            RoomName = "Crafts Room",
            RequiredItems = new[] { new BundleItem { ItemId = "leek", ItemName = "Leek" } },
            CompletedItemIds = new HashSet<string>()
        });

        var items = new List<CollectibleItem>
        {
            new() { Id = "leek", Name = "Leek", CollectionType = CollectionType.Forage },
            new() { Id = "sunfish", Name = "Sunfish", CollectionType = CollectionType.Fish }
        };

        var annotated = CreateService().AnnotateWithBundleInfo(items);
        var bundleNeeded = annotated.Where(i => i.IsNeededForBundle).ToList();

        Assert.Single(bundleNeeded);
        Assert.Equal("leek", bundleNeeded[0].Id);
    }

    #endregion
}
