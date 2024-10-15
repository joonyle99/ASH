using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class WaveSpawnMonsterInfo
{
    public enum MonsterInstanceType
    {
        Prefab,
        Field,
    }

    [SerializeField]
    public MonsterInstanceType InstanceType = MonsterInstanceType.Prefab;

    [SerializeField, HideInInspector]
    public GameObject Monster;

    [SerializeField, HideInInspector]
    public int Count;

    [SerializeField, HideInInspector]
    public Vector3 SpawnPosition;
}

[Serializable]
public class WaveInfo
{
    #region Auto Setting Variable
    //�迭 ���Ҹ� 
    [HideInInspector]
    public string WaveIdx;

    /// <summary>
    /// ���̺� ������ ���̺꺰 ���͵��� ����� ���ӿ�����Ʈ�� ������ ���� ���(�������� ������ �۾�x)
    /// </summary>
    /// 
    [SerializeField, HideInInspector]
    public string WaveInfoID;

    #endregion

    [SerializeField, HideInInspector]
    public bool IsClear
    {
        get
        {
            foreach(var monster in WaveMonsters)
            {
                if(!monster.Monster.GetComponentInChildren<MonsterBehaviour>().IsDead)
                {
                    return false;
                }
            }

            return true;
        }
    }

    [SerializeField]
    public WaveSpawnMonsterInfo[] WaveMonsters;

    public void Init(string waveIdx, string waveInfoId)
    {
        WaveInfoID = waveInfoId;
        WaveIdx = waveIdx;
    }

    public bool HasMonster(GameObject targetMonster)
    {
        foreach (var monsterInfo in WaveMonsters)
        {
            if (monsterInfo.InstanceType == WaveSpawnMonsterInfo.MonsterInstanceType.Prefab) continue;

            if (monsterInfo.Monster != null && monsterInfo.Monster == targetMonster) return true;
        }

        return false;
    }
}

[RequireComponent(typeof(Identifier))]
public class WaveSystemController : MonoBehaviour, ITriggerListener, ISceneContextBuildListener
{
    [Header("Prefabs"), Space(10)]

    [SerializeField]
    private GameObject _waveMonstersPref;

    [Header("Variable"), Space(10)]

    [SerializeField] private List<WaveInfo> _waveInfos;
    [SerializeField] private bool _isClearAllWaves = false;

    Coroutine _currentPlayingWaveCoroutine;

    public bool IsClearAllWaves
    {
        get { return _isClearAllWaves; }
        set 
        { 
            _isClearAllWaves = value;

            if(_identifier && _isClearAllWaves)
            {
                _identifier.SaveState("_isClearAllWaves", true);
                _identifier.SaveState("_isClearAllWavesSaved", true);
            }
        }
    }

    private Identifier _identifier;

    [Header("Doors")]

    [SerializeField] private WaveDoor _enterWaveDoor;
    [SerializeField] private WaveDoor _exitWaveDoor;

    #region Editor Property Function
    private void OnValidate()
    {
        PostEditChangeProperty();
    }

    private void PostEditChangeProperty()
    {
        List<GameObject> monsterBundles = new();

        for (int i = 1; i < transform.childCount; i++)
        {
            monsterBundles.Add(transform.GetChild(i).gameObject);
        }

        //WaveInfo������ �߰��� ���
        CheckAddWaveInfoElement(monsterBundles);
        //WaveInfo������ ������ ���
        if (CheckRemoveWaveInfoElement(monsterBundles)) return;
        //waveInfo������ ����ġ �� ���
        if (SwitchWaveInfosAndMonsterBundles()) return;

        //�Ҵ�� �ʵ���� ���͹���� �θ� ��ȯ
        SetParentAssiginedFieldMonster();

        //������ �ʵ���� ���͹���� ���� ����
        RemoveFieldMonsterFromWaveInfos(monsterBundles);
    }

    private void CheckAddWaveInfoElement(List<GameObject> monsterBundles)
    {
        if (monsterBundles.Count < _waveInfos.Count)
        {
            GameObject monsters = Instantiate(_waveMonstersPref, transform);
            _waveInfos[_waveInfos.Count - 1].Init((_waveInfos.Count - 1).ToString(), GetRandomUniqueId());
            monsters.gameObject.name = "Wave(" + _waveInfos.Count + ")MonsterBundle " + _waveInfos[_waveInfos.Count - 1].WaveInfoID;
        }
    }
    private bool CheckRemoveWaveInfoElement(List<GameObject> monsterBundles)
    {
        if (monsterBundles.Count > _waveInfos.Count)
        {
            foreach (var monsterBundle in monsterBundles)
            {
                string monsterBundleId = monsterBundle.gameObject.name.Split(' ')[1];
                bool isMatchedId = false;

                foreach (var waveInfo in _waveInfos)
                {
                    if (waveInfo.WaveInfoID == monsterBundleId)
                    {
                        isMatchedId = true;
                        break;
                    }
                }

                if (!isMatchedId)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(monsterBundle);
                    };

                    return true;
                }
            }
        }

        return false;
    }
    /// <summary>
    /// �ν����� â�� WaveInfos������ ������ �ٲ�� ȣ��Ǵ��Լ�, �ش��Լ��� ȣ��ǰ� true�� �������� ��
    /// RemoveFieldMonsterFromWaveInfos()�� ȣ��ȴٸ� �����Ͱ� �ǵ�ġ �ʰ� ������ �� ����
    /// </summary>
    /// <returns>true : WaveInfos���Ұ� ��ȯ�� �Ͼ ���, false : true�� �ƴ� ���</returns>
    private bool SwitchWaveInfosAndMonsterBundles()
    {
        int idx = 0;
        bool isSwitched = false;

        while (idx < _waveInfos.Count)
        {
            int monsterBundleIdx = idx + 1;
            string monsterBundleId = transform.GetChild(monsterBundleIdx).name.Split(' ')[1];

            //������ �ٲ� idx�� ã���� ��
            if (_waveInfos[idx].WaveInfoID != monsterBundleId)
            {
                monsterBundleIdx = 1;
                while (monsterBundleIdx < _waveInfos.Count)
                {
                    monsterBundleId = transform.GetChild(monsterBundleIdx).name.Split(' ')[1];
                    if (_waveInfos[idx].WaveInfoID == monsterBundleId)
                    {
                        transform.GetChild(monsterBundleIdx).SetSiblingIndex(idx);

                        isSwitched = true;
                    }

                    monsterBundleIdx++;
                }
            }

            idx++;
        }

        return isSwitched;
    }

    private void SetParentAssiginedFieldMonster()
    {
        for (int i = 0; i < _waveInfos.Count; i++)
        {
            WaveSpawnMonsterInfo[] monsterInfo = _waveInfos[i].WaveMonsters;

            foreach (var monster in monsterInfo)
            {
                if (monster.Monster == null) continue;

                bool hasCorrectParent = monster.Monster.transform.parent != null &&
                                        monster.Monster.transform.parent.tag == "WaveMonsterBundle";
                if (monster.InstanceType == WaveSpawnMonsterInfo.MonsterInstanceType.Field &&
                    !hasCorrectParent)
                {
                    monster.Monster.transform.SetParent(transform.GetChild(i + 1), true);
                }
            }
        }
    }

    private void RemoveFieldMonsterFromWaveInfos(List<GameObject> monsterBundles)
    {
        for (int i = 0; i < monsterBundles.Count; i++)
        {
            for (int j = 0; j < monsterBundles[i].transform.childCount; j++)
            {
                GameObject bundledMonster = monsterBundles[i].transform.GetChild(j).gameObject;

                if (!_waveInfos[i].HasMonster(bundledMonster))
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(bundledMonster);
                    };
                }
            }
        }
    }

    private string GetRandomUniqueId()
    {
        string str = UnityEngine.Random.Range(0, 9999).ToString();

        while (true)
        {
            for (int i = 0; i < _waveInfos.Count; i++)
            {
                if (_waveInfos[i].WaveInfoID == str)
                {
                    str = UnityEngine.Random.Range(0, 9999).ToString();
                    break;
                }

                if (i == _waveInfos.Count - 1)
                {
                    return str;
                }
            }
        }
    }
    #endregion

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _identifier = GetComponent<Identifier>();

        foreach (var waveInfo in _waveInfos)
        {
            foreach (var monster in waveInfo.WaveMonsters)
            {
                if (monster == null) continue;

                if (monster.InstanceType == WaveSpawnMonsterInfo.MonsterInstanceType.Field)
                {
                    monster.Monster?.GetComponentInChildren<MonsterBehaviour>().gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnSceneContextBuilt()
    {
        if (_identifier)
        {
            if (SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                _isClearAllWaves = _identifier.LoadState<bool>("_isClearAllWavesSaved", false);
            }
            else
            {
                _isClearAllWaves = _identifier.LoadState<bool>("_isClearAllWaves", false);
            }
        }
    }

    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if(activator.Type == ActivatorType.Player &&
            !_isClearAllWaves && _currentPlayingWaveCoroutine == null)
        {
            StartWaveSystem();
        }
    }

    public void StartWaveSystem()
    {
        _currentPlayingWaveCoroutine = StartCoroutine(WaveSystemLogic());
    }

    IEnumerator WaveSystemLogic()
    {
        StartOfWaveSystemLogic();

        for (int i = 0; i < _waveInfos.Count; i++)
        {
            //�ش� ���̺��� ���� ����
            foreach (var monster in _waveInfos[i].WaveMonsters)
            {
                SpawnWaveMonster(i + 1, monster);
            }

            while (true)
            {
                if (_waveInfos[i].IsClear)
                {
                    //�� ���̺� Ŭ����� �ൿ

                    break;
                }
                yield return null;
            }
        }

        EndOfWaveSystemLogic();
    }

    private void SpawnWaveMonster(int waveStage, WaveSpawnMonsterInfo waveMonsterInfo)
    {
        switch (waveMonsterInfo.InstanceType)
        {
            case WaveSpawnMonsterInfo.MonsterInstanceType.Prefab:
                {
                    for (int i = 0; i < waveMonsterInfo.Count; i++)
                    {
                        GameObject spawnedMonster = Instantiate(waveMonsterInfo.Monster, transform.GetChild(waveStage - 1));
                        spawnedMonster.transform.position = waveMonsterInfo.SpawnPosition;
                    }
                    break;
                }
            case WaveSpawnMonsterInfo.MonsterInstanceType.Field:
                {
                    waveMonsterInfo.Monster?.transform.GetChild(0).gameObject.SetActive(true);
                    waveMonsterInfo.Monster?.GetComponentInChildren<MonsterBehaviour>().RespawnProcess();

                    break;
                }
        }
    }

    private void StartOfWaveSystemLogic()
    {
        _enterWaveDoor.CloseDoor(false);
        _exitWaveDoor.CloseDoor(false);
    }

    private void EndOfWaveSystemLogic()
    {
        _currentPlayingWaveCoroutine = null;
        IsClearAllWaves = true;

        _enterWaveDoor.OpenDoor(false);
        _exitWaveDoor.OpenDoor(false);
    }
}
