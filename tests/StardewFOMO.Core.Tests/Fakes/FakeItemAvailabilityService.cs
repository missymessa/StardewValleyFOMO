using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IItemAvailabilityService"/> for testing.</summary>
public sealed class FakeItemAvailabilityService : IItemAvailabilityService
{
    private readonly Dictionary<string, ItemAvailability> _availability = new();

    /// <summary>Set availability for an item.</summary>
    public void SetAvailable(string itemId, string itemName)
    {
        _availability[itemId] = ItemAvailability.CreateAvailable(itemId, itemName);
    }

    /// <summary>Set an item as unavailable with a specific reason.</summary>
    public void SetUnavailable(string itemId, string itemName, AvailabilityReason reason, string? details = null)
    {
        _availability[itemId] = ItemAvailability.CreateUnavailable(itemId, itemName, reason, details);
    }

    /// <summary>Clear all availability data.</summary>
    public void Clear() => _availability.Clear();

    /// <inheritdoc/>
    public ItemAvailability GetAvailability(string itemId)
    {
        if (_availability.TryGetValue(itemId, out var availability))
            return availability;
        
        // Default to unknown availability
        return new ItemAvailability
        {
            ItemId = itemId,
            ItemName = $"Item {itemId}",
            IsAvailableToday = false,
            Reason = AvailabilityReason.Unknown
        };
    }

    /// <inheritdoc/>
    public bool IsAvailableToday(string itemId)
    {
        return _availability.TryGetValue(itemId, out var availability) && availability.IsAvailableToday;
    }
}
