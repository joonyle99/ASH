using UnityEngine;

public class BossDungeonManager : HappyTools.SingletonBehaviourFixed<BossDungeonManager>
{
    [SerializeField] private int _maxKeyCount = 3;
    [SerializeField] private string _dataGroupName = "BossDungeon";
    [SerializeField] private DialogueData _firstKeyDialogue;

    [SerializeField] private bool _isFirstKeyObtained = false;
    private static bool s_isFirstKeyObtained = false;
    public bool IsFirstKeyObtained
    {
        get => s_isFirstKeyObtained;
        set
        {
            if (!s_isFirstKeyObtained)
            {
                s_isFirstKeyObtained = true;
                _isFirstKeyObtained = true;
            }
        }
    }

    public int CurrentKeyCount => PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount");
    public bool IsAllKeysCollected => PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount") == _maxKeyCount;

    protected override void Awake()
    {
        base.Awake();

        MakeDataGroup();
    }

    public void OnKeyObtained(BossKey key)
    {
        PersistentDataManager.UpdateValue<int>(_dataGroupName, "bossKeyCount", x => x + 1);

        GameUIManager.AddBossKey();

        if (IsFirstKeyObtained == false)
        {
            IsFirstKeyObtained = true;
            DialogueController.Instance.StartDialogue(_firstKeyDialogue);
        }
    }
    public void OnOpenBossDoor()
    {
        PersistentDataManager.UpdateValue<int>(_dataGroupName, "bossKeyCount", x => 0);
    }

    public void MakeDataGroup()
    {
        PersistentDataManager.TryAddDataGroup(_dataGroupName);
    }
}
