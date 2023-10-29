using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    // 범위 안의 상호작용한 오브젝트 리스트
    List<InteractableObject> _interactablesInRange = new List<InteractableObject>();
    InteractionMarker _interactionMarker;

    // 상호작용 중인 타겟 오브젝트
    [SerializeField] InteractableObject _interactionTarget = null;

    ContinuousInteractableObject _interactingObject;
    ContinuousInteractableObject InteractingObject
    {
        get
        {
            return _interactingObject;
        }
        set
        {
            if (_interactingObject == value)
                return;
            if (_interactingObject != null)
            {
                _interactingObject.InteractExit();
            }
            _interactingObject = value;
            if (_interactingObject != null)
            {
                _interactingObject.InteractEnter();
            }
        }
    }

    // Set Interaction Key 'E'
    [SerializeField] KeyCode _interactionKey= KeyCode.E;

    bool _isInteracting { get { return _interactingObject != null; } }

    private void Awake()
    {
        _interactionMarker = FindObjectOfType<InteractionMarker>(true);
    }

    public void AddInteractableInRange(InteractableObject interactable)
    {
        _interactablesInRange.Add(interactable);

        // Debug.Log(interactable.gameObject.name);
    }

    public void RemoveInteractableInRange(InteractableObject interactable)
    {
        _interactablesInRange.Remove(interactable);
    }

    public void RelaseInteractingObject()
    {
        InteractingObject = null;
    }

    void ChangeTarget(InteractableObject newTarget)
    {
        if (newTarget == _interactionTarget)
            return;
        _interactionTarget = newTarget;
        if(_interactionTarget == null)
        {
            _interactionMarker.Disable();
        }
        else
        {
            _interactionMarker.EnableAt(newTarget);
        }
    }

    private void Update()
    {
        if (!_isInteracting)
            UpdateInteractionTarget();

        if (_interactionTarget == null)
            return;

        // 여기
        if (_interactionTarget is InstantInteractableObject)
        {
            if (Input.GetKeyDown(_interactionKey))
            {
                (_interactionTarget as InstantInteractableObject).Interact();
            }
        }
        else
        {
            if (Input.GetKeyDown(_interactionKey))
            {
                InteractingObject = _interactionTarget as ContinuousInteractableObject;
            }
            else if (Input.GetKey(_interactionKey) && _isInteracting)
            {
                InteractingObject.InteractUpdate();
            }
            else if(Input.GetKeyUp(_interactionKey))
            {
                InteractingObject = null;
            }
        }
    }

    void UpdateInteractionTarget()
    {
        _interactablesInRange.RemoveAll(x => x == null || !x.IsIsInteractable);
        if (_interactablesInRange.Count == 0)
        {
            ChangeTarget(null);
            return;
        }

        float minDist = Vector3.SqrMagnitude(_interactablesInRange[0].transform.position - transform.position);
        int minIndex = 0;
        for (int i = 1; i < _interactablesInRange.Count; i++)
        {
            float dist = Vector3.SqrMagnitude(_interactablesInRange[i].transform.position - transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                minIndex = i;
            }
        }
        if (_interactablesInRange[minIndex] != _interactionTarget)
            ChangeTarget(_interactablesInRange[minIndex]);
    }
}
