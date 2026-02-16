using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Aggregates all perfection category services to calculate overall perfection progress.
/// </summary>
public sealed class PerfectionCalculatorService
{
    private readonly ShippingProgressService _shippingService;
    private readonly FishProgressService _fishService;
    private readonly CookingProgressService _cookingService;
    private readonly CraftingProgressService _craftingService;
    private readonly FriendshipProgressService _friendshipService;
    private readonly BuildingProgressService _buildingService;
    private readonly MonsterProgressService _monsterService;
    private readonly StardropProgressService _stardropService;
    private readonly WalnutProgressService _walnutService;
    private readonly SkillProgressService _skillService;
    private readonly IPerfectionRepository _perfectionRepo;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerfectionCalculatorService"/> class.
    /// </summary>
    public PerfectionCalculatorService(
        ShippingProgressService shippingService,
        FishProgressService fishService,
        CookingProgressService cookingService,
        CraftingProgressService craftingService,
        FriendshipProgressService friendshipService,
        BuildingProgressService buildingService,
        MonsterProgressService monsterService,
        StardropProgressService stardropService,
        WalnutProgressService walnutService,
        SkillProgressService skillService,
        IPerfectionRepository perfectionRepo,
        ILogger logger)
    {
        _shippingService = shippingService;
        _fishService = fishService;
        _cookingService = cookingService;
        _craftingService = craftingService;
        _friendshipService = friendshipService;
        _buildingService = buildingService;
        _monsterService = monsterService;
        _stardropService = stardropService;
        _walnutService = walnutService;
        _skillService = skillService;
        _perfectionRepo = perfectionRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the overall perfection progress aggregating all categories.
    /// </summary>
    public PerfectionProgress GetProgress()
    {
        var categories = new List<PerfectionCategory>
        {
            _shippingService.GetProgress(),
            _fishService.GetProgress(),
            _cookingService.GetProgress(),
            _craftingService.GetProgress(),
            _friendshipService.GetProgress(),
            _buildingService.GetProgress(),
            _monsterService.GetProgress(),
            _stardropService.GetProgress(),
            _walnutService.GetProgress(),
            _skillService.GetProgress()
        };

        var totalPercentage = CalculateTotalPercentage(categories);
        var isGingerIslandUnlocked = _perfectionRepo.IsGingerIslandUnlocked();

        _logger.Log(LogLevel.Trace, $"Perfection progress: {totalPercentage:F1}% (Ginger Island: {isGingerIslandUnlocked})");

        return new PerfectionProgress
        {
            TotalPercentage = totalPercentage,
            Categories = categories.AsReadOnly(),
            GingerIslandUnlocked = isGingerIslandUnlocked
        };
    }

    /// <summary>
    /// Gets progress for a specific category.
    /// </summary>
    public PerfectionCategory GetCategoryProgress(string categoryName)
    {
        return categoryName.ToLowerInvariant() switch
        {
            "shipping" => _shippingService.GetProgress(),
            "fish" => _fishService.GetProgress(),
            "cooking" => _cookingService.GetProgress(),
            "crafting" => _craftingService.GetProgress(),
            "friendship" => _friendshipService.GetProgress(),
            "buildings" => _buildingService.GetProgress(),
            "monster slayer" or "monsters" => _monsterService.GetProgress(),
            "stardrops" => _stardropService.GetProgress(),
            "golden walnuts" or "walnuts" => _walnutService.GetProgress(),
            "skills" => _skillService.GetProgress(),
            _ => throw new ArgumentException($"Unknown category: {categoryName}", nameof(categoryName))
        };
    }

    /// <summary>
    /// Gets the incomplete items for a specific category.
    /// </summary>
    public IReadOnlyList<string> GetIncompleteItems(string categoryName, int maxItems = 10)
    {
        return categoryName.ToLowerInvariant() switch
        {
            "shipping" => GetShippingIncomplete(maxItems),
            "fish" => _fishService.GetUncaughtFish().Take(maxItems).Select(f => f.DisplayName).ToList(),
            "cooking" => _cookingService.GetUncookedRecipes().Take(maxItems).Select(r => r.DisplayName).ToList(),
            "crafting" => _craftingService.GetUncraftedRecipes().Take(maxItems).Select(r => r.DisplayName).ToList(),
            "friendship" => _friendshipService.GetIncompleteFriendships().Take(maxItems).Select(f => $"{f.NpcName} ({f.CurrentHearts}/{f.MaxHearts}♥)").ToList(),
            "buildings" => _buildingService.GetUnbuiltBuildings().Take(maxItems).Select(b => b.DisplayName).ToList(),
            "monster slayer" => _monsterService.GetIncompleteGoals().Take(maxItems).Select(g => $"{g.DisplayName} ({g.CurrentKills}/{g.RequiredKills})").ToList(),
            "stardrops" => _stardropService.GetUncollectedStardrops().Take(maxItems).Select(s => s.DisplayName).ToList(),
            "golden walnuts" => _walnutService.GetRemainingWalnutGroups().Take(maxItems).Select(g => $"{g.DisplayName} ({g.CollectedCount}/{g.TotalCount})").ToList(),
            "skills" => _skillService.GetIncompleteSkills().Take(maxItems).Select(s => $"{s.SkillName} (Level {s.CurrentLevel})").ToList(),
            _ => Array.Empty<string>().ToList()
        };
    }

    private List<string> GetShippingIncomplete(int maxItems)
    {
        // Shipping doesn't have detailed items yet, return a generic message
        var progress = _shippingService.GetProgress();
        var remaining = progress.TotalCount - progress.CurrentCount;
        return new List<string> { $"{remaining} items remaining to ship" };
    }

    private static double CalculateTotalPercentage(IReadOnlyList<PerfectionCategory> categories)
    {
        // Sum of (category percent complete * category weight / 100)
        // Each category contributes proportionally to its weight
        return categories.Sum(c => c.ContributionToTotal);
    }
}
