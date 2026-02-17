using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IShippingRepository"/> with in-memory shipping data.</summary>
public sealed class FakeShippingRepository : IShippingRepository
{
    private readonly List<ShippingItem> _shippableItems = new();

    public void AddShippableItem(string itemId, string displayName, string subcategory = "Other")
    {
        _shippableItems.Add(new ShippingItem
        {
            ItemId = itemId,
            DisplayName = displayName,
            IsComplete = false,
            Subcategory = subcategory
        });
    }

    public IReadOnlyList<ShippingItem> GetAllShippableItems() => _shippableItems.AsReadOnly();

    public IReadOnlyList<ShippingItem> GetUnshippedItems(IReadOnlySet<string> shippedItemIds)
    {
        return _shippableItems
            .Where(item => !shippedItemIds.Contains(item.ItemId))
            .ToList()
            .AsReadOnly();
    }
}
