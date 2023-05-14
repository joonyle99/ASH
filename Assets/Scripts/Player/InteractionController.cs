using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public List<InteractableObject> _interactablesInRange = new List<InteractableObject>();


    public InteractableObject _interactionTarget = null;

    InteractionMarker _interactionMarker;

    private void Awake()
    {
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
        UpdateInteractionTarget();
        if(Input.GetKeyDown(KeyCode.Return))
        {
            _interactionTarget?.Interact();
        }
    }

    void UpdateInteractionTarget()
    {
        _interactablesInRange.RemoveAll(x => x == null);
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
