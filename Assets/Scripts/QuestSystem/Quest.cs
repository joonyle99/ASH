using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 현재의 Quest 클래스는 Nullable이 될 수 없다.
/// 왜냐하면 inspector에서 serialized field 사용되기 때문이다.
/// 따라서 serialized field를 사용하지 않고, monoBehaviour를 상속받아 사용해야 한다.
/// 
/// 현재 Quest 클래스는 Merchant와 Ending에서 사용하고 있는데
/// 이를 고려하여 문제가 없을지 확인하고, 재작업이 필요하다면 수정해야 한다.
/// </summary>
[System.Serializable]
public class Quest
{
    #region Attribute

    [SerializeField] private QuestData _questData; // GoalCount과 RewardAmount를 가지고 있는 ScriptableObject
    public QuestData QuestData => _questData;

    [Space]

    [SerializeField] private int _id;
    public int Id => _id;

    [Space]

    [Header("Condition - Count")]
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
    [SerializeField] private int _currentRepeatCount;                               // 현재 퀘스트 반복 횟수
    public int CurrentRepeatCount => _currentRepeatCount;
    [SerializeField] private int _maxRepeatCount;                                   // 최대 퀘스트 반복 횟수
    public int MaxRepeatCount => _maxRepeatCount;

    [Space]

    [Header("Condition - Toggle")]
    [SerializeField] private bool _isActive;                                        // 퀘스트 활성화 여부
    [SerializeField] private bool _isAcceptedBefore;                                // 퀘스트 수락 여부

    [Space]

    [Header("Options")]
    [SerializeField] private bool _isAutoFirst;                                     // 자동 수락 여부 (처음에만)

    [Space]

    [Header("Callback")]
    [SerializeField] private UnityEvent _onAccept;
    public UnityEvent OnAccept => _onAccept;
    [SerializeField] private UnityEvent _onReject;
    public UnityEvent OnReject => _onReject;

    public bool IsAutoFirst
    {
        get => _isAutoFirst;
        set => _isAutoFirst = value;
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

    public bool CanRepeat => _currentRepeatCount < _maxRepeatCount;

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

        _currentCount = 0;
        _currentRepeatCount++;

        IsActive = false;
        IsAcceptedBefore = false;
    }

    public bool IsComplete()
    {
        return _currentCount >= _questData.GoalCount;
    }
    public bool IsRepeatable()
    {
        return _currentRepeatCount < _maxRepeatCount;
    }

    #endregion
}