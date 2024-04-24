using UnityEngine;

/// <summary>
/// ��ȭ ��ȣ�ۿ��� ������ Non-Player Character
/// �Ǵ� ����Ʈ�� ����ϴ� ������ �Ѵ�
/// ���̾�α� �����Ϳ� ����Ʈ �����ʹ� ���������� ����ȴ�
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] private DialogueData _data;

    [Space]

    [SerializeField] private bool _autoAcceptQuest;
    [SerializeField] private Quest _quest;

    // ����Ʈ�� �����Ѵٸ� ��ȭâ�� ��� ����ȴٸ� �ڵ����� ����Ʈ�� �����Ѵ�

    protected override void OnInteract()
    {
        // ��ȣ�ۿ��� ���۵� ���� ��ȭâ�� ����
        DialogueController.Instance.StartDialogue(_data);
    }
    public override void UpdateInteracting()
    {
        // ��ȣ�ۿ� ������Ʈ �� ���̾�αװ� ��Ȱ��ȭ �Ǹ� ��ȣ�ۿ��� �����Ѵ�
        if (!DialogueController.Instance.IsDialogueActive)
        {
            // TODO: ����Ʈ�� �����Ѵٸ� ����Ʈ�� ����Ѵ�

            // ���� ���θ� üũ�Ѵ�

            // ��ȣ�ۿ� ����
            ExitInteraction();
        }
    }
}
