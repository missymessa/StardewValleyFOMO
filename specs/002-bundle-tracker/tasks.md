# Tasks: Community Center Bundle Tracker

**Input**: Design documents from `/specs/002-bundle-tracker/`
**Prerequisites**: plan.md (required), spec.md (required for user stories)

**Tests**: Tests included for all new services following TDD approach.

**Organization**: Tasks grouped by user story to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: New models and interfaces needed across user stories

- [X] T001 [P] Create `RoomProgress` model in `src/StardewFOMO.Core/Models/RoomProgress.cs` with RoomName, TotalBundles, CompletedBundles, PercentComplete properties
- [X] T002 [P] Create `BundleSlot` model in `src/StardewFOMO.Core/Models/BundleSlot.cs` with SlotIndex, ValidItems (list of BundleItem), IsFilled, FilledWithItemId properties
- [X] T003 [P] Create `OwnedItemInfo` model in `src/StardewFOMO.Core/Models/OwnedItemInfo.cs` with ItemId, Quantity, Quality, Location (enum: Inventory, Farmhouse, BuildingName) properties
- [X] T004 [P] Create `ItemAvailability` model in `src/StardewFOMO.Core/Models/ItemAvailability.cs` with ItemId, IsAvailableToday, Reason (enum: Available, WrongSeason, WrongWeather, LocationLocked, WrongTimeOfDay)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core interfaces and adapters that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T005 Extend `IBundleRepository` in `src/StardewFOMO.Core/Interfaces/IBundleRepository.cs` — add GetAllRooms(), GetRoomProgress(roomName), GetBundlesByRoom(roomName), IsCommunityComplete() methods
- [X] T006 [P] Create `IStorageScanner` interface in `src/StardewFOMO.Core/Interfaces/IStorageScanner.cs` with GetOwnedItems(), InvalidateCache(), HasItem(itemId, minQuality) methods
- [X] T007 [P] Create `IItemAvailabilityService` interface in `src/StardewFOMO.Core/Interfaces/IItemAvailabilityService.cs` with GetAvailability(itemId), IsAvailableToday(itemId) methods
- [X] T008 Update `BundleInfo` model in `src/StardewFOMO.Core/Models/BundleInfo.cs` — add Slots property (IReadOnlyList<BundleSlot>) for OR requirement support
- [X] T009 Extend `BundleAdapter` in `src/StardewFOMO.Mod/Adapters/BundleAdapter.cs` — implement GetAllRooms(), GetRoomProgress(), GetBundlesByRoom(), IsCommunityComplete(); support remixed bundles via Game1.netWorldState.Value.BundleData
- [X] T010 [P] Create `StorageScannerAdapter` in `src/StardewFOMO.Mod/Adapters/StorageScannerAdapter.cs` implementing IStorageScanner — scan farmhouse + all farm buildings (Shed, Barn, Coop, Cabin); cache results
- [X] T011 [P] Create `FakeStorageScanner` in `tests/StardewFOMO.Core.Tests/Fakes/FakeStorageScanner.cs` implementing IStorageScanner with in-memory item data
- [X] T012 [P] Extend `FakeBundleRepository` in `tests/StardewFOMO.Core.Tests/Fakes/FakeBundleRepository.cs` — add GetAllRooms(), GetRoomProgress(), GetBundlesByRoom(), IsCommunityComplete() methods

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 — View All Bundle Progress (Priority: P1) 🎯 MVP

**Goal**: Display all 6 CC rooms with completion percentages; show bundles within each room with X/Y item counts

**Independent Test**: Open Bundles tab, verify all rooms listed with accurate percentages matching save file

### Tests for User Story 1

- [X] T013 [P] [US1] Create `BundleProgressServiceTests` in `tests/StardewFOMO.Core.Tests/Services/BundleProgressServiceTests.cs` — test room percentage calculation, bundle item counts, completion detection

### Implementation for User Story 1

- [X] T014 [P] [US1] Create `BundleProgressService` in `src/StardewFOMO.Core/Services/BundleProgressService.cs` — accepts IBundleRepository, ILogger; calculates room progress, overall CC completion percentage
- [X] T015 [US1] Enhance `DrawBundlesTab` in `src/StardewFOMO.Mod/UI/PlannerOverlay.cs` — display rooms with progress bars, bundles with X/Y counts, "Complete" indicators, CC restoration congratulations message
- [X] T016 [US1] Wire `BundleProgressService` in `src/StardewFOMO.Mod/ModEntry.cs` — inject into overlay

**Checkpoint**: User Story 1 complete — rooms and bundles display with accurate progress

---

## Phase 4: User Story 2 — See Items Needed Per Bundle (Priority: P1)

**Goal**: Show exact items needed per incomplete bundle including quality requirements and OR options

**Independent Test**: Expand any incomplete bundle, verify listed items match actual remaining requirements

### Tests for User Story 2

- [X] T017 [P] [US2] Add tests to `BundleProgressServiceTests` for GetRemainingItems with quality requirements and OR requirements (multiple valid items per slot)

### Implementation for User Story 2

- [X] T018 [US2] Extend `BundleProgressService` in `src/StardewFOMO.Core/Services/BundleProgressService.cs` — add GetBundleDetails(bundleName) returning remaining slots with all valid item options and quality requirements
- [X] T019 [US2] Enhance `DrawBundlesTab` in `src/StardewFOMO.Mod/UI/PlannerOverlay.cs` — show remaining items per bundle with quality stars (Silver ☆, Gold ★, Iridium ◆); display OR options as "Item A or Item B"

**Checkpoint**: User Story 2 complete — item requirements with quality and OR options visible

---

## Phase 5: User Story 3 — Highlight Owned Items for Bundles (Priority: P1)

**Goal**: Mark items player owns in inventory/storage with "[HAVE]" indicator; show "X of Y ready"

**Independent Test**: Place bundle-required item in inventory, open Bundles tab, verify "[HAVE]" indicator appears

### Tests for User Story 3

- [X] T020 [P] [US3] Create `BundleItemMatcherServiceTests` in `tests/StardewFOMO.Core.Tests/Services/BundleItemMatcherServiceTests.cs` — test item matching with quality requirements, OR requirements, inventory vs storage detection

### Implementation for User Story 3

- [X] T021 [P] [US3] Create `BundleItemMatcherService` in `src/StardewFOMO.Core/Services/BundleItemMatcherService.cs` — accepts IStorageScanner, IBundleRepository, ILogger; matches owned items to bundle requirements considering quality; returns match status per slot
- [X] T022 [US3] Enhance `DrawBundlesTab` in `src/StardewFOMO.Mod/UI/PlannerOverlay.cs` — add "★ [HAVE]" indicator next to owned items; show "X of Y items ready to deliver" count per bundle; location hint (inventory/chest)
- [X] T023 [US3] Wire storage cache invalidation in `src/StardewFOMO.Mod/ModEntry.cs` — call IStorageScanner.InvalidateCache() on DayStarted, InventoryChanged, and Warped (when leaving Community Center) events

**Checkpoint**: User Story 3 complete — owned items highlighted with delivery-ready counts

---

## Phase 6: User Story 4 — Filter Bundles by Availability (Priority: P2)

**Goal**: "Available Today" filter shows only bundles with items obtainable based on season/weather/time/location

**Independent Test**: Enable filter in Spring rain, verify rain-fish bundles shown; Winter items hidden

### Tests for User Story 4

- [X] T024 [P] [US4] Create `FakeItemAvailabilityService` in `tests/StardewFOMO.Core.Tests/Fakes/FakeItemAvailabilityService.cs` implementing IItemAvailabilityService
- [X] T025 [P] [US4] Create `BundleAvailabilityServiceTests` in `tests/StardewFOMO.Core.Tests/Services/BundleAvailabilityServiceTests.cs` — test seasonal filtering, weather filtering, time-of-day filtering, location unlock filtering

### Implementation for User Story 4

- [X] T026 [P] [US4] Create `ItemAvailabilityAdapter` in `src/StardewFOMO.Mod/Adapters/ItemAvailabilityAdapter.cs` implementing IItemAvailabilityService — check item data for season, weather, time, location requirements against current game state
- [X] T027 [US4] Create `BundleAvailabilityService` in `src/StardewFOMO.Core/Services/BundleAvailabilityService.cs` — accepts IBundleRepository, IItemAvailabilityService, ILogger; filters bundles to those with at least one obtainable item today; provides availability reason per item
- [X] T028 [US4] Enhance `DrawBundlesTab` in `src/StardewFOMO.Mod/UI/PlannerOverlay.cs` — add "Available Today" toggle button; filter displayed bundles; show "Not available this season" indicators on unavailable items
- [X] T029 [US4] Add `AvailabilityFilterDefault` config option in `src/StardewFOMO.Mod/ModConfig.cs` and GMCM registration in `ModEntry.cs`

**Checkpoint**: User Story 4 complete — availability filter works with season/weather/time/location logic

---

## Phase 7: User Story 5 — Bundle Notifications (Priority: P2)

**Goal**: Show notification when player picks up bundle-required item; prevent duplicate notifications

**Independent Test**: Catch bundle-needed fish, verify notification appears showing item and bundle name

### Tests for User Story 5

- [X] T030 [P] [US5] Create `BundleNotificationServiceTests` in `tests/StardewFOMO.Core.Tests/Services/BundleNotificationServiceTests.cs` — test notification generation, duplicate suppression, config disable

### Implementation for User Story 5

- [X] T031 [P] [US5] Create `BundleNotificationService` in `src/StardewFOMO.Core/Services/BundleNotificationService.cs` — accepts IBundleRepository, ILogger; tracks notified item-bundle pairs per session; generates notification text for new bundle item pickups
- [X] T032 [US5] Subscribe to `InventoryChanged` event in `src/StardewFOMO.Mod/ModEntry.cs` — check added items against BundleNotificationService; display HUD message via Game1.addHUDMessage()
- [X] T033 [US5] Add `EnableBundleNotifications` config option in `src/StardewFOMO.Mod/ModConfig.cs` and GMCM registration in `ModEntry.cs`

**Checkpoint**: User Story 5 complete — bundle item pickup notifications working with config option

---

## Phase 8: User Story 6 — Joja Mart Route Handling (Priority: P3)

**Goal**: Graceful messaging when player chose Joja route instead of CC bundles

**Independent Test**: Load Joja-route save, open Bundles tab, verify appropriate message displays

### Implementation for User Story 6

- [X] T034 [US6] Add Joja detection in `BundleAdapter` in `src/StardewFOMO.Mod/Adapters/BundleAdapter.cs` — check Game1.player.hasOrWillReceiveMail("JojaMember") or CC building state; return false from IsCommunityCenterActive()
- [X] T035 [US6] Enhance `DrawBundlesTab` in `src/StardewFOMO.Mod/UI/PlannerOverlay.cs` — check IsCommunityCenterActive(); if false, display "You chose the Joja Mart route. Community Center bundles are not available." message

**Checkpoint**: User Story 6 complete — Joja route handled gracefully

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Final improvements and validation

- [X] T036 [P] Add unit tests for edge cases in all service test files — empty bundles, all complete, remixed bundles with unusual configurations
- [X] T037 Run manual validation following quickstart scenarios — verify all acceptance criteria from spec.md
- [X] T038 Update README.md with bundle tracker feature documentation

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phases 3-8)**: All depend on Foundational phase completion
  - US1, US2, US3 are all P1 and should be done first (in order due to shared UI work)
  - US4, US5 are P2 and can proceed after P1 stories
  - US6 is P3 polish and can be done last
- **Polish (Phase 9)**: Depends on desired user stories being complete

### User Story Dependencies

- **US1 (View Progress)**: Foundation only — establishes room/bundle display structure
- **US2 (Items Needed)**: Builds on US1 UI layout for bundle expansion
- **US3 (Highlight Owned)**: Uses US2 item display; adds ownership indicators
- **US4 (Availability Filter)**: Can work independently but benefits from US1-3 UI
- **US5 (Notifications)**: Uses bundle matching logic similar to US3
- **US6 (Joja Handling)**: Independent polish task

### Parallel Opportunities

```bash
# Phase 1 - All model creation in parallel:
T001, T002, T003, T004

# Phase 2 - Interface creation in parallel:
T006, T007, T011, T012

# Phase 2 - Adapter creation in parallel after interfaces:
T010 (after T006)

# Tests can be written in parallel with their services
```

---

## Implementation Strategy

### MVP First (User Stories 1-3)

1. Complete Phase 1: Setup (models)
2. Complete Phase 2: Foundational (interfaces, adapters)
3. Complete Phases 3-5: User Stories 1, 2, 3 (P1 features)
4. **STOP and VALIDATE**: Test bundle viewing, item requirements, owned item highlighting
5. Deploy/demo if ready — core bundle tracking functional

### Incremental Delivery

1. Setup + Foundational → Infrastructure ready
2. Add US1 → Room/bundle progress visible → Demo
3. Add US2 → Item requirements visible → Demo
4. Add US3 → Owned items highlighted → Demo (MVP complete!)
5. Add US4 → Availability filter → Demo
6. Add US5 → Notifications → Demo
7. Add US6 → Joja handling → Complete

---

## Summary

- **Total Tasks**: 38
- **Phase 1 (Setup)**: 4 tasks
- **Phase 2 (Foundational)**: 8 tasks
- **US1 (View Progress)**: 4 tasks
- **US2 (Items Needed)**: 3 tasks
- **US3 (Highlight Owned)**: 4 tasks
- **US4 (Availability Filter)**: 6 tasks
- **US5 (Notifications)**: 4 tasks
- **US6 (Joja Handling)**: 2 tasks
- **Polish**: 3 tasks

**Parallel Opportunities**: T001-T004, T006-T007+T011-T012, T010, tests with services

**MVP Scope**: Setup + Foundational + US1 + US2 + US3 = 23 tasks
