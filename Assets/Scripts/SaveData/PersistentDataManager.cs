using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static JsonPersistentData;
using static UnityEngine.Rendering.DebugUI;
using DataGroup = System.Collections.Generic.Dictionary<string, object>;

/// <summary>
/// ���Ӽ� �ִ� ������
/// </summary>
public class PersistentData
{
    public string _sceneName = "";
    public string SceneName
    {
        get => _sceneName; set => _sceneName = value;
    }

    public string _passageName = "";
    public string PassageName
    {
        get => _passageName; set => _passageName = value;
    }

    public Dictionary<string, DataGroup> _dataGroups = new();      // �÷��� �����͸� �����ϴµ� ���
    public DataGroup _globalDataGroup = new();                     // ������ �����͸� �����ϴµ� ���

    public PersistentData CopyPersistentData(PersistentData data)
    {
        PersistentData copyData = new PersistentData();

        copyData._sceneName = data._sceneName;
        copyData._passageName = data._passageName;

        foreach (var pair in data._dataGroups)
        {
            DataGroup copyDataGroup = new DataGroup();

            foreach (var innerPair in pair.Value)
            {
                copyDataGroup.Add(innerPair.Key, innerPair.Value);
            }

            copyData._dataGroups.Add(pair.Key, copyDataGroup);
        }

        foreach (var pair in data._globalDataGroup)
        {
            copyData._globalDataGroup.Add(pair.Key, pair.Value);
        }

        return copyData;
    }

    public static JsonPersistentData ToJsonFormatClassObject(PersistentData persistentData)
    {
        if (persistentData == null) return null;

        JsonPersistentData jsonPersistentData = new JsonPersistentData();

        jsonPersistentData._sceneName = persistentData._sceneName;

        jsonPersistentData._passageName = persistentData._passageName;

        JsonDataArray<string, JsonDataArray<string, SerializableObjectType>> jsonDataGroups = new();
        foreach (var dataGroups in persistentData._dataGroups)
        {
            JsonDataArray<string, SerializableObjectType> jsonDataGroup = new();

            foreach (var dataGroup in dataGroups.Value)
            {
                jsonDataGroup.Add(dataGroup.Key,
                    new SerializableObjectType() { Object = dataGroup.Value });
            }

            jsonDataGroups.Add(dataGroups.Key, jsonDataGroup);
        }
        jsonPersistentData._jsonDataGroups = jsonDataGroups;

        JsonDataArray<string, SerializableObjectType> jsonGlobalDataGroup = new();
        foreach (var dataGroup in persistentData._globalDataGroup)
        {
            jsonGlobalDataGroup.Add(dataGroup.Key,
                new SerializableObjectType() { Object = dataGroup.Value });
        }
        jsonPersistentData._jsonGlobalDataGroup = jsonGlobalDataGroup;

        return jsonPersistentData;
    }

    public void PrintData()
    {
        Debug.Log("Scene Name : " + _sceneName);
        Debug.Log("Passage Name : " + _passageName);
        Debug.Log("=============dataGroups============");
        foreach (var pair in _dataGroups)
        {
            Debug.Log("Group : " + pair.Key);
            foreach (var innerPair in pair.Value)
            {
                Debug.Log("Key : " + innerPair.Key + " Value : " + innerPair.Value);
            }
        }

        Debug.Log("=============GlobalDataGroups============");
        foreach (var pair in _globalDataGroup)
        {
            Debug.Log("Key : " + pair.Key + " Value : " + pair.Value);
        }
    }
}

/// <summary>
/// ���Ӽ��ִ� �����͸� �����ϴ� Ŭ����
/// </summary>
/// 
public class PersistentDataManager : HappyTools.SingletonBehaviourFixed<PersistentDataManager>
{
    private PersistentData _persistentData = new();
    public PersistentData PersistentData => Instance._persistentData;

    private JsonPersistentData _savedPersistentData = new();
    public JsonPersistentData SavedPersistentData => Instance._savedPersistentData;


    [SerializeField] private SkillOrderData _skillOrderData;
    public static SkillOrderData SkillOrderData => Instance._skillOrderData;

    private int _cheatSkillId = 0;

    private void Update()
    {
        // ���� ����, �뽬 ȹ��
        if (Input.GetKeyDown(KeyCode.F8))
        {
            SetByGlobal<bool>(SkillOrderData[_cheatSkillId].Key, true);

            UpdateValueByGlobal<int>("SkillPiece", x => x + 3);

            _cheatSkillId++;
        }

        // �� ��ų ȹ��
        if (Input.GetKeyDown(KeyCode.F7) || Input.GetKeyDown(KeyCode.F4))
        {
            SetByGlobal<bool>("LightSkill", true);
        }

        // ����� �ڵ�
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PrintDataGroup();
        }
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
            Debug.Log("=============save Data============");
            SavedPersistentData.PrintData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Load");
            LoadToSavedData();
        }
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
        Instance._persistentData._dataGroups[groupName] = new DataGroup();

        return true;
    }
    public static void RemoveDataGroup(string groupName)
    {
        if (Instance == null)
            return;

        Instance._persistentData._dataGroups.Remove(groupName);
    }
    public static bool HasDataGroup(string groupName)
    {
        if (Instance == null)
            return false;

        return Instance._persistentData._dataGroups.ContainsKey(groupName);
    }
    public static void Reset(string groupName)
    {
        if (Instance == null)
            return;

        Instance._persistentData._dataGroups[groupName].Clear();
    }
    public static void UpdateValue<T>(string groupName, string key, Func<T, T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;

        Instance._persistentData._dataGroups[groupName][key] = updateFunction(Get<T>(groupName, key));
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

        if (Instance._persistentData._dataGroups.ContainsKey(groupName))
        {
            Instance._persistentData._dataGroups[groupName][key] = value;
        }
    }
    public static bool Has<T>(string groupName, string key) where T : new()
    {
        if (Instance == null)
            return false;

        return Instance._persistentData._dataGroups.ContainsKey(groupName) && Instance._persistentData._dataGroups[groupName].ContainsKey(key);
    }
    public static T Get<T>(string groupName, string key) where T : new()
    {
        if (Instance == null)
            return new T();

        if (Instance._persistentData._dataGroups[groupName].TryGetValue(key, out object value))
        {
            Debug.Log(key + "'s type : " + value.GetType());
            return (T)value;
        }
        else
        {
            Instance._persistentData._dataGroups[groupName][key] = new T();
            return (T)Instance._persistentData._dataGroups[groupName][key];
        }
    }
    #endregion

    #region global data
    public static void ResetByGlobal()
    {
        if (Instance == null)
            return;

        Instance._persistentData._globalDataGroup.Clear();
    }
    public static void UpdateValueByGlobal<T>(string key, Func<T, T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;

        Instance._persistentData._globalDataGroup[key] = updateFunction(GetByGlobal<T>(key));
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

        Instance._persistentData._globalDataGroup[key] = value;
    }
    public static bool HasByGlobal<T>(string key) where T : new()
    {
        if (Instance == null)
            return false;

        return Instance._persistentData._globalDataGroup.ContainsKey(key);
    }
    public static T GetByGlobal<T>(string key) where T : new()
    {
        if (Instance == null)
            return new T();

        if (Instance._persistentData._globalDataGroup.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        else
        {
            Instance._persistentData._globalDataGroup[key] = new T();
            return (T)Instance._persistentData._globalDataGroup[key];
        }
    }
    #endregion

    public static void PrintDataGroup()
    {
        if (Instance == null) return;

        string logMessage = "";

        foreach (var dataGroup in Instance._persistentData._dataGroups)
        {
            logMessage += $"Data Group Name: [[ {dataGroup.Key} ]]\n";
            logMessage += "\n";

            foreach (var data in dataGroup.Value)
            {
                logMessage += $"Key ====> {data.Key}";
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

        foreach (var dataGroup in Instance._persistentData._globalDataGroup)
        {
            logMessage += $"Key ====> {dataGroup.Key}";
            logMessage += "\n";
            logMessage += $"Value ====> {dataGroup.Value}";
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

        if (Instance._savedPersistentData.SceneName == "")
        {
            Instance._savedPersistentData.SceneName = SceneManager.GetActiveScene().name;
        }

        if (Instance._savedPersistentData.PassageName == "")
        {
            Instance._savedPersistentData.PassageName = passageName;
        }
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
        JsonDataManager.JsonLoad();
        Instance._savedPersistentData = JsonDataManager.GetObjectInGlobalSaveData<JsonPersistentData>("PersistentData");

        if(Instance._savedPersistentData != null)
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

            SaveAndLoader.IsChangeSceneByLoading = true;
            SceneChangeManager.Instance.ChangeToPlayableScene(sceneName, passageName);
            return true;
        }

        //����� �����Ͱ� ���� ���
        Debug.Log("Have not saved data");
        return false;
    }
}
