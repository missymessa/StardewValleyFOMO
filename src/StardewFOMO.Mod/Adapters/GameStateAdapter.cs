using StardewModdingAPI;
using StardewValley;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using Season = StardewFOMO.Core.Models.Season;
using Weather = StardewFOMO.Core.Models.Weather;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IGameStateProvider"/> by reading from <c>Game1</c> state.
/// </summary>
public sealed class GameStateAdapter : IGameStateProvider
{
    /// <inheritdoc/>
    public GameDate GetCurrentDate()
    {
        var season = Game1.currentSeason?.ToLowerInvariant() switch
        {
            "spring" => Season.Spring,
            "summer" => Season.Summer,
            "fall" => Season.Fall,
            "winter" => Season.Winter,
            _ => Season.Spring
        };

        return new GameDate
        {
            Season = season,
            Day = Game1.dayOfMonth,
            Year = Game1.year
        };
    }

    /// <inheritdoc/>
    public Weather GetCurrentWeather()
    {
        if (Game1.isSnowing)
            return Weather.Snowy;
        if (Game1.isLightning)
            return Weather.Stormy;
        if (Game1.isRaining)
            return Weather.Rainy;
        if (Game1.isDebrisWeather)
            return Weather.Windy;
        return Weather.Sunny;
    }

    /// <inheritdoc/>
    public int GetTimeOfDay() => Game1.timeOfDay;

    /// <inheritdoc/>
    public Weather GetTomorrowWeather()
    {
        // Game1.weatherForTomorrow is a string in SDV 1.6
        return Game1.weatherForTomorrow switch
        {
            "Sun" => Weather.Sunny,
            "Rain" => Weather.Rainy,
            "Wind" => Weather.Windy,
            "Storm" => Weather.Stormy,
            "Snow" => Weather.Snowy,
            _ => Weather.Sunny
        };
    }

    /// <inheritdoc/>
    public bool IsFestivalDay()
    {
        return Utility.isFestivalDay(Game1.dayOfMonth, GameSeasonFromString(Game1.currentSeason));
    }

    /// <inheritdoc/>
    public string? GetFestivalName()
    {
        if (!IsFestivalDay())
            return null;

        try
        {
            var festivalData = Game1.temporaryContent?.Load<Dictionary<string, string>>(
                $"Data\\Festivals\\{Game1.currentSeason}{Game1.dayOfMonth}");
            if (festivalData != null && festivalData.TryGetValue("name", out var name))
                return name;
        }
        catch
        {
            // Gracefully handle missing festival data
        }

        return "Festival";
    }

    /// <inheritdoc/>
    public bool IsTomorrowFestivalDay()
    {
        var tomorrow = GetCurrentDate().GetTomorrow();
        var sdvSeason = CoreSeasonToGameSeason(tomorrow.Season);
        return Utility.isFestivalDay(tomorrow.Day, sdvSeason);
    }

    /// <inheritdoc/>
    public string? GetTomorrowFestivalName()
    {
        if (!IsTomorrowFestivalDay())
            return null;

        var tomorrow = GetCurrentDate().GetTomorrow();
        var seasonName = CoreSeasonToString(tomorrow.Season);

        try
        {
            var festivalData = Game1.temporaryContent?.Load<Dictionary<string, string>>(
                $"Data\\Festivals\\{seasonName}{tomorrow.Day}");
            if (festivalData != null && festivalData.TryGetValue("name", out var name))
                return name;
        }
        catch
        {
            // Gracefully handle missing festival data
        }

        return "Festival";
    }

    private static StardewValley.Season GameSeasonFromString(string? seasonStr)
    {
        return seasonStr?.ToLowerInvariant() switch
        {
            "spring" => StardewValley.Season.Spring,
            "summer" => StardewValley.Season.Summer,
            "fall" => StardewValley.Season.Fall,
            "winter" => StardewValley.Season.Winter,
            _ => StardewValley.Season.Spring
        };
    }

    private static StardewValley.Season CoreSeasonToGameSeason(Season coreSeason)
    {
        return coreSeason switch
        {
            Season.Spring => StardewValley.Season.Spring,
            Season.Summer => StardewValley.Season.Summer,
            Season.Fall => StardewValley.Season.Fall,
            Season.Winter => StardewValley.Season.Winter,
            _ => StardewValley.Season.Spring
        };
    }

    private static string CoreSeasonToString(Season coreSeason)
    {
        return coreSeason switch
        {
            Season.Spring => "spring",
            Season.Summer => "summer",
            Season.Fall => "fall",
            Season.Winter => "winter",
            _ => "spring"
        };
    }
}
