# Gameplay presentation update — 2026-09-30

This pass improves the existing harvesting presentation without changing damage, prices, target placement or stage progression.

- Floating damage and income numbers use pooled dark shadows and slight horizontal separation to improve contrast over light ground and foliage. Income text is brighter green.
- Palm sprites now have separate pointed fronds, a banded trunk and small fruit shapes, replacing the previous oval canopy. All shapes remain independently authored in code.
- Canyon edges vary gently between connected segments, with dark rim lines and short rock strata. The additional geometry uses the existing terrain batches/material rather than separate objects.
- Tools recoil during their muzzle pulse, remain upright when aiming left, and use the same equipment tint as their inventory icons. The ranger has a subtle walking tilt.

The reference supports white/green combat numbers, distinct palm silhouettes, uneven diagonal cliffs and short shot feedback. The exact offsets, colors and timing used here are original approximations. Appearance unlocks and other unverified progression rules remain deferred.

Unity rebuilt the Windows player with zero warnings/errors. All 65 existing live interaction checks pass with zero runtime errors, covering movement, upgrades, equipment, trials, finale, daily rewards and missions. The interaction run averaged 58.7 FPS across 1,425 frames, including captures and transitions; 30 peak draw calls. Sand/palm and gold-finale captures were inspected for silhouette, aiming, health-bar placement and number contrast. Normal-speed runs after the visual changes averaged 59.8–59.9 FPS with zero runtime errors. The final 78-second run after the capture fix exited 0 with zero runtime errors: 4,487 sampled frames, 59.8 FPS average including captures, 22 peak draw calls and 382 recorded GC bytes/frame. These are desktop development-build results, not Android certification.

The first offscreen HUD capture omitted some static buttons. Forcing graphic geometry alone did not resolve it; an initial render warm-up does. The inspected eight-second comparison frame now contains the complete HUD. The warm-up image is a diagnostic intermediate, not a comparison screenshot.
