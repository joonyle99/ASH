using UnityEngine;

/// <summary>
/// 대화 상호작용이 가능한 Non-Player Character
/// 또는 퀘스트를 등록하는 역할을 한다
/// 다이얼로그 데이터와 퀘스트 데이터는 독립적으로 실행된다
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] private DialogueData _data;

    [Space]

    [SerializeField] private bool _autoAcceptQuest;
    [SerializeField] private Quest _quest;

    // 퀘스트가 존재한다면 대화창이 모두 종료된다면 자동으로 퀘스트를 위임한다

    protected override void OnInteract()
    {
        // 상호작용이 시작된 순간 대화창을 띄운다
        DialogueController.Instance.StartDialogue(_data);
    }
    public override void UpdateInteracting()
    {
        // 상호작용 업데이트 중 다이얼로그가 비활성화 되면 상호작용을 종료한다
        if (!DialogueController.Instance.IsDialogueActive)
        {
            // TODO: 퀘스트가 존재한다면 퀘스트를 등록한다

            // 응답 여부를 체크한다

            // 상호작용 종료
            ExitInteraction();
        }
    }
}
