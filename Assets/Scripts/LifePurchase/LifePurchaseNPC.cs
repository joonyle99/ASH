using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePurchaseNPC : InteractableObject
{
    [SerializeField] CutscenePlayer _cutscenePlayer;
    protected override void OnObjectInteractionEnter()
    {
        _cutscenePlayer.Play();
    }
    public override void UpdateInteracting()
    {
        if (_cutscenePlayer.IsPlaying)
        {
            ExitInteraction();
        }
    }
}
