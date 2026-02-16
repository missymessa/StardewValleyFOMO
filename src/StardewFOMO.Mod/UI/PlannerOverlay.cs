using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewFOMO.Core.Models;

namespace StardewFOMO.Mod.UI;

/// <summary>
/// In-game overlay panel that renders the daily planner using MonoGame/SpriteBatch.
/// Supports priority view (default) and "Show All" toggle.
/// Session-scoped view state that resets on game restart.
/// </summary>
public sealed class PlannerOverlay : IClickableMenu
{
    private const int PanelMargin = 32;
    private const int SectionSpacing = 16;
    private const int LineHeight = 36;
    private const int HeaderHeight = 48;
    private const int ToggleButtonWidth = 120;
    private const int ToggleButtonHeight = 40;
    private const int ScrollStep = 40;

    private DailySummary? _summary;
    private bool _showAll;
    private int _scrollOffset;
    private int _contentHeight;

    private Rectangle _panelBounds;
    private Rectangle _toggleButtonBounds;
    private Rectangle _closeButtonBounds;

    // Birthday hover tracking
    private readonly List<(Rectangle Bounds, NpcBirthday Birthday)> _birthdayHitboxes = new();
    private NpcBirthday? _hoveredBirthday;

    /// <summary>Whether the overlay is currently visible.</summary>
    public bool IsVisible { get; set; }

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

    /// <summary>Toggle between priority and "Show All" views.</summary>
    public void ToggleShowAll()
    {
        _showAll = !_showAll;
        _scrollOffset = 0;
    }

    /// <summary>Reset session state (called on game restart/load).</summary>
    public void ResetSession()
    {
        _showAll = false;
        _scrollOffset = 0;
        _summary = null;
        _birthdayHitboxes.Clear();
        _hoveredBirthday = null;
    }

    private void RecalculateBounds()
    {
        var viewport = Game1.graphics.GraphicsDevice.Viewport;
        int panelWidth = Math.Min(600, viewport.Width - PanelMargin * 2);
        int panelHeight = Math.Min(viewport.Height - PanelMargin * 2, 800);
        int panelX = viewport.Width - panelWidth - PanelMargin;
        int panelY = PanelMargin;

        _panelBounds = new Rectangle(panelX, panelY, panelWidth, panelHeight);

        _toggleButtonBounds = new Rectangle(
            panelX + panelWidth - ToggleButtonWidth - 16 - 40,
            panelY + 8,
            ToggleButtonWidth,
            ToggleButtonHeight);

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

        // Draw panel background with darker tint for readability
        IClickableMenu.drawTextureBox(b,
            Game1.menuTexture,
            new Rectangle(0, 256, 60, 60),
            _panelBounds.X, _panelBounds.Y,
            _panelBounds.Width, _panelBounds.Height,
            new Color(60, 40, 30), 1f, false);

        // Draw slightly lighter inner panel for content area
        b.Draw(Game1.staminaRect, 
            new Rectangle(_panelBounds.X + 12, _panelBounds.Y + 12, 
                _panelBounds.Width - 24, _panelBounds.Height - 24),
            new Color(80, 60, 40, 230));

        // Set up clipping for scroll
        var originalScissor = b.GraphicsDevice.ScissorRectangle;
        var contentArea = new Rectangle(
            _panelBounds.X + 16,
            _panelBounds.Y + HeaderHeight,
            _panelBounds.Width - 32,
            _panelBounds.Height - HeaderHeight - 16);

        b.End();
        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
            null, new RasterizerState { ScissorTestEnable = true });
        b.GraphicsDevice.ScissorRectangle = contentArea;

        int y = contentArea.Y - _scrollOffset;
        int x = contentArea.X;
        int maxWidth = contentArea.Width;

        // Header
        DrawHeader(b, x, ref y, maxWidth);

        // Festival notice
        if (_summary.IsFestivalDay && _summary.FestivalName != null)
        {
            DrawSectionTitle(b, $"🎉 {_summary.FestivalName}", x, ref y, Color.Gold);
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

        // Today's birthdays
        if (_summary.TodayBirthdays.Count > 0)
        {
            DrawSectionTitle(b, "🎂 Today's Birthdays", x, ref y, Color.HotPink);
            foreach (var bday in _summary.TodayBirthdays)
            {
                var entryY = y;
                var isHovered = _hoveredBirthday == bday;
                var textColor = isHovered ? Color.Yellow : Color.White;
                
                DrawText(b, $"  {bday.NpcName} (hover for gifts)", x, ref y, textColor);
                
                // Register hitbox for hover detection (adjusted for scroll)
                var hitbox = new Rectangle(x, entryY, maxWidth, y - entryY);
                _birthdayHitboxes.Add((hitbox, bday));
            }
            y += SectionSpacing;
        }

        // Bundle-needed items
        if (_summary.BundleNeededItems.Count > 0)
        {
            DrawSectionTitle(b, "📦 Bundle Items Available", x, ref y, Color.Yellow);
            foreach (var item in _summary.BundleNeededItems)
            {
                var bundleText = string.Join(", ", item.BundleNames);
                DrawCollectibleItem(b, item, x + 16, ref y, maxWidth - 16, Color.White);
                DrawText(b, $"    → {bundleText}", x, ref y, Color.Khaki);
            }
            y += SectionSpacing;
        }

        // Available fish
        DrawSectionTitle(b, $"🐟 Fish ({_summary.AvailableFish.Count})", x, ref y, Color.Cyan);
        foreach (var fish in _summary.AvailableFish)
        {
            DrawCollectibleItem(b, fish, x + 16, ref y, maxWidth - 16, Color.White);
        }
        y += SectionSpacing;

        // Available forageables
        DrawSectionTitle(b, $"🌿 Forageables ({_summary.AvailableForageables.Count})", x, ref y, Color.LimeGreen);
        foreach (var forage in _summary.AvailableForageables)
        {
            DrawCollectibleItem(b, forage, x + 16, ref y, maxWidth - 16, Color.White);
        }
        y += SectionSpacing;

        // Upcoming birthdays
        if (_summary.UpcomingBirthdays.Count > 0)
        {
            DrawSectionTitle(b, "📅 Upcoming Birthdays", x, ref y, Color.Violet);
            foreach (var bday in _summary.UpcomingBirthdays)
            {
                var entryY = y;
                var isHovered = _hoveredBirthday == bday;
                var textColor = isHovered ? Color.Yellow : Color.White;
                
                DrawText(b, $"  {bday.NpcName} — {bday.BirthdayDate} (hover)", x, ref y, textColor);
                
                // Register hitbox for hover detection
                var hitbox = new Rectangle(x, entryY, maxWidth, y - entryY);
                _birthdayHitboxes.Add((hitbox, bday));
            }
            y += SectionSpacing;
        }

        // Tomorrow preview
        if (_summary.TomorrowPreview != null)
        {
            DrawSectionTitle(b, "🔮 Tomorrow", x, ref y, Color.MediumPurple);
            DrawText(b, $"  Weather: {_summary.TomorrowPreview.WeatherForecast}", x, ref y, Color.White);

            if (_summary.TomorrowPreview.Events.Count > 0)
            {
                foreach (var ev in _summary.TomorrowPreview.Events)
                    DrawText(b, $"  {ev}", x, ref y, Color.Gold);
            }

            if (_summary.TomorrowPreview.SeasonChangeWarning != null)
                DrawText(b, $"  ⚠ {_summary.TomorrowPreview.SeasonChangeWarning}", x, ref y, Color.OrangeRed);

            if (_summary.TomorrowPreview.NewCollectibles.Count > 0)
            {
                DrawText(b, $"  New tomorrow: {_summary.TomorrowPreview.NewCollectibles.Count} item(s)", x, ref y, Color.LightGreen);
            }
            y += SectionSpacing;
        }

        // "Show All" collection items
        if (_showAll && _summary.AllCollectionItems.Count > 0)
        {
            DrawSectionTitle(b, "📋 All Collection Items", x, ref y, Color.Yellow);
            foreach (var item in _summary.AllCollectionItems)
            {
                DrawCollectibleItem(b, item, x + 16, ref y, maxWidth - 16, Color.White);
            }
            y += SectionSpacing;
        }

        _contentHeight = y + _scrollOffset - contentArea.Y;

        // Restore scissor rect
        b.End();
        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        b.GraphicsDevice.ScissorRectangle = originalScissor;

        // Draw toggle button
        DrawToggleButton(b);

        // Draw close button
        DrawCloseButton(b);

        // Draw scroll indicators
        DrawScrollIndicators(b, contentArea);

        // Draw birthday tooltip if hovering
        DrawBirthdayTooltip(b);

        drawMouse(b);
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

    private void DrawHeader(SpriteBatch b, int x, ref int y, int maxWidth)
    {
        var dateStr = _summary != null
            ? $"{_summary.Date.Season} {_summary.Date.Day}, Year {_summary.Date.Year}"
            : "Loading...";
        var weatherStr = _summary != null ? $" — {_summary.CurrentWeather}" : "";

        Utility.drawTextWithShadow(b,
            $"Daily Planner: {dateStr}{weatherStr}",
            Game1.smallFont,
            new Vector2(x, y),
            Color.White);
        y += LineHeight + 4;
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

    private void DrawToggleButton(SpriteBatch b)
    {
        var label = _showAll ? "Priority" : "Show All";
        IClickableMenu.drawTextureBox(b,
            Game1.mouseCursors,
            new Rectangle(432, 439, 9, 9),
            _toggleButtonBounds.X, _toggleButtonBounds.Y,
            _toggleButtonBounds.Width, _toggleButtonBounds.Height,
            Color.White, 4f, false);

        Utility.drawTextWithShadow(b,
            label,
            Game1.smallFont,
            new Vector2(
                _toggleButtonBounds.X + (_toggleButtonBounds.Width - Game1.smallFont.MeasureString(label).X) / 2,
                _toggleButtonBounds.Y + 8),
            Color.White);
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

        if (_toggleButtonBounds.Contains(x, y))
        {
            ToggleShowAll();
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

        var maxScroll = Math.Max(0, _contentHeight - (_panelBounds.Height - HeaderHeight - 16));
        _scrollOffset = Math.Min(_scrollOffset, maxScroll);
    }
}
