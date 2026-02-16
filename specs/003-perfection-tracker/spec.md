# Feature Specification: Perfection Tracker

**Feature Branch**: `003-perfection-tracker`  
**Created**: February 16, 2026  
**Status**: Draft  
**Input**: User description: "Add a new tab for tracking Perfection"

## User Scenarios & Testing *(mandatory)*

### User Story 1 — View Overall Perfection Progress (Priority: P1)

As a player pursuing 100% completion, I want to see my overall perfection percentage and a breakdown of all perfection categories so I can track my progress toward the statue of perfection.

**Why this priority**: The core value of perfection tracking is knowing the overall percentage and which categories need work. Without this, players must mentally track multiple game systems.

**Independent Test**: Can be tested by opening the Perfection tab and verifying the overall percentage matches the in-game perfection tracker, with all categories listed showing accurate progress.

**Acceptance Scenarios**:

1. **Given** a player has partial progress in various perfection categories, **When** they open the Perfection tab, **Then** the overall perfection percentage is displayed prominently along with all category breakdowns.
2. **Given** a category is fully completed, **When** viewing that category, **Then** it displays a "Complete" indicator and is visually distinct from incomplete categories.
3. **Given** the player has achieved 100% perfection, **When** opening the Perfection tab, **Then** a congratulatory message is shown celebrating the achievement.
4. **Given** the player has not yet unlocked Ginger Island, **When** viewing perfection progress, **Then** categories requiring Ginger Island access are shown with an appropriate indicator.

---

### User Story 2 — Track Shipping Completion (Priority: P1)

As a player, I want to see which items I still need to ship to complete the shipping collection so I can systematically work toward 100% shipping.

**Why this priority**: Shipping collection is typically the largest perfection category requiring 145+ unique items shipped. Knowing what's missing is essential.

**Independent Test**: Can be tested by viewing the Shipping section and verifying the count of shipped vs total items, with remaining items listed.

**Acceptance Scenarios**:

1. **Given** a player has shipped some items, **When** viewing Shipping progress, **Then** the percentage complete and count (e.g., "120/145 items") is displayed.
2. **Given** items are grouped by source (crops, artisan goods, forage, etc.), **When** expanding a category, **Then** unshipped items in that category are listed.
3. **Given** a player has an unshipped item in inventory or storage, **When** viewing that item in the list, **Then** it shows a "★ [HAVE]" indicator.

---

### User Story 3 — Track Fish Collection (Priority: P1)

As a player, I want to see which fish I still need to catch so I can plan fishing trips by season and location.

**Why this priority**: Fish are time-gated by season, weather, and time of day. Knowing exactly what's missing helps players plan efficiently.

**Independent Test**: Can be tested by viewing the Fish section and verifying caught/total counts with remaining fish listed alongside their catch conditions.

**Acceptance Scenarios**:

1. **Given** a player has caught some fish, **When** viewing Fish progress, **Then** the percentage complete and count (e.g., "50/67 fish") is displayed.
2. **Given** a fish has not been caught, **When** viewing that fish, **Then** the season, weather, time, and location requirements are displayed.
3. **Given** a fish is catchable today (correct season, weather, time), **When** viewing Fish progress, **Then** that fish is highlighted as "Available Today."
4. **Given** a fish requires specific weather and it is currently that weather, **When** viewing Fish progress, **Then** the fish shows urgency indicator.

---

### User Story 4 — Track Cooking Recipes (Priority: P1)

As a player, I want to see which cooking recipes I still need to cook so I can work toward cooking mastery.

**Why this priority**: Cooking completion requires both learning recipes and cooking them once. Players need to track both aspects.

**Independent Test**: Can be tested by viewing the Cooking section and verifying cooked/total counts with remaining recipes and their ingredient requirements listed.

**Acceptance Scenarios**:

1. **Given** a player has cooked some recipes, **When** viewing Cooking progress, **Then** the percentage complete and count (e.g., "60/80 recipes") is displayed.
2. **Given** a recipe has been learned but not cooked, **When** viewing that recipe, **Then** it shows as "Not Yet Cooked" with required ingredients.
3. **Given** a recipe has not been learned, **When** viewing that recipe, **Then** it shows how to obtain the recipe (TV show, friendship, purchase, etc.).
4. **Given** a player has all ingredients for an uncooked recipe in inventory/storage, **When** viewing that recipe, **Then** it shows "Ready to Cook" indicator.

---

### User Story 5 — Track Crafting Recipes (Priority: P1)

As a player, I want to see which crafting recipes I still need to craft so I can work toward crafting mastery.

**Why this priority**: Similar to cooking, crafting completion requires both learning and making each item once.

**Independent Test**: Can be tested by viewing the Crafting section and verifying crafted/total counts with remaining recipes and their requirements listed.

**Acceptance Scenarios**:

1. **Given** a player has crafted some recipes, **When** viewing Crafting progress, **Then** the percentage complete and count (e.g., "100/129 recipes") is displayed.
2. **Given** a recipe has been learned but not crafted, **When** viewing that recipe, **Then** it shows as "Not Yet Crafted" with required materials.
3. **Given** a recipe has not been learned, **When** viewing that recipe, **Then** it shows how to obtain the recipe (skill level, quest, purchase, etc.).
4. **Given** a player has all materials for an uncrafted recipe, **When** viewing that recipe, **Then** it shows "Ready to Craft" indicator.

---

### User Story 6 — Track Friendship Progress (Priority: P1)

As a player, I want to see my friendship levels with all NPCs and which ones need more hearts so I can prioritize gift-giving.

**Why this priority**: Perfection requires maximum friendship with all villagers. Tracking hearts per NPC helps players prioritize relationships.

**Independent Test**: Can be tested by viewing the Friendship section and verifying heart counts per NPC match in-game values.

**Acceptance Scenarios**:

1. **Given** a player has varying friendship levels, **When** viewing Friendship progress, **Then** each NPC is listed with current hearts and maximum possible hearts.
2. **Given** an NPC needs more hearts, **When** viewing that NPC, **Then** they are sorted toward the top of the list.
3. **Given** an NPC is at maximum friendship, **When** viewing that NPC, **Then** they display a "Max" indicator.
4. **Given** an NPC relationship is blocked (e.g., dating required), **When** viewing that NPC, **Then** the blocker is indicated.

---

### User Story 7 — Track Farm Buildings Progress (Priority: P2)

As a player, I want to see which Obelisks and the Golden Clock I still need to build on my farm for perfection.

**Why this priority**: These are expensive late-game buildings that represent significant gold investment. Knowing what's missing helps with financial planning.

**Independent Test**: Can be tested by viewing the Farm Buildings section and verifying which obelisks/clock are present on the farm.

**Acceptance Scenarios**:

1. **Given** a player has built some obelisks, **When** viewing Farm Buildings progress, **Then** each obelisk type shows "Built" or "Not Built" status.
2. **Given** a player has not built the Golden Clock, **When** viewing Farm Buildings, **Then** the Golden Clock shows as needed with its cost.
3. **Given** all 4 obelisks and Golden Clock are built, **When** viewing Farm Buildings, **Then** the section shows as "Complete."

---

### User Story 8 — Track Monster Slayer Goals (Priority: P2)

As a player, I want to see my progress toward the Monster Slayer Hero goals so I can prioritize which monsters to hunt.

**Why this priority**: Monster eradication goals require killing specific numbers of various monster types. Tracking progress is essential for planning mine runs.

**Independent Test**: Can be tested by viewing the Monster Slayer section and verifying kill counts match the Adventurer's Guild board.

**Acceptance Scenarios**:

1. **Given** a player has killed some monsters, **When** viewing Monster Slayer progress, **Then** each monster category shows current kills vs required kills.
2. **Given** a monster goal is incomplete, **When** viewing that goal, **Then** floor ranges or locations where those monsters spawn are indicated.
3. **Given** all monster goals are complete, **When** viewing Monster Slayer, **Then** the section shows as "Complete" with hero status.

---

### User Story 9 — Track Stardrops Collected (Priority: P2)

As a player, I want to see which Stardrops I've collected and which ones I still need to find.

**Why this priority**: Stardrops are unique collectibles that permanently increase energy. Each has a specific acquisition method.

**Independent Test**: Can be tested by viewing the Stardrops section and verifying collected count matches player's energy level progression.

**Acceptance Scenarios**:

1. **Given** a player has collected some Stardrops, **When** viewing Stardrop progress, **Then** the count (e.g., "5/7 Stardrops") is displayed.
2. **Given** a Stardrop has not been collected, **When** viewing that Stardrop, **Then** a hint about how to obtain it is shown.
3. **Given** all Stardrops are collected, **When** viewing Stardrops, **Then** the section shows as "Complete."

---

### User Story 10 — Track Golden Walnuts (Priority: P2)

As a player who has unlocked Ginger Island, I want to see my Golden Walnut collection progress so I can find the remaining walnuts.

**Why this priority**: 130 Golden Walnuts are scattered across Ginger Island through various activities. Tracking what's found helps locate remaining ones.

**Independent Test**: Can be tested by viewing the Golden Walnuts section and verifying collected count matches in-game walnut count.

**Acceptance Scenarios**:

1. **Given** a player has collected some walnuts, **When** viewing Golden Walnut progress, **Then** the count (e.g., "100/130 walnuts") is displayed.
2. **Given** walnuts are categorized by acquisition method (digging, puzzles, etc.), **When** viewing a category, **Then** remaining walnut opportunities are listed.
3. **Given** Ginger Island is not unlocked, **When** viewing Golden Walnuts, **Then** a message indicates island access is required.

---

### User Story 11 — Track Skill Levels (Priority: P3)

As a player, I want to see which skills are not yet at maximum level so I can focus on leveling them up.

**Why this priority**: All skills at level 10 contributes to perfection. This is typically achieved naturally but worth tracking.

**Independent Test**: Can be tested by viewing the Skills section and verifying skill levels match character screen.

**Acceptance Scenarios**:

1. **Given** a player has varying skill levels, **When** viewing Skills progress, **Then** each skill shows current level out of 10.
2. **Given** a skill is below level 10, **When** viewing that skill, **Then** it indicates XP needed to level up.
3. **Given** all skills are level 10, **When** viewing Skills, **Then** the section shows as "Complete."

---

### Edge Cases

- What happens when the player is on a beach farm (no standard mine access)? → Monster eradication goals still trackable via Skull Cavern and Volcano Dungeon
- ~~How does the tracker handle modded content that adds new fish/items/recipes?~~ → **Resolved: Vanilla only**
- ~~What happens if a multiplayer farm has different players with different progress?~~ → **Resolved: Current player only**
- How does the tracker handle legacy saves that pre-date Ginger Island? → Ginger Island categories show "Not Yet Unlocked" indicator

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST display overall perfection percentage matching the in-game tracker
- **FR-002**: System MUST show individual category progress (Shipping, Fish, Cooking, Crafting, Friendship, Monsters, Stardrops, Walnuts, Skills, Buildings)
- **FR-003**: System MUST list missing items/fish/recipes within each category, grouped into logical subcategories with expand/collapse UI
- **FR-004**: System MUST indicate which missing items the player currently owns in inventory or storage
- **FR-005**: System MUST display acquisition hints for missing collectibles (fish seasons/locations, recipe sources, etc.)
- **FR-006**: System MUST highlight items that are obtainable "today" based on current season, weather, and time
- **FR-007**: System MUST visually distinguish completed categories from incomplete ones
- **FR-008**: System MUST gracefully handle saves where Ginger Island is not yet unlocked
- **FR-009**: System MUST be accessible via a new "Perfection" tab in the mod's UI
- **FR-010**: System MUST update progress in real-time as the player achieves goals during gameplay
- **FR-011**: System MUST track only vanilla Stardew Valley 1.5+ perfection items (modded content ignored)
- **FR-012**: System MUST track only the current logged-in player's progress in multiplayer
- **FR-013**: System MUST display Golden Walnut progress grouped by acquisition type (digging, puzzles, fishing, etc.)

### Key Entities

- **PerfectionCategory**: Represents a perfection tracking category (Shipping, Fish, Cooking, etc.) with completion percentage and list of items
- **PerfectionItem**: Individual trackable item within a category (specific fish, recipe, NPC, etc.) with completion status and acquisition info
- **PerfectionProgress**: Overall perfection state containing all categories and computed total percentage

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Players can view their complete perfection status without visiting multiple in-game menus
- **SC-002**: All perfection categories display accurate progress matching the in-game tracker
- **SC-003**: Players can identify actionable items (fish catchable today, recipes ready to cook) within 10 seconds of opening the tab
- **SC-004**: Missing items clearly indicate how to obtain them
- **SC-005**: Players with owned but undelivered/uncrafted items can identify them via "★ [HAVE]" indicators
- **SC-006**: Tab loads and displays full perfection data within 1 second of opening

## Assumptions

- Perfection tracking follows vanilla Stardew Valley 1.5+ perfection system (130 walnuts, standard fish/recipe counts)
- The mod already has infrastructure for tabs (existing Bundles tab) that can be extended
- Storage scanning capabilities exist from the bundle tracker feature
- Fish season/weather/time data is accessible through game APIs
- Recipe acquisition sources can be determined programmatically or via static data
- **Modded content is ignored** - tracker only counts vanilla game items/fish/recipes; modded additions do not affect perfection percentages

## Clarifications

### Session 2026-02-16

- Q: How should the tracker handle mods that add new fish/items/recipes? → A: Vanilla only - track only base game perfection items, ignore modded content
- Q: How should multiplayer farms handle different players with different progress? → A: Current player only - show only the logged-in player's perfection progress
- Q: How should large categories (145+ items) be organized? → A: Subcategory groups - group items into logical subcategories with expand/collapse UI
- Q: How detailed should Golden Walnut tracking be? → A: Group by type - show counts per acquisition method (digging, puzzles, fishing, etc.)
