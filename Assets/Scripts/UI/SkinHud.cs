using UnityEngine;
using UnityEngine.UI;
namespace VerdantTrail {
 public sealed partial class FeatureHud {
  GameObject skinsPanel;Button skinsTile,skinAction;Text skinTotals,skinDetail,skinWallet;
  Button[] skinCards;Text[] skinLabels;Image[] skinFrames;int selectedSkin;
  void BuildSkins(){
   skinsTile=Button("SKINS",canvas.transform,0,.655f,.145f,.05f,()=>{selectedSkin=sim.skins.Equipped;skinsPanel.SetActive(true);skinsPanel.transform.SetAsLastSibling();hud.ResetMovement();RefreshSkins();});
   skinsPanel=Overlay("Skins overlay",.14f,.82f,out var panel);
   Text("Skin heading",panel,.04f,.015f,.92f,.045f,"CAT WARDROBE",25);
   skinTotals=Text("Owned skin bonuses",panel,.04f,.065f,.92f,.05f,"",18);
   Text("Skin ownership rule",panel,.035f,.118f,.93f,.055f,"Bonuses from ALL owned skins stay active.\nWear any look you like.",16);
   int count=sim.skins.catalog.skins.Length;skinCards=new Button[count];skinLabels=new Text[count];skinFrames=new Image[count];
   for(int i=0;i<count;i++){
    int id=i;var card=Button("",panel,.045f+i%3*.31f,.19f+i/3*.155f,.29f,.143f,()=>{selectedSkin=id;RefreshSkins();});card.name="Skin card "+i;UiFinish.Skin(card.GetComponent<Image>(),"card-art");skinCards[i]=card;skinFrames[i]=SelectionFrame(card.transform);
    var icon=Panel("Skin portrait",card.transform,.18f,.04f,.64f,.70f,Color.white);icon.sprite=OriginalArt.Get(sim.skins.catalog.skins[i].art);icon.type=Image.Type.Simple;icon.preserveAspect=true;icon.raycastTarget=false;
    skinLabels[i]=Text("Skin ownership",card.transform,.03f,.76f,.94f,.20f,"",14);
   }
   skinDetail=Text("Skin details",panel,.035f,.665f,.93f,.097f,"",18);
   skinWallet=Text("Skin wallet",panel,.04f,.77f,.92f,.032f,"",17);
   skinAction=Button("BUY SKIN",panel,.17f,.82f,.66f,.075f,()=>{if(sim.skins.Owns(selectedSkin)){if(sim.skins.Equip(selectedSkin))hud.ShowToast("New look equipped · All bonuses retained");}else if(sim.skins.Buy(selectedSkin))hud.ShowToast("Skin owned · Bonuses now active");RefreshSkins();});
   Button("CLOSE SKINS",panel,.26f,.92f,.48f,.053f,()=>Close(skinsPanel));skinsPanel.SetActive(false);
  }
  void RefreshSkins(){
   var skins=sim.skins;skinTotals.text="FORCE +"+skins.ForceBonus+"%   TEMPO +"+skins.TempoBonus+"%   YIELD +"+skins.YieldBonus+"%";
   for(int i=0;i<skinCards.Length;i++){bool owned=skins.Owns(i);skinCards[i].GetComponent<Image>().color=owned?Color.white:new Color(.72f,.72f,.65f);skinFrames[i].gameObject.SetActive(i==selectedSkin);skinLabels[i].text=skins.Equipped==i?"WEARING":owned?"OWNED":skins.catalog.skins[i].price+" CRYSTALS";}
   var entry=skins.catalog.skins[selectedSkin];skinDetail.text=entry.label+"\nForce +"+entry.force+"%  ·  Tempo +"+entry.tempo+"%  ·  Yield +"+entry.yield+"%";
   bool has=skins.Owns(selectedSkin);skinWallet.text="CRYSTALS "+sim.save.gems+(has?"  ·  Ownership bonuses active":sim.save.gems<entry.price?"  ·  Need "+(entry.price-sim.save.gems)+" more":"");
   skinAction.interactable=has?skins.Equipped!=selectedSkin:skins.CanBuy(selectedSkin);skinAction.GetComponentInChildren<Text>().text=has?(skins.Equipped==selectedSkin?"WEARING":"EQUIP LOOK"):"BUY · "+entry.price+" CRYSTALS";
  }
 }
}
