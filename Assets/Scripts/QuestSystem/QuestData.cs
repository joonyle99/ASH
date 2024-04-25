using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ������ ����Ʈ�� ���� ������ ��� �ִ� Ŭ����
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
    /// ���̾�α� �����Ϳ� �����ϱ� ���� ��ȿ�� �˻�
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