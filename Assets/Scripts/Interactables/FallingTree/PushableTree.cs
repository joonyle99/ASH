using UnityEngine;

public class PushableTree : InteractableObject
{
    [SerializeField] private FallingTreeTrunk _treeTrunk;
    [SerializeField] private float _interactionOverAngle = 15;
    [SerializeField] private float _pushPower = 100;
    [SerializeField] private Transform _forcePoint;

    private float _moveDirection = 0;

    public bool IsFallen => _treeTrunk.PushedAngle > _interactionOverAngle;

    protected override void OnInteract()
    {
        _moveDirection = Player.RecentDir;
    }
    private void Update()
    {
        if (IsInteractable)
        {
            if (IsFallen)
            {
                IsInteractable = false;
            }
        }
    }
    public override void UpdateInteracting()
    {
        if (IsInteractionKeyUp || !IsPlayerInteractionState || IsFallen)
        {
            ExitInteraction();
        }
    }
    public override void FixedUpdateInteracting()
    {
        if (IsPlayerIsDirSync)
            _treeTrunk.Rigidbody.AddForceAtPosition(new Vector2(_moveDirection, 0) * _pushPower, _forcePoint.position, ForceMode2D.Force);
    }
}