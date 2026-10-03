using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
namespace VerdantTrail.Editor {
 public static class ProjectSetup {
  [MenuItem("Verdant Trail/Create playable scene")]
  public static void Prepare() {
   Directory.CreateDirectory("Assets/Scenes");
   if(!File.Exists("Assets/Resources/HarvestTuning.asset")){var data=ScriptableObject.CreateInstance<HarvestTuning>();AssetDatabase.CreateAsset(data,"Assets/Resources/HarvestTuning.asset");}
   if(!File.Exists("Assets/Resources/EquipmentCatalog.asset"))AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EquipmentCatalog>(),"Assets/Resources/EquipmentCatalog.asset");
   if(!File.Exists("Assets/Resources/RewardCatalog.asset"))AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RewardCatalog>(),"Assets/Resources/RewardCatalog.asset");
   if(!File.Exists("Assets/Resources/SkinCatalog.asset"))AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SkinCatalog>(),"Assets/Resources/SkinCatalog.asset");
   var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
   new GameObject("Verdant Trail bootstrap").AddComponent<GameRoot>();
   EditorSceneManager.SaveScene(scene,"Assets/Scenes/Harvest.unity");
   EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Harvest.unity",true)};
   PlayerSettings.companyName="Independent Grove Studio";PlayerSettings.productName="Verdant Trail";
   PlayerSettings.defaultScreenWidth=470;PlayerSettings.defaultScreenHeight=1024;
   PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.resizableWindow=true;
   PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;PlayerSettings.allowedAutorotateToLandscapeLeft=false;PlayerSettings.allowedAutorotateToLandscapeRight=false;
   PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android,"com.independentgrove.verdanttrail");
   PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
   PlayerSettings.runInBackground=true;
   AssetDatabase.SaveAssets();
  }
  static void Require(bool ok,string message){if(!ok)throw new Exception("Validation failed: "+message);}
  [MenuItem("Verdant Trail/Run deterministic checks")]
  public static void Validate() {
   Directory.CreateDirectory("Validation");var t=AssetDatabase.LoadAssetAtPath<HarvestTuning>("Assets/Resources/HarvestTuning.asset");if(t==null)t=ScriptableObject.CreateInstance<HarvestTuning>();
   var log=new StringBuilder();log.AppendLine(SkinValidation.Run(t));log.AppendLine(ProgressionValidation.Run(t));log.AppendLine(RewardValidation.Run(t));log.AppendLine(DeadlineValidation.Run(t));log.AppendLine(SaveRecoveryChecks.Run("Validation"));var s=new HarvestSimulation(t,new SaveData());
   Require(!s.Buy(0),"Cannot buy without cash");s.save.cash=1000;double cost=t.Cost(0);Require(s.Buy(0)&&s.save.force==1&&s.save.cash==1000-cost,"Purchase debits exactly once");
   Require(t.Interval(4)<t.Interval(0),"Tempo shortens interval");Require(t.Damage(4)>t.Damage(0),"Force raises damage");
   var idle=new HarvestSimulation(t,new SaveData());idle.automate=false;var original=idle.units[0].position;
   idle.manualInput=Vector2.left;idle.Step(.1f);Require(idle.units[0].position.x<original.x,"Manual movement works");
   idle.manualInput=Vector2.zero;original=idle.units[0].position;idle.Step(.1f);Require(idle.units[0].position==original,"Manual release stops when approach disabled");
   var reward=new HarvestSimulation(t,new SaveData());reward.automate=false;
   double expected=0;for(int i=0;i<reward.total;i++){expected+=Math.Round(t.Reward(1,0)*(reward.targets[i].kind==0?1:1.6));reward.Damage(i,float.MaxValue);reward.Damage(i,float.MaxValue);}
   for(int i=0;i<130;i++)reward.Step(1f/60);Require(Math.Abs(reward.save.cash-expected)<.01,"Drops credited once including full pool overflow");Require(reward.cleared==reward.total,"Dead targets counted once");
   log.AppendLine("PASS: affordability, debit, stat scaling, manual override/release, duplicate damage, drop accounting and pool overflow.");
   var migrated=SaveStore.Sanitize(new SaveData{version=1,stage=-9,cash=double.NaN,force=-2});Require(migrated.version==SaveStore.CurrentVersion&&migrated.stage==1&&migrated.cash==0&&migrated.force==0,"Save migration and sanitization");
   bool rejected=false;try{SaveStore.Sanitize(new SaveData{version=900});}catch(InvalidDataException){rejected=true;}Require(rejected,"Future saves rejected");
   log.AppendLine("PASS: v1 migration, malformed number sanitization, future-version rejection.");
   s=new HarvestSimulation(t,new SaveData());int serial=s.stageSerial,last=1;float start=0,purchaseTimer=0;double minCash=double.MaxValue;int maxShots=0,maxDrops=0;
   var csv=new StringBuilder("stage,seconds,force_level,tempo_level,yield_level,cash,total_earned\n");
   for(int i=0;i<60*600;i++) {
    s.Step(1f/60);purchaseTimer+=1f/60;
    if(purchaseTimer>=1){purchaseTimer=0;int cat=s.save.force<=s.save.tempo&&s.save.force<=s.save.yield?0:s.save.tempo<=s.save.yield?1:2;s.Buy(cat);}
    int active=0;foreach(var shot in s.shots)if(shot.active)active++;maxShots=Math.Max(maxShots,active);active=0;foreach(var drop in s.drops)if(drop.active)active++;maxDrops=Math.Max(maxDrops,active);minCash=Math.Min(minCash,s.save.cash);
    if(serial!=s.stageSerial){csv.AppendLine($"{last},{s.elapsed-start:0.00},{s.save.force},{s.save.tempo},{s.save.yield},{s.save.cash:0.00},{s.earned:0.00}");start=s.elapsed;last=s.save.stage;serial=s.stageSerial;}
   }
   Require(s.save.stage>=4,"Autonomous harvesting advances at least three stages in ten minutes");Require(minCash>=0,"Economy stays nonnegative");
   log.AppendLine($"PASS: 600-second deterministic run. Reached stage {s.save.stage}; {s.targetsDestroyed} targets; {s.shotsFired} shots; {s.purchases} purchases. Peak shots {maxShots}; drops {maxDrops}.");
   File.WriteAllText("Validation/simulation.csv",csv.ToString());File.WriteAllText("Validation/checks.txt",log.ToString());Debug.Log(log);
   var table=new StringBuilder("stage,targets,target_hp_base,target_reward_base,assumed_force,assumed_tempo,nominal_dps,next_upgrade_cost,ideal_shooting_seconds\n");
   for(int i=1;i<=10;i++){int level=(i-1)*3;float dps=t.Damage(level)/t.Interval(level);table.AppendLine($"{i},{t.TargetCount(i)},{t.Health(i):0.00},{t.Reward(i,level):0.00},{level},{level},{dps:0.00},{t.Cost(level):0.00},{t.TargetCount(i)*t.Health(i)*1.15f/dps:0.00}");}
   File.WriteAllText("Analysis/economy_tuning.csv",table.ToString());
  }
  public static void Build() {
   Prepare();Validate();Directory.CreateDirectory("Builds/Windows");
   var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/Harvest.unity"},locationPathName="Builds/Windows/VerdantTrail.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed: "+report.summary.result);
   File.WriteAllText("Validation/build.txt",$"Unity {Application.unityVersion}\nResult: {report.summary.result}\nBytes: {report.summary.totalSize}\nWarnings: {report.summary.totalWarnings}\nErrors: {report.summary.totalErrors}\n");
  }
 }
}

