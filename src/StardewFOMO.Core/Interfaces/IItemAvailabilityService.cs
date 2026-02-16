using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts item availability checking based on season, weather, time, and location.
/// Implemented by SMAPI adapter reading game state and item data.
/// </summary>
public interface IItemAvailabilityService
{
    /// <summary>Get the availability status of an item for today.</summary>
    /// <param name="itemId">The item identifier to check.</param>
    /// <returns>Availability information including reason if unavailable.</returns>
    ItemAvailability GetAvailability(string itemId);

    /// <summary>Quick check whether an item can be obtained today.</summary>
    /// <param name="itemId">The item identifier to check.</param>
    /// <returns>True if the item is obtainable today.</returns>
    bool IsAvailableToday(string itemId);
}
