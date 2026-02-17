namespace StardewFOMO.Core.Models;

/// <summary>
/// Shippable item with subcategory classification for perfection tracking.
/// </summary>
public sealed class ShippingItem : PerfectionItem
{
    /// <summary>Subcategory for grouping (Crops, ArtisanGoods, AnimalProducts, Forage, Fish, Minerals, Other).</summary>
    public string Subcategory { get; init; } = "Other";
}

/// <summary>
/// Shipping subcategory constants.
/// </summary>
public static class ShippingSubcategory
{
    /// <summary>Crops subcategory.</summary>
    public const string Crops = "Crops";
    /// <summary>Artisan goods subcategory.</summary>
    public const string ArtisanGoods = "Artisan Goods";
    /// <summary>Animal products subcategory.</summary>
    public const string AnimalProducts = "Animal Products";
    /// <summary>Forage items subcategory.</summary>
    public const string Forage = "Forage";
    /// <summary>Fish subcategory.</summary>
    public const string Fish = "Fish";
    /// <summary>Minerals subcategory.</summary>
    public const string Minerals = "Minerals";
    /// <summary>Other items subcategory.</summary>
    public const string Other = "Other";
}
