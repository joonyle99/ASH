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
    [SerializeField] private List<InteractableObject> _interactionTargetsInRange;

    // ���� ��ȣ�ۿ� ���� ������Ʈ
    [SerializeField] private InteractableObject _interactable;
    public InteractableObject Interactable => _interactable;

    private PlayerBehaviour _player;
    private InteractionMarker _interactionMarker;

    public bool IsReadyForDetection => _interactable == null || !_interactable.IsInteracting; // ��ȣ�ۿ� ����� ã�� ���μ����� �����ؾ� �ϴ� ����

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
        _interactionMarker = FindObjectOfType<InteractionMarker>(true);
    }
    private void Start()
    {
        _interactionTargetsInRange = new List<InteractableObject>();
    }
    private void Update()
    {
        if (IsReadyForDetection)
            PickClosestTarget();
        else
            _interactionMarker.DisableMarker();

        if (_interactable != null)
        {
            // ��ȣ�ۿ� Ű�� �Է¹��� ���
            if (InputManager.Instance.State.InteractionKey.KeyDown)
            {
                // ��ȣ�ۿ� ������ State
                if (_player.CanInteract)
                {
                    OnInteractionStart();
                    _interactable.Interact();
                }
            }

            if (_interactable.IsInteracting)
                _interactable.UpdateInteracting();
        }
        else
        {
            if (_player.CurrentStateIs<InteractionState>())
                OnInteractionExit();
        }
    }
    private void FixedUpdate()
    {
        if (_interactable != null)
        {
            if (_interactable.IsInteracting)
                _interactable.FixedUpdateInteracting();
        }
    }

    /// <summary>
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ�� �߰�
    /// </summary>
    /// <param name="target"></param>
    public void AddInteractionTarget(InteractableObject target)
    {
        if(!_interactionTargetsInRange.Contains(target))
            _interactionTargetsInRange.Add(target);
    }

    /// <summary>
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ���� ����
    /// </summary>
    /// <param name="target"></param>
    public void RemoveInteractionTarget(InteractableObject target)
    {
        if (_interactionTargetsInRange.Contains(target))
            _interactionTargetsInRange.Remove(target);
    }

    /// <summary>
    /// ���� ��ȣ�ۿ� ���� ������Ʈ�� ����
    /// TODO: �ڵ� ������ ������� ���ϴ�. �ٽ� ��������
    /// </summary>
    /// <param name="newTarget"></param>
    void ChangeTarget(InteractableObject newTarget)
    {
        if (newTarget == _interactable)
        {

            if (_interactable == null)
                _interactionMarker.DisableMarker();
            return;
        }

        _interactable = newTarget;

        if (_interactable == null)
            _interactionMarker.DisableMarker();
        else
            _interactionMarker.EnableMarkerAt(newTarget);
    }

    /// <summary>
    /// ��ȣ�ۿ��� ���۵Ǹ� Interaction ���·� ��ȯ�Ѵ�
    /// </summary>
    public void OnInteractionStart()
    {
        // ��� ��ȣ�ۿ��� Interaction State�� ���̵ž� �ϴ� ���� �ƴϴ� (���̾�α׵� ��ȣ�ۿ���)
        if (_interactable.StateChange == InteractionStateChangeType.InteractionState)
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
    /// ��ȣ�ۿ� ������ ������Ʈ �ĺ� ����Ʈ ��, ���� ����� Ÿ���� ����
    /// </summary>
    void PickClosestTarget()
    {
        // ����Ʈ�� null�� �����ϸ� �����Ѵ� (x == null�� ��� ��� x�� ���� *���ٽ�*)
        _interactionTargetsInRange.RemoveAll(x => x == null);

        var minDist = float.MaxValue;
        var minIndex = -1;

        for (int i = 0; i < _interactionTargetsInRange.Count; i++)
        {
            // ��ȣ�ۿ� ��� Ȱ��ȭ ���� �Ǵ�
            if (!_interactionTargetsInRange[i].IsInteractable) continue;

            // ������ ������ ��ư ���� �˰����� ����ϸ� ��� ����� ũ�� ������ ������ ���·� ��ȯ
            var nowDist = Vector3.SqrMagnitude(_interactionTargetsInRange[i].transform.position - transform.position);
            if (nowDist < minDist)
            {
                minDist = nowDist;
                minIndex = i;
            }
        }

        // ��ȣ�ۿ� Ÿ���� ���� ���
        if (minIndex == -1)
        {
            ChangeTarget(null);
            return;
        }

        // ��ȣ�ۿ� ���� Ÿ���� �ߺ��� �ƴ϶��
        if (_interactionTargetsInRange[minIndex] != _interactable)
            ChangeTarget(_interactionTargetsInRange[minIndex]);
    }

}
