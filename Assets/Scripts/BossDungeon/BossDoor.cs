using System.Collections;
using UnityEngine;

/// <summary>
/// ���� �� ������ ���� ��
/// �ִϸ��̼��� �����ϸ� ��ȣ�ۿ��� ���� ���� �� �� �ִ�
/// Passage�� �����Ѵ�
/// </summary>
public class BossDoor : InteractableObject
{
    [Header("Boss Door")]
    [Space]

    [SerializeField] private bool _isOpened = false;
    [SerializeField] private GameObject _passage;
    [SerializeField] private float _goInDelay = 1f;
    [SerializeField] private InputSetterScriptableObject _enterInputSetter;
    [SerializeField] private DialogueData _failDialogue;                        // Ű�� ��� ������ �ʾ��� ���� ���
    [SerializeField] private SoundList _soundList;

    private DoorOpenAnimation _doorOpenAnimation;
    private Animator _animator;
    private Collider2D _collider;

    private void Awake()
    {
        _doorOpenAnimation = GetComponent<DoorOpenAnimation>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }
    private void Start()
    {
        if (_isOpened)
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
        InputManager.Instance.ChangeToStayStillSetter();

        SceneEffectManager.Instance.Camera.RemoveFollowTarget(SceneContext.Current.Player.transform);
        SceneEffectManager.Instance.Camera.AddFollowTarget(transform);

        /*
        // yield return _doorOpenAnimation.OpenCoroutine();                                            // �ܼ��� �ڷ�ƾ�� �����Ѵ� (��ȭ������ ��ȭ�� ���� ������ ��ٸ���)
        // yield return _doorOpenAnimation.StartCoroutine(_doorOpenAnimation.OpenCoroutine());      // ��������� �ڷ�ƾ�� �����Ѵ�  (��ȭ�� �������� ��Ź�� ��ȭ�� ���ϰ� ��ȭ�� ���� ������ ��ٸ���)
        */

        // �ڷ�ƾ ���
        yield return _doorOpenAnimation.OpenCoroutine();
        yield return new WaitForSeconds(_goInDelay);

        InputManager.Instance.ChangeInputSetter(_enterInputSetter);
    }
    private IEnumerator CloseDoorCoroutine()
    {
        // InputManager.Instance.ChangeToStayStillSetter();

        SceneEffectManager.Instance.Camera.FollowOnly(transform);

        yield return _doorOpenAnimation.CloseCoroutine();

        // InputManager.Instance.ChangeToDefaultSetter();
    }
    public void OpenDoor()
    {
        _isOpened = true;
        StartCoroutine(OpenDoorCoroutine()); 
    }
    public void CloseDoor()
    {
        _isOpened = false;
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
