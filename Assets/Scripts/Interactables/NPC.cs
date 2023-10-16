using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : InstantInteractableObject
{
    [SerializeField] DialogueData _data;
    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(_data);
    }
}
