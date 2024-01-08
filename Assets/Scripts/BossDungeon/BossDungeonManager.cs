using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossDungeonManager : HappyTools.SingletonBehaviourFixed<BossDungeonManager>
{
    [SerializeField] int _maxKeyCount = 3;
    [SerializeField] string _dataGroupName = "BossDungeon1";

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
    }
}
