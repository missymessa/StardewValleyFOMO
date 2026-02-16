using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Orchestrates all sub-services to build a complete daily summary.
/// Provides both priority view (bundle-needed, time-sensitive, birthdays, alerts)
/// and "Show All" view (full collection tracking).
/// </summary>
public sealed class DailySummaryService
{
    private readonly FishAvailabilityService _fishAvailability;
    private readonly ForageAvailabilityService _forageAvailability;
    private readonly BundleTrackingService _bundleTracking;
    private readonly CollectionTrackingService _collectionTracking;
    private readonly BirthdayService _birthdayService;
    private readonly TomorrowPreviewService _tomorrowPreview;
    private readonly SeasonAlertService _seasonAlert;
    private readonly IGameStateProvider _gameState;
    private readonly ILogger _logger;

    /// <summary>Initializes a new instance of <see cref="DailySummaryService"/>.</summary>
    public DailySummaryService(
        FishAvailabilityService fishAvailability,
        ForageAvailabilityService forageAvailability,
        BundleTrackingService bundleTracking,
        CollectionTrackingService collectionTracking,
        BirthdayService birthdayService,
        TomorrowPreviewService tomorrowPreview,
        SeasonAlertService seasonAlert,
        IGameStateProvider gameState,
        ILogger logger)
    {
        _fishAvailability = fishAvailability;
        _forageAvailability = forageAvailability;
        _bundleTracking = bundleTracking;
        _collectionTracking = collectionTracking;
        _birthdayService = birthdayService;
        _tomorrowPreview = tomorrowPreview;
        _seasonAlert = seasonAlert;
        _gameState = gameState;
        _logger = logger;
    }

    /// <summary>
    /// Build a daily summary with all sections populated (priority view).
    /// This is the default view shown when the planner opens.
    /// </summary>
    public DailySummary GetDailySummary()
    {
        var date = _gameState.GetCurrentDate();
        var weather = _gameState.GetCurrentWeather();

        _logger.Log(LogLevel.Info,
            $"Building daily summary for {date}, weather={weather}",
            new { date.Season, date.Day, date.Year, weather });

        // Gather base collectibles
        var fish = _fishAvailability.GetAvailableFish();
        var forageables = _forageAvailability.GetAvailableForageables();

        // Annotate with bundle info
        var annotatedFish = _bundleTracking.AnnotateWithBundleInfo(fish);
        var annotatedForage = _bundleTracking.AnnotateWithBundleInfo(forageables);

        // Filter bundle-needed items
        var bundleNeeded = annotatedFish.Concat(annotatedForage)
            .Where(i => i.IsNeededForBundle)
            .ToList()
            .AsReadOnly();

        // Birthdays
        var todayBirthdays = _birthdayService.GetTodayBirthdays();
        var upcomingBirthdays = _birthdayService.GetUpcomingBirthdays();

        // Tomorrow preview
        var tomorrow = _tomorrowPreview.GetTomorrowPreview();

        // Season alerts
        var alerts = _seasonAlert.GetLastChanceAlerts();

        // Festival info
        var isFestival = _gameState.IsFestivalDay();
        var festivalName = _gameState.GetFestivalName();

        _logger.Log(LogLevel.Info,
            $"Daily summary complete: {annotatedFish.Count} fish, {annotatedForage.Count} forageables, " +
            $"{bundleNeeded.Count} bundle-needed, {todayBirthdays.Count} birthdays today, {alerts.Count} alerts",
            new
            {
                FishCount = annotatedFish.Count,
                ForageCount = annotatedForage.Count,
                BundleNeededCount = bundleNeeded.Count,
                TodayBirthdayCount = todayBirthdays.Count,
                AlertCount = alerts.Count
            });

        return new DailySummary
        {
            Date = date,
            CurrentWeather = weather,
            AvailableFish = annotatedFish,
            AvailableForageables = annotatedForage,
            BundleNeededItems = bundleNeeded,
            TodayBirthdays = todayBirthdays,
            UpcomingBirthdays = upcomingBirthdays,
            TomorrowPreview = tomorrow,
            LastChanceAlerts = alerts,
            IsFestivalDay = isFestival,
            FestivalName = festivalName
        };
    }

    /// <summary>
    /// Build a priority-focused summary: bundle-needed items, time-sensitive items,
    /// birthdays, and last-chance alerts. Omits full collection tracking.
    /// </summary>
    public DailySummary GetPrioritySummary()
    {
        _logger.Log(LogLevel.Debug, "Building priority summary");
        return GetDailySummary(); // Priority view IS the default daily summary
    }

    /// <summary>
    /// Build a "Show All" summary that includes everything from the priority view
    /// plus full collection tracking (shipping, museum, cooking, crafting).
    /// </summary>
    public DailySummary GetShowAllSummary()
    {
        _logger.Log(LogLevel.Debug, "Building 'Show All' summary with full collection tracking");

        var baseSummary = GetDailySummary();

        // Gather all collection items
        var allItems = new List<CollectibleItem>();

        // Fish and forageables from today
        allItems.AddRange(baseSummary.AvailableFish);
        allItems.AddRange(baseSummary.AvailableForageables);

        // Cooking recipes
        var cookingRecipes = _collectionTracking.GetCookingRecipeStatus();
        allItems.AddRange(cookingRecipes);

        // Crafting recipes
        var craftingRecipes = _collectionTracking.GetCraftingRecipeStatus();
        allItems.AddRange(craftingRecipes);

        _logger.Log(LogLevel.Info,
            $"'Show All' view: {allItems.Count} total items " +
            $"({baseSummary.AvailableFish.Count} fish, {baseSummary.AvailableForageables.Count} forageables, " +
            $"{cookingRecipes.Count} cooking, {craftingRecipes.Count} crafting)",
            new
            {
                TotalItems = allItems.Count,
                Fish = baseSummary.AvailableFish.Count,
                Forage = baseSummary.AvailableForageables.Count,
                Cooking = cookingRecipes.Count,
                Crafting = craftingRecipes.Count
            });

        return new DailySummary
        {
            Date = baseSummary.Date,
            CurrentWeather = baseSummary.CurrentWeather,
            AvailableFish = baseSummary.AvailableFish,
            AvailableForageables = baseSummary.AvailableForageables,
            BundleNeededItems = baseSummary.BundleNeededItems,
            TodayBirthdays = baseSummary.TodayBirthdays,
            UpcomingBirthdays = baseSummary.UpcomingBirthdays,
            TomorrowPreview = baseSummary.TomorrowPreview,
            LastChanceAlerts = baseSummary.LastChanceAlerts,
            AllCollectionItems = allItems.AsReadOnly(),
            IsFestivalDay = baseSummary.IsFestivalDay,
            FestivalName = baseSummary.FestivalName
        };
    }
}
