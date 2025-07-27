using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    //배열 원소명 
    [HideInInspector]
    public string WaveIdx;

    /// <summary>
    /// 웨이브 정보와 웨이브별 몬스터들이 저장될 게임오브젝트의 구분을 위해 사용(실질적인 데이터 작업x)
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
            foreach (var monster in WaveMonsters)
            {
                GameObject monsterObj = monster.Monster;
                MonsterBehaviour monsterBehaviour = null;

                if(monsterObj != null)
                {
                    monsterBehaviour = monsterObj.GetComponentInChildren<MonsterBehaviour>();

                    if (!monsterBehaviour.IsDead)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

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
    [SerializeField] private float _timeOfEndOnceWaveCycle = 1.0f;
    [SerializeField] private List<string> _normalBgm;
    [SerializeField] private string _dungeon1WaveBgm = "Exploration1_Wave";
    [SerializeField] private string _dungeon2WaveBgm = "Exploration2_Wave";


    [Header("External Called GameObject"), Space(10)]

    [SerializeField] private CutscenePlayer _startWaveCutscene;
    [SerializeField] private CutscenePlayer _endWaveCutscene;

    private Coroutine _currentPlayingWaveCoroutine;

    public bool IsClearAllWaves
    {
        get { return _isClearAllWaves; }
        set
        {
            _isClearAllWaves = value;

            if (_identifier && _isClearAllWaves)
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

#if UNITY_EDITOR
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

        //WaveInfo데이터 추가된 경우
        CheckAddWaveInfoElement(monsterBundles);
        //WaveInfo데이터 삭제된 경우
        if (CheckRemoveWaveInfoElement(monsterBundles)) return;
        //waveInfo데이터 스위치 된 경우
        if (SwitchWaveInfosAndMonsterBundles()) return;

        //할당된 필드몬스터 몬스터번들로 부모 전환
        SetParentAssiginedFieldMonster();

        //삭제된 필드몬스터 몬스터번들로 부터 삭제
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
    /// 인스펙터 창의 WaveInfos원소의 순서가 바뀌면 호출되는함수, 해당함수가 호출되고 true를 리턴했을 때
    /// RemoveFieldMonsterFromWaveInfos()가 호출된다면 데이터가 의도치 않게 삭제될 수 있음
    /// </summary>
    /// <returns>true : WaveInfos원소가 교환이 일어난 경우, false : true가 아닌 경우</returns>
    private bool SwitchWaveInfosAndMonsterBundles()
    {
        int idx = 0;
        bool isSwitched = false;

        while (idx < _waveInfos.Count)
        {
            int monsterBundleIdx = idx + 1;
            string monsterBundleId = transform.GetChild(monsterBundleIdx).name.Split(' ')[1];

            //순서가 바뀐 idx를 찾았을 때
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
#endif

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _identifier = GetComponent<Identifier>();
        _normalBgm = new List<string>()
        {
            "Exploration1", "Exploration2"
        };

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

            if(!_isClearAllWaves)
            {
                _startWaveCutscene.GetComponent<PreserveState>().SaveState("_played", false);
                _startWaveCutscene.IsPlayed = false;
            }
        }
    }

    /* renewal에 사용
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if(activator.Type == ActivatorType.Player &&
            !_isClearAllWaves && _currentPlayingWaveCoroutine == null)
        {
            //StartWaveSystem();
        }
    }
    */

    public void StartWaveSystem()
    {
        _currentPlayingWaveCoroutine = StartCoroutine(WaveSystemLogic());
    }

    IEnumerator WaveSystemLogic()
    {
        StartOfWaveSystemLogic();

        bool isFirstOfSpawn = true;

        AudioSource bgmPlayer = GetPlayingNormalBgmPlayer();
        string dungeonWaveClipKey = Regex.IsMatch(SceneManager.GetActiveScene().name, @".*1-.*") ?
            _dungeon1WaveBgm : _dungeon2WaveBgm;
        Debug.Log(_dungeon2WaveBgm);
        if (bgmPlayer != null)
        {
            SoundManager.Instance.PlayCommonBGMFade(dungeonWaveClipKey, 3, null, bgmPlayer.time);
        }
        else
        {
            SoundManager.Instance.PlayCommonBGMFade(dungeonWaveClipKey, 3, null);
        }

        for (int i = 0; i < _waveInfos.Count; i++)
        {
            //해당 웨이브의 몬스터 스폰
            foreach (var monster in _waveInfos[i].WaveMonsters)
            {
                Transform monsterTransform = SpawnWaveMonster(i + 1, monster);

                if (monsterTransform != null &&
                    isFirstOfSpawn)
                {
                    SceneEffectManager.Instance.Camera.StartFollow(monsterTransform);
                    isFirstOfSpawn = false;
                }
            }

            while (true)
            {
                if (_waveInfos[i].IsClear)
                {
                    //한 웨이브 클리어시 행동
                    yield return new WaitForSeconds(_timeOfEndOnceWaveCycle);

                    break;
                }
                yield return null;
            }
        }

        EndOfWaveSystemLogic();
    }

    /// <summary>
    /// 웨이브 몬스터를 스폰하는 작업
    /// </summary>
    /// <param name="waveStage"></param>
    /// <param name="waveMonsterInfo"></param>
    /// <returns> 웨이브 몬스터가 스폰될 지점(Position) </returns>
    private Transform SpawnWaveMonster(int waveStage, WaveSpawnMonsterInfo waveMonsterInfo)
    {
        switch (waveMonsterInfo.InstanceType)
        {
            case WaveSpawnMonsterInfo.MonsterInstanceType.Prefab:
                {
                    GameObject spawnedMonster = null;
                    for (int i = 0; i < waveMonsterInfo.Count; i++)
                    {
                        spawnedMonster = Instantiate(waveMonsterInfo.Monster, transform.GetChild(waveStage - 1));
                        spawnedMonster.transform.position = waveMonsterInfo.SpawnPosition;
                    }

                    return spawnedMonster.transform;
                }
            case WaveSpawnMonsterInfo.MonsterInstanceType.Field:
                {
                    waveMonsterInfo.Monster?.transform.GetChild(0).gameObject.SetActive(true);

                    MonsterBehaviour monsterBehaviour = waveMonsterInfo.Monster?.GetComponentInChildren<MonsterBehaviour>();
                    if (monsterBehaviour != null)
                    {
                        monsterBehaviour.GetComponent<MaterialController>().InitMaterialForRespawn();
                        monsterBehaviour.RespawnProcess();
                    }

                    return waveMonsterInfo.Monster.transform;
                }
        }

        return null;
    }

    private void StartOfWaveSystemLogic()
    {
        /* renewal에 사용
        //_enterWaveDoor.CloseDoor(false);
        //_exitWaveDoor.CloseDoor(false);
        */
    }

    private void EndOfWaveSystemLogic()
    {
        string dungeonWaveClipKey = SceneManager.GetActiveScene().name.Contains("1-") ?
            _dungeon1WaveBgm : _dungeon2WaveBgm;

        AudioSource bgmPlayer = SoundManager.Instance.GetBgmPlayer(dungeonWaveClipKey);
        SoundManager.Instance.StopBGMFade(3, bgmPlayer);

        _currentPlayingWaveCoroutine = null;
        IsClearAllWaves = true;

        _endWaveCutscene?.Play();

        /* renewal에 사용
        //_enterWaveDoor.OpenDoor(false);
        //_exitWaveDoor.OpenDoor(false);
        */
    }

    //wave 시스템이 있는 일반적인 씬에서 재생되는 bgm 플레이어 리턴
    private AudioSource GetPlayingNormalBgmPlayer()
    {
        for(int i = 0; i < _normalBgm.Count; i++)
        {
            string normalBgmKey = _normalBgm[i];
            AudioSource bgmPlayer = SoundManager.Instance.GetBgmPlayer(normalBgmKey);

            if (bgmPlayer != null)
            {
                return bgmPlayer;
            }
        }

        return null;
    }
}
