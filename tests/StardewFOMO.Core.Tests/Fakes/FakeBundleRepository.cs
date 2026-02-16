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

    public IReadOnlyList<string> GetAllRooms() =>
        _bundles
            .Select(b => b.RoomName)
            .Distinct()
            .OrderBy(r => GetRoomSortOrder(r))
            .ToList()
            .AsReadOnly();

    public RoomProgress GetRoomProgress(string roomName)
    {
        var bundles = GetBundlesByRoom(roomName);
        return new RoomProgress
        {
            RoomName = roomName,
            TotalBundles = bundles.Count,
            CompletedBundles = bundles.Count(b => b.IsComplete)
        };
    }

    public IReadOnlyList<BundleInfo> GetBundlesByRoom(string roomName) =>
        _bundles
            .Where(b => b.RoomName.Equals(roomName, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

    public bool IsCommunityComplete()
    {
        if (!_communityCenterActive)
            return false;
        return _bundles.Count > 0 && _bundles.All(b => b.IsComplete);
    }

    private static int GetRoomSortOrder(string roomName) => roomName.ToLowerInvariant() switch
    {
        "crafts room" => 0,
        "pantry" => 1,
        "fish tank" => 2,
        "boiler room" => 3,
        "bulletin board" => 4,
        "vault" => 5,
        _ => 99
    };
}
