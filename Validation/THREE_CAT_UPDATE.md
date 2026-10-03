# Three-cat equipment squads — 2026-10-03

The user confirmed support for up to three cats with separate gear. All three gear slots are now usable when GEAR unlocks at stage 3; no additional recruitment fee or later unlock is invented. Buy spare guns through delivery or collection, then equip them in CAT 2 and CAT 3 to recruit the followers.

## Behavior

- Each active cat targets and fires independently using its equipped weapon's damage and interval. Owned skin bonuses apply to the squad as before.
- Equipment shows CAT 1 / CAT 2 / CAT 3 and a live squad count. Empty follower slots explain how to add a cat; both followers have removal controls. Collection remains accessible under the leader card.
- Removing either follower returns its gear to inventory. If inventory is full, removal is disabled rather than discarding a weapon. The leader cannot be removed.
- A gap in the middle equipment slot is supported. Simulation and world rendering share the same mapping from active cats to equipped guns, preventing the remaining third-slot cat from showing the wrong gun.
- Existing follower state is preserved when possible during equipment changes. Newly recruited followers start on opposite sides of the leader. Trials and normal stage transitions support all three cats.
- Save version 6 preserves all three slots. Older saves migrate without granting gear; ownership checks reject equipping more copies than owned. Up to fifteen copies of one gun can persist: twelve spare inventory cells plus three equipped copies. Older builds reject the newer save rather than silently discarding the third slot.

## Verification

Twenty dedicated Unity assertions cover buying distinct guns, the three cats' weapon mappings (leader plus two followers), simultaneous projectiles with distinct damage, per-gun cooldowns, middle-slot gaps, removals, reload, trial return, full inventory and invalid duplicate equipment. The standalone suite passes 36,056 assertions with unchanged default solo progression.

Final Windows build: zero warnings/errors. Final isolated player: exit 0, **182 PASS entries and zero runtime errors**. Three-cat gear and battlefield captures were inspected: all three gun designs render and equipment labels fit.

The built-player harness buys Sun flare, drags it into CAT 3, removes and restores CAT 2, saves/reloads all three distinct guns, enters/exits a trial and captures the full squad. Its existing phone, inset-phone and tablet gear checks now run with all three slots filled. Normal player saves remain untouched by validation.

Review captures: `runtime_three_cat_gear.png`, `runtime_three_cat_squad.png`, `runtime_three_cat_trial.png`. These are actual camera/UGUI renders from the Windows player; Android device verification remains outstanding.
