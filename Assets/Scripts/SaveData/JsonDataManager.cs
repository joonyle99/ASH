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

    // 새로운 데이터 추가시 사용
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

    // 들어가있는 데이터 수정시 사용
    public static void Set(string key, string value)
    {
        if(Instance == null)
        {
            return;
        }

        _globalSaveData.saveDataGroup[key] = value;
    }

    // 데이터가 있는지 비교
    public static bool Has(string key)
    {
        if(Instance == null)
        {
            return false;
        }

        return _globalSaveData.saveDataGroup.ContainsKey(key);
    }

    // JSON 파일로 저장
    public static void JsonSave()
    {
        JsonDataArray<string, string> arrayJson = Instance.DictionaryConvert(_globalSaveData.saveDataGroup);

        string json = JsonUtility.ToJson(arrayJson, true);

        File.WriteAllText(Instance.path, json);

        Debug.Log("저장");
    }

    // JSON 파일 불러오기
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

    // Dictionary를 List로 변환할 때 사용
    // Dictionary 자체로 JSON에 저장할수 없기 때문에 사용
    // JsonSave 함수에서 사용됨
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

    // List를 Dictionary로 변환할 때 사용
    // Json파일로 받아온 데이터가 List로 되어있기 때문에 Dictionary로 변환할때 사용
    // JsonLoad 함수에서 사용
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
