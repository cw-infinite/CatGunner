using UnityEngine;
using UnityEngine.UI;
namespace VerdantTrail {
 public sealed partial class FeatureHud {
  GameObject dailyBadge,taskBadge;Text taskBadgeCount;readonly Image[] missionFills=new Image[6];
  RewardSystem rewards;GameObject dailyPanel,missionPanel;Button dailyTile,missionTile,dailyClaim;
  Text dailyStatus,missionReset,passSummary;readonly Text[] dailyLabels=new Text[7],missionLabels=new Text[6],passLabels=new Text[3];
  readonly Image[] dailyCards=new Image[7];readonly Button[] missionButtons=new Button[6],passButtons=new Button[3];
  void BuildRewards(RewardSystem model){
   rewards=model;
   dailyTile=Button("DAILY",canvas.transform,0,.72f,.145f,.05f,()=>{dailyPanel.SetActive(true);dailyPanel.transform.SetAsLastSibling();hud.ResetMovement();RefreshRewards();});
   missionTile=Button("TASKS",canvas.transform,.855f,.72f,.145f,.05f,()=>{missionPanel.SetActive(true);missionPanel.transform.SetAsLastSibling();hud.ResetMovement();RefreshRewards();});
   dailyBadge=RewardBadge(dailyTile,"Daily ready badge",out var dailyMark);dailyMark.text="!";
   taskBadge=RewardBadge(missionTile,"Task ready badge",out taskBadgeCount);
   dailyPanel=Overlay("Daily rewards overlay",.19f,.65f,out var d);
   Text("Daily heading",d,.03f,.025f,.94f,.065f,"SEVEN DAYS IN THE GROVE",25);
   dailyStatus=Text("Daily status",d,.04f,.105f,.92f,.07f,"",18);
   for(int i=0;i<7;i++){
    float x=i==6?.06f:.06f+(i%3)*.30f,y=i==6?.59f:.21f+(i/3)*.19f,w=i==6?.88f:.28f;
    dailyCards[i]=Panel("Day "+(i+1),d,x,y,w,.165f,paper);
    dailyLabels[i]=Text("Daily reward",dailyCards[i].transform,.03f,.04f,.94f,.92f,"",22);
   }
   dailyClaim=Button("CLAIM DAILY",d,.19f,.80f,.62f,.08f,()=>{if(rewards.ClaimDaily())hud.ShowToast("Daily crystals collected");RefreshRewards();});
   Button("CLOSE DAILY",d,.26f,.905f,.48f,.06f,()=>Close(dailyPanel));dailyPanel.SetActive(false);
   missionPanel=Overlay("Missions overlay",.15f,.80f,out var m);
   Text("Missions heading",m,.04f,.017f,.92f,.055f,"FIELD PASS",27);
   passSummary=Text("Pass points",m,.04f,.074f,.92f,.038f,"",19);
   for(int i=0;i<3;i++){
    int index=i;var b=Button("",m,.04f+i*.315f,.13f,.29f,.125f,()=>{rewards.ClaimPass(index);RefreshRewards();});b.name="Claim pass "+i;
    passButtons[i]=b;passLabels[i]=Text("Pass reward",b.transform,.02f,.04f,.96f,.92f,"",18);
   }
   Text("Mission heading",m,.04f,.278f,.92f,.046f,"DAILY MISSIONS",23);
   missionReset=Text("Mission reset",m,.04f,.325f,.92f,.03f,"",16);
   for(int i=0;i<6;i++){
    int index=i;float y=.375f+i*.086f;
    var row=Panel("Mission "+i,m,.035f,y,.93f,.077f,paper);
    missionLabels[i]=Text("Mission progress",row.transform,.025f,.01f,.65f,.78f,"",18);
    var track=Panel("Mission track "+i,row.transform,.05f,.83f,.60f,.07f,new Color(.7f,.69f,.52f));track.raycastTarget=false;missionFills[i]=Panel("Mission fill "+i,track.transform,0,0,1,1,new Color(.25f,.60f,.43f));missionFills[i].raycastTarget=false;
    var b=Button("CLAIM",row.transform,.70f,.13f,.275f,.74f,()=>{rewards.ClaimMission(index);RefreshRewards();});b.name="Claim mission "+i;missionButtons[i]=b;
   }
   Button("CLOSE TASKS",m,.25f,.923f,.5f,.053f,()=>Close(missionPanel));missionPanel.SetActive(false);
  }
  GameObject RewardBadge(Button tile,string name,out Text count){
   var badge=Panel(name,tile.transform,.76f,-.10f,.23f,.42f,new Color(1,.73f,.28f));badge.sprite=OriginalArt.Get("disc");badge.preserveAspect=true;badge.raycastTarget=false;
   count=Text("Ready count",badge.transform,0,0,1,1,"",15);return badge.gameObject;
  }
  void RefreshRewardBadges(){
   dailyBadge.SetActive(rewards.CanClaimDaily);int count=0;
   for(int i=0;i<6;i++)if(rewards.CanClaimMission(i))count++;
   for(int i=0;i<3;i++)if(rewards.CanClaimPass(i))count++;
   taskBadge.SetActive(count>0&&rewards.MissionsUnlocked);taskBadgeCount.text=count.ToString();
  }
  void RefreshRewards(){
   RefreshRewardBadges();
   rewards.RefreshDay();long seconds=rewards.SecondsUntilReset;string remaining=(seconds/3600).ToString("00")+":"+((seconds/60)%60).ToString("00")+":"+(seconds%60).ToString("00");
   if(dailyPanel.activeSelf){
    bool can=rewards.CanClaimDaily;int current=can?rewards.DailyIndex:(sim.save.attendanceClaims+6)%7;
    for(int i=0;i<7;i++){
     bool taken=i<current||(!can&&i==current);dailyCards[i].color=taken?new Color(.64f,.72f,.58f):i==current?mint:paper;
     dailyLabels[i].text="DAY "+(i+1)+"\n<> "+rewards.catalog.dailyGems[i]+"\n"+(taken?"COLLECTED":i==current?"READY":"");
    }
    dailyStatus.text=can?"Your next reward is ready":"Collected today · Next in "+remaining+" UTC";dailyClaim.interactable=can;dailyClaim.GetComponentInChildren<Text>().text=can?"CLAIM DAILY":"COLLECTED TODAY";
   }
   if(missionPanel.activeSelf){
    passSummary.text=sim.save.passPoints+" points earned";missionReset.text="New missions in "+remaining+" UTC";
    for(int i=0;i<3;i++){bool taken=rewards.PassClaimed(i);passButtons[i].interactable=rewards.CanClaimPass(i);passLabels[i].text=rewards.catalog.passGoals[i]+" POINTS\n<> "+rewards.catalog.passGems[i]+"\n"+(taken?"COLLECTED":rewards.CanClaimPass(i)?"CLAIM":"LOCKED");}
    for(int i=0;i<6;i++){
     int goal=rewards.MissionGoal(i);bool taken=rewards.MissionClaimed(i);
     missionLabels[i].text=(i<3?"Harvest plants":"Clear stages")+"  "+Mathf.Min(goal,rewards.MissionProgress(i))+" / "+goal+"\n+"+rewards.catalog.pointsPerMission+" pass points";
     missionFills[i].rectTransform.anchorMax=new Vector2(Mathf.Clamp01((float)rewards.MissionProgress(i)/goal),1);
     bool ready=rewards.CanClaimMission(i);missionButtons[i].interactable=ready;var caption=missionButtons[i].GetComponentInChildren<Text>();caption.text=taken?"DONE":ready?"CLAIM":"IN PROGRESS";caption.fontSize=ready||taken?19:14;
    }
   }
  }
 }
}
