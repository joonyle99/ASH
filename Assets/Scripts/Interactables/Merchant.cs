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

    [SerializeField] private Quest _quest;
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
            Debug.LogWarning($"이미 상호작용 코루틴이 실행 중입니다");
            return;
        }

        Quest currentQuest = QuestController.Instance.CurrentQuest;
        if (currentQuest != null && currentQuest.QuestData != null && currentQuest.IsActive == true)
        {
            _quest = currentQuest;
        }

        _interactCoroutine = StartCoroutine(OnInteractCoroutine(_quest));
    }

    private IEnumerator OnInteractCoroutine(Quest nowQuest)
    {
        Debug.Log("Quest argument in Merchant 'OnInteractCoroutine()' Function : " + nowQuest.QuestData);

        // 퀘스트 활성화 상태 (진행 중)
        if (nowQuest.IsActive)
        {
            // 퀘스트 완료
            if (nowQuest.IsComplete())
            {
                Debug.Log("Complete Quest");
                string completionString = "Completion " + (nowQuest.CurrentRepeatCount + 1).ToString();
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == completionString).Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);

                QuestController.Instance.CompleteQuest();
            }
            // 퀘스트 미완료
            else
            {
                Debug.Log("Not Succeeded Quest");
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "Not Yet Completion").Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
            }
        }
        // 퀘스트 비활성화 상태 (등록 가능)
        else
        {
            // 최대 반복 횟수에 도달
            if (!nowQuest.IsRepeatable())
            {
                Debug.Log("Already Max Repeat Quest Count Played");
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "Final Completion").Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
            }
            // 퀘스트 첫 등록 (자동 수락)
            else if (nowQuest.IsAutoFirst)
            {
                Debug.Log("Accept Quest At First");
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "First Meeting").Value;
                if (CheckInvalid(dialogueData) == true) yield break;

                dialogueData.LinkQuestData(nowQuest);

                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
            }
            // 퀘스트 두번째 등록 (수락 / 거절)
            else
            {
                Debug.Log("Accept Quest After Secend Time");
                /*
                 * 어색해서 주석 처리함
                 * 
                if (!_endingQuest.IsAcceptedBefore)
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

                // 수락 시
                if (nowQuest.IsAcceptedBefore)
                {
                    dialogueData3 = _dialogueCollection.FirstOrDefault(d => d.Key == "Acception").Value;
                    if (CheckInvalid(dialogueData3) == true) yield break;
                    DialogueController.Instance.StartDialogue(dialogueData3, false);
                    yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                }
                // 거절 시
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
            // 상호작용 종료 처리
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

    public void AcceptCallback()
    {
        QuestController.Instance.AcceptQuest(_quest);
    }
    public void RejectCallback()
    {
        QuestController.Instance.RejectQuest(_quest);
    }
}
