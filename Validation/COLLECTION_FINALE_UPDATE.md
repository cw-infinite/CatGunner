# Collection and world-one finale — 2026-09-29

## Changes
The GEAR screen now opens a collection viewer. Both implemented tools can be inspected before or after acquisition; the panel shows discovery, current damage/interval, ownership, equipped count and acquisition source. Returning restores the equipment screen. Browsing does not grant items or change the loadout. Tap-to-equip instructions now remain visible while a tool is selected.

Stage 1-5 now ends with one large gold tree placed beyond the ordinary clusters. It remains a stationary destructible, uses independently drawn golden foliage, has a scaled health bar, and must die before the normal clear/transfer sequence opens the sand/palm biome. Trial entry and return preserve its health and the cleared surrounding grove.

Save sanitization also preserves legitimate full inventories with equipped items; the previous limit could truncate ownership above twelve even though equipped tools occupy separate slots.

## Evidence and assumptions
The collection viewer and weapon detail are visible at 05:21–05:24; the gold tree appears around 07:03–07:09. These were rechecked in the reference contact sheets. Only the two implemented weapons are listed; a large catalog, rarity rules and merging are not inferred from grid silhouettes. The gold tree is restricted to stage 5 because later-world finales were not shown.

Finale tuning is configurable in HarvestTuning: 20 times stage base HP, 8 times base target reward, 1.65 times sprite scale, and placement four world units beyond the last ordinary cluster. These are independent prototype estimates, not recovered source values. The tree replaces the last ordinary target, keeping the configured stage count. Progress still counts targets equally, so the tree occupies the last roughly one percent despite its greater health.

## Verification
- Unity Windows development build: succeeded, zero warnings/errors.
- In-engine checks: one gold target, blocked stage clear, challenge restoration, single payout and sand-biome transition. Full-inventory sanitization and rejection of unaffordable/full purchases pass.
- Built-player interactions: collection previews an unowned tool without granting it, shows updated ownership/equipment after drag-to-equip, and returns to gear. Existing movement, upgrades, trials and save checks pass.
- Finale runtime test: earlier plants are removed for setup, then the two-shooter squad destroys the gold tree through ordinary projectile damage at Force/Tempo level 15. The test confirms stage 6 and palms after transfer. This is a finale integration test, not a complete natural playthrough of stage 5.
- Actual game-rendered captures inspected: runtime_collection_owned.png, runtime_collection_unowned.png, runtime_gold_finale.png, runtime_gold_clear.png and runtime_world_two.png.
- Interaction test exited 0, with zero runtime errors. Its transition/capture-heavy desktop sample averaged 51.0 FPS across 1,377 frames, with 30 peak draw calls. This is not a steady-state or mobile benchmark.
- Ten-minute deterministic core run still reaches stage 8: 491 targets, 1,508 shots, 46 upgrades. Earlier reports showing 498 targets/1,513 shots predate the finale.

Appearance, mission/pass and daily-claim flows remain for later implementation. Android support/device testing is still outstanding.
