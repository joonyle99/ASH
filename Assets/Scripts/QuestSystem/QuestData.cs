using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class QuestData : MonoBehaviour
{
    [System.Serializable]
    public class MonsterHuntQuestData
    {
        [SerializeField] private int _monsterKilled;
        [SerializeField] private int _killGoal;
        [SerializeField] private UnityEvent _reward;

        public int MonsterKilled => _monsterKilled;
        public int KillGoal => _killGoal;
        public UnityEvent Reward => _reward;

        public MonsterHuntQuestData()
        {
            this._monsterKilled = 0;
            this._killGoal = 5;
            this._reward = new UnityEvent();
        }
        public MonsterHuntQuestData(MonsterHuntQuestData monsterQuestData)
        {
            this._monsterKilled = monsterQuestData._monsterKilled;
            this._killGoal = monsterQuestData._killGoal;
            this._reward = monsterQuestData._reward;
        }

        public void IncreaseKillCount()
        {
            _monsterKilled++;
            if (_monsterKilled >= _killGoal)
                _monsterKilled = _killGoal;
        }
        public bool IsCompleteGoal()
        {
            return _monsterKilled >= _killGoal;
        }
        public void GiveReward()
        {
            _reward?.Invoke();
        }
    }

    #region Attribute

    [SerializeField] private int _maxRepeatCount;                               // ����Ʈ �ݺ� Ƚ��
    [SerializeField] private int _currentRepeatCount;                           // ���� ����Ʈ �ݺ� Ƚ��

    [Space]

    [SerializeField] private MonsterHuntQuestData _monsterQuest;                    // ���� ��� ����Ʈ
    public MonsterHuntQuestData MonsterQuest => _monsterQuest;

    private MonsterHuntQuestData _initMonsterQuest;                                 // �ʱ� ���� ��� ����Ʈ

    public bool IsFirst { get; set; } = true;                                   // ����Ʈ�� ó�� �޾Ҵ��� ����
    public bool IsActive { get; set; } = false;                                 // ����Ʈ Ȱ��ȭ ����
    public bool IsAcceptedBefore { get; set; } = false;                               // ����Ʈ ���� ����

    #endregion

    #region Function

    private void Awake()
    {
        // ������ ���縦 ���� �ʱ� �����͸� �����Ѵ�
        _initMonsterQuest = new MonsterHuntQuestData(_monsterQuest);
    }

    public void IncreaseCount()
    {
        _monsterQuest.IncreaseKillCount();

        QuestController.Instance.UpdateQuest();
    }
    public bool IsComplete()
    {
        return _monsterQuest.IsCompleteGoal();
    }
    public bool IsRepeatable()
    {
        return _currentRepeatCount < _maxRepeatCount;
    }
    public void CompleteQuestProcess()
    {
        _monsterQuest.GiveReward();

        _currentRepeatCount++;

        IsActive = false;

        ResetProgress();
    }
    public void ResetProgress()
    {
        // _monsterQuest = _initMonsterQuest; // �̷��� �ϸ� ���� �� ��ü�� �������� ������ ���� �����ڸ� �̿��Ѵ�
        _monsterQuest = new MonsterHuntQuestData(_initMonsterQuest);
    }

    // TEMP
    public void IncreaseMaxHp(int amount)
    {
        SceneContext.Current.Player.IncreaseMaxHp(amount);
    }

    #endregion
}