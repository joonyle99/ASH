using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    None = 0,

    Push,
    Roll,
    Pull,
    Dialogue,

}
public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] Transform _interactionMarkerPoint;
    [SerializeField] bool _isInteractable;
    [SerializeField] InteractionType _interactionType;
    // TODO : 플레이어 상태 및 입력 override 하는 기능

    public InteractionType InteractionTypeWithPlayer { get { return _interactionType; } }
    public Vector3 InteractionMarkerPoint
    {
        get
        {
            if (_interactionMarkerPoint == null)
                return SceneContext.Current.Player.transform.position;

            return _interactionMarkerPoint.position;
        }
    }
    public bool IsInteractable { get { return _isInteractable; } protected set { _isInteractable = value; } }
    public bool IsInteracting { get; private set; }

    protected abstract void OnInteract();
    public abstract void UpdateInteracting();

    public void Interact()
    {
        IsInteracting = true;
        OnInteract();
    }

    public void FinishInteraction()
    {
        IsInteracting = false;

        // 상호작용 종료 시 플레이어를 Idle State로 만들어준다
        SceneContext.Current.Player.GetComponent<PlayerBehaviour>().ChangeState<IdleState>();
    }
}