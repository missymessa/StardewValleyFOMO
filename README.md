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
- Highlights items needed for incomplete bundles
- Shows which bundles need each collectible
- Helps prioritize high-value items

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

Edit `config.json` in the mod folder to customize:

```json
{
  "ToggleKey": "F7",
  "LookaheadDays": 7
}
```

## Requirements

- Stardew Valley 1.6+
- SMAPI 4.0+
- .NET 6.0 (included with SMAPI)

## Building from Source

```bash
# Clone the repository
git clone https://github.com/missymessa/StardewValleyFOMO.git
cd StardewValleyFOMO

# Build the solution
dotnet build -c Release
```

The mod will be automatically copied to your game's Mods folder if the path is configured correctly.

The release zip is generated at `src/StardewFOMO.Mod/bin/Release/net6.0/StardewFOMO.Mod 1.0.0.zip`

### Creating a Release

1. Build the mod locally: `dotnet build -c Release`
2. Go to GitHub → Actions → "Create Release"
3. Enter the version number (e.g., `1.0.1`)
4. Run the workflow
5. Edit the created release and upload the zip from your local build

## Project Structure

```
StardewValleyFOMO/
├── src/
│   ├── StardewFOMO.Core/       # Core domain models and abstractions
│   └── StardewFOMO.Mod/        # SMAPI mod implementation
├── tests/
│   └── StardewFOMO.Tests/      # Unit tests (60+ tests)
└── specs/
    └── 001-daily-planner/      # Feature specifications
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
