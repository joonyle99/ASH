using System.Collections;
using UnityEngine;

/// <summary>
/// �ִϸ��̼��� ������ �� Ư���� ������ Passage�� �����Ѵ�
/// </summary>
public class BossDoor : Door
{
    [Header("Boss Door")]
    [Space]

    [SerializeField] private GameObject _passage;
    [SerializeField] private DialogueData _failDialogue;                        // �� ���� ���� ���
    [SerializeField] private DialogueData _successDialogue;                     // �� ���� ���� ���

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
            //_soundList.PlaySFX("Open"); ���� �÷��̴� DoorOpenAnimation���� ����

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
        // ��ȣ�ۿ����� ���� ��쿡�� InputSetter�� �����Ѵ�
        if (IsInteractable)
            InputManager.Instance.ChangeToStayStillSetter();
        
        // Debug.Log("Call OpenDoor Coroutine");

        yield return StartCoroutine(base.OpenDoorCoroutine(useCameraEffect));

        /*
         * TODO: ������ �ο�� �߰��� ������ ����Ű�� �Ҹ�� �����̹Ƿ� ���� ������ �ʴ� ���� ����
        // ���踦 �Ҹ��Ѵ�
        if (BossDungeonManager.Instance.IsAllKeysCollected)
        {
            BossDungeonManager.Instance.OnOpenBossDoor();
        }
        */

        yield return new WaitForSeconds(_waitTime);

        // ���� ���̾�α� ����
        if (_successDialogue != null)
        {
            DialogueController.Instance.StartDialogue(_successDialogue, false);
            yield return new WaitUntil(() => DialogueController.Instance.IsDialogueActive == false);
        }

        // ��ȣ�ۿ����� ���� ��쿡�� InputSetter�� �����Ѵ�
        if (IsInteractable)
        {
            if (_enterInputSetter)
                InputManager.Instance.ChangeInputSetter(_enterInputSetter);
            else
                InputManager.Instance.ChangeToDefaultSetter();
        }

        /*
        // yield return _doorOpenAnimation.OpenCoroutine();                                             // �ܼ��� �ڷ�ƾ�� �����Ѵ� (��ȭ������ ��ȭ�� ���� ������ ��ٸ���)
        // yield return _doorOpenAnimation.StartCoroutine(_doorOpenAnimation.OpenCoroutine());          // ��������� �ڷ�ƾ�� �����Ѵ�  (��ȭ�� �������� ��Ź�� ��ȭ�� ���ϰ� ��ȭ�� ���� ������ ��ٸ���)
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
