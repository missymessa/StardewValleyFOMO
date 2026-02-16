using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts Community Center bundle state.
/// Implemented by SMAPI adapter reading from game data.
/// </summary>
public interface IBundleRepository
{
    /// <summary>Get all Community Center bundles with their completion state.</summary>
    IReadOnlyList<BundleInfo> GetAllBundles();

    /// <summary>Get only incomplete bundles.</summary>
    IReadOnlyList<BundleInfo> GetIncompleteBundles();

    /// <summary>Check whether a specific item is needed for any incomplete bundle.</summary>
    /// <param name="itemId">The item identifier to check.</param>
    /// <returns>List of bundle names that need this item, or empty if none.</returns>
    IReadOnlyList<string> GetBundleNamesNeedingItem(string itemId);

    /// <summary>Check whether the Community Center route is active (vs. Joja).</summary>
    bool IsCommunityCenterActive();
}
