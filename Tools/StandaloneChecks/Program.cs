using System;
using System.IO;
using System.Text;
using VerdantTrail;
using UnityEngine;
class Program {
 static int checks;
 static void Check(bool condition,string message){if(!condition)throw new Exception(message);checks++;}
 static void Main() {
  var t=new HarvestTuning();var s=new HarvestSimulation(t,new SaveData());
  Check(!s.Buy(0),"Unaffordable purchase");s.save.cash=1000;Check(s.Buy(0)&&s.save.cash==954&&s.save.force==1,"Exact debit");
  Check(t.Interval(5)<t.Interval(0)&&t.Damage(5)>t.Damage(0),"Upgrade effects");
  var manual=new HarvestSimulation(t,new SaveData());manual.automate=false;manual.manualInput=Vector2.left;manual.Step(.1f);Check(manual.units[0].position.x<0,"Manual movement");
  manual.manualInput=Vector2.zero;var pos=manual.units[0].position;manual.Step(.1f);Check((manual.units[0].position-pos).sqrMagnitude==0,"Joystick release");
  var drops=new HarvestSimulation(t,new SaveData());drops.automate=false;double expected=0;
  for(int i=0;i<drops.total;i++){expected+=Math.Round(t.Reward(1,0)*(drops.targets[i].kind==0?1:1.6));drops.Damage(i,float.MaxValue);drops.Damage(i,float.MaxValue);}
  for(int i=0;i<130;i++)drops.Step(1f/60);
  Check(Math.Abs(drops.save.cash-expected)<.001,"Pickup currency not duplicated or lost");Check(drops.cleared==drops.total,"Death count unique");
  var migrated=SaveStore.Sanitize(new SaveData{version=1,cash=double.NaN,stage=-3,force=-1});Check(migrated.version==4&&migrated.cash==0&&migrated.stage==1&&migrated.force==0,"Migration/sanitize");
  bool future=false;try{SaveStore.Sanitize(new SaveData{version=20});}catch(InvalidDataException){future=true;}Check(future,"Future save rejected");
  var a=new HarvestSimulation(t,new SaveData());var b=new HarvestSimulation(t,new SaveData());for(int i=0;i<a.total;i++)Check((a.targets[i].position-b.targets[i].position).sqrMagnitude==0,"Seed determinism");
  a=new HarvestSimulation(t,new SaveData());a.automate=false;int target=a.Nearest(a.units[0].position,10000);a.units[0].position=a.targets[target].position-Vector2.right*2;a.units[0].target=target;
  a.Step(.001f);Check(a.shotsFired==1&&a.targets[target].hp==a.targets[target].maxHp,"Projectile has travel before damage");
  for(int i=0;i<15;i++)a.Step(1f/60);Check(a.targets[target].hp<a.targets[target].maxHp,"Projectile impact damages");
  var csv=new StringBuilder("stage,seconds,force,tempo,yield,cash,earned\n");s=new HarvestSimulation(t,new SaveData());int serial=s.stageSerial,previous=1;float start=0;int maxShots=0,maxDrops=0;
  for(int i=0;i<36000;i++) {
   s.Step(1f/60);
   if(i%60==0){int category=s.save.force<=s.save.tempo&&s.save.force<=s.save.yield?0:s.save.tempo<=s.save.yield?1:2;s.Buy(category);}
   int active=0;foreach(var shot in s.shots)if(shot.active)active++;maxShots=Math.Max(maxShots,active);active=0;foreach(var note in s.drops)if(note.active)active++;maxDrops=Math.Max(maxDrops,active);
   Check(s.save.cash>=0,"Nonnegative cash");
   if(serial!=s.stageSerial){csv.AppendLine($"{previous},{s.elapsed-start:0.00},{s.save.force},{s.save.tempo},{s.save.yield},{s.save.cash:0.00},{s.earned:0.00}");start=s.elapsed;previous=s.save.stage;serial=s.stageSerial;}
  }
  Check(s.save.stage>=4,"Autonomous progression");
  File.WriteAllText("Validation/standalone-simulation.csv",csv.ToString());
  string result=$"PASS: {checks} assertions on exact core C# sources, with a test-only Unity math shim.\n600-second simulation: stage {s.save.stage}; targets {s.targetsDestroyed}; shots {s.shotsFired}; upgrades {s.purchases}; peak active shots {maxShots}, notes {maxDrops}.\nNot an engine play test; renderer, UI, audio, save file I/O and Android performance remain unverified.\n";
  File.WriteAllText("Validation/standalone-checks.txt",result);Console.WriteLine(result);
  var table=new StringBuilder("stage,targets,base_hp,base_reward,assumed_upgrade_level,nominal_dps,next_upgrade_cost,ideal_shooting_seconds\n");
  for(int i=1;i<=10;i++){int l=(i-1)*3;float dps=t.Damage(l)/t.Interval(l);table.AppendLine($"{i},{t.TargetCount(i)},{t.Health(i):0.00},{t.Reward(i,l):0.00},{l},{dps:0.00},{t.Cost(l):0.00},{t.TargetCount(i)*t.Health(i)*1.15/dps:0.00}");}
  File.WriteAllText("Analysis/economy_tuning.csv",table.ToString());
 }
}

