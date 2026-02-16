namespace StardewFOMO.Core.Models;

/// <summary>
/// Represents a cooking or crafting recipe.
/// </summary>
public sealed class RecipeInfo
{
    /// <summary>Unique recipe identifier.</summary>
    public string RecipeId { get; init; } = string.Empty;

    /// <summary>Display name of the recipe.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Whether this is a cooking recipe (true) or crafting recipe (false).</summary>
    public bool IsCookingRecipe { get; init; }

    /// <summary>Ingredients/materials required.</summary>
    public IReadOnlyList<RecipeIngredient> Ingredients { get; init; } = Array.Empty<RecipeIngredient>();
}

/// <summary>
/// A single ingredient/material requirement for a recipe.
/// </summary>
public sealed class RecipeIngredient
{
    /// <summary>The item identifier for this ingredient.</summary>
    public string ItemId { get; init; } = string.Empty;

    /// <summary>The display name of the ingredient.</summary>
    public string ItemName { get; init; } = string.Empty;

    /// <summary>Quantity required.</summary>
    public int Quantity { get; init; } = 1;
}
