using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using DataGroup = System.Collections.Generic.Dictionary<string, object>;

//json������ ���� Ŭ������ �ִٸ� �ش� �κп� �ʱ�ȭ
#region Json����ȭ �������̽� Ŭ����
/// <summary>
/// objectŸ���� ����ȭ �� ������ȭ�� ���� Ÿ��
/// </summary>
[Serializable]
public class SerializableObjectType
{
    private object _object;
    public object Object
    {
        set
        {
            _object = value;
            OnBeforeSerialize();
        }

        get
        {
            OnAfterDeserialize();
            return _object;
        }
    }

    [SerializeField, HideInInspector]
    private string _objectSerialized = "";
    public string ObjectSerialized
    {
        get
        {
            return _objectSerialized;
        }
    }

    public void OnBeforeSerialize()
    {
        if (_object == null)
        {
            _objectSerialized = "n";
            return;
        }
        var type = _object.GetType();
        if (type == typeof(int))
            _objectSerialized = "i" + _object.ToString();
        else if (type == typeof(float))
            _objectSerialized = "f" + _object.ToString();
        else if (type == typeof(Vector3))
        {
            Vector3 v = (Vector3)_object;
            _objectSerialized = "v" + v.x + "|" + v.y + "|" + v.z;
        }
        else if (type == typeof(bool))
        {
            _objectSerialized = "b" + _object.ToString();
        }
        else if (type == typeof(TransformState))
        {
            TransformState v = (TransformState)_object;
            _objectSerialized = "t" + v.Position.x + "|" + v.Position.y + "|" + v.Position.z + "|"
                + v.Rotation.x + "|" + v.Rotation.y + "|" + v.Rotation.z + "|" + v.Rotation.w + "|"
                + v.Scale.x + "|" + v.Scale.y + "|" + v.Scale.z;
        }
    }

    public void OnAfterDeserialize()
    {
        if (_objectSerialized.Length == 0)
            return;
        char type = _objectSerialized[0];
        if (type == 'n')
            _object = null;
        else if (type == 'i')
            _object = int.Parse(_objectSerialized.Substring(1));
        else if (type == 'f')
            _object = float.Parse(_objectSerialized.Substring(1));
        else if (type == 'v')
        {
            string[] v = _objectSerialized.Substring(1).Split('|');
            _object = new Vector3(float.Parse(v[0]), float.Parse(v[1]), float.Parse(v[2]));
        }
        else if (type == 'b')
        {
            _object = bool.Parse(_objectSerialized.Substring(1));
        }
        else if (type == 't')
        {
            string[] v = _objectSerialized.Substring(1).Split('|');
            TransformState ts = new TransformState();
            ts.Position = new Vector3(float.Parse(v[0]), float.Parse(v[1]), float.Parse(v[2]));
            ts.Rotation = new Quaternion(float.Parse(v[3]), float.Parse(v[4]), float.Parse(v[5]), float.Parse(v[6]));
            ts.Scale = new Vector3(float.Parse(v[7]), float.Parse(v[8]), float.Parse(v[9]));

            _object = ts;
        }
    }
}

/// <summary>
/// ����ȭ ������ PersistentData������ Ŭ����
/// </summary>
[Serializable]
public class JsonPersistentData
{
    public string SceneName = "";

    public string PassageName = "";

    public JsonDataArray<string, JsonDataArray<string, SerializableObjectType>> _jsonDataGroups;
    public JsonDataArray<string, SerializableObjectType> _jsonGlobalDataGroup;

    public static PersistentData ToNormalFormatClassObject(JsonPersistentData jsonPersistentData)
    {
        if (jsonPersistentData == null) return null;

        PersistentData persistentData = new PersistentData();

        persistentData.SceneName = jsonPersistentData.SceneName;

        persistentData.PassageName = jsonPersistentData.PassageName;

        Dictionary<string, DataGroup> dataGroups = new();
        for (int i = 0; i < jsonPersistentData._jsonDataGroups.data.Count; i++)
        {
            string key = jsonPersistentData._jsonDataGroups.data[i].Key;
            var value = jsonPersistentData._jsonDataGroups.data[i].Value;

            DataGroup dataGroup = new();
            for (int j = 0; j < value.data.Count; j++)
            {
                dataGroup.Add(value.data[j].Key, value.data[j].Value.Object);
            }

            dataGroups.Add(key, dataGroup);
        }
        persistentData.DataGroups = dataGroups;

        DataGroup globalDataGroup = new();
        for (int i = 0; i < jsonPersistentData._jsonGlobalDataGroup.data.Count; i++)
        {
            string key = jsonPersistentData._jsonGlobalDataGroup.data[i].Key;
            var value = jsonPersistentData._jsonGlobalDataGroup.data[i].Value.Object;

            globalDataGroup.Add(key, value);
        }
        persistentData.GlobalDataGroup = globalDataGroup;

        return persistentData;
    }

    public void PrintData()
    {
        Debug.Log("Scene Name : " + SceneName);
        Debug.Log("Passage Name : " + PassageName);
        Debug.Log("=============dataGroups============");
        for (int i = 0; i < _jsonDataGroups.data.Count; i++)
        {
            string group = _jsonDataGroups.data[i].Key;
            var value = _jsonDataGroups.data[i].Value;
            Debug.Log("Group : " + group);

            for (int j = 0; j < value.data.Count(); j++)
            {
                Debug.Log("Key : " + value.data[j].Key);
                Debug.Log("Value : " + value.data[j].Value.ObjectSerialized);
            }
        }

        Debug.Log("=============GlobalDataGroups============");
        for (int i = 0; i < _jsonGlobalDataGroup.data.Count(); i++)
        {
            Debug.Log("Key : " + _jsonGlobalDataGroup.data[i].Key);
            Debug.Log("Value : " + _jsonGlobalDataGroup.data[i].Value.ObjectSerialized);
        }
    }
}

/// <summary>
/// ����ȭ ������ PlayerStatus���� Ŭ����
/// </summary>
[Serializable]
public class JsonPlayerData
{
    [SerializeField]
    public int _maxHp = 10;
    [SerializeField]
    public int _currentHp = 5;

    public JsonPlayerData(int maxHp, int currentHp)
    {
        _maxHp = maxHp;
        _currentHp = currentHp;
    }
}
#endregion

[Serializable]
public class DataDictionary<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}

[Serializable]
public class JsonDataArray<TKey, TValue>
{
    public List<DataDictionary<TKey, TValue>> data = new();

    public void Add(TKey key, TValue value)
    {
        data.Add(new DataDictionary<TKey, TValue>() { Key = key, Value = value });
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> dictionaryData = new Dictionary<TKey, TValue>();

        for (int i = 0; i < data.Count; i++)
        {
            dictionaryData.Add(data[i].Key, data[i].Value);
        }

        return dictionaryData;
    }
}

[Serializable]
public class SaveData
{
    public Dictionary<string, string> saveDataGroup = new Dictionary<string, string>();

    public void DebugSaveData()
    {
        string message = "Debug - SaveData\n";
        message += "================================\n";
        foreach (var saveData in saveDataGroup)
        {
            message += saveData.Key + " : " + saveData.Value + "\n";
        }
        message += "================================\n";
        Debug.Log(message);
    }
}

public class JsonDataManager : HappyTools.SingletonBehaviourFixed<JsonDataManager>
{
    private string path;

    private SaveData _globalSaveData = new SaveData();
    public SaveData GlobalSaveData => Instance._globalSaveData;

    protected override void Awake()
    {
        path = Path.Combine(Application.dataPath, "database.json");
        JsonLoad();
    }

    private void OnApplicationQuit()
    {
        JsonSave();
    }

    #region Main Json Save Logic
    // ���ο� ������ �߰��� ���
    public static void Add(string key, string value)
    {
        if (Instance == null)
        {
            return;
        }

        if (Has(key))
        {
            Set(key, value);
        }
        else
        {
            Instance._globalSaveData.saveDataGroup.Add(key, value);
        }
    }

    // ���ִ� ������ ������ ���
    public static void Set(string key, string value)
    {
        if (Instance == null)
        {
            return;
        }

        Instance._globalSaveData.saveDataGroup[key] = value;
    }

    // �����Ͱ� �ִ��� ��
    public static bool Has(string key)
    {
        if (Instance == null)
        {
            return false;
        }

        return Instance._globalSaveData.saveDataGroup.ContainsKey(key);
    }

    // JSON ���Ϸ� ����
    public static void JsonSave()
    {
        JsonDataArray<string, string> arrayJson = DictionaryConvert(Instance._globalSaveData.saveDataGroup);

        string json = JsonUtility.ToJson(arrayJson, true);

        File.WriteAllText(Instance.path, json);

        Debug.Log("Save Gamedata To Json File");
    }

    // JSON ���� �ҷ�����
    public static void JsonLoad()
    {
        SaveData data = new SaveData();

        if (!File.Exists(Instance.path))
        {
            JsonSave();
        }
        else
        {
            string fromJsonData = File.ReadAllText(Application.dataPath + "/database.json");

            Dictionary<string, string> dataDic = new Dictionary<string, string>();

            dataDic = ToDictionary<string, string>(fromJsonData);

            Instance._globalSaveData.saveDataGroup = dataDic;

            Debug.Log("Load Gamedata From Json File");
        }
    }

    /**<summary>
     * json���ϰ� ���� ��ȣ�ۿ� �ϴ� _globalSaveData������ key�� �̿��ؼ� �ش� Ŭ������ �ִ���
     * Ȯ���ϰ� �ִٸ� �ش� Ŭ������ ������Ʈ ����
     * ���ش� �Լ� ȣ�� �� �ֽ� json������ ȣ���ϰ� �ʹٸ� JsonLoad()�Լ� ȣ�� �ʿ�
     * </summary>
     */
    public static T GetObjectInGlobalSaveData<T>(string key)
    {
        if (Instance._globalSaveData == null || !Has(key))
            return default(T);

        string json = Instance._globalSaveData.saveDataGroup[key];
        T Object = JsonUtility.FromJson<T>(json);
        return Object;
    }

    /// <summary>
    /// Dictionary�� List�� ��ȯ�� �� ���
    /// Dictionary ��ü�� JSON�� �����Ҽ� ���� ������ ���
    /// JsonSave �Լ����� ����
    /// </summary>
    public static JsonDataArray<TKey, TValue> DictionaryConvert<TKey, TValue>(Dictionary<TKey, TValue> dic)
    {
        List<DataDictionary<TKey, TValue>> dataList = new List<DataDictionary<TKey, TValue>>();
        DataDictionary<TKey, TValue> dataDictionary;

        foreach (TKey key in dic.Keys)
        {
            dataDictionary = new DataDictionary<TKey, TValue>();
            dataDictionary.Key = key;
            dataDictionary.Value = dic[key];
            dataList.Add(dataDictionary);
        }

        JsonDataArray<TKey, TValue> arrayJson = new JsonDataArray<TKey, TValue>();
        arrayJson.data = dataList;

        return arrayJson;
    }

    /// <summary>
    /// List�� Dictionary�� ��ȯ�� �� ���
    /// Json���Ϸ� �޾ƿ� �����Ͱ� List�� �Ǿ��ֱ� ������ Dictionary�� ��ȯ�Ҷ� ���
    /// JsonLoad �Լ����� ���
    /// </summary>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(string path)
    {
        JsonDataArray<TKey, TValue> dataList = JsonUtility.FromJson<JsonDataArray<TKey, TValue>>(path);

        Dictionary<TKey, TValue> returnDictionary = new Dictionary<TKey, TValue>();

        for (int i = 0; i < dataList.data.Count; i++)
        {
            DataDictionary<TKey, TValue> dataDictionary = dataList.data[i];
            returnDictionary[dataDictionary.Key] = dataDictionary.Value;
        }

        return returnDictionary;
    }
    #endregion

    #region Convert To Json Interface Class
    /// <summary>
    /// �� �Լ� �ܵ����� ����� ��� JsonDataManager.JsonSave�� ȣ���� �־�� ��
    /// </summary>
    /// <param name="passageName"></param>
    public static void SavePersistentData(string passageName)
    {
        PersistentDataManager.CopyPDataToSavedPData(passageName);
        string jsonData = JsonUtility.ToJson(PersistentDataManager.Instance.SavedPersistentData);
        Add("PersistentData", jsonData);
    }

    /// <summary>
    /// �� �Լ� �ܵ����� ����� ��� JsonDataManager.JsonSave�� ȣ���� �־�� ��
    /// �ػ�����
    /// </summary>
    /// <param name="jsonPlayerData"></param>
    public static void SavePlayerData(JsonPlayerData jsonPlayerData)
    {
        string jsonData = JsonUtility.ToJson(jsonPlayerData);
        Add("PlayerData", jsonData);
    }
    #endregion

    public void DebugGlobalSaveData()
    {
        _globalSaveData.DebugSaveData();
    }
}
