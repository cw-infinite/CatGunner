using System;
using UnityEngine;
namespace VerdantTrail.Editor {
 public static class ProgressionValidation {
  public static string Run(HarvestTuning tuning){
   void Require(bool ok,string message){if(!ok)throw new Exception("Progression check: "+message);}
   var data=SaveStore.Sanitize(new SaveData{version=2,stage=3,highestStage=3});var sim=new HarvestSimulation(tuning,data);var gear=new EquipmentSystem(sim,ScriptableObject.CreateInstance<EquipmentCatalog>());
   Require(data.version==SaveStore.CurrentVersion&&data.equippedWeapons[0]==0&&sim.unitCount==1,"v2 migration retains starter weapon");
   Require(!gear.BuyDelivery(),"Cannot buy without crystals");data.gems=gear.catalog.deliveryCost;Require(gear.BuyDelivery()&&data.gems==0&&gear.Available(1)==1,"Delivery exact debit");
   Require(gear.Equip(1,1)&&sim.unitCount==2,"Equipping enables follower");Require(!gear.Equip(1,0),"No duplicate use of one item");Require(gear.Unequip(1)&&sim.unitCount==1&&gear.Available(1)==1,"Unequip preserves ownership");gear.Equip(1,1);
   int total=sim.total;sim.Damage(0,sim.targets[0].hp);int cleared=sim.cleared;float hp=sim.targets[1].hp;Require(sim.EnterChallenge(),"Challenge entry");Require(!sim.EnterChallenge(),"Nested challenge rejected");
   int gems=data.gems;for(int i=0;i<sim.total;i++)sim.Damage(i,sim.targets[i].hp);sim.Step(.01f);Require(sim.ChallengeWon&&data.gems==gems+tuning.challengeGemReward,"Success reward");
   sim.Step(10);Require(data.gems==gems+tuning.challengeGemReward,"No duplicate payout");sim.LeaveChallenge();Require(sim.total==total&&sim.cleared==cleared&&!sim.targets[0].active&&sim.targets[1].hp==hp,"Normal progress preserved");
   Require(sim.EnterChallenge(),"Second challenge");gems=data.gems;sim.Step(tuning.challengeSeconds+1);Require(!sim.ChallengeWon&&sim.phase==StagePhase.ChallengeResult&&data.gems==gems,"Timeout no reward");sim.LeaveChallenge();
   sim.EnterChallenge();sim.LeaveChallenge();Require(!sim.InChallenge&&sim.cleared==cleared,"Manual exit preserves progress");
   data.ownedWeapons=new[]{1,0};data.equippedWeapons=new[]{0,0,1};SaveStore.Sanitize(data);Require(data.equippedWeapons[1]==-1&&data.equippedWeapons[2]==-1,"Invalid duplicate loadout repaired");
   var full=SaveStore.Sanitize(new SaveData{stage=3,highestStage=3,gems=100,ownedWeapons=new[]{1,13},equippedWeapons=new[]{1,1,-1}});
   var fullGear=new EquipmentSystem(new HarvestSimulation(tuning,full),gear.catalog);Require(fullGear.InventoryCount==12&&full.ownedWeapons[1]==13,"Full inventory plus equipped tools survive sanitization");Require(!fullGear.BuyDelivery()&&full.gems==100,"Full inventory rejects purchase without debit");
   var natural=new HarvestSimulation(tuning,SaveStore.Sanitize(new SaveData{stage=2,highestStage=2,force=4,tempo=4,yield=4}));
   Require(natural.EnterChallenge(),"Natural trial entry");float elapsed=0;while(natural.phase==StagePhase.Harvest&&elapsed<61){natural.Step(1f/60);elapsed+=1f/60;}Require(natural.ChallengeWon,"First trial beatable at level-four upgrades");
   var finale=new HarvestSimulation(tuning,SaveStore.Sanitize(new SaveData{stage=5,highestStage=5}));finale.automate=false;
   int gold=finale.total-1;Require(finale.targets[gold].kind==5,"Finale target exists");
   for(int i=0;i<gold;i++){Require(finale.targets[i].kind!=5,"Only one gold tree");finale.Damage(i,finale.targets[i].hp);}
   finale.Step(.01f);Require(finale.phase==StagePhase.Harvest&&finale.cleared==gold,"Gold tree blocks clear");
   float originalHp=finale.targets[gold].hp;Require(finale.EnterChallenge(),"Trial from finale");finale.LeaveChallenge();Require(finale.targets[gold].hp==originalHp&&finale.targets[gold].kind==5,"Trial preserves finale");
   for(int i=0;i<130;i++)finale.Step(1f/60);double beforeGold=finale.save.cash;
   finale.Damage(gold,originalHp);finale.Damage(gold,originalHp);for(int i=0;i<130;i++)finale.Step(1f/60);
   Require(Math.Abs(finale.save.cash-beforeGold-Math.Round(tuning.Reward(5,0)*tuning.finaleRewardMultiplier))<.01,"Gold payout once");
   for(int i=0;i<130;i++)finale.Step(1f/60);Require(finale.save.stage==6&&finale.targets[0].kind==3,"Finale reaches sand biome");
   return "Natural first trial: "+elapsed.ToString("0.00")+"s at level-four upgrades.\nPASS: gold finale, clear gate, trial preservation, single payout and sand-biome transition.\nPASS: v2-to-v5 equipment migration, delivery debit, item ownership, equip/unequip, exact grove restoration, challenge success/timeout/exit and single reward payout.";
  }
 }
}
