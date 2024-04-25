using UnityEngine;

/// <summary>
/// 대화 상호작용이 가능한 Non-Player Character
/// 플레이어에게 퀘스트 요청을 할 수 있다.
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] private DialogueData _dialogueData;
    [SerializeField] private QuestData _questData;          // 유일하게 직렬화된 퀘스트 데이터 null이 될 수 없기 때문에 유효성 검사가 필요하다

    protected override void OnInteract()
    {
        // 퀘스트 데이터가 유효하다면 다이얼로그 데이터에 연결
        if (_questData.IsValidQuestData())
        {
            Debug.Log($"{gameObject.name}이(가) 소유한 퀘스트 데이터가 유효합니다. 다이얼로그에 연결합니다");
            _dialogueData.LinkQuestData(_questData);
        }

        DialogueController.Instance.StartDialogue(_dialogueData);
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
