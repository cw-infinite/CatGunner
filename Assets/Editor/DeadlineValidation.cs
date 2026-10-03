using System;
using UnityEngine;
namespace VerdantTrail.Editor {
 public static class DeadlineValidation {
  public static string Run(HarvestTuning tuning){
   void Require(bool ok,string label){if(!ok)throw new Exception("Deadline check: "+label);}
   HarvestSimulation Scenario(float flightTime){
    var sim=new HarvestSimulation(tuning,new SaveData{stage=2,highestStage=2});sim.automate=false;sim.EnterChallenge();
    for(int i=1;i<sim.total;i++)sim.targets[i].active=false;sim.cleared=sim.total-1;
    sim.targets[0].position=new Vector2(100,100);sim.targets[0].hp=sim.targets[0].maxHp=1;
    sim.Step(tuning.challengeSeconds-.01f);
    sim.shots[0]=new ShotState{active=true,target=0,aim=sim.targets[0].position,position=sim.targets[0].position-Vector2.right*tuning.projectileSpeed*flightTime,damage=10};
    return sim;
   }
   var late=Scenario(.02f);int gems=late.save.gems;late.Step(.03f);
   Require(late.phase==StagePhase.ChallengeResult&&!late.ChallengeWon&&late.targets[0].active&&late.save.gems==gems,"Late impact cannot turn timeout into a win");
   var timely=Scenario(.004f);timely.Step(.03f);Require(timely.ChallengeWon&&!timely.targets[0].active,"Impact before deadline still wins on a long frame");
   gems=timely.save.gems;timely.Step(1);Require(timely.save.gems==gems,"Final-slice reward paid once");
   var idle=new HarvestSimulation(tuning,new SaveData());idle.Step(0);idle.Step(-1);idle.Step(float.NaN);idle.Step(float.PositiveInfinity);Require(idle.elapsed==0&&idle.shotsFired==0,"Invalid time steps do not advance simulation");
   return "PASS: final-frame trial deadline rejects late impacts, accepts timely impacts, pays once and ignores invalid time steps.";
  }
 }
}
