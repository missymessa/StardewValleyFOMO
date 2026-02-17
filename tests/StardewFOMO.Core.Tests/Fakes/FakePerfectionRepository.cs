using StardewFOMO.Core.Interfaces;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IPerfectionRepository"/> with configurable state.</summary>
public sealed class FakePerfectionRepository : IPerfectionRepository
{
    public bool GingerIslandUnlocked { get; set; } = true;
    public int GoldenWalnutsCollected { get; set; } = 0;
    public int GoldenWalnutsTotal { get; set; } = 130;

    public void SetWalnutCount(int count) => GoldenWalnutsCollected = count;

    public bool IsGingerIslandUnlocked() => GingerIslandUnlocked;
    public int GetGoldenWalnutsCollected() => GoldenWalnutsCollected;
    public int GetGoldenWalnutsTotal() => GoldenWalnutsTotal;
}
