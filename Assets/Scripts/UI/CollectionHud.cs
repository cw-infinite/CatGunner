using UnityEngine;
using UnityEngine.UI;
namespace VerdantTrail {
 public sealed partial class FeatureHud {
  GameObject collection;Text collectionCount,collectionDetail;Image collectionIcon;
  Button[] collectionCards;Text[] collectionLabels;int selectedCollection;
  void BuildCollection(){
   collection=Overlay("Collection overlay",.20f,.65f,out var c);
   Text("Collection heading",c,.04f,.025f,.92f,.065f,"TOOL COLLECTION",27);
   collectionCount=Text("Discovery count",c,.05f,.10f,.9f,.055f,"",19);
   collectionCards=new Button[equipment.catalog.weapons.Length];collectionLabels=new Text[collectionCards.Length];
   for(int i=0;i<collectionCards.Length;i++){
    int id=i;var b=Button("",c,.065f+i*.45f,.19f,.42f,.21f,()=>{selectedCollection=id;RefreshCollection();});b.name="Collection tool "+i;
    var icon=ToolIcon(b.transform,.20f,.05f,.6f,.42f);icon.color=i==1?new Color(1,.72f,.35f):Color.white;
    collectionCards[i]=b;collectionLabels[i]=Text("Discovery",b.transform,.03f,.50f,.94f,.44f,"",19);
   }
   var detail=Panel("Tool details",c,.06f,.44f,.88f,.37f,paper);
   collectionIcon=ToolIcon(detail.transform,.34f,.02f,.32f,.22f);
   collectionDetail=Text("Collection stats",detail.transform,.035f,.25f,.93f,.72f,"",21);
   Button("BACK TO GEAR",c,.18f,.865f,.64f,.085f,()=>{Close(collection);OpenWeapons();});
   collection.SetActive(false);
  }
  void OpenCollection(){Close(weapons);collection.SetActive(true);collection.transform.SetAsLastSibling();hud.ResetMovement();RefreshCollection();}
  void RefreshCollection(){
   int discovered=0;for(int i=0;i<collectionCards.Length;i++){
    bool owned=sim.save.ownedWeapons[i]>0;if(owned)discovered++;
    collectionLabels[i].text=equipment.catalog.weapons[i].label+"\n"+(owned?"DISCOVERED":"NOT OWNED");
    collectionCards[i].GetComponent<Image>().color=selectedCollection==i?mint:new Color(.70f,.76f,.61f);
   }
   collectionCount.text=discovered+" / "+collectionCards.Length+" tools discovered";
   var weapon=equipment.catalog.weapons[selectedCollection];int ownedCount=sim.save.ownedWeapons[selectedCollection];
   collectionIcon.color=selectedCollection==1?new Color(1,.72f,.35f):Color.white;
   collectionDetail.text=weapon.label+"\n"+Mathf.RoundToInt(sim.tuning.Damage(sim.save.force)*weapon.damageMultiplier)+" force    |    "+(sim.tuning.Interval(sim.save.tempo)*weapon.intervalMultiplier).ToString("0.00")+"s\nOwned "+ownedCount+"    |    Equipped "+(ownedCount-equipment.Available(selectedCollection))+"\n"+(selectedCollection==0?"Starter tool":"Found in crystal deliveries");
  }
 }
}
