using UnityEngine;

/// <summary>
/// 상호작용 가능한 오브젝트 중
/// 대화창을 띄우는 기능을 추가로 가진 Non-Player Character
/// </summary>
public class NPC : InteractableObject
{
    [Header("NPC")]
    [Space]

    [SerializeField] DialogueData _data;

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
