using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class DataDictionary<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}

[Serializable]
public class JsonDataArray<TKey, TValue>
{
    public List<DataDictionary<TKey, TValue>> data;
}

[Serializable]
public class SaveData
{
    public Dictionary<string, string> saveDataGroup = new Dictionary<string, string>();
}


public class JsonDataManager : HappyTools.SingletonBehaviourFixed<JsonDataManager>
{
    private string path;

    public static SaveData _globalSaveData = new SaveData();

    private void Start()
    {
        path = Path.Combine(Application.dataPath, "database.json");
        JsonLoad();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            JsonSave();
        }
    }

    public static void Add(string key, string value)
    {
        if(Instance == null)
        {
            return;
        }

        if(Has(key))
        {
            Set(key, value);
        } else
        {
            _globalSaveData.saveDataGroup.Add(key, value);
        }
    }

    public static void Set(string key, string value)
    {
        if(Instance == null)
        {
            return;
        }

        _globalSaveData.saveDataGroup[key] = value;
    }

    public static bool Has(string key)
    {
        if(Instance == null)
        {
            return false;
        }

        return _globalSaveData.saveDataGroup.ContainsKey(key);
    }

    public static void JsonSave()
    {
        JsonDataArray<string, string> arrayJson = Instance.DictionaryConvert(_globalSaveData.saveDataGroup);

        string json = JsonUtility.ToJson(arrayJson, true);

        File.WriteAllText(Instance.path, json);

        Debug.Log("¿˙¿Â");
    }

    public void JsonLoad()
    {
        SaveData data = new SaveData();

        if (!File.Exists(path))
        {
            JsonSave();
        }
        else
        {
            string fromJsonData = File.ReadAllText(Application.dataPath + "/database.json");

            Dictionary<string, string> dataDic = new Dictionary<string, string>();

            dataDic = ToDictionary<string, string>(fromJsonData);

            _globalSaveData.saveDataGroup = dataDic;
        }
    }

    public JsonDataArray<TKey, TValue> DictionaryConvert<TKey, TValue>(Dictionary<TKey, TValue> dic)
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

    public Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(string path)
    {
        JsonDataArray<TKey, TValue> dataList = JsonUtility.FromJson<JsonDataArray<TKey, TValue>>(path);

        Dictionary<TKey, TValue> returnDictionary = new Dictionary<TKey, TValue>();

        for(int i = 0; i < dataList.data.Count; i++)
        {
            DataDictionary<TKey, TValue> dataDictionary = dataList.data[i];
            returnDictionary[dataDictionary.Key] = dataDictionary.Value;
        }

        return returnDictionary;
    }
}
