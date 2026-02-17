# Stardew Valley FOMO - Daily Planner

A SMAPI mod for Stardew Valley that helps players track collectibles, upcoming events, and bundle progress to eliminate the "Fear Of Missing Out" (FOMO).

![Stardew Valley](https://img.shields.io/badge/Stardew%20Valley-1.6-green)
![SMAPI](https://img.shields.io/badge/SMAPI-4.0%2B-blue)
![.NET](https://img.shields.io/badge/.NET-6.0-purple)

## Features

### 📋 Today's Collectibles Overview
- View all fish available based on current season, weather, and time of day
- See forageable items for the current season grouped by location
- Visual indicators show what you've already collected vs. what's still needed

### 🎁 Community Center Bundle Tracker
- **Room Progress Overview**: View all 6 Community Center rooms with progress bars showing completion percentage
- **Bundle Details**: Each bundle shows X/Y item counts with visual completion indicators
- **Item Requirements**: Expand bundles to see exactly which items are still needed, including quality requirements (Silver ☆, Gold ★, Iridium ◆)
- **OR Requirements**: Remixed bundles display alternative item options ("Item A or Item B")
- **Owned Item Highlighting**: Items you have in inventory or storage show "★ [HAVE]" indicator
- **"Available Today" Filter**: Toggle to show only bundles with items obtainable based on current season, weather, and time
- **Bundle Pickup Notifications**: Get a HUD message when you pick up an item needed for a bundle
- **Joja Route Handling**: Gracefully displays a message if you chose the Joja Mart path

### 🎂 NPC Birthday Reminders
- Shows today's birthdays with gift suggestions
- Displays upcoming birthdays within the next 7 days
- Hover over a birthday to see loved and liked gifts

### 🔮 Tomorrow's Preview
- Weather forecast for planning ahead
- Alerts for rain-exclusive fish becoming available
- Season-end warnings for last-chance items

### 🎮 In-Game Panel
- Press **F7** (configurable) to toggle the daily planner overlay
- Scrollable panel with organized sections
- Non-intrusive overlay that doesn't pause gameplay
- Colorblind-friendly status indicators

## Installation

1. Install [SMAPI](https://smapi.io/) (4.0 or later)
2. Download the latest release from the [Releases](../../releases) page
3. Extract the zip file into your `Stardew Valley/Mods` folder
4. Launch the game through SMAPI

## Usage

- Press **F7** during gameplay to open/close the daily planner
- **Scroll** the mouse wheel to navigate long lists
- **Hover** over a birthday entry to see gift suggestions
- The panel won't open during menus or cutscenes

### Status Indicators

| Symbol | Meaning |
|--------|---------|
| ✓ [DONE] | Already collected/completed |
| ★ [HAVE] | You have this item in inventory or storage |
| ○ | Not yet collected |

## Configuration

### In-Game Configuration (Recommended)
Install [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) for an in-game settings menu. Access it from the title screen or in-game options.

### Manual Configuration
Edit `config.json` in the mod folder:

```json
{
  "ToggleKey": "F7",
  "BirthdayLookaheadDays": 7,
  "SeasonAlertDays": 3,
  "EnableBundleNotifications": true
}
```

| Setting | Default | Description |
|---------|---------|-------------|
| ToggleKey | F7 | Key to open/close the planner |
| BirthdayLookaheadDays | 7 | Days ahead to show upcoming birthdays |
| SeasonAlertDays | 3 | Days before season end for last-chance alerts |
| EnableBundleNotifications | true | Show HUD notification when picking up bundle items |

## Requirements

- Stardew Valley 1.6+
- SMAPI 4.0+
- [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (optional, for in-game settings)

## Building from Source

```bash
# Clone the repository
git clone https://github.com/missymessa/StardewValleyFOMO.git
cd StardewValleyFOMO

# Build and install to game (requires Stardew Valley + SMAPI installed)
dotnet build -c Release
```

The mod will be automatically copied to your game's Mods folder if the path is configured correctly.

The release zip is generated at `src/StardewFOMO.Mod/bin/Release/net6.0/StardewFOMO.Mod X.Y.Z.zip`

### Development Workflow

```bash
# Quick build and install (Debug mode)
dotnet build src/StardewFOMO.Mod/StardewFOMO.Mod.csproj

# Run tests
dotnet test

# Watch mode - auto-rebuild on file changes
dotnet watch build --project src/StardewFOMO.Mod/StardewFOMO.Mod.csproj

# Or use the PowerShell script
./scripts/dev-build.ps1           # Debug build
./scripts/dev-build.ps1 -Release  # Release build
./scripts/dev-build.ps1 -Test     # Run tests first
./scripts/dev-build.ps1 -Watch    # Watch mode
```

**VS Code**: Press `Ctrl+Shift+B` to build and install, or use the Command Palette → "Tasks: Run Task".

### Creating a Release

1. Go to GitHub → Actions → **"Create Release"**
2. Select the version bump type:
   - **patch** (1.0.0 → 1.0.1) - Bug fixes
   - **minor** (1.0.0 → 1.1.0) - New features
   - **major** (1.0.0 → 2.0.0) - Breaking changes
3. Run the workflow (version is auto-incremented from manifest.json)
4. Build locally: `dotnet build -c Release`
5. Edit the release and upload the zip from your local build

## Project Structure

```
StardewValleyFOMO/
├── src/
│   ├── StardewFOMO.Core/       # Core domain models and abstractions
│   └── StardewFOMO.Mod/        # SMAPI mod implementation
├── tests/
│   └── StardewFOMO.Core.Tests/ # Unit tests (95+ tests)
└── specs/
    ├── 001-daily-planner/      # Daily planner feature specifications
    └── 002-bundle-tracker/     # Bundle tracker feature specifications
```

## Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is open source. See [LICENSE](LICENSE) for details.

## Credits

- [SMAPI](https://smapi.io/) - Modding framework by Pathoschild
- [Stardew Valley](https://www.stardewvalley.net/) - Game by ConcernedApe

---

*Made with ❤️ for the Stardew Valley community*
