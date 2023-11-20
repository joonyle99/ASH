using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class InteractionController : MonoBehaviour
{
    // 범위 안의 상호작용한 오브젝트 리스트
    List<InteractableObject> _interactablesInRange = new List<InteractableObject>();
    InteractionMarker _interactionMarker;
    PlayerBehaviour _player;
    InteractableObject _interactionTarget = null;


    public InteractableObject InteractionTarget { get { return _interactionTarget; } }
    bool _shouldDetectInteractable { get { return _interactionTarget == null || !_interactionTarget.IsInteracting; } }

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
            return;

        _interactionTarget = newTarget;

        if(_interactionTarget == null)
            _interactionMarker.Disable();
        else
            _interactionMarker.EnableAt(newTarget);
    }

    private void Update()
    {
        if (_shouldDetectInteractable)
            SetTargetToClosestInteractable();
        else
            _interactionMarker.Disable();

        if (_interactionTarget != null)
        {
            if (InputManager.Instance.InteractionKey.KeyDown)
            {
                if (_player.IsInteractable)
                {
                    _interactionTarget.Interact();

                    // TODO : PlayerBehaviour 코드로 이동해야함
                    _player.ChangeState<InteractionState>();
                    _player.Animator.SetTrigger("Interact");

                }
            }    
            if (_interactionTarget.IsInteracting)
                _interactionTarget.UpdateInteracting();
        }
    }

    void SetTargetToClosestInteractable()
    {
        _interactablesInRange.RemoveAll(x => x == null);

        float minDist = float.MaxValue;
        int minIndex = -1;
        for (int i = 0; i < _interactablesInRange.Count; i++)
        {
            if (!_interactablesInRange[i].IsInteractable)
                continue;
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
