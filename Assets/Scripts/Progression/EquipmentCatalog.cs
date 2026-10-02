using System;
using UnityEngine;
namespace VerdantTrail {
 [Serializable] public sealed class WeaponDefinition {
  public string label;public float damageMultiplier,intervalMultiplier;
  public WeaponDefinition(string name,float damage,float interval){label=name;damageMultiplier=damage;intervalMultiplier=interval;}
 }
 [CreateAssetMenu(menuName="Verdant Trail/Equipment catalog")]
 public sealed class EquipmentCatalog:ScriptableObject {
  public int unlockStage=3,deliveryCost=36,inventoryCapacity=12;
  public WeaponDefinition[] weapons={new WeaponDefinition("Trail pulse",1,1),new WeaponDefinition("Quick spark",.8f,.84f)};
 }
}
