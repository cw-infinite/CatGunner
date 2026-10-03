using UnityEngine;
using UnityEngine.UI;
namespace VerdantTrail {
 public sealed partial class FeatureHud {
  GameObject collection;Text collectionCount,collectionDetail;Image collectionIcon;Button collectionBuy;
  Button[] collectionCards;Text[] collectionLabels;int selectedCollection;
  void BuildCollection(){
   collection=Overlay("Collection overlay",.20f,.65f,out var c);
   Text("Collection heading",c,.04f,.025f,.92f,.065f,"TOOL COLLECTION",27);
   collectionCount=Text("Discovery count",c,.05f,.10f,.9f,.055f,"",19);
   collectionCards=new Button[equipment.catalog.weapons.Length];collectionLabels=new Text[collectionCards.Length];
   for(int i=0;i<collectionCards.Length;i++){
    int id=i;var b=Button("",c,.065f+(i%2)*.45f,.18f+(i/2)*.19f,.42f,.175f,()=>{selectedCollection=id;RefreshCollection();});b.name="Collection tool "+i;
    var icon=ToolIcon(b.transform,.20f,.05f,.6f,.42f);icon.sprite=OriginalArt.Get("weapon"+i);icon.color=Color.white;
    collectionCards[i]=b;collectionLabels[i]=Text("Discovery",b.transform,.03f,.50f,.94f,.44f,"",19);
   }
   var detail=Panel("Tool details",c,.06f,.575f,.88f,.205f,paper);
   collectionIcon=ToolIcon(detail.transform,.03f,.15f,.27f,.60f);
   collectionDetail=Text("Collection stats",detail.transform,.32f,.03f,.66f,.94f,"",17);
   collectionBuy=Button("BUY TOOL",c,.18f,.805f,.64f,.065f,()=>{if(equipment.BuyWeapon(selectedCollection))hud.ShowToast("Tool added to inventory");RefreshCollection();});
   Button("BACK TO GEAR",c,.18f,.90f,.64f,.065f,()=>{Close(collection);OpenWeapons();});
   collection.SetActive(false);
  }
  void OpenCollection(){Close(weapons);collection.SetActive(true);collection.transform.SetAsLastSibling();hud.ResetMovement();RefreshCollection();}
  void RefreshCollection(){
   int discovered=0;for(int i=0;i<collectionCards.Length;i++){
    bool owned=sim.save.ownedWeapons[i]>0;if(owned)discovered++;
    collectionLabels[i].text=equipment.catalog.weapons[i].label+"\n"+(owned?"DISCOVERED":"NOT OWNED");
    collectionCards[i].GetComponent<Image>().color=selectedCollection==i?Color.white:new Color(.70f,.76f,.61f);
   }
   collectionCount.text=discovered+" / "+collectionCards.Length+" tools discovered";
   var weapon=equipment.catalog.weapons[selectedCollection];int ownedCount=sim.save.ownedWeapons[selectedCollection];
   collectionIcon.sprite=OriginalArt.Get("weapon"+selectedCollection);collectionIcon.color=Color.white;
   collectionBuy.interactable=selectedCollection>0&&equipment.Unlocked&&sim.save.gems>=weapon.price&&equipment.InventoryCount<equipment.catalog.inventoryCapacity;collectionBuy.GetComponentInChildren<Text>().text=selectedCollection==0?"STARTER TOOL":"BUY ¡¤ "+weapon.price+" CRYSTALS";
   collectionDetail.text=weapon.label+"\n"+Mathf.RoundToInt(sim.EffectiveDamage*weapon.damageMultiplier)+" force    |    "+(sim.Interval*weapon.intervalMultiplier).ToString("0.00")+"s\nOwned "+ownedCount+"    |    Equipped "+(ownedCount-equipment.Available(selectedCollection))+"\n"+(selectedCollection==0?"Starter tool":"Wallet "+sim.save.gems+" crystals");
  }
 }
}
