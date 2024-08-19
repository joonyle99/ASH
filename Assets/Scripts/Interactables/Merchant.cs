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
            Debug.LogError("Merchant NPC의 다이얼로그에는 퀘스트 데이터가 필요합니다. 퀘스트 데이터를 추가해주세요.");
            yield break;
        }

        // 퀘스트 활성화 상태 (진행 중)
        if (_questData.IsActive)
        {
            // 퀘스트 완료
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
            // 퀘스트 미완료
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
        // 퀘스트 비활성화 상태 (등록 가능)
        else
        {
            // 최대 반복 횟수에 도달
            if (!_questData.IsRepeatable())
            {
                var dialogueData = _dialogueCollection.FirstOrDefault(d => d.Key == "Final Completion").Value;
                if (CheckInvalid(dialogueData) == true) yield break;
                Debug.Log("Start Final Completion");
                DialogueController.Instance.StartDialogue(dialogueData, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                Debug.Log("End Final Completion");
            }
            // 퀘스트 첫 등록 (자동 수락)
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
            // 퀘스트 두번째 등록 (수락 / 거절)
            else
            {
                /*
                 * 어색해서 주석 처리함
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

                // 수락 시
                if (_questData.IsAcceptedBefore)
                {
                    dialogueData3 = _dialogueCollection.FirstOrDefault(d => d.Key == "Acception").Value;
                    if (CheckInvalid(dialogueData3) == true) yield break;
                    Debug.Log("Start Acception");
                    DialogueController.Instance.StartDialogue(dialogueData3, false);
                    yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);
                    Debug.Log("End Acception");
                }
                // 거절 시
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
}
