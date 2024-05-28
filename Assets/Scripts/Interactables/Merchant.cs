using System.Collections;
using UnityEngine;

/// <summary>
/// �÷��̾�� ����Ʈ ��û�� �� �� ������, ������ ������ Ŭ����
/// </summary>
public class Merchant : InteractableObject
{
    [Header("Merchant")]
    [Space]

    [SerializeField] private SerializableDictionary<string, DialogueData> _dialogueDataDictionary;

    [Tooltip("you can link questData to dialogueData")]
    [SerializeField] private QuestData _questData;

    protected override void OnObjectInteractionEnter()
    {
        StartCoroutine(OnInteractCoroutine());
    }

    private IEnumerator OnInteractCoroutine()
    {
        if (_questData == null)
        {
            Debug.LogError("Merchant NPC�� ���̾�α׿��� ����Ʈ �����Ͱ� �ʿ��մϴ�. ����Ʈ �����͸� �߰����ּ���.");
            yield break;
        }

        // ����Ʈ �Ϸ� ���μ���
          if (_questData.IsActive)
        {
            if (_questData.IsComplete())
            {
                var dialogueData = _dialogueDataDictionary["Completion"];
                yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData));

                QuestController.Instance.CompleteQuest();
            }
            else
            {
                var dialogueData = _dialogueDataDictionary["Not Yet Completion"];
                yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData));
            }
        }
        // ����Ʈ ��� ���μ���
        else
        {
            if (!_questData.IsRepeatable())
            {
                var dialogueData = _dialogueDataDictionary["Final Completion"];
                yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData));
            }
            // ó�� ���� ��� First Meeting ��ȭ ����� �Բ�, ����Ʈ�� �ش� ���̾�α׿� ��ũ�Ѵ�. (�ڵ� ����)
            else if (_questData.IsFirst)
            {
                var dialogueData = _dialogueDataDictionary["First Meeting"];
                dialogueData.LinkQuestData(_questData);
                yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData));
            }
            // �̹� �Ϸ��� �� �ִ� ��� Re-request ��ȭ ����� �Բ�, ����Ʈ�� �ش� ���̾�α׿� ��ũ�Ѵ�. (���� ��û)
            else
            {
                if (!_questData.IsAcceptedBefore)
                {
                    var dialogueData = _dialogueDataDictionary["Re-request After Rejection"];
                    yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData, true));
                }

                var dialogueData1 = _dialogueDataDictionary["Re-request"];
                dialogueData1.LinkQuestData(_questData);
                yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData1, true));

                DialogueData dialogueData2;

                // ���� ��
                if (_questData.IsAcceptedBefore)
                {
                    dialogueData2 = _dialogueDataDictionary["Acception"];
                    yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData2));
                }
                // ���� ��
                else
                {
                    dialogueData2 = _dialogueDataDictionary["Rejection"];
                    yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData2));
                }
            }
        }
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
