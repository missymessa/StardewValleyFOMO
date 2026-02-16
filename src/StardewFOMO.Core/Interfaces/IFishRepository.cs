using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts fish availability data.
/// Implemented by SMAPI adapter reading fish data from game content files.
/// </summary>
public interface IFishRepository
{
    /// <summary>Get all fish known to the game.</summary>
    IReadOnlyList<CollectibleItem> GetAllFish();

    /// <summary>Get fish available in the given season and weather.</summary>
    IReadOnlyList<CollectibleItem> GetFishBySeasonAndWeather(Season season, Weather weather);

    /// <summary>Get fish available at the given season, weather, and time of day.</summary>
    IReadOnlyList<CollectibleItem> GetAvailableFish(Season season, Weather weather, int timeOfDay);

    /// <summary>Get all fish exclusive to a specific season.</summary>
    IReadOnlyList<CollectibleItem> GetSeasonExclusiveFish(Season season);
}
