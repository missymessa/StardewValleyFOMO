using StardewFOMO.Core.Interfaces;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IInventoryProvider"/> with in-memory inventory.</summary>
public sealed class FakeInventoryProvider : IInventoryProvider
{
    private readonly Dictionary<string, int> _items = new();

    public void AddItem(string itemId, int quantity = 1)
    {
        if (_items.ContainsKey(itemId))
            _items[itemId] += quantity;
        else
            _items[itemId] = quantity;
    }

    public void Clear() => _items.Clear();

    public bool HasItemInInventoryOrStorage(string itemId) => _items.ContainsKey(itemId) && _items[itemId] > 0;
    public IReadOnlyList<string> GetInventoryItemIds() => _items.Keys.ToList().AsReadOnly();
    public int GetTotalItemCount(string itemId) => _items.TryGetValue(itemId, out var count) ? count : 0;
}
