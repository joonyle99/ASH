using UnityEngine;
using System;

/// <summary>
/// 트랜스폼 상태를 저장하는 구조체
/// </summary>
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

    public override string ToString()
    {
        string str = "TransformState : { Position (" + Position.x + ", " + Position.y + ", " + Position.z + ")" +
            "Rotation(" + Rotation.x + ", " + Rotation.y + ", " + Rotation.z + ", " + Rotation.w + ")" +
            "Rotation(" + Scale.x + ", " + Scale.y + ", " + Scale.z + ") }";

        return str;
    }
}

/// <summary>
/// 오브젝트의 상태를 저장하고 불러오는 클래스
/// 트랜스폼과 파괴 상태에 대한 저장과 불러오기를 자체적으로 지원하고
/// 추가적인 상태를 저장하고 불러오는 기능는 각 오브젝트의 Awake()와 OnDestroy()에서 직접 구현해야한다
/// </summary>
public partial class PreserveState : MonoBehaviour, IDestructionListener, ISceneContextBuildListener
{
    [SerializeField] private string _groupName;                     // 데이터 그룹의 이름
    [SerializeField] private string _ID;                            // 데이터의 ID
    [SerializeField] public string ID => _ID;

    [SerializeField] private bool _preserveTransform = true;        // 트랜스폼 데이터를 저장할지 여부
    [SerializeField] private bool _preserveDestruction = true;      // 파괴 상태를 저장할지 여부

    public string TransformKey => _ID + "_transformState";          // 트랜스폼 데이터의 키
    public string DestructionKey => _ID + "_destructed";            // 파괴 상태의 키

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

    // 트랜스폼과 파괴 상태를 불러와 초기화 하는 작업 (고유한 데이터는 따로 초기화 해야한다)
    private void Awake()
    {
        
    }
    public void OnSceneContextBuilt()
    {
        //OnSave함수 바인딩
        SaveAndLoader.OnSaveStarted += OnSaveData;

        // 데이터 그룹이 존재하지 않는다면 생성
        PersistentDataManager.TryAddDataGroup(_groupName);

        // 트랜스폼 데이터 불러오기
        if (_preserveTransform)
        {
            LoadAndApplyState<TransformState>(_groupName, TransformKey, transformState =>
            {
                // Debug.Log($"Apply Transform");
                transform.localPosition = transformState.Position;
                transform.localRotation = transformState.Rotation;
                transform.localScale = transformState.Scale;
            });
        }

        // 파괴 상태 불러오기
        if (_preserveDestruction)
        {
            LoadAndApplyState<bool>(_groupName, DestructionKey, destruct =>
            {
                if (destruct)
                {
                    // Debug.Log($"Apply Destruction");
                    Destroy(gameObject);
                }
            });
        }
    }

    // 오브젝트 파괴 시 (씬 전환 시) 데이터를 저장하는 작업
    private void OnDestroy()
    {
        // 플레이 모드에서만 저장
        if (Application.isPlaying)
        {
            if (SceneChangeManager.Instance && SceneChangeManager.Instance.SceneChangeType == SceneChangeType.ChangeMap)
            {
                // 트랜스폼 데이터 저장 (씬 전환 시)
                SaveTransformState();
            }
            SaveAndLoader.OnSaveStarted -= OnSaveData;
        }
    }
    // 오브젝트가 파괴되었을 때 파괴 상태의 데이터를 저장하는 작업
    public void OnDestruction()
    {
        // 파괴 상태 데이터 저장(단, 스테이지 초기화 아닐 경우)
        if (_preserveDestruction)
        {
            PersistentDataManager.Set(_groupName, DestructionKey, true);
            SaveAndLoader.OnSaveStarted -= OnSaveData;

        }
    }

    // 트랜스폼 데이터를 저장하는 작업
    public void SaveTransformState()
    {
        if (_preserveTransform)
        {
            try
            {
                // 해당 데이터 그룹이 존재한다면
                if (PersistentDataManager.HasDataGroup(_groupName))
                {
                    PersistentDataManager.Set(_groupName, TransformKey, new TransformState(transform));
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

    private static void LoadAndApplyState<T>(string groupName, string key, Action<T> loadAction) where T : new()
    {
        // 데이터가 존재한다면
        if (PersistentDataManager.Has<T>(groupName, key))
        {
            // 데이터를 가져온다
            // Debug.Log("Loading Saved Data : { key : " + key + " value : " + PersistentDataManager.Get<T>(groupName, key).ToString() + " }");
            var state = PersistentDataManager.Get<T>(groupName, key);

            // 데이터를 적용한다
            loadAction.Invoke(state);
        }
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
    public void OnSaveData()
    {
        SaveTransformState();
    }
}