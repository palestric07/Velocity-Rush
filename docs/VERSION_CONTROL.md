# Velocity Rush — Version Control

Written at **Step 4 (Version Control Hygiene)**. The rules below are what keeps this repository
usable; `PLAN.md` Step 4 tracks the step itself.

---

## 1. Line endings

The machine runs `core.autocrlf=true`, which hands Unity **CRLF** files on checkout while Unity
writes **LF** back on save — a permanent "modified but unchanged" churn on every scene and `.meta`.

`.gitattributes` therefore pins `eol=lf` explicitly for Unity YAML, source and config files. That
overrides `core.autocrlf` in the working tree, so:

- the repository stores LF (it already did — all 111 files were `i/lf` before this step),
- the working tree stays LF,
- Unity's own writes match byte-for-byte and never show as spurious diffs.

Windows-only IDE artefacts (`*.sln`, `*.slnx`, `*.csproj`) are the exception: `eol=crlf`, which is
what Visual Studio expects. They are gitignored anyway.

Verified at Step 4: adding `.gitattributes` produced **no** renormalisation diff — `git status`
reported only the new file itself.

---

## 2. Git LFS

`git-lfs 3.7.1` is installed and the `filter.lfs.*` driver is registered globally.

**Everything is routed through LFS before a single binary exists in the repository.** Assets do not
start arriving until Step 6, so this cost nothing; doing it later would require rewriting history
with `git lfs migrate`, which invalidates every existing clone and commit hash.

Tracked formats (see the LFS block at the bottom of `.gitattributes` for the authoritative list):

| Category | Extensions |
|---|---|
| 3D models | `fbx blend obj dae 3ds max ma c4d` |
| Textures | `png jpg jpeg tga psd tif exr hdr gif bmp` |
| Audio | `wav mp3 aif aiff flac` |
| Fonts | `ttf otf` |
| Native plugins | `dll so dylib a` |
| Packages | `unitypackage zip 7z` |

The seven formats named in `PLAN.md` (`fbx png tga wav mp3 unitypackage blend`) are all included;
the rest were added now because **adding an attribute later only applies to new commits** — files
already stored as plain Git objects stay plain Git objects forever.

`.unitypackage` is listed even though `.gitignore` excludes it; the attribute is inert and becomes
useful if an archive is ever force-added.

### Verified behaviour (Step 4)

A throwaway repository was created outside the project using this exact `.gitattributes`:

```
git lfs ls-files      → e2e1e9280c * test.png        (exit 0)
staged blob size      → 129 bytes  (source file was 4096)
staged content        → version https://git-lfs.github.com/spec/v1 / oid sha256:… / size 4096
object in .git/lfs    → present
SHA256(working file)  → equals pointer oid
```

So `git lfs ls-files` works, pointers are written, and the object store is populated. The temp
repository was deleted afterwards — project history contains no test artefacts.

### Cloning

```bash
git lfs install        # once per machine; registers the filter driver
git clone <repo-url>
```

Cloning without `git lfs` installed gives you pointer files instead of assets. After the first
clone, `git lfs ls-files` should be non-empty once assets exist, and `git lfs pull` fetches any
objects the clone skipped.

---

## 3. Branch strategy

| Branch | Purpose |
|---|---|
| `main` | The stable line. **Every commit on `main` compiles and has been verified** — commits are per completed step, never work-in-progress. |
| `feat/step-N-<name>` | One branch per step that touches code or assets, e.g. `feat/step-6-car-import`. Merge back to `main` only when the step's Done-When criteria pass. |
| `fix/<short-desc>` | For defects found outside a step (e.g. a QA-gate failure). |
| `release/vX.Y` | Cut only at Step 55/56 for store submission. |

Steps 1–4 are project bootstrap and were committed straight to `main`; from **Step 5 (assets)**
onward, work happens on `feat/step-N-*` branches.

**Rule:** a step is not "done" until its branch is merged and `main` is green again. The step's
evidence block in `PLAN.md` records the commit hash, so history and the plan stay in sync.

---

## 4. What is ignored

`.gitignore` is the official GitHub `Unity.gitignore` and already covers `Library/`, `Temp/`,
`Obj/`, `Build(s)/`, `Logs/`, `UserSettings/`, `*.csproj`, `*.sln`, `*.slnx`, `*.log`,
`*.unitypackage`, memory captures, Rider/VS caches and Addressables/Visual Scripting build output.

Verified at Step 4: `Library/` contains **0 tracked files**; 111 files tracked total, `.git` is
0.2 MB.

`.vscode/` is intentionally tracked — its `settings.json` shares C# formatting and Unity extension
choices across machines.

---

## 5. Routine operations

```bash
git status                          # before and after every Unity operation
git add -A && git commit -m "…"     # Unity's .meta files must travel with their assets
git lfs ls-files                    # confirm binaries are pointers
git check-attr -a <path>            # see which rules apply to a path
git ls-files --eol                  # index/worktree line endings per file
```

**Never** commit without the `.meta` for a new file or folder — an orphaned asset gets a fresh GUID
on the next machine and every reference to it breaks. Move files with `git mv` (or `Move-Item`
plus both files) so Git records a rename and the GUID survives.

---

## 6. Fresh clone checklist (Phase 0 Gate)

```bash
git clone <repo-url> <target>       # after: git lfs install
ls <target>/Assets/_Project/        # folder tree matches PLAN.md Step 1
git -C <target> lfs ls-files        # non-empty once assets exist
```

Open the clone in Unity, let it import, and confirm: **0 compile errors**, console ground truth
clean, and `Assets/` containing only `_Project/`.

> Cloning the full project into a second Unity project requires a full import and is memory-heavy
> on this machine (7.5 GB RAM). Do it with the original Editor closed, never both open.
