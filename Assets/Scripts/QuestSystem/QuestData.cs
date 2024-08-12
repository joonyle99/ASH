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

    [SerializeField] private int _maxRepeatCount;                               // 퀘스트 반복 횟수
    [SerializeField] private int _currentRepeatCount;                           // 현재 퀘스트 반복 횟수

    [Space]

    [SerializeField] private MonsterHuntQuestData _monsterQuest;                    // 몬스터 사냥 퀘스트
    public MonsterHuntQuestData MonsterQuest => _monsterQuest;

    private MonsterHuntQuestData _initMonsterQuest;                                 // 초기 몬스터 사냥 퀘스트

    public bool IsFirst { get; set; } = true;                                   // 퀘스트를 처음 받았는지 여부
    public bool IsActive { get; set; } = false;                                 // 퀘스트 활성화 여부
    public bool IsAcceptedBefore { get; set; } = false;                               // 퀘스트 수락 여부

    #endregion

    #region Function

    private void Awake()
    {
        // 데이터 복사를 통해 초기 데이터를 세팅한다
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
        // _monsterQuest = _initMonsterQuest; // 이렇게 하면 참조 값 자체가 같이지기 때문에 복사 생성자를 이용한다
        _monsterQuest = new MonsterHuntQuestData(_initMonsterQuest);
    }

    // TEMP
    public void IncreaseMaxHp(int amount)
    {
        SceneContext.Current.Player.IncreaseMaxHp(amount);
    }

    #endregion
}