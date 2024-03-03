using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiningRock : InteractableObject, IAttackListener
{
    [SerializeField] DialogueData _dialogueOnFirstInteract;
    [SerializeField] DialogueData _dialogueOnConsecutiveInteract;
    [SerializeField] DialogueData _dialogueOnFirstAttack;
    [SerializeField] DialogueData _dialogueAfterSecondAttack;
    [SerializeField] CutscenePlayer _cutscenePlayer;
    [SerializeField] int _hp = 3;
    [SerializeField] GameObject _lightNPC;
    [SerializeField] SoundList _soundList;
    bool _hasPlayedDialogue = false;

    bool _hittable = false;
    void Awake()
    {
        _lightNPC.SetActive(false);
    }
    protected override void OnInteract()
    {
        if (!_hasPlayedDialogue)
        {
            DialogueController.Instance.StartDialogue(_dialogueOnFirstInteract);
            _hasPlayedDialogue = true;
        }
        else
        {
            if (_hp == 3)
                DialogueController.Instance.StartDialogue(_dialogueOnConsecutiveInteract);
            else if (_hp == 2)
                DialogueController.Instance.StartDialogue(_dialogueOnConsecutiveInteract);
            else if (_hp == 1)
                DialogueController.Instance.StartDialogue(_dialogueAfterSecondAttack);
        }
    }

    public override void UpdateInteracting()
    {
        if (!DialogueController.Instance.IsDialogueActive)
        {
            ExitInteraction();
            _hasPlayedDialogue = true;
        }
    }
    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        _hp--;
        _soundList.PlaySFX("Hit");
        if (_hp == 2)
        {
            DialogueController.Instance.StartDialogue(_dialogueOnFirstAttack);
        }
        else if (_hp == 0)
        {
            _lightNPC.transform.parent = transform.parent;
            _lightNPC.SetActive(true);
            _cutscenePlayer.Play();
            _soundList.PlaySFX("Die");
            Destruction.Destruct(gameObject);
        }
        return IAttackListener.AttackResult.Success;
    }
}
