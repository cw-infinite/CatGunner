using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace VerdantTrail {
 public sealed class WeaponDrag:MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler {
  public int WeaponId;public FeatureHud Hud;
  public void OnBeginDrag(PointerEventData e){Hud.BeginWeaponDrag(WeaponId,e.position);}
  public void OnDrag(PointerEventData e){Hud.MoveWeaponDrag(e.position);}
  public void OnEndDrag(PointerEventData e){Hud.EndWeaponDrag();}
 }
 public sealed class WeaponSlotDrop:MonoBehaviour,IDropHandler {
  public int Slot;public FeatureHud Hud;
  public void OnDrop(PointerEventData e){var drag=e.pointerDrag!=null?e.pointerDrag.GetComponent<WeaponDrag>():null;if(drag!=null)Hud.EquipWeapon(drag.WeaponId,Slot);}
 }
 public sealed partial class FeatureHud:MonoBehaviour {
  HarvestSimulation sim;EquipmentSystem equipment;PortraitHud hud;Canvas canvas;Font font;
  GameObject weapons,offer,result;Button weaponTile,challengeTile,exit,delivery;Text timer,resultTitle,resultDetail,offerDetail,inventoryHint;
  Image[] slotIcons=new Image[3],itemIcons=new Image[12];
  Image ToolIcon(Transform parent,float x,float y,float w,float h){var image=Rect("Tool icon",parent,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=OriginalArt.Get("gun");image.preserveAspect=true;image.raycastTarget=false;return image;}
  Image timerFill;Text[] slots=new Text[3];Button[] items=new Button[12];Text[] itemLabels=new Text[12];int[] itemIds=new int[12];
  GameObject timerRoot;RectTransform ghost;float refresh;
  readonly Color ink=new Color(.18f,.24f,.22f),paper=new Color(.98f,.92f,.75f),mint=new Color(.56f,.94f,.76f);
  public bool IsOpen=>weapons.activeSelf||offer.activeSelf||result.activeSelf||collection.activeSelf||dailyPanel.activeSelf||missionPanel.activeSelf;
  RectTransform Rect(string name,Transform parent,float x,float y,float w,float h){if(parent==canvas.transform)parent=hud.ContentRoot;var rt=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();rt.SetParent(parent,false);rt.anchorMin=new Vector2(x,1-y-h);rt.anchorMax=new Vector2(x+w,1-y);rt.offsetMin=rt.offsetMax=Vector2.zero;return rt;}
  Image Panel(string name,Transform parent,float x,float y,float w,float h,Color c){var rt=Rect(name,parent,x,y,w,h);var im=rt.gameObject.AddComponent<Image>();im.color=c;im.sprite=OriginalArt.Get("panel");im.type=Image.Type.Sliced;return im;}
  Text Text(string name,Transform parent,float x,float y,float w,float h,string value,int size=21){var rt=Rect(name,parent,x,y,w,h);var t=rt.gameObject.AddComponent<Text>();t.font=font;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.alignment=TextAnchor.MiddleCenter;t.color=ink;t.text=value;t.raycastTarget=false;return t;}
  Button Button(string label,Transform parent,float x,float y,float w,float h,UnityEngine.Events.UnityAction action){var im=Panel(label,parent,x,y,w,h,mint);var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(action);Text("Label",im.transform,0,0,1,1,label,19);return b;}
  GameObject Overlay(string name,float y,float height,out Transform content){var scrim=Panel(name,canvas.transform,0,0,1,1,Color.clear);scrim.sprite=null;hud.Layout.AddBackdrop(scrim.transform,new Color(0,0,0,.43f));content=Panel(name+" content",scrim.transform,.025f,y,.95f,height,new Color(.89f,.79f,.57f)).transform;return scrim.gameObject;}
  void Close(GameObject panel){panel.SetActive(false);hud.ResetMovement();}
  public void Initialize(HarvestSimulation model,EquipmentSystem gear,PortraitHud portrait,RewardSystem rewardSystem){
   sim=model;equipment=gear;hud=portrait;canvas=hud.Canvas;font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
   weaponTile=Button("GEAR",canvas.transform,0,.785f,.145f,.05f,()=>OpenWeapons());
   challengeTile=Button("TRIAL",canvas.transform,.855f,.785f,.145f,.05f,()=>{offer.SetActive(true);offer.transform.SetAsLastSibling();hud.ResetMovement();});
   exit=Button("EXIT",canvas.transform,0,.79f,.13f,.045f,()=>{sim.LeaveChallenge();hud.ResetMovement();});
   timerRoot=Panel("Trial timer",canvas.transform,.2f,.117f,.54f,.023f,ink).gameObject;
   timerFill=Panel("Timer fill",timerRoot.transform,.015f,.13f,.97f,.74f,new Color(.25f,.85f,.83f));timer=Text("Seconds",timerRoot.transform,0,-.08f,1,1.2f,"60s",21);timer.color=Color.white;
   weapons=Overlay("Equipment overlay",.30f,.67f,out var w);
   Text("Gear heading",w,.02f,.015f,.8f,.055f,"FIELD EQUIPMENT",24);Button("X",w,.9f,.015f,.08f,.05f,()=>Close(weapons));
   for(int i=0;i<3;i++){int slot=i;var b=Button("",w,.035f+i*.323f,.1f,.29f,.24f,()=>{if(equipment.SelectedWeapon>=0)EquipWeapon(equipment.SelectedWeapon,slot);});b.name="Equipment slot "+i;var drop=b.gameObject.AddComponent<WeaponSlotDrop>();drop.Slot=i;drop.Hud=this;slotIcons[i]=ToolIcon(b.transform,.22f,.06f,.56f,.29f);slots[i]=Text("Equipped",b.transform,.04f,.36f,.92f,.61f,"",18);if(i==1)Button("REMOVE",w,.358f,.348f,.29f,.045f,()=>{equipment.Unequip(1);RefreshEquipment();});}
   Button("COLLECTION",w,.681f,.348f,.29f,.045f,OpenCollection);
   Text("Equip instruction",w,.04f,.40f,.92f,.05f,"Drag a tool into an open slot",21);
   for(int i=0;i<items.Length;i++){int index=i;var b=Button("",w,.04f+(i%4)*.235f,.47f+(i/4)*.12f,.215f,.105f,()=>{if(itemIds[index]>=0){equipment.SelectedWeapon=itemIds[index];inventoryHint.text="Choose a slot above";}});b.name="Inventory "+i;var drag=b.gameObject.AddComponent<WeaponDrag>();drag.Hud=this;items[i]=b;itemIcons[i]=ToolIcon(b.transform,.2f,.05f,.6f,.45f);itemLabels[i]=Text("Tool",b.transform,.02f,.51f,.96f,.46f,"",16);}
   inventoryHint=Text("Inventory hint",w,.04f,.825f,.92f,.045f,"",18);
   delivery=Button("DELIVERY  <> "+equipment.catalog.deliveryCost,w,.19f,.9f,.62f,.075f,()=>{if(!equipment.BuyDelivery())hud.ShowToast("Complete a trial to earn crystals");RefreshEquipment();});
   weapons.SetActive(false);
   offer=Overlay("Trial offer",.32f,.34f,out var o);Text("Trial title",o,.05f,.05f,.9f,.12f,"TIMED HARVEST",27);offerDetail=Text("Objective",o,.08f,.22f,.84f,.36f,"",23);Button("START TRIAL",o,.15f,.64f,.7f,.15f,()=>{if(sim.EnterChallenge())Close(offer);});Button("BACK",o,.3f,.83f,.4f,.1f,()=>Close(offer));offer.SetActive(false);
   result=Overlay("Trial result",.31f,.36f,out var r);resultTitle=Text("Result title",r,.04f,.08f,.92f,.17f,"",28);resultDetail=Text("Result detail",r,.08f,.3f,.84f,.28f,"",24);Button("CONTINUE",r,.15f,.7f,.7f,.16f,()=>{sim.LeaveChallenge();Close(result);});result.SetActive(false);
   ghost=Rect("Dragged tool",canvas.transform,0,0,0,0);ghost.anchorMin=ghost.anchorMax=Vector2.zero;ghost.sizeDelta=new Vector2(95,75);var image=ghost.gameObject.AddComponent<Image>();image.sprite=OriginalArt.Get("gun");image.raycastTarget=false;ghost.gameObject.SetActive(false);
   BuildCollection();BuildRewards(rewardSystem);equipment.Changed+=RefreshEquipment;RefreshEquipment();
  }
  public void OpenWeapons(){if(!equipment.Unlocked||sim.InChallenge)return;weapons.SetActive(true);weapons.transform.SetAsLastSibling();hud.ResetMovement();RefreshEquipment();}
  public void EquipWeapon(int id,int slot){if(!equipment.Equip(id,slot))hud.ShowToast(slot==2?"This slot is locked":"Tool already equipped");RefreshEquipment();}
  public void BeginWeaponDrag(int id,Vector2 position){if(id<0)return;equipment.SelectedWeapon=id;ghost.gameObject.SetActive(true);ghost.SetAsLastSibling();MoveWeaponDrag(position);}
  public void MoveWeaponDrag(Vector2 position){ghost.anchoredPosition=hud.Layout.LocalPoint(position);}
  public void EndWeaponDrag(){ghost.gameObject.SetActive(false);}
  void RefreshEquipment(){
   for(int i=0;i<3;i++){int id=sim.save.equippedWeapons[i];slotIcons[i].enabled=id>=0&&i<2;slotIcons[i].color=id==1?new Color(1f,.72f,.35f):Color.white;slots[i].text=i==2?"LOCKED":id<0?"EMPTY\n+":equipment.catalog.weapons[id].label+"\n"+Mathf.RoundToInt(sim.tuning.Damage(sim.save.force)*equipment.catalog.weapons[id].damageMultiplier)+" force\n"+(sim.tuning.Interval(sim.save.tempo)*equipment.catalog.weapons[id].intervalMultiplier).ToString("0.00")+"s";}
   int n=0;for(int id=0;id<equipment.catalog.weapons.Length;id++)for(int k=0;k<equipment.Available(id)&&n<items.Length;k++){itemIds[n]=id;itemIcons[n].enabled=true;itemIcons[n].color=id==1?new Color(1f,.72f,.35f):Color.white;items[n].interactable=true;items[n].GetComponent<WeaponDrag>().WeaponId=id;itemLabels[n].text=equipment.catalog.weapons[id].label;n++;}
   while(n<items.Length){itemIds[n]=-1;itemIcons[n].enabled=false;items[n].interactable=false;items[n].GetComponent<WeaponDrag>().WeaponId=-1;itemLabels[n].text="";n++;}
   inventoryHint.text=equipment.SelectedWeapon>=0?"Choose a slot above":equipment.InventoryCount==0?"Earn crystals in timed trials":"Drag to equip, or tap a tool then a slot";
   delivery.interactable=sim.save.gems>=equipment.catalog.deliveryCost&&equipment.InventoryCount<equipment.catalog.inventoryCapacity;
  }
  public void Tick(float dt){
   bool available=!hud.CoreOverlayOpen&&!IsOpen;weaponTile.gameObject.SetActive(equipment.Unlocked&&!sim.InChallenge&&available);challengeTile.gameObject.SetActive(sim.ChallengeUnlocked&&!sim.InChallenge&&available&&sim.phase==StagePhase.Harvest);
   dailyTile.gameObject.SetActive(available&&!sim.InChallenge);missionTile.gameObject.SetActive(available&&!sim.InChallenge&&rewards.MissionsUnlocked);
   exit.gameObject.SetActive(sim.InChallenge&&sim.phase==StagePhase.Harvest&&!hud.CoreOverlayOpen);timerRoot.SetActive(sim.InChallenge);
   if(sim.phase==StagePhase.ChallengeResult&&!result.activeSelf){result.SetActive(true);result.transform.SetAsLastSibling();resultTitle.text=sim.ChallengeWon?"HARVEST COMPLETE":"TIME'S UP";resultDetail.text=sim.ChallengeWon?"<> +"+sim.tuning.challengeGemReward+" crystals":"Harvested "+sim.cleared+" / "+sim.total+"\nReturn to the grove";hud.ResetMovement();}
   hud.FeatureOverlay=IsOpen;
   refresh-=dt;if(refresh>0)return;refresh=.1f;timer.text=Mathf.CeilToInt(sim.ChallengeRemaining)+"s";timerFill.rectTransform.anchorMax=new Vector2(.015f+.97f*sim.ChallengeRemaining/sim.tuning.challengeSeconds,.87f);
   if(offer.activeSelf)offerDetail.text="Clear every plant in "+sim.tuning.challengeSeconds+" seconds\n\nReward  <> "+sim.tuning.challengeGemReward;
   if(weapons.activeSelf)RefreshEquipment();
   if(collection.activeSelf)RefreshCollection();
   if(dailyPanel.activeSelf||missionPanel.activeSelf)RefreshRewards();
  }
 }
}
