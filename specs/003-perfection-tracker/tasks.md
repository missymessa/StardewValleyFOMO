# Tasks: Perfection Tracker

**Input**: Design documents from `/specs/003-perfection-tracker/`
**Prerequisites**: plan.md (required), spec.md (required for user stories)

**Tests**: Tests are included to maintain existing test coverage standards established in features 001-002.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create new interfaces and models needed across all perfection categories

- [X] T001 [P] Create IPerfectionRepository interface in src/StardewFOMO.Core/Interfaces/IPerfectionRepository.cs
- [X] T002 [P] Create IMonsterRepository interface in src/StardewFOMO.Core/Interfaces/IMonsterRepository.cs
- [X] T003 [P] Create IStardropRepository interface in src/StardewFOMO.Core/Interfaces/IStardropRepository.cs
- [X] T004 [P] Create IWalnutRepository interface in src/StardewFOMO.Core/Interfaces/IWalnutRepository.cs
- [X] T005 [P] Create ISkillRepository interface in src/StardewFOMO.Core/Interfaces/ISkillRepository.cs
- [X] T006 [P] Create IBuildingRepository interface in src/StardewFOMO.Core/Interfaces/IBuildingRepository.cs
- [X] T007 [P] Create IFriendshipRepository interface in src/StardewFOMO.Core/Interfaces/IFriendshipRepository.cs
- [X] T008 [P] Create PerfectionProgress model in src/StardewFOMO.Core/Models/PerfectionProgress.cs
- [X] T009 [P] Create PerfectionCategory model in src/StardewFOMO.Core/Models/PerfectionCategory.cs
- [X] T010 [P] Create PerfectionItem model in src/StardewFOMO.Core/Models/PerfectionItem.cs
- [X] T011 [P] Create ShippingItem model in src/StardewFOMO.Core/Models/ShippingItem.cs
- [X] T012 [P] Create MonsterGoal model in src/StardewFOMO.Core/Models/MonsterGoal.cs
- [X] T013 [P] Create StardropInfo model in src/StardewFOMO.Core/Models/StardropInfo.cs
- [X] T014 [P] Create WalnutGroup model in src/StardewFOMO.Core/Models/WalnutGroup.cs
- [X] T015 [P] Create SkillProgress model in src/StardewFOMO.Core/Models/SkillProgress.cs
- [X] T016 [P] Create FarmBuilding model in src/StardewFOMO.Core/Models/FarmBuilding.cs
- [X] T017 [P] Create FriendshipInfo model in src/StardewFOMO.Core/Models/FriendshipInfo.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core adapters and test fakes that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T018 [P] Create FakePerfectionRepository in tests/StardewFOMO.Core.Tests/Fakes/FakePerfectionRepository.cs
- [X] T019 [P] Create FakeMonsterRepository in tests/StardewFOMO.Core.Tests/Fakes/FakeMonsterRepository.cs
- [X] T020 [P] Create FakeStardropRepository in tests/StardewFOMO.Core.Tests/Fakes/FakeStardropRepository.cs
- [X] T021 [P] Create FakeWalnutRepository in tests/StardewFOMO.Core.Tests/Fakes/FakeWalnutRepository.cs
- [X] T022 [P] Create FakeSkillRepository in tests/StardewFOMO.Core.Tests/Fakes/FakeSkillRepository.cs
- [X] T023 [P] Create FakeBuildingRepository in tests/StardewFOMO.Core.Tests/Fakes/FakeBuildingRepository.cs
- [X] T024 [P] Create FakeFriendshipRepository in tests/StardewFOMO.Core.Tests/Fakes/FakeFriendshipRepository.cs
- [X] T025 [P] Create MonsterAdapter in src/StardewFOMO.Mod/Adapters/MonsterAdapter.cs
- [X] T026 [P] Create StardropAdapter in src/StardewFOMO.Mod/Adapters/StardropAdapter.cs
- [X] T027 [P] Create WalnutAdapter in src/StardewFOMO.Mod/Adapters/WalnutAdapter.cs
- [X] T028 [P] Create SkillAdapter in src/StardewFOMO.Mod/Adapters/SkillAdapter.cs
- [X] T029 [P] Create BuildingAdapter in src/StardewFOMO.Mod/Adapters/BuildingAdapter.cs
- [X] T030 [P] Create FriendshipAdapter in src/StardewFOMO.Mod/Adapters/FriendshipAdapter.cs
- [X] T031 Add GMCM config options (ShowPerfectionTab, PerfectionShowOwnedIndicator, PerfectionShowAvailableToday) in src/StardewFOMO.Mod/ModConfig.cs

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 2 — Track Shipping Completion (Priority: P1)

**Goal**: Display shipping collection progress with subcategory grouping and "★ [HAVE]" indicators

**Independent Test**: View Shipping section, verify shipped/total counts with remaining items listed

### Tests for User Story 2

- [X] T032 [P] [US2] Create ShippingProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/ShippingProgressServiceTests.cs

### Implementation for User Story 2

- [X] T033 [P] [US2] Create ShippingProgressService in src/StardewFOMO.Core/Services/ShippingProgressService.cs
- [ ] T034 [US2] Add GetShippableItems() method to ICollectionRepository in src/StardewFOMO.Core/Interfaces/ICollectionRepository.cs
- [ ] T035 [US2] Implement GetShippableItems() in CollectionAdapter in src/StardewFOMO.Mod/Adapters/CollectionAdapter.cs

**Checkpoint**: Shipping tracking complete and testable independently

---

## Phase 4: User Story 3 — Track Fish Collection (Priority: P1)

**Goal**: Display fish collection progress with season/weather/time availability hints

**Independent Test**: View Fish section, verify caught/total counts with remaining fish and catch conditions

### Tests for User Story 3

- [X] T036 [P] [US3] Create FishProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/FishProgressServiceTests.cs

### Implementation for User Story 3

- [X] T037 [P] [US3] Create FishProgressService in src/StardewFOMO.Core/Services/FishProgressService.cs
- [ ] T038 [US3] Add GetAllCaughtFishIds() method to ICollectionRepository in src/StardewFOMO.Core/Interfaces/ICollectionRepository.cs
- [ ] T039 [US3] Implement GetAllCaughtFishIds() in CollectionAdapter in src/StardewFOMO.Mod/Adapters/CollectionAdapter.cs

**Checkpoint**: Fish tracking complete and testable independently

---

## Phase 5: User Story 4 — Track Cooking Recipes (Priority: P1)

**Goal**: Display cooking recipe progress with learned/cooked status and ingredient requirements

**Independent Test**: View Cooking section, verify cooked/total counts with recipe status and ingredients

### Tests for User Story 4

- [X] T040 [P] [US4] Create CookingProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/CookingProgressServiceTests.cs

### Implementation for User Story 4

- [X] T041 [P] [US4] Create CookingProgressService in src/StardewFOMO.Core/Services/CookingProgressService.cs
- [ ] T042 [US4] Add GetAllCookedRecipeIds() and GetAllKnownCookingRecipeIds() to ICollectionRepository
- [ ] T043 [US4] Implement new methods in CollectionAdapter in src/StardewFOMO.Mod/Adapters/CollectionAdapter.cs

**Checkpoint**: Cooking tracking complete and testable independently

---

## Phase 6: User Story 5 — Track Crafting Recipes (Priority: P1)

**Goal**: Display crafting recipe progress with learned/crafted status and material requirements

**Independent Test**: View Crafting section, verify crafted/total counts with recipe status and materials

### Tests for User Story 5

- [X] T044 [P] [US5] Create CraftingProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/CraftingProgressServiceTests.cs

### Implementation for User Story 5

- [X] T045 [P] [US5] Create CraftingProgressService in src/StardewFOMO.Core/Services/CraftingProgressService.cs
- [ ] T046 [US5] Add GetAllCraftedRecipeIds() and GetAllKnownCraftingRecipeIds() to ICollectionRepository
- [ ] T047 [US5] Implement new methods in CollectionAdapter in src/StardewFOMO.Mod/Adapters/CollectionAdapter.cs

**Checkpoint**: Crafting tracking complete and testable independently

---

## Phase 7: User Story 6 — Track Friendship Progress (Priority: P1)

**Goal**: Display friendship levels with all NPCs, sorted by hearts needed

**Independent Test**: View Friendship section, verify heart counts match in-game values

### Tests for User Story 6

- [X] T048 [P] [US6] Create FriendshipProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/FriendshipProgressServiceTests.cs

### Implementation for User Story 6

- [X] T049 [P] [US6] Create FriendshipProgressService in src/StardewFOMO.Core/Services/FriendshipProgressService.cs

**Checkpoint**: Friendship tracking complete and testable independently

---

## Phase 8: User Story 7 — Track Farm Buildings Progress (Priority: P2)

**Goal**: Display obelisk and Golden Clock building status

**Independent Test**: View Farm Buildings section, verify built/not-built status

### Tests for User Story 7

- [X] T050 [P] [US7] Create BuildingProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/BuildingProgressServiceTests.cs

### Implementation for User Story 7

- [X] T051 [P] [US7] Create BuildingProgressService in src/StardewFOMO.Core/Services/BuildingProgressService.cs

**Checkpoint**: Building tracking complete and testable independently

---

## Phase 9: User Story 8 — Track Monster Slayer Goals (Priority: P2)

**Goal**: Display monster eradication progress with kill counts and spawn locations

**Independent Test**: View Monster Slayer section, verify kill counts match Adventurer's Guild

### Tests for User Story 8

- [X] T052 [P] [US8] Create MonsterProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/MonsterProgressServiceTests.cs

### Implementation for User Story 8

- [X] T053 [P] [US8] Create MonsterProgressService in src/StardewFOMO.Core/Services/MonsterProgressService.cs

**Checkpoint**: Monster tracking complete and testable independently

---

## Phase 10: User Story 9 — Track Stardrops Collected (Priority: P2)

**Goal**: Display stardrop collection status with acquisition hints

**Independent Test**: View Stardrops section, verify collected count and hints for uncollected

### Tests for User Story 9

- [X] T054 [P] [US9] Create StardropProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/StardropProgressServiceTests.cs

### Implementation for User Story 9

- [X] T055 [P] [US9] Create StardropProgressService in src/StardewFOMO.Core/Services/StardropProgressService.cs

**Checkpoint**: Stardrop tracking complete and testable independently

---

## Phase 11: User Story 10 — Track Golden Walnuts (Priority: P2)

**Goal**: Display golden walnut collection grouped by acquisition type

**Independent Test**: View Golden Walnuts section, verify count and group breakdowns

### Tests for User Story 10

- [X] T056 [P] [US10] Create WalnutProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/WalnutProgressServiceTests.cs

### Implementation for User Story 10

- [X] T057 [P] [US10] Create WalnutProgressService in src/StardewFOMO.Core/Services/WalnutProgressService.cs

**Checkpoint**: Walnut tracking complete and testable independently

---

## Phase 12: User Story 11 — Track Skill Levels (Priority: P3)

**Goal**: Display skill levels with XP needed to level up

**Independent Test**: View Skills section, verify skill levels match character screen

### Tests for User Story 11

- [X] T058 [P] [US11] Create SkillProgressServiceTests in tests/StardewFOMO.Core.Tests/Services/SkillProgressServiceTests.cs

### Implementation for User Story 11

- [X] T059 [P] [US11] Create SkillProgressService in src/StardewFOMO.Core/Services/SkillProgressService.cs

**Checkpoint**: Skill tracking complete and testable independently

---

## Phase 13: User Story 1 — View Overall Perfection Progress (Priority: P1)

**Goal**: Display overall perfection percentage with all category breakdowns (🎯 MVP integration)

**Independent Test**: Open Perfection tab, verify overall percentage and all categories displayed

### Tests for User Story 1

- [X] T060 [P] [US1] Create PerfectionCalculatorServiceTests in tests/StardewFOMO.Core.Tests/Services/PerfectionCalculatorServiceTests.cs

### Implementation for User Story 1

- [X] T061 [US1] Create PerfectionCalculatorService in src/StardewFOMO.Core/Services/PerfectionCalculatorService.cs
- [X] T062 [US1] Create PerfectionAdapter aggregating all adapters in src/StardewFOMO.Mod/Adapters/PerfectionAdapter.cs
- [X] T063 [US1] Add DrawPerfectionTab method to PlannerOverlay in src/StardewFOMO.Mod/UI/PlannerOverlay.cs
- [X] T064 [US1] Wire PerfectionCalculatorService and add Perfection tab in ModEntry in src/StardewFOMO.Mod/ModEntry.cs
- [X] T065 [US1] Add Ginger Island unlock detection to show "Not Yet Unlocked" indicator

**Checkpoint**: Full perfection tab functional with all categories displayed

---

## Phase 14: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T066 [P] Add subcategory expand/collapse UI for Shipping in src/StardewFOMO.Mod/UI/PlannerOverlay.cs
- [ ] T067 [P] Add "★ [HAVE]" indicators for owned items across all categories
- [ ] T068 [P] Add "Available Today" highlighting for fish and seasonal items
- [X] T069 [P] Add congratulatory message for 100% perfection
- [X] T070 Run all tests and verify 100% pass rate
- [ ] T071 Manual testing: verify all categories match in-game perfection tracker

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phases 3-13)**: All depend on Foundational phase completion
  - US2-6 (P1 stories) can proceed in parallel
  - US7-10 (P2 stories) can proceed in parallel
  - US11 (P3) can proceed independently
  - US1 depends on all other services being complete (integration phase)
- **Polish (Phase 14)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (US1)**: Depends on US2-11 (aggregates all categories) - implement LAST
- **User Stories 2-11**: Can start after Foundational (Phase 2) - all independent

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Services depend on interfaces/models from Phase 1
- Adapter implementations depend on interface definitions
- Story complete before moving to next

### Parallel Opportunities

- All Phase 1 tasks (T001-T017) can run in parallel
- All Phase 2 tasks (T018-T031) can run in parallel
- US2-6 (Phases 3-7) can be worked on in parallel
- US7-10 (Phases 8-11) can be worked on in parallel
- Within each story, test and implementation tasks marked [P] can run in parallel

---

## Parallel Example: Phase 1 (Setup)

```bash
# Launch all interface creation tasks together:
Task: "Create IPerfectionRepository interface"
Task: "Create IMonsterRepository interface"
Task: "Create IStardropRepository interface"
# ... all T001-T007 in parallel

# Launch all model creation tasks together:
Task: "Create PerfectionProgress model"
Task: "Create PerfectionCategory model"
# ... all T008-T017 in parallel
```

---

## Parallel Example: Core Categories (US2-6)

```bash
# After Phase 2 complete, launch all P1 stories together:
Developer A: User Story 2 (Shipping) - T032-T035
Developer B: User Story 3 (Fish) - T036-T039
Developer C: User Story 4 (Cooking) - T040-T043
Developer D: User Story 5 (Crafting) - T044-T047
Developer E: User Story 6 (Friendship) - T048-T049
```

---

## Implementation Strategy

### MVP First (Minimal Viable Perfection Tab)

1. Complete Phase 1: Setup (all interfaces and models)
2. Complete Phase 2: Foundational (all adapters and fakes)
3. Complete Phase 3-7: Core P1 categories (Shipping, Fish, Cooking, Crafting, Friendship)
4. Complete Phase 13: Integration (Overall Progress + Tab UI)
5. **STOP and VALIDATE**: Test perfection tab with P1 categories
6. Deploy/demo as MVP

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add US2-6 (P1 categories) → Core perfection data available
3. Add US1 (Integration) → Tab visible with 5 categories → **MVP!**
4. Add US7-10 (P2 categories) → Buildings, Monsters, Stardrops, Walnuts
5. Add US11 (P3) → Skills
6. Add Polish → Full feature complete

---

## Notes

- Task count: 71 tasks
- User stories: 11 (6 P1, 4 P2, 1 P3)
- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
