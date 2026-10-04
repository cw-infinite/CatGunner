# Reference HUD update — 2026-10-03

Implemented original illustrated icons and four colored sliced button surfaces, icon-over-label side rails, larger upgrade and currency icons, banknote cost badges, centered five-node grove progress with a cat marker, and icon-only settings/developer buttons. Lilita One provides rounded white text with a near-black outline. Font license ships beside the executable.

Developer tools now grant 100 or 1,000 crystals, step to the previous grove or next stage, and leave a trial for its preserved grove. Upgrade purchases continue to spend banknotes, as requested. Tree health bars are wider and thin. Larger orange-white shots, translucent trails, muzzle bursts and larger impact sparks improve firing feedback.

Fish, Pet, Mine, Boss and Hunt are explicitly locked placeholders. Gun, Skin, Play, Daily and Tasks retain their existing working flows. Three-cat gear and cumulative skin bonuses remain intact.

## Validation

- Unity 6000.3.25f1 Windows development build: succeeded, zero warnings and errors.
- Final isolated player run: 211 PASS, zero FAIL, zero runtime errors.
- Editor checks include 26 skin/weapon checks, 20 three-cat checks, 19 filesystem checks, trial restoration/rewards, and deterministic progression.
- Runtime checks cover crystal grant amounts, previous grove, next stage, return from trial, actual bundled font and all 12 icon loads, locked controls, safe-area bounds and drag mapping.
- Inspected gameplay, developer panel, projectile, notched skin gallery and tablet equipment captures. Corrected generated atlas row boundaries to preserve complete icon silhouettes.
- Final desktop diagnostic: 55.9 mean FPS including captures; peak 74 draw calls. This is not a mobile performance result.

Captures: `runtime_squad.png`, `runtime_dev_icons.png`, `runtime_projectile_effect.png`, `layout_notched_skins.png`, `layout_tablet_gear.png`. Tests use isolated saves; normal player progress was not modified.

Art prompts and font provenance: `Assets/Resources/Art/HUD_PROMPTS.md`.
