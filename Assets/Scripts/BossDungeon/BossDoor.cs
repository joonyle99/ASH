using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BossDoor : InteractableObject
{
    [SerializeField] GameObject _passage;
    [SerializeField] float _doorOpenDelay;
    [SerializeField] ConstantShakePreset _doorOpenPreset;
    [SerializeField] InputSetterScriptableObject _stayStillInputSetter;
    [SerializeField] InputSetterScriptableObject _enterInputSetter;

    [SerializeField] DialogueData _failDialogue;

    Animator _animator;
    Collider2D _collider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _passage.SetActive(false);
    }
    // Start is called before the first frame update

    protected override void OnInteract()
    {
        if (BossDungeonManager.Instance.AllKeysCollected)
        {
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
        SceneEffectManager.Current.Camera.StartConstantShake(_doorOpenPreset);
        yield return new WaitForSeconds(_doorOpenDelay);
        _animator.SetTrigger("Open");
        yield return new WaitUntil(() => _passage.activeSelf);
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
