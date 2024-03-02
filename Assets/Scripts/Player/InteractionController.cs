using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    // 범위 안의 상호작용 가능한 오브젝트 후보 리스트
    List<InteractableObject> _interactablesInRange = new List<InteractableObject>();

    InteractionMarker _interactionMarker;
    [SerializeField] InteractableObject _interactionTarget;

    PlayerBehaviour _player;

    public InteractableObject InteractionTarget { get { return _interactionTarget; } }
    bool _shouldDetectInteractable { get { return _interactionTarget == null || !_interactionTarget.IsInteracting; } }      // 상호작용 대상을 찾는 프로세스를 진행해야 하는 상태

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
        _interactionMarker = FindObjectOfType<InteractionMarker>(true);
    }

    public void AddInteractableInRange(InteractableObject interactable)
    {
        _interactablesInRange.Add(interactable);
    }

    public void RemoveInteractableInRange(InteractableObject interactable)
    {
        _interactablesInRange.Remove(interactable);
    }

    void ChangeTarget(InteractableObject newTarget)
    {
        if (newTarget == _interactionTarget)
        {

            if (_interactionTarget == null)
                _interactionMarker.Disable();
            return;
        }

        _interactionTarget = newTarget;

        if (_interactionTarget == null)
            _interactionMarker.Disable();
        else
            _interactionMarker.EnableAt(newTarget);
    }
    public void OnInteractionStart()
    {
        // 모든 상호작용이 Interaction State로 전이돼야 하는 것은 아니다 (다이얼로그도 상호작용임)
        if (_interactionTarget.StateChange == InteractionStateChangeType.InteractionState)
            _player.ChangeState<InteractionState>();
    }
    public void OnInteractionExit()
    {
        if (_player.CurrentStateIs<InteractionState>())
            _player.ChangeState<IdleState>();
    }

    private void Update()
    {
        if (_shouldDetectInteractable)
            SetTargetToClosestInteractable();
        else
            _interactionMarker.Disable();

        if (_interactionTarget != null)
        {
            // 상호작용 키를 입력받은 경우
            if (InputManager.Instance.State.InteractionKey.KeyDown)
            {
                // 상호작용 가능한 State
                if (_player.CanInteract)
                {
                    OnInteractionStart();
                    _interactionTarget.Interact();
                }
            }

            if (_interactionTarget.IsInteracting)
                _interactionTarget.UpdateInteracting();
        }
        else
        {
            if (_player.CurrentStateIs<InteractionState>())
                OnInteractionExit();
        }
    }
    private void FixedUpdate()
    {
        if (_interactionTarget != null)
        {
            if (_interactionTarget.IsInteracting)
                _interactionTarget.FixedUpdateInteracting();
        }
    }

    void SetTargetToClosestInteractable()
    {
        // 리스트의 null을 제거하며 정리한다 (x == null인 경우 모든 x를 제거 *람다식*)
        _interactablesInRange.RemoveAll(x => x == null);

        float minDist = float.MaxValue;
        int minIndex = -1;
        for (int i = 0; i < _interactablesInRange.Count; i++)
        {
            // 상호작용 기능 활성화 여부 판단
            if (!_interactablesInRange[i].IsInteractable)
                continue;

            // 제곱근 연산은 뉴튼 랩슨 알고리즘을 사용하며 계산 비용이 크기 때문에 제곱의 형태로 반환 (비교에는 문제가 없다)
            float dist = Vector3.SqrMagnitude(_interactablesInRange[i].transform.position - transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                minIndex = i;
            }
        }
        if (minIndex == -1)
        {
            ChangeTarget(null);
            return;
        }

        if (_interactablesInRange[minIndex] != _interactionTarget)
            ChangeTarget(_interactablesInRange[minIndex]);
    }

}
