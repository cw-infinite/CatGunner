using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Profiling;
namespace VerdantTrail {
 // Explicit development command-line harness; never runs during ordinary play.
 public sealed class RuntimeProbe : MonoBehaviour {
  GameRoot root;bool checks;float elapsed,purchaseClock;int shotIndex,errors,frames;double frameSum;
  long allocationSum;int allocationSamples;long peakDraws;
  readonly float[] moments={8,25,50,75};readonly StringBuilder report=new StringBuilder();
  readonly StringBuilder telemetry=new StringBuilder("seconds,stage,progress,cash,force,tempo,yield,camera_x,camera_y\n");
  ProfilerRecorder allocations,draws;string directory;float sampleClock;bool clearCaptured,transferCaptured;
  public void Initialize(GameRoot game,bool validate) {
   root=game;checks=validate;directory=Path.GetFullPath(Path.Combine(Application.dataPath,"..","..","..","Validation"));Directory.CreateDirectory(directory);
   Application.logMessageReceived+=OnLog;
   allocations=ProfilerRecorder.StartNew(ProfilerCategory.Memory,"GC Allocated In Frame");draws=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Draw Calls Count");
   if(checks)StartCoroutine(CheckInteractions());else StartCoroutine(WarmCapture());
  }
  IEnumerator WarmCapture(){yield return new WaitForSecondsRealtime(1);Capture("capture_warmup.png");}
  void OnLog(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert){errors++;report.AppendLine("ERROR: "+message);}}
  void Check(bool condition,string label){report.AppendLine((condition?"PASS: ":"FAIL: ")+label);if(!condition)errors++;}
  Button NamedButton(string name){foreach(var b in FindObjectsByType<Button>(FindObjectsInactive.Include,FindObjectsSortMode.None))if(b.name==name)return b;return null;}
  Button ButtonAt(float x,float y) {
   Canvas.ForceUpdateCanvases();var list=new List<RaycastResult>();var data=new PointerEventData(EventSystem.current){position=new Vector2(Screen.width*x,Screen.height*(1-y))};
   root.Hud.Canvas.GetComponent<GraphicRaycaster>().Raycast(data,list);
   foreach(var r in list){var b=r.gameObject.GetComponentInParent<Button>();if(b!=null)return b;}return null;
  }
  void Click(Button b){Check(b!=null,"UI raycast found button");if(b!=null)ExecuteEvents.Execute(b.gameObject,new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left},ExecuteEvents.pointerClickHandler);}
  IEnumerator CheckInteractions() {
   yield return new WaitForSecondsRealtime(2);
   var sim=root.Simulation;sim.automate=false;
   var drag=FindFirstObjectByType<DragSurface>();Vector2 pos=sim.units[0].position;
   var pointer=new PointerEventData(EventSystem.current){pointerId=17,position=new Vector2(Screen.width*.35f,Screen.height*.35f)};
   ExecuteEvents.Execute(drag.gameObject,pointer,ExecuteEvents.pointerDownHandler);pointer.position+=Vector2.left*Screen.width*.12f;
   ExecuteEvents.Execute(drag.gameObject,pointer,ExecuteEvents.dragHandler);
   yield return new WaitForSecondsRealtime(.3f);Check(sim.units[0].position.x<pos.x-.2f,"Drag moves character through live Update");
   ExecuteEvents.Execute(drag.gameObject,pointer,ExecuteEvents.pointerUpHandler);pos=sim.units[0].position;
   yield return new WaitForSecondsRealtime(.2f);Check((sim.units[0].position-pos).sqrMagnitude<.001f,"Release stops manual movement");
   sim.automate=true;sim.save.cash=1000;
   for(int i=0;i<3;i++){int before=i==0?sim.save.force:i==1?sim.save.tempo:sim.save.yield;Click(ButtonAt(.2f+i*.298f,.91f));int after=i==0?sim.save.force:i==1?sim.save.tempo:sim.save.yield;Check(after==before+1,"Upgrade "+i+" responds to UI click");}
   sim.save.cash=0;int level=sim.save.force;Click(ButtonAt(.2f,.91f));Check(sim.save.force==level,"Unaffordable upgrade cannot debit");
   Click(NamedButton("*"));yield return null;Check(root.Hud.IsOverlayOpen,"Settings opens");Capture("runtime_settings.png");
   float beforeTime=sim.elapsed;yield return new WaitForSecondsRealtime(.4f);Check(sim.elapsed>beforeTime+.3f,"Simulation continues beneath settings");
   Click(NamedButton("Resume"));yield return null;Check(!root.Hud.IsOverlayOpen,"Settings closes");
   sim.save.cash=100;var path=Path.Combine(directory,"save-roundtrip-test.json");
   try{report.AppendLine(SaveRecoveryChecks.Run(directory));}catch(Exception e){Check(false,"Save recovery filesystem checks: "+e.Message);}
   SaveStore.SaveTo(sim.save,path);var loaded=SaveStore.LoadFrom(path);
   Check(loaded.force==sim.save.force&&loaded.tempo==sim.save.tempo&&loaded.cash==100,"Unity JSON save/load roundtrip in isolated path");
   File.WriteAllText(path,"{\"version\":99}");var future=SaveStore.LoadFrom(path);Check(!SaveStore.CanWrite,"Future save disables writes");SaveStore.SaveTo(new SaveData(),path);Check(File.ReadAllText(path).Contains("99"),"Future save is not overwritten");
   Click(NamedButton("*"));yield return new WaitForSecondsRealtime(.15f);Check(!NamedButton("Save progress").interactable,"Settings disables saving for protected files");Capture("runtime_save_protected.png");Click(NamedButton("Resume"));yield return null;
   var recoveryPath=Path.Combine(directory,"save-ui-recovery.json");File.WriteAllText(recoveryPath,"{interrupted");File.WriteAllText(recoveryPath+".bak",JsonUtility.ToJson(sim.save));var recovered=SaveStore.LoadFrom(recoveryPath);
   Click(NamedButton("*"));yield return new WaitForSecondsRealtime(.15f);Check(NamedButton("Save progress").interactable&&GameObject.Find("Save status").GetComponent<Text>().text.Contains("Recovered"),"Settings reports recovery and permits saving");Capture("runtime_save_recovered.png");Click(NamedButton("Resume"));yield return null;SaveStore.SaveTo(recovered,recoveryPath);
   File.WriteAllText(path,"{\"version\":1,\"stage\":2,\"cash\":123}");var migrated=SaveStore.LoadFrom(path);Check(migrated.version==4&&migrated.stage==2&&migrated.cash==123,"Unity JSON version-one migration");
   sim.unitCount=2;sim.Jump(3);yield return new WaitForSecondsRealtime(2);Capture("runtime_squad.png");Check(sim.units[0].target>=0||sim.units[1].target>=0,"Two-unit targeting is active");
   for(int i=0;i<sim.total;i++)sim.Damage(i,sim.targets[i].hp);
   yield return new WaitForSecondsRealtime(.3f);Check(sim.phase==StagePhase.Clear,"Clear phase triggered");Capture("runtime_clear.png");
   yield return new WaitForSecondsRealtime(2);Check(sim.phase==StagePhase.Transfer,"Travel follows clear");Capture("runtime_transfer.png");
   yield return new WaitForSecondsRealtime(1.5f);Check(sim.save.stage==4&&sim.phase==StagePhase.Harvest,"Next stage resumes");
   Capture("runtime_next_stage.png");
   sim.unitCount=1;sim.Jump(2);yield return new WaitForSecondsRealtime(.2f);int normalCount=sim.total,normalCleared=sim.cleared;
   Click(NamedButton("TRIAL"));yield return null;Capture("runtime_trial_offer.png");Click(NamedButton("START TRIAL"));yield return new WaitForSecondsRealtime(2);
   Check(sim.InChallenge&&sim.ChallengeRemaining<59,"Trial starts with live countdown");Capture("runtime_trial.png");
   int gemsBefore=sim.save.gems;for(int i=0;i<sim.total;i++)sim.Damage(i,sim.targets[i].hp);
   yield return new WaitForSecondsRealtime(.2f);Check(sim.ChallengeWon&&sim.save.gems==gemsBefore+sim.tuning.challengeGemReward,"Trial success grants crystals once");Capture("runtime_trial_win.png");
   Click(NamedButton("CONTINUE"));yield return null;Check(!sim.InChallenge&&sim.save.stage==2&&sim.total==normalCount&&sim.cleared==normalCleared,"Trial restores exact normal-stage progress");
   sim.Jump(3);yield return new WaitForSecondsRealtime(.2f);Click(NamedButton("GEAR"));yield return null;
   Click(NamedButton("COLLECTION"));yield return new WaitForSecondsRealtime(.15f);Click(NamedButton("Collection tool 1"));yield return null;
   Check(GameObject.Find("Collection stats").GetComponent<Text>().text.Contains("Owned 0"),"Collection previews unowned tool without granting it");Capture("runtime_collection_unowned.png");
   Click(NamedButton("BACK TO GEAR"));yield return null;
   Click(NamedButton("DELIVERY  <> "+root.Equipment.catalog.deliveryCost));yield return null;Check(root.Equipment.Available(1)==1,"Crystal delivery adds one tool");Capture("runtime_equipment_inventory.png");
   var inventory=NamedButton("Inventory 0");var slot=NamedButton("Equipment slot 1");var dragEvent=new PointerEventData(EventSystem.current){pointerId=29,pointerDrag=inventory.gameObject,position=new Vector2(Screen.width*.15f,Screen.height*.35f)};
   ExecuteEvents.Execute(inventory.gameObject,dragEvent,ExecuteEvents.beginDragHandler);ExecuteEvents.Execute(slot.gameObject,dragEvent,ExecuteEvents.dropHandler);ExecuteEvents.Execute(inventory.gameObject,dragEvent,ExecuteEvents.endDragHandler);
   yield return null;Check(sim.unitCount==2&&sim.save.equippedWeapons[1]==1,"Drag/drop equips second tool and recruits shooter");Capture("runtime_equipment_equipped.png");
   Click(NamedButton("COLLECTION"));yield return new WaitForSecondsRealtime(.15f);
   Check(GameObject.Find("Collection stats").GetComponent<Text>().text.Contains("Owned 1    |    Equipped 1"),"Collection shows current ownership and equipped count");Capture("runtime_collection_owned.png");
   Click(NamedButton("BACK TO GEAR"));yield return null;
   SaveStore.SaveTo(sim.save,path);var equippedSave=SaveStore.LoadFrom(path);Check(equippedSave.equippedWeapons[1]==1&&equippedSave.challengeWins==1,"Equipment and challenge wins persist in v4 save");
   Click(NamedButton("X"));yield return new WaitForSecondsRealtime(2);Capture("runtime_equipped_squad.png");
   float usualDuration=sim.tuning.challengeSeconds;sim.tuning.challengeSeconds=1;gemsBefore=sim.save.gems;sim.EnterChallenge();yield return new WaitForSecondsRealtime(1.3f);
   Check(sim.phase==StagePhase.ChallengeResult&&!sim.ChallengeWon&&sim.save.gems==gemsBefore,"Timeout returns no success reward");Capture("runtime_trial_timeout.png");
   Click(NamedButton("CONTINUE"));sim.tuning.challengeSeconds=usualDuration;yield return null;
   sim.EnterChallenge();yield return new WaitForSecondsRealtime(.2f);Click(NamedButton("EXIT"));yield return null;Check(!sim.InChallenge&&sim.save.stage==3,"Manual exit returns to original grove");
   sim.Jump(5);sim.save.force=15;sim.save.tempo=15;int gold=sim.total-1;
   for(int i=0;i<gold;i++)sim.Damage(i,sim.targets[i].hp);
   sim.units[0].position=sim.targets[gold].position+Vector2.left*2.5f+Vector2.down*.8f;sim.units[1].position=sim.units[0].position+Vector2.down*.6f;
   yield return new WaitForSecondsRealtime(.4f);Check(sim.targets[gold].active&&sim.phase==StagePhase.Harvest,"Gold finale survives opening volley and blocks stage clear");Capture("runtime_gold_finale.png");
   float finaleDeadline=Time.realtimeSinceStartup+20;while(sim.phase==StagePhase.Harvest&&Time.realtimeSinceStartup<finaleDeadline)yield return null;
   Check(sim.phase==StagePhase.Clear&&!sim.targets[gold].active,"Squad clears gold tree through normal shooting");Capture("runtime_gold_clear.png");
   yield return new WaitForSecondsRealtime(3.7f);Check(sim.save.stage==6&&sim.targets[0].kind==3,"Gold finale transitions into palm biome");Capture("runtime_world_two.png");
   Click(ButtonAt(.07f,.745f));yield return new WaitForSecondsRealtime(.15f);Capture("runtime_daily_ready.png");
   int dailyBefore=sim.save.gems;Click(NamedButton("CLAIM DAILY"));Click(NamedButton("CLAIM DAILY"));yield return new WaitForSecondsRealtime(.15f);
   Check(sim.save.gems==dailyBefore+root.Rewards.catalog.dailyGems[0]&&sim.save.attendanceClaims==1,"Daily UI grants one reward despite repeated click");Capture("runtime_daily_collected.png");Click(NamedButton("CLOSE DAILY"));yield return new WaitForSecondsRealtime(.15f);
   Click(ButtonAt(.93f,.745f));yield return new WaitForSecondsRealtime(.15f);Capture("runtime_missions_ready.png");
   int pointsBefore=sim.save.passPoints;Click(NamedButton("Claim mission 0"));Click(NamedButton("Claim mission 0"));Click(NamedButton("Claim mission 3"));yield return new WaitForSecondsRealtime(.15f);
   Check(sim.save.passPoints==pointsBefore+2*root.Rewards.catalog.pointsPerMission,"Mission UI claims completed objectives once");
   int passBefore=sim.save.gems;Click(NamedButton("Claim pass 0"));Click(NamedButton("Claim pass 0"));yield return new WaitForSecondsRealtime(.15f);
   Check(sim.save.gems==passBefore+root.Rewards.catalog.passGems[0],"Pass UI pays earned crystals once");Capture("runtime_missions_claimed.png");
   SaveStore.SaveTo(sim.save,path);var rewardSave=SaveStore.LoadFrom(path);Check(rewardSave.version==4&&rewardSave.attendanceClaims==1&&rewardSave.passPoints==20&&rewardSave.passClaims==1&&rewardSave.missionClaims==9,"Reward claims persist in v4 filesystem save");
   Click(NamedButton("CLOSE TASKS"));yield return null;Check(!root.Hud.IsOverlayOpen,"Closing tasks restores battlefield controls");
   yield return CheckLayouts();
   Finish();
  }
  IEnumerator CheckLayouts(){
   int[] widths={588,720,600,768},heights={1280,1280,1400,1024};string[] names={"reference","wide_phone","notched_phone","tablet"};
   for(int i=0;i<widths.Length;i++){
    Rect safe=i==2?new Rect(18,40,564,1270):new Rect(0,0,widths[i],heights[i]);
    Capture("layout_"+names[i]+"_play.png",widths[i],heights[i],safe);yield return null;
    root.Features.OpenWeapons();yield return null;Capture("layout_"+names[i]+"_gear.png",widths[i],heights[i],safe);Click(NamedButton("X"));yield return new WaitForSecondsRealtime(.15f);
   }
   root.Hud.Layout.Apply(Screen.width,Screen.height,new Rect(20,30,Screen.width-40,Screen.height-80));Canvas.ForceUpdateCanvases();
   var content=root.Hud.ContentRoot;Vector2 point=RectTransformUtility.WorldToScreenPoint(null,content.TransformPoint(Vector3.zero));Vector2 local=root.Hud.Layout.LocalPoint(point);
   Check((local-content.rect.size*.5f).sqrMagnitude<.01f,"Safe-area screen-to-HUD drag coordinates roundtrip");
   root.Features.BeginWeaponDrag(0,point);yield return null;var ghost=GameObject.Find("Dragged tool").GetComponent<RectTransform>();Vector2 ghostScreen=RectTransformUtility.WorldToScreenPoint(null,ghost.position);
   Check((point-ghostScreen).sqrMagnitude<1,"Dragged tool remains under pointer with safe-area inset");root.Features.EndWeaponDrag();root.Hud.Layout.Restore();
  }
  void LateUpdate() {
   if(root==null)return;float dt=Time.unscaledDeltaTime;elapsed+=dt;
   if(elapsed>3){frames++;frameSum+=dt;if(allocations.Valid){allocationSum+=allocations.LastValue;allocationSamples++;}if(draws.Valid)peakDraws=Math.Max(peakDraws,draws.LastValue);}
   if(checks)return;
   var sim=root.Simulation;purchaseClock+=dt;
   if(purchaseClock>=1){purchaseClock=0;int category=sim.save.force<=sim.save.tempo&&sim.save.force<=sim.save.yield?0:sim.save.tempo<=sim.save.yield?1:2;sim.Buy(category);}
   sampleClock+=dt;if(sampleClock>=1){sampleClock=0;var c=root.World.Camera.transform.position;telemetry.AppendLine($"{elapsed:0.00},{sim.save.stage},{sim.Progress:0.000},{sim.save.cash:0.00},{sim.save.force},{sim.save.tempo},{sim.save.yield},{c.x:0.000},{c.y:0.000}");}
   if(!clearCaptured&&sim.phase==StagePhase.Clear){Capture("prototype_clear.png");clearCaptured=true;}
   if(!transferCaptured&&sim.phase==StagePhase.Transfer){Capture("prototype_transfer.png");transferCaptured=true;}
   if(shotIndex<moments.Length&&elapsed>=moments[shotIndex])Capture("prototype_"+(shotIndex++)+".png");
   if(elapsed>78)Finish();
  }
  void Capture(string filename,int width=588,int height=1280,Rect? safe=null) {
   var cam=root.World.Camera;var canvas=root.Hud.Canvas;var oldTarget=cam.targetTexture;var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;float oldDistance=canvas.planeDistance;
   var rt=RenderTexture.GetTemporary(width,height,24,RenderTextureFormat.ARGB32);cam.targetTexture=rt;
   canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=1;root.Hud.Layout.Apply(width,height,safe??new Rect(0,0,width,height));
   // Static graphics need a geometry rebuild when capturing at a different resolution.
   foreach(var graphic in canvas.GetComponentsInChildren<Graphic>())graphic.SetAllDirty();
   Canvas.ForceUpdateCanvases();cam.Render();
   if(filename.StartsWith("layout_")){
    bool inside=true;Rect fit=root.Hud.Layout.PixelRect;var corners=new Vector3[4];
    foreach(var button in canvas.GetComponentsInChildren<Button>()){button.GetComponent<RectTransform>().GetWorldCorners(corners);foreach(var corner in corners){Vector3 v=cam.WorldToViewportPoint(corner);float x=v.x*width,y=v.y*height;if(x<fit.xMin-1||x>fit.xMax+1||y<fit.yMin-1||y>fit.yMax+1)inside=false;}}
    Check(inside,"Active controls fit safe portrait viewport: "+filename);
   }
   var active=RenderTexture.active;RenderTexture.active=rt;var texture=new Texture2D(width,height,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply();File.WriteAllBytes(Path.Combine(directory,filename),texture.EncodeToPNG());
   RenderTexture.active=active;cam.targetTexture=oldTarget;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;canvas.planeDistance=oldDistance;root.Hud.Layout.Restore();Destroy(texture);RenderTexture.ReleaseTemporary(rt);Canvas.ForceUpdateCanvases();
  }
  void Finish() {
   report.AppendLine($"Engine: {Application.unityVersion}; resolution: {Screen.width}x{Screen.height}; rendered capture: 588x1280.");
   report.AppendLine($"Errors: {errors}; sampled frames: {frames}; mean FPS including captures: {(frames/Math.Max(.001,frameSum)):0.0}; mean recorded GC bytes/frame: {(allocationSamples==0?0:allocationSum/allocationSamples)}; peak draw calls: {peakDraws}.");
   report.AppendLine("Desktop development build measurement; not an Android performance result. Images rendered by the running game's camera and UGUI into a RenderTexture.");
   File.WriteAllText(Path.Combine(directory,checks?"runtime-checks.txt":"runtime-capture.txt"),report.ToString());if(!checks)File.WriteAllText(Path.Combine(directory,"runtime-telemetry.csv"),telemetry.ToString());
   enabled=false;Application.Quit(errors==0?0:1);
  }
  void OnDestroy(){Application.logMessageReceived-=OnLog;allocations.Dispose();draws.Dispose();}
 }
}
