# Velocity Rush — Asset Sourcing Plan & License Record

**Step 5 artifact.** Every asset the game will ship with is listed here with its source, license and
destination. Nothing enters `Assets/` without a row in this file.

---

## 1. Sourcing policy

| Rule | Why |
|---|---|
| **Free / CC0 only** — no paid assets, no pirated assets | Legal safety; the original RCC (paid controller) was rejected outright rather than pirated |
| **Zero assets from the WITS Gaming playlist** | Its description links go to Google Drive + Patreon behind a paywall and carry no clear license. The playlist is *genre reference only* — we read the titles, never the descriptions |
| **Unity Asset Store not used** | Its licence is the Unity Asset Store EULA, not CC0, and free-store packs are inconsistent quality. Rejected deliberately, not overlooked |
| **Prefer one visual family** | Kenney (all CC0) gives a consistent low-poly look across cars, roads and buildings; mixing vendors is the usual cause of a project looking assembled rather than designed |
| **Generate what we can** | VFX textures and engine loops are procedural (Steps 15/16) — zero licence surface and exact control |

**Licence shortlist:** CC0 1.0 (<https://creativecommons.org/publicdomain/zero/1.0/>) — public
domain, no attribution required (we credit Kenney anyway; it is the right thing to do).

---

## 2. Status

| Category | Sourced | Missing |
|---|---|---|
| Cars | ✅ 14 driveable vehicles + wheels | — |
| Track / roads | ✅ 95 modular road pieces | 2 more road themes (Step 20 decides) |
| Environment | ✅ 447 buildings/props/nature FBX + asphalt PBR | night/coastal HDRI variants |
| Sky / lighting | ✅ 1 clear-sky HDRI | 2 more, one per `EnvironmentTheme` |
| Audio — UI/SFX | ✅ 431 OGG clips | — |
| Audio — engine loops | ❌ | **generate in-house (Step 15)** |
| VFX textures | ❌ | **generate in-house (Step 16)** |
| Fonts | ✅ 12 TTF | — |
| Logo / car thumbnails | ❌ | **render from imported models (Step 6+)** |

**Downloaded so far: 12 Kenney packs (33.1 MB of archives) + 2 Poly Haven assets (25.5 MB),
5,466 extracted files. 0 failures.**

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
| `car-kit` | 3.1 | 259 | 50 | **Cars** — 14 vehicles + separate wheel meshes + debris | Step 6 |
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

**Vehicle list** (all in `car-kit`, each a separate FBX):
`sedan`, `sedan-sports`, `suv`, `suv-luxury`, `hatchback-sports`, `taxi`, `police`, `race`,
`race-future`, `van`, `delivery`, `box`, `truck`, `firetruck`, `ambulance`, `garbage-truck`,
plus six `kart-*` racers — and **9 separate wheel meshes** (`wheel-default`, `wheel-dark`,
`wheel-racing`, `wheel-truck`, tractor variants).

That wheel separation satisfies the plan's spec directly: *"4 wheels alag objects"*.

### 3.2 Poly Haven — <https://polyhaven.com> — **CC0**

| Asset | Type | Maps | Size | Used for | Import step |
|---|---|---|---|---|---|
| `asphalt_pit_lane` | PBR texture, 2K | Diffuse, Normal(GL), Rough, AO | 12.7 MB | **Track** racing surface | Step 8 / 21 |
| `kloofendal_43d_clear_puresky` | HDRI sky | HDR (2K) + EXR (2K) | 12.8 MB | **Environment** ambient light + skybox, daylight theme | Step 8 / 22 |

Both verified by **magic bytes** after download — the four textures came down as EXR on the first
attempt (the API exposes `jpg`, `png` and `exr` for the same map and the wrong branch won) and were
caught by a header check before ever reaching `Assets/`. They are now genuine `FF D8 FF` JPEGs.

### 3.3 Considered and rejected

| Source | Reason rejected |
|---|---|
| WITS Gaming playlist description links | Paywall + no licence |
| Real Asset Store "free" car packs | Unity EULA, not CC0; inconsistent quality |
| Branded car models (BMW, Ferrari …) | Trademark risk — this project uses generic vehicles only |
| RCC (Racing Car Controller) | Paid; a pirated copy was explicitly refused |
| ambientCG asphalt | Kept as backup only — Poly Haven's `pit_lane` suits racing better |

---

## 4. Requirements each source must satisfy (from `PLAN.md`)

**Cars** — ≤ 30k tris each, 4 wheels as separate objects, pivot at chassis centre, Y-up, metric.
**Track** — modular: straights, curves, T-junctions, banks.
**Environment** — buildings, barriers, street lights, trees, cones.
**VFX** — tyre smoke, skid decal, boost trail, speed lines.
**Audio** — engine loops (idle/low/mid/high per car), tyre skid, boost, UI clicks, 2–3 music tracks.
**UI** — SDF font for TextMeshPro, icons, car thumbnails, logo.

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

**No item on this list blocks Steps 6–14.**

---

## 6. Staging → import map

Originals live in `_SourceAssets/` (**gitignored** — `git check-ignore` verified). Only assets that
actually ship are copied into `Assets/_Project/`, so the repository never carries 5,466 unused
source files or three redundant formats per model.

```
_SourceAssets/<pack>/          ← untouched originals (FBX + OBJ + GLB + PNG + License.txt)
        │
        ├─ car-kit            → Assets/_Project/Art/Models/Cars/
        ├─ city-kit-*         → Assets/_Project/Art/Models/Buildings/,  /Track/
        ├─ nature-kit         → Assets/_Project/Art/Models/Nature/
        ├─ polyhaven/*.jpg    → Assets/_Project/Art/Textures/Road/
        ├─ polyhaven/*.hdr    → Assets/_Project/Settings/ (or Art/Textures/HDRI)
        ├─ kenney-fonts       → Assets/_Project/Fonts/
        └─ *-sounds / audio   → Assets/_Project/Audio/SFX/,  /Music/
```

Only the **FBX** of each model is imported — `.obj` and `.glb` duplicates stay in staging.

**Never** write anything into `Assets/` that is not on this map.

---

## 7. Download log

All fetched **2026-10-26** directly from the vendor's own CDN; no mirrors, no third-party re-uploads.

| Source | Endpoint |
|---|---|
| Kenney pack index | `https://kenney.nl/assets/<slug>` → ZIP URL scraped from page |
| Poly Haven files API | `https://api.polyhaven.com/files/<slug>` |
| Poly Haven CDN | `https://dl.polyhaven.org/file/ph-assets/…` |

**Verification performed on every download:** HTTP 200, non-zero size, `PK` magic bytes for all 12
ZIPs, successful extraction, `License.txt` present in each archive, and JPEG/HDR/EXR header checks
on the Poly Haven files.
