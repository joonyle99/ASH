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

    [Tooltip("you can link questData to dialogueData")]
    [SerializeField] private QuestData _questData;

    protected override void OnInteract()
    {
        // 퀘스트 데이터가 유효하다면 다이얼로그 데이터에 연결
        if (_questData)
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
