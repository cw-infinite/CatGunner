# Daily rewards and missions — validated 2026-09-30

## Player-facing additions
- DAILY opens a seven-day crystal calendar from the start. One claim is available per UTC day, with claimed cards visibly marked. The seven-claim sequence repeats on the next eligible day after day seven; missed days do not discard progress.
- TASKS opens at stage 3. Daily objectives track harvested plants and normal stage clears. Completed objectives grant pass points; the three field-pass milestones grant crystals when claimed.
- Progress continues underneath these menus. Closing them restores movement input. Claims trigger an immediate local save, in addition to the usual save cadence.
- Save version 4 preserves currencies, equipment and trial progress from versions 1–3, and adds daily counters, claim flags, pass points and attendance dates. The existing filename is retained. Historical plant counts are not reconstructed for old saves; tracking begins when the updated game is run.

## Reference evidence and bounded assumptions
The frame at 00:07 shows a seven-day crystal calendar with day one already claimed. The mission screen at 04:00 shows a pass reward track and daily tree-harvesting objectives, with stage objectives further down the list. Existing appearance evidence was also inspected at 05:12–05:15; it shows a starter selection and locked appearances, but does not establish unlock rules, so appearance progression is still deferred.

The implementation uses original labels, art and independent reward values. RewardCatalog contains harvest goals 100/300/700, normal-stage goals 1/3/5, ten points per mission, pass thresholds 20/40/60 with 24/36/48 crystals, and daily rewards 12/18/24/30/36/42/60. Unlock at stage 3, midnight UTC resets, seven-claim repeating attendance without a missed-day penalty, and counting trial plants toward harvest missions are explicit prototype assumptions. Trials do not count as normal stage clears.

The field pass is a finite free three-milestone track; its points and claims survive daily reset. Seasonal resets, paid tracks, advertisements and a larger weapon reward catalog are not implemented because their full behavior is unverified. Time comes from the device UTC clock; rollback does not reopen prior claims, but this is not a server-verified anti-cheat system.

## Validation
Unity editor checks cover v3-to-v4 migration, one count per destroyed plant, normal-stage counting, trial counting boundaries, incomplete-claim rejection, repeated-claim rejection, JSON persistence, UTC rollover, rollback handling, and the seven-day cycle. The existing core, equipment, trial and finale checks also pass. The separate ten-minute core run still reaches stage 8, with 491 destroyed targets, 1,508 shots and 46 upgrades.

The rebuilt Windows player exited 0 with zero runtime errors. Actual UI-event checks confirm one daily payout despite repeated clicks, mission claims awarding points once, an earned pass milestone paying crystals once, v4 filesystem save persistence, and restoration of battlefield controls. The first test run clicked Tasks before the HUD refreshed after closing Daily; the harness now waits for that refresh, and the repeated run passes.

Inspected game-rendered captures: runtime_daily_ready.png, runtime_daily_collected.png, runtime_missions_ready.png and runtime_missions_claimed.png. The calendar and all six objective rows fit the portrait panel without clipping. Test progression uses the isolated validation scenario; normal player saves are not modified.

The build completed with zero warnings and errors. The interaction run sampled 1,439 frames at an average 51.2 FPS including captures, with 30 peak draw calls and 2,969 recorded GC bytes/frame. This transition-heavy desktop sample is not a steady-state or Android performance result. Android Build Support and device verification remain outstanding.
