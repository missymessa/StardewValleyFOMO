using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewUI.Framework;
using StardewUITest.Examples;
using StardewUITest;
using System.Linq;

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
            }
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
