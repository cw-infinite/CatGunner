# Reference-driven art replacement — 2026-10-02

The user correctly identified that the previous geometric rabbit, triangular trees and flat panels did not resemble the recording's visual direction. This pass replaces that placeholder approach with original generated cartoon assets.

## Integrated assets

Two transparent atlases supply sixteen sprites: evergreen, broadleaf, flowering tree, palm, cactus, golden finale tree, cream-and-ginger cat, blaster, button, cream menu, brown equipment card, currency pill, banknote, gem, power badge and speed badge. The source sheets and exact prompts are saved under `Assets/Resources/Art/`. The built-in image-generation tool was used, with the supplied gameplay frames as style guidance; reference pixels are not shipped as game assets.

`SpriteArt` defines atlas regions, trims transparent gutters, normalizes world scale, and creates nine-slice UI sprites. `OriginalArt` caches these sprites and keeps procedural effects/fallbacks. All sixteen assets are warmed before play to avoid first-use decoding work during stage transitions. Imports preserve alpha and use shared atlas textures, readable uncompressed RGBA with no mipmaps. Mobile memory/compression decisions remain pending profiling.

## Presentation changes

The cat replaces the rabbit and turns with aim. Trees have rounded silhouettes and thick warm outlines. HUD currency and upgrade icons use the new artwork; major buttons, menus and equipment cells use textured scalable panels. Upgrade values and the stage label have contrasting outlines. The grass palette is lighter, small grass tufts replace line flecks, and finer cliff-edge subdivision reduces the long straight segments. Equipment highlights use a separate untextured frame so the brown card pixels cannot darken the gold selection color.

The first integration exposed undersized nine-slice borders that made stretched buttons pointed; the final borders preserve the full rounded corners. An existing fixed-time targeting assertion sampled between targets and failed once. It now watches up to five seconds and requires each squad unit to acquire a target; gameplay logic was not changed to satisfy the check.

## Remaining visual gaps

This is a first integrated art replacement, not visual parity. The tree distribution still concentrates too much at the right edge in early views. Terrain cliff faces remain procedurally simple. Typography, exact mint/cream balance, side-menu hierarchy and animated character poses need further work. The sprite sheets contain static poses with existing bob/recoil effects, not new animation cycles.

Review `runtime_art_forest.png`, `runtime_equipment_selected.png`, `runtime_settings.png` and `layout_reference_play.png` against the reference frames in `Analysis/`. Player captures render the real camera and UGUI; they are not concept mockups or physical Android captures.

## Final validation

The final Windows build succeeded with zero warnings/errors. The isolated player exited 0 with **110 PASS entries and zero runtime errors**, including all sixteen generated atlas sprite loads, both squad units acquiring targets, equipment/reward flows, saves and four fitted portrait viewport profiles. The interaction/capture run is not a steady-state performance benchmark. The normal player save remains untouched; no APK or Android device validation is claimed.
