using UnityEngine;
namespace VerdantTrail {
 // Regions describe independently generated sheets, never pixels from the reference video.
 public static class SpriteArt {
  public static Sprite Load(string key){
   bool ui=false;Rect region;Vector2 pivot=new Vector2(.5f,0);bool panel=false;
   switch(key){
    case "tree0":region=new Rect(0,0,448,480);break;
    case "tree1":region=new Rect(448,0,448,480);break;
    case "tree2":region=new Rect(896,0,416,480);break;
    case "tree3":region=new Rect(1312,0,480,480);break;
    case "tree4":region=new Rect(0,480,448,416);break;
    case "tree5":region=new Rect(448,480,448,416);break;
    case "ranger":region=new Rect(896,480,448,416);break;
    case "gun":region=new Rect(1344,480,448,416);pivot=new Vector2(.18f,.45f);break;
    case "button-art":ui=true;panel=true;region=new Rect(0,0,535,480);break;
    case "menu-art":ui=true;panel=true;region=new Rect(535,0,465,480);break;
    case "card-art":ui=true;panel=true;region=new Rect(1000,0,335,480);break;
    case "pill-art":ui=true;panel=true;region=new Rect(1335,0,457,480);break;
    case "note":ui=true;region=new Rect(0,480,535,416);break;
    case "gem":ui=true;region=new Rect(535,480,415,416);break;
    case "power-art":ui=true;region=new Rect(950,480,385,416);break;
    case "speed-art":ui=true;region=new Rect(1335,480,457,416);break;
    default:return null;
   }
   var texture=Resources.Load<Texture2D>(ui?"Art/ui-v2":"Art/world-v2");
   if(texture==null)return null;
   float sx=texture.width/1792f,sy=texture.height/896f;
   int left=Mathf.RoundToInt(region.x*sx),right=Mathf.RoundToInt(region.xMax*sx),bottom=texture.height-Mathf.RoundToInt(region.yMax*sy),top=texture.height-Mathf.RoundToInt(region.y*sy);
   int minX=right,minY=top,maxX=left,maxY=bottom;
   // Trim transparent gutters once when cached. Preserve the source alpha and atlas batching.
   var pixels=texture.GetPixels32();
   for(int y=bottom;y<top;y++)for(int x=left;x<right;x++)if(pixels[y*texture.width+x].a>20){minX=Mathf.Min(minX,x);maxX=Mathf.Max(maxX,x);minY=Mathf.Min(minY,y);maxY=Mathf.Max(maxY,y);}
   if(maxX<=minX||maxY<=minY)return null;
   var rect=new Rect(minX,minY,maxX-minX+1,maxY-minY+1);
   float ppu=ui?100:key=="gun"?rect.width/2:rect.height/2;
   if(ui)pivot=new Vector2(.5f,.5f);
   float border=panel?Mathf.Min((key=="button-art"?98:key=="pill-art"?66:60)*sx,Mathf.Min(rect.width,rect.height)*.49f):0;
   var sprite=Sprite.Create(texture,rect,pivot,ppu,0,SpriteMeshType.FullRect,new Vector4(border,border,border,border));sprite.name="Painted_"+key;return sprite;
  }
 }
}
