using System.Collections;
using UnityEngine;

/// <summary>
/// 보스 방 입장의 위한 문
/// 애니메이션이 존재하며 상호작용을 통해 문을 열 수 있다
/// Passage를 제어한다
/// </summary>
public class BossDoor : InteractableObject
{
    [Header("Boss Door")]
    [Space]

    [SerializeField] private bool _isOpened = false;
    [SerializeField] private GameObject _passage;
    [SerializeField] private float _goInDelay = 1f;
    [SerializeField] private InputSetterScriptableObject _enterInputSetter;
    [SerializeField] private DialogueData _failDialogue;                        // 키가 모두 모이지 않았을 때의 대사
    [SerializeField] private SoundList _soundList;

    [Space]

    [SerializeField] private bool _isControlableInputSetter = true;

    private PreserveState _statePreserver;
    private DoorOpenAnimation _doorOpenAnimation;
    private Animator _animator;
    private Collider2D _collider;

    private void Awake()
    {
        _statePreserver = GetComponent<PreserveState>();
        _doorOpenAnimation = GetComponent<DoorOpenAnimation>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }
    private void Start()
    {
        if (_statePreserver?.LoadState("_isOpened", _isOpened) ?? _isOpened)
        {
            _animator.SetTrigger("InstantOpen");

            InitOpening();
        }
        else
        {
            InitClosing();
        }
    }

    // interaction
    protected override void OnObjectInteractionEnter()
    {
        if (BossDungeonManager.Instance.IsAllKeysCollected)
        {
            _soundList.PlaySFX("Open");
            SceneEffectManager.Instance.PushCutscene(new Cutscene(this, OpenDoorCoroutine()));
        }
        else
        {
            DialogueController.Instance.StartDialogue(_failDialogue);
        }
    }
    public override void UpdateInteracting()
    {
        if (BossDungeonManager.Instance.IsAllKeysCollected)
        {

        }
        else
        {
            if (!DialogueController.Instance.IsDialogueActive)
                ExitInteraction();
        }
    }

    // conrol door (open / close)
    private IEnumerator OpenDoorCoroutine()
    {
        if (_isControlableInputSetter) InputManager.Instance.ChangeToStayStillSetter();

        SceneEffectManager.Instance.Camera.StartFollow(transform);
        yield return _doorOpenAnimation.OpenCoroutine();
        SceneEffectManager.Instance.Camera.StartFollow(SceneContext.Current.Player.transform);
        yield return new WaitForSeconds(_goInDelay);

        if (_isControlableInputSetter)
        {
            if (_enterInputSetter) InputManager.Instance.ChangeInputSetter(_enterInputSetter);
            else InputManager.Instance.ChangeToDefaultSetter();
        }

        _isOpened = true;

        if (_statePreserver)
        {
            _statePreserver.SaveState("_isOpened", _isOpened);
        }

        /*
        // yield return _doorOpenAnimation.OpenCoroutine();                                             // 단순히 코루틴을 시작한다 (영화관에서 영화가 끝날 때까지 기다린다)
        // yield return _doorOpenAnimation.StartCoroutine(_doorOpenAnimation.OpenCoroutine());          // 명시적으로 코루틴을 시작한다  (영화관 직원에게 부탁해 영화를 상영하고 영화가 끝날 때까지 기다린다)
        */
    }
    private IEnumerator CloseDoorCoroutine()
    {
        if (_isControlableInputSetter) InputManager.Instance.ChangeToStayStillSetter();

        SceneEffectManager.Instance.Camera.StartFollow(transform);
        yield return _doorOpenAnimation.CloseCoroutine();
        SceneEffectManager.Instance.Camera.StartFollow(SceneContext.Current.Player.transform);
        yield return new WaitForSeconds(_goInDelay);

        if (_isControlableInputSetter)
        {
            if (_enterInputSetter) InputManager.Instance.ChangeInputSetter(_enterInputSetter);
            else InputManager.Instance.ChangeToDefaultSetter();
        }

        _isOpened = false;

        if (_statePreserver)
        {
            _statePreserver.SaveState("_isOpened", _isOpened);
        }
    }
    public void OpenDoor()
    {
        StartCoroutine(OpenDoorCoroutine());
    }
    public void CloseDoor()
    {
        StartCoroutine(CloseDoorCoroutine());
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
}
