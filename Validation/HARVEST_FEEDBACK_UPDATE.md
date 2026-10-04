# Harvest feedback — 2026-10-03

Trees now squash slightly when hit and tilt/fade for 0.24 seconds after harvesting. Health bars include an amber segment that catches up to remaining health, with yellow/orange fill at low health. Floating numbers briefly enlarge on appearance and use wider alternating horizontal offsets to help separate simultaneous squad hits.

The effects reuse pooled renderers. Stage changes reset the temporary visual state. Harvest rewards, hit detection, banknote costs, equipment and skin bonuses retain their existing rules.

Runtime coverage checks damaged health display, catch-up, hit squash, hiding health during harvest, completion without an extra harvest, and stage-reset behavior. Captures: `runtime_harvest_feedback.png` and `runtime_harvest_fade.png`.

Final Windows build succeeded with zero warnings/errors. Isolated runtime validation passed 218 checks with zero failures/errors. Inspected hit and fade captures from the running player; existing three-cat equipment, skin, reward and safe-area checks also passed. Player saves were not modified. Android performance remains unverified.
