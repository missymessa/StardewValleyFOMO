# Tasks: Daily Planner — Stardew Valley FOMO Mod

**Input**: Design documents from `/specs/001-daily-planner/`
**Prerequisites**: plan.md (✅), spec.md (✅)

**Tests**: TDD is MANDATORY per constitution (Principle II). Tests are written before implementation, must fail first, and must run without game assemblies.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Core library**: `src/StardewFOMO.Core/`
- **SMAPI adapter**: `src/StardewFOMO.Mod/`
- **Tests**: `tests/StardewFOMO.Core.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization, solution structure, build configuration

- [X] T001 Create solution file `StardewFOMO.sln` at repository root with `dotnet new sln`
- [X] T002 Create Core class library project `src/StardewFOMO.Core/StardewFOMO.Core.csproj` targeting net6.0 with no game references
- [X] T003 Create SMAPI mod project `src/StardewFOMO.Mod/StardewFOMO.Mod.csproj` targeting net6.0 with Pathoschild.Stardew.ModBuildConfig NuGet reference and project reference to Core
- [X] T004 Create xUnit test project `tests/StardewFOMO.Core.Tests/StardewFOMO.Core.Tests.csproj` with references to Core, xUnit, and Moq only (NO game DLL references)
- [X] T005 Add all projects to the solution file
- [X] T006 Create SMAPI mod manifest `src/StardewFOMO.Mod/manifest.json` with mod metadata (Name, Author, Version, UniqueID, EntryDll, MinimumApiVersion)
- [X] T007 Create mod configuration POCO `src/StardewFOMO.Mod/ModConfig.cs` with configurable hotkey (default: F7), birthday lookahead days (default: 7), and season alert days (default: 3)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core interfaces, domain models, logging abstraction, and test infrastructure that ALL user stories depend on

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Core Interfaces

- [X] T008 [P] Create `ILogger` interface in `src/StardewFOMO.Core/Interfaces/ILogger.cs` with Log(level, message, context) method
- [X] T009 [P] Create `IGameStateProvider` interface in `src/StardewFOMO.Core/Interfaces/IGameStateProvider.cs` with methods for GetCurrentDate(), GetCurrentWeather(), GetTimeOfDay(), GetTomorrowWeather()
- [X] T010 [P] Create `IBundleRepository` interface in `src/StardewFOMO.Core/Interfaces/IBundleRepository.cs` with methods for GetAllBundles(), GetIncompleteBundles(), IsItemNeededForBundle(itemId)
- [X] T011 [P] Create `ICollectionRepository` interface in `src/StardewFOMO.Core/Interfaces/ICollectionRepository.cs` with methods for HasCaughtFish(id), HasShippedItem(id), HasDonatedToMuseum(id), HasCookedRecipe(id), HasCraftedRecipe(id)
- [X] T012 [P] Create `IInventoryProvider` interface in `src/StardewFOMO.Core/Interfaces/IInventoryProvider.cs` with methods for HasItemInInventoryOrStorage(itemId), GetInventoryItems()
- [X] T013 [P] Create `INpcRepository` interface in `src/StardewFOMO.Core/Interfaces/INpcRepository.cs` with methods for GetAllBirthdays(), GetLovedGifts(npcName), GetLikedGifts(npcName)
- [X] T014 [P] Create `IFishRepository` interface in `src/StardewFOMO.Core/Interfaces/IFishRepository.cs` with methods for GetAllFish(), GetFishBySeasonAndWeather(season, weather)
- [X] T015 [P] Create `IForageRepository` interface in `src/StardewFOMO.Core/Interfaces/IForageRepository.cs` with methods for GetAllForageables(), GetForageablesBySeason(season)
- [X] T016 [P] Create `IRecipeRepository` interface in `src/StardewFOMO.Core/Interfaces/IRecipeRepository.cs` with methods for GetAllCookingRecipes(), GetAllCraftingRecipes(), GetRecipeIngredients(recipeId)

### Domain Models

- [X] T017 [P] Create `GameDate` model in `src/StardewFOMO.Core/Models/GameDate.cs` with Season (enum), Day, Year, DayOfWeek properties (date-only — weather is modeled separately via Weather enum)
- [X] T018 [P] Create `Season` enum in `src/StardewFOMO.Core/Models/Season.cs` with Spring, Summer, Fall, Winter values
- [X] T019 [P] Create `Weather` enum in `src/StardewFOMO.Core/Models/Weather.cs` with Sunny, Rainy, Stormy, Snowy, Windy values
- [X] T020 [P] Create `CollectionStatus` enum in `src/StardewFOMO.Core/Models/CollectionStatus.cs` with NotCollected, InInventory, EverCollected values
- [X] T021 [P] Create `CollectibleItem` model in `src/StardewFOMO.Core/Models/CollectibleItem.cs` with Id, Name, CollectionType (enum: Fish, Forage, Artifact, Mineral, ShippableItem, CookingRecipe, CraftingRecipe), availability conditions, CollectionStatus, and BundleNames list
- [X] T022 [P] Create `BundleInfo` model in `src/StardewFOMO.Core/Models/BundleInfo.cs` with BundleName, RoomName, RequiredItems list, CompletedItems list, IsComplete property
- [X] T023 [P] Create `NpcBirthday` model in `src/StardewFOMO.Core/Models/NpcBirthday.cs` with NpcName, BirthdayDate (GameDate), LovedGifts list, LikedGifts list
- [X] T024 [P] Create `TomorrowPreview` model in `src/StardewFOMO.Core/Models/TomorrowPreview.cs` with WeatherForecast, NewCollectibles list, Events list, SeasonChangeWarning
- [X] T025 [P] Create `DailySummary` model in `src/StardewFOMO.Core/Models/DailySummary.cs` with Date, AvailableFish, AvailableForageables, BundleNeededItems, TodayBirthdays, UpcomingBirthdays, TomorrowPreview, LastChanceAlerts, AllCollectionItems

### Test Infrastructure

- [X] T026 [P] Create `FakeGameStateProvider` in `tests/StardewFOMO.Core.Tests/Fakes/FakeGameStateProvider.cs` implementing IGameStateProvider with settable properties
- [X] T027 [P] Create `FakeBundleRepository` in `tests/StardewFOMO.Core.Tests/Fakes/FakeBundleRepository.cs` implementing IBundleRepository with in-memory bundle data
- [X] T028 [P] Create `FakeCollectionRepository` in `tests/StardewFOMO.Core.Tests/Fakes/FakeCollectionRepository.cs` implementing ICollectionRepository with in-memory collection data
- [X] T029 [P] Create `FakeInventoryProvider` in `tests/StardewFOMO.Core.Tests/Fakes/FakeInventoryProvider.cs` implementing IInventoryProvider with in-memory inventory
- [X] T030 [P] Create `FakeNpcRepository` in `tests/StardewFOMO.Core.Tests/Fakes/FakeNpcRepository.cs` implementing INpcRepository with in-memory NPC data
- [X] T031 [P] Create `FakeFishRepository` in `tests/StardewFOMO.Core.Tests/Fakes/FakeFishRepository.cs` implementing IFishRepository with in-memory fish data
- [X] T032 [P] Create `FakeForageRepository` in `tests/StardewFOMO.Core.Tests/Fakes/FakeForageRepository.cs` implementing IForageRepository with in-memory forage data
- [X] T033 [P] Create `FakeRecipeRepository` in `tests/StardewFOMO.Core.Tests/Fakes/FakeRecipeRepository.cs` implementing IRecipeRepository with in-memory recipe data
- [X] T034 [P] Create `TestLogger` in `tests/StardewFOMO.Core.Tests/Fakes/TestLogger.cs` implementing ILogger that captures log messages for assertion

**Checkpoint**: Foundation ready — all interfaces, models, and test infrastructure exist. User story implementation can now begin.

---

## Phase 3: User Story 1 — Today's Collectibles Overview (Priority: P1) 🎯 MVP

**Goal**: Player sees all fish and forageable items available today based on season, weather, and time, with collection status indicated.

**Independent Test**: Open planner on any in-game day → correct fish and forageables listed for current season/weather/location; collected items visually distinguished.

### Tests for User Story 1 (TDD — write and verify FAIL first)

- [X] T035 [P] [US1] Write tests for `FishAvailabilityService` in `tests/StardewFOMO.Core.Tests/Services/FishAvailabilityServiceTests.cs` — test filtering by season, weather, time; test collected fish marked correctly; test behavior on festival days where fishing may be restricted
- [X] T036 [P] [US1] Write tests for `ForageAvailabilityService` in `tests/StardewFOMO.Core.Tests/Services/ForageAvailabilityServiceTests.cs` — test filtering by season; test grouping by location; test collected forageables marked correctly; test behavior on festival days where foraging may be restricted

### Implementation for User Story 1

- [X] T037 [US1] Implement `FishAvailabilityService` in `src/StardewFOMO.Core/Services/FishAvailabilityService.cs` — accepts IFishRepository, IGameStateProvider, ICollectionRepository, IInventoryProvider; returns fish available today with collection status
- [X] T038 [US1] Implement `ForageAvailabilityService` in `src/StardewFOMO.Core/Services/ForageAvailabilityService.cs` — accepts IForageRepository, IGameStateProvider, ICollectionRepository, IInventoryProvider; returns forageables available today grouped by location with collection status
- [X] T039 [US1] Run tests for US1 and verify all pass (Red → Green)

**Checkpoint**: User Story 1 core logic complete. Fish and forage availability with collection status is testable independently.

---

## Phase 4: User Story 2 — Community Center Bundle Tracker (Priority: P1) 🎯 MVP

**Goal**: Today's available collectibles are flagged when they are needed for incomplete Community Center bundles.

**Independent Test**: Load save with partial bundles → planner highlights bundle-needed items with bundle name.

### Tests for User Story 2 (TDD — write and verify FAIL first)

- [X] T040 [P] [US2] Write tests for `BundleTrackingService` in `tests/StardewFOMO.Core.Tests/Services/BundleTrackingServiceTests.cs` — test matching available items to incomplete bundles; test multiple bundles for same item; test completed bundles excluded; test Joja route hides bundles

### Implementation for User Story 2

- [X] T041 [US2] Implement `BundleTrackingService` in `src/StardewFOMO.Core/Services/BundleTrackingService.cs` — accepts IBundleRepository, IInventoryProvider; cross-references available items with incomplete bundle needs; returns items with associated bundle names
- [X] T042 [US2] Run tests for US2 and verify all pass (Red → Green)

**Checkpoint**: User Story 2 core logic complete. Bundle tracking overlays on US1 collectibles data.

---

## Phase 5: User Story 3 — NPC Birthday Reminders (Priority: P2)

**Goal**: Player sees today's and upcoming NPC birthdays with loved/liked gift lists.

**Independent Test**: Advance to NPC birthday → planner shows NPC name, date, gift preferences; 7-day lookahead works.

### Tests for User Story 3 (TDD — write and verify FAIL first)

- [X] T043 [P] [US3] Write tests for `BirthdayService` in `tests/StardewFOMO.Core.Tests/Services/BirthdayServiceTests.cs` — test today's birthday detection; test 7-day lookahead; test cross-season lookahead; test no birthdays in window shows next birthday; test gift list retrieval

### Implementation for User Story 3

- [X] T044 [US3] Implement `BirthdayService` in `src/StardewFOMO.Core/Services/BirthdayService.cs` — accepts INpcRepository, IGameStateProvider; returns today's birthdays and upcoming birthdays within configurable lookahead window with loved/liked gifts
- [X] T045 [US3] Run tests for US3 and verify all pass (Red → Green)

**Checkpoint**: User Story 3 core logic complete. Birthday reminders work independently of collectibles.

---

## Phase 6: User Story 4 — Tomorrow's Preview & Preparation (Priority: P2)

**Goal**: Player sees tomorrow's weather forecast, newly available collectibles, and upcoming events/festivals.

**Independent Test**: Check "Tomorrow" section → correct weather, new fish/forageables, festival info displayed.

### Tests for User Story 4 (TDD — write and verify FAIL first)

- [X] T046 [P] [US4] Write tests for `TomorrowPreviewService` in `tests/StardewFOMO.Core.Tests/Services/TomorrowPreviewServiceTests.cs` — test weather forecast inclusion; test rain-exclusive fish highlighted; test season-end warning with uncollected items; test festival day display

### Implementation for User Story 4

- [X] T047 [US4] Implement `TomorrowPreviewService` in `src/StardewFOMO.Core/Services/TomorrowPreviewService.cs` — accepts IGameStateProvider, IFishRepository, IForageRepository, ICollectionRepository; builds tomorrow preview with weather, new collectibles, events, season-change warnings
- [X] T048 [US4] Run tests for US4 and verify all pass (Red → Green)

**Checkpoint**: User Story 4 core logic complete. Tomorrow preview works independently.

---

## Phase 7: User Story 5 — In-Game Planner Panel (Priority: P2)

**Goal**: Player opens/closes the planner panel via hotkey; priority view shows bundle-needed + time-sensitive items; "Show All" toggle reveals full collection tracking; view state persists within session.

**Independent Test**: Press hotkey → panel opens with priority view; toggle Show All; press hotkey → panel closes; panel does not open during menus/cutscenes.

### Tests for User Story 5 (TDD — write and verify FAIL first)

- [X] T049 [P] [US5] Write tests for `DailySummaryService` in `tests/StardewFOMO.Core.Tests/Services/DailySummaryServiceTests.cs` — test orchestration of all sub-services into a single DailySummary; test priority view filtering (bundle-needed + time-sensitive + birthdays + alerts); test "Show All" returns full collection data

### Implementation for User Story 5

- [X] T050 [US5] Implement `DailySummaryService` in `src/StardewFOMO.Core/Services/DailySummaryService.cs` — orchestrates FishAvailabilityService, ForageAvailabilityService, BundleTrackingService, CollectionTrackingService, BirthdayService, TomorrowPreviewService, SeasonAlertService; returns DailySummary with priority and full views
- [X] T051 [US5] Run tests for US5 and verify all pass (Red → Green)
- [X] T052 [US5] Create `SmapiLoggerAdapter` in `src/StardewFOMO.Mod/Adapters/SmapiLoggerAdapter.cs` implementing ILogger by forwarding to SMAPI's IMonitor
- [X] T053 [US5] Create `GameStateAdapter` in `src/StardewFOMO.Mod/Adapters/GameStateAdapter.cs` implementing IGameStateProvider by reading from Game1 and game context
- [X] T054 [US5] Create `BundleAdapter` in `src/StardewFOMO.Mod/Adapters/BundleAdapter.cs` implementing IBundleRepository by reading Community Center data from game state
- [X] T055 [US5] Create `CollectionAdapter` in `src/StardewFOMO.Mod/Adapters/CollectionAdapter.cs` implementing ICollectionRepository by reading player's collection tabs
- [X] T056 [US5] Create `InventoryAdapter` in `src/StardewFOMO.Mod/Adapters/InventoryAdapter.cs` implementing IInventoryProvider by reading player inventory and chest storage
- [X] T057 [US5] Create `NpcAdapter` in `src/StardewFOMO.Mod/Adapters/NpcAdapter.cs` implementing INpcRepository by reading NPC data from game content
- [X] T058 [US5] Create `FishDataAdapter` in `src/StardewFOMO.Mod/Adapters/FishDataAdapter.cs` implementing IFishRepository by reading fish data from game content files
- [X] T059 [US5] Create `ForageDataAdapter` in `src/StardewFOMO.Mod/Adapters/ForageDataAdapter.cs` implementing IForageRepository by reading forage data from game content
- [X] T060 [US5] Create `RecipeDataAdapter` in `src/StardewFOMO.Mod/Adapters/RecipeDataAdapter.cs` implementing IRecipeRepository by reading recipe data from game content
- [X] T061 [US5] Implement `PlannerOverlay` in `src/StardewFOMO.Mod/UI/PlannerOverlay.cs` — MonoGame/SpriteBatch rendering of DailySummary; priority view as default; "Show All" toggle; session-scoped view state (resets on game restart); close button; Stardew-native visual styling; include vertical scrolling for overflow content; panel dimensions should adapt to screen resolution or use a fixed proportion
- [X] T062 [US5] Implement `ModEntry` in `src/StardewFOMO.Mod/ModEntry.cs` — SMAPI entry point; reads config; wires all adapters to core services; registers hotkey handler to toggle PlannerOverlay; suppresses panel during menus/cutscenes; refreshes DailySummary on day change via SMAPI DayStarted event

**Checkpoint**: User Story 5 complete. Full mod is playable — panel opens with all data from US1–US4.

---

## Phase 8: User Story 6 — Season Change & Last-Chance Alerts (Priority: P3)

**Goal**: Player receives prominent alerts during the final 3 days of a season for uncollected season-exclusive items.

**Independent Test**: Advance to last 3 days of season with uncollected seasonal items → "Last Chance" alert appears with item list and weather notes.

### Tests for User Story 6 (TDD — write and verify FAIL first)

- [X] T063 [P] [US6] Write tests for `SeasonAlertService` in `tests/StardewFOMO.Core.Tests/Services/SeasonAlertServiceTests.cs` — test alert triggers in last 3 days only; test lists uncollected season-exclusive items; test no alert when all collected; test weather-dependent items note weather requirement; test items not obtainable today note "wait until next year"

### Implementation for User Story 6

- [X] T064 [US6] Implement `SeasonAlertService` in `src/StardewFOMO.Core/Services/SeasonAlertService.cs` — accepts IGameStateProvider, IFishRepository, IForageRepository, ICollectionRepository; checks if within alert window; returns list of at-risk season-exclusive items with weather notes
- [X] T065 [US6] Run tests for US6 and verify all pass (Red → Green)

**Checkpoint**: User Story 6 complete. Last-chance alerts integrated into DailySummary and visible in PlannerOverlay.

---

## Phase 9: Collection Tracking — Shipping, Museum, Cooking, Crafting (Priority: P1 expanded scope)

**Goal**: "Show All" view includes full tracking for shipping collection, museum donations, cooking recipes, and crafting recipes.

**Independent Test**: Toggle "Show All" → shipping/museum/cooking/crafting items shown with completion status; cooking/crafting highlight when ingredients/materials are in inventory.

### Tests for Phase 9 (TDD — write and verify FAIL first)

- [X] T066 [P] Write tests for `CollectionTrackingService` in `tests/StardewFOMO.Core.Tests/Services/CollectionTrackingServiceTests.cs` — test shipping collection tracking; test museum donation tracking; test cooking recipe tracking with ingredient availability; test crafting recipe tracking with material availability

### Implementation for Phase 9

- [X] T067 Implement `CollectionTrackingService` in `src/StardewFOMO.Core/Services/CollectionTrackingService.cs` — accepts ICollectionRepository, IInventoryProvider, IRecipeRepository; returns completion status for all collection types; for cooking/crafting, checks ingredient/material availability via IInventoryProvider
- [X] T068 Run tests for collection tracking and verify all pass (Red → Green)
- [X] T069 Integrate `CollectionTrackingService` into `DailySummaryService` "Show All" output in `src/StardewFOMO.Core/Services/DailySummaryService.cs`

**Checkpoint**: Full collection tracking available in "Show All" view.

---

## Phase 10: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [X] T070 [P] Add XML-doc comments to all public APIs in `src/StardewFOMO.Core/` (constitution quality gate)
- [X] T071 [P] Add XML-doc comments to all public APIs in `src/StardewFOMO.Mod/` (constitution quality gate)
- [X] T072 Verify structured logging with context (player name, in-game date, event type) across all services in `src/StardewFOMO.Core/Services/`
- [X] T073 Verify mod handles Joja route gracefully — bundle tracking hidden when Community Center is not active in `src/StardewFOMO.Core/Services/BundleTrackingService.cs`
- [X] T074 Verify mod handles mid-playthrough install — all adapters read current save state correctly in `src/StardewFOMO.Mod/Adapters/`
- [X] T075 Verify DailySummary refreshes on day change and season transitions via SMAPI DayStarted event in `src/StardewFOMO.Mod/ModEntry.cs`
- [X] T076 Performance validation — panel opens under 1 second; no FPS degradation during gameplay
- [X] T077 [P] Ensure crossplatform compatibility — use Path.Combine, Helper.DirectoryPath, PathUtilities per SMAPI guidelines in all file paths across `src/StardewFOMO.Mod/`
- [X] T078 Manual play-test: load mod in game, verify all 6 user stories work end-to-end (document findings as issues or TODOs per constitution)
- [X] T079 Verify mod gracefully handles unknown/modded item IDs — unrecognized items are skipped without errors across all services (edge case from spec)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion — BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational (Phase 2)
- **User Story 2 (Phase 4)**: Depends on Foundational (Phase 2); integrates with US1 output but independently testable
- **User Story 3 (Phase 5)**: Depends on Foundational (Phase 2) — fully independent of US1/US2
- **User Story 4 (Phase 6)**: Depends on Foundational (Phase 2); uses fish/forage repos but independently testable
- **User Story 5 (Phase 7)**: Depends on US1, US2, US3, US4 for full orchestration; SMAPI adapters + UI
- **User Story 6 (Phase 8)**: Depends on Foundational (Phase 2) — core logic independent; integrates into DailySummary
- **Collection Tracking (Phase 9)**: Depends on Foundational (Phase 2); integrates into DailySummary "Show All"
- **Polish (Phase 10)**: Depends on all previous phases

### User Story Dependencies

- **US1 (P1)**: Can start after Phase 2 — No dependencies on other stories
- **US2 (P1)**: Can start after Phase 2 — Uses same item data as US1 but different service
- **US3 (P2)**: Can start after Phase 2 — Fully independent (different data domain: NPCs)
- **US4 (P2)**: Can start after Phase 2 — Uses fish/forage repos but independently testable
- **US5 (P2)**: Depends on US1–US4 + US6 for full panel content; SMAPI layer is US5-specific
- **US6 (P3)**: Can start after Phase 2 — Core logic independent

### Within Each User Story

- Tests MUST be written and FAIL before implementation (TDD — Constitution Principle II)
- Implementation makes tests pass (Red → Green → Refactor)
- Story complete before moving to next priority (unless parallelizing across developers)

### Parallel Opportunities

- All Phase 2 tasks (T008–T034) marked [P] can run in parallel (different files)
- US1, US2, US3, US4, US6 core logic (Phases 3, 4, 5, 6, 8) can all start in parallel after Phase 2
- Within each story: test tasks marked [P] can run in parallel
- All SMAPI adapter tasks (T052–T060) marked separately can run in parallel
- Phase 9 (Collection Tracking) can run in parallel with US3, US4, US6
- Phase 10 polish tasks marked [P] can run in parallel

---

## Parallel Example: After Phase 2 Completes

```text
# All of these can start simultaneously after Phase 2:

Stream A (MVP path):
  T035 + T036 (US1 tests) → T037 + T038 (US1 impl) → T039 (verify)
  T040 (US2 tests) → T041 (US2 impl) → T042 (verify)

Stream B (independent):
  T043 (US3 tests) → T044 (US3 impl) → T045 (verify)

Stream C (independent):
  T046 (US4 tests) → T047 (US4 impl) → T048 (verify)

Stream D (independent):
  T063 (US6 tests) → T064 (US6 impl) → T065 (verify)

Stream E (independent):
  T066 (Collection tests) → T067 (Collection impl) → T068 (verify)

# After Streams A–E converge:
  T049–T062 (US5: orchestration + SMAPI adapters + UI)
  T069 (integrate collection tracking)
  T070–T078 (polish)
```

---

## Implementation Strategy

### MVP First (User Stories 1 + 2 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks all stories)
3. Complete Phase 3: User Story 1 (Today's Collectibles)
4. Complete Phase 4: User Story 2 (Bundle Tracker)
5. **STOP and VALIDATE**: Core value delivered — player can see what to collect today with bundle priorities
6. Minimal US5 (panel + adapters) to make it visible in-game

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add US1 + US2 → Test independently → **MVP! Core planner with bundle tracking**
3. Add US3 → Test independently → Birthday reminders
4. Add US4 → Test independently → Tomorrow preview
5. Add US5 → Full panel with priority/Show All views → **Playable mod**
6. Add US6 → Season alerts → **Feature complete**
7. Add Phase 9 → Full collection tracking → **Completionist mode**
8. Phase 10 → Polish → **Release candidate**

Each increment adds value without breaking previous stories.

---

## Notes

- [P] tasks = different files, no dependencies on incomplete tasks
- [Story] label maps task to specific user story for traceability
- TDD is mandatory (Constitution Principle II) — all test tasks must be completed and failing before implementation
- All Core services accept interfaces, never concrete game types (Constitution Principle I)
- ModEntry is a thin adapter only (Constitution Principle III)
- All services accept ILogger for structured logging (Constitution Principle IV)
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
