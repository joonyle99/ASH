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

public class PreserveState : MonoBehaviour
{
    [SerializeField] string _groupName;
    [SerializeField] string _ID;

    [SerializeField] bool _preserveTransform = true;

    string _transformKey => _ID + "_transformState";

    private void Start()
    {
        if (_preserveTransform && PersistentDataManager.Has<TransformState>(_groupName, _transformKey))
        {
            var transformState = PersistentDataManager.Get<TransformState>(_groupName, _transformKey);
            transform.localPosition = transformState.Position;
            transform.localRotation = transformState.Rotation;
            transform.localScale = transformState.Scale;
        }
    }
    void Save()
    {
        if (_preserveTransform)
            PersistentDataManager.Set(_groupName, _transformKey, new TransformState(transform));

    }
    private void OnDestroy()
    {
        try
        {
            Save();
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}