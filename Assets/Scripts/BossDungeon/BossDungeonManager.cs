using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossDungeonManager : HappyTools.SingletonBehaviourFixed<BossDungeonManager>, ISceneContextBuildListener
{
    [SerializeField] int _maxKeyCount = 3;
    [SerializeField] string _dataGroupName = "BossDungeon1";
    BossKey[] _bossKeys; 
    void Awake()
    {
        base.Awake();
        PersistentDataManager.TryAddDataGroup(_dataGroupName);
    }
    public void OnSceneContextBuilt()
    {
        _bossKeys = FindObjectsByType<BossKey>(FindObjectsSortMode.InstanceID);
        foreach(var key in _bossKeys)
        {
            bool isDestroyed = PersistentDataManager.Get<bool>(_dataGroupName, "bossKey" + key.ID);
            if (isDestroyed)
                Destroy(key.gameObject);
        }
    }
    
    public void OnKeyObtained(BossKey key)
    {
        PersistentDataManager.Set<bool>(_dataGroupName, "bossKey" + key.ID, true);
    }
}
