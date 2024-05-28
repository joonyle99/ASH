using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� ���ؿ��� ��ȣ�ۿ� ������ ������Ʈ�� ��Ʈ���Ѵ�
/// </summary>
public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interaction Controller")]
    [Space]

    [SerializeField] private InteractableObject _closetTarget;                  // ���� ��ȣ�ۿ� ���� ������Ʈ
    public InteractableObject ClosetTarget
    {
        get => _closetTarget;
        set
        {
            // �̺�Ʈ ���
            if (_closetTarget != null && value == null)
            {
                ClosetTarget.ExitInteraction();

                if (_interactionMarker.IsMarking)
                {
                    _interactionMarker.DisableMarker();
                }
            }

            _closetTarget = value;
        }
    }

    [Space]

    [SerializeField] private List<InteractableObject> _interactionTargetList;               // ���� ���� ��ȣ�ۿ� ������ ������Ʈ ����Ʈ

    private InteractionMarker _interactionMarker;
    private PlayerBehaviour _player;

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
        _interactionMarker = FindObjectOfType<InteractionMarker>(true);
    }
    private void Start()
    {
        _interactionTargetList = new List<InteractableObject>();
    }
    private void Update()
    {
        // ��ȣ�ۿ� ��� ������Ʈ
        if (_closetTarget && !_closetTarget.IsInteracting && _interactionTargetList.Count >= 1)
        {
            PickClosestTarget();
        }

        // ��ȣ�ۿ� ����� �ִ� ���
        if (ClosetTarget)
        {
            // �̹� ��ȣ�ۿ� ���̶��
            if (ClosetTarget.IsInteracting)
            {
                // Debug.Log("Interacting");

                // ��󿡼� ��ȣ�ۿ� ������Ʈ �Լ��� �����ش�
                ClosetTarget.UpdateInteracting();
            }
            // ��ȣ�ۿ� ���� �ƴ϶��
            else
            {
                // ��ȣ�ۿ� Ű�� �Է¹޴´ٸ�
                if (InputManager.Instance.State.InteractionKey.KeyDown)
                {
                    // Debug.Log("Interact");

                    // �׸��� ��ȣ�ۿ� ������ ���¶��
                    if (_player.CanInteract)
                    {
                        // ��ȣ�ۿ� ������Ʈ������ ��ȣ�ۿ� ���� ����
                        ClosetTarget.Interact();
                    }
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (ClosetTarget)
        {
            if (ClosetTarget.IsInteracting)
                ClosetTarget.FixedUpdateInteracting();
        }
    }

    /// <summary>
    /// ��ȣ�ۿ��� ���۵Ǹ� Interaction ���·� ��ȯ�Ѵ�
    /// </summary>
    public void OnPlayerInteractionStart()
    {
        // ��ȣ�ۿ� UI ��� ����
        _interactionMarker.DeactivateMarker();

        // ��� ��ȣ�ۿ��� Interaction State�� ���̵ž� �ϴ� ���� �ƴϴ� (���̾�α׵� ��ȣ�ۿ���)
        if (ClosetTarget.StateChange == InteractionStateChangeType.InteractionState)
        {
            _player.ChangeState<InteractionState>();
        }
    }
    /// <summary>
    /// ��ȣ�ۿ��� ����Ǹ� Idle ���·� ��ȯ�Ѵ�
    /// </summary>
    public void OnPlayerInteractionExit()
    {
        // ��ȣ�ۿ� UI �ٽ� �ѱ�
        _interactionMarker.ActivateMarker();

        if (_player.CurrentStateIs<InteractionState>())
        {
            _player.ChangeState<IdleState>();
        }
    }

    /// <summary>
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ�� �߰�
    /// </summary>
    /// <param name="target"></param>
    public void AddInteractionTarget(InteractableObject target)
    {
        if (!_interactionTargetList.Contains(target))
        {
            _interactionTargetList.Add(target);

            // ��ȣ�ۿ� Ÿ���� ���� ��� �־��ش�
            if (ClosetTarget == null)
                ChangeClosetTarget(target);
        }
    }
    /// <summary>
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ���� ����
    /// </summary>
    /// <param name="target"></param>
    public void RemoveInteractionTarget(InteractableObject target)
    {
        if (_interactionTargetList.Contains(target))
        {
            _interactionTargetList.Remove(target);

            // ��ȣ�ۿ� Ÿ���� ���� ��� �������ش�
            if (ClosetTarget == target)
                ClosetTarget = null;
        }
    }

    /// <summary>
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ ��, ���� ����� Ÿ���� ����
    /// closetTarget�� null�� �� �� �ִ�.
    /// </summary>
    public void PickClosestTarget()
    {
        // ����Ʈ�� null�� �����ϸ� �����Ѵ� (x == null�� ��� ��� x�� ���� *���ٽ�*)
        // _interactionTargetList.RemoveAll(x => x == null);

        var minDist = float.MaxValue;
        var minIndex = -1;

        // �ĺ� ����Ʈ�� ��ȸ
        for (int i = 0; i < _interactionTargetList.Count; i++)
        {
            // ��ȣ�ۿ� ��� Ȱ��ȭ ���� �Ǵ�
            if (!_interactionTargetList[i].IsInteractable) continue;

            // ������ ������ ��ư ���� �˰����� ����ϸ� ��� ����� ũ�� ������ ������ ���·� ��ȯ
            var nowDist = Vector3.SqrMagnitude(_interactionTargetList[i].transform.position - transform.position);
            if (nowDist < minDist)
            {
                minDist = nowDist;
                minIndex = i;
            }
        }

        // �ĺ� ����Ʈ �߿��� ��ȣ�ۿ� Ÿ���� ���� ���
        if (minIndex == -1) return;

        // �ߺ��� �ƴ϶�� �ִ´�
        if (_interactionTargetList[minIndex] != ClosetTarget)
        {
            ChangeClosetTarget(_interactionTargetList[minIndex]);
        }
    }
    /// <summary>
    /// ���� ��ȣ�ۿ� ���� ������Ʈ�� ����
    /// </summary>
    /// <param name="newTarget"></param>
    public void ChangeClosetTarget(InteractableObject newTarget)
    {
        if (newTarget == null) return;
        if (ClosetTarget == newTarget) return;

        ClosetTarget = newTarget;
        _interactionMarker.EnableMarker(ClosetTarget);
    }
}
