using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableTree : InteractableObject
{
    [SerializeField] FallingTreeTrunk _treeTrunk;
    [SerializeField] float _interactionOverAngle = 15;
    [SerializeField] float _pushPower = 100;
    [SerializeField] Transform _forcePoint;

    float _moveDirection = 0;
    protected override void OnInteract()
    {
        _moveDirection = Player.RecentDir;
    }

    public override void UpdateInteracting()
    {
        if (IsInteractionKeyUp || IsPlayerStateChanged || _treeTrunk.FallenAngle > _interactionOverAngle)
        {
            if (_treeTrunk.FallenAngle > _interactionOverAngle)
            {
                IsInteractable = false;
            }
            ExitInteraction();
            return;
        }
    }
    public override void FixedUpdateInteracting()
    {
        _treeTrunk.Rigidbody.AddForceAtPosition(new Vector2(_moveDirection, 0) * _pushPower, _forcePoint.position, ForceMode2D.Force);
    }
}