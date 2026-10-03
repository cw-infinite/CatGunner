using System;
using UnityEngine;
namespace VerdantTrail {
 public sealed class EquipmentSystem {
  public readonly EquipmentCatalog catalog;readonly HarvestSimulation sim;
  readonly int[] activeSlots={0,-1,-1};
  public int SelectedWeapon=-1;public event Action Changed;
  public bool Unlocked=>sim.save.highestStage>=catalog.unlockStage;
  public EquipmentSystem(HarvestSimulation model,EquipmentCatalog data){sim=model;catalog=data;sim.equipment=this;RefreshSquad();}
  public int Available(int id){int n=sim.save.ownedWeapons[id];foreach(int equipped in sim.save.equippedWeapons)if(equipped==id)n--;return n;}
  public int InventoryCount{get{int count=0;for(int i=0;i<sim.save.ownedWeapons.Length;i++)count+=Available(i);return count;}}
  public bool BuyWeapon(int id){if(!Unlocked||id<=0||id>=catalog.weapons.Length||sim.save.gems<catalog.weapons[id].price||InventoryCount>=catalog.inventoryCapacity)return false;sim.save.gems-=catalog.weapons[id].price;sim.save.ownedWeapons[id]++;Changed?.Invoke();return true;}
  public bool BuyDelivery(){if(!Unlocked||sim.save.gems<catalog.deliveryCost||InventoryCount>=catalog.inventoryCapacity)return false;sim.save.gems-=catalog.deliveryCost;sim.save.ownedWeapons[1]++;Changed?.Invoke();return true;}
  public bool Equip(int id,int slot){if(!Unlocked||id<0||id>=catalog.weapons.Length||slot<0||slot>2||Available(id)<1)return false;sim.save.equippedWeapons[slot]=id;SelectedWeapon=-1;RefreshSquad();Changed?.Invoke();return true;}
  public bool Unequip(int slot){if((slot<1||slot>2)||sim.save.equippedWeapons[slot]<0||InventoryCount>=catalog.inventoryCapacity)return false;sim.save.equippedWeapons[slot]=-1;RefreshSquad();Changed?.Invoke();return true;}
  public int WeaponIdForUnit(int unit){int n=0;foreach(int id in sim.save.equippedWeapons){if(id<0)continue;if(n++==unit)return id;}return 0;}
  public WeaponDefinition ForUnit(int unit)=>catalog.weapons[WeaponIdForUnit(unit)];
  public void RefreshSquad(){
   var previous=(UnitState[])sim.units.Clone();var oldSlots=(int[])activeSlots.Clone();int count=0;
   for(int slot=0;slot<3;slot++)if(sim.save.equippedWeapons[slot]>=0){
    int old=Array.IndexOf(oldSlots,slot);activeSlots[count]=slot;
    sim.units[count]=old>=0?previous[old]:new UnitState{position=sim.units[0].position+new UnityEngine.Vector2(count==1?-1:1,-.6f),target=-1,aim=UnityEngine.Vector2.right,cooldown=count*.17f};count++;
   }
   for(int i=count;i<3;i++)activeSlots[i]=-1;sim.unitCount=count;
  }
 }
}
