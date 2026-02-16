using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

public class ForageAvailabilityServiceTests
{
    private readonly FakeGameStateProvider _gameState = new();
    private readonly FakeForageRepository _forageRepo = new();
    private readonly FakeCollectionRepository _collectionRepo = new();
    private readonly FakeInventoryProvider _inventoryProvider = new();
    private readonly TestLogger _logger = new();

    private ForageAvailabilityService CreateService() =>
        new(_forageRepo, _gameState, _collectionRepo, _inventoryProvider, _logger);

    #region Season Filtering

    [Fact]
    public void GetAvailableForageables_SpringDay_ReturnsOnlySpringForageables()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 5, Year = 1 };

        _forageRepo.AddForageable(CreateForage("leek", "Leek", new[] { Season.Spring }, "Forest"));
        _forageRepo.AddForageable(CreateForage("grape", "Grape", new[] { Season.Fall }, "Forest"));

        var result = CreateService().GetAvailableForageables();

        Assert.Single(result);
        Assert.Equal("leek", result[0].Id);
    }

    [Fact]
    public void GetAvailableForageables_AllSeasonForage_AlwaysIncluded()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Winter, Day = 10, Year = 1 };

        _forageRepo.AddForageable(new CollectibleItem
        {
            Id = "crystal",
            Name = "Crystal Fruit",
            CollectionType = CollectionType.Forage,
            AvailableSeasons = new[] { Season.Winter },
            Locations = new[] { "Forest" }
        });
        _forageRepo.AddForageable(new CollectibleItem
        {
            Id = "common_mushroom",
            Name = "Common Mushroom",
            CollectionType = CollectionType.Forage,
            AvailableSeasons = Array.Empty<Season>(), // all seasons
            Locations = new[] { "Cave" }
        });

        var result = CreateService().GetAvailableForageables();

        Assert.Equal(2, result.Count);
    }

    #endregion

    #region Location Grouping

    [Fact]
    public void GetAvailableForageablesGroupedByLocation_ReturnsGroupedCorrectly()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 5, Year = 1 };

        _forageRepo.AddForageable(CreateForage("leek", "Leek", new[] { Season.Spring }, "Forest"));
        _forageRepo.AddForageable(CreateForage("daffodil", "Daffodil", new[] { Season.Spring }, "Bus Stop"));
        _forageRepo.AddForageable(CreateForage("dandelion", "Dandelion", new[] { Season.Spring }, "Forest"));

        var result = CreateService().GetAvailableForageablesGroupedByLocation();

        Assert.Equal(2, result.Count); // Forest and Bus Stop
        Assert.True(result.ContainsKey("Forest"));
        Assert.True(result.ContainsKey("Bus Stop"));
        Assert.Equal(2, result["Forest"].Count);
        Assert.Single(result["Bus Stop"]);
    }

    #endregion

    #region Collection Status

    [Fact]
    public void GetAvailableForageables_PreviouslyForaged_MarkedAsEverCollected()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 5, Year = 1 };

        _forageRepo.AddForageable(CreateForage("leek", "Leek", new[] { Season.Spring }, "Forest"));
        _collectionRepo.AddShippedItem("leek");

        var result = CreateService().GetAvailableForageables();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.EverCollected, result[0].CollectionStatus);
    }

    [Fact]
    public void GetAvailableForageables_InInventory_MarkedAsInInventory()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 5, Year = 1 };

        _forageRepo.AddForageable(CreateForage("leek", "Leek", new[] { Season.Spring }, "Forest"));
        _inventoryProvider.AddItem("leek");

        var result = CreateService().GetAvailableForageables();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.InInventory, result[0].CollectionStatus);
    }

    [Fact]
    public void GetAvailableForageables_NeverForaged_MarkedAsNotCollected()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 5, Year = 1 };

        _forageRepo.AddForageable(CreateForage("leek", "Leek", new[] { Season.Spring }, "Forest"));

        var result = CreateService().GetAvailableForageables();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.NotCollected, result[0].CollectionStatus);
    }

    #endregion

    #region Festival Days

    [Fact]
    public void GetAvailableForageables_FestivalDay_StillReturnsForageables()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 13, Year = 1 };
        _gameState.FestivalDay = true;
        _gameState.FestivalName_ = "Egg Festival";

        _forageRepo.AddForageable(CreateForage("leek", "Leek", new[] { Season.Spring }, "Forest"));

        // Forageables still returned — UI layer handles festival messaging
        var result = CreateService().GetAvailableForageables();

        Assert.Single(result);
    }

    #endregion

    #region Helpers

    private static CollectibleItem CreateForage(
        string id,
        string name,
        Season[] seasons,
        string location) =>
        new()
        {
            Id = id,
            Name = name,
            CollectionType = CollectionType.Forage,
            AvailableSeasons = seasons,
            Locations = new[] { location }
        };

    #endregion
}
