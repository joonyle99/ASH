using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] Transform _interactionMarkerPoint;
    [SerializeField] bool _isInteractable;

    // TODO : �÷��̾� ���� �� �Է� override �ϴ� ���

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
        PlayerBehaviour player = SceneContext.Current.Player.GetComponent<PlayerBehaviour>();

        // ��ȣ�ۿ��� ������ ���¸� ����
        if (!player.StateIs<IdleState>() && !player.StateIs<RunState>())
            return;

        IsInteracting = true;

        OnInteract();

        // ��ȣ�ۿ� �� �÷��̾ Interaction State�� ������ش�
        player.ChangeState<InteractionState>();

        // �÷��̾���� ��ȣ�ۿ� �ִϸ��̼� Ʈ����
        player.Animator.SetTrigger("Interact");
    }

    public void FinishInteraction()
    {
        IsInteracting = false;

        // ��ȣ�ۿ� ���� �� �÷��̾ Idle State�� ������ش�
        SceneContext.Current.Player.GetComponent<PlayerBehaviour>().ChangeState<IdleState>();
    }
}