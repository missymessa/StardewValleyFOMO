using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewUI.Framework;
using StardewUITest.Examples;
using StardewUITest;
using System.Linq;
using System.Collections.Generic;

namespace StardewValleyFOMO
{
    internal sealed class ModEntry : Mod
    {
        private readonly GalleryApi api = new();

        // Initialized in Entry
        //private ModConfig config = null!;

        // Initialized in GameLaunched
        private string viewAssetPrefix = null!;
        private IViewEngine viewEngine = null!;

        public override void Entry(IModHelper helper)
        {
            viewAssetPrefix = $"Mods/{ModManifest.UniqueID}/Views";

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

            if (!Context.IsPlayerFree)
            {
                return;
            }
            switch (e.Button)
            {
                case SButton.F8:
                    ShowGallery();
                    break;
                case SButton.F9: // Example: Get bundle data when F9 is pressed
                    GetCommunityCenterData();
                    break;
            }
        }

        private void GetCommunityCenterData()
        {
            this.Monitor.Log("--- Retrieving Community Center Bundle Data ---", LogLevel.Info);

            // 1. Game1.netWorldState.Value.Bundles (NetDictionary<int, bool[]>)
            //    - Key (int): Community Center Area ID.
            //    - Value (bool[]): Completion status of all item slots in that area.
            var areaBundleCompletionStatus = Game1.netWorldState.Value.Bundles;
            if (areaBundleCompletionStatus != null)
            {
                this.Monitor.Log("--- Area Bundle Completion Status (Game1.netWorldState.Value.Bundles) ---", LogLevel.Debug);
                if (!areaBundleCompletionStatus.Pairs.Any())
                {
                    this.Monitor.Log("  No data in Game1.netWorldState.Value.Bundles.", LogLevel.Debug);
                }
                foreach (var pair in areaBundleCompletionStatus.Pairs)
                {
                    // Example: Key = 3 (Pantry area ID), Value = [true, true, false, ...]
                    this.Monitor.Log($"  Area ID: {pair.Key} -> Item Slots Status: [{string.Join(", ", pair.Value)}]", LogLevel.Debug);
                }
            }
            else
            {
                this.Monitor.Log("  Game1.netWorldState.Value.Bundles is null.", LogLevel.Warn);
            }

            // 2. Game1.netWorldState.Value.BundleData (Dictionary<string, string>)
            //    - Key (string): Bundle identifier, e.g., "Pantry/0".
            //    - Value (string): Bundle definition string, e.g., "Spring Crops/20 Spring Seeds/1 495 1 0,1 496 1 0,.../0/0".
            var bundleDefinitions = Game1.netWorldState.Value.BundleData;
            if (bundleDefinitions != null)
            {
                this.Monitor.Log("--- Bundle Definitions (Game1.netWorldState.Value.BundleData) ---", LogLevel.Debug);
                if (!bundleDefinitions.Any())
                {
                    this.Monitor.Log("  No data in Game1.netWorldState.Value.BundleData.", LogLevel.Debug);
                }
                foreach (KeyValuePair<string, string> pair in bundleDefinitions)
                {
                    this.Monitor.Log($"  Bundle Key: \"{pair.Key}\" -> Definition: \"{pair.Value}\"", LogLevel.Debug);
                }
            }
            else
            {
                this.Monitor.Log("  Game1.netWorldState.Value.BundleData is null.", LogLevel.Warn);
            }

            // The previous complex logic for player-specific contributions has been removed for clarity
            // as per the request to display all data in `bundles` and `bundleData` directly.
            // To get player-specific contributions, you would iterate through CommunityCenter.getBundle(index).playersWhoDonatedList
            // after loading Data/Bundles.xnb to map bundle indices correctly, as shown in prior versions.
            this.Monitor.Log("--- Finished Displaying Bundle Data ---", LogLevel.Info);
        }

        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI")!;
            viewEngine.RegisterViews(viewAssetPrefix, "assets/views");
            viewEngine.EnableHotReloadingWithSourceSync();
            viewEngine.PreloadAssets();
        }

        private void OpenExample(string assetName, object? context)
        {
            viewEngine.CreateMenuControllerFromAsset(assetName, context).Launch();
        }

        private void ShowGallery()
        {
            var context = new GalleryViewModel(
                [
                    new(
                        "Item Grid",//I18n.Gallery_Example_ItemGrid_Title(),
                        "'Intro to Stardew UI': A simple grid of items, showing their names and tooltips, like you might see in an inventory menu.",//I18n.Gallery_Example_ItemGrid_Description(),
                        "(BC)232",
                        ShowItemGrid
                    ),
                    .. api.Registrations.Select(register => register()),
                ]
            );
            Game1.activeClickableMenu = viewEngine.CreateMenuFromAsset($"{viewAssetPrefix}/Gallery", context);
        }

        private void ShowItemGrid()
        {
            var context = EdiblesViewModel.LoadFromGameData();
            OpenExample($"{viewAssetPrefix}/ScrollingItemGrid", context);
        }
    }
}