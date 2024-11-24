using System.Collections;
using UnityEngine;

/// <summary>
/// 애니메이션이 연출이 들어간 특별한 문으로 Passage를 제어한다
/// </summary>
public class BossDoor : Door
{
    [Header("Boss Door")]
    [Space]

    [SerializeField] private GameObject _passage;
    [SerializeField] private DialogueData _failDialogue;                        // 문 열기 실패 대사
    [SerializeField] private DialogueData _successDialogue;                     // 문 열기 성공 대사

    [Space] private float _waitTime = 2f;

    protected PreserveState _statePreserver;

    public override bool IsOpened
    {
        get => _isOpened;
        set
        {
            _isOpened = value;

            if (_statePreserver)
            {
                _statePreserver.SaveState("_isOpened", IsOpened);
            }
        }
    }

    public override bool CanOpenDoor
    {
        get => BossDungeonManager.Instance.IsAllKeysCollected;
    }

    protected override void Awake()
    {
        base.Awake();
        _statePreserver = GetComponent<PreserveState>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SaveAndLoader.OnSaveStarted -= SaveDoorState;
    }

    public override void OnSceneContextBuilt()
    {
        if(_statePreserver)
        {
            if(SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                if(IsOpened = _statePreserver.LoadState<bool>("_isOpenSaved", IsOpened))
                {
                    _animator.SetTrigger("InstantOpen");

                    InitOpening();
                }
                else
                {
                    InitClosing();
                }
            }else
            {
                if (IsOpened = _statePreserver.LoadState<bool>("_isOpened", IsOpened))
                {
                    _animator.SetTrigger("InstantOpen");

                    InitOpening();
                }
                else
                {
                    InitClosing();
                }
            }
        }

        SaveAndLoader.OnSaveStarted += SaveDoorState;
    }

    // interaction
    protected override void OnObjectInteractionEnter()
    {
        if (CanOpenDoor)
        {
            //_soundList.PlaySFX("Open"); 사운드 플레이는 DoorOpenAnimation에서 수행

            StartCoroutine(SceneEffectManager.Instance.PushCutscene(new Cutscene(this, OpenDoorCoroutine())));
        }
        else
        {
            DialogueController.Instance.StartDialogue(_failDialogue, false);
        }
    }

    public override void UpdateInteracting()
    {
        if (CanOpenDoor)
        {

        }
        else
        {
            if (DialogueController.Instance.IsDialogueActive == false)
            {
                ExitInteraction();
            }
        }
    }

    // control door (open / close)
    protected override IEnumerator OpenDoorCoroutine(bool useCameraEffect = true)
    {
        // 상호작용으로 여는 경우에만 InputSetter를 변경한다
        if (IsInteractable)
            InputManager.Instance.ChangeToStayStillSetter();
        
        // Debug.Log("Call OpenDoor Coroutine");

        yield return StartCoroutine(base.OpenDoorCoroutine(useCameraEffect));

        /*
         * TODO: 보스와 싸우다 중간에 죽으면 보스키가 소모된 상태이므로 문이 열리지 않는 버그 존재
        // 열쇠를 소모한다
        if (BossDungeonManager.Instance.IsAllKeysCollected)
        {
            BossDungeonManager.Instance.OnOpenBossDoor();
        }
        */

        yield return new WaitForSeconds(_waitTime);

        // 성공 다이얼로그 실행
        if (_successDialogue != null)
        {
            DialogueController.Instance.StartDialogue(_successDialogue, false);
            yield return new WaitUntil(() => DialogueController.Instance.IsDialogueActive == false);
        }

        // 상호작용으로 여는 경우에만 InputSetter를 변경한다
        if (IsInteractable)
        {
            if (_enterInputSetter)
                InputManager.Instance.ChangeInputSetter(_enterInputSetter);
            else
                InputManager.Instance.ChangeToDefaultSetter();
        }

        /*
        // yield return _doorOpenAnimation.OpenCoroutine();                                             // 단순히 코루틴을 시작한다 (영화관에서 영화가 끝날 때까지 기다린다)
        // yield return _doorOpenAnimation.StartCoroutine(_doorOpenAnimation.OpenCoroutine());          // 명시적으로 코루틴을 시작한다  (영화관 직원에게 부탁해 영화를 상영하고 영화가 끝날 때까지 기다린다)
        */
    }

    public override void OpenDoor(bool useCameraEffect = true)
    {
        StartCoroutine(OpenDoorCoroutine(useCameraEffect));
    }

    protected override IEnumerator CloseDoorCoroutine(bool useCameraEffect)
    {
        if (IsInteractable)
            InputManager.Instance.ChangeToStayStillSetter();

        yield return StartCoroutine(base.CloseDoorCoroutine(useCameraEffect));

        IsOpened = false;

        if (IsInteractable)
        {
            if (_enterInputSetter)
                InputManager.Instance.ChangeInputSetter(_enterInputSetter);
            else
                InputManager.Instance.ChangeToDefaultSetter();
        }
    }

    public override void CloseDoor(bool useCameraEffect = true)
    {
        StartCoroutine(CloseDoorCoroutine(useCameraEffect));
    }

    // anim event
    public void AnimEvent_OnOpenDone()
    {
        InitOpening();
    }
    public void AnimEvent_OnCloseDone()
    {
        InitClosing();
    }

    private void InitOpening()
    {
        if (_passage)
        {
            _passage.SetActive(true);
        }
        _collider.enabled = false;
    }

    private void InitClosing()
    {
        if (_passage)
        {
            _passage.SetActive(false);
        }
        _collider.enabled = true;
    }

    protected void SaveDoorState()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState<bool>("_isOpenSaved", IsOpened);
        }
    }
}
