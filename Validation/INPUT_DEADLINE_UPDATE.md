# Input interruption and trial deadline fixes — 2026-10-02

## Changes
- Application pause and focus loss clear held movement, its pointer ownership, any inventory drag ghost, and the selected tool. Closing an equipment-related panel also cancels unfinished dragging.
- Equipment delivery, equip and unequip changes now request an immediate save in normal play, in addition to the existing periodic and lifecycle saves. Validation mode remains isolated from the normal player save.
- A timed trial simulates only the portion of an update remaining before its deadline. A projectile that would arrive after time expires cannot convert the timeout into a win. A hit arriving within the remaining interval still counts, and the success reward remains single-payout.
- Non-positive, NaN and infinite simulation time steps are ignored.

These correct implementation edge cases; no new reference mechanics, balance formulas or save format are introduced. The trial timer still measures simulated active play, not a server or offline clock.

## Verification
Unity's deadline checks exercise a projectile arriving after expiry and one arriving before expiry during an update that crosses the deadline, plus repeated reward processing and invalid time steps. All pass. The standalone core regression suite still passes 36,056 assertions, and the ten-minute model remains at stage 8 with 491 destroyed targets, 1,508 shots and 46 upgrades.

The built-player harness now checks that a second pointer cannot take over or release the active joystick; an application-pause callback clears movement; stale pointer events cannot restart movement; focus loss cancels the dragged tool; and closing equipment clears an unfinished drag. These use Unity lifecycle callbacks and pointer events in the Windows player, not physical Android multi-touch or OS suspend/resume.

The rebuilt Windows player exited 0 with zero runtime errors. All five new input regressions pass alongside the existing gameplay, save recovery and layout checks. The build completed with zero warnings/errors. The capture-heavy interaction run averaged 49.9 FPS; it is not a normal-play or mobile performance benchmark. Physical Android suspend/resume and multi-touch verification remain outstanding.
