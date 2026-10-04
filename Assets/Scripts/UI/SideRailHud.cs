using UnityEngine;
using UnityEngine.UI;
namespace VerdantTrail {
 public sealed partial class FeatureHud {
  readonly Button[] lockedTiles=new Button[5];
  void Rail(Button button,string caption,string icon,int style,float x,float y,bool locked=false){
   var rect=(RectTransform)button.transform;rect.anchorMin=new Vector2(x,1-y-.060f);rect.anchorMax=new Vector2(x+.145f,1-y);rect.offsetMin=rect.offsetMax=Vector2.zero;
   UiFinish.Skin(button.GetComponent<Image>(),"rail"+style);
   var label=button.GetComponentInChildren<Text>();label.text=caption;label.fontSize=18;label.rectTransform.anchorMin=new Vector2(.015f,.015f);label.rectTransform.anchorMax=new Vector2(.985f,.34f);
   var art=Panel("Navigation icon",button.transform,.17f,-.03f,.66f,.74f,Color.white);art.sprite=OriginalArt.Get(icon);art.type=Image.Type.Simple;art.preserveAspect=true;art.raycastTarget=false;
   if(locked){button.interactable=false;art.color=new Color(.78f,.78f,.78f);var padlock=Panel("Locked",button.transform,-.015f,-.08f,.31f,.37f,Color.white);padlock.sprite=OriginalArt.Get("hud10");padlock.type=Image.Type.Simple;padlock.preserveAspect=true;padlock.raycastTarget=false;}
  }
  void BuildSideRails(){
   Rail(weaponTile,"Gun","hud0",0,0,.775f);Rail(skinsTile,"Skin","hud1",1,0,.710f);
   Rail(challengeTile,"Play","hud4",0,.855f,.775f);
   Rail(dailyTile,"Daily","hud11",2,0,.515f);Rail(missionTile,"Tasks","hud11",1,.855f,.515f);
   string[] names={"FISH","PET","MINE","BOSS","HUNT"},labels={"Fish","Pet","Mine","Boss","Hunt"};int[] icons={2,3,5,6,7};float[] y={.645f,.580f,.710f,.645f,.580f};
   for(int i=0;i<5;i++){lockedTiles[i]=Button(names[i],canvas.transform,0,0,.1f,.1f,()=>{});Rail(lockedTiles[i],labels[i],"hud"+icons[i],3,i<2?0:.855f,y[i],true);}
  }
  public void ReturnToGrove(){
   foreach(var panel in new[]{weapons,offer,result,collection,dailyPanel,missionPanel,skinsPanel})panel.SetActive(false);
   CancelWeaponDrag();hud.FeatureOverlay=false;hud.ResetMovement();
  }
 }
}
