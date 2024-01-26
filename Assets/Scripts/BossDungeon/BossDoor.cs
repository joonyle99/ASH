using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BossDoor : InteractableObject
{
    [SerializeField] GameObject _passage;
    [SerializeField] float _goInDelay = 1f;
    [SerializeField] InputSetterScriptableObject _stayStillInputSetter;
    [SerializeField] InputSetterScriptableObject _enterInputSetter;

    [SerializeField] SoundList _soundList;

    [SerializeField] DialogueData _failDialogue;

    DoorOpenAnimation _doorOpenAnimation;

    Animator _animator;
    Collider2D _collider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _doorOpenAnimation = GetComponent<DoorOpenAnimation>();
        _passage.SetActive(false);
    }
    // Start is called before the first frame update

    protected override void OnInteract()
    {
        if (BossDungeonManager.Instance.AllKeysCollected)
        {
            _soundList.PlaySFX("Open");
            SceneEffectManager.Current.PushCutscene(new Cutscene(this, OpenCoroutine()));
        }
        else
        {
            DialogueController.Instance.StartDialogue(_failDialogue);
        }
    }
    public override void UpdateInteracting()
    {
        if (BossDungeonManager.Instance.AllKeysCollected)
        {

        }
        else
        {
            if (!DialogueController.Instance.IsDialogueActive)
                ExitInteraction();
        }
    }
    IEnumerator OpenCoroutine()
    {
        InputManager.Instance.ChangeInputSetter(_stayStillInputSetter);
        SceneEffectManager.Current.Camera.RemoveFollowTarget(SceneContext.Current.Player.transform);
        SceneEffectManager.Current.Camera.AddFollowTarget(transform);
        yield return _doorOpenAnimation.OpenCoroutine();
        yield return new WaitUntil(() => _passage.activeSelf);
        yield return new WaitForSeconds(_goInDelay);
        InputManager.Instance.ChangeInputSetter(_enterInputSetter);
    }
    void OpenDoor()
    {
        StartCoroutine(OpenCoroutine()); 
    }
    public void AnimEvent_OnOpenDone()
    {
        SceneEffectManager.Current.Camera.StopConstantShake();
        _passage.SetActive(true); 
        _collider.enabled = false;
    }
}
