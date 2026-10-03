# Skin ownership and content expansion — 2026-10-03

## Reference and scope

The user's explanation is now the specification for skin ownership bonuses. The supplied video's 05:12 frame also explicitly says effects apply without equipping the skin, and shows separate Power, Speed and Money totals. The 05:15 starter detail shows zero bonuses. The previous assumption that the entire skin system was unknown was too broad.

Exact later-skin prices, bonuses and purchase currency are not established by those two frames. This implementation uses crystals, additive percentage bonuses and progressively increasing tiers as explicit prototype tuning. The new two-gun direct-purchase offers are original content additions requested by the user, not a claim that this exact shop or these stats exist in the recording.

## Playable additions

- SKINS opens a nine-look wardrobe: the starter plus eight generated costumes. Select a card to inspect its three bonuses and price. BUY grants ownership and bonuses without changing appearance; EQUIP LOOK changes appearance only. All purchased bonuses remain active with any outfit, including the starter. The whole squad uses the chosen look.
- The first two purchases cost 36 and 72 crystals. Together they grant Force +13%, Tempo +8%, Yield +13%. Force multiplies base upgraded damage; Tempo divides the shot interval by 1.08; Yield multiplies target rewards before rounding. Bonuses stack additively within each stat, and are derived from ownership rather than saved as separate accumulated numbers.
- Four distinct weapon sprites replace tint-only variants. Trail pulse and Quick spark retain their previous combat multipliers. Briar burst (90 crystals, 1.35 damage / 0.92 interval multiplier) and Sun flare (180 crystals, 1.8 / 0.8) expand the collection. Purchases add a spare tool for normal inventory equip. The existing 36-crystal Quick spark delivery remains available.
- Eight new scenery props dress forest, flowering-grove and sand environments and arena edges. Stages 3–5 get a softer ground palette and more blossom shrubs. These are decorative additions, not new collisions, enemies or authored campaign levels.
- Outfit art, weapon art and scenery are original generated assets in three new transparent atlases. See `Assets/Resources/Art/EXPANSION_PROMPTS.md` for exact prompts and tool provenance.
- Texture imports retain their original non-power-of-two dimensions, preventing aspect distortion of the gun sheet. Drag previews use the selected gun's actual sprite.

## Persistence

Save version 5 adds an ownership bitmask and equipped skin ID. Versions 1–4 migrate to starter ownership and preserve currencies, upgrades, rewards and previous weapon counts/loadout. The weapon ownership array expands to four entries without discarding the first two. Invalid equipped skins fall back to the starter. Purchases and outfit changes request immediate saves in normal play. Validation uses isolated saves and never edits the user's normal save.

Skin prices and bonuses live in `Assets/Resources/SkinCatalog.asset`; weapon values live in `EquipmentCatalog.asset`. IDs are stable catalog indices; future reordering would require migration.

## Validation

Final Windows build: zero warnings and zero errors. Final isolated player: exit 0, **167 PASS entries and zero runtime errors**. Unity also passes **26 dedicated skin/expanded-weapon assertions**. Gallery, shop and battlefield captures were inspected after the final atlas import correction; labels fit and sprite proportions are preserved.

Unity editor checks cover migration, invalid IDs, unaffordable and repeat purchases, cumulative bonuses while wearing the starter, outfit changes, JSON roundtrips, actual projectile damage/cooldown, reward drops, new weapon purchases/equips and increasing skin tiers. The standalone core suite passes 36,056 assertions with unchanged default ten-minute progression (stage 8, 491 targets, 1,508 shots, 46 upgrades).

Built-player checks cover gallery interaction, crystal debit, permanent bonus totals, outfit changes, filesystem persistence, four weapon cards, buying/equipping Briar burst and new menu bounds on inset-phone/tablet profiles. Captures are rendered by the real player camera and UGUI. Android performance and device testing remain outstanding; capture-heavy runs do not establish mobile frame rate.

Visual gaps remain: static costume poses use the existing bob/recoil effects; cliff art and combat composition still need refinement. This does not complete the reference's entire wardrobe or gun catalog.

During the final rebuild, one isolated save-recovery test encountered a file-replacement failure ("Unable to remove the file to be replaced"). A rerun passed without changing production save code or weakening the recovery assertions. This intermittent filesystem failure remains worth monitoring during storage/relaunch soak testing.
