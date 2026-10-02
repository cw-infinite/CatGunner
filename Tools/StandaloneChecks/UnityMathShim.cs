// Test-only compatibility layer for the mathematical Unity APIs used by the exact core sources.
// This does not simulate the Unity renderer, input system, audio, lifecycle or serialization.
using System;
namespace UnityEngine {
 public class ScriptableObject {}
 public sealed class CreateAssetMenuAttribute:Attribute {public string menuName;}
 public sealed class HeaderAttribute:Attribute {public HeaderAttribute(string text){}}
 public static class Application {public static string persistentDataPath=>System.IO.Path.GetTempPath();}
 public static class Debug {public static void LogWarning(object v)=>Console.WriteLine(v);}
 public static class JsonUtility {public static T FromJson<T>(string s)=>throw new NotSupportedException();public static string ToJson(object o,bool pretty)=>throw new NotSupportedException();}
 public static class Mathf {
  public const float PI=(float)Math.PI;
  public static float Pow(float x,float y)=>(float)Math.Pow(x,y);public static float Exp(float x)=>(float)Math.Exp(x);
  public static float Sin(float x)=>(float)Math.Sin(x);public static float Cos(float x)=>(float)Math.Cos(x);
  public static float Min(float x,float y)=>Math.Min(x,y);public static float Max(float x,float y)=>Math.Max(x,y);public static int Clamp(int x,int min,int max)=>Math.Min(max,Math.Max(min,x));
  public static int CeilToInt(float x)=>(int)Math.Ceiling(x);
 }
 public struct Vector2 {
  public float x,y;public Vector2(float x,float y){this.x=x;this.y=y;}
  public static Vector2 zero=>new Vector2();public static Vector2 right=>new Vector2(1,0);public static Vector2 left=>new Vector2(-1,0);public static Vector2 up=>new Vector2(0,1);
  public float magnitude=>(float)Math.Sqrt(sqrMagnitude);
  public static float Dot(Vector2 a,Vector2 b)=>a.x*b.x+a.y*b.y;
  public float sqrMagnitude=>x*x+y*y;public Vector2 normalized {get{float length=(float)Math.Sqrt(sqrMagnitude);return length>0?this/length:zero;}}
  public static Vector2 operator +(Vector2 a,Vector2 b)=>new Vector2(a.x+b.x,a.y+b.y);public static Vector2 operator -(Vector2 a,Vector2 b)=>new Vector2(a.x-b.x,a.y-b.y);
  public static Vector2 operator *(Vector2 a,float b)=>new Vector2(a.x*b,a.y*b);public static Vector2 operator /(Vector2 a,float b)=>new Vector2(a.x/b,a.y/b);
  public static Vector2 MoveTowards(Vector2 a,Vector2 b,float step){var d=b-a;return d.sqrMagnitude<=step*step?b:a+d.normalized*step;}
 }
}
