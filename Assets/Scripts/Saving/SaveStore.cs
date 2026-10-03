using System;
using System.IO;
using System.Text;
using UnityEngine;
namespace VerdantTrail {
 [Serializable] public sealed class SaveData {
  public int version=6, stage=1, highestStage=1;
  public double cash;
  public int gems, force, tempo, yield;
  public int challengeWins;
  public int ownedSkins=1,equippedSkin;
  public long missionDay,lastAttendanceDay;
  public int dailyHarvests,dailyStages,missionClaims,passPoints,passClaims,attendanceClaims;
  public int[] ownedWeapons={1,0,0,0},equippedWeapons={0,-1,-1};
  public string[] unlockedSystems=Array.Empty<string>();
  public int[] characterProgress=Array.Empty<int>(), weaponProgress=Array.Empty<int>(), companionProgress=Array.Empty<int>();
  public int prestige;
  public bool sound=true;
  public long offlineTimestamp;
 }
 public static class SaveStore {
  public const int CurrentVersion=6;
  static string PathName=>Path.Combine(Application.persistentDataPath,"verdant-trail-v2.json");
  public static string LastError {get;private set;}
  public static bool CanWrite {get;private set;}=true;
  public static bool Recovered {get;private set;}
  public static string StatusMessage {get;private set;}="Progress saves automatically.";
  static string recoveredPath,recoveredSource;
  [Serializable] sealed class Header {public int version=0;}
  sealed class FutureSaveException:Exception {public FutureSaveException():base("Save is from a newer version."){} }
  static SaveData ReadValidated(string path){
   string json=File.ReadAllText(path).Trim();
   if(!json.StartsWith("{")||!json.EndsWith("}"))throw new InvalidDataException("Incomplete save JSON.");
   var header=JsonUtility.FromJson<Header>(json);
   if(header==null||header.version<1)throw new InvalidDataException("Save version is missing.");
   if(header.version>CurrentVersion)throw new FutureSaveException();
   var data=JsonUtility.FromJson<SaveData>(json);if(data==null)throw new InvalidDataException("Save is empty.");return Sanitize(data);
  }
  public static SaveData Sanitize(SaveData d) {
   if(d==null)d=new SaveData();
   if(d.version>CurrentVersion)throw new InvalidDataException("Save is from a newer version.");
   // Keep the existing filename; absent reward fields migrate to unclaimed/zero progress.
   d.version=CurrentVersion;
   d.ownedSkins=(d.ownedSkins&511)|1;d.equippedSkin=Mathf.Clamp(d.equippedSkin,0,8);if((d.ownedSkins&(1<<d.equippedSkin))==0)d.equippedSkin=0; d.stage=Mathf.Clamp(d.stage,1,100); d.highestStage=Mathf.Clamp(Math.Max(d.stage,d.highestStage),1,100);
   d.missionDay=Math.Max(0,d.missionDay);d.lastAttendanceDay=Math.Max(0,d.lastAttendanceDay);
   d.dailyHarvests=Mathf.Clamp(d.dailyHarvests,0,1000000);d.dailyStages=Mathf.Clamp(d.dailyStages,0,1000000);
   d.missionClaims&=63;d.passClaims&=7;d.passPoints=Mathf.Clamp(d.passPoints,0,1000000);d.attendanceClaims=Mathf.Clamp(d.attendanceClaims,0,1000000);
   d.force=Mathf.Clamp(d.force,0,100);d.tempo=Mathf.Clamp(d.tempo,0,100);d.yield=Mathf.Clamp(d.yield,0,100);
   if(double.IsNaN(d.cash)||double.IsInfinity(d.cash)||d.cash<0)d.cash=0;
   d.unlockedSystems=d.unlockedSystems??Array.Empty<string>();d.characterProgress=d.characterProgress??Array.Empty<int>();
   d.weaponProgress=d.weaponProgress??Array.Empty<int>();d.companionProgress=d.companionProgress??Array.Empty<int>();
   d.gems=Mathf.Clamp(d.gems,0,1000000);d.challengeWins=Mathf.Clamp(d.challengeWins,0,100);
   if(d.ownedWeapons==null)d.ownedWeapons=new[]{1,0,0,0};else if(d.ownedWeapons.Length!=4)Array.Resize(ref d.ownedWeapons,4);
   // Ownership includes equipped items as well as the twelve inventory cells and up to three squad slots.
   d.ownedWeapons[0]=Mathf.Clamp(d.ownedWeapons[0],1,15);for(int i=1;i<4;i++)d.ownedWeapons[i]=Mathf.Clamp(d.ownedWeapons[i],0,15);
   if(d.equippedWeapons==null||d.equippedWeapons.Length!=3)d.equippedWeapons=new[]{0,-1,-1};
   if(d.equippedWeapons[0]<0||d.equippedWeapons[0]>3||d.ownedWeapons[d.equippedWeapons[0]]==0)d.equippedWeapons[0]=0;
   var equippedCounts=new int[4];equippedCounts[d.equippedWeapons[0]]++;
   for(int slot=1;slot<3;slot++){int id=d.equippedWeapons[slot];if(id<0||id>=4||equippedCounts[id]>=d.ownedWeapons[id])d.equippedWeapons[slot]=-1;else equippedCounts[id]++;}
   return d;
  }
  public static SaveData Load()=>LoadFrom(PathName);
  public static SaveData LoadFrom(string path) {
   LastError=null;CanWrite=true;Recovered=false;recoveredPath=null;recoveredSource=null;StatusMessage="Progress saves automatically.";
   path=Path.GetFullPath(path);bool found=false;string failure=null;
   // A valid primary wins over uncommitted data; prefer the last committed backup next.
   foreach(string candidate in new[]{path,path+".bak",path+".tmp"}){
    if(!File.Exists(candidate))continue;found=true;
    try {
     var data=ReadValidated(candidate);
     if(candidate!=path){Recovered=true;recoveredPath=path;recoveredSource=candidate;StatusMessage=candidate.EndsWith(".bak")?"Recovered progress from backup.":"Recovered an interrupted save.";}
     else StatusMessage="Saved progress loaded.";
     return data;
    }catch(FutureSaveException e){failure=e.Message;break;}
    catch(Exception e){failure=e.Message;}
   }
   if(found){LastError=failure;CanWrite=false;StatusMessage="Save unavailable. Existing files protected; saving disabled.";Debug.LogWarning("Save load failed; existing files protected: "+failure);}
   return new SaveData();
  }
  public static void Save(SaveData data)=>SaveTo(data,PathName);
  public static void SaveTo(SaveData data,string path) {
   if(!CanWrite)return;
   try {
    LastError=null;data.offlineTimestamp=DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    path=Path.GetFullPath(path);bool repairing=string.Equals(path,recoveredPath,StringComparison.OrdinalIgnoreCase);
    // Preserve the bad primary and keep the known-good backup during the first repair.
    if(repairing&&File.Exists(path))File.Copy(path,path+".damaged-"+Guid.NewGuid().ToString("N"));
    if(repairing&&recoveredSource==path+".tmp"&&File.Exists(recoveredSource))File.Copy(recoveredSource,path+".recovered-"+Guid.NewGuid().ToString("N"));
    string temp=path+".tmp";
    using(var stream=new FileStream(temp,FileMode.Create,FileAccess.Write,FileShare.None)){
     using(var writer=new StreamWriter(stream,new UTF8Encoding(false),1024,true)){writer.Write(JsonUtility.ToJson(data,true));writer.Flush();}
     stream.Flush(true);
    }
    if(File.Exists(path))File.Replace(temp,path,repairing?null:path+".bak");else File.Move(temp,path);
    if(repairing)recoveredPath=null;StatusMessage="Progress saved.";
   }catch(Exception e){LastError=e.Message;StatusMessage="Save failed. Check storage access.";Debug.LogWarning("Save write: "+e.Message);}
  }
 }
}
