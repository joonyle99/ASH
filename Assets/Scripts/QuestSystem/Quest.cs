using UnityEngine;

[System.Serializable]
public class Quest
{
    public Quest(Quest quest)
    {
        _questData = quest.QuestData;

        _currentCount = quest.CurrentCount;

        _maxRepeatCount = quest.MaxRepeatCount;
        _currentRepeatCount = quest.CurrentRepeatCount;

        IsFirst = quest.IsFirst;
        IsActive = quest.IsActive;
        IsAcceptedBefore = quest.IsAcceptedBefore;
    }

    #region Attribute

    // quest�� ���� ID
    [SerializeField] private int _id;
    public int Id => _id;

    [Space]

    [SerializeField] private QuestData _questData;
    public QuestData QuestData => _questData;

    [Space]

    [SerializeField] private int _currentCount;
    public int CurrentCount
    {
        get => _currentCount;
        set
        {
            _currentCount = value;

            if (_currentCount >= _questData.GoalCount)
                _currentCount = _questData.GoalCount;
        }
    }

    [Space]

    [SerializeField] private int _maxRepeatCount;                                   // �ִ� ����Ʈ �ݺ� Ƚ��
    public int MaxRepeatCount => _maxRepeatCount;
    [SerializeField] private int _currentRepeatCount;                               // ���� ����Ʈ �ݺ� Ƚ��
    public int CurrentRepeatCount => _currentRepeatCount;

    [Space]

    [SerializeField] private bool _isFirst = true;                                  // ����Ʈ�� ó�� �޾Ҵ��� ����
    [SerializeField] private bool _isActive;                                        // ����Ʈ Ȱ��ȭ ����
    [SerializeField] private bool _isAcceptedBefore;                                // ����Ʈ ���� ����

    public bool IsFirst
    {
        get => _isFirst;
        set => _isFirst = value;
    }                                     
    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }
    public bool IsAcceptedBefore
    {
        get => _isAcceptedBefore;
        set => _isAcceptedBefore = value;
    }

    #endregion

    #region Function

    public void IncreaseCount()
    {
        CurrentCount++;

        QuestController.Instance.UpdateQuest();
    }
    public void CompleteProcess()
    {
        SceneContext.Current.Player.IncreaseMaxHp(_questData.RewardAmount);

        _currentRepeatCount++;
        _currentCount = 0;

        IsActive = false;
    }

    public bool IsComplete()
    {
        return CurrentCount >= _questData.GoalCount;
    }
    public bool IsRepeatable()
    {
        return _currentRepeatCount < _maxRepeatCount;
    }

    #endregion
}