# Validation commands

- `powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Build-Windows.ps1`: successful Unity import, in-engine core checks and Windows development build; see build.txt and unity-build.log.
- `powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Check-Compile.ps1`: all C# sources compile against installed Unity managed assemblies; see static-compile.txt.
- `powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Check-Core.ps1`: standalone core checks with Unity math shim; see standalone-checks.txt.
- `Builds/Windows/VerdantTrail.exe -validatePrototype`: live event/raycast interaction, isolated JSON roundtrip/migration, squad and stage-transition checks; see runtime-checks.txt.
- `Builds/Windows/VerdantTrail.exe -capturePrototype`: 78-second normal-speed run with balanced purchases, camera-rendered captures and telemetry; see runtime-capture.txt and runtime-telemetry.csv.

The earlier missing-license prerequisite was resolved by the user. Android Build Support is still absent. Keep DLLs and data/runtime folders beside the executable.
