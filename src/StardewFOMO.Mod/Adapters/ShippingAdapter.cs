using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.TokenizableStrings;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IShippingRepository"/> by reading from game content data.
/// </summary>
public sealed class ShippingAdapter : IShippingRepository
{
    // Categories that count for shipping perfection
    private static readonly HashSet<int> ShippableCategories = new()
    {
        -75,  // Vegetables
        -79,  // Fruits
        -80,  // Flowers
        -81,  // Forage (greens)
        -26,  // Artisan Goods
        -5,   // Eggs
        -6,   // Milk
        -14,  // Meat
        -18,  // Animal Products (wool, etc)
        -4,   // Fish
        -23,  // Sell at Fish Shop
        -27,  // Syrup/Tapper Products
        -74,  // Seeds (don't ship, but some special items)
        -2,   // Gems (some count)
        -12,  // Minerals
        -15,  // Metal resources
        -16,  // Building resources
        -17,  // Crafting/misc object
        -28,  // Monster drops
    };

    /// <inheritdoc/>
    public IReadOnlyList<ShippingItem> GetAllShippableItems()
    {
        var items = new List<ShippingItem>();

        if (!Context.IsWorldReady)
            return items.AsReadOnly();

        var objectData = Game1.objectData;
        if (objectData == null)
            return items.AsReadOnly();

        foreach (var kvp in objectData)
        {
            var itemId = kvp.Key;
            var data = kvp.Value;

            // Check if this item counts for shipping perfection
            if (!IsShippableForPerfection(itemId, data))
                continue;

            items.Add(new ShippingItem
            {
                ItemId = itemId,
                DisplayName = TokenParser.ParseText(data.DisplayName) ?? data.Name ?? itemId,
                IsComplete = false,
                Subcategory = GetSubcategory(data.Category)
            });
        }

        return items.OrderBy(i => i.Subcategory).ThenBy(i => i.DisplayName).ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<ShippingItem> GetUnshippedItems(IReadOnlySet<string> shippedItemIds)
    {
        var allShippable = GetAllShippableItems();
        return allShippable
            .Where(item => !shippedItemIds.Contains(item.ItemId))
            .ToList()
            .AsReadOnly();
    }

    private static bool IsShippableForPerfection(string itemId, ObjectData data)
    {
        // Exclude quest items, secret notes, etc.
        if (data.ExcludeFromShippingCollection)
            return false;

        // Must be a valid object category
        if (data.Category == 0)
            return false;

        // Stone, weeds, twigs, etc. don't count
        if (data.Type == "Litter" || data.Type == "Quest" || data.Type == "asdf")
            return false;

        // Check category (most shippable items have negative category)
        if (!ShippableCategories.Contains(data.Category))
        {
            // Still include some specific items by type
            if (data.Type != "Basic" && data.Type != "Minerals" && data.Type != "Fish")
                return false;
        }

        return true;
    }

    private static string GetSubcategory(int category) => category switch
    {
        -75 or -79 => ShippingSubcategory.Crops,
        -80 => ShippingSubcategory.Crops, // Flowers
        -81 => ShippingSubcategory.Forage,
        -26 or -27 => ShippingSubcategory.ArtisanGoods,
        -5 or -6 or -14 or -18 => ShippingSubcategory.AnimalProducts,
        -4 or -23 => ShippingSubcategory.Fish,
        -2 or -12 or -15 => ShippingSubcategory.Minerals,
        _ => ShippingSubcategory.Other
    };
}
