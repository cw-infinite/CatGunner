using UnityEngine;
namespace VerdantTrail {
 public sealed partial class WorldView {
  readonly SpriteRenderer[] impactRings=new SpriteRenderer[24],impactBursts=new SpriteRenderer[24];
  readonly float[] impactLife=new float[24];readonly int[] impactStyle=new int[24];int impactCursor;
  static Color ShotColor(int style,float alpha){Color c=style==1?new Color(.40f,.88f,1):style==2?new Color(.67f,1,.33f):style==3?new Color(1,.72f,.22f):new Color(1,.90f,.68f);c.a=alpha;return c;}
  void InitializeWeaponEffects(Transform root){
   for(int i=0;i<impactRings.Length;i++){impactRings[i]=OriginalArt.Sprite("Weapon impact ring "+i,"shockwave",root,2040);impactBursts[i]=OriginalArt.Sprite("Weapon impact burst "+i,"muzzle",root,2041);impactRings[i].enabled=impactBursts[i].enabled=false;}
   sim.ProjectileImpact+=ShowWeaponImpact;
  }
  void ShowWeaponImpact(Vector2 position,int weapon){
   int i=impactCursor++%impactLife.Length;impactLife[i]=weapon>=2?.28f:.12f;impactStyle[i]=Mathf.Clamp(weapon,0,3);
   impactRings[i].transform.position=impactBursts[i].transform.position=Pos(position);
  }
  void RenderWeaponEffects(float dt){
   for(int i=0;i<impactLife.Length;i++){
    impactLife[i]=Mathf.Max(0,impactLife[i]-Mathf.Max(0,dt));bool live=impactLife[i]>0;int style=impactStyle[i];
    impactRings[i].enabled=live&&style>=2;impactBursts[i].enabled=live;if(!live)continue;
    float progress=1-impactLife[i]/(style>=2?.28f:.12f),radius=style==3?.68f:style==2?.48f:.18f;
    impactRings[i].transform.localScale=Vector3.one*radius*(.45f+progress*.75f);impactRings[i].color=ShotColor(style,(1-progress)*.70f);
    impactBursts[i].transform.localScale=Vector3.one*radius*(1-progress*.7f);impactBursts[i].transform.rotation=Quaternion.Euler(0,0,i*39+progress*35);impactBursts[i].color=ShotColor(style,(1-progress)*.85f);
   }
  }
  void ResetWeaponEffects(){for(int i=0;i<impactLife.Length;i++){impactLife[i]=0;impactRings[i].enabled=impactBursts[i].enabled=false;}}
 }
}
