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

    /// <summary>Whether to start with the "Available Today" bundle filter enabled. Default: false.</summary>
    public bool AvailabilityFilterDefault { get; set; } = false;

    /// <summary>Whether to show HUD notifications when picking up bundle items. Default: true.</summary>
    public bool EnableBundleNotifications { get; set; } = true;

    /// <summary>Whether to show the Perfection tracking tab. Default: true.</summary>
    public bool ShowPerfectionTab { get; set; } = true;

    /// <summary>Whether to show "★ [HAVE]" indicators on owned perfection items. Default: true.</summary>
    public bool PerfectionShowOwnedIndicator { get; set; } = true;

    /// <summary>Whether to highlight items available today on the Perfection tab. Default: true.</summary>
    public bool PerfectionShowAvailableToday { get; set; } = true;
}
