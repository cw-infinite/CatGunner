using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace VerdantTrail {
 // Preserve the reference composition inside the usable display area.
 public sealed class PortraitLayout {
  readonly Canvas canvas;readonly CanvasScaler scaler;public readonly RectTransform Root;
  public Rect PixelRect {get;private set;}
  int lastWidth,lastHeight;Rect lastSafe;
  readonly List<RectTransform> backdrops=new List<RectTransform>();
  int appliedWidth,appliedHeight;
  public PortraitLayout(Canvas owner,CanvasScaler scaling){canvas=owner;scaler=scaling;Root=new GameObject("Safe portrait content",typeof(RectTransform)).GetComponent<RectTransform>();Root.SetParent(canvas.transform,false);Refresh();}
  public static Rect Fit(int width,int height,Rect safe){
   float left=Mathf.Clamp(safe.xMin,0,width),bottom=Mathf.Clamp(safe.yMin,0,height),right=Mathf.Clamp(safe.xMax,left,width),top=Mathf.Clamp(safe.yMax,bottom,height);
   if(right-left<1||top-bottom<1){left=bottom=0;right=width;top=height;}
   float scale=Mathf.Min((right-left)/588f,(top-bottom)/1280f),w=588*scale,h=1280*scale;
   return new Rect(left+(right-left-w)*.5f,bottom+(top-bottom-h)*.5f,w,h);
  }
  public bool Refresh(){int w=Screen.width,h=Screen.height;Rect safe=Screen.safeArea;if(w==lastWidth&&h==lastHeight&&safe==lastSafe)return false;lastWidth=w;lastHeight=h;lastSafe=safe;Apply(w,h,safe);return true;}
  public void Apply(int width,int height,Rect safe){
   appliedWidth=width;appliedHeight=height;
   PixelRect=Fit(width,height,safe);scaler.uiScaleMode=CanvasScaler.ScaleMode.ConstantPixelSize;scaler.scaleFactor=PixelRect.width/588f;canvas.scaleFactor=scaler.scaleFactor;
   Root.anchorMin=new Vector2(PixelRect.xMin/width,PixelRect.yMin/height);Root.anchorMax=new Vector2(PixelRect.xMax/width,PixelRect.yMax/height);Root.offsetMin=Root.offsetMax=Vector2.zero;
   foreach(var backdrop in backdrops)FitBackdrop(backdrop);
  }
  void FitBackdrop(RectTransform rt){rt.anchorMin=new Vector2(-PixelRect.x/PixelRect.width,-PixelRect.y/PixelRect.height);rt.anchorMax=new Vector2((appliedWidth-PixelRect.x)/PixelRect.width,(appliedHeight-PixelRect.y)/PixelRect.height);rt.offsetMin=rt.offsetMax=Vector2.zero;}
  public void AddBackdrop(Transform parent,Color color){var go=new GameObject("Full screen modal shade",typeof(RectTransform),typeof(Image));var rt=go.GetComponent<RectTransform>();rt.SetParent(parent,false);go.GetComponent<Image>().color=color;backdrops.Add(rt);FitBackdrop(rt);}
  public void Restore(){Apply(Screen.width,Screen.height,Screen.safeArea);}
  public Vector2 LocalPoint(Vector2 screenPoint){RectTransformUtility.ScreenPointToLocalPointInRectangle(Root,screenPoint,canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera,out var point);return point+Root.rect.size*.5f;}
 }
}
