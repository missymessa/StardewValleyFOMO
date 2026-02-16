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
    public bool TravelingMerchantDay { get; set; }
    public bool TomorrowTravelingMerchantDay { get; set; }
    public bool NightMarketDay { get; set; }
    public bool TomorrowNightMarketDay { get; set; }
    public bool QueenOfSauceDay { get; set; }
    public string? QueenOfSauceRecipe_ { get; set; }
    public bool TomorrowQueenOfSauceDay { get; set; }
    public double DailyLuck { get; set; } = 0;
    public string LuckDescription_ { get; set; } = "Neutral day.";
    public List<string> TodayEvents_ { get; set; } = new();
    public List<string> TomorrowEvents_ { get; set; } = new();

    public GameDate GetCurrentDate() => CurrentDate;
    public Weather GetCurrentWeather() => CurrentWeather;
    public int GetTimeOfDay() => TimeOfDay;
    public Weather GetTomorrowWeather() => TomorrowWeather;
    public bool IsFestivalDay() => FestivalDay;
    public string? GetFestivalName() => FestivalName_;
    public bool IsTomorrowFestivalDay() => TomorrowFestivalDay;
    public string? GetTomorrowFestivalName() => TomorrowFestivalName_;
    public bool IsTravelingMerchantDay() => TravelingMerchantDay;
    public bool IsTomorrowTravelingMerchantDay() => TomorrowTravelingMerchantDay;
    public bool IsNightMarketDay() => NightMarketDay;
    public bool IsTomorrowNightMarketDay() => TomorrowNightMarketDay;
    public bool IsQueenOfSauceDay() => QueenOfSauceDay;
    public string? GetQueenOfSauceRecipe() => QueenOfSauceRecipe_;
    public bool IsTomorrowQueenOfSauceDay() => TomorrowQueenOfSauceDay;
    public double GetDailyLuck() => DailyLuck;
    public string GetLuckDescription() => LuckDescription_;
    public IReadOnlyList<string> GetTodayEvents() => TodayEvents_.AsReadOnly();
    public IReadOnlyList<string> GetTomorrowEvents() => TomorrowEvents_.AsReadOnly();
}
