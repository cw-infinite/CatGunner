# Validation report — working Windows prototype

## Build and engine checks
Unity 6000.3.25f1 imported the project and built `Builds/Windows/VerdantTrail.exe` successfully: **zero warnings, zero errors**. The previous license prerequisite is resolved. The invalid legacy-input package entry found on first import was removed.

All runtime/editor C# files compile. The core checks also pass inside Unity, using the actual ScriptableObject asset. The 600-second deterministic run reaches stage 8 with 498 destroyed targets, 1,513 shots and 46 purchases. Peak simultaneous solo shots: 1; notes: 8. See `checks.txt`, `simulation.csv`, `build.txt` and `unity-build.log`.

The standalone math-shim tests remain an additional logic check; they are no longer the only evidence.

## Built-player checks
`-validatePrototype` exits 0. Live Unity event/raycast tests confirm:
- Drag movement through the normal Update loop; releasing the pointer stops manual movement.
- All three upgrade buttons respond and unaffordable purchases are rejected.
- Settings open/close, while the simulation continues underneath.
- JSON filesystem roundtrip, v1 migration, and protection against overwriting a future-version save, using an isolated test file.
- Two-unit developer test, clear phase, transfer phase and resumption in the next stage.

The test does not simulate physical Android touch or a full application relaunch. Normal player saves are untouched by validation modes. The deliberate future-version test emits an expected warning; no runtime errors were recorded.

## Visual inspection and normal-speed run
A 78-second normal-speed desktop run exited 0 with **zero runtime errors**. It used a fresh save and one balanced purchase attempt per second. Four scene captures and clear/transfer captures were inspected; see `VISUAL_COMPARISON.md` and `runtime-telemetry.csv`.

The first stage advances at about **53.7 seconds** in the actual player. The fixed-step model gives 50.55s / 70.38s for the first two cycles, including 3.5 seconds of clear/transfer. Different purchase checks and frame quantization account for some variation; the second full stage cycle was checked in simulation, not completed within the 78-second player run.

Changes driven by captures: denser target clusters; larger ranger silhouette; current-sector marker; clearer rounded upgrade/currency panels; corrected modal sorting; non-overflowing developer damage labels; modest squad separation.

Images are rendered by the running game's camera and UGUI into a 588 × 1280 RenderTexture. They are not operating-system screenshots. Direct screenshot capture of a hidden player window failed; the explicit camera capture works and avoids claiming that failure was a success.

## Desktop performance sample
Core-loop baseline before the equipment/trial extension, on the local NVIDIA GTX 1050 Ti:
- 4,490 measured frames after startup.
- Average **59.8 FPS**, including capture overhead.
- Peak **22 draw calls**; previous renderer peaked at 38 in its recorded opening run.
- Average recorded **346 GC bytes/frame**, including UI strings, stage changes and validation capture overhead. This is not an allocation-free loop.

Terrain now reuses three batched meshes, one material and pooled ground decorations instead of recreating hundreds of objects/materials at each stage. The shorter interaction test averaged 54.4 FPS while repeatedly capturing and forcing transitions; it is not a steady-state benchmark. Recorder summaries are in `runtime-capture.txt` and `runtime-checks.txt`.

These measurements are desktop development-build results. They do **not** certify the 60 FPS mid-range Android target.

## Remaining validation and scope
Android Build Support remains absent for this Editor; no APK or iOS build exists. Physical touch, notch/safe-area behavior, Android CPU/GPU/memory, battery use, release build stripping/signing and long-duration save/relaunch soak remain unverified. Synthesized sound playback is implemented, but reference audio was not auditioned and audio fidelity is unknown.

The first playable covers the core loop. Equipment and timed challenges are now covered by the extension below. Missions, appearance menus and later-game pacing still require subsequent phases. Placeholder art/animation and environmental density are not a final visual match.

## Equipment and trial extension
See [PROGRESSION_UPDATE.md](PROGRESSION_UPDATE.md) for the implemented features, assumptions and current verification. Rebuilt with zero warnings/errors; live interaction tests exited 0 with zero runtime errors. Actual drag/drop equips the second shooter; success, timeout and manual exit restore the normal grove. Save v2-to-v3 migration and equipment persistence pass. First trial clears naturally in 28.02s at level-four upgrades. The current short interaction run averaged 58.4 FPS including captures; it does not replace the earlier normal-loop baseline.

## Collection and finale extension
The gold tree and collection viewer are now implemented. See [COLLECTION_FINALE_UPDATE.md](COLLECTION_FINALE_UPDATE.md) for reference evidence, tuning assumptions and runtime checks. The updated ten-minute deterministic run reaches stage 8 with 491 targets, 1,508 shots and 46 upgrades; the older figures above predate the finale. A full inventory plus equipped items is preserved by save sanitization.

## Daily rewards and missions — 2026-09-30
Daily attendance, daily objectives and the finite free field pass are implemented and validated. See [REWARDS_UPDATE.md](REWARDS_UPDATE.md) for evidence and assumptions. Save version 4 migration, serialized claim flags, UTC rollover and duplicate-payout protection pass engine checks. The built-player interaction run exited 0 with zero runtime errors; new panels were visually inspected. Its 51.2 FPS average includes captures and transitions and is not a mobile benchmark. Missions are no longer outstanding; appearance progression, seasonal/premium pass behavior and broader content remain deferred.

## Presentation and current normal-speed sample — 2026-09-30
See [PRESENTATION_UPDATE.md](PRESENTATION_UPDATE.md) for the visual changes and capture fix. The Windows build succeeds with zero warnings/errors. All 65 existing live interaction checks pass. The final 78-second normal-speed run exits 0 with zero runtime errors, averaging 59.8 FPS across 4,487 sampled frames; 22 peak draw calls and 382 recorded GC bytes/frame including captures. Forest, gold-tree and sand/palm images were inspected. Android remains unbuilt and unverified.

## Safe-area portrait layout — 2026-09-30
The HUD now preserves portrait composition inside the reported safe area, and joystick/drag coordinates follow the fitted content. Full-screen modal shades cover the surrounding display. The build succeeds with zero warnings/errors; all 79 current runtime checks pass with zero runtime errors. Eight gameplay/equipment render profiles and two inset coordinate checks cover four simulated screen shapes. Final tablet and notched-phone menu images were inspected. See [LAYOUT_UPDATE.md](LAYOUT_UPDATE.md) for sizes and limits; this does not certify actual mobile hardware.

## Save recovery — 2026-09-30
Backup and interrupted-write recovery, preservation of damaged primary files, and visible Settings status are implemented. The build succeeds with zero warnings/errors. Nineteen filesystem cases pass in both editor and player, and the full player run exits 0 with zero runtime errors. Protected/recovered Settings screenshots were inspected. See [SAVE_RECOVERY_UPDATE.md](SAVE_RECOVERY_UPDATE.md) for recovery order, expected diagnostic warnings and limits; hardware power-loss and mobile filesystem behavior remain unverified.
