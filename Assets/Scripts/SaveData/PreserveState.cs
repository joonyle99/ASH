using UnityEngine;
using System;

/// <summary>
/// Ʈ������ ���¸� �����ϴ� ����ü
/// </summary>
struct TransformState
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    // ������
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
/// ������Ʈ�� ���¸� �����ϰ� �ҷ����� Ŭ����
/// Ʈ�������� �ı� ���¿� ���� ����� �ҷ����⸦ ��ü������ �����ϰ�
/// �߰����� ���¸� �����ϰ� �ҷ����� ��ɴ� �� ������Ʈ�� Awake()�� OnDestroy()���� ���� �����ؾ��Ѵ�
/// </summary>
public partial class PreserveState : MonoBehaviour, IDestructionListener, ISceneContextBuildListener
{
    [SerializeField] private string _groupName;                     // ������ �׷��� �̸�
    [SerializeField] private string _ID;                            // �������� ID
    [SerializeField] public string ID => _ID;

    [SerializeField] private bool _preserveTransform = true;        // Ʈ������ �����͸� �������� ����
    [SerializeField] private bool _preserveDestruction = true;      // �ı� ���¸� �������� ����

    public string TransformKey => _ID + "_transformState";          // Ʈ������ �������� Ű
    public string DestructionKey => _ID + "_destructed";            // �ı� ������ Ű

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

    // Ʈ�������� �ı� ���¸� �ҷ��� �ʱ�ȭ �ϴ� �۾� (������ �����ʹ� ���� �ʱ�ȭ �ؾ��Ѵ�)
    private void Awake()
    {
        
    }
    public void OnSceneContextBuilt()
    {
        //OnSave�Լ� ���ε�
        SaveAndLoader.OnSaveStarted += OnSaveData;

        // ������ �׷��� �������� �ʴ´ٸ� ����
        PersistentDataManager.TryAddDataGroup(_groupName);

        // Ʈ������ ������ �ҷ�����
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

        // �ı� ���� �ҷ�����
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

    // ������Ʈ �ı� �� (�� ��ȯ ��) �����͸� �����ϴ� �۾�
    private void OnDestroy()
    {
        // �÷��� ��忡���� ����
        if (Application.isPlaying)
        {
            if (SceneChangeManager.Instance && SceneChangeManager.Instance.SceneChangeType == SceneChangeType.ChangeMap)
            {
                // Ʈ������ ������ ���� (�� ��ȯ ��)
                SaveTransformState();
            }
            SaveAndLoader.OnSaveStarted -= OnSaveData;
        }
    }
    // ������Ʈ�� �ı��Ǿ��� �� �ı� ������ �����͸� �����ϴ� �۾�
    public void OnDestruction()
    {
        // �ı� ���� ������ ����(��, �������� �ʱ�ȭ �ƴ� ���)
        if (_preserveDestruction)
        {
            PersistentDataManager.Set(_groupName, DestructionKey, true);
            SaveAndLoader.OnSaveStarted -= OnSaveData;

        }
    }

    // Ʈ������ �����͸� �����ϴ� �۾�
    public void SaveTransformState()
    {
        if (_preserveTransform)
        {
            try
            {
                // �ش� ������ �׷��� �����Ѵٸ�
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
        // �����Ͱ� �����Ѵٸ�
        if (PersistentDataManager.Has<T>(groupName, key))
        {
            // �����͸� �����´�
            // Debug.Log("Loading Saved Data : { key : " + key + " value : " + PersistentDataManager.Get<T>(groupName, key).ToString() + " }");
            var state = PersistentDataManager.Get<T>(groupName, key);

            // �����͸� �����Ѵ�
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