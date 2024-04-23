using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 기준에서 상호작용 가능한 오브젝트를 컨트롤한다
/// </summary>
public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interaction Controller")]
    [Space]

    // 범위 안의 상호작용 가능한 오브젝트 '후보' 리스트
    [SerializeField] private List<InteractableObject> _interactionTargetList;

    // 가까운 상호작용 가능한 오브젝트
    [SerializeField] private InteractableObject _closetTarget;

    public InteractableObject ClosetTarget
    {
        get => _closetTarget;
        set
        {
            _closetTarget = value;

            // 이벤트 방식
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

    // 가장 가까운 타겟이 없거나 해당 타겟이 상호작용 중이 아니라면
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
    /// 상호작용이 시작되면 Interaction 상태로 전환한다
    /// </summary>
    public void OnPlayerInteractionStart()
    {
        // 모든 상호작용이 Interaction State로 전이돼야 하는 것은 아니다 (다이얼로그도 상호작용임)
        if (_closetTarget.StateChange == InteractionStateChangeType.InteractionState)
            _player.ChangeState<InteractionState>();
    }
    /// <summary>
    /// 상호작용이 종료되면 Idle 상태로 전환한다
    /// </summary>
    public void OnPlayerInteractionExit()
    {
        if (_player.CurrentStateIs<InteractionState>())
            _player.ChangeState<IdleState>();
    }

    /// <summary>
    /// 상호작용 가능한 오브젝트 후보 리스트에 추가
    /// </summary>
    /// <param name="target"></param>
    public void AddInteractionTarget(InteractableObject target)
    {
        if(!_interactionTargetList.Contains(target))
            _interactionTargetList.Add(target);
    }
    /// <summary>
    /// 상호작용 가능한 오브젝트 후보 리스트에서 제거
    /// </summary>
    /// <param name="target"></param>
    public void RemoveInteractionTarget(InteractableObject target)
    {
        if (_interactionTargetList.Contains(target))
            _interactionTargetList.Remove(target);
    }

    void CheckInteraction()
    {
        // 상호작용 대상이 있는 경우
        if (_closetTarget)
        {
            // 상호작용 키를 입력받는다면
            if (InputManager.Instance.State.InteractionKey.KeyDown)
            {
                // 그리고 상호작용 가능한 상태라면
                if (_player.CanInteract)
                {
                    // 상호작용 오브젝트에서의 상호작용 로직 실행
                    _closetTarget.Interact();
                }
            }

            // 이미 상호작용 중이라면
            if (_closetTarget.IsInteracting)
            {
                // 대상에서 상호작용 업데이트 함수를 돌려준다
                _closetTarget.UpdateInteracting();
            }
        }
    }
    /// <summary>
    /// 상호작용 가능한 오브젝트 후보 리스트 중, 가장 가까운 타겟을 고른다
    /// closetTarget에 null이 들어갈 수 있다.
    /// </summary>
    void PickClosestTarget()
    {
        // 리스트의 null을 제거하며 정리한다 (x == null인 경우 모든 x를 제거 *람다식*)
        // _interactionTargetList.RemoveAll(x => x == null);

        var minDist = float.MaxValue;
        var minIndex = -1;

        // 상호작용 후보 리스트를 순회
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

        // 상호작용 타겟이 없는 경우 closetTarget에 null을 넣는다
        if (minIndex == -1)
        {
            ChangeClosetTarget(null);
            return;
        }

        // 상호작용 중이 타겟이 중복이 아니라면
        if (_interactionTargetList[minIndex] != _closetTarget)
            ChangeClosetTarget(_interactionTargetList[minIndex]);
    }
    /// <summary>
    /// 현재 상호작용 중인 오브젝트르 변경
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
