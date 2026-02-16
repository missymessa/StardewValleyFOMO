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

    /// <inheritdoc/>
    public bool IsTravelingMerchantDay()
    {
        // Traveling Merchant visits on Friday (5) and Sunday (0)
        var dayOfWeek = Game1.dayOfMonth % 7;
        return dayOfWeek == 5 || dayOfWeek == 0;
    }

    /// <inheritdoc/>
    public bool IsTomorrowTravelingMerchantDay()
    {
        var tomorrow = GetCurrentDate().GetTomorrow();
        var dayOfWeek = tomorrow.Day % 7;
        return dayOfWeek == 5 || dayOfWeek == 0;
    }

    /// <inheritdoc/>
    public bool IsNightMarketDay()
    {
        return Game1.currentSeason == "winter" && Game1.dayOfMonth >= 15 && Game1.dayOfMonth <= 17;
    }

    /// <inheritdoc/>
    public bool IsTomorrowNightMarketDay()
    {
        var tomorrow = GetCurrentDate().GetTomorrow();
        return tomorrow.Season == Season.Winter && tomorrow.Day >= 15 && tomorrow.Day <= 17;
    }

    /// <inheritdoc/>
    public bool IsQueenOfSauceDay()
    {
        // Queen of Sauce airs on Sundays (day % 7 == 0) and reruns on Wednesdays (day % 7 == 3)
        var dayOfWeek = Game1.dayOfMonth % 7;
        return dayOfWeek == 0 || dayOfWeek == 3;
    }

    /// <inheritdoc/>
    public string? GetQueenOfSauceRecipe()
    {
        if (!IsQueenOfSauceDay())
            return null;

        try
        {
            // The TV channel provides recipes via CraftingRecipe data
            // For now, return a general message - specific recipe requires more complex lookup
            var dayOfWeek = Game1.dayOfMonth % 7;
            return dayOfWeek == 0 ? "New recipe today!" : "Rerun episode today";
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public bool IsTomorrowQueenOfSauceDay()
    {
        var tomorrow = GetCurrentDate().GetTomorrow();
        var dayOfWeek = tomorrow.Day % 7;
        return dayOfWeek == 0 || dayOfWeek == 3;
    }

    /// <inheritdoc/>
    public double GetDailyLuck()
    {
        return Game1.player?.DailyLuck ?? 0;
    }

    /// <inheritdoc/>
    public string GetLuckDescription()
    {
        var luck = GetDailyLuck();
        return luck switch
        {
            >= 0.07 => "Very lucky day! Spirits are very happy.",
            >= 0.02 => "Good luck. The spirits are in good humor.",
            >= -0.02 => "Neutral day. The spirits feel neutral.",
            >= -0.07 => "Somewhat unlucky. The spirits feel somewhat miffed.",
            _ => "Bad luck day. The spirits are very displeased."
        };
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetTodayEvents()
    {
        var events = new List<string>();

        if (IsTravelingMerchantDay())
            events.Add("🛒 Traveling Merchant (Cindersap Forest)");

        if (IsNightMarketDay())
            events.Add("🌙 Night Market (Beach, 5PM-2AM)");

        if (IsQueenOfSauceDay())
        {
            var recipe = GetQueenOfSauceRecipe();
            events.Add($"📺 Queen of Sauce ({recipe ?? "TV"})");
        }

        // Check for Krobus shop special (Wednesdays have dishes)
        if (Game1.dayOfMonth % 7 == 3)
            events.Add("🏪 Krobus has special dishes today");

        return events.AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetTomorrowEvents()
    {
        var events = new List<string>();

        if (IsTomorrowTravelingMerchantDay())
            events.Add("🛒 Traveling Merchant visits tomorrow");

        if (IsTomorrowNightMarketDay())
            events.Add("🌙 Night Market tomorrow (Beach, 5PM-2AM)");

        if (IsTomorrowQueenOfSauceDay())
            events.Add("📺 Queen of Sauce airs tomorrow");

        // Check for Krobus shop special tomorrow
        var tomorrow = GetCurrentDate().GetTomorrow();
        if (tomorrow.Day % 7 == 3)
            events.Add("🏪 Krobus has special dishes tomorrow");

        return events.AsReadOnly();
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
