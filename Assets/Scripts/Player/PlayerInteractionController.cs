using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ ��ȣ�ۿ� ������ ������Ʈ�� ��Ʈ���Ѵ�
/// </summary>
public class PlayerInteractionController : MonoBehaviour
{
    #region Variable

    [Header("Interaction Controller")]
    [Space]

    [SerializeField] private InteractableObject _closetTarget;                  // ���� ��ȣ�ۿ� ���� ������Ʈ
    public InteractableObject ClosetTarget
    {
        get => _closetTarget;
        set
        {
            // 3. �÷��̾�� ������Ʈ�� ��ȣ�ۿ��� �Ÿ��� �־��� ���� ��� (feat. InteractionMarker.cs)
            if (_closetTarget != null && value == null)
            {
                // ������Ʈ�� ��ȣ�ۿ� ���� �Լ��� ȣ��
                ClosetTarget.ExitInteraction();

                // ��ȣ�ۿ� ��Ŀ UI ��Ȱ��ȭ
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

    private PlayerBehaviour _player;
    private InteractionMarker _interactionMarker;

    #endregion

    #region Function

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
        if (!ClosetTarget) return;

        var isInteracting = ClosetTarget.IsInteracting;
        var isInteractable = ClosetTarget.IsInteractable;

        // 4. �÷��̾ ��ȣ�ۿ��� ����ģ ��� (feat. InteractionMarker.cs)
        if (!isInteractable)
        {
            ClosetTarget.ExitInteraction();

            if (_interactionMarker.IsMarking)
            {
                _interactionMarker.DisableMarker();
            }

            return;
        }

        if (isInteracting)
        {
            ClosetTarget.UpdateInteracting();
        }
        else
        {
            if (_interactionTargetList.Count >= 2)
            {
                var isPickedNewTarget = PickClosestTarget();

                if (isPickedNewTarget)
                {
                    return;
                }
            }

            if (InputManager.Instance.State.InteractionKey.KeyDown)
            {
                if (_player.CanInteract)
                {
                    ClosetTarget.EnterInteraction();
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
    public void OnPlayerInteractionEnter()
    {
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
                SetClosetTarget(target);
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
    /// </summary>
    public bool PickClosestTarget()
    {
        InteractableObject newClosetTarget = FindClosestTarget();

        if (newClosetTarget == null) return false;

        // �ߺ��� �ƴ϶�� �ִ´�
        if (newClosetTarget != ClosetTarget)
        {
            SetClosetTarget(newClosetTarget);
            return true;
        }

        return false;
    }
    /// <summary>
    /// �ĺ� ����Ʈ���� ���� ����� Ÿ���� ã�´�
    /// </summary>
    /// <returns></returns>
    public InteractableObject FindClosestTarget()
    {
        InteractableObject closestTarget = null;
        var minDist = float.MaxValue;

        foreach (var target in _interactionTargetList)
        {
            if (!target.IsInteractable) continue;

            var sqrDist = (target.transform.position - transform.position).sqrMagnitude;

            if (!(sqrDist < minDist)) continue;

            minDist = sqrDist;
            closestTarget = target;
        }

        return closestTarget;
    }
    /// <summary>
    /// ���� ��ȣ�ۿ� ���� ������Ʈ�� ����
    /// </summary>
    /// <param name="newTarget"></param>
    public void SetClosetTarget(InteractableObject newTarget)
    {
        // ���ο� Ÿ���� ���ٸ� ����
        if (newTarget == null) return;
        // �̹� ���� Ÿ���̶�� ����
        if (ClosetTarget == newTarget) return;

        ClosetTarget = newTarget;

        // 1. �÷��̾ ��ȣ�ۿ� ������ ������Ʈ�� Ʈ���� �ڽ��� �� ��� (feat. InteractionMarker.cs)
        _interactionMarker.EnableMarker(ClosetTarget);
    }

    #endregion
}
