using System;
using UnityEngine;

public class BossDungeonManager : HappyTools.SingletonBehaviourFixed<BossDungeonManager>
{
    [SerializeField] private string _dataGroupName = "BossDungeon";

    //key
    [SerializeField] private DialogueData _firstKeyDialogue;
    [SerializeField] private int _maxKeyCount = 3;
    public int MaxKeyCount => _maxKeyCount;         // Boss Key Slot의 개수와 일치해야 한다

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
    public int CurrentKeyCount
    {
        get
        {
            if (!PersistentDataManager.HasDataGroup(_dataGroupName))
                MakeDataGroup();

            return PersistentDataManager.Get<int>(_dataGroupName, "_bossKeyCountSaved");
        }
    }
    public bool IsAllKeysCollected => PersistentDataManager.Get<int>(_dataGroupName, "_bossKeyCountSaved") == _maxKeyCount;

    public void OnKeyObtained(BossKey key = null)
    {
        if (!PersistentDataManager.HasDataGroup(_dataGroupName))
            MakeDataGroup();
        PersistentDataManager.UpdateValue<int>(_dataGroupName, "_bossKeyCountSaved", x => Math.Clamp(x + 1, 0, _maxKeyCount));

        GameUIManager.AddBossKey();

        if (IsFirstKeyObtained == false)
        {
            IsFirstKeyObtained = true;
            DialogueController.Instance.StartDialogue(_firstKeyDialogue);
        }
    }

    public void OnOpenBossDoor()
    {
        PersistentDataManager.UpdateValue<int>(_dataGroupName, "_bossKeyCountSaved", x => 0);
    }

    public void OnDashObtainEventPlayed()
    {
        PersistentDataManager.UpdateValue<bool>(_dataGroupName, "_dashObtainEventSaved", x => true);
    }

    public bool DashObtainEventNotPlayed()
    {
        if (!PersistentDataManager.HasDataGroup(_dataGroupName))
            MakeDataGroup();

        if (!PersistentDataManager.Has<bool>(_dataGroupName, "_dashObtainEventSaved"))
        {
            return true;
        }

        return !PersistentDataManager.Get<bool>(_dataGroupName, "_dashObtainEventSaved");
    }

    public void MakeDataGroup()
    {
        PersistentDataManager.TryAddDataGroup(_dataGroupName);
    }
}
