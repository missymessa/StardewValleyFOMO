using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewFOMO.Core.Models;
using StardewFOMO.Core.Services;
using StardewFOMO.Core.Interfaces;

namespace StardewFOMO.Mod.UI;

/// <summary>Available tabs in the planner overlay.</summary>
public enum PlannerTab
{
    Today,
    Events,
    Bundles,
    Birthdays,
    Tomorrow,
    Collections,
    Perfection
}

/// <summary>
/// In-game overlay panel that renders the daily planner using MonoGame/SpriteBatch.
/// Features a tabbed interface for different views.
/// Session-scoped view state that resets on game restart.
/// </summary>
public sealed class PlannerOverlay : IClickableMenu
{
    private const int PanelMargin = 32;
    private const int SectionSpacing = 16;
    private const int LineHeight = 36;
    private const int HeaderHeight = 48;
    private const int TabBarHeight = 44;
    private const int TabWidth = 80;
    private const int TabSpacing = 2;
    private const int ScrollStep = 40;

    private static readonly PlannerTab[] AllTabs = 
    {
        PlannerTab.Today,
        PlannerTab.Events,
        PlannerTab.Bundles,
        PlannerTab.Birthdays,
        PlannerTab.Tomorrow,
        PlannerTab.Collections,
        PlannerTab.Perfection
    };

    private DailySummary? _summary;
    private BundleProgressService? _bundleProgressService;
    private IBundleRepository? _bundleRepository;
    private BundleAvailabilityService? _bundleAvailabilityService;
    private PerfectionCalculatorService? _perfectionService;
    private PlannerTab _activeTab = PlannerTab.Today;
    private int _scrollOffset;
    private int _contentHeight;
    private bool _showAvailableTodayOnly;
    private Rectangle _availabilityFilterButtonBounds;

    private Rectangle _panelBounds;
    private Rectangle _closeButtonBounds;
    private readonly Rectangle[] _tabBounds = new Rectangle[7];

    // Birthday hover tracking
    private readonly List<(Rectangle Bounds, NpcBirthday Birthday)> _birthdayHitboxes = new();
    private NpcBirthday? _hoveredBirthday;

    /// <summary>Whether the overlay is currently visible.</summary>
    public bool IsVisible { get; set; }

    /// <summary>Gets or sets the active tab.</summary>
    public PlannerTab ActiveTab
    {
        get => _activeTab;
        set
        {
            if (_activeTab != value)
            {
                _activeTab = value;
                _scrollOffset = 0;
            }
        }
    }

    /// <summary>Initializes the overlay with screen-proportional dimensions.</summary>
    public PlannerOverlay()
        : base(0, 0, 0, 0)
    {
        RecalculateBounds();
    }

    /// <summary>Update the daily summary data displayed in the overlay.</summary>
    public void SetSummary(DailySummary? summary)
    {
        _summary = summary;
        _scrollOffset = 0;
        _birthdayHitboxes.Clear();
        _hoveredBirthday = null;
    }

    /// <summary>Set the bundle progress service for the Bundles tab.</summary>
    public void SetBundleServices(
        BundleProgressService? progressService, 
        IBundleRepository? bundleRepository,
        BundleAvailabilityService? availabilityService = null)
    {
        _bundleProgressService = progressService;
        _bundleRepository = bundleRepository;
        _bundleAvailabilityService = availabilityService;
    }

    /// <summary>Set the perfection calculator service for the Perfection tab.</summary>
    public void SetPerfectionService(PerfectionCalculatorService? perfectionService)
    {
        _perfectionService = perfectionService;
    }

    /// <summary>Set the default availability filter state from config.</summary>
    public void SetAvailabilityFilterDefault(bool enabled)
    {
        _showAvailableTodayOnly = enabled;
    }

    /// <summary>Reset session state (called on game restart/load).</summary>
    public void ResetSession()
    {
        _activeTab = PlannerTab.Today;
        _scrollOffset = 0;
        _summary = null;
        _birthdayHitboxes.Clear();
        _hoveredBirthday = null;
        _showAvailableTodayOnly = false;
    }

    private void RecalculateBounds()
    {
        var viewport = Game1.graphics.GraphicsDevice.Viewport;
        int panelWidth = Math.Min(560, viewport.Width - PanelMargin * 2);
        int panelHeight = Math.Min(viewport.Height - PanelMargin * 2, 800);
        int panelX = viewport.Width - panelWidth - PanelMargin;
        int panelY = PanelMargin;

        _panelBounds = new Rectangle(panelX, panelY, panelWidth, panelHeight);

        // Calculate tab bounds - evenly distribute across panel width
        int tabAreaWidth = panelWidth - 32; // 16px padding on each side
        int actualTabWidth = (tabAreaWidth - (AllTabs.Length - 1) * TabSpacing) / AllTabs.Length;
        int tabStartX = panelX + 16;
        int tabY = panelY + HeaderHeight;
        
        for (int i = 0; i < AllTabs.Length; i++)
        {
            _tabBounds[i] = new Rectangle(
                tabStartX + i * (actualTabWidth + TabSpacing),
                tabY,
                actualTabWidth,
                TabBarHeight - 8);
        }

        _closeButtonBounds = new Rectangle(
            panelX + panelWidth - 36,
            panelY + 8,
            28,
            28);
    }

    /// <inheritdoc/>
    public override bool isWithinBounds(int x, int y)
    {
        return _panelBounds.Contains(x, y);
    }
    /// <inheritdoc/>
    public override void draw(SpriteBatch b)
    {
        if (!IsVisible || _summary == null)
            return;

        RecalculateBounds();

        // Update hover state using hitboxes from previous frame
        var mousePos = Game1.getMousePosition();
        _hoveredBirthday = null;
        foreach (var (bounds, birthday) in _birthdayHitboxes)
        {
            if (bounds.Contains(mousePos))
            {
                _hoveredBirthday = birthday;
                break;
            }
        }

        // Clear hitboxes to rebuild during this frame
        _birthdayHitboxes.Clear();

        // Draw panel background - dark charcoal for maximum readability
        IClickableMenu.drawTextureBox(b,
            Game1.menuTexture,
            new Rectangle(0, 256, 60, 60),
            _panelBounds.X, _panelBounds.Y,
            _panelBounds.Width, _panelBounds.Height,
            new Color(30, 30, 35), 1f, false);

        // Draw slightly lighter inner panel for content area
        b.Draw(Game1.staminaRect, 
            new Rectangle(_panelBounds.X + 12, _panelBounds.Y + 12, 
                _panelBounds.Width - 24, _panelBounds.Height - 24),
            new Color(20, 22, 28, 245));

        // Draw header
        DrawHeader(b);

        // Draw tab bar
        DrawTabBar(b);

        // Set up clipping for scroll
        var originalScissor = b.GraphicsDevice.ScissorRectangle;
        var contentArea = new Rectangle(
            _panelBounds.X + 16,
            _panelBounds.Y + HeaderHeight + TabBarHeight,
            _panelBounds.Width - 32,
            _panelBounds.Height - HeaderHeight - TabBarHeight - 16);

        b.End();
        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
            null, new RasterizerState { ScissorTestEnable = true });
        b.GraphicsDevice.ScissorRectangle = contentArea;

        int y = contentArea.Y - _scrollOffset;
        int x = contentArea.X;
        int maxWidth = contentArea.Width;

        // Draw content based on active tab
        switch (_activeTab)
        {
            case PlannerTab.Today:
                DrawTodayTab(b, x, ref y, maxWidth);
                break;
            case PlannerTab.Events:
                DrawEventsTab(b, x, ref y, maxWidth);
                break;
            case PlannerTab.Bundles:
                DrawBundlesTab(b, x, ref y, maxWidth);
                break;
            case PlannerTab.Birthdays:
                DrawBirthdaysTab(b, x, ref y, maxWidth);
                break;
            case PlannerTab.Tomorrow:
                DrawTomorrowTab(b, x, ref y, maxWidth);
                break;
            case PlannerTab.Collections:
                DrawCollectionsTab(b, x, ref y, maxWidth);
                break;
            case PlannerTab.Perfection:
                DrawPerfectionTab(b, x, ref y, maxWidth);
                break;
        }

        _contentHeight = y + _scrollOffset - contentArea.Y;

        // Restore scissor rect
        b.End();
        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        b.GraphicsDevice.ScissorRectangle = originalScissor;

        // Draw close button
        DrawCloseButton(b);

        // Draw scroll indicators
        DrawScrollIndicators(b, contentArea);

        // Draw birthday tooltip if hovering
        DrawBirthdayTooltip(b);

        drawMouse(b);
    }

    private void DrawHeader(SpriteBatch b)
    {
        var dateStr = _summary != null
            ? $"{_summary.Date.Season} {_summary.Date.Day}, Year {_summary.Date.Year}"
            : "Loading...";
        var weatherStr = _summary != null ? $" — {_summary.CurrentWeather}" : "";

        Utility.drawTextWithShadow(b,
            $"Daily Planner: {dateStr}{weatherStr}",
            Game1.smallFont,
            new Vector2(_panelBounds.X + 16, _panelBounds.Y + 12),
            Color.White);
    }

    private void DrawTabBar(SpriteBatch b)
    {
        for (int i = 0; i < AllTabs.Length; i++)
        {
            var tab = AllTabs[i];
            var bounds = _tabBounds[i];
            var isActive = tab == _activeTab;

            // Tab background - darker colors for better contrast
            var bgColor = isActive ? new Color(60, 70, 90) : new Color(35, 40, 50);
            var borderColor = isActive ? Color.Gold : new Color(60, 65, 75);

            IClickableMenu.drawTextureBox(b,
                Game1.mouseCursors,
                new Rectangle(432, 439, 9, 9),
                bounds.X, bounds.Y,
                bounds.Width, bounds.Height,
                bgColor, 3f, false);

            // Tab label
            var label = GetTabLabel(tab);
            var labelSize = Game1.smallFont.MeasureString(label);
            var textColor = isActive ? Color.White : Color.LightGray;

            Utility.drawTextWithShadow(b,
                label,
                Game1.smallFont,
                new Vector2(
                    bounds.X + (bounds.Width - labelSize.X) / 2,
                    bounds.Y + (bounds.Height - labelSize.Y) / 2),
                textColor);

            // Active tab indicator
            if (isActive)
            {
                b.Draw(Game1.staminaRect,
                    new Rectangle(bounds.X + 4, bounds.Bottom - 3, bounds.Width - 8, 2),
                    Color.Gold);
            }
        }
    }

    private static string GetTabLabel(PlannerTab tab) => tab switch
    {
        PlannerTab.Today => "Today",
        PlannerTab.Events => "Events",
        PlannerTab.Bundles => "Bundles",
        PlannerTab.Birthdays => "Bdays",
        PlannerTab.Tomorrow => "Next",
        PlannerTab.Collections => "All",
        _ => "?"
    };

    private void DrawTodayTab(SpriteBatch b, int x, ref int y, int maxWidth)
    {
        // Brief luck indicator
        var luckIcon = _summary!.DailyLuck >= 0.02 ? "🍀" : _summary.DailyLuck <= -0.02 ? "💀" : "⚖";
        var luckColor = _summary.DailyLuck switch
        {
            >= 0.07 => Color.Gold,
            >= 0.02 => Color.LightGreen,
            >= -0.02 => Color.White,
            >= -0.07 => Color.Orange,
            _ => Color.OrangeRed
        };
        DrawText(b, $"{luckIcon} {_summary.LuckDescription}", x, ref y, luckColor);
        y += 4;

        // Festival notice
        if (_summary.IsFestivalDay && _summary.FestivalName != null)
        {
            DrawSectionTitle(b, $"🎉 {_summary.FestivalName}", x, ref y, Color.Gold);
            y += SectionSpacing;
        }

        // Brief events summary (if any special events today)
        if (_summary.TodayEvents.Count > 0)
        {
            foreach (var evt in _summary.TodayEvents.Take(2))
            {
                DrawText(b, $"  {evt}", x, ref y, Color.Cyan);
            }
            if (_summary.TodayEvents.Count > 2)
            {
                DrawText(b, $"  +{_summary.TodayEvents.Count - 2} more (see Events tab)", x, ref y, Color.LightGray);
            }
            y += 8;
        }

        // Last-chance alerts (highest priority)
        if (_summary.LastChanceAlerts.Count > 0)
        {
            DrawSectionTitle(b, "⚠ Last Chance!", x, ref y, Color.OrangeRed);
            foreach (var item in _summary.LastChanceAlerts)
            {
                DrawCollectibleItem(b, item, x + 16, ref y, maxWidth - 16, Color.OrangeRed);
            }
            y += SectionSpacing;
        }

        // Today's birthdays (brief)
        if (_summary.TodayBirthdays.Count > 0)
        {
            DrawSectionTitle(b, "🎂 Today's Birthdays", x, ref y, Color.HotPink);
            foreach (var bday in _summary.TodayBirthdays)
            {
                var entryY = y;
                var isHovered = _hoveredBirthday == bday;
                var textColor = isHovered ? Color.Yellow : Color.White;
                
                DrawText(b, $"  {bday.NpcName} (hover for gifts)", x, ref y, textColor);
                
                var hitbox = new Rectangle(x, entryY, maxWidth, y - entryY);
                _birthdayHitboxes.Add((hitbox, bday));
            }
            y += SectionSpacing;
        }

        // Bundle-needed items (brief summary)
        if (_summary.BundleNeededItems.Count > 0)
        {
            DrawSectionTitle(b, $"📦 Bundle Items Today ({_summary.BundleNeededItems.Count})", x, ref y, Color.Yellow);
            foreach (var item in _summary.BundleNeededItems.Take(5))
            {
                DrawCollectibleItem(b, item, x + 16, ref y, maxWidth - 16, Color.White);
            }
            if (_summary.BundleNeededItems.Count > 5)
            {
                DrawText(b, $"    ... and {_summary.BundleNeededItems.Count - 5} more (see Bundles tab)", x, ref y, Color.Khaki);
            }
            y += SectionSpacing;
        }

        // Available fish
        DrawSectionTitle(b, $"🐟 Fish ({_summary.AvailableFish.Count})", x, ref y, Color.Cyan);
        foreach (var fish in _summary.AvailableFish.Take(8))
        {
            DrawCollectibleItem(b, fish, x + 16, ref y, maxWidth - 16, Color.White);
        }
        if (_summary.AvailableFish.Count > 8)
        {
            DrawText(b, $"    ... and {_summary.AvailableFish.Count - 8} more", x, ref y, Color.LightGray);
        }
        y += SectionSpacing;

        // Available forageables
        DrawSectionTitle(b, $"🌿 Forageables ({_summary.AvailableForageables.Count})", x, ref y, Color.LimeGreen);
        foreach (var forage in _summary.AvailableForageables.Take(8))
        {
            DrawCollectibleItem(b, forage, x + 16, ref y, maxWidth - 16, Color.White);
        }
        if (_summary.AvailableForageables.Count > 8)
        {
            DrawText(b, $"    ... and {_summary.AvailableForageables.Count - 8} more", x, ref y, Color.LightGray);
        }
        y += SectionSpacing;
    }

    private void DrawEventsTab(SpriteBatch b, int x, ref int y, int maxWidth)
    {
        // Daily Luck section
        DrawSectionTitle(b, "🍀 Today's Fortune", x, ref y, Color.LimeGreen);
        
        var luckColor = _summary!.DailyLuck switch
        {
            >= 0.07 => Color.Gold,
            >= 0.02 => Color.LightGreen,
            >= -0.02 => Color.White,
            >= -0.07 => Color.Orange,
            _ => Color.OrangeRed
        };
        DrawText(b, $"  {_summary.LuckDescription}", x, ref y, luckColor);
        
        var luckPercent = (_summary.DailyLuck * 100).ToString("+0.0;-0.0;0");
        DrawText(b, $"  Luck modifier: {luckPercent}%", x, ref y, Color.LightGray);
        y += SectionSpacing;

        // Festival today
        if (_summary.IsFestivalDay && _summary.FestivalName != null)
        {
            DrawSectionTitle(b, "🎉 Festival Today!", x, ref y, Color.Gold);
            DrawText(b, $"  {_summary.FestivalName}", x, ref y, Color.White);
            DrawText(b, "  Most shops closed. Enjoy the festivities!", x, ref y, Color.Khaki);
            y += SectionSpacing;
        }

        // Today's special events
        DrawSectionTitle(b, "📅 Today's Events", x, ref y, Color.Cyan);
        if (_summary.TodayEvents.Count > 0)
        {
            foreach (var evt in _summary.TodayEvents)
            {
                DrawText(b, $"  {evt}", x, ref y, Color.White);
            }
        }
        else
        {
            DrawText(b, "  No special events today.", x, ref y, Color.LightGray);
        }
        y += SectionSpacing;

        // Tomorrow's events preview
        if (_summary.TomorrowPreview != null && _summary.TomorrowPreview.Events.Count > 0)
        {
            DrawSectionTitle(b, "📆 Coming Tomorrow", x, ref y, Color.MediumPurple);
            foreach (var evt in _summary.TomorrowPreview.Events)
            {
                DrawText(b, $"  {evt}", x, ref y, Color.LightGray);
            }
            y += SectionSpacing;
        }

        // Weekly schedule reminder
        DrawSectionTitle(b, "📋 Weekly Schedule", x, ref y, Color.Violet);
        DrawText(b, "  Friday & Sunday: Traveling Merchant", x, ref y, Color.LightGray);
        DrawText(b, "  Sunday: Queen of Sauce (new recipe)", x, ref y, Color.LightGray);
        DrawText(b, "  Wednesday: Queen of Sauce (rerun)", x, ref y, Color.LightGray);
        DrawText(b, "  Winter 15-17: Night Market", x, ref y, Color.LightGray);
        y += SectionSpacing;
    }

    private void DrawBundlesTab(SpriteBatch b, int x, ref int y, int maxWidth)
    {
        // Check if bundle progress service is available
        if (_bundleProgressService == null || _bundleRepository == null)
        {
            DrawSectionTitle(b, "📦 Bundle Tracker", x, ref y, Color.Yellow);
            DrawText(b, "  Bundle tracking loading...", x, ref y, Color.LightGray);
            y += SectionSpacing;
            return;
        }

        // Check if Community Center is active
        if (!_bundleRepository.IsCommunityCenterActive())
        {
            DrawSectionTitle(b, "📦 Community Center", x, ref y, Color.Gray);
            y += 8;
            DrawText(b, "  You chose the Joja Mart route.", x, ref y, Color.LightGray);
            DrawText(b, "  Community Center bundles are", x, ref y, Color.LightGray);
            DrawText(b, "  not available.", x, ref y, Color.LightGray);
            y += SectionSpacing;
            return;
        }

        // Check if CC is complete
        if (_bundleProgressService.IsCommunityComplete())
        {
            DrawSectionTitle(b, "🎉 Community Center Complete!", x, ref y, Color.Gold);
            y += 8;
            DrawText(b, "  Congratulations!", x, ref y, Color.LightGreen);
            DrawText(b, "  You've restored the Community Center!", x, ref y, Color.LightGreen);
            DrawText(b, "  The Junimos thank you!", x, ref y, Color.LightGreen);
            y += SectionSpacing;
            return;
        }

        // Overall progress header
        var overallProgress = _bundleProgressService.GetOverallProgress();
        DrawSectionTitle(b, "📦 Community Center Progress", x, ref y, Color.Yellow);
        y += 4;
        
        // Draw overall progress bar
        DrawProgressBar(b, x + 16, y, maxWidth - 32, 16, overallProgress.PercentComplete, Color.Gold);
        y += 24;
        DrawText(b, $"  {overallProgress.CompletedBundles}/{overallProgress.TotalBundles} bundles complete ({overallProgress.PercentComplete}%)", x, ref y, Color.White);
        
        // Availability filter toggle button
        var filterText = _showAvailableTodayOnly ? "[✓] Available Today" : "[ ] Available Today";
        var filterColor = _showAvailableTodayOnly ? Color.LightGreen : Color.Gray;
        _availabilityFilterButtonBounds = new Rectangle(x + 16, y + 4, 200, 28);
        DrawText(b, $"  {filterText}", x, ref y, filterColor);
        y += SectionSpacing;

        // Draw each room
        var roomProgressList = _bundleProgressService.GetRoomProgressList();
        foreach (var roomProgress in roomProgressList)
        {
            // Room header with progress
            var roomColor = roomProgress.IsComplete ? Color.LightGreen : Color.LightBlue;
            var roomIcon = roomProgress.IsComplete ? "✓" : "○";
            DrawText(b, $"  {roomIcon} {roomProgress.RoomName}", x, ref y, roomColor);
            
            // Room progress bar (smaller)
            DrawProgressBar(b, x + 32, y, maxWidth - 64, 10, roomProgress.PercentComplete, 
                roomProgress.IsComplete ? Color.LightGreen : Color.CornflowerBlue);
            y += 16;

            // Bundle details within room
            var bundleCounts = _bundleProgressService.GetBundleCountsForRoom(roomProgress.RoomName);
            foreach (var bundle in bundleCounts)
            {
                var bundleIcon = bundle.IsComplete ? "★" : "·";
                var bundleColor = bundle.IsComplete ? Color.DarkGreen : Color.Gray;
                var countText = bundle.IsComplete 
                    ? "Complete" 
                    : $"{bundle.CompletedItems}/{bundle.TotalItems}";
                
                DrawText(b, $"      {bundleIcon} {bundle.BundleName}: {countText}", x, ref y, bundleColor);
            }
            y += 8;
        }

        y += SectionSpacing;

        // Legacy: Show bundle items available today (if any)
        if (_summary?.BundleNeededItems.Count > 0)
        {
            DrawSectionTitle(b, "🌟 Available Today for Bundles", x, ref y, Color.Orange);
            y += 4;
            
            // Group by bundle
            var byBundle = new Dictionary<string, List<CollectibleItem>>();
            foreach (var item in _summary.BundleNeededItems)
            {
                foreach (var bundleName in item.BundleNames)
                {
                    if (!byBundle.ContainsKey(bundleName))
                        byBundle[bundleName] = new List<CollectibleItem>();
                    byBundle[bundleName].Add(item);
                }
            }

            foreach (var (bundleName, items) in byBundle.OrderBy(kvp => kvp.Key))
            {
                DrawText(b, $"  {bundleName}:", x, ref y, Color.Gold);
                foreach (var item in items.Take(3)) // Limit to 3 per bundle to save space
                {
                    DrawCollectibleItem(b, item, x + 32, ref y, maxWidth - 32, Color.White);
                }
                if (items.Count > 3)
                {
                    DrawText(b, $"    ...and {items.Count - 3} more", x, ref y, Color.LightGray);
                }
                y += 4;
            }
            y += SectionSpacing;
        }
    }

    /// <summary>Draw a progress bar.</summary>
    private void DrawProgressBar(SpriteBatch b, int x, int y, int width, int height, int percent, Color fillColor)
    {
        // Background
        b.Draw(Game1.staminaRect, new Rectangle(x, y, width, height), Color.DarkGray);
        
        // Fill
        var fillWidth = (int)(width * (percent / 100.0));
        if (fillWidth > 0)
        {
            b.Draw(Game1.staminaRect, new Rectangle(x, y, fillWidth, height), fillColor);
        }
        
        // Border
        var borderColor = new Color(60, 60, 60);
        b.Draw(Game1.staminaRect, new Rectangle(x, y, width, 1), borderColor);
        b.Draw(Game1.staminaRect, new Rectangle(x, y + height - 1, width, 1), borderColor);
        b.Draw(Game1.staminaRect, new Rectangle(x, y, 1, height), borderColor);
        b.Draw(Game1.staminaRect, new Rectangle(x + width - 1, y, 1, height), borderColor);
    }

    private void DrawBirthdaysTab(SpriteBatch b, int x, ref int y, int maxWidth)
    {
        // Today's birthdays with full gift lists
        if (_summary!.TodayBirthdays.Count > 0)
        {
            DrawSectionTitle(b, "🎂 Today's Birthdays", x, ref y, Color.HotPink);
            foreach (var bday in _summary.TodayBirthdays)
            {
                var entryY = y;
                var isHovered = _hoveredBirthday == bday;
                var textColor = isHovered ? Color.Yellow : Color.White;
                
                DrawText(b, $"  ★ {bday.NpcName}", x, ref y, textColor);
                
                // Show gifts inline
                if (bday.LovedGifts.Count > 0)
                {
                    DrawText(b, $"    Loved: {string.Join(", ", bday.LovedGifts.Take(4))}", x, ref y, Color.LightPink);
                }
                if (bday.LikedGifts.Count > 0)
                {
                    DrawText(b, $"    Liked: {string.Join(", ", bday.LikedGifts.Take(4))}", x, ref y, Color.Khaki);
                }
                
                var hitbox = new Rectangle(x, entryY, maxWidth, y - entryY);
                _birthdayHitboxes.Add((hitbox, bday));
                y += 8;
            }
            y += SectionSpacing;
        }
        else
        {
            DrawSectionTitle(b, "🎂 No Birthdays Today", x, ref y, Color.Gray);
            y += SectionSpacing;
        }

        // Upcoming birthdays with more detail
        DrawSectionTitle(b, "📅 Upcoming Birthdays", x, ref y, Color.Violet);
        if (_summary.UpcomingBirthdays.Count > 0)
        {
            foreach (var bday in _summary.UpcomingBirthdays)
            {
                var entryY = y;
                var isHovered = _hoveredBirthday == bday;
                var textColor = isHovered ? Color.Yellow : Color.White;
                
                DrawText(b, $"  {bday.BirthdayDate}: {bday.NpcName}", x, ref y, textColor);
                
                if (isHovered && bday.LovedGifts.Count > 0)
                {
                    DrawText(b, $"    ❤ {string.Join(", ", bday.LovedGifts.Take(3))}", x, ref y, Color.LightPink);
                }
                
                var hitbox = new Rectangle(x, entryY, maxWidth, y - entryY);
                _birthdayHitboxes.Add((hitbox, bday));
            }
        }
        else
        {
            DrawText(b, "  No upcoming birthdays in the next 7 days.", x, ref y, Color.LightGray);
        }
        y += SectionSpacing;
    }

    private void DrawTomorrowTab(SpriteBatch b, int x, ref int y, int maxWidth)
    {
        if (_summary!.TomorrowPreview == null)
        {
            DrawSectionTitle(b, "🔮 Tomorrow's Preview", x, ref y, Color.MediumPurple);
            DrawText(b, "  Preview not available.", x, ref y, Color.LightGray);
            return;
        }

        var preview = _summary.TomorrowPreview;

        DrawSectionTitle(b, "🔮 Tomorrow's Forecast", x, ref y, Color.MediumPurple);
        DrawText(b, $"  Weather: {preview.WeatherForecast}", x, ref y, Color.White);
        y += SectionSpacing;

        // Events
        if (preview.Events.Count > 0)
        {
            DrawSectionTitle(b, "📅 Events", x, ref y, Color.Gold);
            foreach (var ev in preview.Events)
            {
                DrawText(b, $"  • {ev}", x, ref y, Color.White);
            }
            y += SectionSpacing;
        }

        // Season change warning
        if (preview.SeasonChangeWarning != null)
        {
            DrawSectionTitle(b, "⚠ Season Warning", x, ref y, Color.OrangeRed);
            DrawText(b, $"  {preview.SeasonChangeWarning}", x, ref y, Color.OrangeRed);
            y += SectionSpacing;
        }

        // New collectibles tomorrow
        if (preview.NewCollectibles.Count > 0)
        {
            DrawSectionTitle(b, "🆕 New Tomorrow", x, ref y, Color.LightGreen);
            foreach (var item in preview.NewCollectibles)
            {
                DrawCollectibleItem(b, item, x + 16, ref y, maxWidth - 16, Color.White);
            }
            y += SectionSpacing;
        }

        // Unavailable tomorrow (expiring items)
        if (preview.ExpiringItems.Count > 0)
        {
            DrawSectionTitle(b, "❌ Last Day for These", x, ref y, Color.OrangeRed);
            foreach (var item in preview.ExpiringItems)
            {
                DrawCollectibleItem(b, item, x + 16, ref y, maxWidth - 16, Color.OrangeRed);
            }
            y += SectionSpacing;
        }
    }

    private void DrawCollectionsTab(SpriteBatch b, int x, ref int y, int maxWidth)
    {
        DrawSectionTitle(b, "📋 All Collection Items", x, ref y, Color.Yellow);
        y += 8;

        if (_summary!.AllCollectionItems.Count == 0)
        {
            DrawText(b, "  No collection data available.", x, ref y, Color.LightGray);
        }
        else
        {
            // Group by collection status
            var collected = _summary.AllCollectionItems.Where(i => i.CollectionStatus == CollectionStatus.EverCollected).ToList();
            var inInventory = _summary.AllCollectionItems.Where(i => i.CollectionStatus == CollectionStatus.InInventory).ToList();
            var notCollected = _summary.AllCollectionItems.Where(i => i.CollectionStatus == CollectionStatus.NotCollected).ToList();

            // Summary
            DrawText(b, $"  Total: {_summary.AllCollectionItems.Count} | Collected: {collected.Count} | Remaining: {notCollected.Count}", x, ref y, Color.White);
            y += SectionSpacing;

            // Not collected (priority)
            if (notCollected.Count > 0)
            {
                DrawText(b, $"  ○ Not Yet Collected ({notCollected.Count}):", x, ref y, Color.LightCoral);
                foreach (var item in notCollected)
                {
                    DrawCollectibleItem(b, item, x + 32, ref y, maxWidth - 32, Color.White);
                }
                y += SectionSpacing;
            }

            // In inventory
            if (inInventory.Count > 0)
            {
                DrawText(b, $"  ★ In Inventory ({inInventory.Count}):", x, ref y, Color.LightGreen);
                foreach (var item in inInventory)
                {
                    DrawCollectibleItem(b, item, x + 32, ref y, maxWidth - 32, Color.White);
                }
                y += SectionSpacing;
            }

            // Already collected
            if (collected.Count > 0)
            {
                DrawText(b, $"  ✓ Already Collected ({collected.Count}):", x, ref y, Color.Gray);
                foreach (var item in collected.Take(20))
                {
                    DrawCollectibleItem(b, item, x + 32, ref y, maxWidth - 32, Color.DarkGray);
                }
                if (collected.Count > 20)
                {
                    DrawText(b, $"    ... and {collected.Count - 20} more", x, ref y, Color.DarkGray);
                }
            }
        }
        y += SectionSpacing;
    }

    private void DrawPerfectionTab(SpriteBatch b, int x, ref int y, int maxWidth)
    {
        if (_perfectionService == null)
        {
            DrawSectionTitle(b, "🎯 Perfection Tracker", x, ref y, Color.Yellow);
            y += 8;
            DrawText(b, "  Perfection data not available.", x, ref y, Color.LightGray);
            return;
        }

        var progress = _perfectionService.GetProgress();

        // Header with overall percentage
        var headerColor = progress.IsComplete ? Color.Gold : Color.Yellow;
        DrawSectionTitle(b, $"🎯 Perfection: {progress.TotalPercentage:F1}%", x, ref y, headerColor);
        y += 8;

        // Progress bar
        DrawProgressBar(b, x + 16, y, maxWidth - 32, 20, progress.TotalPercentage / 100.0);
        y += 28;

        // Ginger Island status
        if (!progress.GingerIslandUnlocked)
        {
            DrawText(b, "  🏝️ Ginger Island not yet unlocked", x, ref y, Color.Orange);
            y += 8;
        }

        // Congratulations message if perfect
        if (progress.IsComplete)
        {
            DrawText(b, "  ✨ Congratulations! You've achieved 100% Perfection! ✨", x, ref y, Color.Gold);
            y += SectionSpacing;
        }

        // Category breakdown
        y += 8;
        foreach (var category in progress.Categories)
        {
            DrawPerfectionCategory(b, category, x, ref y, maxWidth);
        }
    }

    private void DrawPerfectionCategory(SpriteBatch b, PerfectionCategory category, int x, ref int y, int maxWidth)
    {
        var statusIcon = category.IsComplete ? "✓" : "○";
        var color = category.IsComplete ? Color.LightGreen : Color.White;
        var percentText = $"{category.PercentComplete:F1}%";
        var countText = $"({category.CurrentCount}/{category.TotalCount})";
        var weightText = $"[{category.Weight:F0}%]";

        var line = $"  {statusIcon} {category.CategoryName}: {percentText} {countText} {weightText}";
        DrawText(b, line, x, ref y, color);
    }

    private void DrawProgressBar(SpriteBatch b, int x, int y, int width, int height, double fillPercent)
    {
        // Background
        b.Draw(Game1.staminaRect, new Rectangle(x, y, width, height), Color.DarkGray);

        // Fill
        var fillWidth = (int)(width * Math.Min(fillPercent, 1.0));
        if (fillWidth > 0)
        {
            var fillColor = fillPercent >= 1.0 ? Color.Gold : Color.LimeGreen;
            b.Draw(Game1.staminaRect, new Rectangle(x, y, fillWidth, height), fillColor);
        }

        // Border
        var borderColor = Color.White * 0.5f;
        b.Draw(Game1.staminaRect, new Rectangle(x, y, width, 2), borderColor);
        b.Draw(Game1.staminaRect, new Rectangle(x, y + height - 2, width, 2), borderColor);
        b.Draw(Game1.staminaRect, new Rectangle(x, y, 2, height), borderColor);
        b.Draw(Game1.staminaRect, new Rectangle(x + width - 2, y, 2, height), borderColor);
    }

    private void DrawBirthdayTooltip(SpriteBatch b)
    {
        if (_hoveredBirthday == null)
            return;

        var mousePos = Game1.getMousePosition();
        
        // Build tooltip text
        var lines = new List<string>
        {
            $"♥ {_hoveredBirthday.NpcName}'s Favorites"
        };

        if (_hoveredBirthday.LovedGifts.Count > 0)
        {
            lines.Add("");
            lines.Add("Loved Gifts:");
            foreach (var gift in _hoveredBirthday.LovedGifts.Take(6))
                lines.Add($"  ❤ {gift}");
        }

        if (_hoveredBirthday.LikedGifts.Count > 0)
        {
            lines.Add("");
            lines.Add("Liked Gifts:");
            foreach (var gift in _hoveredBirthday.LikedGifts.Take(4))
                lines.Add($"  💛 {gift}");
        }

        if (_hoveredBirthday.LovedGifts.Count == 0 && _hoveredBirthday.LikedGifts.Count == 0)
        {
            lines.Add("  (No gift data available)");
        }

        // Calculate tooltip size
        var font = Game1.smallFont;
        float maxWidth = 0;
        float totalHeight = 0;
        foreach (var line in lines)
        {
            var size = font.MeasureString(line);
            maxWidth = Math.Max(maxWidth, size.X);
            totalHeight += size.Y;
        }

        int padding = 16;
        int tooltipWidth = (int)maxWidth + padding * 2;
        int tooltipHeight = (int)totalHeight + padding * 2;

        // Position tooltip to the left of the cursor, or right if not enough space
        int tooltipX = mousePos.X - tooltipWidth - 16;
        if (tooltipX < 0)
            tooltipX = mousePos.X + 24;

        int tooltipY = mousePos.Y;
        var viewport = Game1.graphics.GraphicsDevice.Viewport;
        if (tooltipY + tooltipHeight > viewport.Height)
            tooltipY = viewport.Height - tooltipHeight;

        // Draw tooltip background
        IClickableMenu.drawTextureBox(b,
            Game1.menuTexture,
            new Rectangle(0, 256, 60, 60),
            tooltipX, tooltipY,
            tooltipWidth, tooltipHeight,
            new Color(40, 30, 20), 1f, false);

        // Draw tooltip text
        float textY = tooltipY + padding;
        foreach (var line in lines)
        {
            var color = line.StartsWith("Loved") ? Color.HotPink :
                        line.StartsWith("Liked") ? Color.Gold :
                        line.StartsWith("  ❤") ? Color.LightPink :
                        line.StartsWith("  💛") ? Color.Khaki :
                        line.StartsWith("♥") ? Color.White :
                        Color.LightGray;

            Utility.drawTextWithShadow(b, line, font, 
                new Vector2(tooltipX + padding, textY), color);
            textY += font.MeasureString(line).Y;
        }
    }

    private void DrawSectionTitle(SpriteBatch b, string title, int x, ref int y, Color color)
    {
        Utility.drawTextWithShadow(b, title, Game1.smallFont, new Vector2(x, y), color);
        y += LineHeight;
    }

    private void DrawText(SpriteBatch b, string text, int x, ref int y, Color color)
    {
        Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(x, y), color);
        y += LineHeight - 8;
    }

    private static void DrawCollectibleItem(SpriteBatch b, CollectibleItem item, int x, ref int y, int maxWidth, Color baseColor)
    {
        // Use distinct symbols for accessibility (colorblind-friendly)
        var (statusIcon, statusSuffix) = item.CollectionStatus switch
        {
            CollectionStatus.EverCollected => ("✓ ", " [DONE]"),
            CollectionStatus.InInventory => ("★ ", " [HAVE]"),
            CollectionStatus.NotCollected => ("○ ", ""),
            _ => ("  ", "")
        };

        // Use consistent white color for all items - status indicated by symbols
        var color = Color.White;

        var details = "";
        if (item.RequiredWeather.Count > 0)
            details += $" [{string.Join("/", item.RequiredWeather)}]";

        Utility.drawTextWithShadow(b,
            $"{statusIcon}{item.Name}{details}{statusSuffix}",
            Game1.smallFont,
            new Vector2(x, y),
            color);
        y += LineHeight - 4;
    }

    private void DrawCloseButton(SpriteBatch b)
    {
        IClickableMenu.drawTextureBox(b,
            Game1.mouseCursors,
            new Rectangle(337, 494, 12, 12),
            _closeButtonBounds.X, _closeButtonBounds.Y,
            _closeButtonBounds.Width, _closeButtonBounds.Height,
            Color.White, 2f, false);

        Utility.drawTextWithShadow(b, "X", Game1.smallFont,
            new Vector2(_closeButtonBounds.X + 6, _closeButtonBounds.Y + 2),
            Color.Red);
    }

    private void DrawScrollIndicators(SpriteBatch b, Rectangle contentArea)
    {
        if (_scrollOffset > 0)
        {
            // Up arrow indicator
            Utility.drawTextWithShadow(b, "▲", Game1.smallFont,
                new Vector2(contentArea.X + contentArea.Width / 2 - 8, contentArea.Y - 4),
                Color.White);
        }

        if (_contentHeight > contentArea.Height + _scrollOffset)
        {
            // Down arrow indicator
            Utility.drawTextWithShadow(b, "▼", Game1.smallFont,
                new Vector2(contentArea.X + contentArea.Width / 2 - 8, contentArea.Bottom - 16),
                Color.White);
        }
    }

    /// <inheritdoc/>
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (_closeButtonBounds.Contains(x, y))
        {
            IsVisible = false;
            if (playSound)
                Game1.playSound("bigDeSelect");
            return;
        }

        // Check tab clicks
        for (int i = 0; i < AllTabs.Length; i++)
        {
            if (_tabBounds[i].Contains(x, y))
            {
                ActiveTab = AllTabs[i];
                if (playSound)
                    Game1.playSound("smallSelect");
                return;
            }
        }

        // Check availability filter toggle (only on Bundles tab)
        if (_activeTab == PlannerTab.Bundles && _availabilityFilterButtonBounds.Contains(x, y))
        {
            _showAvailableTodayOnly = !_showAvailableTodayOnly;
            if (playSound)
                Game1.playSound("smallSelect");
            return;
        }
    }

    /// <inheritdoc/>
    public override void receiveScrollWheelAction(int direction)
    {
        _scrollOffset -= direction > 0 ? ScrollStep : -ScrollStep;
        _scrollOffset = Math.Max(0, _scrollOffset);

        var maxScroll = Math.Max(0, _contentHeight - (_panelBounds.Height - HeaderHeight - TabBarHeight - 16));
        _scrollOffset = Math.Min(_scrollOffset, maxScroll);
    }
}
