using UnityEngine;

/// <summary>
/// �÷��̾�� ���� ������ ���� �Ϲ����� ��ȭ�� ������ Ŭ����
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] private DialogueData _dialogueData;

    protected override void OnInteract()
    {
        DialogueController.Instance.StartDialogue(_dialogueData);
    }
    public override void UpdateInteracting()
    {
        if (!DialogueController.Instance.IsDialogueActive)
        {
            ExitInteraction();
        }
    }
}
