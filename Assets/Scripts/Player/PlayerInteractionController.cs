using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 기준에서 상호작용 가능한 오브젝트를 컨트롤한다
/// </summary>
public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interaction Controller")]
    [Space]

    [SerializeField] private InteractableObject _closetTarget;                  // 현재 상호작용 중인 오브젝트
    public InteractableObject ClosetTarget
    {
        get => _closetTarget;
        set
        {
            // 이벤트 방식
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

    [SerializeField] private List<InteractableObject> _interactionTargetList;               // 범위 안의 상호작용 가능한 오브젝트 리스트

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
        // 상호작용 대상 업데이트
        if (_closetTarget && !_closetTarget.IsInteracting && _interactionTargetList.Count >= 1)
        {
            PickClosestTarget();
        }

        // 상호작용 대상이 있는 경우
        if (ClosetTarget)
        {
            // 이미 상호작용 중이라면
            if (ClosetTarget.IsInteracting)
            {
                // Debug.Log("Interacting");

                // 대상에서 상호작용 업데이트 함수를 돌려준다
                ClosetTarget.UpdateInteracting();
            }
            // 상호작용 중이 아니라면
            else
            {
                // 상호작용 키를 입력받는다면
                if (InputManager.Instance.State.InteractionKey.KeyDown)
                {
                    // Debug.Log("Interact");

                    // 그리고 상호작용 가능한 상태라면
                    if (_player.CanInteract)
                    {
                        // 상호작용 오브젝트에서의 상호작용 로직 실행
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
    /// 상호작용이 시작되면 Interaction 상태로 전환한다
    /// </summary>
    public void OnPlayerInteractionStart()
    {
        // 상호작용 UI 잠시 끄기
        _interactionMarker.DeactivateMarker();

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
        // 상호작용 UI 다시 켜기
        _interactionMarker.ActivateMarker();

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
                ChangeClosetTarget(target);
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
    /// closetTarget에 null이 들어갈 수 있다.
    /// </summary>
    public void PickClosestTarget()
    {
        // 리스트의 null을 제거하며 정리한다 (x == null인 경우 모든 x를 제거 *람다식*)
        // _interactionTargetList.RemoveAll(x => x == null);

        var minDist = float.MaxValue;
        var minIndex = -1;

        // 후보 리스트를 순회
        for (int i = 0; i < _interactionTargetList.Count; i++)
        {
            // 상호작용 기능 활성화 여부 판단
            if (!_interactionTargetList[i].IsInteractable) continue;

            // 제곱근 연산은 뉴튼 랩슨 알고리즘을 사용하며 계산 비용이 크기 때문에 제곱의 형태로 반환
            var nowDist = Vector3.SqrMagnitude(_interactionTargetList[i].transform.position - transform.position);
            if (nowDist < minDist)
            {
                minDist = nowDist;
                minIndex = i;
            }
        }

        // 후보 리스트 중에서 상호작용 타겟이 없는 경우
        if (minIndex == -1) return;

        // 중복이 아니라면 넣는다
        if (_interactionTargetList[minIndex] != ClosetTarget)
        {
            ChangeClosetTarget(_interactionTargetList[minIndex]);
        }
    }
    /// <summary>
    /// 현재 상호작용 중인 오브젝트르 변경
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
