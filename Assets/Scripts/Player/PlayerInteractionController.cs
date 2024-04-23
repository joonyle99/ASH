using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� ���ؿ��� ��ȣ�ۿ� ������ ������Ʈ�� ��Ʈ���Ѵ�
/// </summary>
public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interaction Controller")]
    [Space]

    // ���� ���� ��ȣ�ۿ� ������ ������Ʈ '�ĺ�' ����Ʈ
    [SerializeField] private List<InteractableObject> _interactionTargetList;

    // ����� ��ȣ�ۿ� ������ ������Ʈ
    [SerializeField] private InteractableObject _closetTarget;
    public InteractableObject ClosetTarget => _closetTarget;

    private PlayerBehaviour _player;
    private InteractionMarker _interactionMarker;

    private bool IsReadyForPicking => _closetTarget == null || !_closetTarget.IsInteracting;

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
        if (IsReadyForPicking) PickClosestTarget();
        else _interactionMarker.DisableMarker();

        // ��ȣ�ۿ� ���μ���
        if (_closetTarget != null)
        {
            // ��ȣ�ۿ� Ű�� �Է¹��� ���
            if (InputManager.Instance.State.InteractionKey.KeyDown)
            {
                // ��ȣ�ۿ� ������ State
                if (_player.CanInteract)
                {
                    OnInteractionStart();
                    _closetTarget.Interact();
                }
            }

            if (_closetTarget.IsInteracting)
                _closetTarget.UpdateInteracting();
        }
        else
        {
            if (_player.CurrentStateIs<InteractionState>())
                OnInteractionExit();
        }
    }
    private void FixedUpdate()
    {
        if (_closetTarget != null)
        {
            if (_closetTarget.IsInteracting)
                _closetTarget.FixedUpdateInteracting();
        }
    }

    /// <summary>
    /// ��ȣ�ۿ��� ���۵Ǹ� Interaction ���·� ��ȯ�Ѵ�
    /// </summary>
    public void OnInteractionStart()
    {
        // ��� ��ȣ�ۿ��� Interaction State�� ���̵ž� �ϴ� ���� �ƴϴ� (���̾�α׵� ��ȣ�ۿ���)
        if (_closetTarget.StateChange == InteractionStateChangeType.InteractionState)
            _player.ChangeState<InteractionState>();
    }
    /// <summary>
    /// ��ȣ�ۿ��� ����Ǹ� Idle ���·� ��ȯ�Ѵ�
    /// </summary>
    public void OnInteractionExit()
    {
        if (_player.CurrentStateIs<InteractionState>())
            _player.ChangeState<IdleState>();
    }

    /// <summary>
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ�� �߰�
    /// </summary>
    /// <param name="target"></param>
    public void AddInteractionTarget(InteractableObject target)
    {
        if(!_interactionTargetList.Contains(target))
            _interactionTargetList.Add(target);
    }
    /// <summary>
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ���� ����
    /// </summary>
    /// <param name="target"></param>
    public void RemoveInteractionTarget(InteractableObject target)
    {
        if (_interactionTargetList.Contains(target))
            _interactionTargetList.Remove(target);
    }

    /// <summary>
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ ��, ���� ����� Ÿ���� ����
    /// </summary>
    void PickClosestTarget()
    {
        // ����Ʈ�� null�� �����ϸ� �����Ѵ� (x == null�� ��� ��� x�� ���� *���ٽ�*)
        // _interactionTargetList.RemoveAll(x => x == null);

        var minDist = float.MaxValue;
        var minIndex = -1;

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

        // ��ȣ�ۿ� Ÿ���� ���� ��� closetTarget�� null�� �ִ´�
        if (minIndex == -1)
        {
            ChangeTarget(null);
            return;
        }

        // ��ȣ�ۿ� ���� Ÿ���� �ߺ��� �ƴ϶��
        if (_interactionTargetList[minIndex] != _closetTarget)
            ChangeTarget(_interactionTargetList[minIndex]);
    }
    /// <summary>
    /// ���� ��ȣ�ۿ� ���� ������Ʈ�� ����
    /// TODO: �ڵ� ������ ������� ���ϴ�. �ٽ� ��������
    /// </summary>
    /// <param name="newTarget"></param>
    void ChangeTarget(InteractableObject newTarget)
    {
        if (_closetTarget == newTarget)
        {
            if (_closetTarget == null)
                _interactionMarker.DisableMarker();

            return;
        }

        _closetTarget = newTarget;

        if (_closetTarget == null) _interactionMarker.DisableMarker();
        else _interactionMarker.EnableMarkerAt(newTarget);
    }

}
