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
    [SerializeField] SoundClipData _bossDungeonBGM;

    public int CurrentKeyCount => PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount");

    public bool AllKeysCollected => PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount") == _maxKeyCount;
    BossKey[] _bossKeys; 
    new void Awake()
    {
        base.Awake();
        if(PersistentDataManager.TryAddDataGroup(_dataGroupName))
        {
            SoundManager.Instance.PlayBGM(_bossDungeonBGM.Clip, _bossDungeonBGM.Volume);
        }
    }
    public void OnKeyObtained(BossKey key)
    {
        GameUIManager.AddBossKey();
        PersistentDataManager.UpdateValue<int>(_dataGroupName, "bossKeyCount", x=>x+1);
        if (PersistentDataManager.Get<int>(_dataGroupName, "bossKeyCount") == 1)
        {
            DialogueController.Instance.StartDialogue(_firstKeyDialogue);
        }
    }
}
