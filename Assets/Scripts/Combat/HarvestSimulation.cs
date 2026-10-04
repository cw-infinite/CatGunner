using System;
using UnityEngine;
namespace VerdantTrail {
 public enum StagePhase { Harvest, Clear, Transfer, ChallengeResult }
 public struct TargetState { public bool active;public int id,kind;public Vector2 position;public float hp,maxHp,hit; }
 public struct ShotState { public bool active;public int target,weaponId;public Vector2 position,aim;public float damage,life; }
 public struct DropState { public bool active;public Vector2 position,velocity;public float age;public double value; }
 public struct PopupState { public bool active,income;public Vector2 position;public float age;public string text; }
 public struct SparkState { public bool active;public Vector2 position,velocity;public float age; }
 public struct UnitState { public Vector2 position,aim;public int target;public float cooldown,flash,walk; }
 public sealed class HarvestSimulation {
  public readonly HarvestTuning tuning;
  public EquipmentSystem equipment;public SkinSystem skins;
  public float EffectiveDamage=>tuning.Damage(save.force)*(1+(skins?.ForceBonus??0)/100);
  public float Interval=>tuning.Interval(save.tempo)/(1+(skins?.TempoBonus??0)/100);
  public double Reward=>tuning.Reward(save.stage,save.yield)*(1+(skins?.YieldBonus??0)/100);
  public bool InChallenge {get;private set;}
  public bool ChallengeWon {get;private set;}
  public float ChallengeRemaining {get;private set;}
  public int ChallengeIndex {get;private set;}
  TargetState[] suspendedTargets;UnitState[] suspendedUnits;int suspendedTotal,suspendedCleared;
  public bool ChallengeUnlocked=>save.highestStage>=tuning.challengeUnlockStage;
  void SettleDrops(){for(int i=0;i<drops.Length;i++)if(drops[i].active){save.cash+=drops[i].value;earned+=drops[i].value;drops[i].active=false;}}
  void ClearTransient(){Array.Clear(shots,0,shots.Length);Array.Clear(drops,0,drops.Length);Array.Clear(popups,0,popups.Length);Array.Clear(sparks,0,sparks.Length);}
  public bool EnterChallenge(){
   if(!ChallengeUnlocked||InChallenge||phase!=StagePhase.Harvest)return false;
   SettleDrops();suspendedTargets=(TargetState[])targets.Clone();suspendedUnits=(UnitState[])units.Clone();suspendedTotal=total;suspendedCleared=cleared;
   InChallenge=true;ChallengeWon=false;ChallengeRemaining=tuning.challengeSeconds;ChallengeIndex=Math.Min(save.challengeWins,tuning.challengeCounts.Length-1);
   Array.Clear(targets,0,targets.Length);ClearTransient();total=Math.Min(targets.Length,tuning.challengeCounts[ChallengeIndex]);cleared=0;
   for(int i=0;i<total;i++){float angle=i*2.399963f;float radius=1.5f+(tuning.arenaRadius-2)* (float)Math.Sqrt((i+.5f)/total);int kind=ChallengeIndex==0?i%2:ChallengeIndex==1?3:4;float hp=tuning.Health(save.stage)*tuning.challengeHealth[ChallengeIndex];targets[i]=new TargetState{active=true,id=i,kind=kind,position=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius,hp=hp,maxHp=hp};}
   for(int i=0;i<units.Length;i++)units[i]=new UnitState{position=new Vector2(i==2?1:-i,i==0?0:-.6f),target=-1,aim=Vector2.right};phaseTime=0;stageSerial++;return true;
  }
  void FinishChallenge(bool won){if(!InChallenge||phase==StagePhase.ChallengeResult)return;SettleDrops();ChallengeWon=won;if(won){save.gems+=tuning.challengeGemReward;save.challengeWins++;}phase=StagePhase.ChallengeResult;phaseTime=0;}
  public void LeaveChallenge(){if(!InChallenge)return;SettleDrops();Array.Copy(suspendedTargets,targets,targets.Length);Array.Copy(suspendedUnits,units,units.Length);total=suspendedTotal;cleared=suspendedCleared;ClearTransient();InChallenge=false;phase=StagePhase.Harvest;phaseTime=0;stageSerial++;suspendedTargets=null;suspendedUnits=null;}

  public readonly SaveData save;
  public readonly TargetState[] targets=new TargetState[128];
  public readonly ShotState[] shots=new ShotState[96];
  public readonly DropState[] drops=new DropState[192];
  public readonly PopupState[] popups=new PopupState[128];
  public readonly SparkState[] sparks=new SparkState[96];
  public readonly UnitState[] units=new UnitState[3];
  public StagePhase phase;public float phaseTime,elapsed;
  public int total,cleared,unitCount=1,stageSerial,shotsFired,targetsDestroyed,purchases;
  public double earned; public bool automate=true;
  public Vector2 manualInput;
  public event Action<Vector2,int> ProjectileImpact;
  public event Action ShotFired,TargetDestroyed,StageCleared,UpgradeBought;
  int popupCursor,sparkCursor;System.Random random;
  public HarvestSimulation(HarvestTuning config,SaveData data){tuning=config;save=data;BeginStage();}
  public float Progress=>total==0?0:(float)cleared/total;
  public static Vector2 Direction=>new Vector2(1,.48f).normalized;
  public static Vector2 Normal=>new Vector2(-Direction.y,Direction.x);
  public void BeginStage() {
   // Collect pending notes before switching maps so visual travel cannot lose earned cash.
   for(int i=0;i<drops.Length;i++)if(drops[i].active){save.cash+=drops[i].value;earned+=drops[i].value;}
   Array.Clear(targets,0,targets.Length);Array.Clear(shots,0,shots.Length);Array.Clear(drops,0,drops.Length);
   Array.Clear(popups,0,popups.Length);Array.Clear(sparks,0,sparks.Length);
   random=new System.Random(7103+save.stage*7919);total=tuning.TargetCount(save.stage);cleared=0;
   for(int i=0;i<total;i++) {
    int cluster=i/Math.Max(1,tuning.targetsPerCluster);float along=tuning.firstClusterDistance+cluster*tuning.clusterSpacing+(float)random.NextDouble()*tuning.clusterDepth;
    float across=tuning.clusterLateralOffset+(float)random.NextDouble()*tuning.clusterWidth;
    int kind=save.stage>5?3:save.stage==1?i%2:i%3;
    float hp=tuning.Health(save.stage)*(kind==0?.85f:kind==1?1.25f:1.6f);
    targets[i]=new TargetState{active=true,id=i,kind=kind,position=Direction*along+Normal*across,hp=hp,maxHp=hp};
   }
   // Only the first world's gold-tree finale is confirmed by the recording.
   if(save.stage==5){float end=0;for(int i=0;i<total-1;i++)end=Mathf.Max(end,Vector2.Dot(targets[i].position,Direction));float hp=tuning.Health(save.stage)*tuning.finaleHealthMultiplier;targets[total-1]=new TargetState{active=true,id=total-1,kind=5,position=Direction*(end+4)+Normal*.5f,hp=hp,maxHp=hp};}
   for(int i=0;i<units.Length;i++)units[i]=new UnitState{position=new Vector2(i==2?.7f:-i*.7f,i==0?0:-.65f),target=-1,cooldown=i*.17f,aim=Vector2.right};
   phase=StagePhase.Harvest;phaseTime=0;stageSerial++;
  }
  public bool Buy(int category) {
   int level=category==0?save.force:category==1?save.tempo:save.yield;
   if(level>=100)return false;double cost=tuning.Cost(level);if(save.cash<cost)return false;
   save.cash-=cost;if(category==0)save.force++;else if(category==1)save.tempo++;else save.yield++;
   purchases++;UpgradeBought?.Invoke();return true;
  }
  public int Nearest(Vector2 p,float radius) {
   float best=radius*radius;int result=-1;
   for(int i=0;i<total;i++)if(targets[i].active){float d=(targets[i].position-p).sqrMagnitude;if(d<best){best=d;result=i;}}
   return result;
  }
  public void Step(float dt) {
   if(dt<=0||float.IsNaN(dt)||float.IsInfinity(dt))return;
   if(InChallenge&&phase==StagePhase.Harvest){
    if(ChallengeRemaining<=0){FinishChallenge(cleared>=total);return;}
    // Only simulate time that actually remains; a long frame cannot grant late hits.
    dt=Mathf.Min(dt,ChallengeRemaining);
   }
   elapsed+=dt;
   if(InChallenge&&phase==StagePhase.Harvest)ChallengeRemaining=Mathf.Max(0,ChallengeRemaining-dt);
   for(int i=0;i<total;i++)targets[i].hit=Mathf.Max(0,targets[i].hit-dt);
   StepFeedback(dt);
   if(phase!=StagePhase.Harvest) {
    phaseTime+=dt;
    if(phase==StagePhase.Clear&&phaseTime>=tuning.clearDuration){phase=StagePhase.Transfer;phaseTime=0;}
    else if(phase==StagePhase.Transfer&&phaseTime>=tuning.transferDuration){save.stage=Math.Min(100,save.stage+1);save.highestStage=Math.Max(save.highestStage,save.stage);BeginStage();}
    return;
   }
   for(int u=0;u<unitCount;u++) {
    ref UnitState unit=ref units[u];unit.cooldown-=dt;unit.flash=Mathf.Max(0,unit.flash-dt);
    if(unit.target<0||!targets[unit.target].active||(targets[unit.target].position-unit.position).sqrMagnitude>tuning.engagementRange*tuning.engagementRange*1.1f)
     unit.target=Nearest(unit.position,tuning.engagementRange);
    Vector2 move=Vector2.zero;
    if(u==0&&manualInput.sqrMagnitude>.01f)move=manualInput;
    else if(automate&&unit.target<0) {
     int next=Nearest(unit.position,10000);
     if(next>=0)move=(targets[next].position-unit.position).normalized;
    }
    if(u>0) {
     Vector2 anchor=units[0].position+new Vector2(u==1?-1.0f:1.0f,-.6f);
     if((anchor-unit.position).sqrMagnitude>3.5f)move=(anchor-unit.position).normalized;
    }
    if(u>0) {
     for(int other=0;other<u;other++){Vector2 apart=unit.position-units[other].position;float distance=apart.magnitude;if(distance<.85f){Vector2 away=distance>.01f?apart/distance:new Vector2(-1,-.2f).normalized;unit.position+=away*(.85f-distance)*Mathf.Min(1,dt*10);}}
    }
    if(move.sqrMagnitude>.01f){unit.position+=move*tuning.movementSpeed*dt;unit.walk+=dt*14;}
    else unit.walk=0;
    if(InChallenge&&unit.position.magnitude>tuning.arenaRadius-.4f)unit.position=unit.position.normalized*(tuning.arenaRadius-.4f);
    if(unit.target>=0) {
     unit.aim=(targets[unit.target].position-unit.position).normalized;
     if(unit.cooldown<=0) {
      Fire(u);unit.cooldown=Interval*(equipment!=null?equipment.ForUnit(u).intervalMultiplier:u==0?1:.86f);unit.flash=.075f;
     }
    }
   }
   for(int i=0;i<shots.Length;i++)if(shots[i].active) {
    ref ShotState shot=ref shots[i];shot.life+=dt;
    Vector2 delta=shot.aim-shot.position;float distance=tuning.projectileSpeed*dt;
    if(delta.sqrMagnitude<=distance*distance) {
     if(shot.target>=0&&targets[shot.target].active){Damage(shot.target,shot.damage);ProjectileImpact?.Invoke(shot.aim,shot.weaponId);}
     shot.active=false;
    }else {shot.position+=delta.normalized*distance;if(shot.life>1)shot.active=false;}
   }
   if(InChallenge){if(cleared>=total)FinishChallenge(true);else if(ChallengeRemaining<=0)FinishChallenge(false);return;}
   if(cleared>=total) {phase=StagePhase.Clear;phaseTime=0;StageCleared?.Invoke();}
  }
  void Fire(int u) {
   for(int i=0;i<shots.Length;i++)if(!shots[i].active) {
    var unit=units[u];shots[i]=new ShotState{active=true,target=unit.target,weaponId=equipment!=null?equipment.WeaponIdForUnit(u):0,position=unit.position+unit.aim*.76f+Vector2.up*.47f,
     aim=targets[unit.target].position+Vector2.up*.38f,damage=EffectiveDamage*(equipment!=null?equipment.ForUnit(u).damageMultiplier:u==0?1:.8f)};
    shotsFired++;ShotFired?.Invoke();return;
   }
  }
  public void Damage(int index,float damage) {
   if(index<0||index>=total||!targets[index].active)return;
   ref TargetState t=ref targets[index];t.hp-=damage;t.hit=.13f;
   Popup(t.position+Vector2.up*(t.kind==5?2.8f:1.4f),damage<1000000?Mathf.CeilToInt(damage).ToString():Format(damage),false);
   for(int j=0;j<4;j++) {int k=sparkCursor++%sparks.Length;float a=(float)random.NextDouble()*Mathf.PI*2;
    sparks[k]=new SparkState{active=true,position=t.position+Vector2.up*.5f,velocity=new Vector2(Mathf.Cos(a),Mathf.Sin(a))*2.5f};}
   if(t.hp>0)return;
   t.active=false;cleared++;targetsDestroyed++;TargetDestroyed?.Invoke();
   double reward=Math.Round(Reward*(t.kind==5?tuning.finaleRewardMultiplier:t.kind==0?1:1.6));
   Popup(t.position+Vector2.up*.6f,"+"+Format(reward),true);
   for(int j=0;j<4;j++) {
    bool stored=false;
    for(int k=0;k<drops.Length;k++)if(!drops[k].active) {
     float a=(float)random.NextDouble()*Mathf.PI*2;
     drops[k]=new DropState{active=true,position=t.position+Vector2.up*.5f,velocity=new Vector2(Mathf.Cos(a),Mathf.Sin(a))*2.6f,value=reward/4};stored=true;break;
    }
    if(!stored){save.cash+=reward/4;earned+=reward/4;}
   }
  }
  void Popup(Vector2 p,string text,bool income){int i=popupCursor++%popups.Length;popups[i]=new PopupState{active=true,position=p,text=text,income=income};}
  void StepFeedback(float dt) {
   for(int i=0;i<popups.Length;i++)if(popups[i].active){popups[i].age+=dt;popups[i].position+=Vector2.up*dt*.65f;if(popups[i].age>.85f)popups[i].active=false;}
   for(int i=0;i<sparks.Length;i++)if(sparks[i].active){sparks[i].age+=dt;sparks[i].position+=sparks[i].velocity*dt;if(sparks[i].age>.2f)sparks[i].active=false;}
   for(int i=0;i<drops.Length;i++)if(drops[i].active) {
    ref DropState d=ref drops[i];d.age+=dt;
    if(d.age<.25f){d.position+=d.velocity*dt;d.velocity*=Mathf.Exp(-8*dt);}
    else {
     int closest=0;for(int u=1;u<unitCount;u++)if((units[u].position-d.position).sqrMagnitude<(units[closest].position-d.position).sqrMagnitude)closest=u;
     d.position=Vector2.MoveTowards(d.position,units[closest].position,dt*(6+d.age*8));
     if((d.position-units[closest].position).sqrMagnitude<.08f||d.age>2){save.cash+=d.value;earned+=d.value;d.active=false;}
    }
   }
  }
  public void Jump(int stage){if(InChallenge)LeaveChallenge();save.stage=Mathf.Clamp(stage,1,100);save.highestStage=Math.Max(save.stage,save.highestStage);BeginStage();}
  public static string Format(double n)=>n>=1000000?(n/1000000).ToString("0.##")+"m":n>=1000?(n/1000).ToString("0.##")+"k":Math.Floor(n).ToString();
 }
}
