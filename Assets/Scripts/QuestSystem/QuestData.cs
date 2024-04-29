using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 고유한 퀘스트에 관한 정보를 담고 있는 클래스
/// 현재로써는 일반몬스터 사냥 퀘스트만을 지원한다
/// </summary>
[System.Serializable]
public class QuestData : MonoBehaviour
{
    [System.Serializable]
    public class MonsterQuestData
    {
        public int current;
        public int goal;
        public UnityEvent reward;

        public MonsterQuestData()
        {
            this.current = 0;
            this.goal = 10;
            this.reward = new UnityEvent();
        }

        public MonsterQuestData(MonsterQuestData monsterQuestData)
        {
            this.current = monsterQuestData.current;
            this.goal = monsterQuestData.goal;
            this.reward = monsterQuestData.reward;
        }

        public void PrintData()
        {
            Debug.Log($"current: {current} / goal: {goal}");
        }
    }

    [SerializeField] private MonsterQuestData _monsterQuest;    // 몬스터 사냥 퀘스트 데이터
    private MonsterQuestData _initMonsterQuestData;              // 초기 몬스터 사냥 퀘스트 데이터

    public bool IsActive { get; set; }                          // 퀘스트 활성화 여부
    public MonsterQuestData MonsterQuest
    {
        get => _monsterQuest;
        private set => _monsterQuest = value;
    }

    private void Awake()
    {
        // 초기 데이터 저장
        _initMonsterQuestData = new MonsterQuestData(_monsterQuest);

        _initMonsterQuestData.PrintData();

        _monsterQuest.PrintData();
    }

    /// <summary>
    /// 현재 진행 상황을 증가하고 퀘스트를 업데이트하는 메서드
    /// </summary>
    public void IncreaseCurrent()
    {
        MonsterQuest.current++;

        QuestController.Instance.UpdateQuest();

        if (MonsterQuest.current >= MonsterQuest.goal)
        {
            MonsterQuest.current = MonsterQuest.goal;

            Debug.Log("퀘스트 완료");

            QuestController.Instance.CompleteQuest();
        }
    }

    public void InitializeQuestData()
    {
        _monsterQuest = _initMonsterQuestData;

        _initMonsterQuestData.PrintData();

        _monsterQuest.PrintData();
    }
}