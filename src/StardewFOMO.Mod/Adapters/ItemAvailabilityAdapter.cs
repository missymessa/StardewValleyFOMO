using StardewValley;
using StardewValley.GameData.Objects;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using Season = StardewFOMO.Core.Models.Season;
using Weather = StardewFOMO.Core.Models.Weather;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IItemAvailabilityService"/> by checking
/// item data against current game state for season, weather, time, and location.
/// </summary>
public sealed class ItemAvailabilityAdapter : IItemAvailabilityService
{
    private readonly IGameStateProvider _gameState;
    private readonly IFishRepository _fishRepository;
    private readonly IForageRepository _forageRepository;

    public ItemAvailabilityAdapter(
        IGameStateProvider gameState,
        IFishRepository fishRepository,
        IForageRepository forageRepository)
    {
        _gameState = gameState;
        _fishRepository = fishRepository;
        _forageRepository = forageRepository;
    }

    /// <inheritdoc/>
    public ItemAvailability GetAvailability(string itemId)
    {
        var itemName = GetItemDisplayName(itemId);
        var currentDate = _gameState.GetCurrentDate();
        var currentWeather = _gameState.GetCurrentWeather();
        var timeOfDay = _gameState.GetTimeOfDay();

        // Check if it's a fish
        var fish = _fishRepository.GetAllFish().FirstOrDefault(f => f.Id == itemId);
        if (fish != null)
        {
            return GetFishAvailability(fish, itemName, currentDate.Season, currentWeather, timeOfDay);
        }

        // Check if it's a forageable
        var forage = _forageRepository.GetAllForageables().FirstOrDefault(f => f.Id == itemId);
        if (forage != null)
        {
            return GetForageAvailability(forage, itemName, currentDate.Season);
        }

        // Check if it's a crop product (seasonal crops)
        var cropAvailability = GetCropAvailability(itemId, itemName, currentDate.Season);
        if (cropAvailability != null)
        {
            return cropAvailability;
        }

        // Default: assume available (artifacts, gems, crafted items, etc.)
        return ItemAvailability.CreateAvailable(itemId, itemName);
    }

    /// <inheritdoc/>
    public bool IsAvailableToday(string itemId)
    {
        return GetAvailability(itemId).IsAvailableToday;
    }

    private ItemAvailability GetFishAvailability(
        CollectibleItem fish,
        string itemName,
        Season currentSeason,
        Weather currentWeather,
        int timeOfDay)
    {
        // Check season
        if (fish.AvailableSeasons.Count > 0 && !fish.AvailableSeasons.Contains(currentSeason))
        {
            var seasons = string.Join(", ", fish.AvailableSeasons);
            return ItemAvailability.CreateUnavailable(
                fish.Id,
                itemName,
                AvailabilityReason.WrongSeason,
                $"Available in {seasons}");
        }

        // Check weather
        if (fish.RequiredWeather.Count > 0 &&
            !fish.RequiredWeather.Contains(currentWeather) &&
            !(currentWeather == Weather.Stormy && fish.RequiredWeather.Contains(Weather.Rainy)))
        {
            var weather = string.Join(" or ", fish.RequiredWeather);
            return ItemAvailability.CreateUnavailable(
                fish.Id,
                itemName,
                AvailabilityReason.WrongWeather,
                $"Requires {weather} weather");
        }

        // Check time
        if (fish.StartTime.HasValue || fish.EndTime.HasValue)
        {
            var start = fish.StartTime ?? 600;
            var end = fish.EndTime ?? 2600;
            if (timeOfDay < start || timeOfDay > end)
            {
                return ItemAvailability.CreateUnavailable(
                    fish.Id,
                    itemName,
                    AvailabilityReason.WrongTimeOfDay,
                    $"Available {FormatTime(start)} - {FormatTime(end)}");
            }
        }

        return ItemAvailability.CreateAvailable(fish.Id, itemName);
    }

    private static ItemAvailability GetForageAvailability(
        CollectibleItem forage,
        string itemName,
        Season currentSeason)
    {
        // Check season
        if (forage.AvailableSeasons.Count > 0 && !forage.AvailableSeasons.Contains(currentSeason))
        {
            var seasons = string.Join(", ", forage.AvailableSeasons);
            return ItemAvailability.CreateUnavailable(
                forage.Id,
                itemName,
                AvailabilityReason.WrongSeason,
                $"Available in {seasons}");
        }

        return ItemAvailability.CreateAvailable(forage.Id, itemName);
    }

    private ItemAvailability? GetCropAvailability(string itemId, string itemName, Season currentSeason)
    {
        // Check if this is a seasonal crop product
        var cropSeasons = GetCropSeasons(itemId);
        if (cropSeasons == null || cropSeasons.Count == 0)
            return null; // Not a crop or couldn't determine

        if (!cropSeasons.Contains(currentSeason))
        {
            var seasons = string.Join(", ", cropSeasons);
            return ItemAvailability.CreateUnavailable(
                itemId,
                itemName,
                AvailabilityReason.WrongSeason,
                $"Grows in {seasons}");
        }

        return ItemAvailability.CreateAvailable(itemId, itemName);
    }

    private static List<Season>? GetCropSeasons(string itemId)
    {
        // Try to find crop data for this item
        var cropData = Game1.content.Load<Dictionary<string, StardewValley.GameData.Crops.CropData>>("Data\\Crops");
        
        foreach (var crop in cropData.Values)
        {
            if (crop.HarvestItemId == itemId || crop.HarvestItemId == $"(O){itemId}")
            {
                var seasons = new List<Season>();
                foreach (StardewValley.Season sdvSeason in crop.Seasons)
                {
                    switch (sdvSeason.ToString().ToLowerInvariant())
                    {
                        case "spring": seasons.Add(Season.Spring); break;
                        case "summer": seasons.Add(Season.Summer); break;
                        case "fall": seasons.Add(Season.Fall); break;
                        case "winter": seasons.Add(Season.Winter); break;
                    }
                }
                if (seasons.Count > 0)
                    return seasons;
            }
        }

        return null;
    }

    private static string GetItemDisplayName(string itemId)
    {
        // Try to get from Objects data
        var objectsData = Game1.content.Load<Dictionary<string, ObjectData>>("Data\\Objects");
        if (objectsData.TryGetValue(itemId, out var objData))
        {
            return objData.DisplayName ?? objData.Name ?? itemId;
        }

        // Try parsing qualified item ID
        var qualifiedId = itemId.StartsWith("(") ? itemId : $"(O){itemId}";
        var item = ItemRegistry.Create(qualifiedId, allowNull: true);
        return item?.DisplayName ?? itemId;
    }

    private static string FormatTime(int time)
    {
        var hours = time / 100;
        var minutes = time % 100;
        var period = hours >= 12 ? "PM" : "AM";
        var displayHours = hours > 12 ? hours - 12 : (hours == 0 ? 12 : hours);
        return minutes == 0 ? $"{displayHours}{period}" : $"{displayHours}:{minutes:D2}{period}";
    }
}
