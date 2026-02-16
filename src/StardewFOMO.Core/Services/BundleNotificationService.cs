using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for generating bundle item pickup notifications.
/// Tracks notified items to prevent duplicate notifications within a session.
/// </summary>
public sealed class BundleNotificationService
{
    private readonly IBundleRepository _bundleRepo;
    private readonly ILogger _logger;
    private readonly HashSet<string> _notifiedItemBundles = new();

    /// <summary>Creates a new instance of the bundle notification service.</summary>
    public BundleNotificationService(IBundleRepository bundleRepo, ILogger logger)
    {
        _bundleRepo = bundleRepo;
        _logger = logger;
    }

    /// <summary>Check if a picked-up item is needed for any bundle.</summary>
    /// <param name="itemId">The item ID that was picked up.</param>
    /// <param name="itemName">The item name for display.</param>
    /// <returns>Notification message if needed, null if not a bundle item or already notified.</returns>
    public string? CheckForBundleItem(string itemId, string itemName)
    {
        // Get bundles needing this item
        var bundleNames = _bundleRepo.GetBundleNamesNeedingItem(itemId);
        
        if (bundleNames.Count == 0)
        {
            return null;
        }

        // Check for duplicates - create a unique key for this item-bundle combination
        var notificationKey = $"{itemId}:{string.Join(",", bundleNames)}";
        if (_notifiedItemBundles.Contains(notificationKey))
        {
            _logger.Log(LogLevel.Debug, $"Suppressed duplicate notification for {itemName}");
            return null;
        }

        // Mark as notified
        _notifiedItemBundles.Add(notificationKey);

        // Generate notification message
        var bundleText = bundleNames.Count == 1 
            ? bundleNames[0] 
            : $"{bundleNames[0]} (+{bundleNames.Count - 1} more)";
        
        var message = $"Bundle Item: {itemName} for {bundleText}";
        _logger.Log(LogLevel.Info, $"Bundle notification: {message}");
        
        return message;
    }

    /// <summary>Reset session tracking, allowing fresh notifications.</summary>
    public void ResetSession()
    {
        _notifiedItemBundles.Clear();
        _logger.Log(LogLevel.Debug, "Bundle notification session reset");
    }

    /// <summary>Check if an item has already been notified this session.</summary>
    public bool HasBeenNotified(string itemId) =>
        _notifiedItemBundles.Any(key => key.StartsWith($"{itemId}:"));
}
