$ErrorActionPreference='Stop'
$project=Split-Path -Parent $PSScriptRoot
$editor='C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Unity.exe'
$buildArgs=@('-batchmode','-nographics','-projectPath',('"'+$project+'"'),'-executeMethod','VerdantTrail.Editor.ProjectSetup.Build','-quit','-logFile',('"'+(Join-Path $project 'Validation\unity-build.log')+'"'))
$process=Start-Process -FilePath $editor -ArgumentList $buildArgs -WindowStyle Hidden -PassThru
$process.WaitForExit()
if($process.ExitCode -ne 0){throw "Unity build failed (exit $($process.ExitCode)). See Validation/unity-build.log."}

