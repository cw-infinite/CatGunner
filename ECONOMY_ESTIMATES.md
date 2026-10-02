# Economy estimates

## Observations
Cash (green notes) is the repeating upgrade resource; blue gems are a scarce reward/acquisition resource. Three upgrade categories remain at bottom in normal and timed modes. Purchases occur every few seconds when affordable. Values and prices are distinct: power/income grow proportionally; the speed index grows more nearly linearly. First weapon details expose a damage baseline and an interval; the second unit shows a lower damage value and somewhat faster interval. Current cash alone cannot determine gross income because purchases are frequent.

Observed snapshots for evidence, not copied balance data: at 00:09 all three stat indices are 100 with initial cost 50; at 03:05 equipment panels show first weapon 100/0.6s and second 80/0.5s; by 11:45 core levels are around 20/20/19. Numeric suffixes abbreviate thousands. No exact hidden formula is established.

## Independent starting tuning (prototype assumptions)
Use asset-configured base hit 92, multiplier 1.077 per Force level; base interval 0.62s divided by (1 + 0.078 × Tempo level); base target payout 16 × 1.15^(stage-1) × 1.077^Yield. Upgrade costs = round(46 × 1.365^level). These deliberately redesigned values approximate the observed purchase rhythm; they are not claimed to recover the source formula.

Per-target HP = 140 × 1.29^(stage-1) × species multiplier (0.85–1.6). Stage counts vary. Initial clearing estimate includes quantized shots, 0.1s flight, approach time and no credit for parallel squad fire. Tuning table is `Analysis/economy_tuning.csv`. Values are proposed, not measurements; actual autonomous runs belong in `Validation/`.

Budget criterion: first upgrade after roughly 2–3 targets; price increases faster than stat strength, so purchases spread out unless later targets pay more. Separate stage clearing from collection timing; do not pay twice. Premium economy, gacha odds, offline earnings and prestige currency are outside the minimum prototype and unknown without additional evidence.

## Follow-up balancing
Compare 00:09–00:54 solo segment at normal speed with fixed purchase schedule; measure targets/minute, seconds between purchases and stage time. Then compare the observed two-unit period separately; do not compensate for missing squad DPS by permanently weakening every later tree. A reference-feeling economy is NOT certified by formulas alone.


## Engine and player comparison
The final in-engine fixed-step simulation produces first/second stage cycles of 50.55s and 70.38s, including 3.5s clear/transfer. The normal-speed Windows player advanced to stage 2 at about 53.7s; its different frame and purchase-check cadence produces a small difference. Reference estimates are approximately 49s and 75s active play plus transitions. Stage 3 is 62.79s in this solo model, and this baseline predates the newly implemented equipment/squad unlock.

Visual comparison showed sparse encounters, so cluster spacing was reduced from 3.9 to 3.0 world units and clusters changed from five to six targets. Raising base HP to 140 retained early pacing. Layout parameters and balance live in HarvestTuning.asset. The first pass also paid too generously; base payout is now 16. These independently designed formulas provide a starting approximation, not recovery of hidden source data.


## Equipment/trial extension
Successful trials grant 36 crystals; deterministic deliveries cost 36. These are prototype tuning assumptions, not recovered premium-economy formulas. Unlocks use stages 2 and 3; initial arena has 24 targets and a 60-second limit. A solo run with level-four upgrades clears it in 28.02 seconds. Later templates contain 40 palms and 64 cacti with higher HP; their full economy and difficulty curves remain uncalibrated. Quick spark uses 0.8 times starter damage and 0.84 times its interval. See Validation/PROGRESSION_UPDATE.md.

## Daily rewards and mission pass
RewardCatalog now defines the local seven-day crystal cycle and daily harvest/stage objectives. Missions award ten points each; the finite pass grants 24/36/48 crystals at 20/40/60 points. Daily attendance awards 12/18/24/30/36/42/60 crystals. These independently chosen amounts add equipment acquisition sources and have not been calibrated across a multi-day playthrough. See Validation/REWARDS_UPDATE.md for reset and migration assumptions.
