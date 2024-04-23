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

    public InteractableObject ClosetTarget
    {
        get => _closetTarget;
        set
        {
            _closetTarget = value;

            // �̺�Ʈ ���
            if (_closetTarget == null)
            {
                if (_interactionMarker.IsMarking)
                {
                    _interactionMarker.DisableMarker();
                }
            }
        }
    }

    private PlayerBehaviour _player;
    private InteractionMarker _interactionMarker;

    // ���� ����� Ÿ���� ���ų� �ش� Ÿ���� ��ȣ�ۿ� ���� �ƴ϶��
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
        if (IsReadyForPicking)
            PickClosestTarget();

        CheckInteraction();
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
    public void OnPlayerInteractionStart()
    {
        // ��� ��ȣ�ۿ��� Interaction State�� ���̵ž� �ϴ� ���� �ƴϴ� (���̾�α׵� ��ȣ�ۿ���)
        if (_closetTarget.StateChange == InteractionStateChangeType.InteractionState)
            _player.ChangeState<InteractionState>();
    }
    /// <summary>
    /// ��ȣ�ۿ��� ����Ǹ� Idle ���·� ��ȯ�Ѵ�
    /// </summary>
    public void OnPlayerInteractionExit()
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

    void CheckInteraction()
    {
        // ��ȣ�ۿ� ����� �ִ� ���
        if (_closetTarget)
        {
            // ��ȣ�ۿ� Ű�� �Է¹޴´ٸ�
            if (InputManager.Instance.State.InteractionKey.KeyDown)
            {
                // �׸��� ��ȣ�ۿ� ������ ���¶��
                if (_player.CanInteract)
                {
                    // ��ȣ�ۿ� ������Ʈ������ ��ȣ�ۿ� ���� ����
                    _closetTarget.Interact();
                }
            }

            // �̹� ��ȣ�ۿ� ���̶��
            if (_closetTarget.IsInteracting)
            {
                // ��󿡼� ��ȣ�ۿ� ������Ʈ �Լ��� �����ش�
                _closetTarget.UpdateInteracting();
            }
        }
    }
    /// <summary>
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ ��, ���� ����� Ÿ���� ����
    /// closetTarget�� null�� �� �� �ִ�.
    /// </summary>
    void PickClosestTarget()
    {
        // ����Ʈ�� null�� �����ϸ� �����Ѵ� (x == null�� ��� ��� x�� ���� *���ٽ�*)
        // _interactionTargetList.RemoveAll(x => x == null);

        var minDist = float.MaxValue;
        var minIndex = -1;

        // ��ȣ�ۿ� �ĺ� ����Ʈ�� ��ȸ
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
            ChangeClosetTarget(null);
            return;
        }

        // ��ȣ�ۿ� ���� Ÿ���� �ߺ��� �ƴ϶��
        if (_interactionTargetList[minIndex] != _closetTarget)
            ChangeClosetTarget(_interactionTargetList[minIndex]);
    }
    /// <summary>
    /// ���� ��ȣ�ۿ� ���� ������Ʈ�� ����
    /// </summary>
    /// <param name="newTarget"></param>
    void ChangeClosetTarget(InteractableObject newTarget)
    {
        if (_closetTarget == newTarget) return;

        // null or object
        _closetTarget = newTarget;

        if(_closetTarget) _interactionMarker.EnableMarkerAt(_closetTarget);
    }

}
