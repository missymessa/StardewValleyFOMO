using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="IGameStateProvider"/> with settable properties.</summary>
public sealed class FakeGameStateProvider : IGameStateProvider
{
    public GameDate CurrentDate { get; set; } = new() { Season = Season.Spring, Day = 1, Year = 1 };
    public Weather CurrentWeather { get; set; } = Weather.Sunny;
    public int TimeOfDay { get; set; } = 600;
    public Weather TomorrowWeather { get; set; } = Weather.Sunny;
    public bool FestivalDay { get; set; }
    public string? FestivalName_ { get; set; }
    public bool TomorrowFestivalDay { get; set; }
    public string? TomorrowFestivalName_ { get; set; }

    public GameDate GetCurrentDate() => CurrentDate;
    public Weather GetCurrentWeather() => CurrentWeather;
    public int GetTimeOfDay() => TimeOfDay;
    public Weather GetTomorrowWeather() => TomorrowWeather;
    public bool IsFestivalDay() => FestivalDay;
    public string? GetFestivalName() => FestivalName_;
    public bool IsTomorrowFestivalDay() => TomorrowFestivalDay;
    public string? GetTomorrowFestivalName() => TomorrowFestivalName_;
}
