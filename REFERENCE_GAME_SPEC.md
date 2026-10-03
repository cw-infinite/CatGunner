# Reference game specification

## Scope and evidence
Primary evidence: user-supplied portrait MP4, 588 × 1280, 29.944 fps, 21,333 frames, 712.43 seconds. Inspected timestamped samples every 3 seconds over the entire clip, enlarged menu frames, and 0.2–0.4 second sequences for combat and transitions. This is visual frame analysis, not a claim of continuous audiovisual playback. Audio was not auditioned; sound timing remains UNKNOWN. Analysis images are private reference evidence only and must never enter Assets or a distributable build.

OBSERVED means directly visible. INFERRED means a minimal proposed explanation. UNKNOWN means the recording does not establish the behavior. Screen coordinates use the entire 588 × 1280 recording; the brown top strip occupies about 7.3%.

## Important conclusion
This is an automated vegetation-clearing shooter with optional touch movement, NOT a demonstrated combat game against mobile hostile creatures. Trees are targets, money drops from destroyed trees, and the world scrolls around a small armed character. Do not add enemies that chase, player health, dodging, or a conventional boss fight. A joystick is directly visible at 00:15. Shooting occurs without a visible attack button. Later two armed characters independently fire; weapon equipping is associated with the second unit's arrival. The later magnet-shaped Auto button is NOT sufficient evidence of auto-movement unlocking.

## System specification
| System | OBSERVED | INFERRED for prototype | UNKNOWN |
|---|---|---|---|
| 1. Camera | Orthographic-looking 2D sprites, oblique ground, vertical tree trunks, diagonal cliffs. Leader near (50%,46%). Background translates, no visible zoom or rotation. | Orthographic XY camera follows leader tightly, with mild smoothing and leader above screen center. | Exact projection, smoothing constant, boundary policy. |
| 2. Orientation | Tall portrait, aspect 0.4594. | Portrait mobile, 588 × 1280 reference canvas. | Other aspect behavior, safe-area policy. |
| 3. Battlefield | Green canyon, diagonal upward-right route, dense vegetation ahead. Sand/palms from world 2. Timed arenas are open with curved dark boundaries. | Preplaced deterministic target clusters along a corridor. | Procedural versus authored source level generation. |
| 4. Player position | Single unit approximately 11–13% of screen width, feet near 48%; later units spread around center. | Small independently drawn animal ranger with original equipment. | Exact collision capsule. |
| 5. Movement | World moves between kills; character can fire while changing position. Joystick at 00:15. No attack control. | Automatic approach to nearby vegetation with manual drag override; pause near target, resume when targets move out of range/die. | Whether all early motion is touch-driven with joystick hidden; precise acceleration. |
| 6. Squad | Two units after weapon equip around 03:06; distinct angles, targets and damage values. Separation varies and units overlap briefly. | Independent target selection, simple follower offset with catch-up. | Third-slot behavior, collision resolution. |
| 7. Targeting | White segmented crosshair near tree trunk; persists across shots, switches after depletion. Different squad targets at 03:07–03:11. | Nearest in-range target, retain until dead; deterministic index tie-break. | Hidden weighting, detection cone/radius. |
| 8. Auto attack | Repeating orange/yellow shot streaks; one unit shoots without attack-button presses. Weapon UI exposes interval. | Per-unit cooldown, no reload, fire while moving if in range. | Animation-to-damage frame alignment. |
| 9. Spawning | Trees appear in dense groups as camera reveals terrain; no portal or timed hostile wave. | Stage places all vegetation in spatial clusters. | Offscreen streaming, exact count. |
| 10. Enemy motion | Target vegetation is stationary. Trunks/canopies shake on impact and vanish on destruction. | Static destructibles; no chase AI or attacks. | Any hostile content outside this recording. |
| 11. Damage | White numbers above target; small horizontal HP strips above canopy. | Apply damage on projectile arrival; brief recoil/flash. | Critical chance, armor, hidden health formulas. |
| 12. Projectiles | Luminous orange streaks extend from gun toward target, visible between endpoints. | Fast pooled traveling projectiles, no splash or piercing. | True engine simulation versus stretched visual tracer. |
| 13. Drops | Green notes burst at tree, green values rise, notes shrink/travel toward squad. | Pooled scatter then short magnet collection; credit once. | Whether wallet credit occurs at death or final pickup. |
| 14. Upgrades | Exactly three persistent cards: damage, firing speed, income. Levels, stat value and cost always visible. | Original labels Force / Tempo / Yield, same hierarchy. | Press-and-hold purchasing. |
| 15. Costs | Early costs rise substantially faster than stats; bright cyan affordable states versus dark interiors/red costs. | Shared geometric cost curve in tuning asset; only affordable purchases succeed. | Exact rounding and high-level formulas. |
| 16. Stage flow | Top percentage fills, five-node world strip. 1-1 to 1-5 then 2-1. | Percent = cleared targets / stage target count; five sectors per biome. | Whether percentage weights target HP or count. |
| 17. Boss flow | Large gold tree near 07:03–07:12, disappears before world change. Locked Boss side icon. | Large durable end-of-world vegetation only if extending scope. | Actual Boss mode, attack phases, failure state. Do not equate timer arena with boss. |
| 18. Menus | Settings, attendance, challenge dialog, weapons, appearance, weapon collection, mission/pass screens. Combat visibly continues behind settings at 00:21–00:22. | Non-pausing simple settings; secondary menus deferred until core match. | All tabs and rewards not opened. |
| 19. Secondary systems | Several locked side tiles appear after weapon tutorial. | Preserve map and defer unknown systems. | Pet/fish/hunt/mine/rebirth implementation. |
| 20. Unlocks | Early HUD sparse; challenge button by 00:42; broad side rails by 03:09. | Stage-1-3 milestone for squad/menu preview, subject to refinement. | Whether level, quest, elapsed time, or tutorial causes unlock. |
| 21. UI | Thin progress strip at y7.3%; stage below; resources upper-right; upgrades near bottom; rails at edges. | Anchored portrait overlay. | Text scaling on other devices. |
| 22. Animation | Small walk bob, gun rotates, muzzle pulse, tree hit shake, no lengthy death animation. | Short procedural animation, independent of artwork. | Exact walk cycle frames. |
| 23. VFX | Bright warm shot, green bill shower, white damage, multicolor stage confetti. | Pooled simple original shapes. | Particle shader details. |
| 24. Sound | Not assessed from frames. | Optional synthesized click/shot/impact only; no copied audio. | All reference audio timing, music, mix. |
| 25. Feedback | Upgrade card flashes; stat changes immediately. Clear banner rises then fades. | 0.15s button feedback; about 2s banner plus 1.5s transition. | Haptics. |
| 26. Idle | Attacks and apparent progress continue while settings overlay is open. | Simulation continues under menus. | Long idle caps/auto-collect behavior. |
| 27. Offline | No return-from-offline claim screen observed. | Save timestamp only; no invented offline income. | Formula, cap, boosts, claim screen. |
| 28. Prestige | Locked rebirth tile visible. Never entered. | Reserve save field, do not implement reward/reset rules. | Unlock, reset scope, permanent gains. |
| 29. Economy | Green cash, blue premium gems. Cash buys three upgrades; gems used in weapon roll. Challenge rewards gems; mission progression shown. | Independent numbers matching approximate purchase cadence. | Monetization, paid transactions, exact suffix scheme. |
| 30. Difficulty | Increasing foliage density and numeric damage; squad increases total fire rate. Final cactus challenge times out despite progress. | Tune HP/count/rewards per stage without target aggression. | Long-term plateau or failure penalties. |

## Target catalog
- Conifer: tall triangular silhouette, roughly 17–23% viewport width; early groups of several; stationary; disappears into money/leaf effects.
- Rounded deciduous tree: round crown, often broader; mixed with conifers; HP varies but unmeasured.
- Pink flowering tree: dense later forest clusters; functionally same stationary destructible in evidence.
- Palm: sand biome and second timed challenge; separated open-arena layout or dense corridor clusters.
- Cactus: third timed challenge; stationary, varying silhouettes, partial progress before timeout.
- Large gold tree: world-1 finale around 07:03; larger than standard tree; durable focal target. No attack observed.
All visual replacements must be independently authored. No enemy speed or attack ranges can be assigned as observed; target speed is zero in inspected frames.

## Implementation order
1. Commit evidence/specification and unknowns before gameplay coding.
2. Portrait Unity 6.3 LTS project, orthographic XY projection, oblique corridor and small original placeholders.
3. Data asset for speeds, range, HP, reward, stage counts, upgrade curves.
4. Stationary pooled targets placed in clusters; deterministic nearest targeting.
5. Automatic approach plus floating joystick/manual override; tight camera follow.
6. Pooled traveling shots, impact damage, HP bar, crosshair, target depletion.
7. Pooled currency scatter/magnet, number feedback, wallet.
8. Exactly three upgrade controls with affordability and immediate stat updates.
9. Percent and five-sector stage strip; clear banner/confetti and short transfer.
10. Versioned saving, settings, developer tools; compilation and deterministic simulation checks.
11. Record/inspect prototype and compare composition/timing; document actual gaps.
12. Only after core comparison: weapon acquisition/equip and second squad member, then timed harvest arenas and success/timeout return.
13. Appearance and collection viewer, mission/pass flows, daily claim; use original names/icons.
14. Do not invent locked modes, offline gains, prestige resets or monetization. Obtain additional reference footage when these enter scope.
15. Device profiling and Android build after platform module installation; iOS after Android validation.

## Skin clarification - 2026-10-03

At 05:12 the appearance panel explicitly says effects apply without equipping the skin. The user confirms that purchased skins cumulatively increase Force, Tempo and Yield, while equipped appearance is independent. Implemented in the v5 prototype with nine original looks. The ownership rule is confirmed; crystal prices and per-skin bonus values are configurable prototype tuning, not measured values from the video. See `Validation/SKINS_CONTENT_UPDATE.md`.

## Squad clarification - 2026-10-03

The user confirms a maximum of three cats, recruited by equipping purchased gear. All three equipment slots are implemented, each with independent weapon stats. The third slot is available with the existing stage-3 gear unlock; no separate later unlock or recruitment charge is specified.
