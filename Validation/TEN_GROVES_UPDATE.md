# Ten grove themes and weapon readability — 2026-10-03

Each grove contains five sectors. Theme index is ((stage - 1) / 5) modulo 10. Grove 11 repeats Grove 1, and the cycle repeats independently of encounter-count layouts. Existing saves adopt their current grove's theme automatically.

| Grove in cycle | Theme | Vegetation / scenery |
|---|---|---|
| 1 | Meadow Woods | Oak, mossy stones and daisies |
| 2 | Sunlit Coast | Palm, sandstone and shells |
| 3 | Blossom Spring | Cherry blossom, pink flowering stones |
| 4 | Amber Autumn | Maple, fallen leaves and logs |
| 5 | Snowfall Pines | Snowy spruce, snow-covered rocks |
| 6 | Moonlit Willow | Purple willow, violet mushrooms |
| 7 | Bamboo Garden | Bamboo, bamboo stumps and green stones |
| 8 | Mushroom Hollow | Giant mushroom trees, small spotted mushrooms |
| 9 | Crystal Valley | Crystal trees and cyan mineral clusters |
| 10 | Ember Grove | Ember trees, cracked charcoal rocks |

Each has a coordinated ground, plateau and cliff palette. The HUD names the theme. The original gold-tree finale remains at Grove 1-5; timed trials retain their own target rules. These are original game content additions, not claims about unobserved reference-game stages.

Four simplified gun sprites replace the earlier detailed variants. Held gun scale increased from .31 to .43 (about 39%). Projectiles now originate near the enlarged muzzle. Trail pulse fires orange pulses; Quick spark fires cyan lightning; Briar burst fires green seed orbs with compact explosive rings; Sun flare fires gold orbs with a larger, brief golden burst. Impact effects retain the firing gun's identity even if equipment changes before impact.

Explosions are visual effects with existing single-target damage and prices. Rings last at most .28 seconds, remain local to the hit, render below damage numbers, and use a capped 24-entry pool. No screen flash or camera shake. Stage/trial transitions reset the pool.

Original tree, scenery and weapon PNGs and exact built-in image-generation prompts are recorded in `Assets/Resources/Art/THEME_PROMPTS.md`. Cropping boundaries follow transparent atlas gutters; generated alpha is preserved.

Foreground tree silhouettes become partially transparent when covering a cat. DEV Next theme jumps to the first sector of the next grove, capped at stage 100; it changes development progress just like Next stage.

## Final verification

Windows build succeeded with zero warnings/errors. Runtime validation: 256 PASS, zero FAIL and zero runtime errors. Standalone core checks: 36,056 assertions passed. All ten theme captures and weapon effects were inspected; foreground mushroom overlap was corrected with tree transparency. Existing equipment, skins, trial restoration, rewards, save recovery and safe-area checks passed.

Runtime captures: theme_01.png through theme_10.png and weapon_effect_0.png through weapon_effect_3.png. The diagnostic includes fourteen extra captures in one frame, so its peak draw calls and allocation mean are not a steady-gameplay or Android benchmark. Android device performance remains unverified. Normal player saves were not modified by validation.
