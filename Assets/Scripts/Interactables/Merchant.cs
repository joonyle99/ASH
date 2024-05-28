using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어에게 퀘스트 요청을 할 수 있으며, 응답이 가능한 클래스
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
            Debug.LogError("Merchant NPC의 다이얼로그에는 퀘스트 데이터가 필요합니다. 퀘스트 데이터를 추가해주세요.");
            yield break;
        }

        // 퀘스트 완료 프로세스
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
        // 퀘스트 등록 프로세스
        else
        {
            if (!_questData.IsRepeatable())
            {
                var dialogueData = _dialogueDataDictionary["Final Completion"];
                yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData));
            }
            // 처음 만난 경우 First Meeting 대화 실행과 함께, 퀘스트를 해당 다이얼로그에 링크한다. (자동 수락)
            else if (_questData.IsFirst)
            {
                var dialogueData = _dialogueDataDictionary["First Meeting"];
                dialogueData.LinkQuestData(_questData);
                yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData));
            }
            // 이미 완료한 적 있는 경우 Re-request 대화 실행과 함께, 퀘스트를 해당 다이얼로그에 링크한다. (응답 요청)
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

                // 수락 시
                if (_questData.IsAcceptedBefore)
                {
                    dialogueData2 = _dialogueDataDictionary["Acception"];
                    yield return StartCoroutine(DialogueController.Instance.StartDialogueCoroutine(dialogueData2));
                }
                // 거절 시
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
            // 상호작용 종료 처리
            ExitInteraction();
        }
    }
}
