# Reference match checklist

MATCHED = directly verified against a reference observation. CLOSE = bounded approximation with evidence. NEEDS WORK = missing, mismatched or unverified. UNKNOWN = reference evidence insufficient.

| Category | Status | Evidence / limitation |
|---|---|---|
| Video coverage and system discovery | MATCHED | Full 11:52 duration sampled, including menus, success and timeout |
| Stationary targets / automatic shooting | CLOSE | Implemented, rendered and exercised in the 78-second player run |
| Original assets | MATCHED | Procedural artwork and synthesized sounds; no reference media under Assets |
| Portrait camera / diagonal canyon | CLOSE | Actual captures inspected; camera follows without zoom/rotation; connected uneven cliff rims and rock details added; geometry remains simplified |
| Leader position / sprite scale | CLOSE | Centered, slightly above mid-screen; silhouette enlarged after comparison, character shape differs |
| Manual drag + auto approach | UNKNOWN | Drag/release tested; exact reference automation policy remains uncertain |
| Target retention / independent squad targets | CLOSE | Deterministic persistence and player-facing second-shooter equip tested |
| Traveling projectile / damage / reward integrity | CLOSE | Core checks pass; projectile/drop visuals and higher-contrast number shadows inspected |
| Three upgrade categories and layout | CLOSE | All three UGUI controls tested by raycast/click; affordability and portrait layout inspected |
| Early stage and purchase cadence | CLOSE | In-engine cycles 50.55s / 70.38s; actual player's first advance ~53.7s; schedules differ from recording |
| Stage clear / short transfer | CLOSE | Clear, transfer and next-stage resumption tested and captured; simpler banner/effects |
| Later squad progression | CLOSE | Crystal delivery and second-slot equip tested; third slot and merging deferred |
| Five-sector world progression | CLOSE | Model advances through five sectors then changes biome; world-one gold tree implemented and clear/transfer tested |
| Timed harvesting arenas | CLOSE | Countdown, trees/palms/cacti, reward, timeout and grove return implemented; arena tuning inferred |
| Weapons / appearance / missions | NEEDS WORK | Basic equipment implemented; collection viewer implemented; missions/free pass implemented; merging and appearance remain deferred |
| Enemy attacks / player health | UNKNOWN | Not demonstrated; not invented |
| Conventional boss mode | UNKNOWN | Locked icon only; no attack sequence observed |
| Offline rewards / prestige / pets | UNKNOWN | No confirmed functional demonstration |
| Audio fidelity | UNKNOWN | Reference audio not auditioned; original placeholder tones only |
| Save migration and sanitization | CLOSE | Actual Unity JSON/file roundtrip, v1/v2/v3 migration, equipment/reward persistence, backup/temp recovery and future-version protection pass; power-loss/relaunch soak pending |
| Pooling / performance | CLOSE | Fixed core pools; reused terrain; final desktop sample 59.8 FPS / 22 peak draws; mobile and long-duration profiling pending |
| Android / iOS | NEEDS WORK | Editor license now active; Android module still absent; no mobile build |

The minimum Windows mechanical prototype is playable and engine-tested. The overall recreation remains incomplete; composition and early cadence are approximate, with original placeholder visuals and secondary systems deferred. See `Validation/VISUAL_COMPARISON.md` for inspected images and `Validation/REPORT.md` for test boundaries.

Collection and world-one finale evidence: Validation/COLLECTION_FINALE_UPDATE.md. Current desktop interaction sample is transition-heavy; older steady-state figures are retained only as baseline measurements.

Daily reward and mission implementation, assumptions and test boundaries: Validation/REWARDS_UPDATE.md. Seasonal resets, VIP rewards and appearance unlock rules remain unverified.

Portrait usability now includes a fitted safe-area HUD and simulated reference, wide-phone, notched-phone and tablet profiles. See Validation/LAYOUT_UPDATE.md. These Windows render checks do not replace physical Android testing.
