using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identifier : MonoBehaviour, ISceneContextBuildListener
{
    [SerializeField] private string _groupName;                     // 데이터 그룹의 이름
    public string GroupName => _groupName;
    [SerializeField] private string _id;
    public string ID => _id;

#if UNITY_EDITOR
    public string EditorGroupName
    {
        get => _groupName;
        set => _groupName = value;
    }
    public string EditorID
    {
        get => _id;
        set => _id = value;
    }
#endif

    public void OnSceneContextBuilt()
    {
        // 데이터 그룹이 존재하지 않는다면 생성
        PersistentDataManager.TryAddDataGroup(_groupName);
    }

    public T LoadState<T>(string additionalKey, T defaultValue) where T : new()
    {
        if (PersistentDataManager.Has<T>(_groupName, _id + additionalKey))
        {
            return PersistentDataManager.Get<T>(_groupName, _id + additionalKey);
        }

        return defaultValue;
    }
    public void SaveState<T>(string additionalKey, T value) where T : new()
    {
        PersistentDataManager.Set(_groupName, _id + additionalKey, value);
    }

    public bool HasState<T>(string additionalKey) where T : new()
    {
        return PersistentDataManager.Has<T>(_groupName, _id + additionalKey);
    }

    
}
