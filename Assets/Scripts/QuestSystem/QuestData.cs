using UnityEngine;

[CreateAssetMenu(menuName = "Quest Data", fileName = "New Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Data")]
    [Space]

    [SerializeField] private int _goalCount;
    [SerializeField] private int _rewardAmount;

    public int GoalCount => _goalCount;
    public int RewardAmount => _rewardAmount;
}
