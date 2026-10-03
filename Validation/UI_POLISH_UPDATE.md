# UI polish — 2026-10-02

## Changes

- Independently drawn settings, note and crystal icons replace placeholder symbols on the main HUD. Currency amounts retain compact formatting.
- Buttons share dark outlines and pressed/disabled feedback. Equipment selection uses gold outlines on the selected tool and both usable slots; the locked third slot stays unhighlighted.
- The equipment screen shows crystals, inventory usage, delivery cost and the number of crystals still needed.
- DAILY has a ready indicator; TASKS counts currently claimable missions and pass rewards. Badges update during play and after claims, including while their menu is closed.
- Mission rows show progress bars and distinguish CLAIM, IN PROGRESS and DONE. The daily action changes to COLLECTED TODAY after collecting.
- The trial timer changes to orange in its final ten seconds.

The pass changes presentation only; progression, rewards, unlock conditions and the save format are unchanged.

## Verification

Unity built the Windows development player successfully with **zero warnings and zero errors**. Its editor checks passed, including the 600-second model run, reward progression, trial deadlines and save recovery.

The isolated built-player interaction run exited 0: **94 PASS entries, zero runtime errors**. Checks include daily badge availability/removal, duplicate reward prevention, tool delivery and drag/drop, input interruption, trials, save persistence and four viewport profiles (reference portrait, wider phone, inset phone and tablet). The normal player save was not modified.

Inspected rendered captures of equipment selection, missions, daily collection and gameplay: labels fit and the new controls remain readable. Captures come from the running camera and UGUI, not physical mobile devices. The interaction run includes capture overhead and is not a performance benchmark; Android build/device verification remains outstanding.

Evidence: `runtime-checks.txt`, `checks.txt`, `build.txt`, `runtime_equipment_selected.png`, `runtime_missions_ready.png`, `runtime_daily_collected.png`, `layout_reference_play.png`.
