using System;
using System.Collections.Generic;
using UnityEngine;

using DataGroup = System.Collections.Generic.Dictionary<string, object>;

/// <summary>
/// ���Ӽ��ִ� �����͸� �����ϴ� Ŭ����
/// </summary>
public class PersistentDataManager : HappyTools.SingletonBehaviourFixed<PersistentDataManager>
{
    [SerializeField] private SkillOrderData _skillOrderData;
    public static SkillOrderData SkillOrderData => Instance._skillOrderData;

    private Dictionary<string, DataGroup> _dataGroups = new();      // �÷��� �����͸� �����ϴµ� ���
    private DataGroup _globalDataGroup = new();                     // ������ �����͸� �����ϴµ� ���

    private int _cheatSkillId = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            // 
            SetByGlobal<bool>(SkillOrderData[_cheatSkillId].Key, true);

            // 
            UpdateValueByGlobal<int>("SkillPiece", x => x + 3);

            _cheatSkillId++;
        }

        if (Input.GetKeyDown(KeyCode.F7) || Input.GetKeyDown(KeyCode.F4))
        {
            SetByGlobal<bool>("LightSkill", true);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PrintDataGroup();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PrintGlobalDataGroup();
        }
    }

    // group data
    public static bool TryAddDataGroup(string groupName)
    {
        if (Instance == null)
            return false;

        // �̹� �����ϴ� ������ �׷��̶�� �߰����� �ʴ´�
        if (HasDataGroup(groupName))
            return false;

        // ���ο� ������ �׷��� �����Ѵ�
        Instance._dataGroups[groupName] = new DataGroup();

        return true;
    }
    public static void RemoveDataGroup(string groupName)
    {
        if (Instance == null)
            return;

        Instance._dataGroups.Remove(groupName);
    }
    public static bool HasDataGroup(string groupName)
    {
        if (Instance == null)
            return false;

        return Instance._dataGroups.ContainsKey(groupName);
    }
    public static void Reset(string groupName)
    {
        if (Instance == null)
            return;

        Instance._dataGroups[groupName].Clear();
    }
    public static void UpdateValue<T>(string groupName, string key, Func<T, T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;

        Instance._dataGroups[groupName][key] = updateFunction(Get<T>(groupName, key));
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

        if (Instance._dataGroups.ContainsKey(groupName))
            Instance._dataGroups[groupName][key] = value;
    }
    public static bool Has<T>(string groupName, string key) where T : new()
    {
        if (Instance == null)
            return false;

        return Instance._dataGroups.ContainsKey(groupName) && Instance._dataGroups[groupName].ContainsKey(key);
    }
    public static T Get<T>(string groupName, string key) where T : new()
    {
        if (Instance == null)
            return new T();

        if (Instance._dataGroups[groupName].TryGetValue(key, out object value))
            return (T)value;
        else
        {
            Instance._dataGroups[groupName][key] = new T();
            return (T)Instance._dataGroups[groupName][key];
        }
    }

    // global data
    public static void ResetByGlobal()
    {
        if (Instance == null)
            return;

        Instance._globalDataGroup.Clear();
    }
    public static void UpdateValueByGlobal<T>(string key, Func<T, T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;

        Instance._globalDataGroup[key] = updateFunction(GetByGlobal<T>(key));
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

        Instance._globalDataGroup[key] = value;
    }
    public static bool HasByGlobal<T>(string key) where T : new()
    {
        if (Instance == null)
            return false;

        return Instance._globalDataGroup.ContainsKey(key);
    }
    public static T GetByGlobal<T>(string key) where T : new()
    {
        if (Instance == null)
            return new T();

        if (Instance._globalDataGroup.TryGetValue(key, out object value))
            return (T)value;
        else
        {
            Instance._globalDataGroup[key] = new T();
            return (T)Instance._globalDataGroup[key];
        }
    }

    public static void PrintDataGroup()
    {
        if (Instance == null) return;

        string logMessage = "";

        foreach (var dataGroup in Instance._dataGroups)
        {
            logMessage += $"Data Group Name: [[ {dataGroup.Key} ]]\n";
            logMessage += "\n";

            foreach (var data in dataGroup.Value)
            {
                logMessage += $"Key ====> {data.Key}";
                logMessage += "\n\n";
                // logMessage += $"Value ====> {data.Value}";
                // logMessage += "\n\n";
            }

            logMessage += "==================================================\n\n";
        }

        Debug.Log(logMessage);
    }
    public static void PrintGlobalDataGroup()
    {
        if (Instance == null) return;

        string logMessage = "";

        foreach (var dataGroup in Instance._globalDataGroup)
        {
            logMessage += $"Key ====> {dataGroup.Key}";
            logMessage += "\n";
            logMessage += $"Value ====> {dataGroup.Value}";
            logMessage += "\n\n";
        }

        Debug.Log(logMessage);
    }
}
