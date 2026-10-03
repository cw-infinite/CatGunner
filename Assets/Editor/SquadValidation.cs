using System;
using UnityEngine;
namespace VerdantTrail.Editor {
 public static class SquadValidation {
  public static string Run(HarvestTuning tuning){
   int checks=0;void Require(bool ok,string message){checks++;if(!ok)throw new Exception("Squad check: "+message);}
   var data=SaveStore.Sanitize(new SaveData{version=5,stage=3,highestStage=3,gems=126});
   var sim=new HarvestSimulation(tuning,data);var gear=new EquipmentSystem(sim,ScriptableObject.CreateInstance<EquipmentCatalog>());
   Require(data.version==6&&sim.unitCount==1,"Previous save migrates without granting cats or gear");
   Require(gear.BuyDelivery()&&gear.BuyWeapon(2)&&data.gems==0,"Buy distinct gear for two followers");
   Require(gear.Equip(1,1)&&gear.Equip(2,2)&&sim.unitCount==3,"Three filled slots recruit three cats");
   Require(gear.WeaponIdForUnit(0)==0&&gear.WeaponIdForUnit(1)==1&&gear.WeaponIdForUnit(2)==2,"Each cat maps to its own gun");
   Require(!gear.Equip(2,1)&&!gear.Equip(2,3)&&sim.unitCount==3,"Unowned duplicate and fourth slot rejected");
   sim.automate=false;
   for(int i=0;i<3;i++){sim.units[i].position=sim.targets[0].position+Vector2.left*(1+i*.1f);sim.units[i].target=0;sim.units[i].cooldown=0;}
   sim.Step(.001f);int shots=0;bool[] damageSeen=new bool[3];
   foreach(var shot in sim.shots)if(shot.active){shots++;for(int i=0;i<3;i++)if(Math.Abs(shot.damage-sim.EffectiveDamage*gear.catalog.weapons[i].damageMultiplier)<.001)damageSeen[i]=true;}
   Require(shots==3&&damageSeen[0]&&damageSeen[1]&&damageSeen[2],"Three cats fire independently with their gun damage");
   for(int i=0;i<3;i++)Require(Math.Abs(sim.units[i].cooldown-sim.Interval*gear.catalog.weapons[i].intervalMultiplier)<.001,"Each cat uses its own fire interval");
   Require(gear.Unequip(1)&&sim.unitCount==2&&gear.WeaponIdForUnit(1)==2&&gear.Available(1)==1,"Middle removal preserves last cat's gun and returns gear");
   var gap=SaveStore.Sanitize(JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(data)));var gapSim=new HarvestSimulation(tuning,gap);var gapGear=new EquipmentSystem(gapSim,gear.catalog);
   Require(gapSim.unitCount==2&&gapGear.WeaponIdForUnit(1)==2&&gap.equippedWeapons[1]==-1,"Save reload preserves empty middle slot");
   Require(gear.Equip(1,1)&&sim.unitCount==3,"Re-equipping middle slot restores three cats");
   var reload=SaveStore.Sanitize(JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(data)));var reloadSim=new HarvestSimulation(tuning,reload);var reloadGear=new EquipmentSystem(reloadSim,gear.catalog);
   Require(reloadSim.unitCount==3&&reloadGear.WeaponIdForUnit(2)==2,"All three cats and distinct guns survive reload");
   Require(sim.EnterChallenge()&&sim.unitCount==3,"Three cats enter trial");sim.LeaveChallenge();Require(sim.unitCount==3&&gear.WeaponIdForUnit(2)==2,"Three-cat trial return retains loadout");
   Require(gear.Unequip(2)&&sim.unitCount==2&&gear.Available(2)==1,"Third cat removal returns gear");Require(!gear.Unequip(0),"Leader cannot be removed");
   var full=SaveStore.Sanitize(new SaveData{stage=3,highestStage=3,ownedWeapons=new[]{15,0,0,0},equippedWeapons=new[]{0,0,0}});var fullSim=new HarvestSimulation(tuning,full);var fullGear=new EquipmentSystem(fullSim,gear.catalog);
   Require(fullGear.InventoryCount==12&&fullSim.unitCount==3&&full.ownedWeapons[0]==15,"Twelve spares plus three equipped copies survive sanitization");
   Require(!fullGear.Unequip(2)&&fullSim.unitCount==3,"Full inventory removal cannot lose gear");
   var invalid=SaveStore.Sanitize(new SaveData{ownedWeapons=new[]{1,1,0,0},equippedWeapons=new[]{0,1,1}});Require(invalid.equippedWeapons[1]==1&&invalid.equippedWeapons[2]==-1,"Extra equipped copy is rejected during load");
   return "PASS: "+checks+" three-cat checks: acquisition, independent combat stats, gaps, removals, reload, trials, capacity and duplicate ownership.";
  }
 }
}
