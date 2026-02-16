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
}
