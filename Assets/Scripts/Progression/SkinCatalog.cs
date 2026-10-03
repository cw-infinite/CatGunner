using System;
using UnityEngine;
namespace VerdantTrail {
 [Serializable] public sealed class SkinDefinition {
  public string label,art;public int price;public float force,tempo,yield;
  public SkinDefinition(string label,string art,int price,float force,float tempo,float yield){this.label=label;this.art=art;this.price=price;this.force=force;this.tempo=tempo;this.yield=yield;}
 }
 [CreateAssetMenu(menuName="Verdant Trail/Skin catalog")]
 public sealed class SkinCatalog:ScriptableObject {
  public SkinDefinition[] skins={
   new SkinDefinition("Trail cat","ranger",0,0,0,0),
   new SkinDefinition("Moss scout","skin0",36,5,3,5),
   new SkinDefinition("Workshop ace","skin1",72,8,5,8),
   new SkinDefinition("Dune explorer","skin2",120,12,8,12),
   new SkinDefinition("Tide sailor","skin3",180,18,12,18),
   new SkinDefinition("Moon alchemist","skin4",260,24,16,24),
   new SkinDefinition("Ember knight","skin5",360,32,22,32),
   new SkinDefinition("Blossom royal","skin6",480,42,28,42),
   new SkinDefinition("Star guardian","skin7",640,55,36,55)
  };
 }
}
