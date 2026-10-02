using UnityEngine;
namespace VerdantTrail {
 [CreateAssetMenu(menuName="Verdant Trail/Harvest tuning")]
 public sealed class HarvestTuning : ScriptableObject {
  [Header("Movement and camera")]
  public float movementSpeed=1.9f, engagementRange=3.2f, cameraDamping=15f;
  [Header("Combat")]
  public float baseDamage=92f, damageGrowth=1.077f, shotInterval=.62f, speedStep=.078f, projectileSpeed=27f;
  [Header("Economy")]
  public float baseHealth=140f, healthGrowth=1.29f, baseReward=16f, rewardGrowth=1.15f, baseCost=46f, costGrowth=1.365f;
  [Header("Encounter layout")]
  public int targetsPerCluster=6;
  public float firstClusterDistance=3.1f, clusterSpacing=3.0f, clusterDepth=2.6f, clusterWidth=4.4f, clusterLateralOffset=-.9f;
  [Header("Timed harvest challenges")]
  public int challengeUnlockStage=2,challengeGemReward=36;
  public float challengeSeconds=60,arenaRadius=7.5f;
  public int[] challengeCounts={24,40,64};
  public float[] challengeHealth={1f,1.45f,2.2f};
  [Header("Stage flow")]
  public float finaleHealthMultiplier=20f, finaleRewardMultiplier=8f, finaleScale=1.65f;
  public float clearDuration=2f, transferDuration=1.5f;
  public int[] stageTargetCounts={44,72,62,68,72,40,66,76,80,84};
  public int TargetCount(int stage)=>Mathf.Clamp(stageTargetCounts[(stage-1)%stageTargetCounts.Length],1,128);
  public double Cost(int level)=>System.Math.Round(baseCost*System.Math.Pow(costGrowth,level));
  public float Damage(int level)=>baseDamage*Mathf.Pow(damageGrowth,level);
  public float Interval(int level)=>shotInterval/(1+speedStep*level);
  public float Health(int stage)=>baseHealth*Mathf.Pow(healthGrowth,stage-1);
  public double Reward(int stage,int level)=>baseReward*System.Math.Pow(rewardGrowth,stage-1)*System.Math.Pow(damageGrowth,level);
 }
}



