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

    // Merchant�� ���� ������ ����Ʈ�� ���� �� ����
    [SerializeField] private Quest _quest;          // ����ȭ �Ǿ��� ������ null�� �ƴ� ���·� ����
    public Quest Quest => _quest;

    [SerializeField] private List<DialogueDictionary> _dialogueCollection = new List<DialogueDictionary>();

    private Coroutine _interactCoroutine;

    private void Awake()
    {
        _interactCoroutine = null;
    }

    protected override void OnObjectInteractionEnter()
    {
        if (_interactCoroutine != null)
        {
            Debug.LogWarning($"�̹� ��ȣ�ۿ� �ڷ�ƾ�� ���� ���Դϴ�");
            return;
        }

        /*
         * TODO: ����Ʈ �ý��� �ϼ��ϱ�
        Quest currentQuest = QuestController.Instance.CurrentQuest;
        if (currentQuest != null)
            _quest = currentQuest;

        _interactCoroutine = StartCoroutine(OnInteractCoroutine(_quest));
        */
    }

    private IEnumerator OnInteractCoroutine(Quest nowQuest)
    {
        // ����Ʈ Ȱ��ȭ ���� (���� ��)
        if (nowQuest.IsActive)
        {
            // ����Ʈ �Ϸ�
            if (nowQuest.IsComplete())
            {
                string completionString = "Completion " + (nowQuest.CurrentRepeatCount + 1).ToString();
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == completionString).Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);

                QuestController.Instance.CompleteQuest();
            }
            // ����Ʈ �̿Ϸ�
            else
            {
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "Not Yet Completion").Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
            }
        }
        // ����Ʈ ��Ȱ��ȭ ���� (��� ����)
        else
        {
            // �ִ� �ݺ� Ƚ���� ����
            if (!nowQuest.IsRepeatable())
            {
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "Final Completion").Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
            }
            // ����Ʈ ù ��� (�ڵ� ����)
            else if (nowQuest.IsFirst)
            {
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "First Meeting").Value;
                if (CheckInvalid(dialogueData) == true) yield break;

                dialogueData.LinkQuestData(nowQuest);

                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
            }
            // ����Ʈ �ι�° ��� (���� / ����)
            else
            {
                /*
                 * ����ؼ� �ּ� ó����
                 * 
                if (!_quest.IsAcceptedBefore)
                {
                    var dialogueData1 = _dialogueCollection.FirstOrDefault(d => d.Key == "Re-request After Rejection").Value;
                    if (CheckInvalid(dialogueData1) == true) yield break;
                    Debug.Log("Start Re-request After Rejection");
                    DialogueController.Instance.StartDialogue(dialogueData1, false, !InteractAtFirst);
                    yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                    Debug.Log("End Re-request After Rejection");
                }
                */

                string requestString = "Re-request " + nowQuest.CurrentRepeatCount.ToString();
                var dialogueData2 = _dialogueCollection.FirstOrDefault(d => d.Key == requestString).Value;
                if (CheckInvalid(dialogueData2) == true) yield break;

                dialogueData2.LinkQuestData(nowQuest);

                DialogueController.Instance.StartDialogue(dialogueData2, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);

                DialogueData dialogueData3;

                // ���� ��
                if (nowQuest.IsAcceptedBefore)
                {
                    dialogueData3 = _dialogueCollection.FirstOrDefault(d => d.Key == "Acception").Value;
                    if (CheckInvalid(dialogueData3) == true) yield break;
                    DialogueController.Instance.StartDialogue(dialogueData3, false);
                    yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                }
                // ���� ��
                else
                {
                    dialogueData3 = _dialogueCollection.FirstOrDefault(d => d.Key == "Rejection").Value;
                    if (CheckInvalid(dialogueData3) == true) yield break;
                    DialogueController.Instance.StartDialogue(dialogueData3, false);
                    yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                }
            }
        }

        _interactCoroutine = null;
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
