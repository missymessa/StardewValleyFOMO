namespace StardewFOMO.Core.Models;

/// <summary>
/// Reason why an item may or may not be available today.
/// </summary>
public enum AvailabilityReason
{
    /// <summary>Item is available to obtain today.</summary>
    Available,

    /// <summary>Item is not available in the current season.</summary>
    WrongSeason,

    /// <summary>Item requires specific weather that is not present today.</summary>
    WrongWeather,

    /// <summary>Item's location is locked or not accessible.</summary>
    LocationLocked,

    /// <summary>Item is only available at a different time of day.</summary>
    WrongTimeOfDay,

    /// <summary>Item availability is unknown or cannot be determined.</summary>
    Unknown
}

/// <summary>
/// Represents whether an item can be obtained today and why.
/// </summary>
public sealed class ItemAvailability
{
    /// <summary>The item identifier.</summary>
    public string ItemId { get; init; } = string.Empty;

    /// <summary>The display name of the item.</summary>
    public string ItemName { get; init; } = string.Empty;

    /// <summary>Whether this item can be obtained today.</summary>
    public bool IsAvailableToday { get; init; }

    /// <summary>The reason for the availability status.</summary>
    public AvailabilityReason Reason { get; init; } = AvailabilityReason.Unknown;

    /// <summary>Optional details about the availability (e.g., "Available in Winter").</summary>
    public string? Details { get; init; }

    /// <summary>Creates an available item.</summary>
    public static ItemAvailability CreateAvailable(string itemId, string itemName) => new()
    {
        ItemId = itemId,
        ItemName = itemName,
        IsAvailableToday = true,
        Reason = AvailabilityReason.Available
    };

    /// <summary>Creates an unavailable item with a reason.</summary>
    public static ItemAvailability CreateUnavailable(string itemId, string itemName, AvailabilityReason reason, string? details = null) => new()
    {
        ItemId = itemId,
        ItemName = itemName,
        IsAvailableToday = false,
        Reason = reason,
        Details = details
    };
}
