$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$unityData='C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Data'
$refs=@('-nologo','-target:library','-langversion:latest','-define:UNITY_EDITOR','-out:Validation/StaticCompile.dll')
Get-ChildItem "$unityData\UnityReferenceAssemblies\unity-4.8-api" -Filter *.dll | ForEach-Object { $refs += '-r:"'+$_.FullName+'"' }
Get-ChildItem "$unityData\Managed\UnityEngine" -Filter *.dll | ForEach-Object { $refs += '-r:"'+$_.FullName+'"' }
Get-ChildItem "$unityData\Managed" -Filter UnityEditor*.dll | Where-Object { $_.Name -ne 'UnityEditor.dll' } | ForEach-Object { $refs += '-r:"'+$_.FullName+'"' }
$refs += '-r:"'+$unityData+'\UnityReferenceAssemblies\unity-4.8-api\Facades\netstandard.dll"'
$refs += '-r:"'+$unityData+'\Resources\PackageManager\ProjectTemplates\libcache\com.unity.template.2d-cross-platform-2d-6.1.6\ScriptAssemblies\UnityEngine.UI.dll"'
Get-ChildItem "$root\Assets" -Filter *.cs -Recurse | ForEach-Object { $refs+='"'+$_.FullName+'"' }
$refs | Set-Content "$root\Validation\compile.rsp"
'' | Set-Content Validation/static-compile.txt
& "$unityData\NetCoreRuntime\dotnet.exe" "$unityData\DotNetSdkRoslyn\csc.dll" '@Validation/compile.rsp' 2>&1 | Tee-Object -FilePath Validation/static-compile.txt
if($LASTEXITCODE -eq 0){"PASS: all runtime and editor C# sources compile against Unity managed assemblies." | Add-Content Validation/static-compile.txt}
exit $LASTEXITCODE


