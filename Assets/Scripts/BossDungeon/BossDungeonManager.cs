using UnityEngine;

public class BossDungeonManager : HappyTools.SingletonBehaviourFixed<BossDungeonManager>
{
    [SerializeField] private int _maxKeyCount = 3;
    [SerializeField] private string _dataGroupName = "BossDungeon1";
    [SerializeField] private DialogueData _firstKeyDialogue;
    [SerializeField] private SoundClipData _bossDungeonBGM;

    public int CurrentKeyCount => PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount");
    public bool AllKeysCollected => PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount") == _maxKeyCount;

    private void Start()
    {
        PersistentDataManager.TryAddDataGroup(_dataGroupName);
    }

    public void OnKeyObtained(BossKey key)
    {
        PersistentDataManager.UpdateValue<int>(_dataGroupName, "bossKeyCount", x => x + 1);

        GameUIManager.AddBossKey();

        if (PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount") == 1)
        {
            DialogueController.Instance.StartDialogue(_firstKeyDialogue);
        }
    }

    public void PlayBossDungeon1Bgm()
    {
        SoundManager.Instance.PlayBGM(_bossDungeonBGM.Clip, _bossDungeonBGM.Volume);
    }
}
