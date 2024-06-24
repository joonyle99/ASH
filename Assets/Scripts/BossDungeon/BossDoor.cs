using System.Collections;
using UnityEngine;

public class BossDoor : InteractableObject
{
    [Header("Boss Door")]
    [Space]

    [SerializeField] private GameObject _passage;
    [SerializeField] private float _goInDelay = 1f;

    [Space]

    [SerializeField] private DialogueData _failDialogue;
    [SerializeField] private InputSetterScriptableObject _enterInputSetter;
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
        _passage.SetActive(false);
    }

    // interaction
    protected override void OnObjectInteractionEnter()
    {
        if (BossDungeonManager.Instance.IsAllKeysCollected)
        {
            _soundList.PlaySFX("Open");
            SceneEffectManager.Instance.PushCutscene(new Cutscene(this, OpenCoroutine()));
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

    // open door
    private IEnumerator OpenCoroutine()
    {
        InputManager.Instance.ChangeToStayStillSetter();

        SceneEffectManager.Instance.Camera.RemoveFollowTarget(SceneContext.Current.Player.transform);
        SceneEffectManager.Instance.Camera.AddFollowTarget(transform);

        yield return _doorOpenAnimation.OpenCoroutine();
        yield return new WaitUntil(() => _passage.activeSelf);
        yield return new WaitForSeconds(_goInDelay);

        InputManager.Instance.ChangeInputSetter(_enterInputSetter);
    }
    public void OpenDoor()
    {
        StartCoroutine(OpenCoroutine()); 
    }

    // anim event
    public void AnimEvent_OnOpenDone()
    {
        SceneEffectManager.Instance.Camera.StopConstantShake();
        _passage.SetActive(true); 
        _collider.enabled = false;
    }
}
