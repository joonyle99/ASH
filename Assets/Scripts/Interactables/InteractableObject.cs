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
/// 플레이어와 상호작용이 가능한 오브젝트
/// </summary>
public abstract class InteractableObject : MonoBehaviour
{
    #region Variable

    [Header("Interactable Object")]
    [Space]

    [SerializeField] private bool _isInteractable = true;                   // 상호작용 가능 여부
    [SerializeField] private Transform _interactionMarkerPoint;             // 상호작용 마커 포인트 (가이드 텍스트가 출력되는 위치)

    [Space]
    
    [SerializeField] private InteractionStateChangeType _stateChange;       // 상호작용 시 변경할 플레이어의 상태 제어
    [SerializeField] private InteractionAnimationType _animationType;       // 상호작용 시 전달할 플레이어 애니메이션 타입 (NPC의 경우 None)
    
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

    protected bool IsInteractionKeyUp =>  InputManager.Instance.State.InteractionKey.KeyUp;             // 상호작용 키를 떼는 순간인지
    protected bool IsPlayerInteractionState => Player.CurrentStateIs<InteractionState>();               // 플레이어가 상호작용 상태인지
    protected bool IsPlayerIsDirSync => Player.IsDirSync;                                               // 플레이어의 바로보는 방향과 입력 방향이 동기화 되었는지

    #endregion

    #region Function

    protected abstract void OnInteract();                   // 상호작용 시작 시 호출되는 함수 (모든 상호작용 오브젝트가 구현하도록 한다)
    public virtual void UpdateInteracting() { }             // 상호작용 동안 호출되는 업데이트 함수
    public virtual void FixedUpdateInteracting() { }        // 상호작용 동안 호출되는 물리 업데이트 함수
    protected virtual void OnInteractionExit() { }          // 상호작용 종료 시 호출되는 함수 (모든 상호작용 오브젝트가 구현하지는 않도록 한다)

    /// <summary>
    /// 플레이이어와 상호작용을 시작하는 함수
    /// </summary>
    public void Interact()
    {
        IsInteracting = true;

        OnInteract();

        Player.PlayerInteractionController.OnPlayerInteractionStart();     // 플레이어에게 상호작용 시작을 알린다
    }
    /// <summary>
    /// 플레이어와 상호작용을 종료하는 함수
    /// </summary>
    public void ExitInteraction()
    {
        IsInteracting = false;

        OnInteractionExit();

        Player.PlayerInteractionController.OnPlayerInteractionExit();     // 플레이어에게 상호작용 종료를 알린다
    }

    public void OnDestroy()
    {
        if (!IsInteracting) return;

        ExitInteraction();
    }

    #endregion
}