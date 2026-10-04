using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace VerdantTrail {
 public sealed class DragSurface : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler {
  public Vector2 Value;public Vector2 Origin;public bool Held;public float Radius=60;
  int pointer=int.MinValue;
  public void OnPointerDown(PointerEventData e){if(Held)return;pointer=e.pointerId;Origin=e.position;Held=true;Value=Vector2.zero;}
  public void OnDrag(PointerEventData e){if(pointer!=e.pointerId)return;Value=Vector2.ClampMagnitude((e.position-Origin)/Mathf.Max(1,Radius),1);}
  public void OnPointerUp(PointerEventData e){if(pointer!=e.pointerId)return;Held=false;Value=Vector2.zero;pointer=int.MinValue;}
  public void ResetInput(){Held=false;Value=Vector2.zero;pointer=int.MinValue;}
  void OnDisable(){ResetInput();}
 }
 public sealed class PortraitHud : MonoBehaviour {
  HarvestSimulation sim;Canvas canvas;Font font;
  Text cash,gems,stage,percent,notice,toast,debugStats;Text[] values=new Text[3],prices=new Text[3],levels=new Text[3];
  Text saveStatus;Button saveButton;
  Image sectorLine,sectorCat;Image progress;readonly Image[] sectorNodes=new Image[5];Button[] upgrades=new Button[3];Image[] cards=new Image[3];
  GameObject transfer,banner,settings,debug;RectTransform joystick,knob;DragSurface input;
  float refresh,toastTime;int shownStage=-1;string noticeText="";
  public Vector2 Movement=>input.Value;
  public Canvas Canvas=>canvas;
  public PortraitLayout Layout {get;private set;}
  public RectTransform ContentRoot=>Layout.Root;
  public void RefreshLayout(){if(Layout.Refresh())ResetMovement();input.Radius=Layout.PixelRect.width*.11f;}
  public void ResetMovement()=>input.ResetInput();
  public bool CoreOverlayOpen=>settings.activeSelf||debug.activeSelf;
  public bool FeatureOverlay;
  public bool IsOverlayOpen=>CoreOverlayOpen||FeatureOverlay;
  readonly int[] upgradeLevels=new int[3];
  readonly Color ink=new Color(.30f,.18f,.10f),paper=new Color(.98f,.92f,.75f),mint=new Color(.56f,.94f,.76f),dark=new Color(.24f,.34f,.27f);
  RectTransform Rect(string name,Transform parent,float x,float y,float w,float h) {
   if(parent==canvas.transform)parent=ContentRoot;
   var go=new GameObject(name,typeof(RectTransform));var rt=go.GetComponent<RectTransform>();rt.SetParent(parent,false);
   rt.anchorMin=new Vector2(x,1-y-h);rt.anchorMax=new Vector2(x+w,1-y);rt.offsetMin=rt.offsetMax=Vector2.zero;return rt;
  }
  Image Panel(string name,Transform parent,float x,float y,float w,float h,Color color) {var rt=Rect(name,parent,x,y,w,h);var im=rt.gameObject.AddComponent<Image>();im.color=color;if(name.EndsWith("pill")||name=="Upgrade border"||name=="Settings"||name=="Developer tools"){im.sprite=OriginalArt.Get("panel");im.type=Image.Type.Sliced;if(name=="Cash pill"||name=="Gem pill")UiFinish.Skin(im,"pill-art");if(name=="Settings"||name=="Developer tools")UiFinish.Skin(im,"menu-art");}return im;}
  Text Label(string name,Transform parent,float x,float y,float w,float h,string text,int size,Color color,TextAnchor anchor=TextAnchor.MiddleCenter) {
   var rt=Rect(name,parent,x,y,w,h);var t=rt.gameObject.AddComponent<Text>();t.font=font;t.text=text;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.color=color;t.alignment=anchor;t.raycastTarget=false;UiFinish.WhiteText(t);return t;
  }
  Button Button(string title,Transform parent,float x,float y,float w,float h,Color color,UnityEngine.Events.UnityAction action,int fontSize=22) {
   var panel=Panel(title,parent,x,y,w,h,color);panel.sprite=OriginalArt.Get("panel");panel.type=Image.Type.Sliced;UiFinish.Skin(panel,"button-art");var b=panel.gameObject.AddComponent<Button>();b.targetGraphic=panel;b.onClick.AddListener(action);
   UiFinish.Button(b);
   Label("Caption",panel.transform,0,0,1,1,title,fontSize,ink);return b;
  }
  public void Initialize(HarvestSimulation model) {
   sim=model;font=UiFinish.Font;
   var go=new GameObject("Portrait HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=10000;
   var scaler=go.GetComponent<CanvasScaler>();Layout=new PortraitLayout(canvas,scaler);
   if(FindFirstObjectByType<EventSystem>()==null)new GameObject("Touch event system",typeof(EventSystem),typeof(StandaloneInputModule));
   var touch=Panel("Movement surface",canvas.transform,0,.17f,1,.68f,new Color(0,0,0,0));input=touch.gameObject.AddComponent<DragSurface>();
   Panel("Top breathing room",canvas.transform,0,0,1,.073f,new Color(.30f,.19f,.10f));
   var track=Panel("Clearing progress track",canvas.transform,0,.074f,1,.014f,ink);
   progress=Panel("Fill",track.transform,0,0,1,1,new Color(.94f,.72f,.37f));percent=Label("Percent",track.transform,0,-.25f,1,1.5f,"0%",17,Color.white);
   IconButton("*","hud8",.015f,()=>Toggle(settings));
   stage=Label("Stage",canvas.transform,.28f,.133f,.44f,.028f,"",22,Color.white);UiFinish.WhiteText(stage);UiFinish.WhiteText(percent);
   sectorLine=Panel("Sector line",canvas.transform,.30f,.113f,.40f,.005f,paper);
   for(int i=0;i<5;i++){var node=Panel("Sector "+i,canvas.transform,.282f+i*.10f,.106f,.036f,.017f,paper);node.sprite=OriginalArt.Get("disc");sectorNodes[i]=node;}
   themeLabel=Label("Grove theme",canvas.transform,.23f,.161f,.54f,.022f,"",14,Color.white);
   sectorCat=Panel("Current grove cat",canvas.transform,.27f,.092f,.060f,.040f,Color.white);sectorCat.sprite=OriginalArt.Get("hud1");sectorCat.preserveAspect=true;sectorCat.raycastTarget=false;
   var cashPill=Panel("Cash pill",canvas.transform,.79f,.1f,.196f,.029f,paper);var gemPill=Panel("Gem pill",canvas.transform,.79f,.136f,.196f,.027f,paper);
   var noteIcon=Panel("Notes icon",cashPill.transform,.015f,-.1f,.32f,1.2f,Color.white);noteIcon.sprite=OriginalArt.Get("note");noteIcon.preserveAspect=true;noteIcon.raycastTarget=false;
   var gemIcon=Panel("Crystals icon",gemPill.transform,.015f,-.1f,.32f,1.2f,Color.white);gemIcon.sprite=OriginalArt.Get("gem");gemIcon.preserveAspect=true;gemIcon.raycastTarget=false;
   cash=Label("Cash",cashPill.transform,.27f,0,.70f,1,"",22,ink);gems=Label("Gems",gemPill.transform,.27f,0,.70f,1,"",20,new Color(.22f,.5f,.65f));
   string[] names={"FORCE","TEMPO","YIELD"};string[] symbols={"power-art","speed-art","note"};
   for(int i=0;i<3;i++) {
    int k=i;float x=.065f+i*.298f;
    var b=Button("",canvas.transform,x,.867f,.272f,.075f,mint,()=>Purchase(k));upgrades[i]=b;cards[i]=b.GetComponent<Image>();
    Label("Category",canvas.transform,x,.845f,.2f,.02f,names[i],18,Color.white);
    levels[i]=Label("Level",canvas.transform,x+.18f,.851f,.092f,.017f,"",14,Color.white);
    var upgradeIcon=Panel("Upgrade icon",b.transform,-.055f,-.13f,.30f,.40f,Color.white);upgradeIcon.sprite=OriginalArt.Get(symbols[i]);upgradeIcon.preserveAspect=true;upgradeIcon.raycastTarget=false;
    values[i]=Label("Stat",b.transform,.07f,.10f,.86f,.43f,"",29,ink);UiFinish.WhiteText(values[i]);
    var price=Panel("Cost pill",b.transform,.06f,.58f,.88f,.3f,ink);var costIcon=Panel("Upgrade banknote",price.transform,-.025f,-.10f,.28f,1.20f,Color.white);costIcon.sprite=OriginalArt.Get("note");costIcon.preserveAspect=true;costIcon.raycastTarget=false;prices[i]=Label("Price",price.transform,.23f,0,.74f,1,"",22,Color.white);
   }
   notice=Label("Hint",canvas.transform,.15f,.79f,.7f,.035f,"AUTO FIRE  /  DRAG TO MOVE",15,new Color(.27f,.36f,.25f));
   joystick=Rect("Floating joystick",canvas.transform,0,0,0,0);joystick.anchorMin=joystick.anchorMax=Vector2.zero;joystick.sizeDelta=new Vector2(112,112);var ji=joystick.gameObject.AddComponent<Image>();ji.sprite=OriginalArt.Get("ring");ji.color=new Color(1,1,1,.4f);ji.raycastTarget=false;
   knob=Rect("Thumb",joystick,.35f,.35f,.3f,.3f);var ki=knob.gameObject.AddComponent<Image>();ki.sprite=OriginalArt.Get("shadow");ki.color=Color.white;ki.raycastTarget=false;
   banner=Panel("Stage clear",canvas.transform,0,.3f,1,.046f,paper).gameObject;Label("Clear title",banner.transform,0,0,1,1,"GROVE CLEARED",28,ink);banner.SetActive(false);
   transfer=Panel("Travel",canvas.transform,0,.089f,1,.75f,new Color(.86f,.85f,.6f,.96f)).gameObject;
   Label("Travel text",transfer.transform,0,.47f,1,.08f,"ON TO THE NEXT GROVE",23,ink);var mini=Panel("Traveler",transfer.transform,.455f,.57f,.09f,.075f,Color.white);mini.sprite=OriginalArt.Get("ranger");mini.preserveAspect=true;transfer.SetActive(false);
   settings=Modal("Settings");Label("Heading",settings.transform,0,.05f,1,.11f,"VERDANT TRAIL",26,ink);
   saveStatus=Label("Save status",settings.transform,.05f,.18f,.9f,.14f,SaveStore.StatusMessage,16,ink);
   Button("Sound on / off",settings.transform,.1f,.35f,.8f,.13f,mint,()=>{sim.save.sound=!sim.save.sound;ShowToast(sim.save.sound?"Sound on":"Sound off");});
   saveButton=Button("Save progress",settings.transform,.1f,.53f,.8f,.13f,paper,()=>{SaveStore.Save(sim.save);ShowToast(SaveStore.LastError==null?"Progress saved":"Save failed; see Settings");});
   Button("Resume",settings.transform,.1f,.76f,.8f,.13f,mint,()=>Toggle(settings));settings.SetActive(false);
   toast=Label("Toast",canvas.transform,.11f,.60f,.78f,.055f,"",18,ink);
   BuildDebug();
  }
  GameObject Modal(string title) {
   var overlay=Panel(title+" scrim",canvas.transform,0,0,1,1,Color.clear);Layout.AddBackdrop(overlay.transform,new Color(0,0,0,.4f));var panel=Panel(title,overlay.transform,.1f,.3f,.8f,.36f,new Color(.88f,.77f,.54f));
   // The overlay is toggled as a unit; content lives in its centered panel.
   return panel.gameObject;
  }
  void Toggle(GameObject panel){bool open=!panel.activeSelf;panel.SetActive(open);panel.transform.parent.gameObject.SetActive(open);input.ResetInput();}
  void Purchase(int i){if(sim.Buy(i)){refresh=0;}else ShowToast("More notes needed");}
  public void ShowToast(string message){toast.text=message;toastTime=2;}
  void IconButton(string name,string art,float x,UnityEngine.Events.UnityAction action){
   var b=Button(name,canvas.transform,x,.097f,.075f,.035f,Color.white,action);b.GetComponentInChildren<Text>().text="";b.GetComponent<Image>().color=Color.clear;
   var icon=Panel(name+" icon",b.transform,0,0,1,1,Color.white);icon.sprite=OriginalArt.Get(art);icon.preserveAspect=true;icon.raycastTarget=false;
  }
  Text themeLabel;
  public void ReturnToGrove(){Time.timeScale=1;sim.LeaveChallenge();FindFirstObjectByType<FeatureHud>().ReturnToGrove();if(debug.activeSelf)Toggle(debug);ResetMovement();ShowToast("Back in the grove");}
  void BuildDebug() {
   debug=Modal("Developer tools");debug.transform.parent.gameObject.SetActive(false);
   var rt=(RectTransform)debug.transform;rt.anchorMin=new Vector2(.06f,.19f);rt.anchorMax=new Vector2(.94f,.8f);
   Label("Dev heading",debug.transform,0,.01f,1,.07f,"DEVELOPER TOOLS",24,ink);
   debugStats=Label("Stats",debug.transform,.04f,.08f,.92f,.12f,"",17,ink);
   Button("+1000 notes",debug.transform,.04f,.22f,.44f,.075f,mint,()=>sim.save.cash+=1000,18);
   Button("+100 crystals",debug.transform,.52f,.22f,.44f,.075f,mint,()=>sim.save.gems=Mathf.Min(1000000,sim.save.gems+100),18);
   Button("+1000 crystals",debug.transform,.04f,.315f,.44f,.075f,mint,()=>sim.save.gems=Mathf.Min(1000000,sim.save.gems+1000),18);
   Button("Clear targets",debug.transform,.52f,.315f,.44f,.075f,paper,()=>{for(int i=0;i<sim.total;i++)sim.Damage(i,sim.targets[i].hp);},18);
   Button("Previous grove",debug.transform,.04f,.41f,.44f,.075f,paper,()=>{sim.LeaveChallenge();sim.Jump(Mathf.Max(1,sim.save.stage-1));},18);
   Button("Next stage",debug.transform,.52f,.41f,.44f,.075f,paper,()=>{sim.LeaveChallenge();sim.Jump(sim.save.stage+1);},18);
   Button("Return to grove",debug.transform,.04f,.505f,.92f,.075f,mint,ReturnToGrove,20);
   Button("Auto approach",debug.transform,.04f,.60f,.44f,.075f,mint,()=>sim.automate=!sim.automate,18);
   Button("Next theme",debug.transform,.52f,.60f,.44f,.075f,paper,()=>sim.Jump(Mathf.Min(100,((sim.save.stage-1)/5+1)*5+1)),18);
   int[] rates={1,2,5,10};for(int i=0;i<4;i++){int speed=rates[i];Button(speed+"x",debug.transform,.04f+i*.235f,.70f,.21f,.075f,paper,()=>Time.timeScale=speed,20);}
   Button("Close",debug.transform,.2f,.86f,.6f,.085f,mint,()=>Toggle(debug),20);
   debug.SetActive(false);
   if(Debug.isDebugBuild||Application.isEditor)IconButton("DEV","hud9",.10f,()=>Toggle(debug));
  }
  public void Tick(float dt) {
   // Hide scrims initially; panel active state controls overlay.
   settings.transform.parent.gameObject.SetActive(settings.activeSelf);debug.transform.parent.gameObject.SetActive(debug.activeSelf);
   joystick.gameObject.SetActive(input.Held&&!IsOverlayOpen);
   if(input.Held){joystick.anchoredPosition=Layout.LocalPoint(input.Origin);knob.anchoredPosition=input.Value*37;}
   banner.SetActive(sim.phase==StagePhase.Clear);transfer.SetActive(sim.phase==StagePhase.Transfer);
   if(toastTime>0){toastTime-=dt;if(toastTime<=0)toast.text="";}
   refresh-=dt;if(refresh>0)return;refresh=.1f;
   if(settings.activeSelf){saveStatus.text=SaveStore.StatusMessage;saveButton.interactable=SaveStore.CanWrite;}
   cash.text=HarvestSimulation.Format(sim.save.cash);gems.text=HarvestSimulation.Format(sim.save.gems);
   sectorCat.enabled=!sim.InChallenge;sectorLine.enabled=!sim.InChallenge;stage.enabled=!sim.InChallenge;foreach(var node in sectorNodes)node.enabled=!sim.InChallenge;
   sectorCat.rectTransform.anchorMin=new Vector2(.27f+(sim.save.stage-1)%5*.10f,.868f);sectorCat.rectTransform.anchorMax=new Vector2(.33f+(sim.save.stage-1)%5*.10f,.908f);
   stage.text="GROVE "+((sim.save.stage-1)/5+1)+" - "+((sim.save.stage-1)%5+1);
   themeLabel.text=sim.InChallenge?"TRIAL ARENA":GroveThemes.Names[GroveThemes.Index(sim.save.stage)];
   percent.text=Mathf.RoundToInt(sim.Progress*100)+"%";progress.rectTransform.anchorMax=new Vector2(sim.Progress,1);
   var lv=upgradeLevels;lv[0]=sim.save.force;lv[1]=sim.save.tempo;lv[2]=sim.save.yield;
   for(int i=0;i<3;i++) {
    levels[i].text="L."+lv[i];double cost=sim.tuning.Cost(lv[i]);prices[i].text=lv[i]>=100?"MAX":HarvestSimulation.Format(cost);
    bool can=sim.save.cash>=cost&&lv[i]<100;cards[i].color=can?Color.white:new Color(.73f,.76f,.62f);prices[i].color=can?Color.white:new Color(1,.5f,.48f);values[i].color=Color.white;
    values[i].text=i==0?Mathf.RoundToInt(sim.EffectiveDamage).ToString():i==1?Mathf.RoundToInt(100*(1+sim.tuning.speedStep*lv[i])*(1+(sim.skins?.TempoBonus??0)/100)).ToString():Mathf.RoundToInt(100*Mathf.Pow(sim.tuning.damageGrowth,lv[i])*(1+(sim.skins?.YieldBonus??0)/100)).ToString();
   }
   if(shownStage!=sim.save.stage){shownStage=sim.save.stage;for(int i=0;i<sectorNodes.Length;i++)sectorNodes[i].color=i==(shownStage-1)%5?new Color(1,.7f,.25f):i<(shownStage-1)%5?mint:paper;noticeText=shownStage==1?"AUTO FIRE  /  DRAG TO MOVE":"";notice.text=noticeText;}
   if(debug.activeSelf)debugStats.text="Stage "+sim.save.stage+"  |  targets "+(sim.total-sim.cleared)+"\nShots "+sim.shotsFired+"  |  FPS "+Mathf.RoundToInt(1/Mathf.Max(.001f,Time.unscaledDeltaTime));
  }
 }
}
