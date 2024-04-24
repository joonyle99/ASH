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
    [SerializeField] private bool _autoAcceptQuest;

    // ����Ʈ�� �����Ѵٸ� ��ȭâ�� ��� ����ȴٸ� �ڵ����� ����Ʈ�� �����Ѵ�

    protected override void OnInteract()
    {
        // ��ȣ�ۿ��� ���۵� ���� ��ȭâ�� ����
        DialogueController.Instance.StartDialogue(_data);
    }
    public override void UpdateInteracting()
    {
        // ��ȣ�ۿ� ������Ʈ �� ���̾�αװ� ��Ȱ��ȭ �Ǹ� ��ȣ�ۿ��� �����Ѵ�
        if(!DialogueController.Instance.IsDialogueActive)
            ExitInteraction();
    }
}
