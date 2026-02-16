using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewFOMO.Core.Services;
using StardewFOMO.Mod.Adapters;
using StardewFOMO.Mod.UI;

namespace StardewFOMO.Mod;

/// <summary>
/// SMAPI mod entry point. Thin adapter that wires all game data adapters to core services
/// and manages the planner overlay lifecycle.
/// </summary>
public sealed class ModEntry : StardewModdingAPI.Mod
{
    private ModConfig _config = null!;
    private PlannerOverlay _overlay = null!;
    private DailySummaryService _summaryService = null!;
    private bool _isInitialized;
    
    // Scroll suppression state
    private bool _suppressToolbarScroll;
    private int _savedToolbarIndex;

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        _config = helper.ReadConfig<ModConfig>();
        _overlay = new PlannerOverlay();

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.Input.MouseWheelScrolled += OnMouseWheelScrolled;
        helper.Events.Display.RenderedHud += OnRenderedHud;

        Monitor.Log("StardewFOMO Daily Planner loaded.", LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        Monitor.Log("Game launched — planner ready.", LogLevel.Debug);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        try
        {
            InitializeServices();
            _overlay.ResetSession();
            Monitor.Log("Save loaded — services initialized.", LogLevel.Info);
        }
        catch (Exception ex)
        {
            Monitor.Log($"Error initializing services: {ex.Message}", LogLevel.Error);
            _isInitialized = false;
        }
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!_isInitialized)
            return;

        try
        {
            var summary = _overlay.IsVisible
                ? _summaryService.GetDailySummary()
                : null;
            _overlay.SetSummary(summary);

            Monitor.Log($"Day started: {Game1.currentSeason} {Game1.dayOfMonth}, Year {Game1.year}", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            Monitor.Log($"Error refreshing daily summary: {ex.Message}", LogLevel.Error);
        }
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        _overlay.IsVisible = false;
        _overlay.ResetSession();
        _isInitialized = false;
        Monitor.Log("Returned to title — session reset.", LogLevel.Debug);
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        // Only respond when the player can interact (not during menus/cutscenes)
        if (!Context.IsWorldReady || !Context.CanPlayerMove)
            return;

        if (_config.ToggleKey.JustPressed())
        {
            ToggleOverlay();
        }
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!_overlay.IsVisible || !Context.IsWorldReady)
            return;

        // Handle left click on overlay
        if (e.Button == SButton.MouseLeft)
        {
            var mousePos = Game1.getMousePosition();
            _overlay.receiveLeftClick(mousePos.X, mousePos.Y);
            Helper.Input.Suppress(e.Button);
        }
        // Handle escape to close
        else if (e.Button == SButton.Escape)
        {
            _overlay.IsVisible = false;
            Game1.playSound("bigDeSelect");
            Helper.Input.Suppress(e.Button);
        }
    }

    private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
    {
        if (!_overlay.IsVisible || !Context.IsWorldReady)
            return;

        // Check if mouse is over the panel
        var mousePos = Game1.getMousePosition();
        if (_overlay.isWithinBounds(mousePos.X, mousePos.Y))
        {
            // Save current toolbar index and flag for restoration after game processes scroll
            _savedToolbarIndex = Game1.player.CurrentToolIndex;
            _suppressToolbarScroll = true;
            
            _overlay.receiveScrollWheelAction(e.Delta);
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        // Restore toolbar index after game has processed the scroll
        if (_suppressToolbarScroll && Context.IsWorldReady && Game1.player != null)
        {
            Game1.player.CurrentToolIndex = _savedToolbarIndex;
            _suppressToolbarScroll = false;
        }
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (!_overlay.IsVisible || !Context.IsWorldReady)
            return;

        _overlay.draw(e.SpriteBatch);
    }

    private void ToggleOverlay()
    {
        _overlay.IsVisible = !_overlay.IsVisible;

        if (_overlay.IsVisible && _isInitialized)
        {
            try
            {
                var summary = _summaryService.GetShowAllSummary();
                _overlay.SetSummary(summary);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error building daily summary: {ex.Message}", LogLevel.Error);
                _overlay.SetSummary(null);
            }
        }

        var state = _overlay.IsVisible ? "opened" : "closed";
        Monitor.Log($"Planner overlay {state}.", LogLevel.Debug);
    }

    private void InitializeServices()
    {
        var logger = new SmapiLoggerAdapter(Monitor);
        var gameState = new GameStateAdapter();
        var bundleRepo = new BundleAdapter();
        var collectionRepo = new CollectionAdapter();
        var inventoryProvider = new InventoryAdapter();
        var npcRepo = new NpcAdapter();
        var fishRepo = new FishDataAdapter();
        var forageRepo = new ForageDataAdapter();
        var recipeRepo = new RecipeDataAdapter();

        var fishAvailability = new FishAvailabilityService(fishRepo, gameState, collectionRepo, inventoryProvider, logger);
        var forageAvailability = new ForageAvailabilityService(forageRepo, gameState, collectionRepo, inventoryProvider, logger);
        var bundleTracking = new BundleTrackingService(bundleRepo, inventoryProvider, logger);
        var collectionTracking = new CollectionTrackingService(collectionRepo, inventoryProvider, recipeRepo, logger);
        var birthdayService = new BirthdayService(npcRepo, gameState, logger, _config.BirthdayLookaheadDays);
        var tomorrowPreview = new TomorrowPreviewService(gameState, fishRepo, forageRepo, collectionRepo, logger);
        var seasonAlert = new SeasonAlertService(gameState, fishRepo, forageRepo, collectionRepo, logger, _config.SeasonAlertDays);

        _summaryService = new DailySummaryService(
            fishAvailability,
            forageAvailability,
            bundleTracking,
            collectionTracking,
            birthdayService,
            tomorrowPreview,
            seasonAlert,
            gameState,
            logger);

        _isInitialized = true;
    }
}
