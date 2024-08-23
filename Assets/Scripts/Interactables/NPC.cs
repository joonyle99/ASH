using System.Collections;
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
    [SerializeField] private DialogueData _responseDialogueData;

    protected override void OnObjectInteractionEnter()
    {
        if(_canSkip == false)
            _canSkip = true;

        DialogueController.Instance.StartDialogue(_dialogueData, false);
    }
    public override void UpdateInteracting()
    {
        if (!DialogueController.Instance.IsDialogueActive)
        {
            if (_responseDialogueData == null) ExitInteraction();
            else StartCoroutine(ExitInteractionAfterResponse());
        }
    }
    private IEnumerator ExitInteractionAfterResponse()
    {
        InputManager.Instance.ChangeToStayStillSetter();
        yield return new WaitForSeconds(1f);
        DialogueController.Instance.StartDialogue(_responseDialogueData, false);
        yield return new WaitUntil(() => !DialogueController.Instance.IsDialogueActive);
        InputManager.Instance.ChangeToDefaultSetter();
        ExitInteraction();
    }
}
