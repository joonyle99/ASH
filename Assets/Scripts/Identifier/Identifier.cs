using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identifier : MonoBehaviour, ISceneContextBuildListener
{
    [SerializeField] private string _groupName;                     // ������ �׷��� �̸�
    public string GroupName => _groupName;
    [SerializeField] private string _ID;
    public string ID => _ID;

    public void OnSceneContextBuilt()
    {
        // ������ �׷��� �������� �ʴ´ٸ� ����
        PersistentDataManager.TryAddDataGroup(_groupName);
    }

    public T LoadState<T>(string additionalKey, T defaultValue) where T : new()
    {
        if (PersistentDataManager.Has<T>(_groupName, _ID + additionalKey))
        {
            return PersistentDataManager.Get<T>(_groupName, _ID + additionalKey);
        }

        return defaultValue;
    }
    public void SaveState<T>(string additionalKey, T value) where T : new()
    {
        PersistentDataManager.Set(_groupName, _ID + additionalKey, value);
    }

    public bool HasState<T>(string additionalKey) where T : new()
    {
        return PersistentDataManager.Has<T>(_groupName, _ID + additionalKey);
    }

    
}
