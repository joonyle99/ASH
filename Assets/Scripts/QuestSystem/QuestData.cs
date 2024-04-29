using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ������ ����Ʈ�� ���� ������ ��� �ִ� Ŭ����
/// ����ν�� �Ϲݸ��� ��� ����Ʈ���� �����Ѵ�
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

    [SerializeField] private MonsterQuestData _monsterQuest;    // ���� ��� ����Ʈ ������
    private MonsterQuestData _initMonsterQuestData;              // �ʱ� ���� ��� ����Ʈ ������

    public bool IsActive { get; set; }                          // ����Ʈ Ȱ��ȭ ����
    public MonsterQuestData MonsterQuest
    {
        get => _monsterQuest;
        private set => _monsterQuest = value;
    }

    private void Awake()
    {
        // �ʱ� ������ ����
        _initMonsterQuestData = new MonsterQuestData(_monsterQuest);

        _initMonsterQuestData.PrintData();

        _monsterQuest.PrintData();
    }

    /// <summary>
    /// ���� ���� ��Ȳ�� �����ϰ� ����Ʈ�� ������Ʈ�ϴ� �޼���
    /// </summary>
    public void IncreaseCurrent()
    {
        MonsterQuest.current++;

        QuestController.Instance.UpdateQuest();

        if (MonsterQuest.current >= MonsterQuest.goal)
        {
            MonsterQuest.current = MonsterQuest.goal;

            Debug.Log("����Ʈ �Ϸ�");

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