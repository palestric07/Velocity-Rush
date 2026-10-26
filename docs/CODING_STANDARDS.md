# Velocity Rush — Code Standards

Living contract for every script in the project. Written at **Step 3**; following it is cheaper than
reviewing later. New code that breaks these rules gets fixed before commit.

---

## 1. Assembly Definitions

Seven assemblies, **strictly layered bottom-up**. A file may only reference assemblies *below* it.

```
VelocityRush.Utils      ← zero dependencies (extensions, object pooling, Log)
      ↑
VelocityRush.Data       ← ScriptableObject definitions
      ↑
VelocityRush.Core       ← Singleton, GameState, EventBus, GameManager, SceneLoader
      ↑
VelocityRush.Vehicle    ← CarController, WheelData, CarInput
      ↑
VelocityRush.Race       ← RaceManager, LapTracker, Checkpoint
      ↑
VelocityRush.AI         ← AIDriver, WaypointGraph, AIPersonality
      ↑
VelocityRush.UI         ← HUD, Menus, GarageUI   (also references Race directly)
```

**Hard rules**

| Rule | Why |
|---|---|
| `VelocityRush.Core` must never reference `Vehicle`, `AI`, `Race` or `UI` | It would close a cycle — Core is the layer those systems subscribe to |
| `VelocityRush.Race` must never reference `AI` | Race spawns cars through `Vehicle` types + prefab references; AI *reads* Race state, not the reverse |
| One `.asmdef` per script folder — a folder without one falls into `Assembly-CSharp`, which **no** asmdef can reference | This is why `Data/` and `Utils/` have assemblies even while empty |
| Cycle check before committing any new `.asmdef` | Unity reports a cryptic "Assembly has reference to non-existent assembly" otherwise |

> The original plan listed only five assemblies. `Data/` and `Utils/` were added because Unity gives
> every script folder without an asmdef to `Assembly-CSharp`, which the five cannot reference — the
> two extra assemblies are required, not optional. Recorded in `PLAN.md` at Step 3.

**Deciding where a new script goes:** pick the folder *before* writing the file, never move later.
`using` order in a file: `System.*` → `UnityEngine.*` → `VelocityRush.*`.

---

## 2. Naming

| Kind | Convention | Example |
|---|---|---|
| Namespaces, classes, structs, enums, interfaces | `PascalCase` | `GameManager`, `GameState` |
| Public / protected methods & properties | `PascalCase` | `SetState`, `IsRaceActive` |
| Private **non-serialized** fields | `_camelCase` | `_state`, `_hasStarted` |
| Private **serialized** fields | `m_camelCase` + `[SerializeField]` | `m_initialState` |
| Private **static** fields | `s_camelCase` | `s_instance` |
| Constants | `PascalCase` | `MainMenuSceneName`, `ProgressEpsilon` |
| Locals, parameters | `camelCase` | `sceneName`, `previous` |
| Events | `On…` (per plan) | `OnRaceStarted`, `OnLapCompleted` |
| Event raisers | `Raise…` | `RaiseLapCompleted` |

**The `m_` vs `_` split is the rule people get wrong:** `m_` marks *serialized* state (what the
Inspector edits), `_` marks *transient* private state. A non-serialized field with an `m_` prefix is
a bug in this codebase.

Always declare serialized fields `[SerializeField] private` — never `public`.

---

## 3. Logging

**Never call `UnityEngine.Debug` from game code.** Use `VelocityRush.Utils.Log`:

```csharp
Log.Info($"[GameManager] state: {previous} -> {next}");   // stripped from release builds
Log.Warning("...", this);                                 // stripped, optional click-to-select context
Log.Error("...");                                         // KEPT in release builds
```

`Log.Info` / `Log.Warning` are `[Conditional("UNITY_EDITOR")] [Conditional("DEVELOPMENT_BUILD")]`, so
in a release player the compiler removes the call **and its string interpolation** — zero cost, no
`#if` noise at the call site. `Log.Error` is intentionally unconditional.

Message format: `[Tag] description`. The tag is what makes console output greppable — keep it even
though `Log` could add it for you.

---

## 4. Components

- `[DisallowMultipleComponent]` on any `MonoBehaviour` meant to exist once per object — and on
  `Singleton<T>` itself so every singleton inherits the guarantee.
- `[RequireComponent]` when a component is physically meaningless alone (e.g. `CarController`
  requires `Rigidbody` — Step 9). Do **not** add it speculatively.
- `[DefaultExecutionOrder]` only when ordering is a real requirement (GameManager uses `-1000` so it
  is the first thing to see scene state).
- `[Tooltip]` on every serialized field — it is the only documentation a designer sees in the
  Inspector.

---

## 5. Documentation

Every **type** gets an XML `<summary>` that states its responsibility **in one line** — what it owns,
not how it works. The one-line discipline is the point: if you cannot summarise a type in a line, it
is doing too much.

Document public members only; private helpers are explained with a plain comment when the *why* is
non-obvious (see the `ReferenceEquals` note in `Singleton.OnDestroy`).

---

## 6. Architecture Rules carried from `PLAN.md`

- **No system calls `GameManager` directly — everything goes through `EventBus`.** Use
  `Raise*` helpers; they null-check so publishers never care who subscribed.
- Static event subscriptions **must** be cleared at
  `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]` — domain reload
  is disabled, so they would otherwise leak between play sessions.
- Any system that must survive scene loads derives from `Singleton<T>` with
  `m_persistAcrossScenes` on; anything scene-local leaves it off.
