using UnityEngine;
using System;

struct TransformState
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    // 생성자
    public TransformState(Transform transform)
    {
        Position = transform.localPosition;
        Rotation = transform.localRotation;
        Scale = transform.localScale;
    }
}

public class PreserveState : MonoBehaviour, IDestructionListener
{
    [SerializeField] private string _groupName;
    [SerializeField] private string _ID;

    [SerializeField] private bool _preserveTransform = true;
    [SerializeField] private bool _preserveDestruction = true;

    private string _transformKey => _ID + "_transformState";
    private string _destructionKey => _ID + "_destructed";

#if UNITY_EDITOR
    public string EditorGroupName
    {
        get => _groupName;
        set => _groupName = value;
    }
    public string EditorID
    {
        get => _ID;
        set => _ID = value;
    }
#endif

    private void Awake()
    {
        // 트랜스폼 데이터 불러오기
        if (_preserveTransform)
            LoadState<TransformState>(_groupName, _transformKey, transformState =>
            {
                transform.localPosition = transformState.Position;
                transform.localRotation = transformState.Rotation;
                transform.localScale = transformState.Scale;
            });

        // 파괴 상태 불러오기
        if (_preserveDestruction)
            LoadState<bool>(_groupName, _destructionKey, destruct =>
            {
                if (destruct)
                    Destroy(gameObject);
            });
    }

    private void OnDestroy()
    {
        // 플레이 모드에서만 저장
        if (Application.isPlaying)
        {
            // 트랜스폼 데이터 저장 (씬 전환 시)
            if (_preserveTransform)
            {
                try
                {
                    if (PersistentDataManager.HasDataGroup(_groupName))
                    {
                        PersistentDataManager.Set(_groupName, _transformKey, new TransformState(transform));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }

    private static void LoadState<T>(string groupName, string key, Action<T> loadAction) where T : new()
    {
        if (PersistentDataManager.Has<T>(groupName, key))
        {
            var state = PersistentDataManager.Get<T>(groupName, key);
            loadAction.Invoke(state);
        }
    }

    public void Save<T>(string additionalKey, T value) where T : new()
    {
        PersistentDataManager.Set(_groupName, _ID + additionalKey, value);
    }
    public T Load<T>(string additionalKey, T defaultValue) where T : new()
    {
        if (PersistentDataManager.Has<T>(_groupName, _ID + additionalKey))
            return PersistentDataManager.Get<T>(_groupName, _ID + additionalKey);
        else
            return defaultValue;
    }

    public void OnDestruction()
    {
        if (_preserveDestruction)
        {
            PersistentDataManager.Set(_groupName, _destructionKey, true);
        }
    }
}