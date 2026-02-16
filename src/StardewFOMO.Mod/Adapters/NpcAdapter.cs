using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.TokenizableStrings;
using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;
using Season = StardewFOMO.Core.Models.Season;

namespace StardewFOMO.Mod.Adapters;

/// <summary>
/// SMAPI adapter implementing <see cref="INpcRepository"/> by reading NPC data from game content.
/// Uses SDV 1.6+ <c>Data\Characters</c> with <see cref="CharacterData"/> objects.
/// </summary>
public sealed class NpcAdapter : INpcRepository
{
    /// <inheritdoc/>
    public IReadOnlyList<NpcBirthday> GetAllBirthdays()
    {
        var birthdays = new List<NpcBirthday>();
        var characters = Game1.content.Load<Dictionary<string, CharacterData>>("Data\\Characters");

        foreach (var kvp in characters)
        {
            var npcName = kvp.Key;
            var charData = kvp.Value;

            // Skip non-giftable NPCs
            if (charData.CanReceiveGifts == false)
                continue;

            // BirthSeason is StardewValley.Season? in SDV 1.6
            if (charData.BirthSeason.HasValue && charData.BirthDay > 0)
            {
                var season = ConvertSeason(charData.BirthSeason.Value);
                // Resolve localized display name using TokenParser
                var displayName = ResolveDisplayName(charData.DisplayName, npcName);
                birthdays.Add(new NpcBirthday
                {
                    NpcName = displayName,
                    BirthdayDate = new GameDate { Season = season, Day = charData.BirthDay, Year = 1 },
                    LovedGifts = GetLovedGifts(npcName),
                    LikedGifts = GetLikedGifts(npcName)
                });
            }
        }

        return birthdays.AsReadOnly();
    }

    private static string ResolveDisplayName(string? displayName, string fallback)
    {
        if (string.IsNullOrEmpty(displayName))
            return fallback;

        // Use TokenParser to resolve localized strings like "[LocalizedText Strings\NPCNames:Willy]"
        return TokenParser.ParseText(displayName) ?? fallback;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetLovedGifts(string npcName)
    {
        // Try to get from the NPC's Character data
        var result = GetGiftTasteItemsFromCharacter(npcName, "Love");
        if (result.Count > 0)
            return result;
        
        // Fallback to traditional NPCGiftTastes
        return GetGiftTasteItemsLegacy(npcName, 0);
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetLikedGifts(string npcName)
    {
        var result = GetGiftTasteItemsFromCharacter(npcName, "Like");
        if (result.Count > 0)
            return result;
        
        return GetGiftTasteItemsLegacy(npcName, 2);
    }

    private static IReadOnlyList<string> GetGiftTasteItemsFromCharacter(string npcName, string tasteLevel)
    {
        // Try to find the NPC and get their actual favorites
        var npc = Game1.getCharacterFromName(npcName);
        if (npc == null)
            return Array.Empty<string>();

        var names = new List<string>();

        // Scan object data for items this NPC loves/likes
        if (Game1.objectData != null)
        {
            foreach (var kvp in Game1.objectData)
            {
                var itemId = kvp.Key;
                var taste = npc.getGiftTasteForThisItem(ItemRegistry.Create(itemId));
                
                bool isMatch = tasteLevel switch
                {
                    "Love" => taste == 0,  // GiftTaste.Love
                    "Like" => taste == 2,  // GiftTaste.Like
                    _ => false
                };

                if (isMatch && names.Count < 8)
                {
                    var displayName = TokenParser.ParseText(kvp.Value.DisplayName) ?? kvp.Value.Name ?? itemId;
                    names.Add(displayName);
                }
            }
        }

        return names.AsReadOnly();
    }

    private static IReadOnlyList<string> GetGiftTasteItemsLegacy(string npcName, int tasteIndex)
    {
        var giftTastes = Game1.NPCGiftTastes;
        if (giftTastes == null || !giftTastes.TryGetValue(npcName, out var tasteData))
            return Array.Empty<string>();

        // Gift taste format: "Love items/Love dialogue/Like items/Like dialogue/..."
        var sections = tasteData.Split('/');

        if (sections.Length <= tasteIndex)
            return Array.Empty<string>();

        var itemStr = sections[tasteIndex].Trim();
        if (string.IsNullOrEmpty(itemStr))
            return Array.Empty<string>();

        var items = itemStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var names = new List<string>();

        foreach (var itemIdStr in items)
        {
            if (itemIdStr.StartsWith("-"))
                continue;

            if (Game1.objectData != null && Game1.objectData.TryGetValue(itemIdStr, out var info))
            {
                var displayName = TokenParser.ParseText(info.DisplayName) ?? info.Name ?? itemIdStr;
                names.Add(displayName);
            }
        }

        return names.AsReadOnly();
    }

    private static Season ConvertSeason(StardewValley.Season sdvSeason)
    {
        return sdvSeason switch
        {
            StardewValley.Season.Spring => Season.Spring,
            StardewValley.Season.Summer => Season.Summer,
            StardewValley.Season.Fall => Season.Fall,
            StardewValley.Season.Winter => Season.Winter,
            _ => Season.Spring
        };
    }
}
