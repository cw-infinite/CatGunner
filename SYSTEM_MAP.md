# Feature and system map

| System/control | Visible | Interacted | Function confirmed | Plan |
|---|---|---|---|---|
| Damage / speed / income cards | Yes throughout | Repeatedly | Core stat buying with cash | Minimum prototype |
| Settings gear | 00:21 | Yes | Audio toggles, save/load-related buttons visible | Minimal local settings/save |
| Attendance | 00:03 | Yes | Seven-day premium rewards | Seven-day local claim cycle implemented; UTC timing inferred |
| Challenge play tile | By 00:42 | Yes | Timed vegetation arena; gem success reward, timeout return | Implemented; success/timeout/exit tested |
| Weapon tile | 03:00 and 05:18 | Yes | Acquisition, inventory, three equip slots, drag-to-equip | Acquisition and two-slot equip implemented; third locked |
| Collection book | 05:21 | Yes | Weapon encyclopedia and stat detail | Two implemented tools inspectable; live stats and ownership |
| Appearance tile | 05:12 | Yes | Selection grid and ownership-based passive summary; user confirmed cumulative bonuses independent of equipped look | Implemented: nine looks; prices/bonuses are prototype tuning |
| Missions/pass | 04:00, 07:24, 10:57 | Yes | Task claim list and reward track | Daily objectives and finite free pass implemented; season/VIP deferred |
| Gem animal utility icon | Later right edge | Not confirmed | Unknown | Do not invent |
| Magnet Auto | Later right edge | Not confirmed | Only label/icon confirmed; could be collection automation | Do not assume movement toggle |
| Damage/income x3 | Later left edge | Not confirmed | Multiplier implied by icon; duration/source unknown | Defer |
| Shop | Later right | Not confirmed | Shop label only | Defer |
| Hunt | Later right, locked | No | Unknown | Defer |
| Boss | Later right, locked | No | Unknown | Defer |
| Mine | Later right, locked | No | Unknown | Defer |
| Rebirth | Later left, locked | No | Unknown | Reserved save field, no reset rules |
| Pet | Later left, locked | No | Unknown | Defer |
| Fish | Later left, locked | No | Unknown | Defer |
| Trophy | Upper-left | Not confirmed | Unknown | Defer |
| Economy-mode pill | From 03:09 | Not confirmed | Unknown | Defer |
| Offline claim | No | No | Unknown | Timestamp only, no reward |

## Progression map
Attendance → solo forest 1-1 → challenge access appears → 1-2 → successful optional timed challenge → 1-3 weapon tutorial and second equipped shooter → broad mostly locked side rails → 1-4 and palm challenge → appearance/collection inspection → 1-5 large gold tree → sand world 2-1 → 2-2 → cactus challenge timeout → 2-3 → 2-4 ongoing at recording end.

Association is observed; exact unlock conditions are inferred. Three equipment slots do not prove three characters are available immediately. Only two active shooters are confidently visible after the tutorial.

## First playable boundary
Implement solo harvesting, movement/manual override, camera, targeting, traveling shots, trees, drops, three upgrades, stage clear and saving. Secondary menu tiles must not masquerade as functioning systems. Developer stage/squad tests are separate from player-facing unlock promises.

## Current extension
The first playable now includes timed harvesting arenas and player-facing equipment with a second shooter. See Validation/PROGRESSION_UPDATE.md for implementation assumptions and test evidence.
