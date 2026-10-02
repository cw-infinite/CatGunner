# Equipment and timed-trial update — 2026-09-29

## Playable additions
- TRIAL unlocks at stage 2. Enter a circular arena with a 60-second countdown; harvest all plants to earn 36 crystals. Success, timeout and manual exit restore the exact normal-grove target state from entry.
- Trials advance from trees to palms to cacti. Cash collected inside a trial is retained. Later attempts repeat the final arena template.
- GEAR unlocks at stage 3. A 36-crystal delivery grants a Quick spark tool. Drag it from inventory into slot two, or tap the item and then the slot, to activate the second shooter. Removing it returns the tool to inventory.
- Both equipped tools show damage and firing interval. The third slot remains locked because its unlock was not demonstrated.
- Save version 3 persists owned/equipped tools and trial wins, migrates older saves, and retains the existing save filename. Reloading restarts the normal stage; it does not resume an in-progress trial.

## Reference boundaries and assumptions
The reference confirms weapon acquisition, drag-to-equip, a second shooter, and timed vegetation challenges with success and timeout. Exact unlock conditions, acquisition odds, arena geometry, target counts and repeat rules are not established. Stage 2/3 unlocks, 36-crystal payout/cost, deterministic delivery and 24/40/64 target arenas are explicit prototype assumptions. Tool damage/interval ratios approximate the visible 100/0.6s and 80/0.5s relationship. No merging, random rarity or third-character unlock is claimed.

## Verification
Unity 6000.3.25f1 rebuilt the Windows player with zero warnings/errors. In-engine checks cover old-save migration, exact currency debit, item ownership, duplicate-equipment rejection, equip/unequip, normal-grove restoration, success/timeout/manual exit and one reward payout per win. An autonomous first trial with level-four upgrades cleared in 28.02 seconds without forced damage.

The actual player's UI test exited 0 with zero runtime errors. It bought a delivery, executed inventory drag/drop, verified the second shooter, roundtripped equipment/trial progress through JSON, and exercised success, timeout and return. Success and timeout UI cases use forced completion/shortened time to test transitions; the 28.02-second result comes from the separate unforced simulation. Captures were inspected after adding original tool icons and repairing palm trunks.

The interaction run sampled 861 frames, averaging 58.4 FPS including capture overhead, with 29 peak draw calls and 2,568 recorded GC bytes/frame. This short transition-heavy desktop run is not a steady-state or Android benchmark. Previous normal-loop performance figures remain a baseline from before this extension.

## Captures
- `runtime_trial.png`: live arena and countdown.
- `runtime_trial_win.png`: successful reward screen.
- `runtime_equipment_inventory.png`: acquired tool before equipping.
- `runtime_equipment_equipped.png`: two equipped tools.
- `runtime_equipped_squad.png`: squad after equipping.
- `runtime_trial_timeout.png`: timeout and return action.

Android Build Support is still absent; no APK is supplied. Missions, appearance, collection/merging, the final gold tree, fuller animation and mobile verification remain outstanding.
