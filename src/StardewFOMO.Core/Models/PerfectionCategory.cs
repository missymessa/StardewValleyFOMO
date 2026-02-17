namespace StardewFOMO.Core.Models;

/// <summary>
/// Represents a perfection tracking category (Shipping, Fish, Cooking, etc.)
/// with completion percentage and list of items.
/// </summary>
public sealed class PerfectionCategory
{
    /// <summary>Category name (e.g., "Shipping", "Fish", "Cooking").</summary>
    public string CategoryName { get; init; } = string.Empty;

    /// <summary>Number of items/goals completed in this category.</summary>
    public int CurrentCount { get; init; }

    /// <summary>Total number of items/goals in this category.</summary>
    public int TotalCount { get; init; }

    /// <summary>Completion percentage for this category (0-100).</summary>
    public double PercentComplete => TotalCount > 0 ? (double)CurrentCount / TotalCount * 100.0 : 0.0;

    /// <summary>Whether this category is fully complete.</summary>
    public bool IsComplete => CurrentCount >= TotalCount;

    /// <summary>Weight contribution to overall perfection percentage.</summary>
    public double Weight { get; init; }

    /// <summary>Contribution to overall perfection (Weight * PercentComplete / 100).</summary>
    public double ContributionToTotal => Weight * PercentComplete / 100.0;

    /// <summary>Items within this category (for detailed display).</summary>
    public IReadOnlyList<PerfectionItem> Items { get; init; } = Array.Empty<PerfectionItem>();

    /// <summary>Subcategory grouping for large categories like Shipping.</summary>
    public string? Subcategory { get; init; }
}
