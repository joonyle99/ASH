using UnityEngine;

/// <summary>
/// ��ȭ ��ȣ�ۿ��� ������ Non-Player Character
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] private DialogueData _data;
    [SerializeField] private Quest _quest;

    protected override void OnInteract()
    {
        // ��ȭâ�� ����Ʈ ������ �����Ѵ�
        _data.LinkQuest(_quest);

        // ��ȣ�ۿ��� ���۵� ���� ��ȭâ�� ����
        DialogueController.Instance.StartDialogue(_data);
    }
    public override void UpdateInteracting()
    {
        if (!DialogueController.Instance.IsDialogueActive)
        {
            // ��ȣ�ۿ� ����
            ExitInteraction();
        }
    }
}
