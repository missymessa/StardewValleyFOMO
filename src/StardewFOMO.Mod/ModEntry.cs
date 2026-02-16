using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewFOMO.Core.Services;
using StardewFOMO.Mod.Adapters;
using StardewFOMO.Mod.Integration;
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
    private BundleProgressService _bundleProgressService = null!;
    private BundleNotificationService? _bundleNotificationService;
    private BundleAdapter _bundleAdapter = null!;
    private StorageScannerAdapter? _storageScanner;
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
        helper.Events.Player.InventoryChanged += OnInventoryChanged;
        helper.Events.Player.Warped += OnPlayerWarped;

        Monitor.Log("StardewFOMO Daily Planner loaded.", LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        Monitor.Log("Game launched — planner ready.", LogLevel.Debug);
        RegisterWithGMCM();
    }

    private void RegisterWithGMCM()
    {
        // Get the GMCM API (returns null if GMCM isn't installed)
        var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (gmcm == null)
        {
            Monitor.Log("Generic Mod Config Menu not found — using config.json only.", LogLevel.Debug);
            return;
        }

        // Register the mod
        gmcm.Register(
            mod: ModManifest,
            reset: () => _config = new ModConfig(),
            save: () => Helper.WriteConfig(_config)
        );

        // Add section title
        gmcm.AddSectionTitle(
            mod: ModManifest,
            text: () => "Daily Planner Settings"
        );

        // Toggle key option
        gmcm.AddKeybindList(
            mod: ModManifest,
            getValue: () => _config.ToggleKey,
            setValue: value => _config.ToggleKey = value,
            name: () => "Toggle Key",
            tooltip: () => "The key to open/close the Daily Planner overlay. Default: F7"
        );

        // Birthday lookahead days
        gmcm.AddNumberOption(
            mod: ModManifest,
            getValue: () => _config.BirthdayLookaheadDays,
            setValue: value => _config.BirthdayLookaheadDays = value,
            name: () => "Birthday Lookahead Days",
            tooltip: () => "How many days ahead to show upcoming NPC birthdays.",
            min: 1,
            max: 28
        );

        // Season alert days
        gmcm.AddNumberOption(
            mod: ModManifest,
            getValue: () => _config.SeasonAlertDays,
            setValue: value => _config.SeasonAlertDays = value,
            name: () => "Season Alert Days",
            tooltip: () => "Days before season end to show last-chance item alerts.",
            min: 1,
            max: 14
        );

        // Bundle Tracker section
        gmcm.AddSectionTitle(
            mod: ModManifest,
            text: () => "Bundle Tracker Settings"
        );

        // Availability filter default
        gmcm.AddBoolOption(
            mod: ModManifest,
            getValue: () => _config.AvailabilityFilterDefault,
            setValue: value => _config.AvailabilityFilterDefault = value,
            name: () => "Default Availability Filter",
            tooltip: () => "Whether to start with 'Available Today' filter enabled in Bundles tab."
        );

        // Bundle notifications
        gmcm.AddBoolOption(
            mod: ModManifest,
            getValue: () => _config.EnableBundleNotifications,
            setValue: value => _config.EnableBundleNotifications = value,
            name: () => "Bundle Item Notifications",
            tooltip: () => "Show HUD notification when you pick up an item needed for a bundle."
        );

        Monitor.Log("Registered with Generic Mod Config Menu.", LogLevel.Info);
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

        // Invalidate storage cache at day start
        _storageScanner?.InvalidateCache();

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

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (!_isInitialized || !e.IsLocalPlayer)
            return;

        // Invalidate storage cache when inventory changes
        _storageScanner?.InvalidateCache();

        // Check for bundle item notifications if enabled
        if (_config.EnableBundleNotifications && _bundleNotificationService != null)
        {
            foreach (var item in e.Added)
            {
                var itemId = item.ItemId;
                var itemName = item.DisplayName ?? item.Name ?? itemId;
                var notification = _bundleNotificationService.CheckForBundleItem(itemId, itemName);
                if (notification != null)
                {
                    Game1.addHUDMessage(new HUDMessage(notification, HUDMessage.newQuest_type));
                }
            }
        }
    }

    private void OnPlayerWarped(object? sender, WarpedEventArgs e)
    {
        if (!_isInitialized || !e.IsLocalPlayer)
            return;

        // Invalidate storage cache when leaving the Community Center
        if (e.OldLocation?.Name == "CommunityCenter")
        {
            _storageScanner?.InvalidateCache();
            Monitor.Log("Player left Community Center — storage cache invalidated.", LogLevel.Debug);
        }
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
        _bundleAdapter = new BundleAdapter();
        _storageScanner = new StorageScannerAdapter();
        var collectionRepo = new CollectionAdapter();
        var inventoryProvider = new InventoryAdapter();
        var npcRepo = new NpcAdapter();
        var fishRepo = new FishDataAdapter();
        var forageRepo = new ForageDataAdapter();
        var recipeRepo = new RecipeDataAdapter();

        var fishAvailability = new FishAvailabilityService(fishRepo, gameState, collectionRepo, inventoryProvider, logger);
        var forageAvailability = new ForageAvailabilityService(forageRepo, gameState, collectionRepo, inventoryProvider, logger);
        var bundleTracking = new BundleTrackingService(_bundleAdapter, inventoryProvider, logger);
        var collectionTracking = new CollectionTrackingService(collectionRepo, inventoryProvider, recipeRepo, logger);
        var birthdayService = new BirthdayService(npcRepo, gameState, logger, _config.BirthdayLookaheadDays);
        var tomorrowPreview = new TomorrowPreviewService(gameState, fishRepo, forageRepo, collectionRepo, logger);
        var seasonAlert = new SeasonAlertService(gameState, fishRepo, forageRepo, collectionRepo, logger, _config.SeasonAlertDays);

        // Create bundle progress service for the Bundles tab
        _bundleProgressService = new BundleProgressService(_bundleAdapter, logger);

        // Create bundle notification service for pickup alerts
        _bundleNotificationService = new BundleNotificationService(_bundleAdapter, logger);

        // Create item availability adapter for filtering
        var itemAvailability = new ItemAvailabilityAdapter(gameState, fishRepo, forageRepo);
        var bundleAvailability = new BundleAvailabilityService(_bundleAdapter, itemAvailability, logger);

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

        // Wire bundle services to overlay
        _overlay.SetBundleServices(_bundleProgressService, _bundleAdapter, bundleAvailability);
        _overlay.SetAvailabilityFilterDefault(_config.AvailabilityFilterDefault);

        _isInitialized = true;
    }
}
