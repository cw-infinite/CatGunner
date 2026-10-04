# UI layout reconstruction
Coordinates: full 588 × 1280 recording; x/y top-left normalized in percent. Accuracy roughly ±1–2 percentage points. Brown y0–7.3 strip may include safe-area presentation: retain comparable breathing room without treating it as playable ground.

| Element | x% | y% | width% | height% | Behavior |
|---|---:|---:|---:|---:|---|
| Progress track | 0 | 7.3 | 100 | 1.7 | Filled left-to-right; centered percentage |
| Settings icon | 1.5 | 9.8 | 6.4 | 3.2 | Opens compact modal |
| Achievement icon | 9 | 9.8 | 6.4 | 3.2 | Function beyond icon unverified |
| World node track | 25 | 10 | 50 | 2.5 | Five positions, current marker |
| Stage label | 38 | 12.4 | 24 | 2.2 | World-sector identifier |
| Cash pill | 79 | 10 | 20 | 2.8 | Icon then right-aligned amount |
| Gem pill | 79 | 13.3 | 20 | 2.8 | Same hierarchy |
| Later economy-mode button | 42 | 15 | 16 | 2.8 | Function not demonstrated |
| Later missions icon | 92 | 17 | 8 | 4.6 | Full-height mission panel |
| Primary ground view | 0 | 9 | 100 | 76 | HUD overlays world; not a hard clipped rectangle |
| Leader | 44 | 43 | 12 | 6 | Feet near 48%, shadow below |
| Left rail later | 0 | 48 | 12 | 36 | Two boosters then four menu tiles |
| Right rail later | 87 | 48 | 13 | 36 | Utility controls then five tiles |
| Upgrade card 1 | 6.5 | 86.6 | 28 | 8 | Label/level just above, stat large, price pill at base |
| Upgrade card 2 | 36 | 86.6 | 28 | 8 | ~1.5–2% gaps |
| Upgrade card 3 | 65.8 | 86.6 | 28 | 8 | Same dimensions |
| Early challenge control | 87 | 78.5 | 13 | 5 | Appears after early progress |
| Arena timer | 20 | 11.5 | 54 | 2 | Cyan timer replaces stage path |
| Arena Exit | 0 | 79.5 | 12 | 4 | Bottom-left above upgrades |
| Floating joystick | variable | around 70 | ~23 | ~11 | Only visible while dragging; do not make permanent |

## Panels
- Weapon panel: starts near y31%, fills to bottom, top three large equipment cards across width, inventory grid below y54%, acquisition and collection controls at bottom. Overlay dims game. Drag inventory item to equipment slot; same-grade merging is instructed by panel text but not demonstrated.
- Appearance panel: top near y29%, three-stat summary, four-column scrolling grid. Detail dialog centered. Selected item has checkmark, locked entries darkened.
- Challenge dialog: centered, compact illustration header, objective, reward, affirmative button, close at top-right.
- Pass/missions: near-full-screen purple panel, rewards along top, two reward lanes, scrolling mission rows with claim buttons; return close at upper-right.
- Settings: small centered panel with audio controls and save/load-related actions; simulation continues underneath.
- Clear feedback: thin full-width striped banner moves into upper-middle, confetti from both sides, then pale full-screen transfer state with small center character.

Original implementation uses independently drawn geometric icons and different typography/palette details. Reference frames are not production textures.

## Implemented wardrobe and collection - 2026-10-03

Skin occupies the left rail at normalized y=.710. Its fitted panel contains ownership bonus totals, a three-column nine-look grid, selected-look stats, wallet, a buy/equip action and close. The smaller implemented catalog fits without scrolling; the reference has a larger four-column wardrobe. Collection uses a two-by-two four-gun grid, selected-gun details, purchase action and return. Skin and collection menus receive inset-phone layout checks; skin also receives a tablet capture.

## Illustrated HUD pass - 2026-10-03

Rail buttons are 14.5% wide and 6% high. From bottom upward: Gun/Skin/Fish/Pet on the left; Play/Mine/Boss/Hunt on the right. Daily and Tasks remain above them. Locked entries display padlocks. The five-node grove track spans x=.30 to .70, centered with the stage label; a cat head marks the current sector. Settings and developer tools use illustrated icon buttons. Bundled Lilita One text is white with a near-black outline. Upgrade cards show enlarged stat icons and banknote price icons. World health bars are wider and thin; orange-white shots have trails and muzzle flashes.
