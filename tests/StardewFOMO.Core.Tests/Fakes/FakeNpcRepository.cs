using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Tests.Fakes;

/// <summary>Fake implementation of <see cref="INpcRepository"/> with in-memory NPC data.</summary>
public sealed class FakeNpcRepository : INpcRepository
{
    private readonly List<NpcBirthday> _birthdays = new();
    private readonly Dictionary<string, List<string>> _lovedGifts = new();
    private readonly Dictionary<string, List<string>> _likedGifts = new();

    public void AddBirthday(NpcBirthday birthday)
    {
        _birthdays.Add(birthday);
        _lovedGifts[birthday.NpcName] = birthday.LovedGifts.ToList();
        _likedGifts[birthday.NpcName] = birthday.LikedGifts.ToList();
    }

    public IReadOnlyList<NpcBirthday> GetAllBirthdays() => _birthdays.AsReadOnly();
    public IReadOnlyList<string> GetLovedGifts(string npcName) =>
        _lovedGifts.TryGetValue(npcName, out var gifts) ? gifts.AsReadOnly() : Array.Empty<string>();
    public IReadOnlyList<string> GetLikedGifts(string npcName) =>
        _likedGifts.TryGetValue(npcName, out var gifts) ? gifts.AsReadOnly() : Array.Empty<string>();
}
