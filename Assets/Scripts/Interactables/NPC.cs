using UnityEngine;

/// <summary>
/// �÷��̾�� ���� ������ ���� �Ϲ����� ��ȭ�� ������ Ŭ����
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
