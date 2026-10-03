using UnityEngine;
using UnityEngine.UI;
namespace VerdantTrail {
 public static class UiFinish {
  public static Outline Border(Graphic graphic){var outline=graphic.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.27f,.13f,.065f,.9f);outline.effectDistance=Vector2.zero;outline.useGraphicAlpha=true;return outline;}
  public static void Skin(Image image,string art){image.sprite=OriginalArt.Get(art);image.type=Image.Type.Sliced;image.pixelsPerUnitMultiplier=art=="button-art"?4:2;image.color=Color.white;}
  public static void WhiteText(Text text){text.color=new Color(1,.98f,.9f);var o=text.gameObject.AddComponent<Outline>();o.effectColor=new Color(.28f,.15f,.09f);o.effectDistance=new Vector2(2,-2);}
  public static void Button(Button button){Border(button.targetGraphic);var colors=button.colors;colors.highlightedColor=new Color(1,.98f,.85f);colors.pressedColor=new Color(.75f,.86f,.77f);colors.disabledColor=new Color(.65f,.69f,.63f,.8f);colors.fadeDuration=.08f;button.colors=colors;}
 }
}
