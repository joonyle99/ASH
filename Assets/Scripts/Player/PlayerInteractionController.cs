using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 상호작용 가능한 오브젝트를 컨트롤한다
/// </summary>
public class PlayerInteractionController : MonoBehaviour
{
    #region Variable

    [Header("Interaction Controller")]
    [Space]

    [SerializeField] private InteractableObject _closetTarget;                  // 현재 상호작용 중인 오브젝트
    public InteractableObject ClosetTarget
    {
        get => _closetTarget;
        set
        {
            // 3. 플레이어와 오브젝트의 상호작용이 거리가 멀어져 끝난 경우 (feat. InteractionMarker.cs)
            if (_closetTarget != null && value == null)
            {
                // 오브젝트의 상호작용 종료 함수를 호출
                ClosetTarget.ExitInteraction();

                // 상호작용 마커 UI 비활성화
                if (_interactionMarker.IsMarking)
                {
                    _interactionMarker.DisableMarker();
                }
            }

            _closetTarget = value;
        }
    }

    [Space]

    [SerializeField] private List<InteractableObject> _interactionTargetList;               // 범위 안의 상호작용 가능한 오브젝트 리스트

    private PlayerBehaviour _player;
    private InteractionMarker _interactionMarker;

    private bool _canInteract => !DialogueController.Instance.IsDialogueActive;
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
        if (!ClosetTarget || !_canInteract) return;

        var isInteracting = ClosetTarget.IsInteracting;
        var isInteractable = ClosetTarget.IsInteractable;

        // 4. 플레이어가 상호작용을 끝마친 경우 (feat. InteractionMarker.cs)
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
        if (!_canInteract) return;

        if (ClosetTarget)
        {
            if (ClosetTarget.IsInteracting)
                ClosetTarget.FixedUpdateInteracting();
        }
    }

    /// <summary>
    /// 상호작용이 시작되면 Interaction 상태로 전환한다
    /// </summary>
    public void OnPlayerInteractionEnter()
    {
        // 모든 상호작용이 Interaction State로 전이돼야 하는 것은 아니다 (다이얼로그도 상호작용임)
        if (ClosetTarget.StateChange == InteractionStateChangeType.InteractionState)
        {
            _player.ChangeState<InteractionState>();
        }
    }
    /// <summary>
    /// 상호작용이 종료되면 Idle 상태로 전환한다
    /// </summary>
    public void OnPlayerInteractionExit()
    {
        if (_player.CurrentStateIs<InteractionState>())
        {
            _player.ChangeState<IdleState>();
        }
    }

    /// <summary>
    /// 상호작용 가능한 오브젝트 후보 리스트에 추가
    /// </summary>
    /// <param name="target"></param>
    public void AddInteractionTarget(InteractableObject target)
    {
        if (!_interactionTargetList.Contains(target))
        {
            _interactionTargetList.Add(target);

            // 상호작용 타겟이 없는 경우 넣어준다
            if (ClosetTarget == null)
                SetClosetTarget(target);
        }
    }
    /// <summary>
    /// 상호작용 가능한 오브젝트 후보 리스트에서 제거
    /// </summary>
    /// <param name="target"></param>
    public void RemoveInteractionTarget(InteractableObject target)
    {
        if (_interactionTargetList.Contains(target))
        {
            _interactionTargetList.Remove(target);

            // 상호작용 타겟이 같은 경우 제거해준다
            if (ClosetTarget == target)
                ClosetTarget = null;
        }
    }

    /// <summary>
    /// 상호작용 가능한 오브젝트 후보 리스트 중, 가장 가까운 타겟을 고른다
    /// </summary>
    public bool PickClosestTarget()
    {
        InteractableObject newClosetTarget = FindClosestTarget();

        if (newClosetTarget == null) return false;

        // 중복이 아니라면 넣는다
        if (newClosetTarget != ClosetTarget)
        {
            SetClosetTarget(newClosetTarget);
            return true;
        }

        return false;
    }
    /// <summary>
    /// 후보 리스트에서 가장 가까운 타겟을 찾는다
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
    /// 현재 상호작용 중인 오브젝트르 변경
    /// </summary>
    /// <param name="newTarget"></param>
    public void SetClosetTarget(InteractableObject newTarget)
    {
        // 새로운 타겟이 없다면 리턴
        if (newTarget == null) return;
        // 이미 같은 타겟이라면 리턴
        if (ClosetTarget == newTarget) return;

        ClosetTarget = newTarget;

        // 1. 플레이어가 상호작용 가능한 오브젝트의 트리거 박스에 들어간 경우 (feat. InteractionMarker.cs)
        _interactionMarker.EnableMarker(ClosetTarget);
    }

    #endregion
}
