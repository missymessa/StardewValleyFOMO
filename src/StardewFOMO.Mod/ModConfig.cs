using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace StardewFOMO.Mod;

/// <summary>
/// Configuration POCO for the Daily Planner mod.
/// Read via SMAPI's <c>IModHelper.ReadConfig&lt;ModConfig&gt;()</c>.
/// </summary>
public sealed class ModConfig
{
    /// <summary>Hotkey to toggle the planner panel. Default: F7.</summary>
    public KeybindList ToggleKey { get; set; } = KeybindList.Parse("F7");

    /// <summary>Number of days ahead to show upcoming NPC birthdays. Default: 7.</summary>
    public int BirthdayLookaheadDays { get; set; } = 7;

    /// <summary>Number of days before season end to trigger last-chance alerts. Default: 3.</summary>
    public int SeasonAlertDays { get; set; } = 3;
}
