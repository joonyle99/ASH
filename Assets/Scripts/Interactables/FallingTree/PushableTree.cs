using UnityEngine;

public class PushableTree : InteractableObject
{
    [SerializeField] private FallingTreeTrunk _treeTrunk;
    [SerializeField] private float _interactionOverAngle = 15;
    [SerializeField] private float _pushPower = 100;
    [SerializeField] private Transform _forcePoint;

    private float _moveDirection = 0;
    private PreserveState _statePreserver;

    public bool IsFallen => _treeTrunk.PushedAngle > _interactionOverAngle;

    private void Awake()
    {
        // Debug.Log("pushable tree awake");

        _statePreserver = GetComponent<PreserveState>();

        if (_statePreserver)
        {
            bool isInteractable = _statePreserver.LoadState("_isInteractable", IsInteractable);
            if (isInteractable)
            {
                IsInteractable = true;
            }

            var treeTransform = new TransformState(_treeTrunk.transform);
            //저장 시점의 데이터를 불러오는 경우
            if(SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                var newTreeTransform = _statePreserver.LoadState("_FallingTreeTransformSaved", treeTransform);
                _treeTrunk.transform.localPosition = newTreeTransform.Position;
                _treeTrunk.transform.localRotation = newTreeTransform.Rotation;
                _treeTrunk.transform.localScale = newTreeTransform.Scale;
            }
            else
            {
                var newTreeTransform = _statePreserver.LoadState("_isFallingTreeTransform", treeTransform);
                _treeTrunk.transform.localPosition = newTreeTransform.Position;
                _treeTrunk.transform.localRotation = newTreeTransform.Rotation;
                _treeTrunk.transform.localScale = newTreeTransform.Scale;
            }

            SaveAndLoader.OnSaveStarted += SaveFallingTreeState;
        }
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

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_statePreserver)
        {
            if(SceneChangeManager.Instance.SceneChangeType == SceneChangeType.ChangeMap)
            {
                _statePreserver.SaveState("_isInteractable", IsInteractable);

                _statePreserver.SaveState("_isFallingTreeTransform", new TransformState(_treeTrunk.transform));
            }

            SaveAndLoader.OnSaveStarted -= SaveFallingTreeState;
        }
    }

    protected override void OnObjectInteractionEnter()
    {
        _moveDirection = Player.RecentDir;
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

    private void SaveFallingTreeState()
    {
        if (_statePreserver)
        {
            // falling tree의 데이터를 저장한다.
            _statePreserver.SaveState("_FallingTreeTransformSaved", new TransformState(_treeTrunk.transform));
        }
    }
}