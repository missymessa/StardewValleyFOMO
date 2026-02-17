using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IStardropRepository"/> with configurable stardrops.</summary>
public sealed class FakeStardropRepository : IStardropRepository
{
    private readonly List<StardropInfo> _stardrops = new();
    private readonly HashSet<string> _collectedSources = new();

    public void AddStardrop(StardropInfo stardrop) => _stardrops.Add(stardrop);

    public void CollectStardrop(string source) => _collectedSources.Add(source);

    public void SetStardropCollected(string source, bool collected)
    {
        // Ensure stardrop exists
        if (!_stardrops.Any(s => s.Source == source))
        {
            _stardrops.Add(new StardropInfo
            {
                Source = source,
                DisplayName = source,
                IsCollected = false,
                AcquisitionHint = $"Obtain from {source}"
            });
        }
        if (collected)
            _collectedSources.Add(source);
        else
            _collectedSources.Remove(source);
    }

    public void SetAllCollected()
    {
        _stardrops.Clear();
        _collectedSources.Clear();
        var sources = new[] { "Fair", "Mines", "Krobus", "Old Master", "Spouse", "Museum", "Fish" };
        foreach (var source in sources)
        {
            _stardrops.Add(new StardropInfo
            {
                Source = source,
                DisplayName = $"Stardrop ({source})",
                IsCollected = true,
                AcquisitionHint = "Already collected"
            });
            _collectedSources.Add(source);
        }
    }

    public IReadOnlyList<StardropInfo> GetAllStardrops() => _stardrops
        .Select(s => new StardropInfo
        {
            Source = s.Source,
            DisplayName = s.DisplayName,
            IsCollected = _collectedSources.Contains(s.Source),
            AcquisitionHint = s.AcquisitionHint
        })
        .ToList()
        .AsReadOnly();

    public int GetCollectedCount() => _collectedSources.Count;

    public int GetTotalCount() => _stardrops.Count > 0 ? _stardrops.Count : 7;

    public bool HasCollectedStardrop(string stardropSource) => _collectedSources.Contains(stardropSource);
}
