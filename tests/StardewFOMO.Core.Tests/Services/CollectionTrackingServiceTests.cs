using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

public class CollectionTrackingServiceTests
{
    private readonly FakeCollectionRepository _collectionRepo = new();
    private readonly FakeInventoryProvider _inventoryProvider = new();
    private readonly FakeRecipeRepository _recipeRepo = new();
    private readonly TestLogger _logger = new();

    private CollectionTrackingService CreateService() =>
        new(_collectionRepo, _inventoryProvider, _recipeRepo, _logger);

    #region Shipping Collection

    [Fact]
    public void GetShippingStatus_NeverShipped_MarkedNotCollected()
    {
        var items = new List<CollectibleItem>
        {
            new() { Id = "parsnip", Name = "Parsnip", CollectionType = CollectionType.ShippableItem }
        };

        var result = CreateService().GetShippingCollectionStatus(items);

        Assert.Single(result);
        Assert.Equal(CollectionStatus.NotCollected, result[0].CollectionStatus);
    }

    [Fact]
    public void GetShippingStatus_AlreadyShipped_MarkedEverCollected()
    {
        _collectionRepo.AddShippedItem("parsnip");

        var items = new List<CollectibleItem>
        {
            new() { Id = "parsnip", Name = "Parsnip", CollectionType = CollectionType.ShippableItem }
        };

        var result = CreateService().GetShippingCollectionStatus(items);

        Assert.Single(result);
        Assert.Equal(CollectionStatus.EverCollected, result[0].CollectionStatus);
    }

    #endregion

    #region Museum Donation

    [Fact]
    public void GetMuseumStatus_NeverDonated_MarkedNotCollected()
    {
        var items = new List<CollectibleItem>
        {
            new() { Id = "dwarf_scroll_i", Name = "Dwarf Scroll I", CollectionType = CollectionType.Artifact }
        };

        var result = CreateService().GetMuseumDonationStatus(items);

        Assert.Single(result);
        Assert.Equal(CollectionStatus.NotCollected, result[0].CollectionStatus);
    }

    [Fact]
    public void GetMuseumStatus_Donated_MarkedEverCollected()
    {
        _collectionRepo.AddDonatedItem("dwarf_scroll_i");

        var items = new List<CollectibleItem>
        {
            new() { Id = "dwarf_scroll_i", Name = "Dwarf Scroll I", CollectionType = CollectionType.Artifact }
        };

        var result = CreateService().GetMuseumDonationStatus(items);

        Assert.Single(result);
        Assert.Equal(CollectionStatus.EverCollected, result[0].CollectionStatus);
    }

    #endregion

    #region Cooking Recipes

    [Fact]
    public void GetCookingStatus_KnownNeverCooked_WithIngredients_MarkedInInventory()
    {
        _collectionRepo.AddKnownCookingRecipe("maki_roll");
        _inventoryProvider.AddItem("fish", 1);
        _inventoryProvider.AddItem("seaweed", 1);
        _inventoryProvider.AddItem("rice", 1);

        _recipeRepo.AddCookingRecipe(new RecipeInfo
        {
            RecipeId = "maki_roll",
            Name = "Maki Roll",
            IsCookingRecipe = true,
            Ingredients = new[]
            {
                new RecipeIngredient { ItemId = "fish", ItemName = "Any Fish", Quantity = 1 },
                new RecipeIngredient { ItemId = "seaweed", ItemName = "Seaweed", Quantity = 1 },
                new RecipeIngredient { ItemId = "rice", ItemName = "Rice", Quantity = 1 }
            }
        });

        var result = CreateService().GetCookingRecipeStatus();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.InInventory, result[0].CollectionStatus); // Has ingredients
    }

    [Fact]
    public void GetCookingStatus_KnownNeverCooked_MissingIngredients_MarkedNotCollected()
    {
        _collectionRepo.AddKnownCookingRecipe("maki_roll");
        // No ingredients in inventory

        _recipeRepo.AddCookingRecipe(new RecipeInfo
        {
            RecipeId = "maki_roll",
            Name = "Maki Roll",
            IsCookingRecipe = true,
            Ingredients = new[]
            {
                new RecipeIngredient { ItemId = "fish", ItemName = "Any Fish", Quantity = 1 },
                new RecipeIngredient { ItemId = "seaweed", ItemName = "Seaweed", Quantity = 1 },
                new RecipeIngredient { ItemId = "rice", ItemName = "Rice", Quantity = 1 }
            }
        });

        var result = CreateService().GetCookingRecipeStatus();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.NotCollected, result[0].CollectionStatus);
    }

    [Fact]
    public void GetCookingStatus_AlreadyCooked_MarkedEverCollected()
    {
        _collectionRepo.AddKnownCookingRecipe("maki_roll");
        _collectionRepo.AddCookedRecipe("maki_roll");

        _recipeRepo.AddCookingRecipe(new RecipeInfo
        {
            RecipeId = "maki_roll",
            Name = "Maki Roll",
            IsCookingRecipe = true,
            Ingredients = Array.Empty<RecipeIngredient>()
        });

        var result = CreateService().GetCookingRecipeStatus();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.EverCollected, result[0].CollectionStatus);
    }

    #endregion

    #region Crafting Recipes

    [Fact]
    public void GetCraftingStatus_KnownNeverCrafted_WithMaterials_MarkedInInventory()
    {
        _collectionRepo.AddKnownCraftingRecipe("chest");
        _inventoryProvider.AddItem("wood", 50);

        _recipeRepo.AddCraftingRecipe(new RecipeInfo
        {
            RecipeId = "chest",
            Name = "Chest",
            IsCookingRecipe = false,
            Ingredients = new[]
            {
                new RecipeIngredient { ItemId = "wood", ItemName = "Wood", Quantity = 50 }
            }
        });

        var result = CreateService().GetCraftingRecipeStatus();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.InInventory, result[0].CollectionStatus);
    }

    [Fact]
    public void GetCraftingStatus_AlreadyCrafted_MarkedEverCollected()
    {
        _collectionRepo.AddKnownCraftingRecipe("chest");
        _collectionRepo.AddCraftedRecipe("chest");

        _recipeRepo.AddCraftingRecipe(new RecipeInfo
        {
            RecipeId = "chest",
            Name = "Chest",
            IsCookingRecipe = false,
            Ingredients = Array.Empty<RecipeIngredient>()
        });

        var result = CreateService().GetCraftingRecipeStatus();

        Assert.Single(result);
        Assert.Equal(CollectionStatus.EverCollected, result[0].CollectionStatus);
    }

    #endregion
}
