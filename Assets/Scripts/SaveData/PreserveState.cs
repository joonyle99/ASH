using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

struct TransformState
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public TransformState(Transform transform)
    {
        Position = transform.localPosition;
        Rotation = transform.localRotation;
        Scale = transform.localScale;
    }
}

public class PreserveState : MonoBehaviour, IDestructionListener
{
    [SerializeField] string _groupName;
    [SerializeField] string _ID;

    [SerializeField] bool _preserveTransform = true;
    [SerializeField] bool _preserveDestruction = true;

    string _transformKey => _ID + "_transformState";
    string _destructionKey => _ID + "_destructed";

    private void Awake()
    {
        if (_preserveTransform)
            LoadState<TransformState>(_groupName, _transformKey, transformState =>
            {
                transform.localPosition = transformState.Position;
                transform.localRotation = transformState.Rotation;
                transform.localScale = transformState.Scale;
            });
        if (_preserveDestruction)
            LoadState<bool>(_groupName, _destructionKey, destruct =>
            {
                if (destruct)
                    Destroy(gameObject);
            });
    }
    public static void LoadState<T>(string groupName, string key, Action<T> loadAction) where T : new()
    {
        if (PersistentDataManager.Has<T>(groupName, key))
        {
            var state = PersistentDataManager.Get<T>(groupName, key);
            loadAction.Invoke(state);
        }
    }
    public void OnDestruction()
    {
        if (_preserveDestruction)
        {
            PersistentDataManager.Set(_groupName, _destructionKey, true);
        }
    }
    private void OnDestroy()
    {
        if (_preserveTransform)
        {
            try
            {
                if (PersistentDataManager.HasDataGroup(_groupName))
                    PersistentDataManager.Set(_groupName, _transformKey, new TransformState(transform));
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

}