using System;
using UnityEngine;
namespace VerdantTrail.Editor {
 public static class RewardValidation {
  public static string Run(HarvestTuning tuning){
   void Require(bool ok,string label){if(!ok)throw new Exception("Reward check: "+label);}
   long now=20000L*86400+100;var catalog=ScriptableObject.CreateInstance<RewardCatalog>();
   var save=SaveStore.Sanitize(new SaveData{version=3,stage=3,highestStage=3,gems=5});var sim=new HarvestSimulation(tuning,save);var rewards=new RewardSystem(sim,catalog,()=>now);
   Require(save.version==4&&save.gems==5&&save.dailyHarvests==0,"v3 migration preserves wallet");
   Require(!rewards.ClaimMission(0)&&!rewards.ClaimPass(0),"Incomplete rewards unavailable");
   sim.Damage(0,sim.targets[0].hp);sim.Damage(0,999);Require(save.dailyHarvests==1,"One count per destroyed target");
   for(int i=1;i<sim.total;i++)sim.Damage(i,sim.targets[i].hp);sim.Step(.01f);Require(save.dailyStages==1,"Normal stage counts once");sim.Step(.01f);Require(save.dailyStages==1,"No duplicate clear");
   sim.Jump(3);int stages=save.dailyStages;sim.EnterChallenge();for(int i=0;i<sim.total;i++)sim.Damage(i,sim.targets[i].hp);sim.Step(.01f);Require(save.dailyStages==stages,"Trials count plants but not normal stage clears");sim.LeaveChallenge();
   save.dailyHarvests=100;Require(rewards.ClaimMission(0)&&!rewards.ClaimMission(0),"Mission claim once");Require(rewards.ClaimMission(3)&&save.passPoints==20,"Stage mission adds pass points");
   int gems=save.gems;Require(rewards.ClaimPass(0)&&!rewards.ClaimPass(0)&&save.gems==gems+catalog.passGems[0],"Pass payout once");
   Require(rewards.ClaimDaily()&&!rewards.ClaimDaily(),"Daily payout once");
   var copy=SaveStore.Sanitize(JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(save)));var restored=new RewardSystem(new HarvestSimulation(tuning,copy),catalog,()=>now);
   Require(!restored.ClaimDaily()&&!restored.ClaimMission(0)&&!restored.ClaimPass(0),"Claims persist across serialization");
   now+=86400;rewards.RefreshDay();Require(save.dailyHarvests==0&&save.dailyStages==0&&save.missionClaims==0&&save.passPoints==20&&rewards.PassClaimed(0),"UTC rollover resets missions only");
   Require(rewards.ClaimDaily()&&save.attendanceClaims==2,"Next day advances attendance");now-=86400;Require(!rewards.ClaimDaily(),"Clock rollback cannot reclaim attendance");rewards.RefreshDay();Require(save.missionDay==20001,"Clock rollback cannot reset missions");
   now+=86400;for(int i=2;i<7;i++){now+=86400;Require(rewards.ClaimDaily(),"Remaining attendance days");}Require(save.attendanceClaims==7&&rewards.DailyIndex==0&&!rewards.CanClaimDaily,"Seven-day cycle waits until tomorrow");now+=86400;Require(rewards.ClaimDaily()&&save.attendanceClaims==8,"Next cycle starts");
   return "PASS: reward save migration, kill/clear counters, claim eligibility, single payouts, serialized claims, UTC reset, clock rollback and seven-day cycle.";
  }
 }
}
