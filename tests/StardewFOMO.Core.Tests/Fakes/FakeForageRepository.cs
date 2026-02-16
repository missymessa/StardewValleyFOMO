using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IForageRepository"/> with in-memory forage data.</summary>
public sealed class FakeForageRepository : IForageRepository
{
    private readonly List<CollectibleItem> _allForageables = new();

    public void AddForageable(CollectibleItem forageable) => _allForageables.Add(forageable);

    public IReadOnlyList<CollectibleItem> GetAllForageables() => _allForageables.AsReadOnly();

    public IReadOnlyList<CollectibleItem> GetForageablesBySeason(Season season) =>
        _allForageables
            .Where(f => f.AvailableSeasons.Count == 0 || f.AvailableSeasons.Contains(season))
            .ToList()
            .AsReadOnly();

    public IReadOnlyList<CollectibleItem> GetSeasonExclusiveForageables(Season season) =>
        _allForageables
            .Where(f => f.AvailableSeasons.Contains(season) && f.IsSeasonExclusive)
            .ToList()
            .AsReadOnly();
}
