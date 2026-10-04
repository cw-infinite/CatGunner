using UnityEngine;
using UnityEngine.UI;
namespace VerdantTrail {
 public static class UiFinish {
  static Font rounded;public static Font Font=>rounded!=null?rounded:(rounded=Resources.Load<Font>("Fonts/LilitaOne-Regular")??Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
  public static Outline Border(Graphic graphic){var outline=graphic.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.27f,.13f,.065f,.9f);outline.effectDistance=Vector2.zero;outline.useGraphicAlpha=true;return outline;}
  public static void Skin(Image image,string art){image.sprite=OriginalArt.Get(art);image.type=Image.Type.Sliced;image.pixelsPerUnitMultiplier=art.StartsWith("rail")?7:art=="button-art"?4:2;image.color=Color.white;}
  public static void WhiteText(Text text){text.color=Color.white;text.fontStyle=FontStyle.Normal;var o=text.GetComponent<Outline>()??text.gameObject.AddComponent<Outline>();o.effectColor=new Color(.08f,.045f,.025f,1);o.effectDistance=new Vector2(1.6f,-1.6f);o.useGraphicAlpha=true;}
  public static void Button(Button button){Border(button.targetGraphic);var colors=button.colors;colors.highlightedColor=new Color(1,.98f,.85f);colors.pressedColor=new Color(.75f,.86f,.77f);colors.disabledColor=new Color(.65f,.69f,.63f,.8f);colors.fadeDuration=.08f;button.colors=colors;}
 }
}
