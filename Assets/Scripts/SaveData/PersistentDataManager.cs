using System;
using System.Collections.Generic;
using UnityEngine;
using DataGroup = System.Collections.Generic.Dictionary<string, object>;

using static JsonPersistentData;
using UnityEditor;
using JetBrains.Annotations;
using System.Linq;

/// <summary> ���Ӽ� �ִ� ������ </summary>
public class PersistentData
{
    public string SceneName = "";
    public string PassageName = "";

    public Dictionary<string, DataGroup> DataGroups = new();      // �÷��� �����͸� �����ϴµ� ���
    public DataGroup GlobalDataGroup = new();                     // ������ �����͸� �����ϴµ� ���

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
                //������õ� ū ���� ����� �Ʒ� �ڵ� ���� ���
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

/// <summary> ���Ӽ��ִ� �����͸� �����ϴ� Ŭ���� </summary>
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
        // CHEAT: F7 Ű�� ������ �� ��ų ȹ��
        if (Input.GetKeyDown(KeyCode.F7) && GameSceneManager.Instance.CheatMode == true)
        {
            SetByGlobal<bool>("LightSkill", true);
        }

        // CHEAT: F8 Ű�� ������ ���� ����, �뽬 ȹ��
        if (Input.GetKeyDown(KeyCode.F8) && GameSceneManager.Instance.CheatMode == true)
        {
            SetByGlobal<bool>(SkillOrderData[_cheatSkillId].Key, true);

            UpdateValueByGlobal<int>("SkillPiece", x => x + 3);

            _cheatSkillId = _cheatSkillId == 0 ? 1 : 0;
        }

#if UNITY_EDITOR
        /*
        // ������ �׷� ���
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PrintDataGroup();
        }
        // �۷ι� ������ �׷� ���
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

        // �̹� �����ϴ� ������ �׷��̶�� �߰����� �ʴ´�
        if (HasDataGroup(groupName))
            return false;

        // ���ο� ������ �׷��� �����Ѵ�
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

        // �ߺ��� key�� ���ο� value�� ������ �����
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
     * �ֽ������� �����ϰ� �ִ� _persistentData�� ������
     * _savedPersistentData�� ����
     * 
     * ��_savedPersistentData�� ���(Ȥ�� �ҷ�����)�� �ε�� ������
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
     * ���� �ε�� ������Ʈ�� �����ϴ� _persistentData�� 
     * _savedPersistentData�� ��ü�Ͽ� ����� �����Ͱ� �ҷ���
     * </summary>
     */
    public static void ReplacePDataToSavedPData()
    {
        Instance._persistentData = ToNormalFormatClassObject(Instance._savedPersistentData);
    }

    /**
     * <summary>
     * ����� �����͸� �ҷ����� ���
     * ����� ���� �����͸� �ҷ����� ������ �ش� �Լ� ���
     * �߰��� ������ �����Ѵٸ� false ����
     * </summary>
     */
    public static bool LoadToSavedData()
    {
        if((object)InputManager.Instance.CurrentSetter != InputManager.Instance.DefaultInputSetter)
        {
            Debug.LogWarning("Can't load saved data, because input setter is not Default");

            return false;
        }

        ///---------�� �ı� �� ����Ǿ�� �ϴ� ��----------
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

        JsonDataManager.JsonLoad();
        Instance._savedPersistentData = JsonDataManager.GetObjectInGlobalSaveData<JsonPersistentData>("PersistentData");

        if (Instance._savedPersistentData != null)
        {
            ReplacePDataToSavedPData();

            MonsterRespawnManager.Instance.StopRespawnCoroutine();
            string sceneName = Instance.PersistentData.SceneName;
            string passageName = Instance.PersistentData.PassageName;
            if (sceneName == "" || passageName == "")
            {
                Debug.LogWarning("Not Saved Scene or PassageData Load");
                return false;
            }

            SceneChangeManager.Instance.SceneChangeType = SceneChangeType.Loading;
            SceneChangeManager.Instance.ChangeToPlayableScene(sceneName, passageName);
            return true;
        }

        //����� �����Ͱ� ���� ���
        Debug.Log("Have not saved data");
        return false;
    }

    //���ѷα� ȭ�鿡�� ȣ��ǵ��� �ϱ�
    public static void ClearPersistentData()
    {
        Instance.PersistentData.SceneName = "";
        Instance.PersistentData.PassageName = "";
        Instance.PersistentData.DataGroups.Clear();
        Instance.PersistentData.GlobalDataGroup.Clear();
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
}
