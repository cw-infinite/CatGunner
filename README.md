# Verdant Trail — Unity mechanical prototype

A working **Windows development build** is available at `Builds/Windows/VerdantTrail.exe`. Run it from that folder and keep the accompanying `VerdantTrail_Data`, DLLs and Mono runtime beside it. The Unity project uses **6000.3.25f1 (Unity 6.3 LTS)** and the built-in 2D renderer.

This is a playable reference-driven prototype with original generated cartoon sprite atlases and procedural terrain/effects. It reproduces the core harvesting/shooting loop; visual reconstruction is still underway. Android Build Support is not installed, so no APK exists yet. See [development status and remaining milestones](DEVELOPMENT_STATUS.md) and [art assets and generation prompts](Assets/Resources/Art/PROVENANCE.md).

## Play
- The HUD fits inside the reported safe area and keeps its portrait proportions on wider displays.
- Shooting and approach are automatic. Drag on the battlefield to override movement; WASD/arrows also work. Pausing or leaving the app clears held touch movement and unfinished inventory drags.
- Buy **Force**, **Tempo**, or **Yield** at the bottom. Dim cards and red prices indicate insufficient cash.
- **Play** unlocks at stage 2: clear a 60-second arena for crystals, then return to your grove.
- **Gun** unlocks at stage 3: buy spare guns, then drag or tap-equip them into CAT 2 and CAT 3 to build a **three-cat squad**. Each cat fires its own equipped gun. Removing a follower returns its gear to inventory.
- **DAILY** offers one crystal claim per UTC day. **TASKS** unlocks at stage 3: claim completed objectives for pass points, then claim earned crystal milestones.
- Gold badges indicate available rewards. Missions show progress bars; selecting a spare tool highlights the equipment slots that accept it. GEAR also shows your crystal balance and inventory capacity.
- Open **COLLECTION** inside GEAR to inspect four tools and buy additional guns for crystals, then equip them from inventory.
- **Skin** offers the starter plus eight costumes. Buying adds permanent, cumulative Force/Tempo/Yield bonuses; EQUIP LOOK changes appearance without changing those bonuses. All owned bonuses stay active even with the starter outfit.
- Stage **1-5** ends with a durable gold tree before the sand biome.
- The upper-left settings button opens sound and save controls. Combat continues underneath.
- The upper-left crossed-tools icon opens **DEV**: add notes or crystals, previous grove, next stage, return from a trial to the saved grove, clear targets, automatic approach and 1x/2x/5x/10x speeds.
- Settings shows save status. Valid backups or interrupted-write files can recover a damaged save; unrecoverable/newer-version files are protected from overwrite.
- Progress saves locally every ten seconds, after reward/equipment changes, on focus loss and on exit. Current stage and upgrades persist; in-stage vegetation restarts on reopening.

The automatic-approach default is a documented inference: the video proves joystick input and automatic shooting, but does not establish the exact movement-automation policy or magnet Auto button's function.

New illustrated side rails place Gun/Skin/Fish/Pet on the left and Play/Mine/Boss/Hunt on the right. Fish, Pet, Mine, Boss and Hunt are locked placeholders; their gameplay is not implemented. Upgrade prices remain banknotes. See [HUD reference update](Validation/HUD_REFERENCE_UPDATE.md) for this visual pass.

## Analysis and comparison
- [Reference specification](REFERENCE_GAME_SPEC.md): 30 systems with OBSERVED / INFERRED / UNKNOWN labels and implementation order.
- [Timestamped observations](GAMEPLAY_TIMING.md): the full 11:52 clip, menus, success and timeout, subsecond action estimates.
- [UI layout](UI_LAYOUT.md), [system/progression map](SYSTEM_MAP.md), [economy](ECONOMY_ESTIMATES.md).
- [Visual comparison](Validation/VISUAL_COMPARISON.md), [match checklist](REFERENCE_MATCH_CHECKLIST.md), [validation report](Validation/REPORT.md).

## Open in Unity
1. Add this folder to Unity Hub and open with 6000.3.25f1.
2. Open `Assets/Scenes/Harvest.unity`, choose a portrait Game view, and press Play.
3. If rebuilding the scene/settings, use **Verdant Trail > Create playable scene**. This recreates the bootstrap scene, so save any custom scene work separately first.
4. Edit `Assets/Resources/HarvestTuning.asset` for movement, range, projectile speed, target clustering, stage counts, HP, rewards and upgrade curves.

Unity 6.3 LTS reference: https://unity.com/blog/unity-6-3-lts-is-now-available

## Build and verify
- `Tools/Build-Windows.ps1`: imports, runs in-engine core checks, and builds the Windows development player. Last build succeeded with zero warnings/errors.
- `Tools/Check-Compile.ps1`: standalone C# compilation against installed Unity assemblies.
- `Tools/Check-Core.ps1`: exact core sources tested using a test-only Unity math shim; additional fallback check, not a renderer test.
- Run the executable with `-validatePrototype` for isolated live UI-event, drag/release, save/load, three-cat equipment and stage-transition checks.
- Run with `-capturePrototype` for an isolated fresh-save, balanced-purchase, 78-second normal-speed run. Camera-rendered images, telemetry and desktop measurements go into `Validation/`.

Validation modes never write the normal player's save. Images are rendered by the actual running game and UGUI into a 588 × 1280 RenderTexture; they are not operating-system window screenshots. Hidden-window screen capture was unavailable. The interactive Windows window defaults to 470 × 1024 to fit ordinary displays.

For Android, install Android Build Support with SDK/NDK/OpenJDK for this Editor through Hub, then switch Build Profiles to Android. Portrait and minimum API 26 are configured. Physical touch, mobile safe areas, Android GPU/CPU behavior, release signing and iOS remain unverified. Desktop timing is not proof of the 60 FPS Android target.

## Architecture
- `HarvestTuning`: saved ScriptableObject for core balance and encounter layout.
- `HarvestSimulation`: deterministic movement, targeting, combat, economy and stage flow with fixed-capacity pools.
- `WorldView`: orthographic following camera, pooled visuals, reusable batched terrain meshes and ground decoration.
- `OriginalArt`: independently authored procedural sprite shapes, cached once; no reference textures are loaded.
- `PortraitHud` / `PortraitLayout` / `DragSurface`: safe-area portrait UGUI and coordinate-correct floating joystick.
- `EquipmentCatalog` / `EquipmentSystem` / `FeatureHud`: acquisition, ownership, drag-to-equip, second shooter and timed-trial screens.
- `SaveStore`: versioned local JSON, v1/v2/v3 migration, sanitization, flushed atomic replacement, backup/temp recovery and damaged-file archival; invalid/future saves are protected from overwrite.
- `RewardCatalog` / `RewardSystem` / `RewardHud`: daily objectives, pass milestones, attendance and calendar checks.
- `GameRoot`: lifecycle/composition, synthesized feedback sounds, save cadence.
- `RuntimeProbe`: explicitly invoked capture/interaction harness.
- `ProjectSetup`: scene/build configuration and in-engine checks.

## Remaining scope
Player-facing equipment, a second shooter and timed trials are now implemented and tested; see [progression update](Validation/PROGRESSION_UPDATE.md). Acquisition and arena tuning use documented assumptions. Combat numbers, palms, canyon edges and weapon feedback received a visual pass; see [presentation update](Validation/PRESENTATION_UPDATE.md). The collection viewer and world-one gold tree are now implemented; see [collection/finale update](Validation/COLLECTION_FINALE_UPDATE.md). Daily missions, a finite field-pass reward track and seven-day claims are now implemented; see [rewards update](Validation/REWARDS_UPDATE.md). Skin ownership bonuses, nine looks, four guns and biome scenery are now implemented; see [skin/content update](Validation/SKINS_CONTENT_UPDATE.md). Merging remains unimplemented. Rebirth, pets, conventional hostile bosses and offline income remain unsupported by this recording; reserved save fields are not functioning features. Original generated sprite art is integrated; animation and sound remain simple, and later progression differs from the reference.

## Evidence handling
`Analysis/` contains reference frames/contact sheets solely for comparison. It is outside `Assets/` and excluded from the game build. The uploaded MP4 remains in Downloads. No original sprites, audio, logos or code are reused.

Latest layout work: [portrait/safe-area update](Validation/LAYOUT_UPDATE.md), including simulated phone/tablet captures and input checks.

Save reliability: [recovery behavior and validation](Validation/SAVE_RECOVERY_UPDATE.md).

Latest correctness fixes: [input interruption and trial deadlines](Validation/INPUT_DEADLINE_UPDATE.md).

Three-cat equipment, independent weapon stats, empty-slot handling and save version 6 are implemented; see [three-cat validation](Validation/THREE_CAT_UPDATE.md).
