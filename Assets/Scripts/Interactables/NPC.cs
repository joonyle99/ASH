using UnityEngine;

/// <summary>
/// 플레이어에게 정보 전달을 위한 일방적인 대화만 가능한 클래스
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] private bool _canSkip = false;

    [Space]

    [SerializeField] private DialogueData _dialogueData;

    protected override void OnObjectInteractionEnter()
    {
        if(_canSkip == false)
            _canSkip = true;

        DialogueController.Instance.StartDialogue(_dialogueData, false, !InteractAtFirst);
    }
    public override void UpdateInteracting()
    {
        if (!DialogueController.Instance.IsDialogueActive)
        {
            ExitInteraction();
        }
    }
}
