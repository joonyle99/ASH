using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using UnityEngine;

//광선이 연결되는 판정은 해야하는데 랜턴은 아닌 것들
public class LightDoor : LanternLike
{
    public override Transform LightPoint { get { return _lightStone; } }
    [SerializeField] Transform _lightStone;
    [SerializeField] ConstantShakePreset _doorOpenPreset;
    [SerializeField] float _doorOpenDelay;
    Animator _animator;
    Collider2D _collider;
    CameraController _cameraController;
    public enum State
    {
        Closed, Opening, Opened, Closing
    }
    public State CurrentState { get; private set; } = State.Closed;
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _cameraController = Camera.main.GetComponent<CameraController>();
        
    }
    public void Update()
    {
        if (LanternSceneContext.Current.IsAllRelationsFullyConnected(this))
        {
            IsLightOn = true;
        }
    }
    public override void OnBeamConnected(LightBeam beam)
    {
    }
    public override void OnBeamDisconnected(LightBeam beam)
    {
        _collider.isTrigger = false;
        _animator.SetTrigger("Close");
    }
    public void Open()
    {
        Debug.Log("문열림");
        StartCoroutine(OpenCoroutine());
    }
    IEnumerator OpenCoroutine()
    {
        CurrentState = State.Opening;
        _cameraController.StartConstantShake(_doorOpenPreset);
        yield return new WaitForSeconds(_doorOpenDelay);
        _animator.SetTrigger("Open");
    }
    public void AnimEvent_OnOpenDone()
    {
        _collider.isTrigger = true;
        CurrentState = State.Opened;
    }
}
