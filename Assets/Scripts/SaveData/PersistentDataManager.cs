using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DataGroup = System.Collections.Generic.Dictionary<string, object>;

public class PersistentDataManager : HappyTools.SingletonBehaviourFixed<PersistentDataManager>
{
    Dictionary<string, DataGroup> _dataGroups = new Dictionary<string, DataGroup>();
    DataGroup _globalDataGroup = new DataGroup();

    public static bool TryAddDataGroup(string groupName)
    {
        if (HasDataGroup(groupName))
            return false;
        Instance._dataGroups[groupName] = new DataGroup();
        return true;
    }
    public static void ResetGroup(string groupName)
    {
        Instance._dataGroups[groupName].Clear();
    }
    public static void ResetGlobalGroup()
    {
        Instance._globalDataGroup.Clear();
    }
    public static void RemoveDataGroup(string groupName)
    {
        Instance._dataGroups.Remove(groupName);
    }
    public static bool HasDataGroup(string groupName)
    {
        return Instance._dataGroups.ContainsKey(groupName);
    }

    public static void UpdateValue<T>(string key, Func<T, T> updateFunction) where T : new()
    {
        Instance._globalDataGroup[key] = updateFunction(Get<T>(key));
    }
    public static void UpdateValue<T>(string groupName, string key, Func<T, T> updateFunction) where T : new()
    {
        Instance._dataGroups[groupName][key] = updateFunction(Get<T>(groupName, key));
    }
    public static void UpdateRef<T>(string key, Action<T> updateFunction) where T : new()
    {
        updateFunction(Get<T>(key));
    }
    public static void UpdateRef<T>(string groupName, string key, Action<T> updateFunction) where T : new()
    {
        updateFunction(Get<T>(groupName, key));
    }
    public static void Set<T>(string key, T value) where T : new()
    {
        Instance._globalDataGroup[key] = value;
    }
    public static void Set<T>(string groupName, string key, T value) where T : new()
    {
        Instance._dataGroups[groupName][key] = value;
    }
    public static bool Has<T>(string key) where T : new()
    {
        return Instance._globalDataGroup.ContainsKey(key);
    }
    public static bool Has<T>(string groupName, string key) where T : new()
    {
        return Instance._dataGroups[groupName].ContainsKey(key);
    }
    public static T Get<T>(string key) where T : new()
    {
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
        if (Instance._dataGroups[groupName].TryGetValue(key, out object value))
            return (T)value;
        else
        {
            Instance._dataGroups[groupName][key] = new T();
            return (T)Instance._dataGroups[groupName][key];
        }
    }
}
