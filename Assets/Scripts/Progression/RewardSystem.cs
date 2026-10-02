using System;
namespace VerdantTrail {
 public sealed class RewardSystem {
  readonly HarvestSimulation sim;readonly Func<long> clock;
  public readonly RewardCatalog catalog;public event Action Claimed;
  public bool MissionsUnlocked=>sim.save.highestStage>=catalog.missionUnlockStage;
  public RewardSystem(HarvestSimulation model,RewardCatalog data,Func<long> utcSeconds=null){
   sim=model;catalog=data;clock=utcSeconds??(()=>DateTimeOffset.UtcNow.ToUnixTimeSeconds());
   sim.TargetDestroyed+=OnHarvest;sim.StageCleared+=OnStage;RefreshDay();
  }
  long Day=>clock()/86400;
  public long SecondsUntilReset=>86400-clock()%86400;
  public void RefreshDay(){
   long today=Day;if(today<=sim.save.missionDay)return;
   sim.save.missionDay=today;sim.save.dailyHarvests=0;sim.save.dailyStages=0;sim.save.missionClaims=0;
  }
  void OnHarvest(){RefreshDay();sim.save.dailyHarvests=Math.Min(1000000,sim.save.dailyHarvests+1);}
  void OnStage(){RefreshDay();sim.save.dailyStages=Math.Min(1000000,sim.save.dailyStages+1);}
  public int MissionGoal(int index)=>index<3?catalog.harvestGoals[index]:catalog.stageGoals[index-3];
  public int MissionProgress(int index)=>index<3?sim.save.dailyHarvests:sim.save.dailyStages;
  public bool MissionClaimed(int index)=>(sim.save.missionClaims&(1<<index))!=0;
  public bool CanClaimMission(int index)=>index>=0&&index<6&&MissionsUnlocked&&!MissionClaimed(index)&&MissionProgress(index)>=MissionGoal(index);
  public bool ClaimMission(int index){
   RefreshDay();if(!CanClaimMission(index))return false;
   sim.save.missionClaims|=1<<index;sim.save.passPoints=Math.Min(1000000,sim.save.passPoints+catalog.pointsPerMission);Claimed?.Invoke();return true;
  }
  public bool PassClaimed(int index)=>(sim.save.passClaims&(1<<index))!=0;
  public bool CanClaimPass(int index)=>index>=0&&index<catalog.passGoals.Length&&MissionsUnlocked&&!PassClaimed(index)&&sim.save.passPoints>=catalog.passGoals[index];
  public bool ClaimPass(int index){RefreshDay();if(!CanClaimPass(index))return false;sim.save.passClaims|=1<<index;sim.save.gems+=catalog.passGems[index];Claimed?.Invoke();return true;}
  public int DailyIndex=>sim.save.attendanceClaims%catalog.dailyGems.Length;
  public bool CanClaimDaily=>Day>sim.save.lastAttendanceDay;
  public bool ClaimDaily(){
   if(!CanClaimDaily)return false;int reward=catalog.dailyGems[DailyIndex];
   sim.save.lastAttendanceDay=Day;sim.save.attendanceClaims++;sim.save.gems+=reward;Claimed?.Invoke();return true;
  }
 }
}
