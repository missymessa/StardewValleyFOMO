using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IFishRepository"/> with in-memory fish data.</summary>
public sealed class FakeFishRepository : IFishRepository
{
    private readonly List<CollectibleItem> _allFish = new();

    public void AddFish(CollectibleItem fish) => _allFish.Add(fish);

    /// <summary>Add a simple fish with just an ID for testing.</summary>
    public void AddFish(string fishId) => _allFish.Add(new CollectibleItem
    {
        Id = fishId,
        Name = fishId,
        CollectionType = CollectionType.Fish
    });

    public IReadOnlyList<CollectibleItem> GetAllFish() => _allFish.AsReadOnly();

    public IReadOnlyList<CollectibleItem> GetFishBySeasonAndWeather(Season season, Weather weather) =>
        _allFish
            .Where(f => IsAvailableInSeason(f, season) && IsAvailableInWeather(f, weather))
            .ToList()
            .AsReadOnly();

    public IReadOnlyList<CollectibleItem> GetAvailableFish(Season season, Weather weather, int timeOfDay) =>
        _allFish
            .Where(f => IsAvailableInSeason(f, season)
                        && IsAvailableInWeather(f, weather)
                        && IsAvailableAtTime(f, timeOfDay))
            .ToList()
            .AsReadOnly();

    public IReadOnlyList<CollectibleItem> GetSeasonExclusiveFish(Season season) =>
        _allFish
            .Where(f => f.AvailableSeasons.Contains(season) && f.IsSeasonExclusive)
            .ToList()
            .AsReadOnly();

    private static bool IsAvailableInSeason(CollectibleItem fish, Season season) =>
        fish.AvailableSeasons.Count == 0 || fish.AvailableSeasons.Contains(season);

    private static bool IsAvailableInWeather(CollectibleItem fish, Weather weather) =>
        fish.RequiredWeather.Count == 0 || fish.RequiredWeather.Contains(weather)
        || (weather == Weather.Stormy && fish.RequiredWeather.Contains(Weather.Rainy));

    private static bool IsAvailableAtTime(CollectibleItem fish, int timeOfDay) =>
        (!fish.StartTime.HasValue || timeOfDay >= fish.StartTime.Value)
        && (!fish.EndTime.HasValue || timeOfDay <= fish.EndTime.Value);
}
