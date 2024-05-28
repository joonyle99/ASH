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
    public enum type
    {
        String,
        Float,
        Vector3
    }
    public JsonDataArray<string, string> testDic = new JsonDataArray<string, string>();
}


public class JsonDataManager : MonoBehaviour
{
    private string path;

    private void Start()
    {
        path = Path.Combine(Application.dataPath, "database.json");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            JsonSave();
        }
    }

    public void JsonSave()
    {
        SaveData saveData = new SaveData();

        Dictionary<string, string> testData = new Dictionary<string, string>();

        float volume = 1.0f;

        string testName = "testBGMVolume";
        string name = testName + "/" + SaveData.type.Float;

        testData.Add(name, volume.ToString());

        saveData.testDic = DictionaryConvert(testData);

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(path, json);

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
