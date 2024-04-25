using UnityEngine;

/// <summary>
/// ��ȭ ��ȣ�ۿ��� ������ Non-Player Character
/// �÷��̾�� ����Ʈ ��û�� �� �� �ִ�.
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] private DialogueData _dialogueData;
    [SerializeField] private QuestData _questData;          // �����ϰ� ����ȭ�� ����Ʈ ������ null�� �� �� ���� ������ ��ȿ�� �˻簡 �ʿ��ϴ�

    protected override void OnInteract()
    {
        // ����Ʈ �����Ͱ� ��ȿ�ϴٸ� ���̾�α� �����Ϳ� ����
        if (_questData.IsValidQuestData())
        {
            Debug.Log($"{gameObject.name}��(��) ������ ����Ʈ �����Ͱ� ��ȿ�մϴ�. ���̾�α׿� �����մϴ�");
            _dialogueData.LinkQuestData(_questData);
        }

        DialogueController.Instance.StartDialogue(_dialogueData);
    }
    public override void UpdateInteracting()
    {
        if (!DialogueController.Instance.IsDialogueActive)
        {
            // ��ȣ�ۿ� ���� ó��
            ExitInteraction();
        }
    }
}
