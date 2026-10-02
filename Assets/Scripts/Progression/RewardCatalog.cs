using UnityEngine;
namespace VerdantTrail {
 [CreateAssetMenu(menuName="Verdant Trail/Reward catalog")]
 public sealed class RewardCatalog:ScriptableObject {
  public int missionUnlockStage=3,pointsPerMission=10;
  public int[] harvestGoals={100,300,700},stageGoals={1,3,5};
  public int[] passGoals={20,40,60},passGems={24,36,48};
  public int[] dailyGems={12,18,24,30,36,42,60};
 }
}
