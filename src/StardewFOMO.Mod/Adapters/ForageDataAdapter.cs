using StardewValley;
using StardewValley.GameData.Locations;
using StardewValley.TokenizableStrings;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using Season = StardewFOMO.Core.Models.Season;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="IForageRepository"/> by reading forage data from game content.
/// Uses SDV 1.6+ <c>Data\Locations</c> with <see cref="LocationData"/> objects.
/// </summary>
public sealed class ForageDataAdapter : IForageRepository
{
    private List<CollectibleItem>? _forageCache;

    /// <inheritdoc/>
    public IReadOnlyList<CollectibleItem> GetAllForageables()
    {
        if (_forageCache != null)
            return _forageCache.AsReadOnly();

        _forageCache = new List<CollectibleItem>();
        var locationData = Game1.content.Load<Dictionary<string, LocationData>>("Data\\Locations");
        var seenItems = new Dictionary<string, CollectibleItem>();

        foreach (var kvp in locationData)
        {
            var locationName = kvp.Key;
            var locData = kvp.Value;

            if (locData.Forage == null || locData.Forage.Count == 0)
                continue;

            foreach (var forageEntry in locData.Forage)
            {
                if (string.IsNullOrEmpty(forageEntry.ItemId))
                    continue;

                var itemId = forageEntry.ItemId;
                var seasons = ParseSeasonsFromCondition(forageEntry.Condition);

                // If no season condition, available all year
                if (seasons.Count == 0)
                    seasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter };

                if (seenItems.TryGetValue(itemId, out var existing))
                {
                    // Merge seasons and locations
                    var mergedSeasons = existing.AvailableSeasons.ToList();
                    foreach (var s in seasons)
                    {
                        if (!mergedSeasons.Contains(s))
                            mergedSeasons.Add(s);
                    }

                    var mergedLocations = existing.Locations.ToList();
                    if (!mergedLocations.Contains(locationName))
                        mergedLocations.Add(locationName);

                    seenItems[itemId] = new CollectibleItem
                    {
                        Id = existing.Id,
                        Name = existing.Name,
                        CollectionType = existing.CollectionType,
                        AvailableSeasons = mergedSeasons.AsReadOnly(),
                        Locations = mergedLocations.AsReadOnly()
                    };
                }
                else
                {
                    var name = GetItemName(itemId);

                    seenItems[itemId] = new CollectibleItem
                    {
                        Id = itemId,
                        Name = name,
                        CollectionType = CollectionType.Forage,
                        AvailableSeasons = seasons.AsReadOnly(),
                        Locations = new[] { locationName }
                    };
                }
            }
        }

        _forageCache = seenItems.Values.ToList();
        return _forageCache.AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<CollectibleItem> GetForageablesBySeason(Season season)
    {
        return GetAllForageables()
            .Where(f => f.AvailableSeasons.Contains(season))
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyList<CollectibleItem> GetSeasonExclusiveForageables(Season season)
    {
        return GetAllForageables()
            .Where(f => f.AvailableSeasons.Count == 1 && f.AvailableSeasons[0] == season)
            .ToList()
            .AsReadOnly();
    }

    private static List<Season> ParseSeasonsFromCondition(string? condition)
    {
        var seasons = new List<Season>();
        if (string.IsNullOrEmpty(condition))
            return seasons;

        var condLower = condition.ToLowerInvariant();
        if (condLower.Contains("spring"))
            seasons.Add(Season.Spring);
        if (condLower.Contains("summer"))
            seasons.Add(Season.Summer);
        if (condLower.Contains("fall"))
            seasons.Add(Season.Fall);
        if (condLower.Contains("winter"))
            seasons.Add(Season.Winter);

        return seasons;
    }

    private static string GetItemName(string itemId)
    {
        // Try qualified item ID first (e.g., "(O)16")
        var unqualified = itemId.StartsWith("(O)") ? itemId[3..] : itemId;

        if (Game1.objectData != null && Game1.objectData.TryGetValue(unqualified, out var info))
        {
            // Resolve tokenized display names like "[LocalizedText Strings\Objects:Daffodil_Name]"
            return TokenParser.ParseText(info.DisplayName) ?? info.Name ?? $"Item {itemId}";
        }

        return $"Item {itemId}";
    }
}
