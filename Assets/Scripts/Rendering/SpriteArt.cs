using UnityEngine;
namespace VerdantTrail {
 // Regions describe independently generated sheets, never pixels from the reference video.
 public static class SpriteArt {
  public static Sprite Load(string key){
   bool ui=false;string atlas=null;Rect region;Vector2 pivot=new Vector2(.5f,0);bool panel=false;
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
    default:
     if(key.StartsWith("hud")&&int.TryParse(key.Substring(3),out int icon)&&icon>=0&&icon<12){atlas="Art/hud-icons-v1";ui=true;float[] rows={0,338,594,896};int row=icon/4;region=new Rect(icon%4*448,rows[row],448,rows[row+1]-rows[row]);}
     else if(key.StartsWith("rail")&&int.TryParse(key.Substring(4),out int style)&&style>=0&&style<4){atlas="Art/rail-buttons-v1";ui=true;panel=true;region=new Rect(style%2*896,style/2*448,896,448);}
     else if(key.StartsWith("skin")&&int.TryParse(key.Substring(4),out int skin)&&skin>=0&&skin<8){atlas="Art/skins-v1";float[] edges={0,480,910,1344,1792};region=new Rect(edges[skin%4],skin<4?0:450,edges[skin%4+1]-edges[skin%4],skin<4?450:446);}
     else if(key.StartsWith("weapon")&&int.TryParse(key.Substring(6),out int weapon)&&weapon>=0&&weapon<4){atlas="Art/guns-v2";region=new Rect(weapon%2==0?0:933,weapon<2?0:448,weapon%2==0?933:859,448);pivot=new Vector2(.22f,.45f);}
     else if(key.StartsWith("scenery")&&int.TryParse(key.Substring(7),out int prop)&&prop>=0&&prop<8){atlas="Art/scenery-v1";region=new Rect(prop%4*448,prop/4*448,448,448);}
     else return null;break;
   }
   var texture=Resources.Load<Texture2D>(atlas??(ui?"Art/ui-v2":"Art/world-v2"));
   if(texture==null)return null;
   float sx=texture.width/1792f,sy=texture.height/896f;
   int left=Mathf.RoundToInt(region.x*sx),right=Mathf.RoundToInt(region.xMax*sx),bottom=texture.height-Mathf.RoundToInt(region.yMax*sy),top=texture.height-Mathf.RoundToInt(region.y*sy);
   int minX=right,minY=top,maxX=left,maxY=bottom;
   // Trim transparent gutters once when cached. Preserve the source alpha and atlas batching.
   var pixels=texture.GetPixels32();
   for(int y=bottom;y<top;y++)for(int x=left;x<right;x++)if(pixels[y*texture.width+x].a>20){minX=Mathf.Min(minX,x);maxX=Mathf.Max(maxX,x);minY=Mathf.Min(minY,y);maxY=Mathf.Max(maxY,y);}
   if(maxX<=minX||maxY<=minY)return null;
   var rect=new Rect(minX,minY,maxX-minX+1,maxY-minY+1);
   float ppu=ui?100:(key=="gun"||key.StartsWith("weapon"))?rect.width/2:rect.height/2;
   if(ui)pivot=new Vector2(.5f,.5f);
   float border=panel?Mathf.Min((key.StartsWith("rail")?145:key=="button-art"?98:key=="pill-art"?66:60)*sx,Mathf.Min(rect.width,rect.height)*.49f):0;
   var sprite=Sprite.Create(texture,rect,pivot,ppu,0,SpriteMeshType.FullRect,new Vector4(border,border,border,border));sprite.name="Painted_"+key;return sprite;
  }
 }
}
