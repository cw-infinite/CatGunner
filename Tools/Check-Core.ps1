$ErrorActionPreference='Stop'
$unityData='C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Data'
$argsList=@('-nologo','-target:exe','-langversion:latest','-out:Validation/CoreChecks.dll')
Get-ChildItem "$unityData\NetCoreRuntime\shared\Microsoft.NETCore.App\6.0.21" -Filter *.dll | Where-Object { ($_.Name -like 'System.*' -and $_.Name -notlike '*.Native.dll') -or $_.Name -in @('mscorlib.dll','netstandard.dll','Microsoft.CSharp.dll') } | ForEach-Object { $argsList+='-r:"'+$_.FullName+'"' }
$argsList+=@('Tools/StandaloneChecks/UnityMathShim.cs','Tools/StandaloneChecks/Program.cs','Assets/Scripts/Core/HarvestTuning.cs','Assets/Scripts/Combat/HarvestSimulation.cs','Assets/Scripts/Saving/SaveStore.cs','Assets/Scripts/Progression/EquipmentSystem.cs','Assets/Scripts/Progression/EquipmentCatalog.cs')
$argsList | Set-Content Validation/core-checks.rsp
'{"runtimeOptions":{"tfm":"net6.0","framework":{"name":"Microsoft.NETCore.App","version":"6.0.21"}}}' | Set-Content Validation/CoreChecks.runtimeconfig.json
& "$unityData\NetCoreRuntime\dotnet.exe" "$unityData\DotNetSdkRoslyn\csc.dll" '@Validation/core-checks.rsp'
if($LASTEXITCODE -ne 0){exit $LASTEXITCODE}
& "$unityData\NetCoreRuntime\dotnet.exe" Validation/CoreChecks.dll
exit $LASTEXITCODE

