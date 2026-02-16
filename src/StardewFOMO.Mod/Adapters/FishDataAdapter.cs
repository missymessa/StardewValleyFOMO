using StardewValley;
using StardewValley.TokenizableStrings;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using Season = StardewFOMO.Core.Models.Season;
using Weather = StardewFOMO.Core.Models.Weather;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IFishRepository"/> by reading fish data from game content.
/// Parses <c>Data\Fish</c> to build fish availability information.
/// </summary>
public sealed class FishDataAdapter : IFishRepository
{
    private List<CollectibleItem>? _fishCache;

    /// <inheritdoc/>
    public IReadOnlyList<CollectibleItem> GetAllFish()
    {
        if (_fishCache != null)
            return _fishCache.AsReadOnly();

        _fishCache = new List<CollectibleItem>();
        var fishData = Game1.content.Load<Dictionary<string, string>>("Data\\Fish");

        foreach (var kvp in fishData)
        {
            var fields = kvp.Value.Split('/');
            if (fields.Length < 8)
                continue;

            // Skip non-fish entries (trap fish, etc.)
            if (fields[1] == "trap")
                continue;

            try
            {
                // Resolve display name from object data
                var name = GetFishDisplayName(kvp.Key, fields[0]);
                var seasons = ParseSeasons(fields[6]);
                var weather = ParseWeather(fields[7]);
                var (startTime, endTime) = ParseTimeRange(fields[5]);
                var locations = ParseLocations(fields.Length > 4 ? fields[4] : "");

                _fishCache.Add(new CollectibleItem
                {
                    Id = kvp.Key,
                    Name = name,
                    CollectionType = CollectionType.Fish,
                    AvailableSeasons = seasons,
                    RequiredWeather = weather,
                    StartTime = startTime,
                    EndTime = endTime,
                    Locations = locations
                });
            }
            catch
            {
                // Skip malformed entries from modded content gracefully
            }
        }

        return _fishCache.AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<CollectibleItem> GetFishBySeasonAndWeather(Season season, Weather weather)
    {
        return GetAllFish()
            .Where(f => MatchesSeason(f, season) && MatchesWeather(f, weather))
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<CollectibleItem> GetAvailableFish(Season season, Weather weather, int timeOfDay)
    {
        return GetAllFish()
            .Where(f => MatchesSeason(f, season) && MatchesWeather(f, weather) && MatchesTime(f, timeOfDay))
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<CollectibleItem> GetSeasonExclusiveFish(Season season)
    {
        return GetAllFish()
            .Where(f => f.AvailableSeasons.Count == 1 && f.AvailableSeasons[0] == season)
            .ToList()
            .AsReadOnly();
    }

    private static bool MatchesSeason(CollectibleItem fish, Season season) =>
        fish.AvailableSeasons.Count == 0 || fish.AvailableSeasons.Contains(season);

    private static bool MatchesWeather(CollectibleItem fish, Weather weather) =>
        fish.RequiredWeather.Count == 0 || fish.RequiredWeather.Contains(weather)
        || (weather == Weather.Stormy && fish.RequiredWeather.Contains(Weather.Rainy));

    private static bool MatchesTime(CollectibleItem fish, int timeOfDay)
    {
        if (fish.StartTime == null && fish.EndTime == null)
            return true;
        var start = fish.StartTime ?? 600;
        var end = fish.EndTime ?? 2600;
        return timeOfDay >= start && timeOfDay <= end;
    }

    private static IReadOnlyList<Season> ParseSeasons(string seasonStr)
    {
        var seasons = new List<Season>();
        var parts = seasonStr.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            switch (part)
            {
                case "spring": seasons.Add(Season.Spring); break;
                case "summer": seasons.Add(Season.Summer); break;
                case "fall": seasons.Add(Season.Fall); break;
                case "winter": seasons.Add(Season.Winter); break;
            }
        }
        return seasons.AsReadOnly();
    }

    private static IReadOnlyList<Weather> ParseWeather(string weatherStr)
    {
        return weatherStr.ToLowerInvariant() switch
        {
            "rainy" => new[] { Weather.Rainy },
            "sunny" => new[] { Weather.Sunny },
            _ => Array.Empty<Weather>() // "both" = any weather
        };
    }

    private static (int? Start, int? End) ParseTimeRange(string timeStr)
    {
        var parts = timeStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2 &&
            int.TryParse(parts[0], out var start) &&
            int.TryParse(parts[1], out var end))
        {
            return (start, end);
        }
        return (null, null);
    }

    private static IReadOnlyList<string> ParseLocations(string locationStr)
    {
        if (string.IsNullOrWhiteSpace(locationStr))
            return Array.Empty<string>();

        // Location data can be complex; parse basic location names
        var locations = new List<string>();
        var parts = locationStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            // In SDV fish data, location entries may have area IDs; take just the name
            if (!int.TryParse(part, out _))
                locations.Add(part);
        }
        return locations.AsReadOnly();
    }

    private static string GetFishDisplayName(string fishId, string fallbackName)
    {
        if (Game1.objectData != null && Game1.objectData.TryGetValue(fishId, out var info))
        {
            // Resolve tokenized display names
            return TokenParser.ParseText(info.DisplayName) ?? info.Name ?? fallbackName;
        }
        return fallbackName;
    }
}
