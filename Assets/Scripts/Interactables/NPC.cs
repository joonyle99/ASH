using UnityEngine;

/// <summary>
/// ��ȣ�ۿ� ������ ������Ʈ ��
/// ��ȭâ�� ���� ����� �߰��� ���� Non-Player Character
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] DialogueData _data;

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
