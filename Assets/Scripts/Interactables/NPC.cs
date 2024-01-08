using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : InteractableObject
{
    [SerializeField] DialogueData _data;
    protected override void OnInteract()
    {
        DialogueController.Instance.StartDialogue(_data);
    }
    public override void UpdateInteracting()
    {
        if(!DialogueController.Instance.IsDialogueActive)
        {
            ExitInteraction();
        }
    }
}
