# Velocity Rush — Asset Sourcing Plan & License Record

**Step 5 artifact (revised in the Step 6 rework).** Every asset the game will ship with is listed
here with its source, license and destination. Nothing enters `Assets/` without a row in this file.

---

## 1. Sourcing policy

| Rule | Why |
|---|---|
| **Free / CC0 only** — no paid assets, no pirated assets | Legal safety. **RCC (paid controller):** a pirated copy was refused earlier. The three `.unitypackage` versions on the desktop were then explained by the user as supplied by the studio that commissioned a game from them. Provenance recorded as stated — see §3.4 |
| **Zero assets from the WITS Gaming playlist** | Its description links go to Google Drive + Patreon behind a paywall and carry no clear license. The playlist is *genre reference only* — we read the titles, never the descriptions |
| **Unity Asset Store not used** | Its licence is the Unity Asset Store EULA, not CC0, and free-store packs are inconsistent quality. Rejected deliberately, not overlooked |
| **One family per domain, never mixed inside a domain** | The world (track, buildings, nature) is entirely Kenney `city-kit`/`nature-kit` — one low-poly family, so nothing in the scenery clashes. Cars were originally Kenney too, but the low-poly toy look was rejected for the driving roster, so driveable vehicles now come from a second CC0 source (Grab3D, realistic PBR). Mixing vendors *within* a domain is what produces the "assembled, not designed" look — this split is domain-level and deliberate |
| **Generate what we can** | VFX textures and engine loops are procedural (Steps 15/16) — zero licence surface and exact control |

**Licence shortlist:** CC0 1.0 (<https://creativecommons.org/publicdomain/zero/1.0/>) — public
domain, no attribution required (we credit Kenney anyway; it is the right thing to do).

---

## 2. Status

| Category | Sourced | Missing |
|---|---|---|
| Cars | ✅ **6 realistic CC0 vehicles** (Grab3D, PBR) — 6 prefabs built, verified | — |
| Track / roads | ✅ 95 modular road pieces | 2 more road themes (Step 20 decides) |
| Environment | ✅ 447 buildings/props/nature FBX + asphalt PBR | night/coastal HDRI variants |
| Sky / lighting | ✅ 1 clear-sky HDRI | 2 more, one per `EnvironmentTheme` |
| Audio — UI/SFX | ✅ 431 OGG clips | — |
| Audio — engine loops | ❌ | **generate in-house (Step 15)** |
| VFX textures | ❌ | **generate in-house (Step 16)** |
| Fonts | ✅ 12 TTF | — |
| Logo / car thumbnails | ❌ | **render from imported models (Step 6+)** |
| Traffic vehicles (Step 24) | ⚠️ Kenney `car-kit` staged only, not imported | decide at Step 24 |

**Downloaded so far: 12 Kenney packs (33.1 MB of archives) + 2 Poly Haven assets (25.5 MB),
5,466 extracted files, 0 failures — plus 6 Grab3D car models (FBX + 34 PBR textures + a
per-car `LICENSE.txt` and `manifest.json`), 0 download failures, 0 missing texture references.**

---

## 3. License table

### 3.1 Kenney — <https://kenney.nl> — **CC0**

Every pack below shipped with a `License.txt` inside the archive containing:

> `License: (Creative Commons Zero, CC0)` — `You can use this content for personal, educational,
> and commercial purposes.` — `Support by crediting 'Kenney' or 'www.kenney.nl' (this is not a
> requirement)`

Verified by reading the file from each extracted archive, not by trusting the website.

| Pack | Version | Files | FBX | Used for | Import step |
|---|---|---|---|---|---|
| `car-kit` | 3.1 | 259 | 50 | ~~**Cars**~~ **removed from `Assets/`** — the low-poly toy look was rejected for the driving roster. Archive still staged; kept as a candidate source for AI traffic vehicles | Step 6 (revised) |
| `city-kit-roads` | — | 487 | 95 | **Track** — straights, bends, crossings, bridges, barriers, cones, street lights, poles | Step 7 / 21 |
| `city-kit-commercial` | 2.1 | 219 | 41 | **Environment** — shops, towers, signage | Step 7 / 22 |
| `city-kit-suburban` | 2.0 | 216 | 40 | **Environment** — houses, fences, gardens | Step 7 / 22 |
| `city-kit-industrial` | 2.0 | 201 | 37 | **Environment** — warehouses, silos, pipes (industrial theme) | Step 7 / 22 |
| `nature-kit` | — | 3,618 | 329 | **Environment** — trees, rocks, grass, terrain props | Step 7 / 22 |
| `kenney-fonts` | — | 13 | — | **UI** — 12 TTF, SDF-ified for TextMeshPro | Step 34 |
| `interface-sounds` | — | 103 | — | **UI** clicks, hovers, confirms | Step 35 |
| `ui-audio` | — | 55 | — | **UI** extra blips | Step 35 |
| `impact-sounds` | — | 133 | — | **SFX** collisions, scrapes, kerb strikes | Step 16 |
| `digital-audio` | — | 67 | — | **SFX** boost, whoosh, transitions | Step 16 |
| `music-jingles` | — | 89 | — | **Music** menu / result stingers | Step 36 |

**Vehicle list (staged only, none imported):** `sedan`, `sedan-sports`, `suv`, `suv-luxury`,
`hatchback-sports`, `taxi`, `police`, `race`, `race-future`, `van`, `delivery`, `box`, `truck`,
`firetruck`, `ambulance`, `garbage-truck`, six `kart-*` racers, and 9 separate wheel meshes
(`wheel-default`, `wheel-dark`, `wheel-racing`, `wheel-truck`, tractor variants).

All 26 Kenney entries were deleted from `Assets/` in the Step 6 rework. The recoverable copies are
`_SourceAssets/car-kit.zip` (staged, gitignored) and git commit `edbf140`.

### 3.2 Grab3D — <https://grab3d.com> — **CC0 1.0**

Source of the **6 driveable vehicles**. Chosen after the Kenney roster was rejected as too
toy-like: these are realistic low-poly cars with full PBR maps at a vertex budget that suits mobile.

Each model was fetched as four files from the vendor's own CDN (`https://grab3d.com/dl/<slug>/…`):
`<slug>.fbx`, `<slug>-obj.zip` (holds the textures), `LICENSE.txt`, `manifest.json`.

The per-model `LICENSE.txt` reads:

> `Creative Commons CC0 1.0 Universal — To the extent possible under law, the authors have waived
> all copyright and related or neighboring rights to this work.`

Read from **each of the six** downloaded files, not inferred from the site.

| Slug | Prefab | Size (W×H×L, m) | Verts | Wheelbase | Notes |
|---|---|---|---|---|---|
| `rally-car` | `Car_Rally` | 1.90 × 1.69 × 4.39 | 16,664 | 2.52 m | boxy rally hatch |
| `muscle-car` | `Car_Muscle` | 1.92 × 1.40 × 4.68 | 16,769 | 2.86 m | longest wheelbase of the road cars |
| `1990s-sports-car` | `Car_Sports1990` | 1.88 × 1.23 × 4.42 | 14,596 | 2.54 m | wedge 90s coupe |
| `1980s-sports-car` | `Car_Sports1980` | 1.83 × 1.23 × 4.42 | 16,079 | 2.52 m | pop-up-light 80s coupe |
| `dragster` | `Car_Dragster` | 1.85 × 1.92 × 5.90 | 13,019 | 4.49 m | narrow 0.68 m front track, no windows |
| `dune-buggy` | `Car_DuneBuggy` | 2.02 × 1.51 × 3.28 | 12,183 | 2.04 m | shortest, 7 renderers (steering wheel) |

**Specification (from `manifest.json`, cross-checked against measured bounds):**

- Wheels are **four separate objects**, pivot on the hub, roll by rotating about local X.
- Front faces **−Y** in the source file — Unity imports it as **+Z**, which is Unity's forward.
- Textures are 2048 px PBR: base colour, normal (baked), metallic, roughness. `ai_content: false`.
- Every FBX's texture references were parsed and matched against the extracted files:
  **0 missing across all 6 cars** (dragster ships 4 maps, the others 6).

**Materials built (18):** `M_<Car>_Body`, `M_<Car>_Windows`, `M_<Car>_Tire` per car —
URP **Universal Render Pipeline/Lit** with `_BaseMap` + `_BumpMap`, scalar metallic/smoothness
(body 0.10 / 0.75 for clearcoat paint, windows 0.00 / 0.92 tinted, tyres 0.00 / 0.40).
Dragster has no tyre maps, so its tyre material falls back to the body maps with a grey tint.

> **Known simplification:** the metallic and roughness maps ship as two separate greyscale JPEGs,
> which URP Lit's `_MetallicGlossMap` cannot consume (it expects R = metallic, A = smoothness).
> Packing them into one RGBA map is a recorded follow-up; scalars were used so nothing ships with a
> fake smoothness of 1.0.

### 3.3 Poly Haven — <https://polyhaven.com> — **CC0**

| Asset | Type | Maps | Size | Used for | Import step |
|---|---|---|---|---|---|
| `asphalt_pit_lane` | PBR texture, 2K | Diffuse, Normal(GL), Rough, AO | 12.7 MB | **Track** racing surface | Step 8 / 21 |
| `kloofendal_43d_clear_puresky` | HDRI sky | HDR (2K) + EXR (2K) | 12.8 MB | **Environment** ambient light + skybox, daylight theme | Step 8 / 22 |

Both verified by **magic bytes** after download — the four textures came down as EXR on the first
attempt (the API exposes `jpg`, `png` and `exr` for the same map and the wrong branch won) and were
caught by a header check before ever reaching `Assets/`. They are now genuine `FF D8 FF` JPEGs.

### 3.4 Considered and rejected / deferred

| Source | Status |
|---|---|
| WITS Gaming playlist description links | **Rejected** — paywall + no licence |
| Real Asset Store "free" car packs | **Rejected** — Unity EULA, not CC0; inconsistent quality. They also require an account login, which the sourcing policy avoids |
| Branded car models (BMW, Ferrari …) | **Rejected** — trademark risk; this project uses generic vehicles only |
| Google Drive folder "Assets Mobile Car Racing 2024" | **Rejected** — it repackages paid store assets (`RaceTrack Pack 01` by HereVR ≈ $60, `Garage` ≈ Pit Garage Pack ≈ $50) with no licence. Same piracy line the project already drew |
| RCC (Racing Car Controller) | **Available, not yet imported.** A pirated copy was refused earlier; the user then explained that the three desktop `.unitypackage` versions (v3.3 / v3.70 / v4.4.0) were supplied by the studio that commissioned a game from them. Provenance recorded as stated. Note that a loose package does not itself evidence a licence — if the asset is attached to a Unity account this project can use, Package Manager → My Assets is the clean import path. Architecturally it would replace ~12 custom-controller steps (9–20), so RCC-vs-custom is a **scheduled decision at Step 9**, not an assumption either way |
| ambientCG asphalt | Kept as backup only — Poly Haven's `pit_lane` suits racing better |

---

## 4. Requirements each source must satisfy (from `PLAN.md`)

**Cars** — ≤ 30k tris each, 4 wheels as separate objects, pivot at chassis centre, Y-up, metric.
**Track** — modular: straights, curves, T-junctions, banks.
**Environment** — buildings, barriers, street lights, trees, cones.
**VFX** — tyre smoke, skid decal, boost trail, speed lines.
**Audio** — engine loops (idle/low/mid/high per car), tyre skid, boost, UI clicks, 2–3 music tracks.
**UI** — SDF font for TextMeshPro, icons, car thumbnails, logo.

**Cars as delivered:** all six are metric, Y-up in Unity, ground plane at Y = 0, wheels four
separate hub-pivoted objects, 12k–17k verts each (≈ 6k–8.5k tris) — comfortably inside the 30k
budget, with headroom for Step 9's WheelColliders.

---

## 5. Missing list — with a decision for each

| # | Missing | Decision | Owner step |
|---|---|---|---|
| 1 | **Engine loops** (idle/low/mid/high per car) | **Generate in-house.** Synthesise looping WAVs from layered combustion harmonics, one set per engine archetype. No source has licence-clear, loop-seamless, RPM-matched sets — and Step 15 drives pitch from RPM anyway, so a clean synthetic base beats a mismatched sample | 15 |
| 2 | **Tyre skid loop** | Generate in-house (filtered noise + resonance) — needs to loop seamlessly and modulate with slip, which a stock sample will not do | 16 |
| 3 | **Tyre smoke / skid decal / boost / speed-line textures** | Generate procedurally as PNGs (radial gradient × value noise). Zero licence surface, exact control over alpha falloff for mobile fill-rate | 16 |
| 4 | **Logo + car thumbnails** | Render from the imported car FBX once materials exist; logo as TextMeshPro text/graphic | 6, 34 |
| 5 | **2 more HDRIs** — one per `EnvironmentTheme` | Same Poly Haven API, `puresky` set: need a dusk/coastal and a night sky | 22 |
| 6 | **2 road themes** | Decide at Step 20 once the waypoint spine exists — likely `city-kit-commercial` + industrial asphalt rather than more downloads | 20 |
| 7 | **Skybox textures** (non-HDRI fallback for mobile) | Bake from the HDRI if mobile fill-rate demands it; hold until profiling | 22 |
| 8 | **Packed metallic+smoothness maps** for the 6 cars | Pack Grab3D's two greyscale JPEGs into one RGBA PNG so URP Lit can use the real roughness; until then materials use tuned scalars | 8 |
| 9 | **RCC vs custom controller** | RCC is on hand (supplied by the commissioning studio, per the user). Decide at Step 9 whether to adopt it — it would replace ~12 custom-controller steps (9–20), but the Step 6 prefab hierarchy survives either way as the visual root. Blocks nothing before then | 9 |

**No item on this list blocks Steps 6–8.**

---

## 6. Staging → import map

Originals live in `_SourceAssets/` (**gitignored** — `git check-ignore` verified). Only assets that
actually ship are copied into `Assets/_Project/`, so the repository never carries 5,466 unused
source files or three redundant formats per model.

```
_SourceAssets/<pack>/          ← untouched originals (FBX + OBJ + GLB + PNG + License.txt)
        │
        ├─ grab3d/<slug>       → Assets/_Project/Art/Models/Cars/   (FBX + PBR JPGs)
        ├─ grab3d/tex/         → Assets/_Project/Art/Models/Cars/   (34 textures, slug-prefixed)
        ├─ car-kit             → (staged only — removed from Assets/ in the Step 6 rework)
        ├─ city-kit-*          → Assets/_Project/Art/Models/Buildings/,  /Track/
        ├─ nature-kit          → Assets/_Project/Art/Models/Nature/
        ├─ polyhaven/*.jpg     → Assets/_Project/Art/Textures/Road/
        ├─ polyhaven/*.hdr     → Assets/_Project/Settings/ (or Art/Textures/HDRI)
        ├─ kenney-fonts        → Assets/_Project/Fonts/
        └─ *-sounds / audio    → Assets/_Project/Audio/SFX/,  /Music/
```

Only the **FBX** of each model is imported — `.obj` and `.glb` duplicates stay in staging.
Grab3D textures are the one exception: the FBX references them by bare filename, so Unity can only
resolve them if they sit **beside** the `.fbx`. All 34 are slug-prefixed, so there are no filename
collisions with each other or with the road/environment textures.

**Never** write anything into `Assets/` that is not on this map.

---

## 7. Download log

All fetched **2026-10-26** directly from the vendor's own CDN; no mirrors, no third-party re-uploads.

| Source | Endpoint |
|---|---|
| Kenney pack index | `https://kenney.nl/assets/<slug>` → ZIP URL scraped from page |
| Grab3D models | `https://grab3d.com/dl/<slug>/<file>` (FBX, OBJ zip, `LICENSE.txt`, `manifest.json`) |
| Poly Haven files API | `https://api.polyhaven.com/files/<slug>` |
| Poly Haven CDN | `https://dl.polyhaven.org/file/ph-assets/…` |

**Verification performed on every download:** HTTP 200, non-zero size, `PK` magic bytes for all 12
ZIPs, successful extraction, `License.txt` present in each archive, and JPEG/HDR/EXR header checks
on the Poly Haven files.

**Verification performed on the Grab3D set:** 6 cars × 4 files = 24 requests, 0 failures; all six
`LICENSE.txt` files read and confirmed CC0 1.0; all 34 textures extracted; every texture filename
referenced by each FBX parsed out of the binary and matched against what is on disk — **0 missing**.
