# Portrait layout and safe areas — 2026-09-30

## Changes
The HUD now fits a 588:1280 portrait composition inside Unity's reported screen safe area, instead of stretching with the window. Wider displays retain a centered portrait control area while the world fills the display. Tall screens and asymmetric safe-area insets preserve control proportions. Layout updates when the screen dimensions or safe rectangle change, and active movement input is reset on that change.

Joystick placement, movement radius and equipment drag coordinates now use the fitted HUD. Menu dimming extends across the full screen, including the space outside the control area; interactive menu controls remain inside the fitted safe area. Existing reference-ratio composition is retained.

## Render profiles
| Profile | Render size | Simulated safe rectangle (bottom-left coordinates) |
|---|---|---|
| Reference | 588 × 1280 | Full display |
| Wider phone | 720 × 1280 | Full display |
| Tall phone with cutouts | 600 × 1400 | x=18, y=40, width=564, height=1270 |
| Portrait tablet | 768 × 1024 | Full display |

The runtime harness renders gameplay and equipment for each profile. It projects every active button's corners to verify they stay within the fitted safe viewport, and checks screen-to-HUD coordinates and the drag ghost's alignment with the pointer under an inset. Profiles use render textures and simulated safe rectangles in the Windows player; these are not physical device tests or actual Android notch measurements.

## Validation
The initial player run passed the eight profile-boundary checks and both inset input checks along with the prior gameplay checks. Inspection found that the original modal shade stopped at the control-area edges; this was corrected to fill the display. The final Windows build succeeded with zero warnings/errors. The player exited 0 with all 79 interaction checks passing and zero runtime errors, including all profile-boundary and inset input checks. The final tablet and notched-phone equipment captures were inspected: controls remain inside the safe portrait area, text fits, and modal dimming now covers the entire display. Captures are named layout_reference_*, layout_wide_phone_*, layout_notched_phone_* and layout_tablet_* in this directory.

The expanded interaction run sampled 1,485 frames and averaged 51.5 FPS including numerous captures and transitions, with 30 peak draw calls. This is not a steady-state benchmark.

No balance or save-format changes are included. The most recent steady-state desktop sample (before this layout update) remains 59.8 FPS; layout interaction runs are not equivalent performance measurements. Android compilation, physical multi-touch, device safe areas and landscape behavior remain unverified. Portrait is the intended orientation.
