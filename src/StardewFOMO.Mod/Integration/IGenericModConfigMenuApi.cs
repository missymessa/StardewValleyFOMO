using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace StardewFOMO.Mod.Integration;

/// <summary>
/// API interface for Generic Mod Config Menu.
/// See: https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu
/// </summary>
public interface IGenericModConfigMenuApi
{
    /// <summary>Register a mod whose config can be edited through the UI.</summary>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="reset">Reset the mod's config to its default values.</param>
    /// <param name="save">Save the mod's current config to the config.json file.</param>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

    /// <summary>Add a section title to the form.</summary>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="text">The title text.</param>
    /// <param name="tooltip">An optional tooltip.</param>
    void AddSectionTitle(IManifest mod, Func<string> text, Func<string>? tooltip = null);

    /// <summary>Add a paragraph of text to the form.</summary>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="text">The paragraph text.</param>
    void AddParagraph(IManifest mod, Func<string> text);

    /// <summary>Add a boolean option to the form.</summary>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="getValue">Get the current value.</param>
    /// <param name="setValue">Set a new value.</param>
    /// <param name="name">The label text.</param>
    /// <param name="tooltip">An optional tooltip.</param>
    /// <param name="fieldId">An optional field ID for use with OnFieldChanged.</param>
    void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);

    /// <summary>Add an integer option to the form.</summary>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="getValue">Get the current value.</param>
    /// <param name="setValue">Set a new value.</param>
    /// <param name="name">The label text.</param>
    /// <param name="tooltip">An optional tooltip.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="interval">The interval between values.</param>
    /// <param name="formatValue">Format the value for display.</param>
    /// <param name="fieldId">An optional field ID for use with OnFieldChanged.</param>
    void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string>? tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null);

    /// <summary>Add a keybinding option to the form.</summary>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="getValue">Get the current value.</param>
    /// <param name="setValue">Set a new value.</param>
    /// <param name="name">The label text.</param>
    /// <param name="tooltip">An optional tooltip.</param>
    /// <param name="fieldId">An optional field ID for use with OnFieldChanged.</param>
    void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
}
