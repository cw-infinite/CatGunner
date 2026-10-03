using System;
namespace VerdantTrail {
 public sealed class SkinSystem {
  readonly HarvestSimulation sim;public readonly SkinCatalog catalog;
  public float ForceBonus {get;private set;}public float TempoBonus {get;private set;}public float YieldBonus {get;private set;}
  public event Action Changed;
  public SkinSystem(HarvestSimulation sim,SkinCatalog catalog){this.sim=sim;this.catalog=catalog;sim.skins=this;RefreshBonuses();}
  public bool Owns(int id)=>id>=0&&id<catalog.skins.Length&&(sim.save.ownedSkins&(1<<id))!=0;
  public int Equipped=>sim.save.equippedSkin;
  public string EquippedArt=>catalog.skins[Equipped].art;
  public bool CanBuy(int id)=>id>0&&id<catalog.skins.Length&&!Owns(id)&&sim.save.gems>=catalog.skins[id].price;
  public bool Buy(int id){if(!CanBuy(id))return false;sim.save.gems-=catalog.skins[id].price;sim.save.ownedSkins|=1<<id;RefreshBonuses();Changed?.Invoke();return true;}
  public bool Equip(int id){if(!Owns(id)||Equipped==id)return false;sim.save.equippedSkin=id;Changed?.Invoke();return true;}
  void RefreshBonuses(){ForceBonus=TempoBonus=YieldBonus=0;for(int i=0;i<catalog.skins.Length;i++)if(Owns(i)){var s=catalog.skins[i];ForceBonus+=s.force;TempoBonus+=s.tempo;YieldBonus+=s.yield;}}
 }
}
