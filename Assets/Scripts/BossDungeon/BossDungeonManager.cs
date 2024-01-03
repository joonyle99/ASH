using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossDungeonManager : HappyTools.SingletonBehaviourFixed<BossDungeonManager>, ISceneContextBuildListener
{
    [SerializeField] int _currentKeyCount = 0;
    [SerializeField] int _maxKeyCount = 3;
    [SerializeField] string _dataGroupName = "BossDungeon1";

    public string DataGroupName => _dataGroupName;
    public string DoorDataID => "openDoor";
    BossKey[] _bossKeys; 
    new void Awake()
    {
        base.Awake();
        PersistentDataManager.TryAddDataGroup(_dataGroupName);
        for(int i=0; i<_maxKeyCount; i++)
        {
            if (PersistentDataManager.Get<bool>(_dataGroupName, "bossKey" + i))
            {
                _currentKeyCount++;
                if (_currentKeyCount == _maxKeyCount)
                    PersistentDataManager.Set<bool>(_dataGroupName, DoorDataID, true);
            }
        }
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
        _currentKeyCount++;
        if (_currentKeyCount == _maxKeyCount)
            PersistentDataManager.Set<bool>(_dataGroupName, DoorDataID, true);
    }
}
