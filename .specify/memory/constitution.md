<!--
  Sync Impact Report
  ==================
  Version change: N/A (initial) → 1.0.0
  Modified principles: N/A (initial creation)
  Added sections:
    - Core Principles (5 principles)
    - Technology Stack
    - Development Workflow
    - Governance
  Removed sections: None
  Templates requiring updates:
    - .specify/templates/plan-template.md ✅ no changes needed (generic)
    - .specify/templates/spec-template.md ✅ no changes needed (generic)
    - .specify/templates/tasks-template.md ✅ no changes needed (generic)
    - .specify/templates/checklist-template.md ✅ no changes needed (generic)
    - .specify/templates/agent-file-template.md ✅ no changes needed (generic)
  Follow-up TODOs: None
-->

# StardewFOMO Constitution

## Core Principles

### I. Game-Decoupled Core (NON-NEGOTIABLE)

All game logic, data transformations, and decision-making MUST reside
in pure C# classes with zero direct dependencies on Stardew Valley or
SMAPI types. Interaction with game state MUST occur through
project-owned interfaces (e.g., `IGameStateProvider`,
`INotificationService`).

- Domain models MUST be plain C# objects — no inheritance from game
  classes.
- Services MUST accept interfaces, never concrete SMAPI or game types.
- The SMAPI entry-point class (`ModEntry`) MUST be a thin adapter that
  wires interfaces to game implementations and forwards SMAPI events
  to core logic.

**Rationale**: This is the foundational enabler for out-of-game
testing. If core logic touches game assemblies, tests require a
running game instance and become slow, brittle, and
non-deterministic.

### II. Test-First

TDD is mandatory for all core logic.

- Tests MUST be written before implementation code.
- Red → Green → Refactor cycle MUST be followed.
- All tests MUST execute without Stardew Valley, SMAPI, or any game
  assemblies on the test runner.
- Test projects MUST reference only the core library and test
  framework packages — never game DLLs.
- Mocks or fakes of game-facing interfaces MUST be maintained in a
  shared test-utilities project or folder.

**Rationale**: Out-of-game testability is a stated project
requirement. Tests that need the game running are integration tests,
not unit tests, and MUST be clearly separated.

### III. Thin SMAPI Integration Layer

The SMAPI-facing layer MUST be as small as possible.

- `ModEntry` MUST only: register event handlers, read/write config,
  and delegate to core services.
- SMAPI event handlers MUST map event args to domain types and call
  core service methods — no business logic inline.
- Configuration MUST be read via SMAPI's `IModHelper` and mapped to a
  plain settings POCO consumed by core services.

**Rationale**: Keeping the integration layer thin minimises untestable
code and reduces coupling to SMAPI version changes.

### IV. Observability

All runtime behaviour MUST be observable through structured logging.

- Core services MUST accept an `ILogger` (or equivalent project
  interface) — never call `IMonitor` directly.
- The SMAPI adapter MUST implement `ILogger` by forwarding to
  `IMonitor`.
- Log messages MUST include context (e.g., player name, in-game date,
  event type) to support debugging without a debugger attached.
- Tests MUST be able to capture and assert on log output via a
  test-double `ILogger`.

**Rationale**: Mod debugging often happens via log files shared by
users. Structured, context-rich logs drastically reduce diagnosis
time.

### V. Simplicity & YAGNI

Start with the simplest design that satisfies current requirements.

- No speculative abstractions — add indirection only when a second
  concrete use case demands it.
- Prefer fewer projects/assemblies; split only when a clear dependency
  boundary justifies it (e.g., core vs. SMAPI adapter vs. tests).
- Avoid third-party NuGet packages unless they solve a problem that
  would otherwise require significant custom code; each dependency
  MUST be justified.
- Configuration options MUST default to sensible values so the mod
  works with zero user setup.

**Rationale**: Mods are community software with long maintenance
tails. Complexity that is not immediately needed becomes a maintenance
burden.

## Technology Stack

- **Language**: C# (target the .NET version required by the current
  stable SMAPI release).
- **Mod Framework**: SMAPI (Stardew Modding API).
- **Testing Framework**: xUnit (preferred) or NUnit; assertion library
  at developer discretion (e.g., FluentAssertions).
- **Mocking**: Moq or NSubstitute for interface mocking in unit tests.
- **Build**: `dotnet` CLI and MSBuild; solution file at repository
  root.
- **Packaging**: Standard SMAPI mod folder layout (`manifest.json` +
  compiled DLL).
- **IDE**: Visual Studio or VS Code with C# Dev Kit; no IDE-specific
  build steps that prevent CLI builds.

## Development Workflow

- **Branching**: Feature branches off `main`; short-lived, merged via
  pull request.
- **Commit Messages**: Conventional Commits format
  (`feat:`, `fix:`, `test:`, `docs:`, `chore:`).
- **Quality Gates**:
  - All unit tests MUST pass before merge.
  - No warnings treated as errors in Release configuration.
  - New public APIs MUST have XML-doc comments.
- **Manual Play-Testing**: After unit tests pass, the mod SHOULD be
  loaded in-game to verify integration. Play-test findings MUST be
  documented as issues or inline TODOs, not left untracked.

## Governance

This constitution is the authoritative source of project standards. It
supersedes ad-hoc decisions and informal practices.

- **Amendments**: Any change to this document MUST include a rationale,
  an updated version number, and review of dependent templates for
  consistency.
- **Versioning**: MAJOR.MINOR.PATCH semantic versioning.
  - MAJOR: Principle removed or fundamentally redefined.
  - MINOR: New principle or section added, or material expansion.
  - PATCH: Clarifications, typo fixes, non-semantic refinements.
- **Compliance**: All pull requests and code reviews MUST verify
  adherence to these principles. Violations MUST be resolved or
  formally exempted with documented justification in a Complexity
  Tracking table (see plan template).

**Version**: 1.0.0 | **Ratified**: 2026-02-15 | **Last Amended**: 2026-02-15
