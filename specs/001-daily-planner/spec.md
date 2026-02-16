# Feature Specification: Daily Planner — Stardew Valley FOMO Mod

**Feature Branch**: `001-daily-planner`  
**Created**: February 15, 2026  
**Status**: Draft  
**Input**: User description: "This Stardew Valley mod should help the player determine what achievements they can do on the current day and look forward to the next day to prepare them for what items they'll need for that day. It should look at the weather and season and inform the player about fish or foragables that can be gathered, especially if any are missing from the community center bundles. It should tell the user which NPCs are having birthdays (or upcoming birthdays) and what their loved/liked items are so the user can gather them. It will need a UI for the game, and it will need to interact with the players current game variables."

## User Scenarios & Testing *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must be INDEPENDENTLY TESTABLE - meaning if you implement just ONE of them,
  you should still have a viable MVP (Minimum Viable Product) that delivers value.
  
  Assign priorities (P1, P2, P3, etc.) to each story, where P1 is the most critical.
  Think of each story as a standalone slice of functionality that can be:
  - Developed independently
  - Tested independently
  - Deployed independently
  - Demonstrated to users independently
-->

### User Story 1 — Today's Collectibles Overview (Priority: P1)

As a player, I want to see a summary of all trackable collectibles available today — including fish, forageables, artifacts/minerals obtainable through foraging or geodes, shippable items, cooking recipes (if ingredients are available), and craftable items (if materials are available) — based on the current season, weather, and time of day so that I know what I can pursue without missing anything.

**Why this priority**: The core value of this mod is eliminating FOMO. Knowing what's available *right now* is the single most impactful piece of information the mod can deliver. A player who sees "today you can catch a Catfish (rainy, Spring)" or "you have ingredients for Maki Roll (uncollected recipe)" gets immediate, actionable value.

**Independent Test**: Can be fully tested by opening the daily planner panel on any in-game day and verifying it lists the correct fish, forageable items, obtainable artifacts/minerals, achievable cooking/crafting recipes, and unshipped items for the current season, weather, and location. Delivers immediate value by informing the player what to pursue today.

**Acceptance Scenarios**:

1. **Given** it is Spring, Day 3, and it is raining, **When** the player opens the daily planner, **Then** the panel displays all fish catchable in rainy Spring weather and all Spring forageable items, grouped by location.
2. **Given** it is Summer and sunny, **When** the player opens the daily planner, **Then** only fish available in sunny Summer weather are shown (rain-exclusive fish are hidden).
3. **Given** the player has already caught certain fish, **When** the player opens the daily planner, **Then** previously caught fish are visually distinguished (e.g., dimmed or checked off) from uncollected fish.

---

### User Story 2 — Community Center Bundle Tracker (Priority: P1)

As a player, I want the daily planner to highlight which of today's available fish and forageable items are still needed for incomplete Community Center bundles so I can prioritize collecting those items first.

**Why this priority**: Community Center completion is a primary progression goal. Surfacing bundle-relevant items among today's collectibles transforms the planner from a reference tool into a strategic guide, which is the mod's key differentiator.

**Independent Test**: Can be tested by loading a save with partially completed bundles and verifying the planner highlights bundle-needed items with the correct bundle name. Delivers value by focusing the player on high-priority collecting.

**Acceptance Scenarios**:

1. **Given** the player has not completed the Spring Foraging Bundle and it is Spring, **When** the player opens the daily planner, **Then** each Spring forageable item needed for the bundle is flagged with the bundle name (e.g., "Spring Foraging Bundle").
2. **Given** the player has already completed all bundles that use Spring fish, **When** the player opens the daily planner in Spring, **Then** no fish are marked as bundle-needed (but they still appear in the general collectibles list).
3. **Given** multiple bundles need the same item, **When** the player views that item in the planner, **Then** all relevant bundle names are shown.

---

### User Story 3 — NPC Birthday Reminders (Priority: P2)

As a player, I want to see which NPCs have birthdays today or in the upcoming days so I can prepare and give them loved or liked gifts to maximize friendship.

**Why this priority**: Birthday gifts give a large friendship boost and are easy to miss. This is high-value, low-complexity information that complements the collecting focus and rounds out the "don't miss anything today" theme.

**Independent Test**: Can be tested by advancing to any NPC's birthday and confirming the planner displays the NPC name, birthday date, and gift preferences. Delivers standalone value for friendship optimization.

**Acceptance Scenarios**:

1. **Given** it is Haley's birthday (Spring 14), **When** the player opens the daily planner, **Then** a birthday alert shows Haley's name, and her loved and liked gift items are listed.
2. **Given** today is Spring 11, **When** the player opens the daily planner, **Then** an "Upcoming Birthdays" section shows birthdays within the next 7 days (e.g., Haley on Spring 14).
3. **Given** no NPC has a birthday within the next 7 days, **When** the player opens the daily planner, **Then** the upcoming birthdays section shows the next birthday on the calendar with a note like "Next birthday: Sam on Summer 17".

---

### User Story 4 — Tomorrow's Preview & Preparation (Priority: P2)

As a player, I want to see a preview of tomorrow's expected activities — including weather forecast, newly available collectibles, and upcoming events — so I can prepare items and plan my time the evening before.

**Why this priority**: Planning ahead is the second half of the FOMO equation. Knowing what's coming tomorrow allows the player to craft, purchase, or hold items overnight, turning reactive play into proactive strategy.

**Independent Test**: Can be tested by checking the "Tomorrow" section on any day and confirming it shows the correct weather forecast, any new fish or forageable items becoming available, and upcoming events. Delivers value by enabling overnight preparation.

**Acceptance Scenarios**:

1. **Given** the weather forecast for tomorrow is rain and it is currently sunny, **When** the player opens the daily planner, **Then** the "Tomorrow" section indicates rain and lists rain-exclusive fish that will become available.
2. **Given** tomorrow is the last day of Spring, **When** the player opens the daily planner, **Then** the "Tomorrow" section warns that Spring-exclusive items will no longer be available and lists any uncollected Spring items.
3. **Given** tomorrow is a festival day, **When** the player opens the daily planner, **Then** the "Tomorrow" section shows the festival name, time, and any items the player should bring or prepare.

---

### User Story 5 — In-Game Planner Panel (Priority: P2)

As a player, I want to access the daily planner through a convenient in-game panel that shows me the most important, actionable items first — bundle-needed items, time-sensitive collectibles, and today's birthdays — without overwhelming me, with the option to expand into full collection tracking via a "Show All" toggle.

**Why this priority**: All the mod's information is useless without a way to see it. The panel is the delivery mechanism. It is P2 (not P1) because the data logic must exist first; the panel is the presentation layer. The priority-based default view ensures immediate utility without information overload.

**Independent Test**: Can be tested by pressing the configured hotkey and verifying the panel opens with the priority view (bundle-needed, time-sensitive, birthdays), then toggling "Show All" to see complete collection data, and closing without affecting game state.

**Acceptance Scenarios**:

1. **Given** the player is in normal gameplay, **When** the player presses the configured hotkey, **Then** the daily planner panel opens as an overlay without pausing the game, showing the priority view (bundle-needed items, time-sensitive items, today's birthdays, last-chance alerts).
2. **Given** the planner is open in the default priority view, **When** the player clicks the "Show All" toggle, **Then** the panel expands to show all trackable collection items (shipping, museum, cooking, crafting, fish, forageables) with their completion status.
3. **Given** the daily planner panel is open, **When** the player presses the hotkey again or clicks a close button, **Then** the panel closes and gameplay continues.
4. **Given** the player is in a menu or cutscene, **When** the player presses the hotkey, **Then** the panel does not open (to avoid UI conflicts).

---

### User Story 6 — Season Change & Last-Chance Alerts (Priority: P3)

As a player, I want to receive prominent alerts when the season is about to end and I have uncollected season-exclusive items (fish, forageables, or bundle items) so I don't miss them for an entire in-game year.

**Why this priority**: This is the ultimate anti-FOMO feature but only activates near season boundaries. It builds on P1 data (collectibles + bundle tracking) and enhances it with urgency notifications.

**Independent Test**: Can be tested by advancing to the last 3 days of a season with uncollected seasonal items and verifying an alert appears with the list of at-risk items. Delivers value by preventing year-long delays.

**Acceptance Scenarios**:

1. **Given** it is Spring 26 and the player has not caught a Catfish (Spring rain-only), **When** the player opens the daily planner, **Then** a "Last Chance" alert prominently lists the Catfish and notes it requires rainy weather.
2. **Given** all season-exclusive items have been collected, **When** the season's final days arrive, **Then** no last-chance alert is shown.
3. **Given** it is the last day of the season and the weather does not permit catching a weather-dependent fish, **When** the player opens the planner, **Then** the alert notes the item cannot be obtained today and the player will need to wait until next year.

---

### Edge Cases

- What happens when the player is in Year 1 vs. later years? The mod reflects the player's actual bundle state regardless of year — uncompleted bundle items remain highlighted.
- How does the mod handle the player choosing the Joja route instead of the Community Center? Bundle tracking is hidden or adapted; the planner still shows collectible information without bundle associations.
- What happens when the player has completed all Community Center bundles? Bundle-related highlights disappear; the planner still shows collectibles for completionist tracking such as the shipping collection.
- How does the planner handle festival days where normal foraging/fishing is restricted? The planner notes that normal activities are limited and highlights festival-specific opportunities instead.
- What happens when the player installs the mod mid-playthrough? The mod reads the current save state and correctly reflects what has and has not been collected/completed.
- How does the mod handle modded content (e.g., new fish or NPCs from other mods)? The mod gracefully handles unknown items and only displays data it can confirm from the game state; unrecognized items are ignored rather than causing errors.

### Out of Scope

- **Skill progression tracking**: The planner does not track or display skill levels, XP progress, or level-up milestones (e.g., fishing level, farming level).
- **Quest tracking**: The planner does not track active quests, quest objectives, or quest completion status. Players should use the game's built-in quest log for this.
- **Relationship tracking beyond birthdays**: The planner shows birthday reminders and gift suggestions but does not track overall friendship levels, dialogue progress, or dating/marriage milestones.
- **Farm optimization advice**: The planner does not recommend crop planting schedules, farm layout, or profit calculations.
- **Multiplayer coordination**: The planner is designed for single-player use and does not sync or share data between players in multiplayer sessions.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The mod MUST read the current in-game date, season, weather, and time of day from the player's active save.
- **FR-002**: The mod MUST display a list of all fish catchable on the current day based on season, weather, time of day, and location.
- **FR-003**: The mod MUST display a list of all forageable items available in the current season, grouped by location.
- **FR-004**: The mod MUST indicate collection status contextually: for items needed by bundles, gifts, or other achievements, the mod checks whether the item is currently in the player's inventory or storage; for general completionist tracking, the mod uses the game's collection tab data (fish caught, items foraged).
- **FR-005**: The mod MUST read the player's Community Center bundle completion state from the active save.
- **FR-006**: The mod MUST highlight fish and forageable items that are still needed for incomplete Community Center bundles, showing the associated bundle name.
- **FR-007**: The mod MUST track the player's shipping collection and indicate items that have never been shipped.
- **FR-008**: The mod MUST track the player's museum donation progress and indicate artifacts and minerals that have not yet been donated.
- **FR-009**: The mod MUST track the player's cooking recipe collection and indicate known but never cooked recipes, highlighting when the player currently has the required ingredients to cook them.
- **FR-010**: The mod MUST track the player's crafting recipe collection and indicate known but never crafted recipes, highlighting when the player currently has the required materials to craft them.
- **FR-011**: The mod MUST display NPC birthdays occurring today, with a list of their loved and liked gift items.
- **FR-012**: The mod MUST display upcoming NPC birthdays within a 7-day lookahead window, with their loved and liked gift items.
- **FR-013**: The mod MUST show a "Tomorrow" preview section that includes the weather forecast, any new collectibles becoming available, and relevant events or festivals.
- **FR-014**: The mod MUST display a "Last Chance" alert during the final 3 days of a season for any uncollected season-exclusive items.
- **FR-015**: The mod MUST provide an in-game panel accessible via a configurable hotkey.
- **FR-016**: The panel MUST open and close without disrupting active gameplay or conflicting with other game menus.
- **FR-017**: The panel MUST default to a priority view showing only bundle-needed items, time-sensitive collectibles, today's/upcoming birthdays, and last-chance alerts; a "Show All" toggle MUST expand the view to display all trackable collection items with completion status. The panel MUST remember the player's last view state (priority vs. Show All) within the same play session, and MUST reset to the priority view when the game is restarted.
- **FR-018**: The mod MUST not open the panel during cutscenes, events, or when other menus are active.
- **FR-019**: The mod MUST gracefully handle save files where the Joja route has been chosen (hiding or adapting bundle-related information).
- **FR-020**: The mod MUST work correctly when installed on an existing mid-playthrough save, reading the current game state accurately.
- **FR-021**: The mod MUST update its displayed information when the in-game day changes (new day, season transition, weather change).

### Key Entities

- **Daily Summary**: Represents the compiled planner data for a single in-game day — includes date, season, weather, available fish, available forageables, birthday NPCs, upcoming events, and last-chance alerts.
- **Collectible Item**: Any trackable item across all collection types (fish, forageable, artifact, mineral, shippable item, cooking recipe, crafting recipe) with attributes including name, collection type, availability conditions (season, weather, time, location, ingredients/materials), collection status (contextual: "in inventory/storage" for actionable needs like bundles or gifts; "ever collected/shipped/donated/cooked/crafted" via the relevant collection tab for completionist tracking), and bundle associations where applicable.
- **Bundle**: A Community Center bundle with its name, required items, and completion status per item slot.
- **NPC Birthday**: An NPC's name, birthday date, and their loved and liked gift item lists.
- **Tomorrow Preview**: A forward-looking summary containing the weather forecast, newly available collectibles, and scheduled events/festivals for the following day.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Players can identify all available collectibles for the current day within 10 seconds of opening the planner.
- **SC-002**: Players can identify which of today's collectibles are needed for Community Center bundles within 5 seconds of opening the planner.
- **SC-003**: Players are alerted to NPC birthdays at least 7 in-game days in advance, with gift suggestions immediately visible.
- **SC-004**: Players receive last-chance season alerts with at least 3 in-game days of lead time, giving them sufficient opportunity to act.
- **SC-005**: The planner panel opens and closes in under 1 second, with no visible impact on game performance.
- **SC-006**: 100% of displayed collectible data matches the actual in-game availability and completion status for all tracked collection types (fish, forageables, shipping, museum, cooking, crafting).
- **SC-007**: The mod correctly reflects the player's actual collection and bundle completion state with no stale or incorrect data after day transitions.
- **SC-008**: Players who use the mod complete Community Center bundles with fewer missed seasonal items compared to playing without the mod (qualitative goal — reduced FOMO).

## Clarifications

### Session 2026-02-15

- Q: What defines an item as "collected" for the planner's check-off purposes? → A: Context-dependent — for bundle/gift/achievement needs, the item must be in the player's current inventory or storage; for general completionist tracking, the game's collection tab data is used.
- Q: What scope of "achievements" should the planner track beyond bundles? → A: All trackable collections — shipping, museum donations (artifacts & minerals), cooking recipes, and crafting recipes, in addition to fish and forageables.
- Q: How should the planner panel organize the expanded content? → A: Priority-based single view — default view shows only bundle-needed and time-sensitive items; a "Show All" toggle reveals complete collection tracking.
- Q: Should the planner track skill-based milestones or quest progress? → A: Out of scope — the planner tracks only collectibles, bundles, birthdays, and time-sensitive events. Skills and quests are excluded.
- Q: Should the planner remember the player's last view state (priority vs. Show All) between opens? → A: Remember within the same play session; reset to priority view on game restart.

## Assumptions

- The mod targets Stardew Valley version 1.6+ with SMAPI as the modding framework (industry standard for Stardew Valley mods).
- Fish availability data (season, weather, time, location conditions) is sourced from the game's internal data files rather than hardcoded, ensuring compatibility with game updates.
- The 7-day birthday lookahead window is a reasonable default that covers roughly one week of in-game time, giving players enough time to find or craft gifts.
- The "Last Chance" alert window of 3 days before season end balances urgency with actionability.
- The hotkey for opening the planner defaults to a common unused key but is configurable through the mod's settings.
- The mod does not modify any game state — it is purely a read-only information overlay.
- The panel UI follows Stardew Valley's visual style to feel native to the game.
- Performance impact is negligible since the mod only reads game data and renders a UI overlay.
