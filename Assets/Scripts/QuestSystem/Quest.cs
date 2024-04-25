using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 퀘스트 정보
/// </summary>
[System.Serializable]
public class Quest
{
    public enum QuestType
    {
        Kill,
        Collect,
        Talk
    }

    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private UnityEvent _reward;

    public string Title => _title;
    public string Description => _description;
    public UnityEvent Reward => _reward;

    // public QuestGOal goal;

}