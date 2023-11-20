using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class InteractionController : MonoBehaviour
{
    // ���� ���� ��ȣ�ۿ��� ������Ʈ ����Ʈ
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

                    // TODO : PlayerBehaviour �ڵ�� �̵��ؾ���
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
