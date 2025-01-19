using System.Collections;
using UnityEngine;

public class LightDoor : LanternLike, ISceneContextBuildListener
{
    public enum State
    {
        Closed, Opening, Opened, Closing
    }

    public State CurrentState { get; private set; } = State.Closed;

    [Header("LightDoor")]
    [Space]

    [SerializeField] private SpriteRenderer _lightStone;

    private Animator _animator;
    private Collider2D _collider;
    private CameraController _cameraController;

    private PreserveState _statePreserver;
    private SceneEffectEvent _recentEvent;

    public override Transform LightPoint => _lightStone.transform;
    public bool IsOpened => CurrentState == State.Opened;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _cameraController = Camera.main.GetComponent<CameraController>();

        _statePreserver = GetComponent<PreserveState>();
    }
    public void OnSceneContextBuilt()
    {
        if (_statePreserver)
        {
            SaveAndLoader.OnSaveStarted += SaveDoorOpenState;

            if (SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                if (_statePreserver.LoadState("_isOpenSaved", false))
                {
                    OpenDoorImmediately();
                }
            }
            else
            {
                if (_statePreserver.LoadState("_opened", false))
                {
                    OpenDoorImmediately();
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        if (_statePreserver)
        {
            if(SceneChangeManager.Instance && SceneChangeManager.Instance.SceneChangeType == SceneChangeType.ChangeMap)
            {
                _statePreserver.SaveState("_opened", CurrentState == State.Opened);
            }

            SaveAndLoader.OnSaveStarted -= SaveDoorOpenState;
        }
    }
    public void Update()
    {
        // CHEAT: F9 키를 누르면 빛의 문 열기
        if (Input.GetKeyDown(KeyCode.F9) && GameSceneManager.Instance.CheatMode == true)
        {
            StartCoroutine(OpenCoroutine());
        }

        if (LanternSceneContext.Current.IsAllRelationsFullyConnected(this))
        {
            IsLightOn = true;
        }
    }

    public override void OnBeamConnected(LightBeam beam)
    {
        StartCoroutine(LightenStoneCoroutine());
    }
    public override void OnBeamDisconnected(LightBeam beam)
    {
        _collider.isTrigger = false;
        _animator.SetTrigger("_Close");
    }
    private IEnumerator LightenStoneCoroutine()
    {
        float eTime = 0f;
        float duration = 0.15f;
        while(eTime < duration)
        {
            _lightStone.color = new Color(1, 1, 1, eTime / duration);
            yield return null;
            eTime += Time.deltaTime;
        }
        _lightStone.color = new Color(1, 1, 1, 1);
    }
    public IEnumerator OpenCoroutine()
    {
        yield return GetComponent<DoorOpenAnimation>().OpenCoroutine();
    }
    public void AnimEvent_OnOpenDone()
    {
        _collider.enabled = false;
        CurrentState = State.Opened;
    }

    private void OpenDoorImmediately()
    {
        _collider.enabled = false;
        CurrentState = State.Opened;
        _animator.SetTrigger("InstantOpen");
    }

    private void SaveDoorOpenState()
    {
        _statePreserver.SaveState("_isOpenSaved", IsOpened);
    }
}
