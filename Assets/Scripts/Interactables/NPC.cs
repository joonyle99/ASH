using UnityEngine;

/// <summary>
/// 대화 상호작용이 가능한 Non-Player Character
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] private DialogueData _data;
    [SerializeField] private Quest _quest;

    protected override void OnInteract()
    {
        // 대화창에 퀘스트 정보를 연결한다
        _data.LinkQuest(_quest);

        // 상호작용이 시작된 순간 대화창을 띄운다
        DialogueController.Instance.StartDialogue(_data);
    }
    public override void UpdateInteracting()
    {
        if (!DialogueController.Instance.IsDialogueActive)
        {
            // 상호작용 종료
            ExitInteraction();
        }
    }
}
