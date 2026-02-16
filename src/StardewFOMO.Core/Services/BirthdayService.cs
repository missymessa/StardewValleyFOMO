using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Finds today's and upcoming NPC birthdays within a configurable lookahead window.
/// </summary>
public sealed class BirthdayService
{
    private readonly INpcRepository _npcRepository;
    private readonly IGameStateProvider _gameStateProvider;
    private readonly ILogger _logger;
    private readonly int _lookaheadDays;

    /// <summary>Initializes a new instance of <see cref="BirthdayService"/>.</summary>
    public BirthdayService(
        INpcRepository npcRepository,
        IGameStateProvider gameStateProvider,
        ILogger logger,
        int lookaheadDays = 7)
    {
        _npcRepository = npcRepository;
        _gameStateProvider = gameStateProvider;
        _logger = logger;
        _lookaheadDays = lookaheadDays;
    }

    /// <summary>Get NPC birthdays occurring today.</summary>
    public IReadOnlyList<NpcBirthday> GetTodayBirthdays()
    {
        var today = _gameStateProvider.GetCurrentDate();
        var allBirthdays = _npcRepository.GetAllBirthdays();

        var todayBirthdays = allBirthdays
            .Where(b => b.BirthdayDate.Season == today.Season && b.BirthdayDate.Day == today.Day)
            .ToList();

        _logger.Log(Interfaces.LogLevel.Info,
            $"Found {todayBirthdays.Count} birthday(s) today ({today})",
            new { Count = todayBirthdays.Count, today.Season, today.Day });

        return todayBirthdays.AsReadOnly();
    }

    /// <summary>
    /// Get NPC birthdays within the lookahead window (excludes today).
    /// </summary>
    public IReadOnlyList<NpcBirthday> GetUpcomingBirthdays()
    {
        var today = _gameStateProvider.GetCurrentDate();
        var allBirthdays = _npcRepository.GetAllBirthdays();

        var upcoming = allBirthdays
            .Where(b =>
            {
                if (b.BirthdayDate.Season == today.Season && b.BirthdayDate.Day == today.Day)
                    return false; // Exclude today

                var daysUntil = today.DaysUntil(b.BirthdayDate);
                return daysUntil > 0 && daysUntil <= _lookaheadDays;
            })
            .OrderBy(b => today.DaysUntil(b.BirthdayDate))
            .ToList();

        _logger.Log(Interfaces.LogLevel.Debug,
            $"Found {upcoming.Count} upcoming birthday(s) within {_lookaheadDays} days",
            new { Count = upcoming.Count, LookaheadDays = _lookaheadDays });

        return upcoming.AsReadOnly();
    }

    /// <summary>
    /// Get the next birthday on the calendar (useful when no birthdays are in the lookahead window).
    /// Returns null if no birthdays exist.
    /// </summary>
    public NpcBirthday? GetNextBirthday()
    {
        var today = _gameStateProvider.GetCurrentDate();
        var allBirthdays = _npcRepository.GetAllBirthdays();

        if (allBirthdays.Count == 0)
            return null;

        return allBirthdays
            .Where(b => !(b.BirthdayDate.Season == today.Season && b.BirthdayDate.Day == today.Day))
            .OrderBy(b => today.DaysUntil(b.BirthdayDate))
            .FirstOrDefault();
    }
}
