using System;
using System.IO;
using UnityEngine;
namespace VerdantTrail {
 // Explicitly invoked diagnostics, always under a unique validation-owned directory.
 public static class SaveRecoveryChecks {
  public static string Run(string validationRoot){
   string root=Path.GetFullPath(validationRoot),folder=Path.GetFullPath(Path.Combine(root,"save-recovery-"+Guid.NewGuid().ToString("N")));
   if(!folder.StartsWith(root+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new Exception("Invalid diagnostic directory.");
   Directory.CreateDirectory(folder);int count=0;
   void Require(bool ok,string label){if(!ok)throw new Exception("Save recovery: "+label);count++;}
   string Json(int stage,double cash)=>JsonUtility.ToJson(new SaveData{stage=stage,highestStage=stage,cash=cash,gems=42,ownedWeapons=new[]{1,1},equippedWeapons=new[]{0,1,-1},missionClaims=9,passClaims=1,passPoints=20,attendanceClaims=3,lastAttendanceDay=20000});
   try {
    string fresh=Path.Combine(folder,"fresh.json");var data=SaveStore.LoadFrom(fresh);Require(SaveStore.CanWrite&&!SaveStore.Recovered&&data.stage==1,"Missing save starts fresh");
    SaveStore.SaveTo(data,fresh);Require(File.Exists(fresh)&&SaveStore.LastError==null,"First save creates primary");
    string good=Json(5,120),p=Path.Combine(folder,"backup.json");File.WriteAllText(p,"{broken");File.WriteAllText(p+".bak",good);
    data=SaveStore.LoadFrom(p);Require(SaveStore.Recovered&&SaveStore.CanWrite&&data.stage==5&&data.cash==120,"Corrupt primary recovers backup");
    Require(data.equippedWeapons[1]==1&&data.missionClaims==9&&data.attendanceClaims==3,"Recovery includes loadout and claimed rewards");
    Require(File.ReadAllText(p)=="{broken","Loading does not modify damaged file");data.cash=321;SaveStore.SaveTo(data,p);
    Require(SaveStore.LastError==null&&File.ReadAllText(p+".bak")==good,"Repair retains good backup");
    var archives=Directory.GetFiles(folder,"backup.json.damaged-*");Require(archives.Length==1&&File.ReadAllText(archives[0])=="{broken","Damaged primary archived intact");
    data=SaveStore.LoadFrom(p);Require(!SaveStore.Recovered&&data.cash==321&&data.passClaims==1,"Repaired primary reloads with claim flags");
    File.WriteAllText(p+".tmp",Json(8,999));data=SaveStore.LoadFrom(p);Require(data.cash==321&&!SaveStore.Recovered,"Committed primary wins over temp");
    string interrupted=Path.Combine(folder,"interrupted.json");File.WriteAllText(interrupted+".tmp",good);data=SaveStore.LoadFrom(interrupted);
    Require(SaveStore.Recovered&&data.cash==120,"Interrupted first write recovered");SaveStore.SaveTo(data,interrupted);
    Require(File.Exists(interrupted)&&Directory.GetFiles(folder,"interrupted.json.recovered-*").Length==1,"Temp source preserved before repair");
    string missing=Path.Combine(folder,"missing.json");File.WriteAllText(missing+".bak",good);data=SaveStore.LoadFrom(missing);Require(SaveStore.Recovered&&data.stage==5,"Missing primary recovers backup");
    string future=Path.Combine(folder,"future.json");File.WriteAllText(future,"{\"version\":99}");File.WriteAllText(future+".bak",good);SaveStore.LoadFrom(future);Require(!SaveStore.CanWrite&&!SaveStore.Recovered,"Future primary never falls back");SaveStore.SaveTo(new SaveData(),future);Require(File.ReadAllText(future).Contains("99"),"Future primary unchanged");
    string bad=Path.Combine(folder,"bad.json");File.WriteAllText(bad,"{}");File.WriteAllText(bad+".bak","null");File.WriteAllText(bad+".tmp","{truncated");SaveStore.LoadFrom(bad);Require(!SaveStore.CanWrite,"All invalid candidates protect files");SaveStore.SaveTo(new SaveData(),bad);Require(File.ReadAllText(bad)=="{}","Invalid source not silently reset");
    File.WriteAllText(bad+".bak","{\"version\":99}");File.WriteAllText(bad+".tmp",good);SaveStore.LoadFrom(bad);Require(!SaveStore.CanWrite,"Future backup blocks downgrade to temp");
    string blocked=Path.Combine(folder,"blocked.json");File.WriteAllText(blocked,good);data=SaveStore.LoadFrom(blocked);Directory.CreateDirectory(blocked+".tmp");SaveStore.SaveTo(data,blocked);Require(SaveStore.LastError!=null&&File.ReadAllText(blocked)==good,"Failed temp write preserves committed save");
    Directory.Delete(blocked+".tmp");SaveStore.SaveTo(data,blocked);Require(SaveStore.LastError==null&&File.ReadAllText(blocked+".bak")==good,"Write retries successfully after I/O fault");
    return "PASS: "+count+" filesystem checks for backup/temp recovery, archival, equipment/reward persistence, future-version protection and failed-write retry.";
   }finally {SaveStore.LoadFrom(Path.Combine(folder,"reset-state.json"));Directory.Delete(folder,true);}
  }
 }
}
