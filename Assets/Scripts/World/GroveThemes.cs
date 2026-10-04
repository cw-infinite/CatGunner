using UnityEngine;
namespace VerdantTrail {
 public static class GroveThemes {
  public const int Count=10;
  public static int Index(int stage)=>Mathf.Max(0,(stage-1)/5)%Count;
  public static readonly string[] Names={"Meadow Woods","Sunlit Coast","Blossom Spring","Amber Autumn","Snowfall Pines","Moonlit Willow","Bamboo Garden","Mushroom Hollow","Crystal Valley","Ember Grove"};
  static readonly string[] Ground={"#cad68f","#ddc18b","#e1cdd4","#dcc08a","#d5e8ed","#9c93b7","#bfd29b","#cbb8a0","#b5d5df","#b5a09a"};
  static readonly string[] Plateau={"#82a85c","#c9ad75","#b899b5","#b68a51","#a5c6d5","#716484","#7fa06c","#9c816c","#819fb7","#776469"};
  static readonly string[] Rock={"#745341","#a68a62","#987786","#986746","#8096aa","#51465f","#60795e","#766052","#626b92","#574950"};
  static Color Parse(string hex){ColorUtility.TryParseHtmlString(hex,out var c);return c;}
  public static Color Floor(int i)=>Parse(Ground[i]);
  public static Color Top(int i)=>Parse(Plateau[i]);
  public static Color Cliff(int i)=>Parse(Rock[i]);
 }
}
