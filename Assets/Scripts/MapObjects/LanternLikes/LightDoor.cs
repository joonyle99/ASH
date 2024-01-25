using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using UnityEngine;

public class LightDoor : LanternLike
{
    public override Transform LightPoint { get { return _lightStone.transform; } }
    [SerializeField] SpriteRenderer _lightStone;
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

    

    PreserveState _statePreserver;


    SceneEffectEvent _recentEvent;
    private void OnDestroy()
    {
        if (_statePreserver)
            _statePreserver.Save("opened", CurrentState == State.Opened);
    }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _cameraController = Camera.main.GetComponent<CameraController>();
        _statePreserver = GetComponent<PreserveState>();
        if (_statePreserver && _statePreserver.Load("opened", false))
        {
            _collider.isTrigger = true;
            CurrentState = State.Opened;
            _animator.SetTrigger("InstantOpen");
        }

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
        StartCoroutine(LightenStoneCoroutine());
    }

    IEnumerator LightenStoneCoroutine()
    {
        float eTime = 0f;
        float duration = 0.3f;
        while(eTime < duration)
        {
            _lightStone.color = new Color(1, 1, 1, eTime / duration);
            yield return null;
            eTime += Time.deltaTime;
        }
        _lightStone.color = new Color(1, 1, 1, 1);
    }
    public override void OnBeamDisconnected(LightBeam beam)
    {
        _collider.isTrigger = false;
        _animator.SetTrigger("Close");
    }
    public void Open()
    {
        Debug.Log("¹®¿­¸²");
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
        _collider.enabled = false;
        CurrentState = State.Opened;
    }
}
