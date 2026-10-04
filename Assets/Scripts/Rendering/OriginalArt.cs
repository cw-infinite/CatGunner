using System.Collections.Generic;
using UnityEngine;
namespace VerdantTrail {
 // Generated original sprite sheets with procedural fallback/effects. Reference frames are never loaded by the game.
 public static class OriginalArt {
  static readonly Dictionary<string,Sprite> cache=new Dictionary<string,Sprite>();
  static readonly Color32 Ink=new Color32(53,59,46,255);
  static Color32 C(string hex){ColorUtility.TryParseHtmlString(hex,out Color c);return c;}
  sealed class Paint {
   public readonly Color32[] px=new Color32[256*256];
   public void Ellipse(float x,float y,float rx,float ry,Color32 c){for(int iy=Mathf.Max(0,(int)(y-ry));iy<Mathf.Min(256,y+ry);iy++)for(int ix=Mathf.Max(0,(int)(x-rx));ix<Mathf.Min(256,x+rx);ix++){float a=(ix-x)/rx,b=(iy-y)/ry;if(a*a+b*b<1)px[iy*256+ix]=c;}}
   public void Oval(float x,float y,float rx,float ry,string fill,float border=4){Ellipse(x,y,rx,ry,Ink);Ellipse(x,y,rx-border,ry-border,C(fill));}
   public void Box(int x,int y,int w,int h,Color32 c){for(int iy=Mathf.Max(0,y);iy<Mathf.Min(256,y+h);iy++)for(int ix=Mathf.Max(0,x);ix<Mathf.Min(256,x+w);ix++)px[iy*256+ix]=c;}
   public void Poly(Color32 c,params Vector2[] pts){for(int y=0;y<256;y++)for(int x=0;x<256;x++){bool inside=false;for(int i=0,j=pts.Length-1;i<pts.Length;j=i++){Vector2 a=pts[i],b=pts[j];if((a.y>y)!=(b.y>y)&&x<(b.x-a.x)*(y-a.y)/(b.y-a.y)+a.x)inside=!inside;}if(inside)px[y*256+x]=c;}}
  }
  public static Sprite Get(string key) {
   if(cache.TryGetValue(key,out var result))return result;
   result=SpriteArt.Load(key);if(result!=null){cache.Add(key,result);return result;}
   Paint p=new Paint();Vector2 pivot=new Vector2(.5f,0);
   if(key.StartsWith("tree")) {
    int kind=int.Parse(key.Substring(4));
    p.Oval(128,39,14,37,"#aa7951");p.Box(119,8,6,45,C("#cc9d69"));
    if(kind==0) {
     p.Poly(Ink,new Vector2(22,67),new Vector2(60,128),new Vector2(43,128),new Vector2(95,198),new Vector2(128,246),new Vector2(162,198),new Vector2(212,128),new Vector2(194,128),new Vector2(233,67));
     p.Poly(C("#68985f"),new Vector2(32,73),new Vector2(73,137),new Vector2(55,137),new Vector2(128,236),new Vector2(201,137),new Vector2(184,137),new Vector2(223,73));
     p.Poly(C("#88b777"),new Vector2(42,82),new Vector2(121,209),new Vector2(111,98));
    } else if(kind==5) {
     p.Oval(128,64,26,60,"#b78434");p.Box(117,14,8,92,C("#f1cc69"));
     p.Oval(69,146,59,48,"#c99739");p.Oval(187,147,58,48,"#d9aa45");
     p.Oval(95,195,63,46,"#ebc65c");p.Oval(158,202,62,47,"#f3d679");p.Oval(128,149,67,55,"#dfb343");
     p.Ellipse(89,205,28,13,C("#fff0a2"));p.Ellipse(165,174,23,13,C("#f8df83"));
    } else if(kind==4) {
     p.Oval(128,116,26,107,"#628e67");p.Oval(74,132,20,59,"#628e67");p.Oval(179,163,20,57,"#628e67");
     p.Oval(97,90,39,20,"#628e67");p.Oval(157,119,42,19,"#628e67");p.Box(119,29,5,175,C("#91b775"));p.Box(68,110,4,65,C("#91b775"));
    } else if(kind==3) {
     p.Oval(128,88,14,83,"#aa7951");p.Box(120,15,5,135,C("#cc9d69"));
     for(int band=0;band<6;band++)p.Box(116,27+band*19,21,4,C("#896644"));
     Vector2 crown=new Vector2(128,168);
     for(int i=0;i<7;i++){
      float a=(i*47+15)*Mathf.PI/180;Vector2 dir=new Vector2(Mathf.Cos(a),Mathf.Sin(a)*.65f),perp=new Vector2(-dir.y,dir.x).normalized;
      Vector2 tip=crown+dir*119+new Vector2(0,-21),shoulder=crown+dir*59;
      p.Poly(Ink,crown-perp*8,shoulder-perp*20+Vector2.down*8,tip,shoulder+perp*24,crown+perp*8);
      p.Poly(C(i%2==0?"#76a160":"#58845c"),crown,shoulder-perp*14+Vector2.down*5,tip-dir*9,shoulder+perp*18);
      p.Poly(C("#9fbb72"),crown,shoulder+perp*4,tip-dir*15,shoulder);
     }
     p.Oval(119,160,12,13,"#ae844a",3);p.Oval(139,161,12,13,"#bd9554",3);
    } else {
     string color=kind==2?"#cf9eb5":"#91ad67";
     p.Oval(84,132,57,55,color);p.Oval(174,135,57,54,color);p.Oval(128,184,67,56,color);p.Oval(126,126,71,57,color);
     p.Ellipse(99,180,27,15,kind==2?C("#e7b6c8"):C("#b0c779"));
     p.Ellipse(167,142,18,10,kind==2?C("#b985a3"):C("#779458"));
    }
   }else if(key=="ranger") {
    p.Oval(77,58,39,28,"#bb794e");p.Oval(108,25,25,16,"#685647");p.Oval(156,25,25,16,"#685647");
    p.Oval(132,75,51,52,"#dba268");p.Oval(106,187,19,56,"#dfbb86");p.Oval(154,189,18,51,"#dfbb86");
    p.Ellipse(106,195,8,36,C("#ac7c68"));p.Ellipse(154,196,7,32,C("#ac7c68"));
    p.Oval(130,135,64,53,"#e2bf8c");p.Oval(130,140,51,22,"#53776f",3);
    p.Ellipse(112,143,5,8,C("#fff3bf"));p.Ellipse(150,143,5,8,C("#fff3bf"));
    p.Oval(130,100,48,11,"#5eb0a0",3);p.Poly(C("#5eb0a0"),new Vector2(86,102),new Vector2(68,65),new Vector2(100,83));
    p.Box(117,52,29,18,Ink);p.Box(121,56,21,10,C("#f5d48c"));
   }else if(key=="gun") {
    pivot=new Vector2(.15f,.5f);p.Box(30,108,156,42,Ink);p.Box(35,114,145,30,C("#789293"));p.Box(174,120,65,17,Ink);p.Box(85,76,22,37,Ink);p.Box(112,142,42,11,C("#e8b663"));
   }else if(key=="bullet") {
    pivot=new Vector2(.5f,.5f);p.Ellipse(128,128,125,60,new Color32(255,172,37,40));p.Ellipse(128,128,116,40,C("#d76b13"));p.Ellipse(132,128,106,31,C("#ffb12e"));p.Ellipse(144,128,90,18,C("#fffce4"));
   }else if(key=="muzzle") {
    pivot=new Vector2(.5f,.5f);p.Poly(C("#ffad28"),new Vector2(128,250),new Vector2(153,166),new Vector2(229,193),new Vector2(179,130),new Vector2(248,75),new Vector2(160,91),new Vector2(122,8),new Vector2(102,91),new Vector2(16,62),new Vector2(80,127),new Vector2(22,204),new Vector2(101,168));p.Ellipse(128,128,43,43,C("#fffadc"));
   }else if(key=="note") {
    pivot=new Vector2(.5f,.5f);p.Poly(Ink,new Vector2(43,67),new Vector2(201,83),new Vector2(218,182),new Vector2(58,170));
    p.Poly(C("#8ae46c"),new Vector2(54,78),new Vector2(192,93),new Vector2(205,171),new Vector2(68,160));p.Oval(131,124,25,31,"#c8f599",6);
   }else if(key=="ring") {
    pivot=new Vector2(.5f,.5f);p.Ellipse(128,128,110,110,Ink);p.Ellipse(128,128,99,99,C("#fffbe1"));p.Ellipse(128,128,87,87,new Color32(0,0,0,0));
    p.Box(119,1,18,49,C("#fffbe1"));p.Box(119,206,18,49,C("#fffbe1"));p.Box(1,119,49,18,C("#fffbe1"));p.Box(206,119,49,18,C("#fffbe1"));
   }else if(key=="gem") {
    pivot=new Vector2(.5f,.5f);p.Poly(Ink,new Vector2(63,216),new Vector2(185,216),new Vector2(242,140),new Vector2(128,22),new Vector2(14,140));
    p.Poly(C("#65c6dc"),new Vector2(69,204),new Vector2(178,204),new Vector2(227,140),new Vector2(128,39),new Vector2(30,140));
    p.Poly(C("#b5f3f0"),new Vector2(69,204),new Vector2(110,145),new Vector2(30,140));
    p.Poly(C("#39899e"),new Vector2(110,145),new Vector2(227,140),new Vector2(128,39));
    p.Poly(C("#e5ffef"),new Vector2(69,204),new Vector2(178,204),new Vector2(110,145));
   }else if(key=="gear") {
    pivot=new Vector2(.5f,.5f);
    for(int i=0;i<8;i++){float a=i*Mathf.PI/4;Vector2 d=new Vector2(Mathf.Cos(a),Mathf.Sin(a)),n=new Vector2(-d.y,d.x),c=new Vector2(128,128);p.Poly(Ink,c+d*72+n*23,c+d*110+n*23,c+d*110-n*23,c+d*72-n*23);}
    p.Ellipse(128,128,87,87,Ink);p.Ellipse(128,128,40,40,new Color32(0,0,0,0));
   }else if(key=="grass") {
    pivot=new Vector2(.5f,0);p.Ellipse(74,66,20,51,Color.white);p.Ellipse(124,85,22,72,Color.white);p.Ellipse(174,66,20,51,Color.white);
   }else if(key=="disc") {
    pivot=new Vector2(.5f,.5f);p.Ellipse(128,128,122,122,Color.white);
   }else if(key=="panel") {
    pivot=new Vector2(.5f,.5f);p.Box(32,0,192,256,Color.white);p.Box(0,32,256,192,Color.white);
    p.Ellipse(32,32,32,32,Color.white);p.Ellipse(224,32,32,32,Color.white);p.Ellipse(32,224,32,32,Color.white);p.Ellipse(224,224,32,32,Color.white);
   }else if(key=="shadow") {
    pivot=new Vector2(.5f,.5f);p.Ellipse(128,128,115,62,new Color32(47,58,35,50));
   }else {pivot=new Vector2(.5f,.5f);p.Box(0,0,256,256,Color.white);}
   var tex=new Texture2D(256,256,TextureFormat.RGBA32,false){name="Original_"+key,filterMode=FilterMode.Bilinear};tex.SetPixels32(p.px);tex.Apply(false,true);
   result=UnityEngine.Sprite.Create(tex,new Rect(0,0,256,256),pivot,128,0,SpriteMeshType.FullRect,key=="panel"?new Vector4(32,32,32,32):Vector4.zero);cache.Add(key,result);return result;
  }
  public static SpriteRenderer Sprite(string name,string art,Transform parent,int order) {
   var go=new GameObject(name);go.transform.SetParent(parent,false);var sr=go.AddComponent<SpriteRenderer>();sr.sprite=Get(art);sr.sortingOrder=order;return sr;
  }
 }
}

