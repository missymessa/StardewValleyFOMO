using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Interfaces;

/// <summary>
/// Abstracts game date, weather, and time state.
/// Implemented by SMAPI adapter reading from <c>Game1</c>.
/// </summary>
public interface IGameStateProvider
{
    /// <summary>Get the current in-game date (season, day, year).</summary>
    GameDate GetCurrentDate();

    /// <summary>Get the current weather condition.</summary>
    Weather GetCurrentWeather();

    /// <summary>Get the current time of day (e.g., 600 for 6:00 AM, 1430 for 2:30 PM).</summary>
    int GetTimeOfDay();

    /// <summary>Get tomorrow's weather forecast.</summary>
    Weather GetTomorrowWeather();

    /// <summary>Check whether today is a festival day.</summary>
    bool IsFestivalDay();

    /// <summary>Get the festival name if today is a festival day; null otherwise.</summary>
    string? GetFestivalName();

    /// <summary>Check whether tomorrow is a festival day.</summary>
    bool IsTomorrowFestivalDay();

    /// <summary>Get tomorrow's festival name if applicable; null otherwise.</summary>
    string? GetTomorrowFestivalName();

    /// <summary>Check if the Traveling Merchant is visiting today (Friday/Sunday).</summary>
    bool IsTravelingMerchantDay();

    /// <summary>Check if the Traveling Merchant visits tomorrow.</summary>
    bool IsTomorrowTravelingMerchantDay();

    /// <summary>Check if today is a Night Market day (Winter 15-17).</summary>
    bool IsNightMarketDay();

    /// <summary>Check if tomorrow is a Night Market day.</summary>
    bool IsTomorrowNightMarketDay();

    /// <summary>Check if Queen of Sauce airs today (Sunday).</summary>
    bool IsQueenOfSauceDay();

    /// <summary>Get the recipe being taught today on Queen of Sauce, if any.</summary>
    string? GetQueenOfSauceRecipe();

    /// <summary>Check if Queen of Sauce airs tomorrow.</summary>
    bool IsTomorrowQueenOfSauceDay();

    /// <summary>Get today's daily luck value (-1 to 1 scale).</summary>
    double GetDailyLuck();

    /// <summary>Get a friendly description of today's luck.</summary>
    string GetLuckDescription();

    /// <summary>Get all special events happening today (excluding festivals).</summary>
    IReadOnlyList<string> GetTodayEvents();

    /// <summary>Get all special events happening tomorrow (excluding festivals).</summary>
    IReadOnlyList<string> GetTomorrowEvents();
}
