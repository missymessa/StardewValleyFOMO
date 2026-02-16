namespace StardewFOMO.Core.Models;

/// <summary>
/// Represents an in-game date. Date-only — weather is modeled separately via <see cref="Weather"/>.
/// </summary>
public sealed class GameDate
{
    /// <summary>The season (Spring, Summer, Fall, Winter).</summary>
    public Season Season { get; init; }

    /// <summary>The day of the month (1–28).</summary>
    public int Day { get; init; }

    /// <summary>The year number (1-based).</summary>
    public int Year { get; init; }

    /// <summary>The day of the week (derived from day number).</summary>
    public DayOfWeek DayOfWeek => (DayOfWeek)((Day - 1) % 7);

    /// <summary>Total number of days in a single season.</summary>
    public const int DaysPerSeason = 28;

    /// <summary>Check whether this date is within the last N days of the season.</summary>
    public bool IsWithinLastDays(int days) => Day > DaysPerSeason - days;

    /// <summary>Check whether tomorrow would be a new season.</summary>
    public bool IsTomorrowNewSeason => Day == DaysPerSeason;

    /// <summary>Get the next day's date (handles season/year rollover).</summary>
    public GameDate GetTomorrow()
    {
        if (Day < DaysPerSeason)
            return new GameDate { Season = Season, Day = Day + 1, Year = Year };

        var nextSeason = (Season)(((int)Season + 1) % 4);
        var nextYear = nextSeason == Season.Spring ? Year + 1 : Year;
        return new GameDate { Season = nextSeason, Day = 1, Year = nextYear };
    }

    /// <summary>Calculate the number of days until a target date (within same year or next).</summary>
    public int DaysUntil(GameDate target)
    {
        int thisDayOfYear = (int)Season * DaysPerSeason + Day;
        int targetDayOfYear = (int)target.Season * DaysPerSeason + target.Day;

        if (targetDayOfYear >= thisDayOfYear)
            return targetDayOfYear - thisDayOfYear;

        // Target is in next year
        return (4 * DaysPerSeason - thisDayOfYear) + targetDayOfYear;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Season} {Day}, Year {Year}";

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is GameDate other && Season == other.Season && Day == other.Day && Year == other.Year;

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Season, Day, Year);
}

/// <summary>Days of the week in Stardew Valley.</summary>
public enum DayOfWeek
{
    /// <summary>Monday.</summary>
    Monday,
    /// <summary>Tuesday.</summary>
    Tuesday,
    /// <summary>Wednesday.</summary>
    Wednesday,
    /// <summary>Thursday.</summary>
    Thursday,
    /// <summary>Friday.</summary>
    Friday,
    /// <summary>Saturday.</summary>
    Saturday,
    /// <summary>Sunday.</summary>
    Sunday
}
