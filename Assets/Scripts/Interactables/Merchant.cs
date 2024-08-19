using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class DialogueDictionary
{
    public string Key;
    public DialogueData Value;

    public DialogueDictionary(string name, DialogueData dialogueData)
    {
        Key = name;
        Value = dialogueData;
    }
}

public class Merchant : InteractableObject
{
    [Header("Merchant")]
    [Space]

    [SerializeField] private List<DialogueDictionary> _dialogueCollection = new List<DialogueDictionary>();
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

        // ����Ʈ Ȱ��ȭ ���� (���� ��)
        if (_questData.IsActive)
        {
            // ����Ʈ �Ϸ�
            if (_questData.IsComplete())
            {
                string completionString = "Completion " + (_questData.CurrentRepeatCount + 1).ToString();
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == completionString).Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                Debug.Log("Start Completion");
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                Debug.Log("End Completion");

                QuestController.Instance.CompleteQuest();
            }
            // ����Ʈ �̿Ϸ�
            else
            {
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "Not Yet Completion").Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                Debug.Log("Start Not Yet Completion");
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                Debug.Log("End Not Yet Completion");
            }
        }
        // ����Ʈ ��Ȱ��ȭ ���� (��� ����)
        else
        {
            // �ִ� �ݺ� Ƚ���� ����
            if (!_questData.IsRepeatable())
            {
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "Final Completion").Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                Debug.Log("Start Final Completion");
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                Debug.Log("End Final Completion");
            }
            // ����Ʈ ù ��� (�ڵ� ����)
            else if (_questData.IsFirst)
            {
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "First Meeting").Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                dialogueData.LinkQuestData(_questData);
                Debug.Log("Start First Meeting");
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                Debug.Log("End First Meeting");
            }
            // ����Ʈ �ι�° ��� (���� / ����)
            else
            {
                /*
                 * ����ؼ� �ּ� ó����
                 * 
                if (!_questData.IsAcceptedBefore)
                {
                    var dialogueData1 = _dialogueCollection.FirstOrDefault(d => d.Key == "Re-request After Rejection").Value;
                    if (CheckInvalid(dialogueData1) == true) yield break;
                    Debug.Log("Start Re-request After Rejection");
                    DialogueController.Instance.StartDialogue(dialogueData1, false, !InteractAtFirst);
                    yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                    Debug.Log("End Re-request After Rejection");
                }
                */

                string requestString = "Re-request " + _questData.CurrentRepeatCount.ToString();
                var dialogueData2 = _dialogueCollection.FirstOrDefault(d => d.Key == requestString).Value;
                if (CheckInvalid(dialogueData2) == true) yield break;
                dialogueData2.LinkQuestData(_questData);
                Debug.Log("Start Re-request");
                DialogueController.Instance.StartDialogue(dialogueData2, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                Debug.Log("End Re-request");

                DialogueData dialogueData3;

                // ���� ��
                if (_questData.IsAcceptedBefore)
                {
                    dialogueData3 = _dialogueCollection.FirstOrDefault(d => d.Key == "Acception").Value;
                    if (CheckInvalid(dialogueData3) == true) yield break;
                    Debug.Log("Start Acception");
                    DialogueController.Instance.StartDialogue(dialogueData3, false);
                    yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                    Debug.Log("End Acception");
                }
                // ���� ��
                else
                {
                    dialogueData3 = _dialogueCollection.FirstOrDefault(d => d.Key == "Rejection").Value;
                    if (CheckInvalid(dialogueData3) == true) yield break;
                    Debug.Log("Start Rejection");
                    DialogueController.Instance.StartDialogue(dialogueData3, false);
                    yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                    Debug.Log("End Rejection");
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

    private bool CheckInvalid<T>(T temp)
    {
        if (temp == null)
        {
            Debug.LogError($"{temp.ToString()} is invalid");
            return true;
        }

        return false;
    }
}
