using StardewFOMO.Core.Interfaces;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Core.Services;

/// <summary>
/// Service for tracking friendship progress toward perfection.
/// </summary>
public sealed class FriendshipProgressService
{
    private readonly IFriendshipRepository _friendshipRepo;
    private readonly ILogger _logger;

    // Weight contribution to overall perfection (11%)
    private const double PerfectionWeight = 11.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendshipProgressService"/> class.
    /// </summary>
    public FriendshipProgressService(IFriendshipRepository friendshipRepo, ILogger logger)
    {
        _friendshipRepo = friendshipRepo;
        _logger = logger;
    }

    /// <summary>
    /// Gets the friendship progress category for perfection tracking.
    /// </summary>
    public PerfectionCategory GetProgress()
    {
        var friendships = _friendshipRepo.GetAllFriendships();
        var maxCount = friendships.Count(f => f.IsMaxFriendship);
        var totalCount = friendships.Count;

        _logger.Log(LogLevel.Trace, $"Friendship progress: {maxCount}/{totalCount} NPCs at max");

        return new PerfectionCategory
        {
            CategoryName = "Friendship",
            CurrentCount = maxCount,
            TotalCount = totalCount,
            Weight = PerfectionWeight
        };
    }

    /// <summary>
    /// Gets NPCs that are not yet at maximum friendship.
    /// </summary>
    public IReadOnlyList<FriendshipInfo> GetIncompleteFriendships()
    {
        return _friendshipRepo.GetAllFriendships()
            .Where(f => !f.IsMaxFriendship)
            .OrderByDescending(f => f.HeartsRemaining)
            .ToList()
            .AsReadOnly();
    }
}
