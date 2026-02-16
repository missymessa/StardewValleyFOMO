# Feature Specification: Community Center Bundle Tracker

**Feature Branch**: `002-bundle-tracker`  
**Created**: February 16, 2026  
**Status**: Draft  
**Input**: User description: "Add functionality to track community center bundles"

## User Scenarios & Testing *(mandatory)*

### User Story 1 — View All Bundle Progress (Priority: P1)

As a player, I want to see a comprehensive overview of all Community Center bundles with their completion status so I can understand my overall progress and prioritize which bundles to focus on.

**Why this priority**: The core value of bundle tracking is knowing what remains to be done. Without this overview, players must visit the Community Center in-game to check progress, breaking their workflow.

**Independent Test**: Can be tested by opening the Bundles tab and verifying all rooms and bundles are listed with accurate completion percentages based on the current save file.

**Acceptance Scenarios**:

1. **Given** a player has partially completed bundles, **When** they open the Bundles tab, **Then** each room (Crafts Room, Pantry, Fish Tank, Boiler Room, Bulletin Board, Vault) is displayed with completion percentage.
2. **Given** a room has multiple bundles, **When** viewing that room, **Then** each bundle within shows items needed vs items delivered.
3. **Given** a bundle is fully completed, **When** viewing that bundle, **Then** it displays a "Complete" indicator and is visually distinct from incomplete bundles.
4. **Given** the Community Center is fully restored, **When** opening the Bundles tab, **Then** a congratulatory message is shown indicating completion.

---

### User Story 2 — See Items Needed Per Bundle (Priority: P1)

As a player, I want to see exactly which items are still needed for each incomplete bundle, including item names and quantities, so I know what to collect.

**Why this priority**: Knowing which specific items to gather is essential for actionable planning. This transforms the tracker from informational to tactical.

**Independent Test**: Can be tested by expanding any incomplete bundle and verifying the listed items match the actual remaining requirements from the save file.

**Acceptance Scenarios**:

1. **Given** a bundle requires 3 of 5 item slots to be filled, **When** viewing bundle details, **Then** only the 2 remaining unfilled slots are shown as "needed."
2. **Given** a bundle item has a quality requirement (e.g., Gold-star Parsnip), **When** viewing that item requirement, **Then** the quality requirement is clearly displayed.
3. **Given** a bundle accepts multiple item choices for a slot (e.g., any Spring Crop), **When** viewing that slot, **Then** all valid item options are listed.

---

### User Story 3 — Highlight Owned Items for Bundles (Priority: P1)

As a player, I want the tracker to highlight which bundle items I currently have in my inventory or storage so I can immediately deliver them.

**Why this priority**: Players often forget they already have needed items. This eliminates the frustration of hunting for items they already possess.

**Independent Test**: Can be tested by placing a bundle-required item in inventory, opening the Bundles tab, and verifying that item shows a "You have this" indicator.

**Acceptance Scenarios**:

1. **Given** a player has a bundle-needed item in their inventory, **When** viewing that bundle, **Then** the item is marked with a "★ [HAVE]" indicator.
2. **Given** a player has a needed item in a chest on their farm, **When** viewing the bundle, **Then** the item is marked as available (with location hint if possible).
3. **Given** a quality requirement exists and the player has the item but wrong quality, **When** viewing the bundle, **Then** the item is NOT marked as available.
4. **Given** multiple items are needed and player has some, **When** viewing the bundle, **Then** a count shows "2 of 4 items ready to deliver."

---

### User Story 4 — Filter Bundles by Availability (Priority: P2)

As a player, I want to filter bundles to show only those with items available today (based on current season, weather, and location accessibility) so I can focus on actionable tasks.

**Why this priority**: Not all bundles are achievable on any given day. Filtering reduces cognitive load and focuses player effort on what's possible now.

**Independent Test**: Can be tested by enabling the "Available Today" filter and verifying only bundles with currently-obtainable items are displayed.

**Acceptance Scenarios**:

1. **Given** it is Spring and raining, **When** filtering by "Available Today," **Then** bundles requiring rain-exclusive Spring fish are shown as actionable.
2. **Given** a bundle requires a Winter-only item and it is Summer, **When** filtering by "Available Today," **Then** that bundle is hidden or marked as "Not available this season."
3. **Given** no bundles have items available today, **When** applying the filter, **Then** a helpful message indicates "No bundle items available today" with a suggestion.

---

### User Story 5 — Bundle Notifications (Priority: P2)

As a player, I want to receive a notification when I pick up or obtain an item that completes a bundle slot so I remember to deliver it.

**Why this priority**: Players often collect items without realizing they've obtained something bundle-worthy. Notifications prevent missed opportunities.

**Independent Test**: Can be tested by picking up a bundle-required item and verifying a notification appears indicating the bundle and item.

**Acceptance Scenarios**:

1. **Given** a player catches a fish needed for a bundle, **When** the fish enters inventory, **Then** a subtle notification appears: "Bundle Item: [Fish Name] for [Bundle Name]."
2. **Given** the player already has the item in inventory, **When** picking up another copy, **Then** no duplicate notification is shown.
3. **Given** the player has disabled notifications in config, **When** picking up a bundle item, **Then** no notification appears.

---

### User Story 6 — Integration with Joja Mart Route (Priority: P3)

As a player who chose the Joja Mart route, I want the Bundles tab to gracefully handle this scenario instead of showing incomplete data.

**Why this priority**: Completeness and polish. Players shouldn't see confusing data for an inapplicable feature.

**Independent Test**: Can be tested by loading a save where Joja Mart was purchased and verifying the Bundles tab shows appropriate messaging.

**Acceptance Scenarios**:

1. **Given** a player has purchased a Joja membership, **When** opening the Bundles tab, **Then** a message indicates "You chose the Joja Mart route. Community Center bundles are not available."
2. **Given** a player is on the Community Center route, **When** opening the Bundles tab, **Then** full bundle tracking is shown.

---

### Edge Cases

- **OR requirements**: Handled via BundleSlot model; UI displays all valid options; ownership check passes if player has ANY valid item.
- **Fish location requirements**: Displayed as informational hints alongside item name; not enforced by tracker.
- **Unlocked room status**: All bundles shown regardless of unlock status (per Clarification session 2026-02-16).
- **Seasonal items outside season**: Displayed with "Not available this season" indicator when filter is active; always visible otherwise.
- **Early game (unrevealed bundles)**: All bundles shown regardless of Junimo reveal status (per Clarification session 2026-02-16).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST display all 6 Community Center rooms with their completion status.
- **FR-002**: System MUST display all bundles within each room with item counts (X/Y items delivered).
- **FR-003**: System MUST list each unfilled bundle slot with exact item requirements (name, quantity, quality if applicable).
- **FR-004**: System MUST indicate when a required item is present in player's inventory.
- **FR-005**: System MUST indicate when a required item is present in player's storage chests.
- **FR-006**: System MUST calculate and display percentage completion for each room and overall.
- **FR-007**: System MUST support filtering bundles by "items available today" based on season, weather, time-of-day, and location unlock status.
- **FR-008**: System SHOULD display a notification when a bundle-required item is collected.
- **FR-009**: System MUST handle save files where Joja Mart route was chosen, displaying appropriate messaging.
- **FR-010**: System MUST respect bundles that have OR requirements (multiple valid items for one slot).
- **FR-011**: System MUST update bundle status when the day changes or when returning from the Community Center.

### Key Entities

- **Room**: A Community Center room (Crafts Room, Pantry, Fish Tank, Boiler Room, Bulletin Board, Vault). Contains multiple bundles.
- **Bundle**: A collection of item slots within a room. Has a name, reward, and list of required items.
- **BundleSlot**: A single requirement within a bundle. May accept one specific item OR multiple valid items. Tracks whether the slot has been filled.
- **BundleItem**: An item that satisfies a bundle slot. Has item ID, required quantity, and optional quality requirement.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Players can view all bundle progress without visiting the Community Center in-game.
- **SC-002**: Players can identify which items they already have for bundles within 5 seconds of opening the Bundles tab.
- **SC-003**: Bundle completion percentage is accurate to 100% compared to in-game Community Center state.
- **SC-004**: "Available Today" filter correctly shows only bundles with currently-obtainable items based on season and weather.
- **SC-005**: Players who enable notifications are alerted when collecting bundle items 100% of the time.
- **SC-006**: Joja Mart route players see clear messaging instead of confusing empty data.

## Clarifications

### Session 2026-02-16

- Q: Should the bundle tracker support remixed bundles (SDV 1.5+ option)? → A: Support both standard and remixed bundles.
- Q: What locations should be scanned for bundle items? → A: Farm buildings + farmhouse (balanced coverage).
- Q: Should tracker show all bundles or only discovered ones? → A: Show all bundles regardless of unlock status.
- Q: How should storage scanning be cached? → A: Cache on day start + inventory changes (balanced).
- Q: What factors determine "Available Today" filter? → A: Full availability including season, weather, time-of-day, and location unlock status.

## Assumptions

- Players understand the Community Center and Joja Mart distinction.
- The mod has access to bundle data via Stardew Valley's save file and content data.
- System supports both standard and remixed Community Center bundles (reading actual bundle data from save file rather than hardcoded values).
- Storage scanning includes player inventory, farmhouse, and all farm buildings (sheds, barns, coops, cabins).
