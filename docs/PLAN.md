# VELOCITY RUSH — Master Development Plan

> **Game:** 3D Arcade Car Racing Game (Mobile-first: Android/iOS + PC)
> **Engine:** Unity 6.3 LTS (6000.3.2f1) · URP · Input System
> **Project:** `D:\games\Velocity-Rush` (git: `main`)
> **Reference:** WITS Gaming car racing playlist (42 videos) — used ONLY to understand the game *genre*, not to copy.
> **Goal:** More than perfect. Ek step poori tarah complete hone tak agla step NAHI.

---

## HOW TO USE THIS PLAN (Protocol)

| # | Rule |
|---|---|
| 1 | **Sirf current step pe kaam.** Agla step tabhi shuru jab current step ka `✅ Done When` 100% pass ho. |
| 2 | Har step ke **Required Assets** pehle arrange karo, phir kaam shuru. Asset na ho toh step block hai — bol do, main alternate suggest karunga. |
| 3 | Har step ke end pe **verify gate** hai — main MCP se live Editor mein check karunga (scene, components, values) aur proof dunga. |
| 4 | Step complete → **git commit** (step number ke saath). Isse kabhi bhi rollback possible. |
| 5 | Agar beech mein requirement badle, plan mein **Phase/Step add** hota hai — existing steps skip nahi honge. |
| 6 | Har phase ke end ek **Phase Gate** hai — poora phase test karke sign-off. |

**Status legend:** `⬜ not started` · `🟨 in progress` · `✅ done` · `🟥 blocked`

---

## PHASE OVERVIEW

| Phase | Naam | Steps | Kyun zaroori |
|---|---|---|---|
| **0** | Foundation | 1–4 | Baaki sab isi pe khada hoga. Yahan galati = baad mein 10x kaam. |
| **1** | Assets | 5–8 | Sahi models/materials nahi toh controller bekaar. |
| **2** | 🚗 Car Controller | 9–19 | Game ka DIL. Yahan 20% extra time = game 2x acchi. |
| **3** | 🛣️ Track & Environment | 20–29 | Racing ka ground. Waypoints yahin bante hain. |
| **4** | 🤖 AI Opponents | 30–34 | Single-player racing ka asli maza. |
| **5** | 🏁 Race Flow & HUD | 35–39 | Countdown → race → lap/position tracking. |
| **6** | 🍔 Meta / Menus | 40–46 | Main menu, garage, save, game modes, pause, results. |
| **7** | 📱 Mobile | 47–51 | Touch controls, platform switch, optimization. |
| **8** | ✨ Polish & Ship | 52–56 | Juice, audio, bug bash, build, release. |

**Total: 56 steps**

---
---

# PHASE 0 — FOUNDATION

### Step 1 — Folder Architecture
- **Required Assets:** None (code + folder only)
- **Kaam:**
  - `Assets/` ke andar standard structure banao:
    ```
    _Project/
      Art/       (Models, Textures, Materials, VFX, Animations)
      Audio/     (Music, SFX, Ambience)
      Fonts/
      Prefabs/   (Car, Track, UI, Effects)
      Scenes/    (MainMenu, Garage, Race_01..N, Loading)
      Scripts/
        Core/        (GameManager, SceneLoader, EventBus, SaveSystem)
        Vehicle/     (CarController, WheelData, CarInput)
        AI/          (AIDriver, WaypointGraph, AIPersonality)
        Race/        (RaceManager, LapTracker, Checkpoint)
        UI/          (HUD, Menus, GarageUI)
        Data/        (ScriptableObject definitions)
        Utils/       (Extensions, Singleton, ObjectPool)
      Settings/  (URP, Quality — already exists)
      Texts/     (Localization ready)
    ```
  - URP `Settings/` aur `Scenes/` ko `_Project/` ke andar shift karo (meta files ke saath, `git mv` use karo taake GUID na toote).
- **Use kaise:** Sab kuch `_Project/` ke andar. Bahar sirf Unity ke default folders.
- **✅ Done When:** Folder tree bana, Unity mein koi pink/missing reference nahi, `git status` clean after commit.

> **✅ STEP 1 COMPLETE** — `commit e967c47 chore(step-1): project folder architecture`
> - 25 folders created under `Assets/_Project/` (Art{Models,Textures,Materials,VFX,Animations},
>   Audio{Music,SFX}, Fonts, Input, Prefabs{Cars,Track,UI,Effects}, Scenes{Menus,Tracks},
>   Scripts{Core,Vehicle,AI,Race,UI,Data,Utils}, Settings, Texts)
> - Moves: `Scenes/`, `Settings/`, `InputSystem_Actions.inputactions` → `_Project/` **with .meta**
>   (git rename detection reports `R` + **0 content changes** = GUIDs preserved; Input actions GUID
>   `052faaac586de48259a63d0c4782560b` unchanged, SampleScene GUID `99c9720ab356a0642a771bea13969a05` unchanged)
> - `EditorBuildSettings` scene path repaired → `Assets/_Project/Scenes/SampleScene.unity` (guid + path both correct on disk)
> - Verified: **0 missing metas**, console ground truth **0 errors / 0 warnings / `compilationFailed: false`**,
>   hierarchy intact (`Main Camera` + `UniversalAdditionalCameraData`, `Directional Light` +
>   `UniversalAdditionalLightData`, `Global Volume` + `Volume`), scene renders correctly (no magenta)
> - **Cleaned up:** an empty `Assets/Screenshots/` folder created accidentally by the
>   `scene_view` screenshot `save_path` was left locked. Root cause found: the Unity CLI MCP
>   **bridge process** (`unity.exe mcp`) spawned at that moment kept an open directory handle.
>   Explorer / Windows Search / Unity's own asset watcher were all ruled out by controlled tests.
>   Killed the orphaned bridge PID and deleted folder + `.meta`. **Lesson:** never point
>   `scene_view` `save_path` inside `Assets/` — it gets imported as a texture and spawns MCP
>   handles; use a Temp path instead.
> - Working tree clean, `Assets/` root contains only `_Project/` + Unity template files.

### Step 2 — Core Scaffolding (GameManager + Scene Flow + EventBus)
- **Required Assets:** None
- **Kaam:**
  - `Singleton<T>` base (DontDestroyOnLoad optional)
  - `GameManager` — game state enum: `Boot, MainMenu, Garage, Loading, Countdown, Racing, Paused, Finished`
  - `SceneLoader` — async `SceneManager.LoadSceneAsync` + **loading progress** (10% budget yaad rakho)
  - `EventBus` — static C# events ya `ScriptableObject`-based channel (decoupling ke liye): `OnRaceStarted`, `OnLapCompleted`, `OnCarSelected`, `OnCountdownTick`
  - Custom `UnityEvent` wrappers nahi — simple `Action` + null-check helper
- **Use kaise:** Koi bhi system GameManager ko directly call NAHI karega — sirf EventBus se. Isse steps 35+ mein judna aasan hoga.
- **✅ Done When:** Empty scene se `MainMenu` load hota hai, console mein state transitions print hote hain, no null refs.

> **✅ STEP 2 COMPLETE** — `commit 46ed0a9 chore(step-2): core scaffolding (GameManager + scene flow + EventBus)`
> - **5 files** in `Assets/_Project/Scripts/Core/`: `Singleton.cs`, `GameState.cs`, `EventBus.cs`,
>   `GameManager.cs`, `SceneLoader.cs` (+ `.meta`)
> - `Singleton<T>` — one live instance per session, `Instance` / `HasInstance` / `EnsureExists` /
>   `ClearInstance`, `m_persistAcrossScenes` → `DontDestroyOnLoad` (optional, as planned). Duplicate
>   instances self-destruct. `OnDestroy` uses `ReferenceEquals` (Unity's `==` already reads null there).
> - `GameState` — all 8 planned members: `Boot, MainMenu, Garage, Loading, Countdown, Racing, Paused, Finished`
> - `EventBus` — static `Action` events + null-safe `Raise*` helpers (no `UnityEvent` wrappers):
>   `OnStateChanged`, `OnRaceStarted`, `OnLapCompleted`, `OnCarSelected`, `OnCountdownTick`,
>   `OnSceneLoaded`
> - `SceneLoader` — `LoadSceneAsync` with **`m_activationBudget = 0.1f`** (Unity only streams to 0.9,
>   the last 10% is activation — the plan's "10% budget"), `m_minimumLoadSeconds` anti-flash,
>   `IsLoading` / `Progress` / `PendingScene`, refuses loads while one is in flight
> - `GameManager` — `[DefaultExecutionOrder(-1000)]`, boot flow `Boot → Loading → MainMenu`,
>   scene→state mapping arrives through `EventBus.OnSceneLoaded` (future systems still never call
>   `GameManager` directly — that contract holds)
> - **Empty scene works:** `GameManager.Bootstrap()` (`RuntimeInitializeOnLoadMethod.AfterSceneLoad`)
>   spawns a `[Core]` host with `SceneLoader` + `GameManager`, so pressing Play in a zero-setup scene
>   boots straight into `MainMenu`
> - `Assets/_Project/Scenes/Menus/MainMenu.unity` created (guid `b402708be0693204f95398c40a3be9ad`)
>   and added to Build Settings at index 1; `SampleScene` path/GUID untouched
> - **Verified end-to-end in play mode, no manual intervention** — 5 logs / **0 errors / 0 warnings**:
>   `run-in-background enabled at frame 0` → `initial state: Boot` → `state: Boot -> Loading` →
>   `state: Loading -> MainMenu` → `loaded 'MainMenu'`. The middle transition's stack proves the whole
>   decoupling chain: `SceneManager.Internal_SceneLoaded` → `SceneLoader.HandleSceneLoaded:126` →
>   `EventBus.RaiseSceneLoaded:61` → `GameManager.HandleSceneLoaded:91` → `SetState:80`.
>   End state: `gmState=MainMenu activeScene=MainMenu sceneCount=1 isLoading=False progress=1`.
>   Console ground truth before/after: `compilationFailed: false`, 0 errors, 0 warnings.
> - **Two real gotchas found and fixed:**
>   1. **Editor background freeze.** With `PlayerSettings.runInBackground=false`, Unity froze the play
>      loop at **frame 2** whenever the Editor was unfocused — every coroutine and async load stalled
>      (`frameCount` and `Time.unscaledTime` frozen, only `realtimeSinceStartup` moved). This is *not*
>      a code bug but it would silently break **every future automated play test**, so it is fixed now:
>      `ConfigureEditorBackgroundRun()` at `SubsystemRegistration` sets `Application.runInBackground = true`
>      under `#if UNITY_EDITOR` and logs its `frameCount` — measured **frame 0**, so it always wins the
>      race against the frame-2 freeze. Proven by `runInBackground` reading **False outside play** and
>      **True inside play** (i.e. our code, not a persisted project setting).
>   2. **`EditorBuildSettings` never reached disk.** `EditorBuildSettings.scenes = …` and MCP
>      `add_to_build` both reported `count: 2` in memory while the file on disk still held only 1 scene
>      — it would have vanished on the next domain reload/quit. Fixed by reassigning the array plus
>      `AssetDatabase.SaveAssets()`, then **re-reading the file** (diff shows only the 3 added lines).
>      **Lesson: after touching Build Settings, always verify the file on disk, not just the API.**
> - Static-state safety under disabled domain reload: `EventBus` and `Singleton` both reset at
>   `SubsystemRegistration`, so no stale subscriptions leak between play sessions.
> - Working tree clean after both commits.

### Step 3 — Code Standards + Assembly Definitions
- **Required Assets:** None
- **Kaam:**
  - `.asmdef` files: `VelocityRush.Core`, `.Vehicle`, `.AI`, `.Race`, `.UI`
  - Naming: `PascalCase` types, `_camelCase` private fields, `m_` serialized, `[SerializeField] private`
  - Har system ke upar XML comment: **responsibility ek line mein**
  - `[DisallowMultipleComponent]`, `[RequireComponent]` use karo
  - Debug logs sirf `#if UNITY_EDITOR` ya custom `Log` wrapper mein
- **Use kaise:** Naya script banate waqt pehle decide karo kaunsi asmdef. Circular dependency forbidden.
- **✅ Done When:** Project compile hota hai, koi asmdef cycle nahi, IDE mein IntelliSense theek.

> **✅ STEP 3 COMPLETE** — `commit 8ca2e93 chore(step-3): code standards - asmdefs, naming, Log wrapper`
>
> **Amendment — 7 assemblies, not 5.** The plan listed `Core / Vehicle / AI / Race / UI`, but
> `Scripts/Data/` and `Scripts/Utils/` are script folders too. Unity dumps any script folder without
> an asmdef into `Assembly-CSharp`, and **no asmdef can reference `Assembly-CSharp`** — so any file
> dropped into `Data/` or `Utils/` later would have been invisible to the five. Both got assemblies;
> the folder tree from Step 1 is unchanged.
>
> - **`.asmdef` × 7**, strict bottom-up layering:
>   `Utils → Data → Core → Vehicle → Race → AI`, with `UI` on top of `Race`.
>   Verified by DFS over the parsed references: **no cycles, every reference resolves**.
>   Guard rails: `Core` may never reference `Vehicle`/`AI`/`Race`/`UI`; `Race` may never reference `AI`
>   (Race spawns cars via `Vehicle` types + prefab refs, AI *reads* Race state).
> - **`Assembly-CSharp` = ABSENT** — every script in the project now lives inside an asmdef.
> - **`VelocityRush.Utils.Log`** — the plan's *"Debug logs sirf `#if UNITY_EDITOR` ya custom `Log`
>   wrapper mein"*. `Info`/`Warning` carry `[Conditional("UNITY_EDITOR")]
>   [Conditional("DEVELOPMENT_BUILD")]`, so release builds delete the call **and** its string
>   interpolation — stronger than `#if` at the call site, which would still pay for the interpolation.
>   `Error` stays unconditional on purpose. Optional `Object context` keeps Console click-to-select.
>   Enforced by scan: **0 direct `Debug.*` calls outside `Log.cs`.**
> - **Naming split made real:** `m_` = serialized, `_` = transient private, `s_` = static,
>   `PascalCase` = const/type/method. `GameManager` had two non-serialized fields wrongly using
>   `m_` (`m_state`, `m_hasStarted`) — renamed `_state`, `_hasStarted`.
>   Automated checker (walks up through attributes only, stops at blank/declarations): **7 fields, 0 violations.**
> - `[DisallowMultipleComponent]` moved onto `Singleton<T>` itself so **every** future singleton
>   inherits the guarantee, not just the two that spelled it out; `GameManager`/`SceneLoader` keep it
>   explicitly. `[RequireComponent]` deliberately **not** used yet — nothing here is meaningless
>   alone; it lands with `CarController` (Step 9).
> - All **6 types** carry a one-line XML `<summary>` stating responsibility.
> - **`docs/CODING_STANDARDS.md`** added — the living contract (asmdef layering table, naming table,
>   logging rules, component attributes, architecture rules). Future steps cite it instead of
>   re-deriving conventions.
> - **Regression-tested in play mode after the refactor:** 5 logs / **0 errors / 0 warnings**,
>   `compilationFailed: false`. Stack traces now route through `VelocityRush.Utils.Log:Info (Log.cs:30)`
>   proving the wrapper is genuinely in use, and the full chain still runs:
>   `frame 0` → `Boot` → `Boot -> Loading` → `Loading -> MainMenu` → `loaded 'MainMenu'`.
> - Working tree clean after commit.

### Step 4 — Version Control Hygiene
- **Required Assets:** None
- **Kaam:**
  - `.gitignore` verify (Library/, Temp/, Obj/, Logs/, *.csproj, *.sln — lekin `.slnx` decide karo)
  - **Git LFS** setup for `*.fbx, *.png, *.tga, *.wav, *.mp3, *.unitypackage, *.blend`
  - `.gitattributes` line endings
  - Branch strategy: `main` (stable) + `feat/step-N-name`
- **Use kaise:** `git lfs install` → `.gitattributes` edit → commit.
- **✅ Done When:** `git lfs ls-files` kaam karta hai, fresh clone se project theek khulta hai.

**🚦 PHASE 0 GATE:** Fresh clone → Unity open → compile 0 errors → folder structure correct → 4 commits.

---
---

# PHASE 1 — ASSETS

### Step 5 — Asset Sourcing Plan
- **Required Assets:** (yehi step assets shortlist karna hai)
- **Kaam:**
  - **Cars:** 6–10 vehicles (starter free + unlockables). Source: Unity Asset Store, Kenney (free), Sketchfab CC, ya TurboSquid. Specs: ≤ 30k tris/car, **4 wheels alag objects**, pivot at chassis center, Y-up, meter scale.
  - **Track:** Modular road kit (straights, curves, T-junctions, banks) ya terrain + decals.
  - **Environment:** City props (buildings, barriers, street lights), trees, cones.
  - **VFX:** Tire smoke (particle texture), skid decals, boost trail, speed lines.
  - **Audio:** Engine loops (idle/low/mid/high per car), tire skid, boost, UI clicks, 2–3 music tracks.
  - **UI:** Font (SDF — TextMeshPro), icons, car thumbnails, logo.
  - Har asset ka **license/source file** `docs/ASSETS.md` mein log karo.
- **Use kaise:** List banao → budget/dost decide → download → `Assets/_Project/Art/Raw/` mein rakho (import baad mein).
- **✅ Done When:** `docs/ASSETS.md` complete, har item ka source + license clear, missing list ready.

### Step 6 — Car Model Import & Setup
- **Required Assets:** Car FBX models (Step 5 se)
- **Kaam:**
  - Import settings: **Scale 1**, Mesh Compression Off (ya Medium), Read/Write OFF, Generate Colliders OFF (collider WheelCollider se aayega), Optimize Game Objects ON.
  - Wheel bones/objects ko `Wheel_FL, Wheel_FR, Wheel_RL, Wheel_RR` naam do.
  - Prefab banao: `Prefabs/Cars/Car_<Name>.prefab` with **empty child rig**:
    ```
    CarRoot (Transform)  ← yahan Rigidbody aayega
      ├─ Body (mesh)
      ├─ Wheels/ (4 visual wheel empties)
      ├─ Colliders/ (headlight/box collider optional)
      ├─ VFX/ (smoke, skid points, boost)
      ├─ Audio/ (engine, skid)
      └─ CameraAnchor
    ```
  - Material: URP **Universal Render Pipeline/Lit**, metallic/smoothness set karo, texture atlas prefer.
- **Use kaise:** Model drag → prefab stage mein rig banao → 6 cars ke liye repeat (template prefab se duplicate).
- **✅ Done When:** 6 car prefabs ready, scene mein drag karke dikhte hain, scale correct (inspector mein size meter mein).

### Step 7 — Track & Environment Asset Import
- **Required Assets:** Road kit, props, terrain textures
- **Kaam:**
  - Modular road pieces import + **pivot at connection point** verify.
  - Props ka LOD setup (LOD0/1/2) + occlusion static flags.
  - Collision meshes alag (simple convex) — render mesh se light.
- **✅ Done When:** Sab prefabs hain, scene mein 1 test track snippet fit hota hai.

### Step 8 — URP Materials & Render Standards
- **Required Assets:** Textures (albedo/normal/mask)
- **Kaam:**
  - Ek **material convention** banao: `_BaseMap`, `_BumpMap`, `_MetallicGlossMap` naming.
  - Texture importer: albedo sRGB ✅, normal map "Normal map" ✅, mask Linear ✅, compression ASTC (mobile) / DXT (PC).
  - URP asset mein **SRP Batcher ON**, HDR ON, MSAA 2x/4x, shadow distance tune.
  - Mobile URP asset (`Mobile_RPAsset`) aur PC asset (`PC_RPAsset`) — quality levels map karo.
- **✅ Done When:** Reference scene render theek, no magenta, SRP batcher compatible (Frame Debugger se verify).

**🚦 PHASE 1 GATE:** 6 cars + 1 test track + environment props ready, materials correct, `ASSETS.md` complete.

---
---

# PHASE 2 — CAR CONTROLLER (Game ka dil)

### Step 9 — Vehicle Physics Rig
- **Required Assets:** 1 car prefab (Step 6)
- **Kaam:**
  - `CarRoot` pe **Rigidbody**: Mass 1200 kg, Drag 0.05, Angular Drag 0.5, **Interpolation = Interpolate**, **Collision Detection = Continuous Dynamic**.
  - 4× `WheelCollider`: Position `±0.85, 0.35, ±1.3` (car ke hisaab se adjust), Radius 0.35, Suspension Distance 0.2.
  - WheelCollider tuning: `Suspension Spring (Spring 35000, Damper 4500)`, Forward Friction + Sideways Friction curves.
  - Center of mass **neeche** (`centerOfMass.y = -0.5`) — warna car paltegi.
  - Rigidbody position exact: 4 wheels ka base ground pe.
- **Use kaise:** Inspector se values daalo, Scene view mein wheel-ground contact visually verify.
- **✅ Done When:** Car scene mein stable khadi hai, palat nahi rahi, wheels ground touch kar rahe hain.

### Step 10 — Input System: Car Input Actions
- **Required Assets:** `InputSystem_Actions.inputactions` (already in project)
- **Kaam:**
  - Action Map `Car` banao: `Steer` (Axis, 2D vector), `Throttle` (Axis 0..1), `Brake` (Axis 0..1), `Handbrake` (Button), `LookBack` (Button), `Pause` (Button).
  - Keyboard bindings: `A/D` + arrows, `W/S`, `Space`, `C`, `Esc`.
  - **`CarInput` script** jo InputSystem se values padhe aur **raw normalized** de (deadzone 0.1).
  - Mobile ke liye bindings baad mein Step 48 mein add honge (same actions, naye controls).
- **Use kaise:** Script `PlayerInput` component se ya `InputActionAsset` reference se. Value 0..1 aur -1..1.
- **✅ Done When:** Play mode mein keys se values log hoti hain (steer -1..1, throttle 0..1).

### Step 11 — Drive Forces (Engine + Gearing)
- **Required Assets:** Engine torque curve (Texture2D ya `AnimationCurve`)
- **Kaam:**
  - `CarController.cs` — `[RequireComponent(typeof(Rigidbody))]`
  - **Engine model:**
    ```
    rpm = wheel RPM avg (gearbox ratio se scaled)
    torque = AnimationCurve.Evaluate(rpm) * throttle
    motorTorque → WheelCollider.motorTorque (rear/all wheel drive configurable)
    ```
  - **Auto gearbox** (4–6 gears): shift up at 0.85 maxRPM, shift down at 0.35. Shifting ke waqt **torque cut 0.15s** (wheel spin avoid).
  - Top speed: `WheelCollider.maxSpeed` ya drag se cap.
  - `AnimationCurve` ScriptableObject mein (`CarTuningData`) — Step 18 mein data-driven.
- **Use kaise:** Curve editor mein realistic shape (low pe high torque, high RPM pe drop).
- **✅ Done When:** Throttle dene se car accelerate karti hai, gears shift sunai dete hain (RPM curve), top speed cap hoti hai.

### Step 12 — Steering, Grip & Drift
- **Required Assets:** None
- **Kaam:**
  - `steerAngle = maxSteer (30°) * steerInput * speedFactor` — **speed se steer kam**: `speedFactor = Mathf.Lerp(1, 0.35, normalizedSpeed)`.
  - `WheelCollider.steerAngle` apply.
  - **Lateral friction tuning**: sideways friction curve ka `extremumSlip`/`slideweight` — drift control.
  - **Drift state:** handbrake ya sharp turn pe `driftFactor` → grip kam + smoke VFX + skid sound.
  - Optional: counter-steer assist (ARC-like) for mobile friendliness.
- **✅ Done When:** High speed pe car unstable nahi, low speed pe sharp turn, drift possible with smoke.

### Step 13 — Braking & Handbrake
- **Required Assets:** None
- **Kaam:**
  - `brakeTorque = brakeInput * maxBrake` on all wheels.
  - Reverse logic: brake hold at near-zero speed → reverse gear (`motorTorque` negative).
  - Handbrake: rear wheels `brakeTorque` high + friction curve softening → controlled skid.
  - **Brake bias** (front 60% / rear 40%) for realistic nose dive.
  - Anti-lock (optional toggle): wheel RPM check se brake pulse.
- **✅ Done When:** Full brake se lock nahi hota, handbrake se 180° turn possible, reverse kaam karta hai.

### Step 14 — Wheel Visuals (Spin + Steer + Suspension)
- **Required Assets:** Wheel prefabs (Step 6)
- **Kaam:**
  - `WheelVisuals` script: har frame `wheel.GetWorldPose(out pos, out rot)` → visual wheel transform pe apply.
  - Front wheels `steerAngle` se rotate, sab wheels `rpm` se spin.
  - Suspension travel visually dikhao (wheel body neeche-neeche jaata hai).
  - Brake light: `brakeInput > 0.1` → emissive material / light intensity.
  - **Yeh step "animation" ka main hissa hai** — ye procedural animation hai (no Animator needed).
- **Use kaise:** `LateUpdate` mein pose copy (taake physics ke baad).
- **✅ Done When:** Drive karte waqt wheels spin + steer + suspend karte dikh rahe hain, brake light on.

### Step 15 — Engine Audio (RPM → Pitch)
- **Required Assets:** Engine loop SFX (idle/low/mid/high), skid SFX
- **Kaam:**
  - `CarAudio` script: 2× `AudioSource` (seamless loop) crossfade based on RPM:
    - source A (low RPM loop), source B (high RPM loop), `t = inverseLerp(idleRPM, maxRPM, rpm)` → volume blend.
  - `pitch = Mathf.Lerp(0.8, 1.6, rpmNormalized)` + slight throttle modulation.
  - Doppler ON, Spatial Blend 0.7, Roll-off linear, Max Distance 40.
  - Skid: `AudioSource` loop, volume = |lateralSlip| (skid amount).
  - Boost: one-shot + pitch up.
- **✅ Done When:** Accelerate karne se engine awaaz ka pitch badhta hai, drift pe tire squeal, gear shift pe dip.

### Step 16 — VFX: Tire Smoke, Skid Marks, Boost
- **Required Assets:** Smoke texture, skid decal texture, boost VFX
- **Kaam:**
  - **Tire smoke:** ParticleSystem at rear wheels. Emission rate = `slipAmount * 100`, start speed low, gravity −0.2, lifetime 1.2s, size over lifetime curve, alpha fade. **ObjectPool** se particles (GC avoid).
  - **Skid marks:** `TrailRenderer` per wheel OR decal projection. `emitting = isSkidding`. Material: black transparent, sorting correct.
  - **Boost pad effect:** when boosting → speed lines overlay + car trail brighten + camera FOV kick.
  - **Brake light glow** already Step 14.
  - Mobile budget: max 100 particles/car, 4 cars → 400 total.
- **✅ Done When:** Drift pe smoke + black skid lines, boost pe visual pop, no GC spikes (Profiler).

### Step 17 — Camera Rig
- **Required Assets:** None (Cinemachine optional — abhi built-in custom)
- **Kaam:**
  - `CarCamera` — target car ke **piche + upar** with:
    - **Smooth follow**: `Vector3.Lerp`/`SmoothDamp` (0.12 damping)
    - **Look-ahead**: velocity-based offset (`target.position + velocity * 0.25`)
    - **Speed FOV**: `FOV = Lerp(60, 78, speed01)` — speed ka feel
    - **Handbrake/look-back**: hold `LookBack` → rotate 180° smoothly
    - **Wall collision**: `Physics.SphereCast` camera path pe — wall ke peeche na jaaye
  - **Shake hook** (baad mein Step 52 juice ke liye)
- **Use kaise:** Camera ek independent GameObject, `LateUpdate` mein target follow.
- **✅ Done When:** Camera smooth hai, speed pe FOV badhta hai, car peeche jaake camera clip nahi karta.

### Step 18 — Car Tuning Data (ScriptableObject)
- **Required Assets:** None
- **Kaam:**
  - `CarTuningData : ScriptableObject` fields:
    `mass, maxSpeed, acceleration, maxSteerAngle, brakeTorque, handbrakeTorque, driveType (FWD/RWD/AWD), gearCount, redlineRPM, torqueCurve, suspensionSpring/Damper, driftGrip, engineAudioSet, smokeRate, carClass`
  - `Data/Cars/Car_<Name>.asset` — 6 files.
  - `CarController` sirf data se padhe, hardcode kahi nahi.
  - **Handle graph**: `CarTuningData` se handling profile (Grip/Drift/Arcade) toggle.
- **Use kaise:** Inspector mein asset banaya → car prefab pe assign → values edit hoti rehti hain, code recompile nahi.
- **✅ Done When:** 6 alag cars ki alag feel hai (top speed/handling), ek bhi magic number code mein nahi.

### Step 19 — 🚦 Car Controller QA Gate
- **Kaam:** 
  - 6 cars scene mein chalao, har ek:
    - Acceleration smooth, gear shifts audible, top speed cap
    - Steering responsive, no spin-out at high speed
    - Braking + handbrake + reverse
    - Wheels visuals correct (spin/steer/suspension)
    - Smoke + skid marks + brake lights
    - Engine audio pitch correct
    - Camera smooth + FOV kick
  - **Profiler:** no GC Alloc spikes in Update, physics stable, 60 FPS (PC) / 30+ (mobile).
- **✅ Done When:** Checklist 100%, `git tag phase2-complete`.

**🚦 PHASE 2 GATE:** Ek complete "car playground" scene — 6 cars, drivable, tuned, VFX/audio on.

---
---

# PHASE 3 — TRACK & ENVIRONMENT

### Step 20 — Track Layout & Waypoint Spine
- **Required Assets:** Road kit (Step 7), track layout diagram (hand-drawn ya Tiled)
- **Kaam:**
  - Track design: 1 lap ~1.5–3 km, 10–15 turns (hairpin, sweeper, chicane), 1 long straight, elevation changes.
  - `Waypoint` prefab (empty + `Waypoint` script: `index`, `next[]`, `width`, `isCheckpoint`, `speedLimit`, `isBoostZone`).
  - Spine path banao (200–400 waypoints) — ye AI + respawn + camera ke liye backbone hai.
  - **Gizmos** se path draw (`OnDrawGizmosSelected` — line color by speed limit).
- **Use kaise:** Scene mein waypoints ka chain banao; duplicate + snap. Duplicate se copy-paste efficient.
- **✅ Done When:** Gizmos mein continuous path dikhta hai, loop closed, no gaps.

### Step 21 — Road Mesh Construction
- **Required Assets:** Road material, barrier meshes
- **Kaam:**
  - Method A: modular road pieces snap karo. Method B: procedural ribbon mesh from waypoints (script se) — **B better hai** (clean UVs + auto-curve).
  - Road mesh ke saath: **side walls/barriers** (collision), curbs, start line.
  - MeshCollider ya box colliders per segment (compound). Physic material: tyre grip high.
  - UV: texture tiling by world distance (road paint repeat).
- **✅ Done When:** Car track pe chal sakti hai, barriers se bounce, no mesh intersection.

### Step 22 — Environment & Props
- **Required Assets:** Buildings, trees, barriers, street lights, cones
- **Kaam:**
  - City/terrain layout around track. **Static batching** flags.
  - Detail props: barrels, cones, guard rails (destructible = Step 52 optional).
  - Skybox / fog per environment theme.
  - LOD groups + occlusion culling setup.
- **✅ Done When:** Track environment mein fit hota hai, frame rate impact minimal.

### Step 23 — Lighting & Post-Processing
- **Required Assets:** None (URP already)
- **Kaam:**
  - Directional light: soft shadows, color graded (golden hour / night theme).
  - **Light Probes** + Reflection Probes (car reflection ke liye zaroori — car paint dry nahi lagegi).
  - Post-processing profile: **Bloom** (lights), **Color Adjustments**, **Tonemapping (ACES)**, **Motion Blur** (subtle), **Vignette**.
  - Mobile: disable expensive effects via quality levels.
- **✅ Done When:** Frame looks cinematic, car paint reflective, no over-exposure.

### Step 24 — Checkpoints & Lap Counting
- **Required Assets:** Waypoints (Step 20)
- **Kaam:**
  - `Checkpoint` trigger colliders (waypoint pe `isCheckpoint=true` → BoxCollider isTrigger).
  - `CheckpointSystem`: player cheezon ko **sequence** mein cross karna zaroori (cheating rokna).
  - `LapTracker`: last checkpoint cross → lap++ → crossing start = lap complete.
  - Event: `EventBus.OnLapCompleted(lap, time)`.
- **Use kaise:** Trigger enter → `checkpoint.index == expected` ? next : wrong-order ignore.
- **✅ Done When:** Lap counter sahi count karta hai, wrong direction pe nahi badhta.

### Step 25 — Start Line, Grid & Finish
- **Required Assets:** Start line texture/material, grid positions
- **Kaam:**
  - Grid positions (staggered, 8 cars max) — `Transform[]` spawn points.
  - Start/finish trigger = lap boundary.
  - Finish: total laps cross → `OnRaceFinished`.
- **✅ Done When:** Cars grid pe spawn hoti hain, lap 3 complete pe race khatam.

### Step 26 — Speed Boost Pads
- **Required Assets:** Boost pad VFX/material
- **Kaam:**
  - Trigger collider + `BoostPad` script: `boostForce`, `boostDuration`, `cooldown`.
  - On trigger: temp `maxSpeed` multiplier + acceleration spike + audio + VFX trail.
  - UI/HUD "BOOST!" flash event.
- **✅ Done When:** Pad pe gadi accelerate, effect dikhta, cooldown kaam karta hai.

### Step 27 — Speed Breakers / Bumps
- **Required Assets:** Bump mesh (or terrain modifier)
- **Kaam:**
  - Physical bumps: `WheelCollider` suspension compress → car thoda utha.
  - Speed penalty: high speed pe bump = car unsettled (suspension wobble), low speed pe smooth.
  - Optional: `downforce` ya `pitch impulse` for feel.
- **✅ Done When:** Bump pe car react karti hai, high speed pe control loss risk.

### Step 28 — Respawn & Off-Track Recovery
- **Required Assets:** Waypoints (Step 20)
- **Kaam:**
  - **Auto-respawn:** car `out of bounds` (track distance > threshold) ya flipped (`up.y < 0.3` for 2s) → last checkpoint pe respawn with fade.
  - `RespawnEffect`: brief fade + camera snap + engine restart.
  - Manual respawn key (mobile: long-press).
  - Nearest waypoint pe align (`closest waypoint` + forward tangent).
- **✅ Done When:** Car palat ke phase mein atki nahi, 2 sec mein wapas track pe.

### Step 29 — 🚦 Track QA Gate
- **Kaam:** Pure track pe 3 laps drive, checkpoints sahi, boosts/bumps/respawn working, FPS stable, no mesh clipping.
- **✅ Done When:** Checklist pass → `git tag phase3-complete`.

**🚦 PHASE 3 GATE:** Complete drivable race track environment.

---
---

# PHASE 4 — AI OPPONENTS

### Step 30 — Waypoint Graph + Racing Line
- **Required Assets:** Waypoints (Step 20)
- **Kaam:**
  - Waypoints se **racing line** nikaalo (inner-edge bias on corners) — `racingLineOffset` per waypoint.
  - Speed data per waypoint: corner radius → `targetSpeed`.
  - **Path query API**: `WaypointGraph.GetNext(current, lookaheadDistance)` → target point.
  - Branches (alternate routes) optional for later.
- **✅ Done When:** Debug draw se racing line dikhti hai, corner pe speed limit logical.

### Step 31 — AI Driver Core
- **Required Assets:** Car prefabs (reuse), `CarController` + `CarTuningData`
- **Kaam:**
  - `AIDriver` component — **same `CarController`** sirf input alag (player input ki jagah AI compute kare).
  - Target selection: `lookahead 8–12 waypoints` → `targetPoint`.
  - Steering: **Pure Pursuit** (`steer = atan2(localTarget.x, localTarget.z)`) → normalized -1..1 → same steering path.
  - Throttle: `speed < targetSpeed ? 1 : 0`, brake if `speed > targetSpeed * 1.15`.
  - Reuse of `CarInput` interface = **player aur AI same physics** → fair gameplay.
- **Use kaise:** `IProvideInput` interface: `CarInputData { steer, throttle, brake, handbrake }`.
- **✅ Done When:** AI car track pe khud chalti hai, 3 laps complete karti hai.

### Step 32 — AI Cornering & Speed Control
- **Required Assets:** None
- **Kaam:**
  - Lookahead distance **speed-proportional** (`lookahead = 5 + speed * 0.4`) — fast pe door dekhe.
  - Steering smoothing (avoid jitter) + **counter-steer** on oversteer.
  - Braking **before** corner (pre-compute braking distance from radius).
  - Downforce/grip check — AI car spin nahi honi chahiye.
- **✅ Done When:** AI corners smooth karti hai, no wall-hitting, no spin.

### Step 33 — Overtaking & Avoidance
- **Required Assets:** None
- **Kaam:**
  - **Obstacle detection**: `Physics.Raycast` 3 directions (left/right/forward) → other cars.
  - If blocked → lateral offset (`racingLineOffset += avoidOffset`) + slow down slightly.
  - **Rubber-band (optional, toggle)**: if behind leader → grip/speed boost 5%, ahead → 3%. Subtle.
  - Collision avoidance: predict other car's path (velocity + waypoint).
  - Cars ke beech **bump physics** — no teleport, just gentle push.
- **✅ Done When:** AI 8 cars mein overtake karti hai, kam se kam 1 baar stack ban jati hai without explosion.

### Step 34 — 🚦 AI QA Gate + Personalities
- **Kaam:**
  - 3 AI personalities (Aggressive/Normal/Cautious): lookahead, brake margin, avoidance distance.
  - 8-car race test: positions change karte hain, koi wall se chipak nahi jaati, frame rate stable.
  - Verify AI resource cost (Update calls, raycasts).
- **✅ Done When:** Race entertaining lagti hai, no AI car stuck → `git tag phase4-complete`.

**🚦 PHASE 4 GATE:** Full 8-car race drivable (player + 7 AI).

---
---

# PHASE 5 — RACE FLOW & HUD

### Step 35 — Race State Machine
- **Required Assets:** GameManager (Step 2)
- **Kaam:**
  - `RaceManager` states: `Grid → Countdown → Racing → FinalLap → Finished → Results`
  - Transitions driven by EventBus, not direct references.
  - **Countdown**: 3-2-1-GO with timer coroutines, engine rev allowed, movement locked.
  - `raceTime` accumulation (stopwatch with `Time.timeSinceLevelLoad` delta).
- **✅ Done When:** Play → countdown → race → finish, state transitions logged.

### Step 36 — Position & Lap Tracking
- **Required Assets:** Waypoints + checkpoints
- **Kaam:**
  - `RacePositionTracker`: per car `(lap, checkpointIndex, progressAlongPath)`.
  - Sort → live position 1st..8th.
  - Progress = waypoint index + normalized distance to next (fractional) — taake close calls sahi rank de.
  - Event: `OnPositionChanged(car, newPos)`.
- **✅ Done When:** 8 cars ki position real-time sahi update hoti hai.

### Step 37 — HUD (Speedometer, Lap, Position, Timer)
- **Required Assets:** UI sprites/icons, font (TMP)
- **Kaam:**
  - `HUD` canvas (Screen Space Overlay, scaler: 1920×1080, match 0.5).
  - Elements:
    - **Speedometer**: circular fill (`Image.fillAmount` from speed01) + numeric KM/H + needle rotation (radial).
    - **Lap**: "LAP 1/3"
    - **Position**: big "1st/2nd/3rd..." with ordinal suffix logic.
    - **Timer**: current lap time + total.
    - **Minimap** (optional but pro): top-down dots from waypoint positions.
  - Update at 10 Hz (not every frame) for perf.
- **Use kaise:** `CanvasGroup` alpha fades. `TextMeshProUGUI` mandatory.
- **✅ Done When:** HUD sahi data dikhata hai, no per-frame string allocation (Profiler).

### Step 38 — Countdown & Start Lights Animation
- **Required Assets:** Light sprites, numbers, sounds
- **Kaam:**
  - **Start lights**: 5 lights sequentially ON (scale/pop animation, `LeanTween` style coroutine ya Animator).
  - Numbers 3→2→1→GO: **scale punch** (0→1.4→1), color change, fade out.
  - **"GO!"**: green flash + screen edge pulse + engine rev sound.
  - Camera: slight zoom-in during countdown (`FOV` dolly).
  - **Ye step ka main "animation" kaam hai.**
- **Use kaise:** Coroutine + `AnimationCurve`/`DOTween`-like manual tween (no extra asset needed).
- **✅ Done When:** Lights + numbers animation smooth, 60fps, sound sync.

### Step 39 — 🚦 Race QA Gate
- **Kaam:** Poora race loop test (grid → countdown → 3 laps → finish → positions), HUD accurate, no timing drift, edge cases (abandon, restart).
- **✅ Done When:** Pass → `git tag phase5-complete`.

**🚦 PHASE 5 GATE:** Complete playable race.

---
---

# PHASE 6 — META / MENUS

### Step 40 — Main Menu + Scene Flow
- **Required Assets:** UI art, music, font
- **Kaam:**
  - `MainMenu` scene: logo, Play, Garage, Settings, Quit.
  - **UI animations**: buttons (hover scale, click punch), panel slide-in (stagger children), logo fade-in.
  - Background: looping car showcase OR video/orbit camera.
  - Loading screen with progress bar.
- **✅ Done When:** Menu responsive, animations smooth, scene flow works.

### Step 41 — Garage & Car Selection UI
- **Required Assets:** Car models, thumbnails, garage environment
- **Kaam:**
  - `Garage` scene: 3D car preview (turntable) + stat bars (Speed/Grip/Brake) + select button.
  - Car list (scroll view), locked/unlocked states, price/level.
  - **Stat bars animate** (fill from 0 → value on select).
  - Switching car = swap prefab in preview rig, fade transition.
- **✅ Done When:** Car select karke stats change, selection persist hoti hai.

### Step 42 — Car Preview Animation (Turntable)
- **Required Assets:** Garage lighting (3-point light setup)
- **Kaam:**
  - Camera orbit around car (auto-rotate) + drag to rotate manually.
  - Entrance: car **drive-in** animation (from off-screen to platform, ease-out) OR turntable spin-in.
  - Light sweep (spotlight passing) for showroom feel.
- **✅ Done When:** Garage dekhne mein premium lage.

### Step 43 — Save System
- **Required Assets:** None
- **Kaam:**
  - `SaveSystem` — JSON to `Application.persistentDataPath/save.json`.
  - Data: selected car, unlocked cars, money/coins, settings (volume, quality), best lap times, tournament progress.
  - Versioned schema + migration. Atomic write (temp file → rename).
  - **Fallback**: PlayerPrefs for settings (lightweight).
  - Corrupt-save handling (backup + reset).
- **Use kaise:** `SaveSystem.Save(new SaveData{...})`, `SaveSystem.Load()`.
- **✅ Done When:** Restart ke baad progress intact, corrupt file test pass.

### Step 44 — Game Modes & Tournaments
- **Required Assets:** UI art
- **Kaam:**
  - Modes: **Race** (position), **Time Attack** (best lap), **Practice** (free roam), **Tournament** (series of 3 races with points).
  - Tournament: points table (10/8/6/4/2/1), standings screen, progression.
  - Mode selection → configure `RaceManager` (`raceType`, `laps`, `opponents`, `timelimit`).
- **✅ Done When:** Sab modes selectable + playable + standings correct.

### Step 45 — Pause Menu
- **Required Assets:** UI art
- **Kaam:**
  - `Esc`/button → `Time.timeScale = 0` + blur overlay (URP `DepthOfField` toggle) + Resume/Restart/Quit.
  - Input: pause action, resume action.
  - **Escape handling**: countdown ke waqt allowed, finish ke baad nahi.
- **✅ Done When:** Pause/resume clean, timeScale restore correct, no audio pitch issue (fix with `AudioListener.pause`).

### Step 46 — Results / Rewards Screen
- **Required Assets:** UI art, sounds
- **Kaam:**
  - Final standings table with **row stagger animation** (rows slide up one by one).
  - Medal/trophy pop animation for winner.
  - Rewards: coins earned → **count-up animation** (0 → 250 number roll).
  - Buttons: Retry, Next Race, Menu.
- **✅ Done When:** Results look rewarding, animations on point.

**🚦 PHASE 6 GATE:** Complete meta loop (Menu → Garage → Race → Results → Menu) with save.

---
---

# PHASE 7 — MOBILE

### Step 47 — Platform Switch (Android/iOS)
- **Required Assets:** App icon, splash screen
- **Kaam:**
  - Build Settings → Android, package name `com.velocityrush.game`, min SDK 24, target 34.
  - Texture compression: **ASTC**, mesh compression, IL2CPP + ARM64.
  - Quality: switch to `Mobile_RPAsset` on build.
  - Icons per density + splash screen.
- **✅ Done When:** Build install hota hai phone pe, khulta hai.

### Step 48 — Touch Controls
- **Required Assets:** UI icons (pedals, steering)
- **Kaam:**
  - **Control scheme** (user selectable):
    - A) Buttons: left/right steer + throttle/brake pedals
    - B) Tilt (accelerometer) + pedals
    - C) Drag steering wheel (radial)
  - Input System: add **Touch** and **Accelerometer** bindings to same action map (Step 10 mein diya tha).
  - Steering wheel UI: visual rotation mapped from input.
  - Haptics: `Handheld.Vibrate()` on collision.
  - Layout: safe area handling, thumb-reachable positions.
- **Use kaise:** `OnScreenStick`/`OnScreenButton` (Input System package) ya custom `TouchSteering`.
- **✅ Done When:** Phone pe pura race khela ja sake, controls comfortable.

### Step 49 — Mobile UI Scaling & Safe Area
- **Required Assets:** None
- **Kaam:**
  - `CanvasScaler` + `SafeAreaFitter` component (notch cutout).
  - Test on multiple aspect ratios (16:9, 19.5:9, tablet).
  - Font sizes touch-friendly (≥ 24sp), buttons ≥ 44dp.
- **✅ Done When:** Alag-alag screens pe UI intact.

### Step 50 — Performance Optimization
- **Required Assets:** None
- **Kaam:**
  - **Profiler se pehle measure karo** (CPU/GPU/Memory).
  - Static batching, GPU instancing for props, LOD on.
  - Occlusion culling bake.
  - Reduce overdraw (transparent VFX), particle budget.
  - Draw call target: **< 150**, memory < 1.2 GB.
  - Object pooling (VFX, projectiles, UI popups) — no runtime `Instantiate`.
  - Addressables (optional, later) for asset streaming.
  - Fix GC: no `string` concat in Update, no `GetComponent` per frame (cache).
- **✅ Done When:** 30+ FPS on mid-range phone (S21/A52 class), profiler clean.

### Step 51 — 🚦 Mobile QA Gate
- **Kaam:** Real device test — install, full race, controls, battery/thermal (10 min continuous), orientation, pause/resume, app switch.
- **✅ Done When:** Pass → `git tag phase7-complete`.

**🚦 PHASE 7 GATE:** Playable Android build on real device.

---
---

# PHASE 8 — POLISH & SHIP

### Step 52 — Juice & Game Feel
- **Required Assets:** SFX (whoosh, impact), VFX
- **Kaam:**
  - **Camera shake** on collision (perlin noise, magnitude by impact force).
  - **Hit-stop** (brief timeScale dip 0.05s on big impact).
  - Speed lines + chromatic aberration at high speed.
  - Button **punch scale** + click sound on all UI.
  - Car **lean/roll** on turns (visual body rotation).
  - Screen edge flash for boost.
  - `DOTween`-style tween utility (ya manual).
- **✅ Done When:** Game feel "juicy" — har action ka feedback.

### Step 53 — Music & Audio Mix
- **Required Assets:** Music tracks (menu, race, garage)
- **Kaam:**
  - `AudioManager`: crossfade menu↔race music, loop with sync.
  - **Audio Mixer** groups: Master/SFX/Music/UI + exposed volume params (settings ke liye).
  - Ducking: countdown/major events pe music volume dip.
  - Engine audio polish: crossfade curves, skid gain.
- **✅ Done When:** Volumes settings se control, koi clipping nahi.

### Step 54 — Bug Bash & QA
- **Required Assets:** None
- **Kaam:**
  - Test matrix: 6 cars × 5 tracks(agar hain) × 3 modes × PC/Android.
  - Edge cases: pause during countdown, restart mid-race, save corruption, no internet, low memory.
  - Unity **Test Framework** se unit tests (lap counting, save/load, position sort).
  - Console: **zero errors/warnings** target.
  - Accessibility: colorblind-safe UI, text scaling.
- **✅ Done When:** Bug list empty (or documented as known), tests pass.

### Step 55 — Build Config & Store Assets
- **Required Assets:** Screenshots, promo video, description, privacy policy
- **Kaam:**
  - Release build: IL2CPP, ARM64, minify, signing keystore (secure backup!), App Bundle (AAB).
  - Version increment + changelog.
  - Store listing: 8 screenshots, feature graphic, short/long description, category.
  - Privacy policy URL (required by Play Store).
- **✅ Done When:** AAB generate, install test pass, listing ready.

### Step 56 — Release 🚀
- **Kaam:** Play Console → internal testing → closed → production rollout (10% → 100%), monitoring crashlytics, feedback loop.
- **✅ Done When:** Game LIVE.

**🚦 PHASE 8 GATE:** Published game.

---
---

## QUICK REFERENCE — STEP ORDER

```
PHASE 0  1 Folder → 2 Core → 3 Standards → 4 Git
PHASE 1  5 Sourcing → 6 Cars → 7 Track assets → 8 Materials
PHASE 2  9 Physics → 10 Input → 11 Drive → 12 Steering → 13 Brake
          14 Wheel visuals → 15 Audio → 16 VFX → 17 Camera
          18 Car data → 19 QA GATE
PHASE 3  20 Waypoints → 21 Road mesh → 22 Env → 23 Lighting
          24 Checkpoints → 25 Start/Finish → 26 Boost → 27 Bumps
          28 Respawn → 29 QA GATE
PHASE 4  30 Racing line → 31 AI core → 32 Cornering → 33 Overtake → 34 QA GATE
PHASE 5  35 State machine → 36 Positions → 37 HUD → 38 Countdown anim → 39 QA GATE
PHASE 6  40 Main menu → 41 Garage → 42 Turntable → 43 Save
          44 Modes → 45 Pause → 46 Results
PHASE 7  47 Platform → 48 Touch → 49 UI scaling → 50 Optimize → 51 QA GATE
PHASE 8  52 Juice → 53 Audio mix → 54 Bug bash → 55 Build → 56 Release
```

## WHY YEH PLAN PLAYLIST SE BEHTAR HAI

| Playlist approach | Yeh plan |
|---|---|
| Linear tutorial, har video alag | **Phased + gated**, har step ka Done Criteria |
| Hardcoded values code mein | **ScriptableObject data-driven** (Step 18) |
| Player car alag, AI alag code | **Ek `IProvideInput` interface** — player & AI share physics |
| Save last mein | **Step 43** — early, versioned, atomic |
| No perf discipline | **QA gates + Profiler targets** har phase pe |
| Mobile sirf end mein | **Step 48–51 dedicated**, UI scaling built-in |
| No juice | **Step 52 dedicated** game-feel pass |
| — | **git tag per phase** for rollback |
