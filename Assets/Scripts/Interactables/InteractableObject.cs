using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] Transform _interactionMarkerPoint;
    [SerializeField] bool _isInteractable;

    // TODO : 플레이어 상태 및 입력 override 하는 기능

    public Vector3 InteractionMarkerPoint
    {
        get
        {
            if (_interactionMarkerPoint == null)
                return SceneContext.Current.Player.transform.position;

            return _interactionMarkerPoint.position;
        }
    }
    public bool IsInteractable { get { return _isInteractable; } private set { _isInteractable = value; } }
    public bool IsInteracting { get; private set; }

    protected abstract void OnInteract();
    public abstract void UpdateInteracting();

    public void Interact()
    {
        IsInteracting = true;

        OnInteract();

        // 상호작용 시 플레이어를 Interaction State로 만들어준다
        SceneContext.Current.Player.GetComponent<PlayerBehaviour>().ChangeState<InteractionState>();

        // 플레이어와의 상호작용 애니메이션 트리거
        SceneContext.Current.Player.GetComponent<PlayerBehaviour>().Animator.SetTrigger("Interact");
    }

    public void FinishInteraction()
    {
        IsInteracting = false;

        // 상호작용 종료 시 플레이어를 Idle State로 만들어준다
        SceneContext.Current.Player.GetComponent<PlayerBehaviour>().ChangeState<IdleState>();
    }
}