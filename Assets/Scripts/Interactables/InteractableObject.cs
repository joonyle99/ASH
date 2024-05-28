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
public abstract class InteractableObject : MonoBehaviour
{
    #region Variable

    [Header("Interactable Object")]
    [Space]

    [SerializeField] private bool _isInteractable = true;                   // ��ȣ�ۿ� ���� ����
    [SerializeField] private Transform _interactionMarkerPoint;             // ��ȣ�ۿ� ��Ŀ ����Ʈ (���̵� �ؽ�Ʈ�� ��µǴ� ��ġ)

    [Space]
    
    [SerializeField] private InteractionStateChangeType _stateChange;       // ��ȣ�ۿ� �� ������ �÷��̾��� ���� ����
    [SerializeField] private InteractionAnimationType _animationType;       // ��ȣ�ۿ� �� ������ �÷��̾� �ִϸ��̼� Ÿ�� (NPC�� ��� None)
    
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
        protected set => _isInteractable = value;
    }

    private bool _isInteracting;
    public bool IsInteracting
    {
        get => _isInteracting;
        private set => _isInteracting = value;
    }

    protected bool IsInteractionKeyUp =>  InputManager.Instance.State.InteractionKey.KeyUp;             // ��ȣ�ۿ� Ű�� ���� ��������
    protected bool IsPlayerInteractionState => Player.CurrentStateIs<InteractionState>();               // �÷��̾ ��ȣ�ۿ� ��������
    protected bool IsPlayerIsDirSync => Player.IsDirSync;                                               // �÷��̾��� �ٷκ��� ����� �Է� ������ ����ȭ �Ǿ�����

    #endregion

    #region Function

    protected abstract void OnInteract();                   // ��ȣ�ۿ� ���� �� ȣ��Ǵ� �Լ� (��� ��ȣ�ۿ� ������Ʈ�� �����ϵ��� �Ѵ�)
    public virtual void UpdateInteracting() { }             // ��ȣ�ۿ� ���� ȣ��Ǵ� ������Ʈ �Լ�
    public virtual void FixedUpdateInteracting() { }        // ��ȣ�ۿ� ���� ȣ��Ǵ� ���� ������Ʈ �Լ�
    protected virtual void OnInteractionExit() { }          // ��ȣ�ۿ� ���� �� ȣ��Ǵ� �Լ� (��� ��ȣ�ۿ� ������Ʈ�� ���������� �ʵ��� �Ѵ�)

    /// <summary>
    /// �÷����̾�� ��ȣ�ۿ��� �����ϴ� �Լ�
    /// </summary>
    public void Interact()
    {
        IsInteracting = true;

        OnInteract();

        Player.PlayerInteractionController.OnPlayerInteractionStart();     // �÷��̾�� ��ȣ�ۿ� ������ �˸���
    }
    /// <summary>
    /// �÷��̾�� ��ȣ�ۿ��� �����ϴ� �Լ�
    /// </summary>
    public void ExitInteraction()
    {
        IsInteracting = false;

        OnInteractionExit();

        Player.PlayerInteractionController.OnPlayerInteractionExit();     // �÷��̾�� ��ȣ�ۿ� ���Ḧ �˸���
    }

    public void OnDestroy()
    {
        if (!IsInteracting) return;

        ExitInteraction();
    }

    #endregion
}