using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Tests.Fakes;
using Xunit;

namespace StardewFOMO.Core.Tests.Services;

public class BirthdayServiceTests
{
    private readonly FakeGameStateProvider _gameState = new();
    private readonly FakeNpcRepository _npcRepo = new();
    private readonly TestLogger _logger = new();

    private BirthdayService CreateService(int lookaheadDays = 7) =>
        new(_npcRepo, _gameState, _logger, lookaheadDays);

    #region Today's Birthday

    [Fact]
    public void GetTodayBirthdays_HasBirthdayToday_ReturnsBirthday()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 14, Year = 1 };

        _npcRepo.AddBirthday(new NpcBirthday
        {
            NpcName = "Haley",
            BirthdayDate = new GameDate { Season = Season.Spring, Day = 14, Year = 1 },
            LovedGifts = new[] { "Sunflower", "Pink Cake" },
            LikedGifts = new[] { "Coconut", "Fruit Salad" }
        });

        var result = CreateService().GetTodayBirthdays();

        Assert.Single(result);
        Assert.Equal("Haley", result[0].NpcName);
        Assert.Contains("Sunflower", result[0].LovedGifts);
    }

    [Fact]
    public void GetTodayBirthdays_NoBirthdayToday_ReturnsEmpty()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 15, Year = 1 };

        _npcRepo.AddBirthday(new NpcBirthday
        {
            NpcName = "Haley",
            BirthdayDate = new GameDate { Season = Season.Spring, Day = 14, Year = 1 },
            LovedGifts = new[] { "Sunflower" },
            LikedGifts = Array.Empty<string>()
        });

        var result = CreateService().GetTodayBirthdays();

        Assert.Empty(result);
    }

    #endregion

    #region Upcoming Birthdays (7-day lookahead)

    [Fact]
    public void GetUpcomingBirthdays_WithinLookahead_ReturnsBirthday()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 11, Year = 1 };

        _npcRepo.AddBirthday(new NpcBirthday
        {
            NpcName = "Haley",
            BirthdayDate = new GameDate { Season = Season.Spring, Day = 14, Year = 1 },
            LovedGifts = new[] { "Sunflower" },
            LikedGifts = Array.Empty<string>()
        });

        var result = CreateService().GetUpcomingBirthdays();

        Assert.Single(result);
        Assert.Equal("Haley", result[0].NpcName);
    }

    [Fact]
    public void GetUpcomingBirthdays_BeyondLookahead_ExcludesBirthday()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 1, Year = 1 };

        _npcRepo.AddBirthday(new NpcBirthday
        {
            NpcName = "Haley",
            BirthdayDate = new GameDate { Season = Season.Spring, Day = 14, Year = 1 },
            LovedGifts = new[] { "Sunflower" },
            LikedGifts = Array.Empty<string>()
        });

        var result = CreateService().GetUpcomingBirthdays();

        Assert.Empty(result);
    }

    [Fact]
    public void GetUpcomingBirthdays_ExcludesTodaysBirthday()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 14, Year = 1 };

        _npcRepo.AddBirthday(new NpcBirthday
        {
            NpcName = "Haley",
            BirthdayDate = new GameDate { Season = Season.Spring, Day = 14, Year = 1 },
            LovedGifts = new[] { "Sunflower" },
            LikedGifts = Array.Empty<string>()
        });

        var result = CreateService().GetUpcomingBirthdays();

        Assert.Empty(result); // Today's birthday is in GetTodayBirthdays, not upcoming
    }

    #endregion

    #region Cross-Season Lookahead

    [Fact]
    public void GetUpcomingBirthdays_CrossSeasonBoundary_IncludesNextSeasonBirthdays()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 26, Year = 1 };

        _npcRepo.AddBirthday(new NpcBirthday
        {
            NpcName = "Sam",
            BirthdayDate = new GameDate { Season = Season.Summer, Day = 1, Year = 1 },
            LovedGifts = new[] { "Cactus Fruit" },
            LikedGifts = Array.Empty<string>()
        });

        var result = CreateService().GetUpcomingBirthdays();

        // Spring 26 + 3 days = Summer 1 — within 7-day window
        Assert.Single(result);
        Assert.Equal("Sam", result[0].NpcName);
    }

    #endregion

    #region Next Birthday (when none in window)

    [Fact]
    public void GetNextBirthday_NoneInWindow_ReturnsNextOnCalendar()
    {
        _gameState.CurrentDate = new GameDate { Season = Season.Spring, Day = 1, Year = 1 };

        _npcRepo.AddBirthday(new NpcBirthday
        {
            NpcName = "Sam",
            BirthdayDate = new GameDate { Season = Season.Summer, Day = 17, Year = 1 },
            LovedGifts = new[] { "Cactus Fruit" },
            LikedGifts = Array.Empty<string>()
        });

        var result = CreateService().GetNextBirthday();

        Assert.NotNull(result);
        Assert.Equal("Sam", result!.NpcName);
    }

    #endregion
}
