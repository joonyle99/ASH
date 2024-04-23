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

        // 상호작용 프로세스
        if (_closetTarget != null)
        {
            // 상호작용 키를 입력받은 경우
            if (InputManager.Instance.State.InteractionKey.KeyDown)
            {
                // 상호작용 가능한 State
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
    /// 상호작용이 시작되면 Interaction 상태로 전환한다
    /// </summary>
    public void OnInteractionStart()
    {
        // 모든 상호작용이 Interaction State로 전이돼야 하는 것은 아니다 (다이얼로그도 상호작용임)
        if (_closetTarget.StateChange == InteractionStateChangeType.InteractionState)
            _player.ChangeState<InteractionState>();
    }
    /// <summary>
    /// 상호작용이 종료되면 Idle 상태로 전환한다
    /// </summary>
    public void OnInteractionExit()
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

    /// <summary>
    /// 상호작용 가능한 오브젝트 후보 리스트 중, 가장 가까운 타겟을 고른다
    /// </summary>
    void PickClosestTarget()
    {
        // 리스트의 null을 제거하며 정리한다 (x == null인 경우 모든 x를 제거 *람다식*)
        // _interactionTargetList.RemoveAll(x => x == null);

        var minDist = float.MaxValue;
        var minIndex = -1;

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
            ChangeTarget(null);
            return;
        }

        // 상호작용 중이 타겟이 중복이 아니라면
        if (_interactionTargetList[minIndex] != _closetTarget)
            ChangeTarget(_interactionTargetList[minIndex]);
    }
    /// <summary>
    /// 현재 상호작용 중인 오브젝트르 변경
    /// TODO: 코드 로직이 깔끔하지 못하다. 다시 수정하자
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
