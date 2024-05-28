using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using DataGroup = System.Collections.Generic.Dictionary<string, object>;

public class PersistentDataManager : HappyTools.SingletonBehaviourFixed<PersistentDataManager>
{
    [SerializeField] private SkillOrderData _skillOrderData;

    private Dictionary<string, DataGroup> _dataGroups = new Dictionary<string, DataGroup>();
    private DataGroup _globalDataGroup = new DataGroup();

    private int _cheatSkillId = 0;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            PersistentDataManager.Set<bool>(PersistentDataManager.SkillOrderData[_cheatSkillId].Key, true);
            PersistentDataManager.UpdateValue<int>("skillPiece", x => x + 3);
            _cheatSkillId++;
        }

        if (Input.GetKeyDown(KeyCode.F7) || Input.GetKeyDown(KeyCode.F4))
        {
            PersistentDataManager.Set<bool>("Light", true);
        }
    }

    public static SkillOrderData SkillOrderData => Instance._skillOrderData;
    public static bool TryAddDataGroup(string groupName)
    {
        if (Instance == null)
            return false;
        if (HasDataGroup(groupName))
            return false;
        Instance._dataGroups[groupName] = new DataGroup();
        return true;
    }
    public static void ResetGroup(string groupName)
    {
        if (Instance == null)
            return;
        Instance._dataGroups[groupName].Clear();
    }
    public static void ResetGlobalGroup()
    {
        if (Instance == null)
            return;
        Instance._globalDataGroup.Clear();
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

    public static void UpdateValue<T>(string key, Func<T, T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;
        Instance._globalDataGroup[key] = updateFunction(Get<T>(key));
    }
    public static void UpdateValue<T>(string groupName, string key, Func<T, T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;
        Instance._dataGroups[groupName][key] = updateFunction(Get<T>(groupName, key));
    }
    public static void UpdateRef<T>(string key, Action<T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;
        updateFunction(Get<T>(key));
    }
    public static void UpdateRef<T>(string groupName, string key, Action<T> updateFunction) where T : new()
    {
        if (Instance == null)
            return;
        updateFunction(Get<T>(groupName, key));
    }
    public static void Set<T>(string key, T value) where T : new()
    {
        if (Instance == null)
            return;
        Instance._globalDataGroup[key] = value;
    }
    public static void Set<T>(string groupName, string key, T value) where T : new()
    {
        if (Instance == null)
            return;
        if (Instance._dataGroups.ContainsKey(groupName))
        {
            Instance._dataGroups[groupName][key] = value;
        }
            
    }
    public static bool Has<T>(string key) where T : new()
    {
        if (Instance == null)
            return false;
        return Instance._globalDataGroup.ContainsKey(key);
    }
    public static bool Has<T>(string groupName, string key) where T : new()
    {
        if (Instance == null)
            return false;
        return Instance._dataGroups.ContainsKey(groupName) && Instance._dataGroups[groupName].ContainsKey(key);
    }
    public static T Get<T>(string key) where T : new()
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
}
