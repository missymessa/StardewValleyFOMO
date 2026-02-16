using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IBundleRepository"/> with in-memory bundle data.</summary>
public sealed class FakeBundleRepository : IBundleRepository
{
    private readonly List<BundleInfo> _bundles = new();
    private bool _communityCenterActive = true;

    public void AddBundle(BundleInfo bundle) => _bundles.Add(bundle);
    public void SetCommunityCenterActive(bool active) => _communityCenterActive = active;

    public IReadOnlyList<BundleInfo> GetAllBundles() => _bundles.AsReadOnly();

    public IReadOnlyList<BundleInfo> GetIncompleteBundles() =>
        _bundles.Where(b => !b.IsComplete).ToList().AsReadOnly();

    public IReadOnlyList<string> GetBundleNamesNeedingItem(string itemId) =>
        _bundles
            .Where(b => !b.IsComplete && b.GetRemainingItems().Any(r => r.ItemId == itemId))
            .Select(b => b.BundleName)
            .ToList()
            .AsReadOnly();

    public bool IsCommunityCenterActive() => _communityCenterActive;
}
