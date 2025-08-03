using System;
using System.Collections.Generic;
using UnityEngine;
using DataGroup = System.Collections.Generic.Dictionary<string, object>;

using static JsonPersistentData;
using UnityEditor;
using JetBrains.Annotations;
using System.Linq;

/// <summary> 지속성 있는 데이터 </summary>
public class PersistentData
{
    public string SceneName = "";
    public string PassageName = "";

    public Dictionary<string, DataGroup> DataGroups = new();      // 플레이 데이터를 저장하는데 사용
    public DataGroup GlobalDataGroup = new();                     // 영구적 데이터를 저장하는데 사용

    public PersistentData CopyPersistentData(PersistentData data)
    {
        PersistentData copyData = new PersistentData();

        copyData.SceneName = data.SceneName;
        copyData.PassageName = data.PassageName;

        foreach (var pair in data.DataGroups)
        {
            DataGroup copyDataGroup = new DataGroup();

            foreach (var innerPair in pair.Value)
            {
                copyDataGroup.Add(innerPair.Key, innerPair.Value);
            }

            copyData.DataGroups.Add(pair.Key, copyDataGroup);
        }

        foreach (var pair in data.GlobalDataGroup)
        {
            copyData.GlobalDataGroup.Add(pair.Key, pair.Value);
        }

        return copyData;
    }

    public static JsonPersistentData ToJsonFormatClassObject(PersistentData persistentData)
    {
        if (persistentData == null) return null;

        JsonPersistentData jsonPersistentData = new JsonPersistentData();

        jsonPersistentData.SceneName = persistentData.SceneName;

        jsonPersistentData.PassageName = persistentData.PassageName;

        JsonDataArray<string, JsonDataArray<string, SerializableObjectType>> jsonDataGroups = new();
        foreach (var dataGroups in persistentData.DataGroups)
        {
            JsonDataArray<string, SerializableObjectType> jsonDataGroup = new();

            foreach (var dataGroup in dataGroups.Value)
            {
                //저장관련된 큰 문제 생기면 아래 코드 삭제 요망
                //if (!dataGroup.Key.Substring(dataGroup.Key.Length - 5, 5).Equals("Saved"))
                //    continue;

                jsonDataGroup.Add(dataGroup.Key,
                    new SerializableObjectType() { Object = dataGroup.Value });
            }

            jsonDataGroups.Add(dataGroups.Key, jsonDataGroup);
        }
        jsonPersistentData._jsonDataGroups = jsonDataGroups;

        JsonDataArray<string, SerializableObjectType> jsonGlobalDataGroup = new();
        foreach (var dataGroup in persistentData.GlobalDataGroup)
        {
            jsonGlobalDataGroup.Add(dataGroup.Key,
                new SerializableObjectType() { Object = dataGroup.Value });
        }
        jsonPersistentData._jsonGlobalDataGroup = jsonGlobalDataGroup;

        return jsonPersistentData;
    }

    public void PrintData()
    {
        Debug.Log("Scene Name : " + SceneName);
        Debug.Log("Passage Name : " + PassageName);
        Debug.Log("=============dataGroups============");
        foreach (var pair in DataGroups)
        {
            Debug.Log("Group : " + pair.Key);
            foreach (var innerPair in pair.Value)
            {
                Debug.Log("Key : " + innerPair.Key + " Value : " + innerPair.Value);
            }
        }

        Debug.Log("=============GlobalDataGroups============");
        foreach (var pair in GlobalDataGroup)
        {
            Debug.Log("Key : " + pair.Key + " Value : " + pair.Value);
        }
    }
}

/// <summary> 지속성있는 데이터를 관리하는 클래스 </summary>
public class PersistentDataManager : HappyTools.SingletonBehaviourFixed<PersistentDataManager>
{
    private PersistentData _persistentData = new();
    public PersistentData PersistentData => Instance._persistentData;

    private JsonPersistentData _savedPersistentData = new();
    public JsonPersistentData SavedPersistentData => Instance._savedPersistentData;

    [SerializeField] private SkillOrderData _skillOrderData;
    public static SkillOrderData SkillOrderData => Instance._skillOrderData;

    private int _cheatSkillId = 0;

    //gui var
    Vector2 scrollPos = Vector2.zero;
    string t = "";

    private void Update()
    {
        // CHEAT: F7 키를 누르면 빛 스킬 획득
        if (Input.GetKeyDown(KeyCode.F7) && GameSceneManager.Instance.CheatMode == true)
        {
            ObtainSkill_Light();
        }

        // CHEAT: F8 키를 누르면 더블 점프, 대쉬 획득
        if (Input.GetKeyDown(KeyCode.F8) && GameSceneManager.Instance.CheatMode == true)
        {
            ObtainSkill_Moving();
        }

#if UNITY_EDITOR
        /*
        // 데이터 그룹 출력
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PrintDataGroup();
        }
        // 글로벌 데이터 그룹 출력
        if (Input.GetKeyDown(KeyCode.E))
        {
            PrintGlobalDataGroup();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("=============Current Data============");
            PersistentData.PrintData();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("=============Save Data============");
            SavedPersistentData.PrintData();
        }
        */
#endif
    }

    #region group data
    public static bool TryAddDataGroup(string groupName)
    {
        if (Instance == null)
            return false;

        // 이미 존재하는 데이터 그룹이라면 추가하지 않는다
        if (HasDataGroup(groupName))
            return false;

        // 새로운 데이터 그룹을 생성한다
        Instance._persistentData.DataGroups[groupName] = new DataGroup();

        return true;
    }
    public static void RemoveDataGroup(string groupName)
    {
        if (Instance == null)
            return;

        Instance._persistentData.DataGroups.Remove(groupName);
    }
    public static bool HasDataGroup(string groupName)
    {
        if (Instance == null)
            return false;

        return Instance._persistentData.DataGroups.ContainsKey(groupName);
    }
    public static void Reset(string groupName)
    {
        if (Instance == null)
            return;

        Instance._persistentData.DataGroups[groupName].Clear();
    }
    public static void UpdateValue<T>(string groupName, string key, Func<T, T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;

        Instance._persistentData.DataGroups[groupName][key] = updateFunction(Get<T>(groupName, key));
    }
    public static void UpdateRef<T>(string groupName, string key, Action<T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;

        updateFunction(Get<T>(groupName, key));
    }
    public static void Set<T>(string groupName, string key, T value) where T : new()
    {
        if (Instance == null)
            return;

        if (Instance._persistentData.DataGroups.ContainsKey(groupName))
        {
            Instance._persistentData.DataGroups[groupName][key] = value;
        }
    }
    public static bool Has<T>(string groupName, string key) where T : new()
    {
        if (Instance == null)
            return false;

        return Instance._persistentData.DataGroups.ContainsKey(groupName) && Instance._persistentData.DataGroups[groupName].ContainsKey(key);
    }
    public static T Get<T>(string groupName, string key) where T : new()
    {
        if (Instance == null)
            return new T();

        if (Instance._persistentData.DataGroups[groupName].TryGetValue(key, out object value))
        {
            return (T)value;
        }
        else
        {
            Instance._persistentData.DataGroups[groupName][key] = new T();
            return (T)Instance._persistentData.DataGroups[groupName][key];
        }
    }
    #endregion

    #region global data
    public static void ResetByGlobal()
    {
        if (Instance == null)
            return;

        Instance._persistentData.GlobalDataGroup.Clear();
    }
    public static void UpdateValueByGlobal<T>(string key, Func<T, T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;

        Instance._persistentData.GlobalDataGroup[key] = updateFunction(GetByGlobal<T>(key));
    }
    public static void UpdateRefByGlobal<T>(string key, Action<T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;

        updateFunction(GetByGlobal<T>(key));
    }
    public static void SetByGlobal<T>(string key, T value) where T : new()
    {
        if (Instance == null)
            return;

        // 중복된 key에 새로운 value가 들어오면 덮어쓴다
        Instance._persistentData.GlobalDataGroup[key] = value;
    }
    public static bool HasByGlobal<T>(string key) where T : new()
    {
        if (Instance == null)
            return false;

        return Instance._persistentData.GlobalDataGroup.ContainsKey(key);
    }
    public static T GetByGlobal<T>(string key) where T : new()
    {
        if (Instance == null)
            return new T();

        if (Instance._persistentData.GlobalDataGroup.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        else
        {
            Instance._persistentData.GlobalDataGroup[key] = new T();
            return (T)Instance._persistentData.GlobalDataGroup[key];
        }
    }
    #endregion

    public static void PrintDataGroup()
    {
        if (Instance == null) return;

        string logMessage = "";

        foreach (var dataGroup in Instance._persistentData.DataGroups)
        {
            logMessage += $"Data Group Name: <color=yellow><b>[[ {dataGroup.Key} ]]</b></color>\n";
            logMessage += "\n";

            foreach (var data in dataGroup.Value)
            {
                logMessage += $"Key ====> <color=orange><b>{data.Key}</b></color>";
                logMessage += "\n\n";
                //logMessage += $"Value ====> {data.Value}";
                //logMessage += "\n\n";
            }

            logMessage += "==================================================\n\n";
        }

        Debug.Log(logMessage);
    }
    public static void PrintGlobalDataGroup()
    {
        if (Instance == null) return;

        string logMessage = "";

        foreach (var dataGroup in Instance._persistentData.GlobalDataGroup)
        {
            logMessage += $"Key ====> <color=orange><b>{dataGroup.Key}</b></color>";
            logMessage += "\n";
            logMessage += $"Value ====> <b>{dataGroup.Value}</b>";
            logMessage += "\n\n";
        }

        Debug.Log(logMessage);
    }

    /**
     * <summary>
     * 최신정보를 저장하고 있는 _persistentData를 가져와
     * _savedPersistentData에 저장
     * 
     * ※_savedPersistentData는 사망(혹은 불러오기)시 로드될 데이터
     * </summary>
     */
    public static void CopyPDataToSavedPData(string passageName)
    {
        Instance._savedPersistentData = PersistentData.ToJsonFormatClassObject(Instance._persistentData);

        Instance._savedPersistentData.SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Instance._savedPersistentData.PassageName = passageName;
    }

    /**
     * <summary>
     * 레벨 로드시 오브젝트가 참조하는 _persistentData를 
     * _savedPersistentData로 대체하여 저장된 데이터가 불러옴
     * </summary>
     */
    public static void ReplacePDataToSavedPData()
    {
        Instance._persistentData = ToNormalFormatClassObject(Instance._savedPersistentData);
    }

    /**
     * <summary>
     * 저장된 데이터를 불러오는 기능
     * 저장된 게임 데이터를 불러오고 싶으면 해당 함수 사용
     * 중간에 로직이 실패한다면 false 리턴
     * </summary>
     */
    public static bool LoadToSavedData(SceneChangeType sceneChangeType = SceneChangeType.Loading)
    {
        JsonDataManager.JsonLoad();
        Instance._savedPersistentData = JsonDataManager.GetObjectInGlobalSaveData<JsonPersistentData>("PersistentData");

        if (Instance._savedPersistentData == null)
        {
            //저장된 데이터가 없는 경우
            Debug.Log("Have not saved data");
            return false;
        }

        SceneChangeManager.Instance.SceneChangeType = sceneChangeType;

        ///---------씬 파괴 전 수행되어야 하는 것----------
        if (SceneContext.Current.Player && SceneContext.Current.Player.CurrentStateIs<DieState>())
        {
            Coroutine playerDieEnterCoroutine = ((DieState)SceneContext.Current.Player.CurrentState).DieEnterCoroutine;
            if (playerDieEnterCoroutine != null)
            {
                Instance.StopCoroutine(playerDieEnterCoroutine);
            }
        }

        DialogueController.Instance.ShutdownDialogue();

        SceneEffectManager.StopPlayingCutscene();

        ///----------------------------------------------

        ReplacePDataToSavedPData();
        //Instance._persistentData.DataGroups.RemoveNonSavedPersistentData();

        MonsterRespawnManager.Instance.StopRespawnCoroutine();

        string sceneName = Instance.PersistentData.SceneName;
        string passageName = Instance.PersistentData.PassageName;
        if (sceneName == "" || passageName == "")
        {
            Debug.LogWarning("Not Saved Scene or PassageData Load");
            return false;
        }

        SceneChangeManager.Instance.ChangeToPlayableScene(sceneName, passageName);

        return true;
    }

    public static void RemoveNonSavedPersistentData()
    {
        Dictionary<string, DataGroup> newGroup = new Dictionary<string, DataGroup>();

        foreach(var dataSet in Instance._persistentData.DataGroups)
        {
            DataGroup newData = new DataGroup();

            foreach(var data in dataSet.Value)
            {
                if(data.Key.Contains("Saved"))
                {
                    newData.Add(data.Key, data.Value);
                }
            }

            newGroup.Add(dataSet.Key, newData);
        }

        Instance._persistentData.DataGroups = newGroup;
    }

    // 프롤로그 화면에서 호출되도록 하기
    public static void ClearPersistentData()
    {
        if (Instance.PersistentData == null)
        {
            return;
        }

        Instance.PersistentData.SceneName = "";
        Instance.PersistentData.PassageName = "";
        Instance.PersistentData.DataGroups.Clear();
        Instance.PersistentData.GlobalDataGroup.Clear();
    }
    public static void ClearSavedPersistentData()
    {
        if (Instance.SavedPersistentData == null)
        {
            return;
        }

        Instance.SavedPersistentData.SceneName = "";
        Instance.SavedPersistentData.PassageName = "";
        Instance.SavedPersistentData._jsonDataGroups?.data?.Clear();
        Instance.SavedPersistentData._jsonGlobalDataGroup?.data?.Clear();
    }

    /*
    private void OnGUI()
    {
        GUIStyle _guiStyle = new();
        _guiStyle.normal.textColor = Color.green;

        GUILayout.BeginArea(new Rect((float)Screen.width - 200, 150, 200, 320));
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(200), GUILayout.Height(200));
        GUILayout.Label(t, _guiStyle);

        GUILayout.EndScrollView();

        if (GUILayout.Button("View Json DataGroup", GUILayout.Width(200), GUILayout.Height(20)))
        {
            for (int i = 0; _savedPersistentData._jsonDataGroups != null &&
                _savedPersistentData._jsonDataGroups.data != null &&
                i < _savedPersistentData._jsonDataGroups.data.Count; i++)
            {
                t += "group : " + _savedPersistentData._jsonDataGroups.data[i].Key + "\n";
                t += "--------------------\n";
                for (int j = 0; j < _savedPersistentData._jsonDataGroups.data[i].Value.data.Count; j++)
                {
                    t += "key : " + _savedPersistentData._jsonDataGroups.data[i].Value.data[j].Key + "\n";
                    t += "value : " + _savedPersistentData._jsonDataGroups.data[i].Value.data[j].Value.ObjectSerialized + "\n";
                }
            }
        }

        if (GUILayout.Button("View Json GlobalDataGroup", GUILayout.Width(200), GUILayout.Height(20)))
        {
            t += "SavedSceneName : " + _savedPersistentData.SceneName + "\n";
            t += "PassageName : " + _savedPersistentData.PassageName + "\n";

            for (int i = 0; _savedPersistentData._jsonGlobalDataGroup != null &&
                _savedPersistentData._jsonGlobalDataGroup.data != null &&
                i < _savedPersistentData._jsonGlobalDataGroup.data.Count; i++)
            {
                t += "key : " + _savedPersistentData._jsonGlobalDataGroup.data[i].Key + "\n";
                t += "value : " + _savedPersistentData._jsonGlobalDataGroup.data[i].Value.ObjectSerialized + "\n";
            }
        }

        if (GUILayout.Button("View DataGroup", GUILayout.Width(200), GUILayout.Height(20)))
        {
            foreach (var pair in Instance.PersistentData.DataGroups)
            {
                t += "group : " + pair.Key + "\n";
                t += "--------------------\n";
                var sortedDict = pair.Value.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
                
                foreach (var innerPair in sortedDict)
                {
                    t += "key : " + innerPair.Key + "\n";
                    t += "value : " + innerPair.Value + "\n";
                }
            }
            t += "\n\n\n";
        }

        if (GUILayout.Button("View GlobalDataGroup", GUILayout.Width(200), GUILayout.Height(20)))
        {
            var sortedDict = Instance.PersistentData.GlobalDataGroup.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var pair in sortedDict)
            {
                t += "key : " + pair.Key + "\n";
                t += "value : " + pair.Value + "\n";
            }
            t += "\n\n\n";
        }

        if (GUILayout.Button("Clear", GUILayout.Width(200), GUILayout.Height(20)))
            t = "";

        GUILayout.EndArea();
    }*/

    public void ObtainSkill_Light()
    {
        SetByGlobal<bool>("LightSkill", true);
    }
    public void ObtainSkill_Moving()
    {
        SetByGlobal<bool>(SkillOrderData[_cheatSkillId].Key, true);

        UpdateValueByGlobal<int>("SkillPiece", x => x + 3);

        _cheatSkillId = _cheatSkillId == 0 ? 1 : 0;
    }
}
