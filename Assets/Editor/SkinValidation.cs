using System;
using UnityEngine;
namespace VerdantTrail.Editor {
 public static class SkinValidation {
  public static string Run(HarvestTuning tuning){
   int checks=0;void Require(bool ok,string message){checks++;if(!ok)throw new Exception("Skin check: "+message);}
   var catalog=ScriptableObject.CreateInstance<SkinCatalog>();
   var save=SaveStore.Sanitize(JsonUtility.FromJson<SaveData>("{\"version\":4,\"stage\":3,\"highestStage\":3,\"gems\":108,\"ownedWeapons\":[1,2],\"equippedWeapons\":[0,1,-1]}"));
   Require(save.version==SaveStore.CurrentVersion&&save.ownedSkins==1&&save.equippedSkin==0,"Old save receives starter only");
   Require(save.ownedWeapons.Length==4&&save.ownedWeapons[1]==2&&save.equippedWeapons[1]==1,"Expanding weapon catalog preserves inventory and loadout");
   var sim=new HarvestSimulation(tuning,save);var skins=new SkinSystem(sim,catalog);float damage=sim.EffectiveDamage,interval=sim.Interval;double reward=sim.Reward;
   Require(!skins.Equip(2)&&!skins.Buy(-1)&&!skins.Buy(99),"Unowned equip and invalid purchases rejected");
   Require(skins.Buy(1)&&save.gems==72&&skins.Equipped==0,"Purchase debits once without changing appearance");
   Require(!skins.Buy(1)&&save.gems==72,"Repeat purchase cannot charge again");
   Require(Math.Abs(sim.EffectiveDamage-damage*1.05f)<.001&&Math.Abs(sim.Interval-interval/1.03f)<.001&&Math.Abs(sim.Reward-reward*1.05)<.001,"All three bonuses apply while starter is worn");
   Require(skins.Buy(2)&&save.gems==0&&skins.ForceBonus==13&&skins.TempoBonus==8&&skins.YieldBonus==13,"Owned bonuses add across skins");
   float buffed=sim.EffectiveDamage;Require(skins.Equip(2)&&skins.Equip(0)&&sim.EffectiveDamage==buffed,"Changing appearance retains all bonuses");
   Require(!skins.Buy(3)&&save.gems==0,"Unaffordable skin cannot debit");
   skins.Equip(2);var restored=SaveStore.Sanitize(JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(save)));var restoredSkins=new SkinSystem(new HarvestSimulation(tuning,restored),catalog);
   Require(restoredSkins.Owns(1)&&restoredSkins.Owns(2)&&restoredSkins.Equipped==2&&restoredSkins.ForceBonus==13,"Ownership, appearance and derived bonuses survive JSON roundtrip");
   var invalid=SaveStore.Sanitize(new SaveData{ownedSkins=0,equippedSkin=8});Require(invalid.ownedSkins==1&&invalid.equippedSkin==0,"Unowned selection repaired");
   // Exercise actual projectile creation and target reward calculation, not just displayed totals.
   sim.automate=false;sim.units[0].position=sim.targets[0].position+Vector2.left;sim.units[0].target=0;sim.units[0].cooldown=0;sim.Step(.001f);
   bool shot=false;foreach(var projectile in sim.shots)if(projectile.active){shot=true;Require(Math.Abs(projectile.damage-buffed)<.001,"Projectile carries ownership force bonus");break;}Require(shot,"Combat fired with skin bonuses");
   Require(Math.Abs(sim.units[0].cooldown-sim.Interval)<.001,"Combat cooldown uses tempo bonus");
   double expected=Math.Round(sim.Reward*(sim.targets[0].kind==0?1:1.6));sim.Damage(0,float.MaxValue);double drops=0;foreach(var drop in sim.drops)if(drop.active)drops+=drop.value;Require(Math.Abs(drops-expected)<.001,"Harvest drops include ownership yield bonus");
   var gear=new EquipmentSystem(sim,ScriptableObject.CreateInstance<EquipmentCatalog>());save.gems=270;
   Require(gear.BuyWeapon(2)&&gear.BuyWeapon(3)&&save.gems==0,"New weapons debit configured prices");
   Require(gear.Equip(3,0)&&gear.ForUnit(0)==gear.catalog.weapons[3],"New weapon equips and supplies combat modifiers");
   var gearReload=SaveStore.Sanitize(JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(save)));Require(gearReload.ownedWeapons[2]==1&&gearReload.ownedWeapons[3]==1&&gearReload.equippedWeapons[0]==3,"New weapon ownership and loadout persist");
   for(int i=1;i<catalog.skins.Length;i++){var current=catalog.skins[i];var previous=catalog.skins[i-1];Require(current.price>previous.price&&current.force>previous.force&&current.tempo>previous.tempo&&current.yield>previous.yield,"Skin price and all bonuses rise with tier");}
   return "PASS: "+checks+" skin/expanded-weapon checks: migration, debit, cumulative ownership bonuses, cosmetic equip, combat effects, persistence and increasing tiers.";
  }
 }
}
