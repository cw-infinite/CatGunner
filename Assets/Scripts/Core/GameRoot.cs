using System;
using System.IO;
using UnityEngine;
namespace VerdantTrail {
 public sealed class GameRoot : MonoBehaviour {
  public HarvestSimulation Simulation {get;private set;}
  WorldView world;PortraitHud hud;FeatureHud features;EquipmentSystem equipment;AudioSource sound;AudioClip shot,impact,upgrade;
  float saveTimer;bool capture;
  public WorldView World => world;
  public PortraitHud Hud => hud;
  public FeatureHud Features=>features;
  public EquipmentSystem Equipment=>equipment;
  public RewardSystem Rewards {get;private set;}
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Boot(){if(FindFirstObjectByType<GameRoot>()==null)new GameObject("Verdant Trail").AddComponent<GameRoot>();}
  void Awake() {
   Application.targetFrameRate=60;QualitySettings.vSyncCount=0;Screen.orientation=ScreenOrientation.Portrait;
   var tuning=Resources.Load<HarvestTuning>("HarvestTuning");if(tuning==null)tuning=ScriptableObject.CreateInstance<HarvestTuning>();
   string[] args=Environment.GetCommandLineArgs();capture=Array.IndexOf(args,"-capturePrototype")>=0 || Array.IndexOf(args,"-validatePrototype")>=0;
   var data=capture?new SaveData():SaveStore.Load();Simulation=new HarvestSimulation(tuning,data);
   var catalog=Resources.Load<EquipmentCatalog>("EquipmentCatalog");if(catalog==null)catalog=ScriptableObject.CreateInstance<EquipmentCatalog>();equipment=new EquipmentSystem(Simulation,catalog);
   var rewards=Resources.Load<RewardCatalog>("RewardCatalog");if(rewards==null)rewards=ScriptableObject.CreateInstance<RewardCatalog>();Rewards=new RewardSystem(Simulation,rewards);
   Rewards.Claimed+=()=>{if(!capture)SaveStore.Save(Simulation.save);};
   world=new GameObject("Following orthographic camera").AddComponent<WorldView>();world.Initialize(Simulation);
   hud=gameObject.AddComponent<PortraitHud>();hud.Initialize(Simulation);features=gameObject.AddComponent<FeatureHud>();features.Initialize(Simulation,equipment,hud,Rewards);
   if(!capture&&(SaveStore.Recovered||!SaveStore.CanWrite))hud.ShowToast(SaveStore.StatusMessage);
   sound=gameObject.AddComponent<AudioSource>();shot=Tone("Pulse",640,190,.055f,.025f);impact=Tone("Leaf tick",190,80,.065f,.018f);upgrade=Tone("Upgrade",450,1150,.1f,.06f);
   Simulation.ShotFired+=()=>Play(shot);Simulation.TargetDestroyed+=()=>Play(impact);Simulation.UpgradeBought+=()=>Play(upgrade);
   Simulation.StageCleared+=()=>{if(!capture)SaveStore.Save(Simulation.save);};
   if(capture)gameObject.AddComponent<RuntimeProbe>().Initialize(this,Array.IndexOf(args,"-validatePrototype")>=0);
  }
  AudioClip Tone(string name,float start,float end,float seconds,float amplitude) {
   int count=(int)(22050*seconds);float[] samples=new float[count];float phase=0;
   for(int i=0;i<count;i++){float t=(float)i/count;phase+=Mathf.Lerp(start,end,t)/22050*2*Mathf.PI;samples[i]=Mathf.Sin(phase)*(1-t)*(1-t)*amplitude;}
   var clip=AudioClip.Create(name,count,1,22050,false);clip.SetData(samples,0);return clip;
  }
  void Play(AudioClip clip){if(Simulation.save.sound)sound.PlayOneShot(clip);}
  void Update() {
   hud.RefreshLayout();Vector2 movement=hud.IsOverlayOpen?Vector2.zero:hud.Movement;
   if(!hud.IsOverlayOpen){Vector2 keys=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));if(keys.sqrMagnitude>.01f)movement=keys.normalized;}
   Simulation.manualInput=movement;Rewards.RefreshDay();
   float remaining=Mathf.Min(Time.deltaTime,.25f);while(remaining>0){float dt=Mathf.Min(remaining,1f/60);Simulation.Step(dt);remaining-=dt;}
   world.Render(Time.deltaTime);hud.Tick(Time.unscaledDeltaTime);features.Tick(Time.unscaledDeltaTime);
   saveTimer+=Time.unscaledDeltaTime;if(saveTimer>=10){saveTimer=0;if(!capture)SaveStore.Save(Simulation.save);}
   
  }
  void OnApplicationFocus(bool focus){if(!focus&&hud!=null)hud.ResetMovement();if(!focus&&!capture&&Simulation!=null)SaveStore.Save(Simulation.save);}
  void OnApplicationPause(bool pause){if(pause&&!capture&&Simulation!=null)SaveStore.Save(Simulation.save);}
  void OnApplicationQuit(){if(!capture&&Simulation!=null)SaveStore.Save(Simulation.save);}
 }
}
