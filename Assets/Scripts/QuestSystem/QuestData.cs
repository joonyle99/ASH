using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 고유한 퀘스트에 관한 정보를 담고 있는 클래스
/// </summary>
[System.Serializable]
public class QuestData
{
    public enum QuestType
    {
        Kill,
        Collect,
        Talk
    }

    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private int _goal;
    [SerializeField] private UnityEvent _reward;

    public bool IsActive { get; set; }
    public string Title => _title;
    public string Description => _description;
    public int Goal => _goal;
    public UnityEvent Reward => _reward;

    /// <summary>
    /// 다이얼로그 데이터에 연결하기 위한 유효성 검사
    /// </summary>
    /// <returns></returns>
    public bool IsValidQuestData()
    {
        if (string.IsNullOrEmpty(_title) || string.IsNullOrWhiteSpace(_title)) return false;
        if (string.IsNullOrEmpty(_description) || string.IsNullOrWhiteSpace(_description)) return false;
        if (_goal <= 0) return false;
        if (_reward == null || _reward.GetPersistentEventCount() == 0) return false;

        return true;
    }
}