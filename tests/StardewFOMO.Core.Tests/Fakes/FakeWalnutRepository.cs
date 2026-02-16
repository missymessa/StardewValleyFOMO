using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IWalnutRepository"/> with configurable walnut data.</summary>
public sealed class FakeWalnutRepository : IWalnutRepository
{
    private readonly List<WalnutGroup> _walnutGroups = new();
    public bool GingerIslandUnlocked { get; set; } = true;
    public int CollectedCount { get; set; } = 0;
    public int TotalCount { get; set; } = 130;

    public void AddWalnutGroup(WalnutGroup group) => _walnutGroups.Add(group);

    public IReadOnlyList<WalnutGroup> GetAllWalnutGroups() => _walnutGroups.AsReadOnly();

    public int GetCollectedCount() => CollectedCount;

    public int GetTotalCount() => TotalCount;

    public bool IsGingerIslandUnlocked() => GingerIslandUnlocked;
}
