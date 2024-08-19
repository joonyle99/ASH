using UnityEngine;

public enum InteractionAnimationType
{
    None = 0,

    Push,
    Roll,
}

public enum InteractionStateChangeType
{
    None = 0,

    InteractionState,
}

/// <summary>
/// �÷��̾�� ��ȣ�ۿ��� ������ ������Ʈ
/// </summary>
public abstract class InteractableObject : MonoBehaviour, ISceneContextBuildListener
{
    #region Variable

    [Header("Interactable Object")]
    [Space]

    [SerializeField] private bool _isInteractable = true;                   // ��ȣ�ۿ� ���� ����
    [SerializeField] private bool _isInteracting = false;                   // ��ȣ�ۿ� ������ ����

    [Space]

    [SerializeField] private Transform _interactionMarkerPoint;             // ��ȣ�ۿ� ��Ŀ ����Ʈ (���̵� �ؽ�Ʈ�� ��µǴ� ��ġ)

    [Space]

    [SerializeField] private InteractionStateChangeType _stateChange;       // ��ȣ�ۿ� �� ������ �÷��̾��� ���� ����
    [SerializeField] private InteractionAnimationType _animationType;       // ��ȣ�ۿ� �� ������ �÷��̾� �ִϸ��̼� Ÿ�� (NPC�� ��� None)

    [SerializeField] private bool _interactAtFirst = true;

    [SerializeField]
    [HideInInspector] private Identifier _identifier;

    #endregion

    #region Property

    protected PlayerBehaviour Player => SceneContext.Current.Player;

    public InteractionAnimationType AnimationType => _animationType;
    public InteractionStateChangeType StateChange => _stateChange;

    public Vector3 InteractionMarkerPoint
    {
        get
        {
            if (_interactionMarkerPoint == null)
                return SceneContext.Current.Player.InteractionMarker.position;

            return _interactionMarkerPoint.position;
        }
    }
    public bool IsInteractable
    {
        get => _isInteractable;
        set => _isInteractable = value;
    }
    public bool IsInteracting
    {
        get => _isInteracting;
        set
        {
            _isInteracting = value;
        }
    }

    public bool InteractAtFirst
    {
        get => _interactAtFirst;
    }

    protected bool IsInteractionKeyUp => InputManager.Instance.State.InteractionKey.KeyUp;             // ��ȣ�ۿ� Ű�� ���� ��������
    protected bool IsPlayerInteractionState => Player.CurrentStateIs<InteractionState>();               // �÷��̾ ��ȣ�ۿ� ��������
    protected bool IsPlayerIsDirSync => Player.IsDirSync;                                               // �÷��̾��� �ٷκ��� ����� �Է� ������ ����ȭ �Ǿ�����

    #endregion

    #region Function

    private void Awake()
    {
        _identifier = GetComponent<Identifier>();
    }

    private void Start()
    {
        _interactAtFirst = true;
    }

    protected abstract void OnObjectInteractionEnter();             // ��ȣ�ۿ� ���� �� ȣ��Ǵ� �Լ� (��� ��ȣ�ۿ� ������Ʈ�� �����ϵ��� �Ѵ�)
    protected virtual void OnObjectInteractionExit() { }            // ��ȣ�ۿ� ���� �� ȣ��Ǵ� �Լ� (��� ��ȣ�ۿ� ������Ʈ�� ���������� �ʵ��� �Ѵ�)

    public virtual void UpdateInteracting() { }                     // ��ȣ�ۿ� ���� ȣ��Ǵ� ������Ʈ �Լ�
    public virtual void FixedUpdateInteracting() { }                // ��ȣ�ۿ� ���� ȣ��Ǵ� ���� ������Ʈ �Լ�

    /// <summary>
    /// �÷����̾�� ��ȣ�ۿ��� �����ϴ� �Լ�
    /// </summary>
    public void EnterInteraction()
    {
        //Debug.Log("Enter Interaction");

        IsInteracting = true;

        OnObjectInteractionEnter();

        Player.PlayerInteractionController.OnPlayerInteractionEnter();     // �÷��̾�� ��ȣ�ۿ� ������ �˸���
    }
    /// <summary>
    /// �÷��̾�� ��ȣ�ۿ��� �����ϴ� �Լ�
    /// </summary>
    public void ExitInteraction()
    {
        //Debug.Log("Exit Interaction");

        if (_identifier)
        {
            if (_isInteracting)
            {
                _interactAtFirst = false;
                _identifier.SaveState<bool>("_interactAtFirst", _interactAtFirst);
            }
        }

        IsInteracting = false;

        OnObjectInteractionExit();

        Player.PlayerInteractionController.OnPlayerInteractionExit();     // �÷��̾�� ��ȣ�ۿ� ���Ḧ �˸���
    }

    public void OnSceneContextBuilt()
    {
        if (_identifier)
        {
            if (SceneChangeManager.Instance &&
                SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                _interactAtFirst = _identifier.LoadState<bool>("_interactAtFirstSaved", true);
            }
            else
            {
                _interactAtFirst = _identifier.LoadState<bool>("_interactAtFirst", true);
            }
        }

        SaveAndLoader.OnSaveStarted += SaveInteractState;
    }

    protected virtual void OnDestroy()
    {
        if (IsInteracting)
        {
            ExitInteraction();
        }

        SaveAndLoader.OnSaveStarted -= SaveInteractState;
    }

    private void SaveInteractState()
    {
        if (_identifier)
        {
            _identifier.SaveState<bool>("_interactAtFirstSaved", _interactAtFirst);
        }
    }
    #endregion
}