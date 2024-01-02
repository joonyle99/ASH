using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : HappyTools.SingletonBehaviourFixed<SaveDataManager>
{
    Dictionary<string, object> _data = new Dictionary<string, object>();


    public static void UpdateValue<T>(string key, Func<T, T> updateFunction) where T : new()
    {
        Instance._data[key] = updateFunction(Get<T>(key));
    }
    public static void UpdateRef<T>(string key, Action<T> updateFunction) where T : new()
    {
        updateFunction(Get<T>(key));
    }
    public static void Set<T>(string key, T value) where T : new()
    {
        Instance._data[key] = value;
    }
    public static bool Has<T>(string key) where T : new()
    {
        return Instance._data.ContainsKey(key);
    }
    public static T Get<T>(string key) where T : new()
    {
        if (Instance._data.TryGetValue(key, out object value))
            return (T)value;
        else
        {
            Instance._data[key] = new T();
            return (T)Instance._data[key];
        }
    }
}
