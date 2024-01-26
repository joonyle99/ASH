using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossDungeonManager : HappyTools.SingletonBehaviourFixed<BossDungeonManager>
{
    [SerializeField] int _maxKeyCount = 3;
    [SerializeField] string _dataGroupName = "BossDungeon1";
    [SerializeField] DialogueData _firstKeyDialogue;

    public bool AllKeysCollected => PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount") == _maxKeyCount;
    BossKey[] _bossKeys; 
    new void Awake()
    {
        base.Awake();
        PersistentDataManager.TryAddDataGroup(_dataGroupName);
    }
    public void OnKeyObtained(BossKey key)
    {
        PersistentDataManager.UpdateValue<int>(_dataGroupName, "bossKeyCount", x=>x+1);
        if (PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount") == 1)
        {
            DialogueController.Instance.StartDialogue(_firstKeyDialogue);
        }
    }
}
