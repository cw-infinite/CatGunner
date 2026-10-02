# Save recovery and status — 2026-09-30

## Behavior
The game now tries the primary save, then its backup, then a complete interrupted-write file. A valid primary always wins over temporary data. Loading a recovery candidate does not immediately change files. A save from a newer version stops recovery at that candidate and disables writes, protecting it from a downgrade.

On the first save after recovery, a damaged primary is copied to a uniquely named `.damaged-*` file before replacement. A valid backup is retained rather than replaced with the damaged primary. If the recovery source is the temporary file, a `.recovered-*` copy is preserved before that temporary path is reused. Writes flush the serialized data to disk before replacing the primary. A failed temporary write leaves the previous committed primary intact and can be retried.

Missing all save files starts a new game. Malformed, empty or versionless JSON does not silently reset an existing save. If no candidate can be loaded, existing files remain protected and saving stays disabled for the session until a valid load occurs.

Settings now displays save/recovery status and disables its Save button when files are protected. Startup shows a brief recovery or protection message. Automatic saving retains its existing cadence; no balance or save-schema change is included.

## Verification
Nineteen real filesystem checks pass in Unity: first save, corrupted/missing-primary recovery, backup retention, damaged-file archival, loadout and reward-claim preservation, repaired-file reload, preference for committed data, temporary-source recovery, future-version protection, invalid-file protection, failed-write preservation and successful retry. Fixtures live in a unique validation-owned directory and are cleaned up afterward; normal player files are not test inputs.

The standalone core regression check passes 36,056 assertions. The ten-minute deterministic run is unchanged: stage 8, 491 destroyed targets, 1,508 shots and 46 upgrades. The Windows build completed with zero warnings/errors. The built-player run exited 0 with zero runtime errors and passes the 19 filesystem cases alongside all existing gameplay/layout checks. New UI checks confirm saving is disabled for protected files and enabled after recovery. runtime_save_protected.png and runtime_save_recovered.png were inspected: both status messages fit the Settings panel and the button states are clear.

Expected warning logs are produced by deliberately invalid/future files and the simulated failed write. These do not indicate a failed check. The interaction run averaged 50.4 FPS including filesystem tests, screenshots and transitions; it is not a normal-play performance benchmark.

## Limits
Recovery restores the available snapshot; changes newer than that snapshot can be lost. This is local file recovery, not cloud sync or a server transaction history. The checks use actual file I/O and repeated loads, but do not simulate hardware power failure or certify Android/iOS filesystem behavior. Syntax, version and model sanitization cannot detect every possible corruption of otherwise valid data.
