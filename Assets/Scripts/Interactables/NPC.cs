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
    [SerializeField] private bool _autoAcceptQuest;

    // 퀘스트가 존재한다면 대화창이 모두 종료된다면 자동으로 퀘스트를 위임한다

    protected override void OnInteract()
    {
        // 상호작용이 시작된 순간 대화창을 띄운다
        DialogueController.Instance.StartDialogue(_data);
    }
    public override void UpdateInteracting()
    {
        // 상호작용 업데이트 중 다이얼로그가 비활성화 되면 상호작용을 종료한다
        if(!DialogueController.Instance.IsDialogueActive)
            ExitInteraction();
    }
}
