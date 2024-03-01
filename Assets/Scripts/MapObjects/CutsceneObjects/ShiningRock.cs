using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiningRock : InteractableObject, IAttackListener
{
    [SerializeField] DialogueData _dialogueOnFirstInteract;
    [SerializeField] DialogueData _dialogueOnConsecutiveInteract;
    [SerializeField] DialogueData _dialogueOnFirstAttack;
    [SerializeField] DialogueData _dialogueAfterSecondAttack;

    [SerializeField] int _hp = 3;
    bool _hasPlayedDialogue = false;

    bool _hittable = false;
    protected override void OnInteract()
    {
        if (_hasPlayedDialogue)
            DialogueController.Instance.StartDialogue(_dialogueOnFirstInteract);
        else
        {
            if (_hp == 3)
                DialogueController.Instance.StartDialogue(_dialogueOnFirstInteract);
            else if (_hp == 2)
                DialogueController.Instance.StartDialogue(_dialogueOnFirstAttack);
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
        if (_hp == 2)
        {
            DialogueController.Instance.StartDialogue(_dialogueOnFirstAttack);
        }
        else if (_hp == 0)
        {
            Destroy(gameObject);
        }
        return IAttackListener.AttackResult.Success;
    }
}
